using Proyecto.Modules.Notificaciones.Entities;

namespace Proyecto.Modules.Notificaciones.Interfaces;

public interface INotificacionService
{
    NotificacionPendiente CrearBorrador(int beneficiarioId, string mensaje);
}
