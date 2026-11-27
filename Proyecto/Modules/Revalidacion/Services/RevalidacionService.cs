using Proyecto.Data.Interfaces;
using Proyecto.Modules.Revalidacion.Entities;
using Proyecto.Modules.Revalidacion.Interfaces;

namespace Proyecto.Modules.Revalidacion.Services;

public sealed class RevalidacionService(IUnitOfWork unitOfWork) : IRevalidacionService
{
    public async Task<IReadOnlyList<RevalidacionBeneficiario>> ObtenerPendientesAsync(
        CancellationToken cancellationToken = default) =>
        await unitOfWork.Repository<RevalidacionBeneficiario>().ListAsync(
            revalidacion => revalidacion.Estado == EstadoRevalidacion.Pendiente,
            cancellationToken);
}
