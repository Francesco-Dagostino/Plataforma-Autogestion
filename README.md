# 🏢 Plataforma de Autogestión de Horas y Gestión de Nómina - Backend API

Este repositorio contiene el backend y la API REST del proyecto, desarrollado como el componente central del Trabajo Práctico Integrador (TPI) para Programación 4 y núcleo de software del Trabajo Integrador Final (TIF) de la Tecnicatura Universitaria en Programación.

La plataforma está diseñada para optimizar y digitalizar el registro de la jornada laboral en PYMES industriales, automatizando el circuito de validación/aprobación por parte de los administradores y procesando de manera transparente y segura el cálculo de la liquidación de haberes mensuales.

---

## 👥 Integrantes del Equipo
* Danilo Mercado
* Francesco D'agostino
* Facundo Nieva

---

## 🛠️ Arquitectura y Stack Tecnológico

El backend se encuentra estructurado bajo el patrón arquitectónico de Clean Architecture (Arquitectura Limpia), promoviendo la inyección de dependencias y el desacoplamiento absoluto de sus capas físicas para garantizar la mantenibilidad y escalabilidad del sistema.

### Capas del Proyecto:
1. PlataformaAutogestion.Domain: Contiene las entidades esenciales del negocio (Usuario, Empresa, JornadaLaboral, Liquidacion, DetalleLiquidacion), tipos enumerados (Roles, Estados) y contratos base independientes de cualquier framework o librería externa.
2. PlataformaAutogestion.Application: Alberga la lógica de negocio central, interfaces y contratos de servicios, DTOs (Data Transfer Objects), y las implementaciones de los casos de uso del sistema.
3. PlataformaAutogestion.Infrastructure: Implementa la persistencia de datos mediante Entity Framework Core (Code First), el contexto de base de datos (ApplicationDbContext), migraciones, mapeos y la concreción del patrón de diseño Generic Repository.
4. PlataformaAutogestion.Api: Capa de presentación web que expone los endpoints REST, configuración de arranque (Program.cs), middlewares de manejo de excepciones, seguridad y configuraciones de Swagger.

### Stack Técnico Principal:
* Lenguaje: C# / .NET 8.0
* Framework: ASP.NET Core Web API
* ORM: Entity Framework Core
* Base de Datos: PostgreSQL (Entorno local y producción) / Azure SQL Database
* Autenticación: JSON Web Tokens (JWT) con políticas basadas en roles (Claims)

---

## 🔑 Características Clave y Reglas de Negocio

* Aislamiento Multi-empresa (Multitenancy): Implementación de un filtro global de datos (HasQueryFilter) en la persistencia. Cada consulta a la base de datos se filtra automáticamente por el identificador de la organización (EmpresaId) del usuario autenticado, asegurando una separación estricta de la información.
* Roles de Acceso Diferenciados:
  * SuperAdmin: Gestión global de empresas y configuraciones de infraestructura.
  * Administrador (Dueño/RRHH): Gestión de empleados, ABM de tarifas/horas y aprobación de jornadas.
  * Operario: Carga diaria de jornadas laborales y consulta de historial.
* Circuito Inmutable de Jornadas: El operario declara sus horas; una vez que el administrador cambia el estado a Aprobado o Rechazado, el registro se vuelve inmutable bloqueando cualquier intento de edición para asegurar la transparencia de la nómina.
* Motor de Liquidación de Haberes: Procesamiento automatizado al cierre de mes que consolida las horas validadas de los trabajadores y computa los montos a transferir en función de las reglas preestablecidas.

---

## 📁 Estructura de la Solución

* PlataformaAutogestion.sln
* PlataformaAutogestion.Api/
  * Controllers/
  * Middlewares/
  * Program.cs
  * appsettings.json
* PlataformaAutogestion.Application/
  * Interfaces/
  * Services/
  * DTOS/
* PlataformaAutogestion.Infrastructure/
  * Context/
    * ApplicationDbContext.cs
  * Migrations/
  * Repositories/
    * GenericRepository.cs
* PlataformaAutogestion.Domain/
  * Entities/
  * Enums/

---

## 🚦 Cumplimiento de Requerimientos de Cátedra (Aprobación Directa)

Para cumplir estrictamente con las pautas de evaluación y promoción de la materia, la solución integra de forma obligatoria:
* [x] Inyección de Dependencias: Utilizada nativamente en todas las capas para desacoplar controladores, servicios y repositorios.
* [x] Patrón Generic Repository: Abstracción unificada del acceso a datos para operaciones CRUD recurrentes.
* [x] Persistencia Real: Modelado Code-First implementado sobre un motor relacional multiusuario (PostgreSQL / Azure SQL), sin el uso de bases de datos InMemory o SQLite.
* [x] Pipeline de CI/CD: Despliegue automatizado continuo mediante GitHub Actions hacia servicios web en Azure.
* [x] Consumo de Servicios Externos: Implementado mediante HttpClientFactory en endpoints dedicados.
* [x] Seguridad Avanzada: Almacenamiento seguro de claves de firma JWT en Variables de Entorno de Azure / Azure Key Vault.

