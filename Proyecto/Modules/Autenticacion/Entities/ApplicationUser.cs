using Microsoft.AspNetCore.Identity;
using Proyecto.Modules.Beneficiarios.Entities;

namespace Proyecto.Modules.Autenticacion.Entities;

public class ApplicationUser : IdentityUser
{
    public int? ComiteId { get; set; }
    public Comite? Comite { get; set; }
}
