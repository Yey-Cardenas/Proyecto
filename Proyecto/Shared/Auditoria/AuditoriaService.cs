using Proyecto.Data;
using Proyecto.Modules.Beneficiarios.Entities;

namespace Proyecto.Shared.Auditoria;

public sealed class AuditoriaService(AppDbContext context) : IAuditoriaService
{
    public async Task RegistrarCambioAsync(
        AuditEntry entrada,
        CancellationToken cancellationToken = default)
    {
        var cambio = new HistorialCambio
        {
            BeneficiarioId = entrada.BeneficiarioId,
            UsuarioId = entrada.UsuarioId,
            FechaUtc = DateTimeOffset.UtcNow,
            Operacion = entrada.Operacion,
            Motivo = entrada.Motivo
        };

        await context.HistorialCambios.AddAsync(cambio, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }
}
