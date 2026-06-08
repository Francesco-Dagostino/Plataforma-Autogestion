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
  - *Descripción:* Crear `ApplicationDbContext`. Crear la interfaz `IRepository<T>` en Domain y su implementación `Repository<T>` en Infrastructure. Inyectar en `Program.cs`.

- [x] **Tarjeta 3: Configuración de Entidades con Fluent API**
  - **Responsable:** Facundo Nieva
  - **Capa:** Domain / Infrastructure
  - *Descripción:* Escribir mapeo de `Usuario`, `Empresa`, `JornadaLaboral`, `Liquidacion`. Definir PKs y FKs.

- [x] **Tarjeta 4: Primera Migración Base**
  - **Responsable:** Danilo Mercado
  - **Capa:** Infrastructure / BD
  - *Descripción:* Correr `Add-Migration InitialCreate` y `Update-Database` para impactar BD.

---

## 🔒 ETAPA 2: Seguridad, Filtro Global y Módulo Empresas (Días 4 a 6)
*Objetivo: Lógica de aislamiento multi-empresa, seguridad JWT y gestión inicial.*

- [ ] **Tarjeta 5: Filtro Global Multi-Empresa (Multitenancy)**
  - **Responsable:** Francesco D'agostino
  - **Capa:** Infrastructure
  - *Descripción:* Configurar `HasQueryFilter` en el DbContext para aislar datos por `EmpresaId`.

- [ ] **Tarjeta 6: Arquitectura de Autenticación (Módulo 6)**
  - **Responsable:** Danilo Mercado
  - **Capa:** Application / API
  - *Descripción:* 1. Crear interfaz `IAuthService` en Application.
    2. Crear clase `AuthService` (lógica de hash y generación JWT).
    3. Crear `AuthController` en API con endpoints `/login`, `/recuperar-password` y `/mfa/verificar`.

- [x] **Tarjeta 7: Arquitectura Gestión de Empresas (Módulo 1)**
  - **Responsable:** Francesco D'agostino
  - **Capa:** Application / API
  - *Descripción:* 1. Crear interfaz `IEmpresaService` en Application.
    2. Crear clase `EmpresaService` (CRUD de empresas).
    3. Crear `EmpresasController` en API (uso exclusivo SuperAdmin).

---

## ⏱️ ETAPA 3: Módulo Operarios y Fichajes (Días 7 a 9)
*Objetivo: Carga de empleados, control de horas de operarios y validación.*

- [ ] **Tarjeta 8: Arquitectura Gestión de Empleados (Módulo 2)**
  - **Responsable:** Facundo Nieva
  - **Capa:** Application / API
  - *Descripción:* 1. Crear interfaz `IUsuarioService` en Application.
    2. Crear clase `UsuarioService` (Alta, baja, modificación y roles).
    3. Crear `UsuariosController` en API (`/api/usuarios`).

- [ ] **Tarjeta 9: Arquitectura Lógica de Jornadas (Módulo 3 - Reglas)**
  - **Responsable:** Facundo Nieva
  - **Capa:** Application
  - *Descripción:* Crear interfaz `IJornadaService` y clase `JornadaService`. Programar validación de topes de horas e inmutabilidad si el estado es aprobado/rechazado.

- [ ] **Tarjeta 10: Controladores de Jornadas (Módulo 3 - Endpoints)**
  - **Responsable:** Danilo Mercado
  - **Capa:** API
  - *Descripción:* Inyectar `IJornadaService` en un nuevo `JornadasController`. Crear endpoints para el Operario (`/cargar`, `/mis-horas`) y para el Admin (`/pendientes`, `/aprobar`, `/rechazar`).

---

## 💰 ETAPA 4: Liquidación, Reportes y Servicios de Terceros (Días 10 a 12)
*Objetivo: Cierre mensual, consumo de APIs externas y salidas de archivos.*

- [ ] **Tarjeta 11: Integración API Externa (HttpClientFactory)**
  - **Responsable:** Francesco D'agostino
  - **Capa:** Infrastructure / Application
  - *Descripción:* Crear `IFeriadoService` y consumir la API de feriados de Argentina usando `HttpClientFactory` (Requisito Prog 4).

- [ ] **Tarjeta 12: Arquitectura de Liquidaciones (Módulo 4)**
  - **Responsable:** Danilo Mercado
  - **Capa:** Application / API
  - *Descripción:* 1. Crear interfaz `ILiquidacionService` y `LiquidacionService` (algoritmo de cálculo cruzando horas y feriados).
    2. Crear `LiquidacionesController` con endpoints `/simular` y `/cerrar-mes`.

- [ ] **Tarjeta 13: Arquitectura de Reportes (Módulo 5)**
  - **Responsable:** Facundo Nieva
  - **Capa:** Application / API
  - *Descripción:* 1. Crear `IReporteService` y `ReporteService` (lógica de generación de PDF y Lote TXT).
    2. Crear `ReportesController` con endpoints `/recibos/{id}` y `/banco/{id}`.

- [ ] **Tarjeta 14: Arquitectura Mantenimiento (Módulo 1)**
  - **Responsable:** Francesco D'agostino
  - **Capa:** Application / API
  - *Descripción:* Crear `ISistemaService` y `SistemaController` para exponer el endpoint `POST /api/sistema/backup`.

---

## 🚀 ETAPA 5: Deploy y Puesta en Producción (Días 13 a 14)
*Objetivo: Dejar la API en internet lista para la defensa oral ante los profesores.*

- [ ] **Tarjeta 15: Servidor de Datos en Azure**
  - **Responsable:** Facundo Nieva
  - **Capa:** DevOps / BD
  - *Descripción:* Levantar SQL Server/MySQL en Azure Cloud y correr migraciones en producción.

- [ ] **Tarjeta 16: Pipeline de Automatización (CI/CD)**
  - **Responsable:** Francesco D'agostino
  - **Capa:** DevOps (GitHub Actions)
  - *Descripción:* Escribir el workflow `.yml` para despliegue automático en Azure App Service al pushear en `main`.

- [ ] **Tarjeta 17: Variables de Entorno y Key Vault**
  - **Responsable:** Danilo Mercado
  - **Capa:** DevOps / API
  - *Descripción:* Ocultar secrets (JWT Key, ConnectionString) usando las Variables de Entorno de Azure o Azure Key Vault.
