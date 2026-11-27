# Design

## Context

El proyecto actual es una aplicación MVC vacía con EF Core SQL Server ya referenciada. Esta propuesta establece los cimientos técnicos descritos en `proposal.md`, respetando el stack .NET 10 de `openspec/config.yaml`.

## Goals / Non-Goals

**Goals:**
- Organizar el monolito modular en carpetas por responsabilidad.
- Crear persistencia relacional con EF Core 10 y Microsoft SQL Server.
- Establecer Identity, roles y vínculo usuario-comité.
- Introducir repositorios, Unit of Work y auditoría sin acoplar módulos al DbContext.
- Preparar pruebas xUnit y la primera prueba ejecutable.

**Non-Goals:**
- Implementar envío real de SMS/WhatsApp.
- Implementar PWA, almacenamiento local o sincronización offline.
- Implementar importación masiva de padrones históricos.
- Implementar mapas o reportes geográficos.
- Integrar RENIEC o SISFOH.

## Decisions

### Arquitectura respetada

Se respeta el enfoque arquitectónico y el estilo de la carpeta `Arquitectura/`: monolito modular MVC con el flujo Controlador -> Servicio -> Unidad de Trabajo/Repositorio -> EF Core. Las capas visuales continúan en Razor y los módulos mantienen límites de carpetas.

**Alternativa considerada:** crear microservicios o proyectos por módulo. Se descarta por DA-05 y la restricción de monolito modular.

### Organización de módulos

Cada módulo tendrá `Entities`, `Interfaces` y `Services` según corresponda:

- `Modules/Autenticacion`
- `Modules/Beneficiarios`
- `Modules/Duplicados`
- `Modules/Revalidacion`
- `Modules/Notificaciones`
- `Modules/Reportes`
- `Modules/Staging`

Las carpetas compartidas serán `Data` para `AppDbContext`, repositorios y Unit of Work, y `Shared/Auditoria` para el registro transversal.

### Persistencia

`AppDbContext` heredará de `IdentityDbContext` y se registrará con `UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))`. En desarrollo se usará una cadena LocalDB.

Las entidades iniciales serán `Comite`, `Beneficiario`, `Programa` e `HistorialCambio`. `Beneficiario.Dni` tendrá índice único filtrado para inscripciones activas, compatible con RN-02 y el rendimiento RNF-02.

**Alternativa considerada:** una clave única global sobre DNI. Se descarta porque permitiría inconsistencias si una inscripción existente debe inactivarse.

### Identity y vínculo territorial

Se utilizará ASP.NET Core Identity con los roles `DirigenteComite` y `PersonalSubGerencia`. La vinculación usuario-comité se implementará mediante una relación propia basada en el usuario de Identity.

**Alternativa considerada:** roles codificados sin persistencia. Se descarta por RF-12, RF-13 y RF-15.

### Auditoría

`HistorialCambio` conservará el identificador o nombre del usuario, fecha UTC y motivo. Los servicios de negocio serán responsables de registrar el cambio después de una operación válida.

**Alternativa considerada:** auditado únicamente con logs de aplicación. Se descarta porque RN-03 y RF-05 requieren una consulta persistida del historial.

### Pruebas

Se creará `Proyecto.Tests` como proyecto xUnit con Coverlet. La prueba inicial deberá verificar un comportamiento pequeño y determinista, preferentemente un registro o consulta de entidad del dominio.

## Risks / Trade-offs

- [LocalDB no disponible fuera de Windows] → Mantener cadenas por entorno y documentar SQL Server alternativo.
- [Un índice único filtrado puede rechazar migraciones con datos existentes] → Ejecutar limpieza o revisión previa antes de aplicar la migración inicial.
- [Identity amplía el modelo de base de datos] → Mantener tablas Identity dentro del mismo AppDbContext.
- [Repositorios y Unit of Work pueden ser abstractos] → Exponerlos solo para operaciones comunes y mantener servicios modulares.
- [El índice DNI no garantiza por sí solo resolver duplicados entre estados] → El servicio aplicará la regla RN-02 además de la restricción de datos.

## Migration Plan

1. Crear el modelo y el DbContext.
2. Generar la migración inicial con EF Core.
3. Revisar índices y relaciones antes de aplicar.
4. Ejecutar `dotnet ef database update` en el entorno de desarrollo.
5. Ejecutar las pruebas xUnit.
6. Verificar que la aplicación inicia y que Identity está disponible.

## Open Questions

- Ninguna decisión pendiente cambia el alcance definido; el nombre exacto de columnas en la relación usuario-comité puede resolverse con convenciones EF Core durante implementación.
