# Proposal

## Why

El proyecto ASP.NET Core MVC ya contiene dependencias EF Core y SQL Server, pero todavía carece de la estructura modular, el modelo de persistencia, la identidad institucional y las pruebas necesarias para comenzar el desarrollo funcional. Esta base técnica evita que los módulos posteriores dependan de decisiones dispersas o incompatibles.

## What Changes

- Crea la estructura interna del monolito modular para Autenticacion, Beneficiarios, Duplicados, Revalidacion, Notificaciones, Reportes y Staging.
- Agrega las carpetas compartidas Data y Shared con DbContext, repositorios, Unit of Work y auditoría.
- Configura Microsoft SQL Server mediante EF Core, una migración inicial y el contexto de la aplicación.
- Configura ASP.NET Core Identity con los roles `DirigenteComite` y `PersonalSubGerencia`, vinculando usuarios con comités.
- Agrega entidades iniciales para Comité, Beneficiario, Programa e HistorialCambio.
- Configura la inyección de dependencias y crea `Proyecto.Tests` con xUnit y Coverlet.
- Amplía los requisitos de acceso por rol y gestión de beneficiarios con vínculos de comité, DNI activo único y auditoría persistida.

## Capabilities

### New Capabilities

None.

### Modified Capabilities

- `role-based-access`: incorpora ASP.NET Core Identity, los roles dirigencial y municipal, y la vinculación obligatoria o opcional del usuario con un comité según corresponda.
- `beneficiary-management`: incorpora la unicidad del DNI para inscripciones activas y el registro persistido de usuario, fecha y motivo en el historial de cambios.

## Impact

- Se modificará `Proyecto/Proyecto.csproj` para incorporar ASP.NET Core Identity y dependencias de pruebas, si aún no están presentes.
- Se modificará `Proyecto/Program.cs` para registrar Identity, AppDbContext, repositorios, Unit of Work y servicios.
- Se actualizará `Proyecto/appsettings.json` con la cadena `DefaultConnection` compatible con LocalDB en desarrollo.
- Se crearán módulos, entidades, interfaces, servicios, migraciones y una prueba xUnit.
- No se implementan notificaciones reales, PWA/offline, importaciones de padrón ni mapas.
