# Spec Delta

## MODIFIED Requirements

### Requirement: Iniciar sesión
El sistema SHALL autenticar a dirigentes y personal municipal mediante credenciales proporcionadas por ASP.NET Core Identity y habilitar únicamente los permisos correspondientes a su rol y asociación con un comité.

#### Scenario: Credenciales válidas de dirigente
- **WHEN** un usuario con el rol DirigenteComite inicia sesión
- **THEN** el sistema habilita las operaciones permitidas para su comité vinculado

#### Scenario: Credenciales válidas de personal municipal
- **WHEN** un usuario con el rol PersonalSubGerencia inicia sesión
- **THEN** el sistema habilita las operaciones municipales autorizadas

#### Scenario: Credenciales válidas
- **WHEN** un usuario inicia sesión con credenciales válidas
- **THEN** el sistema habilita únicamente las funciones autorizadas para su rol

### Requirement: Administrar usuarios
El sistema SHALL permitir al PersonalSubGerencia registrar, actualizar y desactivar usuarios, asignar los roles DirigenteComite o PersonalSubGerencia y vincular los dirigentes con un comité.

#### Scenario: Asignación de rol y comité
- **WHEN** personal municipal autorizado crea o actualiza un usuario dirigente
- **THEN** el sistema asigna el rol correspondiente y conserva la relación usuario-comité

#### Scenario: Asignación de rol
- **WHEN** personal autorizado crea o actualiza un usuario
- **THEN** puede asignar su rol y comité y el cambio se aplica inmediatamente
