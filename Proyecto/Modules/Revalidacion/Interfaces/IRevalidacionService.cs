using Proyecto.Modules.Revalidacion.Entities;

namespace Proyecto.Modules.Revalidacion.Interfaces;

public interface IRevalidacionService
{
    Task<IReadOnlyList<RevalidacionBeneficiario>> ObtenerPendientesAsync(
        CancellationToken cancellationToken = default);
}
