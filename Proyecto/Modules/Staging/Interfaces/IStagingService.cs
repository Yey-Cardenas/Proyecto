using Proyecto.Modules.Staging.Entities;

namespace Proyecto.Modules.Staging.Interfaces;

public interface IStagingService
{
    IReadOnlyList<string> ValidarRegistro(RegistroStaging registro);
}
