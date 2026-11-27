# Design

## Context

El sistema propuesto es un monolito modular ASP.NET Core MVC desplegado como una sola aplicación .NET 10. Las vistas usan Razor, HTML5, CSS3 y Bootstrap; la persistencia usa EF Core 10 con Microsoft SQL Server. La PWA mantiene operaciones locales para continuidad offline y el backend sigue el flujo MVC Controlador -> Servicio -> Unidad de Trabajo/Repositorio -> EF Core.

## Goals / Non-Goals

**Goals:**
- Establecer módulos cohesivos con límites claros dentro del backend.
- Mantener un modelo único de beneficiarios y DNI para evitar duplicados municipales.
- Separar staging de las tablas de producción.
- Proteger datos personales y registrar todas las modificaciones relevantes.
- Permitir sincronización offline sin perder operaciones locales.
- Respetar el stack obligatorio declarado en openspec/config.yaml y los documentos de arquitectura.

**Non-Goals:**
- Elegir proveedores concretos de SMS/WhatsApp.
- Diseñar integraciones futuras con RENIEC o SISFOH.
- Descomponer el sistema en servicios distribuidos.
- Implementar la solución en esta etapa.

## Decisions

### Límites modulares

Separar beneficiarios, duplicados, revalidación, cobertura/reportes, staging, autenticación y notificaciones en módulos del monolito modular.

**Rationale:** coincide con la mantenibilidad requerida y permite incorporar programas sociales sin ampliar el acoplamiento.

**Alternativa considerada:** microservicios. Se descartó por complejidad operacional y por la restricción de monolito modular y bajo costo.

### Decisión de arquitectura

Se respetan el monolito modular y el flujo Controlador -> Servicio -> Unit of Work/Repositorio -> EF Core establecidos en `Arquitectura/enfoque-arquitectonico.md`, `Arquitectura/estilo-arquitectonico.md` y `openspec/config.yaml`.

### DNI as the primary lookup key

Usar el DNI como clave de búsqueda municipal y aplicar una restricción de unicidad para inscripciones activas compatibles con el modelo de datos.

**Rationale:** permite detectar duplicados antes de persistir y responde al escenario de rendimiento esperado.

**Alternative considered:** permitir duplicados temporales y limpiarlos después. Se descartó porque permitiría estados inconsistentes en producción.

### PWA offline-first

Guardar operaciones pendientes en almacenamiento local mediante Service Worker y sincronizarlas al recuperar conectividad. La interfaz utiliza vistas Razor y componentes PWA compatibles dentro de la aplicación MVC.

**Rationale:** es necesario en zonas con cobertura baja y satisface AC01.

**Alternativa considerada:** modo manual de importación posterior. Se descartó por aumentar la pérdida de datos y el trabajo operativo.

### Staging separado de producción

Usar tablas o áreas temporales de staging separadas y exigir aprobación humana antes de consolidar.

**Rationale:** evita propagar errores de Excel o listas físicas al padrón municipal.

**Alternativa considerada:** limpieza directa durante la importación. Se descartó por falta de revisión y trazabilidad.

### Seguridad y auditabilidad

Aplicar ASP.NET Core Identity, autorización por rol/comité, cifrado de datos personales y registros de auditoría transversales.

**Rationale:** protege datos sensibles y permite reconstruir cambios y responsables.

**Alternativa considerada:** autenticación sin roles centralizados. Se descartó por no cumplir RF-12, RF-13 y RF-15.

## Risks / Trade-offs

- [Riesgo de conflictos al sincronizar] → Asignar identificadores locales, registrar operaciones pendientes y resolver duplicados en el servidor.
- [Riesgo de latencia en DNI duplicado] → Indexar DNI y medir el recorrido completo de validación con cargas representativas.
- [Riesgo de volumen histórico irregular] → Validar tipos, requeridos y coherencia geográfica antes de aprobar staging.
- [Riesgo de indisponibilidad del gateway] → Separar notificaciones asíncronamente y conservar estado/reintentos.
- [Riesgo de crecimiento de auditoría] → Definir política de retención y respaldos diarios sin exponer datos innecesarios.

## Migration Plan

1. Crear el esquema Microsoft SQL Server mediante EF Core y las áreas de staging.
2. Importar datos históricos sin activar registros hasta su revisión.
3. Ejecutar validaciones, detección de duplicados y depuración aprobada.
4. Consolidar el padrón inicial y habilitar autenticación.
5. Desplegar la PWA y ejecutar pruebas con usuarios piloto.
6. Activar sincronización offline y notificaciones.

## Open Questions

No hay decisiones pendientes que cambien el alcance. Las reglas detalladas de resolución de duplicados pueden definirse durante el diseño de implementación.

## Implementation Verification Plan

- Beneficiarios y acceso por rol: pruebas xUnit de registro, inactivación, consulta de historial y denegación fuera del rol o comité.
- Duplicados y sincronización offline: pruebas xUnit de DNI existente, conflicto al reconectar y conservación de operaciones locales pendientes.
- Reportes, staging y notificaciones: pruebas xUnit de exportación por permisos, aprobación antes de producción y reintento ante fallo del gateway.
- Rendimiento: carga representativa con 15,000 beneficiarios y medición end-to-end de la validación de DNI por debajo de 0.5 segundos.
- Reconexión: desconectar la PWA, crear múltiples operaciones locales y recuperar conectividad sin pérdida ni duplicación.
- Privacidad, auditoría y respaldo: verificar Identity, cifrado y restricciones por rol, entradas de auditoría con usuario/fecha/motivo y una copia diaria restaurable.
