using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Proyecto.Data;
using Proyecto.Data.Interfaces;
using Proyecto.Modules.Autenticacion;
using Proyecto.Modules.Autenticacion.Entities;
using Proyecto.Modules.Beneficiarios.Entities;
using Proyecto.Modules.Beneficiarios.Services;
using Proyecto.Modules.Duplicados.Services;
using Proyecto.Modules.Notificaciones.Services;
using Proyecto.Modules.Revalidacion.Entities;
using Proyecto.Modules.Revalidacion.Services;
using Proyecto.Modules.Reportes.Services;
using Proyecto.Modules.Staging.Entities;
using Proyecto.Modules.Staging.Services;
using Proyecto.Shared.Auditoria;
using Xunit;

namespace Proyecto.Tests;

public sealed class DomainServicesTests : IAsyncLifetime
{
    private readonly string _connectionString =
        $"Server=(localdb)\\MSSQLLocalDB;Database=ProyectoTests_{Guid.NewGuid():N};Trusted_Connection=True;TrustServerCertificate=True";
    private AppDbContext _context = null!;

    public async Task InitializeAsync()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(_connectionString)
            .Options;
        _context = new AppDbContext(options);
        await _context.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await _context.Database.EnsureDeletedAsync();
        await _context.DisposeAsync();
    }

    [Fact]
    public async Task UnitOfWork_CommitsRepositoryChangesInTransaction()
    {
        var unitOfWork = new UnitOfWork(_context);
        await using var transaction = await unitOfWork.BeginTransactionAsync();

        await unitOfWork.Repository<Comite>().AddAsync(new Comite { Nombre = "Comite A" });
        await unitOfWork.SaveChangesAsync();
        await transaction.CommitAsync();

        Assert.True(await _context.Comites.AnyAsync(comite => comite.Nombre == "Comite A"));
    }

    [Fact]
    public async Task BeneficiarioIndex_AllowsInactiveDniButRejectsSecondActiveRecord()
    {
        var comite = await CrearComiteAsync();
        _context.Beneficiarios.Add(CrearBeneficiario(comite.Id, "12345678"));
        await _context.SaveChangesAsync();

        _context.Beneficiarios.Add(CrearBeneficiario(comite.Id, "12345678"));
        await Assert.ThrowsAsync<DbUpdateException>(() => _context.SaveChangesAsync());
        _context.ChangeTracker.Clear();

        _context.Beneficiarios.Add(CrearBeneficiario(comite.Id, "12345678", activo: false));
        await _context.SaveChangesAsync();
    }

    [Fact]
    public async Task AuditoriaService_PersistsUserUtcTimestampAndReason()
    {
        var comite = await CrearComiteAsync();
        var beneficiario = CrearBeneficiario(comite.Id, "23456789");
        _context.Beneficiarios.Add(beneficiario);
        await _context.SaveChangesAsync();
        var desde = DateTimeOffset.UtcNow;

        await new AuditoriaService(_context).RegistrarCambioAsync(
            new AuditEntry("usuario-1", beneficiario.Id, "Actualizacion", "Cambio de domicilio"));

        var cambio = await _context.HistorialCambios.SingleAsync();
        Assert.Equal("usuario-1", cambio.UsuarioId);
        Assert.Equal("Cambio de domicilio", cambio.Motivo);
        Assert.Equal(TimeSpan.Zero, cambio.FechaUtc.Offset);
        Assert.InRange(cambio.FechaUtc, desde, DateTimeOffset.UtcNow);
    }

    [Fact]
    public async Task BeneficiarioService_RejectsActiveDuplicateAndAuditsSuccessfulRegistration()
    {
        var comite = await CrearComiteAsync();
        var service = new BeneficiarioService(
            new UnitOfWork(_context),
            new AuditoriaService(_context));

        var registrado = await service.RegistrarAsync(
            CrearBeneficiario(comite.Id, "34567890"),
            "usuario-1",
            "Alta inicial");
        var duplicado = await service.RegistrarAsync(
            CrearBeneficiario(comite.Id, "34567890"),
            "usuario-1",
            "Reintento");

        Assert.True(registrado);
        Assert.False(duplicado);
        Assert.Single(await _context.HistorialCambios.ToListAsync());
    }

    [Fact]
    public async Task DuplicateAndReportServicesUseActiveBeneficiaries()
    {
        var comite = await CrearComiteAsync();
        _context.Beneficiarios.Add(CrearBeneficiario(comite.Id, "45678901"));
        _context.Beneficiarios.Add(CrearBeneficiario(comite.Id, "56789012", activo: false));
        await _context.SaveChangesAsync();

        var unitOfWork = new UnitOfWork(_context);
        var duplicateService = new DuplicateDetectionService(unitOfWork);
        var reportService = new ReportesService(unitOfWork);

        Assert.True(await duplicateService.DniActivoExisteAsync("45678901"));
        Assert.False(await duplicateService.DniActivoExisteAsync("56789012"));
        Assert.Equal(1, await reportService.ContarBeneficiariosActivosAsync());
    }

    [Fact]
    public async Task RevalidationService_ReturnsOnlyPendingRecords()
    {
        var comite = await CrearComiteAsync();
        var beneficiario = CrearBeneficiario(comite.Id, "67890123");
        _context.Beneficiarios.Add(beneficiario);
        await _context.SaveChangesAsync();
        _context.Revalidaciones.AddRange(
            new RevalidacionBeneficiario
            {
                BeneficiarioId = beneficiario.Id,
                FechaProgramadaUtc = DateTime.UtcNow
            },
            new RevalidacionBeneficiario
            {
                BeneficiarioId = beneficiario.Id,
                FechaProgramadaUtc = DateTime.UtcNow,
                Estado = EstadoRevalidacion.Completa
            });
        await _context.SaveChangesAsync();

        var pendientes = await new RevalidacionService(new UnitOfWork(_context))
            .ObtenerPendientesAsync();

        Assert.Single(pendientes);
        Assert.Equal(EstadoRevalidacion.Pendiente, pendientes[0].Estado);
    }

    [Fact]
    public void NotificationAndStagingServicesCreateDraftAndReportInvalidRows()
    {
        var borrador = new NotificacionService().CrearBorrador(1, "Revalidacion requerida");
        var errores = new StagingService().ValidarRegistro(
            new RegistroStaging("123", "", ""));

        Assert.Equal(1, borrador.BeneficiarioId);
        Assert.Equal(TimeSpan.Zero, new DateTimeOffset(borrador.CreadaUtc).Offset);
        Assert.Equal(3, errores.Count);
    }

    [Fact]
    public async Task CommitteeAuthorizationRestrictsLeadersAndAllowsMunicipalStaff()
    {
        var comiteA = await CrearComiteAsync();
        var comiteB = await CrearComiteAsync();
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddDbContext<AppDbContext>(options => options.UseSqlServer(_connectionString));
        services.AddIdentityCore<ApplicationUser>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<AppDbContext>();
        services.AddAuthorization(options =>
            options.AddPolicy("ComiteAccess", policy =>
                policy.RequireAuthenticatedUser().AddRequirements(new ComiteAccessRequirement())));
        services.AddScoped<IAuthorizationHandler, ComiteAccessHandler>();
        await using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();

        await RoleSeeder.EnsureRolesAsync(provider);
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        Assert.True(await roleManager.RoleExistsAsync(IdentityRoles.DirigenteComite));
        Assert.True(await roleManager.RoleExistsAsync(IdentityRoles.PersonalSubGerencia));

        var dirigente = new ApplicationUser
        {
            UserName = "dirigente",
            ComiteId = comiteA.Id
        };
        Assert.True((await userManager.CreateAsync(dirigente, "SecurePass!234")).Succeeded);
        Assert.True((await userManager.AddToRoleAsync(dirigente, IdentityRoles.DirigenteComite)).Succeeded);

        var personal = new ApplicationUser { UserName = "personal" };
        Assert.True((await userManager.CreateAsync(personal, "SecurePass!234")).Succeeded);
        Assert.True((await userManager.AddToRoleAsync(personal, IdentityRoles.PersonalSubGerencia)).Succeeded);

        var authorization = scope.ServiceProvider.GetRequiredService<IAuthorizationService>();
        var dirigentePrincipal = CrearPrincipal(dirigente.Id);
        var personalPrincipal = CrearPrincipal(personal.Id);

        Assert.True((await authorization.AuthorizeAsync(
            dirigentePrincipal, comiteA.Id, "ComiteAccess")).Succeeded);
        Assert.False((await authorization.AuthorizeAsync(
            dirigentePrincipal, comiteB.Id, "ComiteAccess")).Succeeded);
        Assert.True((await authorization.AuthorizeAsync(
            personalPrincipal, comiteB.Id, "ComiteAccess")).Succeeded);
    }

    private async Task<Comite> CrearComiteAsync()
    {
        var comite = new Comite { Nombre = $"Comite-{Guid.NewGuid():N}" };
        _context.Comites.Add(comite);
        await _context.SaveChangesAsync();
        return comite;
    }

    private static Beneficiario CrearBeneficiario(int comiteId, string dni, bool activo = true) =>
        new()
        {
            Dni = dni,
            Nombres = "Nombre",
            Apellidos = "Apellido",
            ComiteId = comiteId,
            Activo = activo
        };

    private static ClaimsPrincipal CrearPrincipal(string usuarioId) =>
        new(new ClaimsIdentity(
            [new Claim(ClaimTypes.NameIdentifier, usuarioId)],
            "Test"));
}
