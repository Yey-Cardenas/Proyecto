using Proyecto.Modules.Notificaciones.Entities;
using Proyecto.Modules.Notificaciones.Interfaces;

namespace Proyecto.Modules.Notificaciones.Services;

public sealed class NotificacionService : INotificacionService
{
    public NotificacionPendiente CrearBorrador(int beneficiarioId, string mensaje)
    {
        if (beneficiarioId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(beneficiarioId));
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(mensaje);
        return new NotificacionPendiente(beneficiarioId, mensaje, DateTime.UtcNow);
    }
}
