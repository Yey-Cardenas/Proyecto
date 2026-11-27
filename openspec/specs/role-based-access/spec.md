# role-based-access Specification

## Purpose

Restringir funciones e información según credenciales, rol y comité.

## Requirements

### Requirement: Iniciar sesión
El sistema SHALL autenticar a dirigentes y personal municipal mediante credenciales.

#### Scenario: Credenciales válidas
- **WHEN** un usuario inicia sesión con credenciales válidas
- **THEN** el sistema habilita únicamente las funciones autorizadas para su rol

### Requirement: Administrar usuarios
El sistema SHALL permitir al personal municipal registrar, actualizar y desactivar usuarios.

#### Scenario: Asignación de rol
- **WHEN** personal autorizado crea o actualiza un usuario
- **THEN** puede asignar su rol y comité y el cambio se aplica inmediatamente
