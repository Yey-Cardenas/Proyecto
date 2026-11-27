# Spec Delta

## MODIFIED Requirements

### Requirement: Detectar DNI duplicado
El sistema SHALL buscar el DNI en todo el padrón municipal antes de aceptar una inscripción.

#### Scenario: DNI registrado en otro comité
- **WHEN** un dirigente intenta registrar un DNI existente en otro comité
- **THEN** el sistema bloquea la alta automática y crea una alerta para evaluación

#### Scenario: DNI activo en el mismo comité
- **WHEN** un usuario intenta registrar un DNI que ya tiene una inscripción activa
- **THEN** el sistema muestra un aviso y no registra una nueva inscripción activa

#### Scenario: DNI corresponde a un registro inactivo
- **WHEN** el DNI existe únicamente en registros inactivos
- **THEN** el sistema puede aceptar el alta sin contar una inscripción activa duplicada

### Requirement: Evaluar duplicados
El sistema SHALL permitir al personal municipal revisar y resolver alertas de duplicados.

#### Scenario: Resolución registrada
- **WHEN** personal autorizado resuelve una alerta
- **THEN** el resultado y su motivo quedan incluidos en el historial
