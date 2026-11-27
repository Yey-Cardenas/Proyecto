# Proposal

## Why

El proyecto dispone de información fragmentada sobre actores, historias, requisitos, calidad, restricciones y drivers arquitectónicos. Consolidarla evita interpretaciones contradictorias y proporciona una base verificable para diseñar e implementar el Sistema de Gestión de Programas Sociales.

## What Changes

- Documenta en un único requerimientos.md el alcance funcional y no funcional del sistema.
- Consolida reglas de negocio, restricciones, historias de usuario y criterios generales de aceptación.
- Define capacidades OpenSpec para los comportamientos principales del sistema.
- Establece trazabilidad entre historias de usuario, requisitos y escenarios de calidad.
- Alinea el alcance tecnológico con C# / ASP.NET Core MVC, EF Core 10 y Microsoft SQL Server.

## Capabilities

### New Capabilities

- `beneficiary-management`: Registro, actualización, inactivación, consulta y auditoría del padrón de beneficiarios.
- `duplicate-detection`: Detección de posibles DNI duplicados y revisión de alertas por personal autorizado.
- `periodic-revalidation`: Solicitudes periódicas de revalidación y gestión de su vencimiento.
- `coverage-reporting`: Visualización geográfica, reportes de cobertura y exportaciones.
- `historical-data-staging`: Importación y depuración de padrones históricos antes de su consolidación.
- `role-based-access`: Inicio de sesión, permisos por rol y administración de usuarios.
- `offline-beneficiary-registration`: Registro local sin conexión y sincronización posterior.
- `beneficiary-notifications`: Envío de notificaciones mediante una integración externa SMS/WhatsApp.

### Modified Capabilities

None.

## Impact

- Afecta la documentación de análisis en analisis-de-sistema/requerimientos.md.
- Fija el alcance de los futuros módulos MVC, PWA/offline, autenticación con ASP.NET Core Identity, EF Core, SQL Server, reportes y staging.
- Introduce restricciones de privacidad, auditoría, disponibilidad, rendimiento y operación offline.
- No modifica código de aplicación ni configura infraestructura en esta etapa.
