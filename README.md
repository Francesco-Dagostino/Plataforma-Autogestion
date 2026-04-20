# Plataforma de Autogestión de Horas y Gestión de Nómina

Este proyecto es una API REST Multitenant desarrollada como Trabajo Práctico Integrador (TPI) para la materia Programación 4 de la Tecnicatura Universitaria en Programación (UTN FRRO). El sistema tiene como objetivo principal gestionar la asistencia y las jornadas laborales de los empleados, y generar sus liquidaciones mensuales de forma automática. Permite a la empresa llevar un control estricto de las horas trabajadas, los estados de aprobación y el cálculo de montos a pagar, eliminando errores manuales y centralizando la información laboral.

## 🚀 Tecnologías Utilizadas

* **Backend:** C# .NET 10
* **Base de Datos:** PostgreSQL
* **Arquitectura:** Clean Architecture y esquema Multitenant (filtrado por `EmpresaId`)
* **Seguridad:** Autenticación mediante JWT y recuperación de contraseña por token
* **Documentación:** Swagger / OpenAPI

## 👥 Actores y Roles del Sistema

* **SuperAdministrador:** Gestión de cuentas de Administradores, backups de la base de datos y configuración de parámetros globales.
* **Administrador:** Gestión de usuarios, configuración de parámetros del sistema y supervisión de las liquidaciones de los empleados.
* **Usuario/Empleado (Operario autogestión):** Registro de asistencia y jornadas laborales.

## ⚙️ Funcionalidades Principales

* **Gestión de Empresa:** Alta, modificación de datos (CUIT, nombre, fechas) y relación con usuarios, jornadas y liquidaciones.
* **Gestión de Usuarios:** Alta, baja y modificación de empleados y administradores, asignación de rol y asociación a una empresa.
* **Jornadas Laborales:** Carga de fecha y horas trabajadas. Los estados pueden ser Pendiente, Aprobada o Rechazada.
* **Liquidaciones:** Creación de liquidación mensual por empresa con cálculo automático del total en base al valor hora y las horas registradas.

## 📋 Reglas de Negocio y Restricciones

* Un usuario pertenece a una única empresa.
* Las jornadas de trabajo deben estar aprobadas por un administrador para ser liquidadas.
* No se pueden modificar jornadas que ya han sido aprobadas.
* No se puede generar una liquidación sin jornadas aprobadas previamente.
* El sistema valida que no se registren más horas que el tope diario permitido por la empresa.
* Filtrado obligatorio por `EmpresaId` en todos los endpoints para garantizar la seguridad Multitenant.

## 💻 Estructura del Proyecto (Clean Architecture)

La solución está dividida en cuatro capas principales:

- `PlataformaAutogestion.Domain`: Entidades principales (`Usuario`, `Empresa`, `JornadaLaboral`, `Liquidacion`, `ParametroSistema`, `DetalleLiquidacion`) e interfaces core.
- `PlataformaAutogestion.Application`: Lógica de negocio, casos de uso, DTOs e interfaces de los repositorios.
- `PlataformaAutogestion.Infrastructure`: Implementación de Entity Framework Core con PostgreSQL y servicios externos.
- `PlataformaAutogestion.Api`: Controladores REST, configuración de inyección de dependencias, JWT y Swagger.

## 🚀 Cómo ejecutar el proyecto localmente

1. Clonar el repositorio.
2. Asegurarse de tener instalado el SDK de .NET 10 y PostgreSQL en el equipo.
3. Configurar la cadena de conexión a la base de datos en el archivo `appsettings.json` dentro de `PlataformaAutogestion.Api`.
4. Ejecutar las migraciones de Entity Framework para generar el esquema en PostgreSQL:
   ```bash
   dotnet ef database update --project PlataformaAutogestion.Infrastructure --startup-project PlataformaAutogestion.Api
