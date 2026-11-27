namespace Proyecto.Modules.Duplicados.Interfaces;

public interface IDuplicateDetectionService
{
    Task<bool> DniActivoExisteAsync(string dni, CancellationToken cancellationToken = default);
}
