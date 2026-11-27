using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Proyecto.Modules.Autenticacion.Entities;

namespace Proyecto.Modules.Autenticacion;

public sealed class ComiteAccessHandler(
    UserManager<ApplicationUser> userManager) : AuthorizationHandler<ComiteAccessRequirement, int>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        ComiteAccessRequirement requirement,
        int comiteId)
    {
        var usuario = await userManager.GetUserAsync(context.User);
        if (usuario is null)
        {
            return;
        }

        if (await userManager.IsInRoleAsync(usuario, IdentityRoles.PersonalSubGerencia)
            || (await userManager.IsInRoleAsync(usuario, IdentityRoles.DirigenteComite)
                && usuario.ComiteId == comiteId))
        {
            context.Succeed(requirement);
        }
    }
}
