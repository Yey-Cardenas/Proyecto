# Design

## Context

La base técnica ya respeta el monolito modular, incluye `BeneficiarioService`, `DuplicateDetectionService`, `ComiteAccessHandler`, `AuditoriaService`, Entity Framework Core, Identity y Swashbuckle. Los límites de esta propuesta consisten en completar y ampliar esas superficies, no sustituirlas ni introducir una nueva arquitectura.

## Goals / Non-Goals

**Goals:**
- Completar operaciones CRUD operativas para beneficiarios en MVC y REST.
- Mantener una única validación de duplicados activos y una única regla de alcance por comité.
- Conservar trazabilidad completa de altas, modificaciones e inactivaciones.
- Permitir carga reproducible de datos de desarrollo y medición con 15 000 beneficiarios.
- Mantener contratos Swagger/OpenAPI utilizables desde `/swagger`.

**Non-Goals:**
- Resolver alertas completas de duplicidad entre comités.
- Reactivación automática de beneficiarios inactivos.
- Integraciones de notificaciones, revalidación, staging/offline, mapas o exportación.
- Introducción de servicios separados, colas, mensajería o front-end distinto de MVC/Razor.

## Decisions

### 1. Extender el patrón existente Controlador -> Servicio -> Unit of Work/Repositorio -> EF Core
Los controladores MVC y API delegarán validaciones de negocio al módulo de beneficiarios y duplicados. Los servicios compartirán `IUnitOfWork`, repositorios y `IAuditoriaService`.

Alternativas consideradas: acceso directo de controladores a `DbContext` y organización por repositorios independientes. Se descartan porque contradicen la separación existente y aumentan la repetición de autorización y auditoría.

### 2. Un único punto de validación de DNI activo
Antes de registrar, `DuplicateDetectionService` consultará el índice filtrado único sobre `Beneficiario.Dni` con `Activo = true`. La primera comprobación podrá usar una clave de caché corta por DNI en `IMemoryCache`; toda alta, reactivación o inactivación invalidará o actualizará la entrada correspondiente.

Alternativa considerada: búsquedas sin caché. Se descarta para evitar trabajo adicional bajo carga. Alternativa considerada: caché como fuente de verdad. Se descarta porque la base SQL sigue siendo autoritativa y el caché no debe permitir duplicados si caduca, se invalida mal o contiene datos obsoletos.

### 3. Escrituras transaccionales y auditoría
Registrar, modificar e inactivar se ejecutará dentro de una transacción serializable o equivalente del SQL Server. La modificación del beneficiario y el registro de `HistorialCambio` se confirmarán juntos. Una inactivación exigirá motivo no vacío y conservará el registro para consulta histórica.

Alternativa considerada: actualizar el historial después de confirmar el beneficiario. Se descarta porque podría dejar una operación sin trazabilidad ante fallos parciales.

### 4. Alcance de datos centralizado en autorización
Se reutilizará la política `ComiteAccess` y `ComiteAccessHandler`. Las consultas extraerán el comité del usuario para el `DirigenteComite`; el `PersonalSubGerencia` no aplicará filtro de comité. Las operaciones por identificador validarán primero el alcance antes de modificar.

Alternativa considerada: comprobar el comité solo en Razor. Se descarta porque la API tendría otra ruta de acceso y el control no sería uniforme.

### 5. Contrato REST compatible con MVC
Se usarán controladores separados para `GET /api/beneficiarios`, `GET /api/beneficiarios/dni/{dni}`, `POST /api/beneficiarios`, `PUT /api/beneficiarios/{id}` y `PATCH /api/beneficiarios/{id}/inactivar` o una ruta semánticamente equivalente documentada. Cada endpoint llevará `[ApiController]`, códigos HTTP explícitos y modelos de entrada separados de entidades EF.

La API devolverá 200/201/204/400/401/403/404/409 según corresponda; una inscripción duplicada activa será conflicto (`409`) y no una excepción no controlada.

Alternativa considerada: exponer entidades directamente. Se descarta porque acopla el contrato público al modelo de persistencia y puede exponer navegación o datos innecesarios.

### 6. Interfaz Razor con Bootstrap
Se reutilizarán vistas Razor y Bootstrap existentes para índice, creación y edición. DNI, nombres, apellidos, comité y programa serán los cinco campos obligatorios; dirección y demás datos permanecerán opcionales. Los mensajes de duplicado se mostrarán sin registrar ni redireccionar a una operación duplicada.

Alternativa considerada: formularios SPA. Se descarta por la restricción del stack MVC/Razor.

### 7. Swagger habilitado por entorno
Swashbuckle seguirá registrado en el contenedor y `/swagger` se habilitará en desarrollo. El contrato incluirá DTO, parámetros, respuestas y descripciones; no se expondrá información sensible innecesaria.

### 8. Seeder separado para desarrollo
Un seeder de desarrollo creará o reutilizará dos comités, dos usuarios de prueba con contraseñas conocidas únicamente en entorno local y hasta 15 000 beneficiarios ficticios con DNI válidos y únicos para registros activos. Será idempotente y se ejecutará únicamente en Development.

Alternativa considerada: carga automática en producción. Se descarta por privacidad y seguridad.

## Risks / Trade-offs

- `[Caché obsoleta]` → invalidar en cada escritura y conservar SQL como fuente de verdad en el punto de confirmación.
- `[Condición de carrera]` → índice SQL filtrado por DNI activo más transacción serializable o restricción verificable en escritura.
- `[Carga masiva lenta]` → desactivar seguimiento EF, insertar por lotes controlados y conservar identificadores únicos.
- `[Exposición personal mediante API]` → DTO mínimos, autenticación/autorización obligatoria y filtros por comité.
- `[Datos de prueba accidentalmente desplegados]` → seeder detrás de `IHostEnvironment.IsDevelopment()` y configuración local.
- `[Pruebas dependientes de SQL LocalDB]` → mantener el patrón actual de base temporal y limpiar después de cada clase.

## Migration Plan

1. Añadir o ajustar migraciones únicamente si el modelo efectivo difiere del modelo actual.
2. Ejecutar las migraciones en desarrollo y construir las vistas, servicios y controladores REST sobre las entidades existentes.
3. Ejecutar el seeder de desarrollo después de roles y comités.
4. Ejecutar pruebas unitarias y de integración relacionadas con duplicados, auditoría, inactivación y alcance por comité.
5. Ejecutar la comprobación manual con Swagger y revertir cambios de migración o seeder si una prueba falla antes de desplegar.

## Open Questions

Ninguna. Las rutas REST principales, los modelos de entrada y el comportamiento de conflicto quedan definidos en este diseño sin cambiar los requisitos.
