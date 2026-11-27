# Spec Delta

## ADDED Requirements

### Requirement: Mantener beneficiarios desde la interfaz y la API
El sistema SHALL permitir listar, registrar, editar e inactivar beneficiarios mediante la interfaz autorizada y la API REST.

#### Scenario: Consulta de beneficiarios autorizada
- **WHEN** un usuario autenticado consulta beneficiarios
- **THEN** el sistema aplica la restricción de comité correspondiente y devuelve únicamente los registros que puede administrar

#### Scenario: Edición válida
- **WHEN** un usuario autorizado modifica datos válidos de un beneficiario
- **THEN** el sistema conserva el beneficiario, aplica los cambios y registra la operación en el historial

#### Scenario: Inactivación con motivo
- **WHEN** un usuario autorizado solicita inactivar un beneficiario e informa el motivo
- **THEN** el sistema conserva el registro histórico, marca al beneficiario como inactivo y registra usuario, fecha y motivo

### Requirement: Validar datos de formulario de beneficiario
El sistema SHALL limitar a cinco los campos obligatorios del formulario de alta y SHALL exigir un DNI compuesto exactamente por ocho dígitos.

#### Scenario: DNI con formato válido
- **WHEN** el usuario registra un beneficiario cuyo DNI contiene exactamente ocho dígitos
- **THEN** el sistema acepta el valor para continuar con las validaciones del registro

#### Scenario: DNI con formato inválido
- **WHEN** el usuario registra un DNI vacío, con menos o más de ocho dígitos o con caracteres no numéricos
- **THEN** el sistema rechaza la operación y presenta el error de validación sin guardar el beneficiario

## MODIFIED Requirements

### Requirement: Registrar beneficiario
El sistema SHALL requerir el DNI y los datos necesarios para identificar y ubicar al beneficiario.

#### Scenario: Registro válido
- **WHEN** un dirigente autorizado registra un beneficiario con DNI válido y datos completos
- **THEN** el sistema busca duplicados y crea o mantiene el beneficiario según el resultado

#### Scenario: DNI activo duplicado
- **WHEN** el DNI informado ya corresponde a un beneficiario activo
- **THEN** el sistema informa el duplicado y no crea ni reactiva automáticamente otro registro

### Requirement: Inactivar beneficiario
El sistema SHALL permitir inactivar beneficiarios por fallecimiento, cambio de condición o inasistencia prolongada.

#### Scenario: Inactivación autorizada
- **WHEN** un dirigente autorizado inactiva un beneficiario e indica el motivo
- **THEN** el beneficiario deja de contar como activo y el cambio queda registrado

#### Scenario: Inactivación sin motivo
- **WHEN** un usuario intenta inactivar un beneficiario sin indicar un motivo
- **THEN** el sistema rechaza la operación y conserva el estado activo del beneficiario

### Requirement: Auditoría de cambios
El sistema SHALL registrar usuario, fecha y motivo de cada modificación relevante del padrón.

#### Scenario: Consulta de historial
- **WHEN** personal municipal autorizado consulta un beneficiario
- **THEN** puede revisar las altas, bajas y modificaciones registradas

#### Scenario: Alta, modificación o baja
- **WHEN** se completa una alta, modificación o inactivación autorizada
- **THEN** el historial conserva la operación, el usuario responsable, la fecha UTC y el motivo
