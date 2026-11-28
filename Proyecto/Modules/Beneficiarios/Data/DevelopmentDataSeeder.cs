using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Proyecto.Data;
using Proyecto.Modules.Autenticacion;
using Proyecto.Modules.Autenticacion.Entities;
using Proyecto.Modules.Beneficiarios.Entities;

namespace Proyecto.Modules.Beneficiarios.Data;

public static class DevelopmentDataSeeder
{
    public const string TotalBeneficiariosConfigurationKey = "Seeder:TotalBeneficiarios";
    public const int DefaultTotalBeneficiarios = 1000;
    private const int TamanoLote = 500;
    private static readonly string[] NombresFemeninos =
    [
        "María", "Rosa", "Juana", "Ana", "Carmen", "Luz", "Julia", "Teresa", "Elena", "Isabel",
        "Esperanza", "Francisca", "Mercedes", "Beatriz", "Patricia", "Sonia", "Gladys", "Edith", "Marisol", "Noemí"
    ];
    private static readonly string[] ApellidosAyacuchanos =
    [
        "Quispe", "Huamán", "Cárdenas", "De la Cruz", "Gutiérrez", "Palomino", "Huarcaya",
        "Huaytalla", "Ochoa", "Pacheco", "Ccorimanya", "Sulca", "Chuchón", "Yucra",
        "Pillaca", "Oré", "Llamocca", "Pariona", "Ayala", "Rojas"
    ];

    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        context.Database.SetCommandTimeout(120);
        await context.Database.MigrateAsync();

        var comiteA = await ObtenerComiteAsync(context, "Comité Central");
        var comiteB = await ObtenerComiteAsync(context, "Comité Norte");

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        await CrearUsuarioAsync(
            userManager,
            "dirigente.prueba",
            "dirigente.prueba@local.test",
            "Dirigente123!",
            IdentityRoles.DirigenteComite,
            comiteA.Id);
        await CrearUsuarioAsync(
            userManager,
            "personal.prueba",
            "personal.prueba@local.test",
            "Personal123!",
            IdentityRoles.PersonalSubGerencia,
            null);

        var programa = await context.Programas
            .OrderBy(item => item.Id)
            .FirstOrDefaultAsync();
        if (programa is null)
        {
            programa = new Programa { Nombre = "Programa de prueba" };
            context.Programas.Add(programa);
            await context.SaveChangesAsync();
        }

        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var totalBeneficiarios = configuration.GetValue(
            TotalBeneficiariosConfigurationKey,
            DefaultTotalBeneficiarios);
        if (totalBeneficiarios < 0)
        {
            throw new InvalidOperationException(
                $"La configuración '{TotalBeneficiariosConfigurationKey}' no puede ser negativa.");
        }

                var activos = await context.Beneficiarios.CountAsync(item => item.Activo);
        var faltantes = totalBeneficiarios - activos;
        if (faltantes > 0)
        {
            var dnisExistentes = await context.Beneficiarios
                .Select(item => item.Dni)
                .ToHashSetAsync();
            var lote = new List<Beneficiario>();
            var consecutivo = 1;
            var insertados = 0;
            var programaId = programa.Id;

            while (insertados < faltantes)
            {
                var dni = (10000000 + consecutivo).ToString();
                consecutivo++;
                if (!dnisExistentes.Add(dni))
                {
                    continue;
                }

                var comiteId = insertados % 2 == 0 ? comiteA.Id : comiteB.Id;
                lote.Add(new Beneficiario
                {
                    Dni = dni,
                    Nombres = NombresFemeninos[insertados % NombresFemeninos.Length],
                    Apellidos = ApellidosAyacuchanos[
                        (insertados / NombresFemeninos.Length) % ApellidosAyacuchanos.Length],
                    Direccion = "Dirección ficticia",
                    ComiteId = comiteId,
                    Activo = true,
                    Programas = [programa]
                });
                insertados++;

                if (lote.Count == TamanoLote || insertados == faltantes)
                {
                    context.Beneficiarios.AddRange(lote);
                    await context.SaveChangesAsync();
                    context.ChangeTracker.Clear();
                    programa = await context.Programas.SingleAsync(
                        item => item.Id == programaId);
                    lote.Clear();
                }
            }
        }
    }

    private static async Task<Comite> ObtenerComiteAsync(AppDbContext context, string nombre)
    {
        var comite = await context.Comites.SingleOrDefaultAsync(item => item.Nombre == nombre);
        if (comite is null)
        {
            comite = new Comite { Nombre = nombre };
            context.Comites.Add(comite);
            await context.SaveChangesAsync();
        }

        return comite;
    }

    private static async Task CrearUsuarioAsync(
        UserManager<ApplicationUser> userManager,
        string userName,
        string email,
        string password,
        string role,
        int? comiteId)
    {
        var usuario = await userManager.FindByEmailAsync(email);
        usuario ??= await userManager.FindByNameAsync(userName);
        if (usuario is null)
        {
            usuario = new ApplicationUser
            {
                UserName = userName,
                Email = email,
                EmailConfirmed = true,
                ComiteId = comiteId
            };
            var result = await userManager.CreateAsync(usuario, password);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException(
                    $"No se pudo crear el usuario '{userName}': " +
                    string.Join("; ", result.Errors.Select(error => error.Description)));
            }
        }
        else
        {
            var debeActualizar = usuario.ComiteId != comiteId
                || !usuario.EmailConfirmed;
            if (debeActualizar)
            {
                usuario.ComiteId = comiteId;
                usuario.EmailConfirmed = true;
                var updateResult = await userManager.UpdateAsync(usuario);
                if (!updateResult.Succeeded)
                {
                    throw new InvalidOperationException(
                        $"No se pudo actualizar el usuario '{email}': " +
                        string.Join("; ", updateResult.Errors.Select(error => error.Description)));
                }
            }
        }

        if (!await userManager.IsInRoleAsync(usuario, role))
        {
            await userManager.AddToRoleAsync(usuario, role);
        }
    }
}
