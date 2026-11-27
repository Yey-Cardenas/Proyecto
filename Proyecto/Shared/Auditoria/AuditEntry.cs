namespace Proyecto.Shared.Auditoria;

public sealed record AuditEntry(
    string UsuarioId,
    int BeneficiarioId,
    string Operacion,
    string Motivo);
