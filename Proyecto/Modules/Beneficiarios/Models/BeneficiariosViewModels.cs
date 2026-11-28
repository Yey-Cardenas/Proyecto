using Microsoft.AspNetCore.Mvc.Rendering;
using Proyecto.Modules.Beneficiarios.Entities;

namespace Proyecto.Modules.Beneficiarios.Models;

public sealed class BeneficiariosIndexViewModel
{
    public required IReadOnlyList<Beneficiario> Beneficiarios { get; init; }
    public string? Dni { get; init; }
    public int Pagina { get; init; }
    public int TotalPaginas { get; init; }
    public int Total { get; init; }
}

public sealed class BeneficiarioFormViewModel
{
    public BeneficiarioDto Beneficiario { get; set; } = new();
    public IReadOnlyList<SelectListItem> Comites { get; set; } = [];
}

public sealed class HistorialBeneficiarioViewModel
{
    public required Beneficiario Beneficiario { get; init; }
    public required IReadOnlyList<HistorialCambioViewModel> Cambios { get; init; }
}

public sealed class HistorialCambioViewModel
{
    public required string Usuario { get; init; }
    public DateTimeOffset Fecha { get; init; }
    public required string Operacion { get; init; }
    public required string Motivo { get; init; }
}
