using Proyecto.Data.Interfaces;
using Proyecto.Modules.Beneficiarios.Entities;
using Proyecto.Modules.Duplicados.Interfaces;

namespace Proyecto.Modules.Duplicados.Services;

public sealed class DuplicateDetectionService(IUnitOfWork unitOfWork) : IDuplicateDetectionService
{
    public Task<bool> DniActivoExisteAsync(
        string dni,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(dni);
        return unitOfWork.Repository<Beneficiario>().AnyAsync(
            beneficiario => beneficiario.Dni == dni && beneficiario.Activo,
            cancellationToken);
    }
}
