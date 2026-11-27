# Spec Delta

## MODIFIED Requirements

### Requirement: Registrar beneficiario
El sistema SHALL requerir el DNI y los datos necesarios para identificar y ubicar al beneficiario y SHALL impedir que el mismo DNI tenga más de una inscripción activa.

#### Scenario: Registro válido
- **WHEN** un dirigente autorizado registra un DNI que no tiene otra inscripción activa
- **THEN** el sistema crea el beneficiario y mantiene la unicidad definida por la regla RN-02

#### Scenario: DNI ya activo
- **WHEN** un dirigente autorizado intenta registrar un DNI con inscripción activa
- **THEN** el sistema rechaza el registro automático y deja el caso disponible para la revisión de duplicados

### Requirement: Auditoría de cambios
El sistema SHALL persistir usuario, fecha y motivo para cada modificación relevante del padrón.

#### Scenario: Alta o modificación registrada
- **WHEN** un usuario autorizado crea o modifica un beneficiario
- **THEN** el sistema conserva una entrada de HistorialCambio con usuario responsable, fecha y motivo

#### Scenario: Consulta de historial
- **WHEN** personal municipal autorizado consulta un beneficiario
- **THEN** puede revisar las altas, bajas y modificaciones registradas
