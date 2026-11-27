# Requerimientos consolidados del Sistema de Gestión de Programas Sociales

## 1. Propósito

El Sistema de Gestión de Programas Sociales centraliza el padrón de beneficiarios de los programas municipales de Huamanga, especialmente el Vaso de Leche y los Comedores Populares. El sistema reemplaza padrones independientes, físicos o en hojas de cálculo, por una fuente municipal única que permite registrar beneficiarios, detectar duplicados, revalidar información, supervisar la cobertura y generar reportes.

## 2. Alcance

### Dentro del alcance

- Gestión de beneficiarios por comité.
- Validación de DNI contra el padrón municipal.
- Gestión de altas, bajas, modificaciones e inactivación de beneficiarios.
- Detección y evaluación de posibles duplicados.
- Revalidación periódica de beneficiarios.
- Notificaciones por SMS/WhatsApp.
- Visualización geográfica y reportes de cobertura.
- Importación, limpieza y consolidación de padrones históricos.
- Autenticación y autorización basada en roles.
- Operación offline en dispositivos móviles y sincronización posterior.
- Auditoría, respaldo y exportación de información.

### Fuera del alcance de la primera versión

- Integración en línea con RENIEC.
- Integración en línea con SISFOH.
- Servicios distribuidos o una arquitectura de microservicios.
- Obtención de datos geográficos de fuentes externas no administradas por el municipio.

## 3. Actores

| Actor | Responsabilidad |
|-------|-----------------|
| Dirigente de Comité | Registra, actualiza e inactiva beneficiarios; consulta su padrón y opera sin conexión cuando sea necesario. |
| Personal de la Sub Gerencia de Programas Sociales | Supervisa el padrón, evalúa duplicados, administra usuarios, consulta cobertura y genera reportes. |
| Beneficiario | Recibe avisos sobre aprobación, rechazo o necesidad de revalidación. |
| Gateway SMS/WhatsApp | Recibe solicitudes de envío y entrega las notificaciones externas. |
| Padrón histórico | Fuente de datos Excel o física usada durante la migración inicial. |

## 4. Requisitos funcionales

| ID | Requisito |
|----|-----------|
| RF-01 | Permitir a los dirigentes registrar beneficiarios, con DNI obligatorio y los datos necesarios para identificar y ubicar al beneficiario. |
| RF-02 | Validar automáticamente si el DNI ya existe en el padrón municipal antes de aceptar una inscripción. |
| RF-03 | Permitir marcar un beneficiario como inactivo por fallecimiento, cambio de condición o inasistencia prolongada. |
| RF-04 | Alertar al personal municipal cuando se detecte un posible duplicado. |
| RF-05 | Mantener el historial de altas, bajas y modificaciones, incluyendo fecha, usuario responsable y motivo del cambio. |
| RF-06 | Solicitar la revalidación de cada beneficiario cada seis meses y notificar al comité cuando corresponda. |
| RF-07 | Enviar una notificación por SMS/WhatsApp cuando una inscripción sea aprobada o cuando el beneficiario deba revalidar sus datos. |
| RF-08 | Mostrar un mapa con la cantidad de beneficiarios activos por zona o distrito. |
| RF-09 | Generar reportes de cobertura que comparen beneficiarios registrados con la población estimada por zona. |
| RF-10 | Permitir exportar el padrón depurado y los reportes en Excel o PDF. |
| RF-11 | Proporcionar un módulo temporal de staging para importar, validar, limpiar y estructurar padrones históricos antes de cargarlos a la base principal. |
| RF-12 | Permitir iniciar sesión mediante credenciales y asignar permisos según el rol del usuario. |
| RF-13 | Permitir al personal municipal registrar, actualizar y desactivar usuarios, asignarles rol y vincularlos a un comité cuando corresponda. |
| RF-14 | Permitir guardar registros locales sin conexión y sincronizarlos automáticamente al recuperar internet. |
| RF-15 | Restringir las operaciones de cada usuario al alcance permitido por su rol y comité. |
| RF-16 | Permitir consultar el detalle y el historial de cambios de los beneficiarios autorizados. |

## 5. Reglas de negocio

| ID | Regla |
|----|-------|
| RN-01 | El DNI es el identificador principal de búsqueda y validación dentro del padrón municipal. |
| RN-02 | Un mismo DNI no puede tener una inscripción activa duplicada en el padrón municipal. |
| RN-03 | Todo cambio de estado o modificación relevante debe registrar usuario, fecha y motivo. |
| RN-04 | La revalidación vencida debe generar una alerta o solicitud de revalidación dentro del período configurado de seis meses. |
| RN-05 | Un beneficiario inactivo no debe contabilizarse como cobertura activa. |
| RN-06 | Los registros importados deben pasar por staging y ser aprobados antes de incorporarse al padrón principal. |
| RN-07 | Las notificaciones deben enviarse únicamente a datos de contacto válidos y registrados con autorización. |

## 6. Requisitos no funcionales

| ID | Requisito no funcional |
|----|------------------------|
| RNF-01 | La disponibilidad objetivo debe ser al menos 99% y la PWA debe seguir permitiendo registros sin conexión. |
| RNF-02 | La validación de duplicados por DNI debe responder en menos de 0.5 segundos con hasta 15,000 beneficiarios. |
| RNF-03 | El formulario principal de registro debe requerir como máximo cinco campos obligatorios. |
| RNF-04 | Los datos personales deben estar protegidos mediante cifrado, controles de acceso por rol y prácticas acordes con la Ley peruana N.° 29733. |
| RNF-05 | La interfaz PWA debe funcionar en Android 8 o superior y navegadores web modernos. |
| RNF-06 | Toda modificación al padrón debe ser auditable mediante usuario, fecha y motivo. |
| RNF-07 | El sistema debe soportar al menos 15,000 beneficiarios sin degradación significativa del rendimiento. |
| RNF-08 | La arquitectura debe permitir incorporar nuevos programas sociales sin rediseñar el sistema completo. |
| RNF-09 | Deben generarse copias de seguridad diarias de la base de datos. |
| RNF-10 | La interfaz debe ser simple y adecuada para usuarios con poca experiencia tecnológica. |

## 7. Restricciones

- Arquitectura de Monolito Modular ASP.NET Core MVC.
- Aplicación web con vistas Razor, HTML5, CSS3 y Bootstrap, y soporte PWA con Service Worker.
- Comunicación mediante controladores MVC y API REST cuando se requiera integración.
- Backend C# con ASP.NET Core MVC (.NET 10).
- Persistencia con Entity Framework Core 10 y Microsoft SQL Server.
- Autenticación y autorización con ASP.NET Core Identity.
- Caché en memoria con IMemoryCache.
- Pruebas con xUnit y cobertura con Coverlet.
- Documentación de API con Swagger/OpenAPI mediante Swashbuckle.
- Dos servidores Ubuntu Server 22.04 LTS: aplicación y base de datos.
- Base de datos no expuesta directamente desde redes externas.
- Integración externa con gateway SMS/WhatsApp.
- Git y GitHub para control de versiones.
- Presupuesto municipal limitado y operación de bajo costo.
- Independencia de RENIEC y SISFOH en la primera versión.

## 8. Historias de usuario consolidadas

| ID | Historia |
|----|----------|
| HU01 | Como dirigente, quiero registrar beneficiarios con DNI para mantener actualizado el padrón de mi comité. |
| HU02 | Como dirigente, quiero recibir un aviso si el DNI ya está inscrito en otro comité para evitar duplicados. |
| HU03 | Como dirigente, quiero marcar beneficiarios como inactivos para mantener vigentes los padrones. |
| HU04 | Como personal municipal, quiero revisar duplicados e historial de cambios para depurar el padrón. |
| HU05 | Como personal municipal, quiero solicitar revalidaciones cada seis meses para detectar beneficiarios que ya no califican. |
| HU06 | Como beneficiario, quiero recibir SMS/WhatsApp sobre aprobación o revalidación para conocer mi situación. |
| HU07 | Como personal municipal, quiero consultar cobertura por zona para identificar áreas sobre-atendidas o desatendidas. |
| HU08 | Como personal municipal, quiero generar reportes y exportarlos para sustentar la distribución de recursos. |
| HU09 | Como personal municipal, quiero importar y depurar padrones históricos para consolidarlos sin arrastrar errores. |
| HU10 | Como usuario autorizado, quiero iniciar sesión según mi rol para acceder solo a funciones permitidas. |
| HU11 | Como dirigente, quiero registrar beneficiarios sin conexión para sincronizarlos al recuperar la red. |

## 9. Criterios generales de aceptación

- Un DNI puede consultarse y registrarse en menos de 0.5 segundos.
- Un registro realizado sin conexión permanece disponible localmente y se sincroniza sin pérdida al recuperar conectividad.
- Los usuarios solo ven y modifican datos compatibles con su rol y asociación territorial.
- Las altas, bajas y cambios relevantes producen registros de auditoría.
- Los duplicados detectados no se incorporan automáticamente al padrón sin evaluación.
- Los datos históricos no pasan a producción hasta completar validación y depuración.
- Los reportes e exportaciones reflejan solo beneficiarios activos según las reglas vigentes.
- Las notificaciones externas muestran estado de entrega o falla sin bloquear las operaciones principales.

## 10. Exclusiones y riesgos

| Tipo | Elemento | Tratamiento |
|------|----------|-------------|
| Exclusión | RENIEC/SISFOH | Considerar en una fase posterior mediante integración controlada. |
| Riesgo | Datos históricos incompletos o inconsistentes | Usar staging obligatorio, validaciones y revisión humana. |
| Riesgo | Reconexión con registros locales | Diseñar identificación y reconciliación de operaciones offline. |
| Riesgo | Conflictos de duplicados | Mantener un flujo de revisión y resolución por personal municipal. |
| Riesgo | Baja alfabetización digital | Diseñar una interfaz simple y ejecutar capacitación. |
| Riesgo | Fallos del gateway externo | Registrar errores y reintentar sin impedir el funcionamiento central. |

## 11. Trazabilidad

| Historia | Requisitos funcionales relacionados |
|----------|---------------------------------------|
| HU01 | RF-01 |
| HU02 | RF-02 |
| HU03 | RF-03, RF-05 |
| HU04 | RF-04, RF-05, RF-16 |
| HU05 | RF-06 |
| HU06 | RF-07 |
| HU07 | RF-08 |
| HU08 | RF-09, RF-10 |
| HU09 | RF-11 |
| HU10 | RF-12, RF-13, RF-15 |
| HU11 | RF-14 |
