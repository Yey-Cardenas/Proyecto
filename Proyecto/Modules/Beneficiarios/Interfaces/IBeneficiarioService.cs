using Proyecto.Modules.Beneficiarios.Entities;

namespace Proyecto.Modules.Beneficiarios.Interfaces;

public interface IBeneficiarioService
{
    Task<bool> RegistrarAsync(
        Beneficiario beneficiario,
        string usuarioId,
        string motivo,
        CancellationToken cancellationToken = default);
}
