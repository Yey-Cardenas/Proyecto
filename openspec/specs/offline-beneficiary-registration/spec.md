# offline-beneficiary-registration Specification

## Purpose

Permitir el trabajo del dirigente en zonas sin conectividad confiable.

## Requirements

### Requirement: Registrar sin conexión
El sistema SHALL almacenar localmente los registros realizados sin conexión.

#### Scenario: Conectividad recuperada
- **WHEN** la PWA recupera conexión
- **THEN** sincroniza automáticamente los registros pendientes sin pérdida

### Requirement: Validar al sincronizar
El sistema SHALL validar cada registro sincronizado contra el padrón municipal vigente.

#### Scenario: Registro local duplicado
- **WHEN** un registro local coincide con un DNI existente
- **THEN** el sistema crea una alerta de duplicado y mantiene una resolución pendiente
