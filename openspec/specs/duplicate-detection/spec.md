# duplicate-detection Specification

## Purpose

Evitar inscripciones activas duplicadas y dar al personal municipal un flujo de revisión.

## Requirements

### Requirement: Detectar DNI duplicado
El sistema SHALL buscar el DNI en todo el padrón municipal antes de aceptar una inscripción.

#### Scenario: DNI registrado en otro comité
- **WHEN** un dirigente intenta registrar un DNI existente en otro comité
- **THEN** el sistema bloquea la alta automática y crea una alerta para evaluación

### Requirement: Evaluar duplicados
El sistema SHALL permitir al personal municipal revisar y resolver alertas de duplicados.

#### Scenario: Resolución registrada
- **WHEN** personal autorizado resuelve una alerta
- **THEN** el resultado y su motivo quedan incluidos en el historial
