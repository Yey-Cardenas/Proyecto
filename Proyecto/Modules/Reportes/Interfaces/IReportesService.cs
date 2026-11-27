namespace Proyecto.Modules.Reportes.Interfaces;

public interface IReportesService
{
    Task<int> ContarBeneficiariosActivosAsync(CancellationToken cancellationToken = default);
}
