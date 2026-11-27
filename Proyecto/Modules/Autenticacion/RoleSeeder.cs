using Microsoft.AspNetCore.Identity;

namespace Proyecto.Modules.Autenticacion;

public static class RoleSeeder
{
    public static async Task EnsureRolesAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        foreach (var roleName in new[]
                 {
                     IdentityRoles.DirigenteComite,
                     IdentityRoles.PersonalSubGerencia
                 })
        {
            if (await roleManager.RoleExistsAsync(roleName))
            {
                continue;
            }

            var result = await roleManager.CreateAsync(new IdentityRole(roleName));
            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(error => error.Description));
                throw new InvalidOperationException(
                    $"No se pudo crear el rol '{roleName}': {errors}");
            }
        }
    }
}
