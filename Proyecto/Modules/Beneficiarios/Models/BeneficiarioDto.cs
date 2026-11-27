using System.ComponentModel.DataAnnotations;

namespace Proyecto.Modules.Beneficiarios.Models;

public sealed class BeneficiarioDto
{
    public int Id { get; set; }

    [Required]
    [RegularExpression(@"^\d{8}$", ErrorMessage = "El DNI debe contener exactamente 8 dígitos.")]
    public string Dni { get; set; } = string.Empty;

    [Required]
    [MaxLength(150)]
    public string Nombres { get; set; } = string.Empty;

    [Required]
    [MaxLength(150)]
    public string Apellidos { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Direccion { get; set; }

    [Range(1, int.MaxValue)]
    public int ComiteId { get; set; }

    [Range(1, int.MaxValue)]
    public int? ProgramaId { get; set; }

    public bool Activo { get; set; } = true;
}

public sealed class BeneficiarioInactivarDto
{
    [Required]
    [MaxLength(500)]
    public string Motivo { get; set; } = string.Empty;
}
