using System.Data;
using Proyecto.Data.Interfaces;
using Proyecto.Modules.Beneficiarios.Entities;
using Proyecto.Modules.Beneficiarios.Interfaces;
using Proyecto.Shared.Auditoria;

namespace Proyecto.Modules.Beneficiarios.Services;

public sealed class BeneficiarioService(
    IUnitOfWork unitOfWork,
    IAuditoriaService auditoriaService) : IBeneficiarioService
{
    public async Task<bool> RegistrarAsync(
        Beneficiario beneficiario,
        string usuarioId,
        string motivo,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(beneficiario.Dni);
        ArgumentException.ThrowIfNullOrWhiteSpace(usuarioId);
        ArgumentException.ThrowIfNullOrWhiteSpace(motivo);

        await using var transaction = await unitOfWork.BeginTransactionAsync(
            cancellationToken,
            IsolationLevel.Serializable);

        var repository = unitOfWork.Repository<Beneficiario>();
        if (await repository.AnyAsync(
                existente => existente.Dni == beneficiario.Dni && existente.Activo,
                cancellationToken))
        {
            return false;
        }

        await repository.AddAsync(beneficiario, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await auditoriaService.RegistrarCambioAsync(
            new AuditEntry(usuarioId, beneficiario.Id, "Alta", motivo),
            cancellationToken);

        await transaction.CommitAsync(cancellationToken);
        return true;
    }
}
