# Spec Delta

## ADDED Requirements

### Requirement: Restringir beneficiarios por comité
El sistema SHALL limitar las operaciones sobre beneficiarios del rol `DirigenteComite` al comité asociado con su cuenta y SHALL permitir al rol `PersonalSubGerencia` operar sobre todos los comités.

#### Scenario: Dirigente consulta su comité
- **WHEN** un usuario con rol `DirigenteComite` lista o consulta beneficiarios
- **THEN** el sistema devuelve exclusivamente beneficiarios de su comité

#### Scenario: Dirigente intenta modificar otro comité
- **WHEN** un usuario con rol `DirigenteComite` intenta consultar o modificar un beneficiario de otro comité
- **THEN** el sistema rechaza la operación

#### Scenario: Personal municipal consulta todos los comités
- **WHEN** un usuario con rol `PersonalSubGerencia` consulta beneficiarios
- **THEN** el sistema puede devolver beneficiarios de cualquier comité

### Requirement: Proteger la API REST de beneficiarios
El sistema SHALL exigir autenticación y autorización en las operaciones REST de beneficiarios.

#### Scenario: Operación sin autenticación
- **WHEN** un cliente anónimo solicita una operación protegida de beneficiarios
- **THEN** el sistema rechaza la solicitud

#### Scenario: Operación autorizada
- **WHEN** un usuario autenticado con el rol correspondiente solicita una operación permitida sobre su alcance
- **THEN** el sistema procesa la solicitud y devuelve el resultado correspondiente

## MODIFIED Requirements

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
