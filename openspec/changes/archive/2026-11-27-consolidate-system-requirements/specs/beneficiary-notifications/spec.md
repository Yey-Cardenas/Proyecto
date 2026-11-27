# beneficiary-notifications

## Purpose

Mantener informados a los beneficiarios mediante un canal externo de mensajería.

## ADDED Requirements

### Requirement: Notificar eventos
El sistema SHALL solicitar el envío de notificaciones SMS/WhatsApp para aprobaciones y revalidaciones.

#### Scenario: Inscripción aprobada
- **WHEN** una inscripción recibe aprobación
- **THEN** el sistema solicita la notificación al gateway configurado

### Requirement: Registrar fallos externos
El sistema SHALL registrar errores o respuestas negativas del gateway sin afectar las operaciones principales.

#### Scenario: Gateway no disponible
- **WHEN** el gateway no puede entregar una notificación
- **THEN** el sistema registra el fallo y deja disponible un reintento posterior
