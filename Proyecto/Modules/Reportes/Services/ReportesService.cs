using Proyecto.Data.Interfaces;
using Proyecto.Modules.Beneficiarios.Entities;
using Proyecto.Modules.Reportes.Interfaces;

namespace Proyecto.Modules.Reportes.Services;

public sealed class ReportesService(IUnitOfWork unitOfWork) : IReportesService
{
    public Task<int> ContarBeneficiariosActivosAsync(
        CancellationToken cancellationToken = default) =>
        unitOfWork.Repository<Beneficiario>().CountAsync(
            beneficiario => beneficiario.Activo,
            cancellationToken);
}
