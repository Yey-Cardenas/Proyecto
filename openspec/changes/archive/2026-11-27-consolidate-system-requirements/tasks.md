# Tasks

## 1. Consolidación de requisitos

- [x] 1.1 Módulo de análisis documental: validar que requerimientos.md incluye las seis fuentes del análisis y revisar su cobertura, cubriendo RF-01 a RF-16 y RNF-01 a RNF-10.
- [x] 1.2 Módulo de trazabilidad: cruzar historias HU01 a HU11 con los RF correspondientes y verificar que ninguna historia quede sin requisito.
- [x] 1.3 Módulo de restricciones: revisar RNF y RC contra la sección 7 consolidada y verificar que no falte una restricción tecnológica.

## 2. Revisión de especificaciones OpenSpec

- [x] 2.1 Capacidades de sistema: revisar las specs contra RF/RNF y verificar escenarios principales de éxito y error con el stack ASP.NET Core MVC.
- [x] 2.2 Herramientas OpenSpec: ejecutar validación estricta y verificar que no existan errores estructurales ni encabezados delta.

## 3. Preparación de implementación

- [x] 3.1 Módulos Beneficiarios y Accesos: preparar el plan ASP.NET Core MVC, Identity, EF Core y SQL Server para RF-01, RF-03, RF-12, RF-13, RF-15 y RF-16; verificar pruebas xUnit de registro, inactivación y control de acceso.
- [x] 3.2 Módulos Duplicados y Offline: preparar tareas para RF-02, RF-04, RF-14 y RNF-02; verificar pruebas xUnit de DNI existente, conflicto al reconectar y conservación local.
- [x] 3.3 Módulos Cobertura, Staging y Notificaciones: preparar tareas MVC/EF Core para RF-07 a RF-11 y RNF-09; verificar exportaciones, aprobaciones, respaldos y fallos externos con pruebas xUnit.

## 4. Plan de verificación de calidad

- [x] 4.1 Módulo Duplicados: definir carga con 15,000 beneficiarios para RNF-02 y RNF-07 y verificar respuesta menor a 0.5 segundos.
- [x] 4.2 Módulo Offline: definir reconexión PWA para RF-14 y RNF-01 y verificar que no se pierda ni duplique una operación pendiente.
- [x] 4.3 Seguridad y respaldos: definir pruebas Identity, EF Core y SQL Server para RF-05, RF-15, RNF-04, RNF-06 y RNF-09 y verificar restricciones, auditoría y copia diaria restaurable.

## Workflow follow-up

- Review the proposal, specs, design and tasks with project stakeholders.
- Archive the change after implementation and the project's review requirements are satisfied.
