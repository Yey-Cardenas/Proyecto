# coverage-reporting Specification

## Purpose

Supervisar la cobertura activa y facilitar la distribución sustentada de recursos.

## Requirements

### Requirement: Mostrar cobertura geográfica
El sistema SHALL mostrar beneficiarios activos por zona o distrito.

#### Scenario: Consulta de cobertura
- **WHEN** personal municipal autorizado consulta el mapa
- **THEN** visualiza cantidades y zonas sobre-atendidas o desatendidas

### Requirement: Exportar información
El sistema SHALL permitir exportar el padrón depurado y reportes en Excel o PDF.

#### Scenario: Exportación autorizada
- **WHEN** personal municipal solicita una exportación
- **THEN** el archivo contiene los datos activos permitidos por los permisos del usuario
