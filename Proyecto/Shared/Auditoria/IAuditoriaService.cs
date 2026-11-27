namespace Proyecto.Shared.Auditoria;

public interface IAuditoriaService
{
    Task RegistrarCambioAsync(AuditEntry entrada, CancellationToken cancellationToken = default);
}
