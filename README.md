# 🏢 Plataforma de Autogestión de Horas y Gestión de Nómina - Backend API

Backend REST desarrollado como Trabajo Práctico Integrador (TPI) para Programación 4. La plataforma permite administrar empresas, usuarios, jornadas laborales, aprobaciones y liquidaciones mensuales de haberes para organizaciones con múltiples perfiles de acceso.

La API se encuentra desplegada en Azure App Service y utiliza una base de datos relacional Azure SQL Database mediante Entity Framework Core con enfoque Code First.

---

## 👥 Integrantes

- 👨‍💻 Danilo Mercado
- 👨‍💻 Francesco D'agostino
- 👨‍💻 Facundo Nieva

---

## 🌐 Despliegue

La API se encuentra desplegada en Azure App Service para la instancia de evaluación docente.

- Dominio público: informado en la entrega del CVG.
- Documentación Swagger: disponible en /swagger.

---

## 🧱 Arquitectura

El proyecto está organizado bajo el patrón Clean Architecture, separando responsabilidades por capas y utilizando inyección de dependencias para desacoplar controladores, servicios, repositorios y persistencia.

### 📦 Capas

- 🧩 PlataformaAutogestion.Domain: entidades del negocio, enumeraciones, excepciones e interfaces de repositorios/servicios de dominio.
- ⚙️ PlataformaAutogestion.Application: lógica de negocio, servicios de aplicación, DTOs, requests y responses.
- 🗄️ PlataformaAutogestion.Infrastructure: implementación de persistencia con Entity Framework Core, repositorios, migraciones, ApplicationDbContext y servicios externos.
- 🌐 PlataformaAutogestion.Api: Web API, controladores, configuración de autenticación JWT, Swagger, middleware de errores e inyección de dependencias.

---

## 🛠️ Stack Tecnológico

- 💻 Lenguaje: C#
- 🌐 Framework: ASP.NET Core Web API
- ⚙️ Runtime: .NET 10
- 🧬 ORM: Entity Framework Core
- 🗄️ Base de datos: Azure SQL Database / SQL Server
- 🔐 Autenticación: JWT Bearer con roles y claims
- 📘 Documentación y pruebas: Swagger / OpenAPI
- 🚀 CI/CD: GitHub Actions
- ☁️ Cloud: Azure App Service + Azure SQL Database

---

## ✨ Funcionalidades Principales

- 🏢 Gestión de empresas por usuario SuperAdmin.
- 👥 Gestión de usuarios por empresa.
- 🔐 Autenticación mediante JWT.
- 📲 Verificación en dos pasos (MFA con TOTP) obligatoria para el SuperAdmin.
- 🛡️ Autorización por roles:
  - SuperAdmin
  - Admin
  - Empleado
- ⏱️ Carga de jornadas laborales y edición mientras estén pendientes.
- 💵 Estimación del sueldo del mes para el Empleado.
- 👥 Alta, edición y baja de empleados por parte del Admin.
- ✅ Aprobación y rechazo de jornadas, con motivo de rechazo obligatorio.
- 🕘 Historial de jornadas aprobadas y rechazadas para el Admin.
- 🏭 Edición de los datos de la empresa, del valor por hora y del tope de horas diarias por parte de su Admin.
- 💵 Las horas trabajadas en fin de semana o feriado se pagan con el valor hora de feriado que define cada empresa (por defecto, el doble del valor hora normal). Lo usan la liquidación, la estimación del empleado y las estadísticas.
- 🔀 El historial de jornadas se puede ordenar por estado (pendientes, aprobadas o rechazadas primero).
- 📑 Historial de jornadas paginado (10 por página) para el Empleado y para el Admin.
- 🌗 Tema claro y oscuro, a elección de cada persona (por defecto sigue al sistema).
- 🧾 Historial de valores hora: cada cambio del valor normal o del valor de feriados queda registrado con fecha, valor anterior y usuario.
- ⛔ Tope de horas diarias por empresa: una jornada que lo supera se rechaza al cargarla o editarla.
- 🔔 Avisos internos para el Admin cuando un empleado carga o modifica una jornada (dentro del sistema, sin correo).
- 📊 Estadísticas mensuales para el Admin: horas aprobadas, pendientes y rechazadas, costo estimado y rendimiento por empleado.
- 🧑‍💼 Alta del Admin de cada empresa por parte del SuperAdmin.
- 🧮 Simulación de liquidaciones.
- 💰 Cierre mensual de liquidaciones.
- 🗑️ Anulación de liquidaciones cerradas.
- 📄 Consulta de detalles de liquidación.
- 📑 Generación de reportes:
  - Recibos PDF.
  - Archivo TXT para lote bancario.
- 🌎 Consumo de API externa de feriados mediante HttpClientFactory.

---

## 📌 Reglas de Negocio

- 🏢 Cada usuario pertenece a una empresa, excepto el SuperAdmin.
- 🧑‍💼 Los usuarios Admin gestionan información de su propia empresa.
- ⏱️ Solo el Empleado carga horas; el Admin no carga horas propias, solo aprueba o rechaza las de sus empleados.
- 🚫 No se puede registrar más de una jornada por día (salvo que la anterior haya sido rechazada).
- ⏳ Las jornadas cargadas quedan en estado pendiente hasta ser aprobadas o rechazadas.
- ✏️ El Empleado solo puede editar sus jornadas mientras estén en estado Pendiente.
- 📝 Al rechazar una jornada es obligatorio indicar el motivo (de 3 a 250 caracteres).
- 💲 El valor por hora de la empresa lo define su Admin; el SuperAdmin no lo modifica.
- 🪪 El nombre de usuario y el correo son únicos en todo el sistema.
- 🔒 No se puede eliminar una empresa que tenga datos asociados.
- 🗃️ Un empleado con jornadas o liquidaciones no se borra: se lo da de baja (baja lógica). Deja de aparecer, no puede iniciar sesión y su historial se conserva para que las liquidaciones cerradas sigan cuadrando.
- ✅ Solo las jornadas aprobadas participan en el cierre mensual.
- 📅 Una empresa no puede tener más de una liquidación cerrada para el mismo mes y año.
- 🔁 Para rehacer una liquidación mensual, primero debe anularse la liquidación existente.
- 🔎 Las consultas están filtradas por empresa mediante filtros globales en ApplicationDbContext.

---

## 🔐 Seguridad

La API utiliza autenticación JWT Bearer. El token incluye claims de identificación, rol e IdCompany, permitiendo controlar el acceso a los endpoints protegidos.

El secret utilizado para firmar los tokens JWT se configura mediante variables de entorno en Azure App Service.

### 📲 MFA del SuperAdmin (TOTP)

El SuperAdmin no recibe el JWT definitivo solo con usuario y contraseña. El login responde con un token temporal (5 minutos, sin rol y con otra audiencia, por lo que la API de negocio lo rechaza) que solo sirve en los endpoints `/api/Auth/mfa/*`:

1. Primer ingreso: `POST /api/Auth/mfa/setup` devuelve la URI `otpauth://` para el QR (Google Authenticator, Microsoft Authenticator, etc.). El MFA queda activo al confirmar el primer código en `mfa/verify`.
2. Ingresos siguientes: `POST /api/Auth/mfa/verify` con el código de 6 dígitos entrega el JWT real.

Medidas adicionales: 5 códigos incorrectos bloquean los intentos del usuario por 5 minutos, y el secreto TOTP se guarda cifrado en la base (AES-256-GCM) con la clave `Mfa__EncryptionKey`. Admin y Empleado no usan MFA.

### 🔑 Variables principales

- AutenticacionService__SecretForKey
- AutenticacionService__Issuer
- AutenticacionService__Audience
- Mfa__EncryptionKey (32 bytes en Base64, por ejemplo `openssl rand -base64 32`)

### 🌎 Configuración de API externa de feriados

- HolidayApi__BaseUrl
- HolidayApi__TimeoutSeconds

### 📌 Ejemplo

- HolidayApi__BaseUrl=https://api.argentinadatos.com/v1/feriados/
- HolidayApi__TimeoutSeconds=10

---

## 🚀 CI/CD

El repositorio cuenta con una pipeline de GitHub Actions que se ejecuta ante cada push a main.

La pipeline realiza:

1. 📥 Checkout del repositorio.
2. ⚙️ Instalación/configuración de .NET.
3. 🧪 Build de la solución en modo Release.
4. 📦 Publicación del proyecto Web API.
5. 🗂️ Generación de artefacto.
6. 🔐 Login en Azure.
7. 🚀 Deploy automático a Azure App Service.

Workflow:

- .github/workflows/main_plataforma-autogestion-tpi.yml

---

## 🗄️ Persistencia

La persistencia se implementa con Entity Framework Core utilizando el enfoque Code First.

Incluye:

- ApplicationDbContext
- DbSets para entidades principales
- Configuración de relaciones
- Filtros globales por empresa
- Migraciones de EF Core
- Snapshot del modelo

### 📌 Entidades principales

- Company
- User
- Workday
- Liquidation
- DetailLiquidation

---

## 🧭 Endpoints Principales

### 🔐 Autenticación

POST /api/Auth/login

POST /api/Auth/mfa/setup (token temporal de configuración, SuperAdmin)

POST /api/Auth/mfa/verify (token temporal, SuperAdmin)

### 🔔 Avisos del Admin

GET /api/Notification (últimos avisos y cantidad sin leer)

PUT /api/Notification/{id}/leer

PUT /api/Notification/leer-todas

Los avisos se generan al cargar o modificar una jornada y se marcan como leídos solos cuando el Admin la aprueba o rechaza. Se muestran en la campana del panel, que consulta cada 30 segundos.

### 📊 Estadísticas

GET /api/estadisticas?month=10&year=2026 (Admin; sin parámetros usa el mes actual)

Devuelve totales del mes, costo estimado (horas aprobadas por el valor hora actual), tasa de aprobación, el detalle por empleado y la serie diaria de horas.

### 🏢 Empresas

GET /api/Company

POST /api/Company

GET /api/Company/{id}

PUT /api/Company/{id}

DELETE /api/Company/{id}

GET /api/Company/mi-empresa (Admin)

PUT /api/Company/valor-hora-feriado (Admin; sin valor vuelve al doble del valor hora)

GET /api/Company/historial-valores (Admin; cambios de valor hora, del más nuevo al más viejo)

GET /api/Company/tope-horas (Admin, Empleado)

PUT /api/Company/tope-horas (Admin)

PUT /api/Company/mi-empresa (Admin)

PUT /api/Company/valor-hora (Admin)

### 👥 Usuarios

GET /api/User/Mi Empresa

GET /api/User/company/{companyId}

GET /api/User/me

PUT /api/User/me

POST /api/User

PUT /api/User/empleado/{id} (Admin)

DELETE /api/User/{id}

### ⏱️ Jornadas

GET /api/Workday/mis-horas

GET /api/Workday/PendientesDeAprobacion

POST /api/Workday/cargar

PUT /api/Workday/aprobar/{id}

PUT /api/Workday/{id} (Empleado, jornada pendiente)

PUT /api/Workday/rechazar/{id} (body: { "motivoRechazo": "..." })

GET /api/Workday/Historial (Admin)

### 💰 Liquidaciones

GET /api/Liquidation/empleado/{userId}/simular

POST /api/Liquidation/simular

POST /api/Liquidation/cerrar-mes

GET /api/Liquidation/cierre-mes

DELETE /api/Liquidation/{id}

### 📑 Reportes

GET /api/reports/recibos/{id}

GET /api/reports/banco/{id}

### 🌎 Feriados

GET /api/Holidays/{year}

GET /api/Holidays/check?date=yyyy-MM-dd

---

## 🧪 Ejecución Local

1. Clonar el repositorio.

git clone https://github.com/Francesco-Dagostino/Plataforma-Autogestion.git

2. Restaurar dependencias.

dotnet restore

3. Configurar variables locales o appsettings.Development.json.

4. Ejecutar la API.

dotnet run --project PlataformaAutogestion.Api

5. Abrir Swagger.

https://localhost:{puerto}/swagger

---

## ✅ Cumplimiento de Requerimientos del TPI

- [x] 🧱 Proyectos organizados con Clean Architecture.
- [x] 🧩 Entidades de dominio declaradas.
- [x] 🗄️ Repositorios definidos e implementados.
- [x] ⚙️ Servicios definidos e implementados.
- [x] 🔌 Inyección de dependencias configurada en Program.cs.
- [x] 🧬 ApplicationDbContext configurado en infraestructura.
- [x] 📦 Migraciones y snapshot de EF Core presentes.
- [x] 🌐 Controladores definidos con servicios inyectados.
- [x] 🔐 Authentication Controller funcional.
- [x] 🛡️ Autenticación JWT funcional.
- [x] 🗄️ Persistencia operativa en base de datos relacional con EF Core Code First.
- [x] 🧰 Patrón Generic Repository implementado.
- [x] 🚀 Pipeline CI/CD con GitHub Actions hacia Azure.
- [x] 🌎 Consumo de servicio externo mediante HttpClientFactory.
- [x] 🔑 Secret JWT configurado mediante variables de entorno en Azure.
