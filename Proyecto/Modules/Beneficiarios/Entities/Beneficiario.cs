namespace Proyecto.Modules.Beneficiarios.Entities;

public class Beneficiario
{
    public int Id { get; set; }
    public required string Dni { get; set; }
    public required string Nombres { get; set; }
    public required string Apellidos { get; set; }
    public string? Direccion { get; set; }
    public bool Activo { get; set; } = true;
    public int ComiteId { get; set; }
    public Comite Comite { get; set; } = null!;
    public ICollection<Programa> Programas { get; set; } = [];
    public ICollection<HistorialCambio> HistorialCambios { get; set; } = [];
}
