using Proyecto.Modules.Beneficiarios.Entities;

namespace Proyecto.Modules.Beneficiarios.Interfaces;

public interface IBeneficiarioService
{
    Task<List<Beneficiario>> ListarAsync(
        int? comiteId,
        CancellationToken cancellationToken = default);
    Task<Beneficiario?> ObtenerAsync(
        int id,
        int? comiteId,
        CancellationToken cancellationToken = default);
    Task<Beneficiario?> ObtenerPorDniAsync(
        string dni,
        int? comiteId,
        CancellationToken cancellationToken = default);
    Task<bool> RegistrarAsync(
        Beneficiario beneficiario,
        string usuarioId,
        string motivo,
        CancellationToken cancellationToken = default);
    Task<bool> ActualizarAsync(
        int id,
        Beneficiario datos,
        string usuarioId,
        string motivo,
        int? comiteId,
        CancellationToken cancellationToken = default);
    Task<bool> InactivarAsync(
        int id,
        string usuarioId,
        string motivo,
        int? comiteId,
        CancellationToken cancellationToken = default);
}
