using System.Text.RegularExpressions;
using Proyecto.Modules.Staging.Entities;
using Proyecto.Modules.Staging.Interfaces;

namespace Proyecto.Modules.Staging.Services;

public sealed partial class StagingService : IStagingService
{
    public IReadOnlyList<string> ValidarRegistro(RegistroStaging registro)
    {
        ArgumentNullException.ThrowIfNull(registro);
        var errores = new List<string>();

        if (!DniValido().IsMatch(registro.Dni))
        {
            errores.Add("El DNI debe contener exactamente ocho digitos.");
        }

        if (string.IsNullOrWhiteSpace(registro.Nombres))
        {
            errores.Add("Los nombres son obligatorios.");
        }

        if (string.IsNullOrWhiteSpace(registro.Apellidos))
        {
            errores.Add("Los apellidos son obligatorios.");
        }

        return errores;
    }

    [GeneratedRegex(@"^\d{8}$", RegexOptions.CultureInvariant)]
    private static partial Regex DniValido();
}
