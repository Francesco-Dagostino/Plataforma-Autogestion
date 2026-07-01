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
- 🛡️ Autorización por roles:
  - SuperAdmin
  - Admin
  - Empleado
- ⏱️ Carga de jornadas laborales.
- ✅ Aprobación y rechazo de jornadas.
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
- ⏳ Las jornadas cargadas quedan en estado pendiente hasta ser aprobadas o rechazadas.
- ✅ Solo las jornadas aprobadas participan en el cierre mensual.
- 📅 Una empresa no puede tener más de una liquidación cerrada para el mismo mes y año.
- 🔁 Para rehacer una liquidación mensual, primero debe anularse la liquidación existente.
- 🔎 Las consultas están filtradas por empresa mediante filtros globales en ApplicationDbContext.

---

## 🔐 Seguridad

La API utiliza autenticación JWT Bearer. El token incluye claims de identificación, rol e IdCompany, permitiendo controlar el acceso a los endpoints protegidos.

El secret utilizado para firmar los tokens JWT se configura mediante variables de entorno en Azure App Service.

### 🔑 Variables principales

- AutenticacionService__SecretForKey
- AutenticacionService__Issuer
- AutenticacionService__Audience

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

### 🏢 Empresas

GET /api/Company

POST /api/Company

GET /api/Company/{id}

PUT /api/Company/{id}

DELETE /api/Company/{id}

### 👥 Usuarios

GET /api/User/Mi Empresa

GET /api/User/company/{companyId}

GET /api/User/me

PUT /api/User/me

POST /api/User

DELETE /api/User/{id}

### ⏱️ Jornadas

GET /api/Workday/mis-horas

GET /api/Workday/PendientesDeAprobacion

POST /api/Workday/cargar

PUT /api/Workday/aprobar/{id}

PUT /api/Workday/rechazar/{id}

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
