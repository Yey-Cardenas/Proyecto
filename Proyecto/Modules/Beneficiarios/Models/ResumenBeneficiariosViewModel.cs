namespace Proyecto.Modules.Beneficiarios.Models;

public sealed class ResumenBeneficiariosViewModel
{
    public int TotalActivos { get; init; }
    public required IReadOnlyList<ResumenComiteViewModel> PorComite { get; init; }
}

public sealed class ResumenComiteViewModel
{
    public required string Comite { get; init; }
    public int Total { get; init; }
}
