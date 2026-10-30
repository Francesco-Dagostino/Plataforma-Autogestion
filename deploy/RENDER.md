# API .NET en Render

Crear un **Web Service** desde el repositorio `Plataforma-Autogestion`.

| Campo | Valor |
| --- | --- |
| Branch | `main` |
| Language / Runtime | `Docker` |
| Root Directory | vacio (raiz del repositorio) |
| Dockerfile Path | `./Dockerfile` |
| Docker Build Context | `.` |
| Docker Command | vacio (usa el CMD del Dockerfile) |
| Health Check Path | `/health` |

El contenedor usa .NET 10, escucha en `0.0.0.0:$PORT` e incluye fuentes
para QuestPDF. `/health` comprueba que el servidor responde; no ejecuta
una consulta de disponibilidad de la base de datos.

## Variables en Environment

Configurar los secretos directamente en Render, nunca en Git.

| Nombre | Valor |
| --- | --- |
| `ASPNETCORE_ENVIRONMENT` | `Production` |
| `ConnectionStrings__DefaultConnection` | Cadena Npgsql de Supabase, ver abajo |
| `AutenticacionService__SecretForKey` | Secreto aleatorio de al menos 32 bytes, representado en Base64 |
| `AutenticacionService__Issuer` | `PlataformaAutogestion` |
| `AutenticacionService__Audience` | `PlataformaAutogestion` |
| `Mfa__EncryptionKey` | Otra clave aleatoria de exactamente 32 bytes, representada en Base64 |
| `HolidayApi__BaseUrl` | `https://api.argentinadatos.com/v1/feriados/` |
| `HolidayApi__TimeoutSeconds` | `10` |
| `FrontendUrl` | URL HTTPS final de Vercel, sin barra final; agregar cuando exista |

Render define `RENDER=true` automaticamente. La API deja que Render gestione
la redireccion HTTPS. En desarrollo local conserva su redireccion habitual.
Para permitir otros origenes explicitos se puede usar `Cors__AllowedOrigins__0`,
`Cors__AllowedOrigins__1`, etc. Sin origen configurado, Production no habilita CORS.

### Conexion Supabase

Obtener **Connect > Session pooler** (puerto 5432, compatible con IPv4).
Copiar host y usuario exactos del panel: no deducirlos del ID del proyecto.
La API usa el formato Npgsql, no una URL `postgresql://`:

```text
Host=HOST_DEL_SESSION_POOLER;Port=5432;Database=postgres;Username=USUARIO_DEL_PANEL;Password=PASSWORD_DE_LA_BASE;SSL Mode=Require
```

Es la contrasena de PostgreSQL, no la contrasena de la cuenta Supabase ni una API key.
Si contiene punto y coma o comillas, usar el escapado de NpgsqlConnectionStringBuilder.
El acceso servidor debe tener permisos adecuados; las tablas tienen RLS sin politicas
para clientes anonimos. El usuario postgres del pooler puede operar con ellas.
Para una conexion TLS con verificacion completa del certificado, configurar la CA
de Supabase y `SSL Mode=VerifyFull` segun la documentacion de Npgsql/Supabase.

## Correo

El servicio actual usa SMTP STARTTLS. En un plan que permita SMTP, configurar:

- `EmailService__SmtpHost`
- `EmailService__SmtpPort` = `587`
- `EmailService__SenderEmail`
- `EmailService__SenderPassword`
- `EmailService__SenderName` = `Jornada`

**Render Free bloquea los puertos SMTP 25, 465 y 587.** Para correo real en Free
hay que integrar un proveedor mediante HTTPS; alternativamente usar un servicio
de pago que permita el puerto 587. Sin credenciales, el codigo actual simula
el envio: no considerar validada la recuperacion de contrasena en ese caso.

## Despues del despliegue

1. Abrir `https://NOMBRE.onrender.com/health`: debe devolver `{"status":"ok"}`.
2. Verificar acceso a Supabase y crear el primer SuperAdmin mediante un proceso controlado.
3. Probar login, MFA y generacion de recibos PDF en Linux.
4. En Vercel configurar `VITE_API_URL=https://NOMBRE.onrender.com/api`.
5. Completar `FrontendUrl` en Render y probar el circuito de recuperacion por correo.

Las migraciones se aplican por separado con el SQL generado; este Dockerfile no
ejecuta migraciones automaticamente. El arranque conserva el bloque existente
que asegura la tabla PasswordResetTokens. Swagger solo esta activo en Development.
