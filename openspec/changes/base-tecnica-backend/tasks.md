# Tasks

## 1. Configuración compartida

- [x] 1.1 Datos: configurar `DefaultConnection` y SQL Server LocalDB para desarrollo; verificar que la aplicación lee la cadena y arranca sin errores, cubriendo RC-05 y RNF-04.
- [x] 1.2 Datos: crear `AppDbContext` con EF Core, registrar `UseSqlServer` en `Program.cs` y agregar servicios compartidos; verificar build y arranque cubriendo DA-02 y RNF-07.
- [x] 1.3 Datos/Shared: crear repositorio genérico, Unit of Work e interfaz de auditoría; verificar pruebas xUnit de registro y confirmación de transacciones cubriendo RF-05 y RNF-06.

## 2. Modelo de dominio inicial

- [x] 2.1 Beneficiarios: crear entidades `Comite`, `Beneficiario`, `Programa` e `HistorialCambio` con relaciones iniciales; verificar compilación y cobertura xUnit cubriendo RF-01 y RF-05.
- [x] 2.2 Beneficiarios/Duplicados: configurar DNI obligatorio, índice único filtrado para activos y asociación con comité; verificar migración y prueba xUnit de unicidad cubriendo RN-02, RNF-02 y RNF-07.
- [x] 2.3 Shared Auditoria: implementar persistencia de usuario, fecha UTC y motivo; verificar prueba xUnit cubriendo RN-03, RF-05 y RNF-06.

## 3. Identidad y autorización

- [x] 3.1 Autenticacion: configurar ASP.NET Core Identity sobre `AppDbContext`; verificar inicio de Identity cubriendo RF-12 y RNF-04.
- [x] 3.2 Autenticacion: crear los roles `DirigenteComite` y `PersonalSubGerencia`; verificar su creación y prueba xUnit cubriendo RF-12 y RF-13.
- [x] 3.3 Autenticacion/Beneficiarios: vincular el usuario Identity con su comité; verificar carga y autorización por comité cubriendo RF-13 y RF-15.

## 4. Estructura modular

- [x] 4.1 Beneficiarios/Duplicados: crear carpetas y interfaces iniciales de servicio junto con implementaciones base; verificar build y una prueba xUnit por servicio cubriendo RF-01, RF-02 y RNF-02.
- [x] 4.2 Revalidacion/Reportes: crear estructura de entidades, interfaces y servicios sin implementar flujos externos ni mapas; verificar build y pruebas xUnit básicas cubriendo RF-06 y RF-08.
- [x] 4.3 Notificaciones/Staging: crear estructura base de servicios e interfaces sin proveedores ni importaciones reales; verificar build y pruebas xUnit mínimas cubriendo RF-07 y RF-11.

## 5. Migración y pruebas

- [x] 5.1 Datos: generar la migración inicial de EF Core y revisar índices, relaciones y tablas Identity; verificar que el archivo de migración representa el modelo cubriendo RN-02 y RN-03.
- [x] 5.2 Datos: ejecutar la actualización de SQL Server LocalDB; verificar que el esquema, índices y tablas Identity quedan creados cubriendo RNF-09 y RNF-06.
- [x] 5.3 Proyecto.Tests: crear proyecto xUnit con Coverlet y una prueba de ejemplo; ejecutar `dotnet test` y verificar éxito cubriendo RF-01 y RNF-07.

## 6. Integración de la aplicación

- [x] 6.1 Configuración: registrar todos los servicios nuevos mediante DI en `Program.cs`; verificar build y arranque cubriendo DA-04 y RNF-08.
- [x] 6.2 Seguridad: habilitar autenticación y autorización MVC con Identity; verificar que las rutas protegidas rechazan usuarios no autenticados cubriendo RF-12 y RNF-04.
- [x] 6.3 Verificación final: ejecutar build y pruebas completas; confirmar que no se implementaron PWA/offline, notificaciones reales, importaciones ni mapas y documentar cualquier bloqueo cubriendo el alcance y RNF-08.

## Workflow follow-up

- Ejecutar `dotnet ef migrations add InitialCreate` y `dotnet ef database update` durante la implementación.
- Revisar la base técnica con stakeholders antes de iniciar los módulos funcionales avanzados.
