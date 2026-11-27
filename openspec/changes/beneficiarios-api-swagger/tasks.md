# Tasks

## 1. Beneficiarios: modelo y servicios

- [x] 1.1 (Beneficiarios, RF-01/RF-03/RF-04/RF-13, RNF-04/RNF-06) Ampliar `IBeneficiarioService` y `BeneficiarioService` para listar, obtener, editar e inactivar beneficiarios dentro del alcance autorizado; verificar con pruebas xUnit que los métodos actualizan, conservan el registro y rechazan datos inválidos.
- [x] 1.2 (Beneficiarios, RF-04/RF-13, RNF-06) Implementar inactivación transaccional que exija motivo y registre usuario, fecha UTC y operación en `HistorialCambio`; verificar con una prueba xUnit de inactivación con motivo y una prueba sin motivo.
- [x] 1.3 (Beneficiarios, RF-03/RF-13, RNF-06/RNF-08) Implementar la modificación con auditoría transaccional y DTO apropiado; verificar con una prueba xUnit que la modificación y el historial se confirman juntos.

## 2. Duplicados y rendimiento

- [x] 2.1 (Duplicados, RF-02/RF-05, RNF-02/RNF-07) Completar `DuplicateDetectionService` con consulta indexada por DNI activo y caché `IMemoryCache`; verificar con pruebas xUnit que existe duplicado activo y permite DNI con registros inactivos.
- [x] 2.2 (Duplicados/Beneficiarios, RF-02/RF-13, RNF-02/RNF-04) Integrar la comprobación de duplicados antes de toda alta y invalidar o actualizar caché después de escribir; verificar con prueba xUnit que un segundo alta activa devuelve duplicado y no crea historial adicional.
- [x] 2.3 (Duplicados, RNF-02/RNF-07) Crear prueba o benchmark reproducible con 15 000 beneficiarios ficticios que mida la detección con SQL y caché; verificar que el resultado documente el tiempo y la diferencia frente a la consulta sin caché.

## 3. Acceso por rol y seguridad

- [x] 3.1 (Autenticacion, RF-12, RNF-04) Completar los filtros de alcance de `ComiteAccessHandler` para dirigente y personal municipal en consultas y operaciones por beneficiario; verificar con pruebas xUnit que el dirigente no accede a otro comité y personal accede a todos.
- [x] 3.2 (Beneficiarios/Duplicados, RF-11/RF-12, RNF-04) Proteger todos los endpoints MVC y REST mediante autenticación y autorización antes de invocar servicios; verificar respuestas 401/403 con pruebas o ejecución manual autenticada.
- [x] 3.3 (Beneficiarios, RF-13, RNF-04/RNF-06) Exponer únicamente datos necesarios en las vistas y respuestas REST y restringir consulta de historial a personal autorizado; verificar con pruebas de autorización y revisión manual del contrato.

## 4. Interfaz Razor con Bootstrap

- [x] 4.1 (Beneficiarios, RF-01/RF-03/RF-04, RNF-03) Crear o completar las vistas Razor de listado, alta, edición e inactivación con Bootstrap; verificar manualmente la navegación y que el listado respeta el comité del usuario.
- [x] 4.2 (Beneficiarios, RF-01, RNF-03) Configurar en el formulario cinco campos obligatorios como máximo: DNI, nombres, apellidos, comité y programa; verificar manualmente que dirección y datos adicionales no bloquean el alta.
- [x] 4.3 (Beneficiarios, RF-01/RF-02, RNF-03/RNF-04) Añadir validación Razor/cliente y servidor del DNI de ocho dígitos y mostrar el aviso de DNI activo sin crear el registro; verificar casos válidos e inválidos manualmente y mediante pruebas disponibles.

## 5. API REST y Swagger

- [x] 5.1 (Beneficiarios, RF-01/RF-03/RF-04, RNF-04/RNF-08) Crear controladores REST bajo `/api/beneficiarios` con `[ApiController]`, DTO de entrada/salida y operaciones GET, GET por DNI, POST, PUT e inactivación; verificar con ejecución local los códigos 200/201/204/400/404/409.
- [x] 5.2 (Autenticacion/Beneficiarios, RF-12, RNF-04) Aplicar la política de comité a las operaciones REST y impedir modificación fuera del alcance; verificar pruebas xUnit o pruebas de integración por rol y comité.
- [x] 5.3 (Beneficiarios, RF-01/RF-03/RF-04, RNF-08) Documentar DTO, parámetros y respuestas en Swashbuckle y habilitar `/swagger` solo en desarrollo; verificar que la página se abre y que todos los endpoints aparecen.

## 6. Seeder de desarrollo

- [x] 6.1 (Beneficiarios/Autenticacion, RF-01/RF-12, RNF-02/RNF-07) Implementar un seeder idempotente que cree dos comités, dos usuarios de prueba con sus roles y hasta 15 000 beneficiarios ficticios únicamente en Development; verificar ejecución repetida sin duplicar datos y presencia de los usuarios, comités y volumen esperado.

## 7. Pruebas e integración

- [x] 7.1 (Beneficiarios, RF-01/RF-04/RF-13, RNF-06/RNF-08) Incorporar pruebas xUnit adicionales para alta duplicada, inactivación con motivo e historial con usuario/fecha/motivo; verificar que las pruebas pasan y cubren los escenarios solicitados.
- [x] 7.2 (Autenticacion/Beneficiarios, RF-12, RNF-04) Incorporar prueba xUnit de restricción por comité y permiso de personal municipal sobre todos los comités; verificar que los escenarios del rol pasan.
- [x] 7.3 (Beneficiarios/Duplicados/Autenticacion, RNF-02/RNF-04/RNF-06/RNF-07/RNF-08) Ejecutar `dotnet build` y `dotnet test` de la solución completa; verificar compilación limpia y ausencia de pruebas fallidas.

## Workflow follow-up

- Ejecutar OpenSpec archive únicamente después de revisar e implementar el cambio.
- Validar los artefactos archivados y su integración con las especificaciones principales.
