namespace Proyecto.Modules.Beneficiarios.Entities;

public class HistorialCambio
{
    public int Id { get; set; }
    public int BeneficiarioId { get; set; }
    public Beneficiario Beneficiario { get; set; } = null!;
    public string? UsuarioId { get; set; }
    public DateTimeOffset FechaUtc { get; set; }
    public required string Operacion { get; set; }
    public required string Motivo { get; set; }
}
