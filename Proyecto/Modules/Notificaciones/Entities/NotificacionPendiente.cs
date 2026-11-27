namespace Proyecto.Modules.Notificaciones.Entities;

public sealed record NotificacionPendiente(
    int BeneficiarioId,
    string Mensaje,
    DateTime CreadaUtc);
