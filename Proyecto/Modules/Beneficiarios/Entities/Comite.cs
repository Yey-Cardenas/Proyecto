using Proyecto.Modules.Autenticacion.Entities;

namespace Proyecto.Modules.Beneficiarios.Entities;

public class Comite
{
    public int Id { get; set; }
    public required string Nombre { get; set; }
    public ICollection<Beneficiario> Beneficiarios { get; set; } = [];
    public ICollection<ApplicationUser> Dirigentes { get; set; } = [];
}
