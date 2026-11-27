# beneficiary-management Specification

## Purpose

Permitir registrar y mantener un padrón municipal único y vigente de beneficiarios.

## Requirements

### Requirement: Registrar beneficiario
El sistema SHALL requerir el DNI y los datos necesarios para identificar y ubicar al beneficiario.

#### Scenario: Registro válido
- **WHEN** un dirigente autorizado registra un beneficiario con DNI válido y datos completos
- **THEN** el sistema busca duplicados y crea o mantiene el beneficiario según el resultado

### Requirement: Inactivar beneficiario
El sistema SHALL permitir inactivar beneficiarios por fallecimiento, cambio de condición o inasistencia prolongada.

#### Scenario: Inactivación autorizada
- **WHEN** un dirigente autorizado inactiva un beneficiario e indica el motivo
- **THEN** el beneficiario deja de contar como activo y el cambio queda registrado

### Requirement: Auditoría de cambios
El sistema SHALL registrar usuario, fecha y motivo de cada modificación relevante del padrón.

#### Scenario: Consulta de historial
- **WHEN** personal municipal autorizado consulta un beneficiario
- **THEN** puede revisar las altas, bajas y modificaciones registradas
