# 📋 Tablero Kanban Integrado: TPI Programación 4 & TIF

> **Regla de oro del equipo:** Trabajo secuencial por etapas. No se arranca una etapa nueva hasta que todos los casilleros de la etapa anterior estén tildados `[x]` y el código compile localmente.

---

## 🛠️ ETAPA 1: Los Cimientos y el Modelo de Datos (Días 1 a 3)
*Objetivo: Dejar el repositorio limpio, la base de datos local conectada y el esquema de tablas funcionando.*

- [x] **Tarjeta 1: Limpieza Total de la Solución**
  - **Responsable:** Danilo Mercado
  - **Capa:** API / Application / Infrastructure
  - *Descripción:* Eliminar los archivos por defecto (`Class1.cs` y `WeatherForecastController.cs`) en todas las capas para limpiar la Clean Architecture.

- [ ] **Tarjeta 2: Estructura del DbContext Inicial**
  - **Responsable:** Francesco D'agostino
  - **Capa:** Infrastructure / API
  - *Descripción:* Crear el `ApplicationDbContext` heredando de EF Core. Configurar la inyección de dependencias en `Program.cs` de la API y agregar la `ConnectionString` en el `appsettings.json` local.

- [ ] **Tarjeta 3: Configuración de Entidades en Base de Datos**
  - **Responsable:** Facundo Nieva
  - **Capa:** Domain / Infrastructure
  - *Descripción:* Escribir el mapeo con Fluent API para las entidades del diagrama corregido (`Usuario`, `Empresa`, `JornadaLaboral`, `Liquidacion`) definiendo claves primarias y foráneas.

- [ ] **Tarjeta 4: Primera Migración Base**
  - **Responsable:** Danilo Mercado
  - **Capa:** Infrastructure / BD
  - *Descripción:* Correr comandos `Add-Migration InitialCreate` y `Update-Database` para verificar que el motor de base de datos impacte el esquema local sin errores de relaciones.

---

## 🔒 ETAPA 2: Seguridad, Filtro Global y Autenticación (Días 4 a 6)
*Objetivo: Lograr que el sistema reconozca los roles y aísle los datos por empresa automáticamente (Multitenancy).*

- [ ] **Tarjeta 5: Filtro Global de Multi-Empresa**
  - **Responsable:** Francesco D'agostino
  - **Capa:** Infrastructure
  - *Descripción:* Configurar `HasQueryFilter` en el DbContext sobre la entidad `JornadaLaboral` usando el `EmpresaId` del token para lograr el aislamiento estricto que pide el TIF.

- [ ] **Tarjeta 6: Servicio de Encriptación de Contraseñas**
  - **Responsable:** Facundo Nieva
  - **Capa:** Infrastructure / Application
  - *Descripción:* Crear el componente para hashear con sal las claves antes de registrarlas en la base de datos (seguridad básica).

- [ ] **Tarjeta 7: Generación de Tokens JWT**
  - **Responsable:** Danilo Mercado
  - **Capa:** Application
  - *Descripción:* Programar el servicio encargado de validar credenciales y emitir el token con los claims requeridos (`UsuarioId`, `Rol`, `EmpresaId`).

- [ ] **Tarjeta 8: Controladores de Acceso y Login**
  - **Responsable:** Francesco D'agostino
  - **Capa:** API
  - *Descripción:* Crear `AuthController` con el endpoint `POST /api/auth/login`. Probar con Postman que devuelva el token y bloquee accesos no autorizados.

---

## ⏱️ ETAPA 3: Lógica Operativa - Fichajes y Validaciones (Días 7 a 9)
*Objetivo: Permitir la carga de horas de operarios y habilitar el panel de control del administrador.*

- [ ] **Tarjeta 9: Servicio y Reglas de Jornada Laboral**
  - **Responsable:** Facundo Nieva
  - **Capa:** Application
  - *Descripción:* Implementar la validación en `JornadaLaboralService`: rechazar si pasa el `TopeHorasDiarias` de la empresa. Aplicar inmutabilidad si el estado es aprobado/rechazado.

- [ ] **Tarjeta 10: Controlador de Carga de Horas e Historial**
  - **Responsable:** Danilo Mercado
  - **Capa:** API
  - *Descripción:* Crear `JornadasController` con endpoints `POST /api/jornadas` (fichaje del operario) y `GET /api/jornadas/historial` (filtra directo su historial personal).

- [ ] **Tarjeta 11: Panel de Aprobación del Administrador**
  - **Responsable:** Francesco D'agostino
  - **Capa:** API / Application
  - *Descripción:* Crear endpoint `PUT /api/jornadas/{id}/estado` para que el rol Administrador apruebe o rechace fichajes pendientes.

---

## 💰 ETAPA 4: Motor de Liquidación y Terceros (Días 10 a 12)
*Objetivo: Calcular la nómina mensual cruzando datos con servicios externos para asegurar la Promoción.*

- [ ] **Tarjeta 12: Integración con API Externa de Feriados**
  - **Responsable:** Francesco D'agostino
  - **Capa:** API / Infrastructure
  - *Descripción:* Implementar `HttpClientFactory` en la API para consultar dinámicamente un servicio público de feriados calendarios de Argentina.

- [ ] **Tarjeta 13: Algoritmo de Cierre de Nómina**
  - **Responsable:** Facundo Nieva
  - **Capa:** Application
  - *Descripción:* Programar el servicio de liquidación. Buscar jornadas aprobadas del mes, calcular salarios en base al `ValorHora` de la PYME y aplicar recargo de ley si la Tarjeta 12 detectó que el día era feriado.

- [ ] **Tarjeta 14: Endpoint de Cierre Económico**
  - **Responsable:** Danilo Mercado
  - **Capa:** API
  - *Descripción:* Crear `LiquidacionesController` con el endpoint `POST /api/liquidaciones/procesar` para persistir los totales netos calculados por empresa.

---

## 🚀 ETAPA 5: Deploy y Puesta en Producción (Días 13 a 14)
*Objetivo: Dejar la API en internet lista para la defensa oral ante los profesores.*

- [ ] **Tarjeta 15: Servidor de Datos en Azure**
  - **Responsable:** Facundo Nieva
  - **Capa:** DevOps / Base de Datos
  - *Descripción:* Levantar la base de datos relacional (SQL Server o MySQL) en Azure Cloud y correr las migraciones del proyecto para impactar las tablas en producción.

- [ ] **Tarjeta 16: Pipeline de Automatización (CI/CD)**
  - **Responsable:** Francesco D'agostino
  - **Capa:** DevOps (GitHub Actions)
  - *Descripción:* Escribir el flujo `.github/workflows/deploy.yml` para que compile y publique la API automáticamente en Azure ante cada push en la rama principal.

- [ ] **Tarjeta 17: Ajustes de Variables de Entorno en la Nube**
  - **Responsable:** Danilo Mercado
  - **Capa:** DevOps / API
  - *Descripción:* Cargar el Secret del JWT y la Connection String real dentro de la configuración del Web API en Azure (ocultando los datos sensibles del código público).
