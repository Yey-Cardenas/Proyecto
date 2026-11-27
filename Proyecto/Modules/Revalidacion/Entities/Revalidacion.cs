using Proyecto.Modules.Beneficiarios.Entities;

namespace Proyecto.Modules.Revalidacion.Entities;

public class RevalidacionBeneficiario
{
    public int Id { get; set; }
    public int BeneficiarioId { get; set; }
    public Beneficiario Beneficiario { get; set; } = null!;
    public DateTime FechaProgramadaUtc { get; set; }
    public DateTime? FechaResultadoUtc { get; set; }
    public EstadoRevalidacion Estado { get; set; } = EstadoRevalidacion.Pendiente;
}
