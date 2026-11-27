using Microsoft.Extensions.Caching.Memory;
using Proyecto.Data.Interfaces;
using Proyecto.Modules.Beneficiarios.Entities;
using Proyecto.Modules.Duplicados.Interfaces;

namespace Proyecto.Modules.Duplicados.Services;

public sealed class DuplicateDetectionService(
    IUnitOfWork unitOfWork,
    IMemoryCache? memoryCache = null) : IDuplicateDetectionService
{
    private const string CachePrefix = "beneficiario:dni:";

    public async Task<bool> DniActivoExisteAsync(
        string dni,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(dni);
        if (dni.Length != 8 || !dni.All(char.IsAsciiDigit))
        {
            throw new ArgumentException("El DNI debe contener exactamente 8 digitos.", nameof(dni));
        }

        var cacheKey = CachePrefix + dni;
        if (memoryCache is not null
            && memoryCache.TryGetValue(cacheKey, out bool existe))
        {
            return existe;
        }

        var existeActivo = await unitOfWork.Repository<Beneficiario>().AnyAsync(
            beneficiario => beneficiario.Dni == dni && beneficiario.Activo,
            cancellationToken);

        memoryCache?.Set(cacheKey, existeActivo, TimeSpan.FromSeconds(30));
        return existeActivo;
    }
}
