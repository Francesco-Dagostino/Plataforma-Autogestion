# 📋 Tablero Kanban Integrado: TPI Programación 4 & TIF (Clean Architecture)

> **Regla de oro del equipo:** Trabajo secuencial por etapas. Todo feature debe respetar el flujo: `Controller` -> `IService` -> `Service`.

---

## 🛠️ ETAPA 1: Los Cimientos y el Modelo de Datos (Días 1 a 3)
*Objetivo: Dejar el repositorio limpio, la base de datos local conectada y el esquema de tablas funcionando.*

- [x] **Tarjeta 1: Limpieza Total de la Solución**
  - **Responsable:** Danilo Mercado
  - **Capa:** API / Application / Infrastructure
  - *Descripción:* Eliminar archivos por defecto (`Class1.cs`, `WeatherForecastController`). Dejar las capas limpias.

- [x] **Tarjeta 2: DbContext y Repositorio Genérico (Interfaces)**
  - **Responsable:** Francesco D'agostino
  - **Capa:** Domain / Infrastructure / API
  - *Descripción:* Crear `ApplicationDbContext`. Crear la interfaz `IBaseRepository<T>` en Domain y su implementación `BaseRepository<T>` en Infrastructure. Inyectar en `Program.cs`.

- [x] **Tarjeta 3: Configuración de Entidades con Fluent API**
  - **Responsable:** Facundo Nieva
  - **Capa:** Domain / Infrastructure
  - *Descripción:* Escribir mapeo de `User`, `Company`, `Workday`, `Liquidation`, `DetailLiquidation`. Definir PKs, FKs y `DeleteBehavior.NoAction`.

- [x] **Tarjeta 4: Primera Migración Base**
  - **Responsable:** Danilo Mercado
  - **Capa:** Infrastructure / BD
  - *Descripción:* Correr `Add-Migration InitialCreate` y `Update-Database` para impactar BD. Excluir `appsettings.json` de Git y crear `appsettings.Example.json`.

- [x] **Tarjeta 5: Seed Data Inicial**
  - **Responsable:** Francesco D'agostino
  - **Capa:** Infrastructure / BD
  - *Descripción:* Configurar `HasData` en `ApplicationDbContext` para empresa y usuario SuperAdmin iniciales. Correr migración `SeedData`.

---

## 🏢 ETAPA 2: Módulo Empresas y Empleados (Días 4 a 6)
*Objetivo: CRUD completo de empresas y usuarios con sus validaciones.*

- [x] **Tarjeta 6: Arquitectura Gestión de Empresas (Módulo 1)**
  - **Responsable:** Francesco D'agostino
  - **Capa:** Application / API
  - *Descripción:* 1. Crear `ICompanyRepository` y `CompanyRepository`.
    2. Crear interfaz `ICompanyService` en Application.
    3. Crear `CompanyService` con DTOs (`CompanyDTO`, `CompanyCreateRequest`) y excepciones custom.
    4. Crear `CompanyController` con CRUD completo.

- [x] **Tarjeta 7: Arquitectura Gestión de Empleados (Módulo 2)**
  - **Responsable:** Facundo Nieva
  - **Capa:** Application / API
  - *Descripción:* 1. Crear `IUserRepository` y `UserRepository` con métodos `GetAllByCompanyAsync`, `GetByEmailAsync`, `GetByUserNameAsync`.
    2. Crear interfaz `IUserService` en Application.
    3. Crear `UserService` con DTOs (`UserDTO`, `UserCreateRequest`). Rol siempre `Empleado` al crear.
    4. Crear `UserController` con CRUD completo y endpoint `GET /company/{companyId}`.

- [x] **Tarjeta 8: Serialización de Enums como String**
  - **Responsable:** Francesco D'agostino
  - **Capa:** API
  - *Descripción:* Configurar `JsonStringEnumConverter` en `Program.cs` para que `Roles` y `StatusDay` se serialicen como string en toda la API.

---

## 🔒 ETAPA 3: Seguridad y Filtro Multi-Empresa (Días 7 a 9)
*Objetivo: Autenticación JWT, autorización por roles y aislamiento de datos por empresa.*

- [x] **Tarjeta 9: Arquitectura de Autenticación (Módulo 6)**
  - **Responsable:** Danilo Mercado
  - **Capa:** Application / API
  - *Descripción:* 1. Crear interfaz `IAuthService` en Application.
    2. Crear `AuthService` con lógica de verificación de credenciales y generación de JWT.
    3. Crear `AuthController` con endpoints `POST /api/auth/login` y `POST /api/auth/recuperar-password`.
    4. Registrar JWT en `Program.cs` con `AddAuthentication` y `AddJwtBearer`.

- [x] **Tarjeta 10: Middleware de Autorización por Roles**
  - **Responsable:** Francesco D'agostino
  - **Capa:** API
  - *Descripción:* Agregar `[Authorize]` y `[Authorize(Roles = "...")]` en controllers según corresponda. `CompanyController` solo SuperAdmin. `UserController` solo Admin. Agregar `app.UseAuthentication()` en `Program.cs`.

- [x] **Tarjeta 11: Filtro Global Multi-Empresa (Multitenancy)**
  - **Responsable:** Francesco D'agostino
  - **Capa:** Infrastructure
  - *Descripción:* Inyectar `IHttpContextAccessor` en `ApplicationDbContext`. Configurar `HasQueryFilter` en `User`, `Workday`, `Liquidation` y `DetailLiquidation` para filtrar automáticamente por `IdCompany` del token JWT.

---

## ⏱️ ETAPA 4: Módulo Jornadas (Días 10 a 11)
*Objetivo: Carga de horas por operarios, validación de topes y aprobación por admin.*

- [x] **Tarjeta 12: Arquitectura Lógica de Jornadas (Módulo 3 - Reglas)**
  - **Responsable:** Facundo Nieva
  - **Capa:** Application
  - *Descripción:* Crear `IWorkdayRepository` y `WorkdayRepository`. Crear interfaz `IWorkdayService` y `WorkdayService`. Validar tope de horas diarias contra `ParameterSystem` de la empresa. Inmutabilidad si estado es `Aprobada` o `Desaprobada`.

- [x] **Tarjeta 13: Controladores de Jornadas (Módulo 3 - Endpoints)**
  - **Responsable:** Danilo Mercado
  - **Capa:** API
  - *Descripción:* Crear `WorkdayController`. Endpoints para Operario: `POST /api/workday/cargar`, `GET /api/workday/mis-horas`. Endpoints para Admin: `GET /api/workday/pendientes`, `PUT /api/workday/aprobar/{id}`, `PUT /api/workday/rechazar/{id}`.

---

## 💰 ETAPA 5: Liquidación, Reportes y Servicios Externos (Días 12 a 13)
*Objetivo: Cierre mensual, consumo de API externa y generación de archivos.*

- [x] **Tarjeta 14: Integración API Externa de Feriados (HttpClientFactory)**
  - **Responsable:** Francesco D'agostino
  - **Capa:** Infrastructure / Application
  - *Descripción:* Crear `IFeriadoService` en Application. Implementar en Infrastructure consumiendo la API pública de feriados de Argentina con `HttpClientFactory`. Registrar en `Program.cs`.

- [x] **Tarjeta 15: Arquitectura de Liquidaciones (Módulo 4)**
  - **Responsable:** Danilo Mercado
  - **Capa:** Application / API
  - *Descripción:* 1. Crear `ILiquidationRepository` y `LiquidationRepository`.
    2. Crear `ILiquidationService` y `LiquidationService` con algoritmo de cálculo cruzando horas trabajadas, valor hora y feriados.
    3. Crear `LiquidationController` con endpoints `POST /api/liquidation/simular` y `POST /api/liquidation/cerrar-mes`.

- [x] **Tarjeta 16: Arquitectura de Reportes (Módulo 5)**
  - **Responsable:** Facundo Nieva
  - **Capa:** Application / API
  - *Descripción:* 1. Crear `IReporteService` y `ReporteService` (generación de PDF y archivo TXT de lote bancario).
    2. Crear `ReportesController` con endpoints `GET /api/reportes/recibos/{id}` y `GET /api/reportes/banco/{id}`.


---

## 🚀 ETAPA 6: Deploy y Puesta en Producción (Día 14)
*Objetivo: Dejar la API en internet lista para la defensa oral ante los profesores.*

- [x] **Tarjeta 17: Provisión de Infraestructura Base en Azure**
  - **Responsable:** Facundo Nieva
  - **Capa:** DevOps
  - *Descripción:* Crear el Resource Group dedicado al TP. Dentro de él, crear el App Service Plan y el App Service (Web App) que va a hostear la API. Este recurso es el destino del deploy de la Tarjeta 20.

- [x] **Tarjeta 18: Base de Datos en Azure**
  - **Responsable:** Facundo Nieva
  - **Capa:** DevOps / BD
  - *Descripción:* Levantar Azure Database for PostgreSQL (Flexible Server) dentro del mismo Resource Group de la Tarjeta 17. Correr las migraciones de EF Core contra la instancia de producción (`dotnet ef database update` con la connection string de Azure).

- [x] **Tarjeta 19: Variables de Entorno y Key Vault**
  - **Responsable:** Danilo Mercado
  - **Capa:** DevOps / API
  - *Descripción:* Configurar como Application Settings del App Service: la `ConnectionString` de la base de la Tarjeta 18 y la `Jwt:Key`. Idealmente, alojar la `Jwt:Key` en un Azure Key Vault y referenciarla desde el App Service (Key Vault reference). Como mínimo aceptable, variable de entorno directa en el App Service.

- [ ] **Tarjeta 20: Pipeline de Automatización (CI/CD)**
  - **Responsable:** Francesco D'agostino
  - **Capa:** DevOps (GitHub Actions)
  - *Descripción:* Escribir el workflow `.yml` que, al pushear a `main`, compile el proyecto, publique el artefacto y lo despliegue al App Service de la Tarjeta 17 (vía `azure/webapps-deploy` con publish profile, o `az webapp deploy` con Service Principal). Credenciales como GitHub Secrets. Confirmar al menos una corrida en verde en la pestaña Actions.

- [ ] **Tarjeta 21: Validación End-to-End en Producción**
  - **Responsable:** Equipo completo
  - **Capa:** QA
  - *Descripción:* Probar contra la URL pública de Azure: login y generación de JWT, Swagger accesible, CRUD de Workday/Liquidation funcionando contra la base real, y que el consumo de la API de feriados (HolidayService) responda correctamente. Confirmar que el HttpClient de HolidayService está registrado vía `AddHttpClient` en `Program.cs` (HttpClientFactory), no instanciado manualmente..
