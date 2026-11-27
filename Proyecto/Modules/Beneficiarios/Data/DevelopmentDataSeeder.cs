using System.Globalization;
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
    private const long MaximoIndiceDni = 89_999_999;

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

        var totalExistente = await context.Beneficiarios.CountAsync();
        if (totalExistente < totalBeneficiarios)
        {
            var dnisExistentes = await context.Beneficiarios
                .Select(item => item.Dni)
                .ToHashSetAsync();
            var totalActual = totalExistente;
            long siguienteIndiceDni = totalExistente;
            var programaId = programa.Id;

            while (totalActual < totalBeneficiarios)
            {
                var cantidadLote = Math.Min(TamanoLote, totalBeneficiarios - totalActual);
                var lote = new List<Beneficiario>(cantidadLote);
                while (lote.Count < cantidadLote)
                {
                    if (siguienteIndiceDni > MaximoIndiceDni)
                    {
                        throw new InvalidOperationException(
                            "No quedan DNI de prueba disponibles para completar el seeder.");
                    }

                    var indice = siguienteIndiceDni++;
                    var dni = (10_000_000L + indice).ToString(CultureInfo.InvariantCulture);
                    if (!dnisExistentes.Add(dni))
                    {
                        continue;
                    }

                    var comiteId = (totalActual + lote.Count) % 2 == 0
                        ? comiteA.Id
                        : comiteB.Id;
                    lote.Add(new Beneficiario
                    {
                        Dni = dni,
                        Nombres = $"Nombres{indice}",
                        Apellidos = $"Apellidos{indice}",
                        Direccion = "Dirección ficticia",
                        ComiteId = comiteId,
                        Activo = true,
                        Programas = [programa]
                    });
                }

                context.Beneficiarios.AddRange(lote);
                await context.SaveChangesAsync();
                context.ChangeTracker.Clear();

                totalActual = await context.Beneficiarios.CountAsync();
                if (totalActual > totalBeneficiarios)
                {
                    throw new InvalidOperationException(
                        $"El seeder creó {totalActual} beneficiarios, superando el objetivo " +
                        $"configurado de {totalBeneficiarios}.");
                }

                if (totalActual < totalBeneficiarios)
                {
                    programa = await context.Programas.SingleAsync(item => item.Id == programaId);
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
