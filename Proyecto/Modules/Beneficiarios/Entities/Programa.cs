namespace Proyecto.Modules.Beneficiarios.Entities;

public class Programa
{
    public int Id { get; set; }
    public required string Nombre { get; set; }
    public ICollection<Beneficiario> Beneficiarios { get; set; } = [];
}
