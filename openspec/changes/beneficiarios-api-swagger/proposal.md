# Proposal

## Why

La base técnica ya incluye entidades, Identity, autorización por comité, auditoría y servicios iniciales, pero aún no ofrece de extremo a extremo el flujo funcional de gestión de beneficiarios ni una superficie REST documentada. Esta propuesta completa la primera versión operativa sin duplicar la arquitectura existente y valida los atributos de rendimiento, seguridad, usabilidad y mantenibilidad con datos representativos.

## What Changes

- Completar el módulo de beneficiarios para listar, registrar, editar e inactivar registros con motivo.
- Implementar el formulario Razor de registro y edición con Bootstrap, máximo cinco campos obligatorios y validación del DNI de ocho dígitos.
- Endurecer la detección de DNI activo antes del registro mediante consulta indexada y `IMemoryCache`, bloqueando altas duplicadas.
- Registrar en el historial toda alta, modificación e inactivación con usuario, fecha y motivo.
- Extender la autorización por comité para que el `DirigenteComite` opere únicamente dentro de su comité y el `PersonalSubGerencia` pueda operar sobre todos los comités.
- Exponer el CRUD principal de beneficiarios bajo `/api/beneficiarios` mediante controladores con `[ApiController]` y publicar su contrato OpenAPI en `/swagger` con Swashbuckle.
- Incorporar datos de desarrollo con dos comités, dos usuarios de prueba por rol y hasta 15 000 beneficiarios ficticios.
- Ampliar las pruebas xUnit para DNI duplicado, inactivación con motivo, historial y restricción por comité.
- Mantener fuera de alcance las notificaciones, revalidación, staging, PWA/offline, mapas y exportación.

## Capabilities

### New Capabilities

- No se introducen nombres de capacidad nuevos: las funciones solicitadas completan responsabilidades ya modeladas en las especificaciones existentes.

### Modified Capabilities

- `beneficiary-management`: agrega el flujo completo de listado, registro, edición e inactivación, formulario Razor, validación de DNI y auditoría observable.
- `duplicate-detection`: define que la detección previa bloquea el registro de una inscripción activa y establece el uso combinado de índice SQL y caché sin permitir duplicados activos.
- `role-based-access`: concreta la separación operativa entre dirigente de comité y personal de subgerencia en vistas y API.

## Impact

- **Código afectado:** módulos `Beneficiarios`, `Duplicados` y `Autenticacion`, servicios compartidos de auditoría y configuración de `Program.cs`.
- **Interfaces web:** formularios Razor/Bootstrap y navegación MVC de beneficiarios.
- **API:** nuevas rutas bajo `/api/beneficiarios` para consulta por identificador o DNI, alta, actualización e inactivación.
- **Persistencia:** uso de las entidades existentes y de índices; se requieren migraciones solo si las operaciones de implementación revelan diferencias de modelo.
- **Dependencias:** `IMemoryCache` y Swashbuckle ya considerados por la base técnica; se mantiene Entity Framework Core y SQL Server.
- **Calidad y pruebas:** pruebas xUnit con SQL LocalDB, datos de referencia de 15 000 beneficiarios y medición de la detección por DNI.
