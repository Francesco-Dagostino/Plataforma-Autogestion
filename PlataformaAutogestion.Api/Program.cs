using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;
using PlataformaAutogestion.Api.Middleware;
using PlataformaAutogestion.Application.Interfaces;
using PlataformaAutogestion.Application.Services;
using PlataformaAutogestion.Domain.Interfaces;
using PlataformaAutogestion.Infrastructure.Data;
using PlataformaAutogestion.Infrastructure.Services;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Registramos el servicio necesario para que el DbContext acceda al HttpContext
builder.Services.AddHttpContextAccessor();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(setupAction =>
{
    setupAction.AddSecurityDefinition("PlataformaAutogestionBearerAuth",
        new OpenApiSecurityScheme()
        {
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            Description = "Acá pegar el token generado al loguearse."
        });

    setupAction.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "PlataformaAutogestionBearerAuth"
                    }
                },
                new List<string>()
            }
        });
});

string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!; //conexcion a mi bd, declarada en appseting.json


builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["AutenticacionService:Issuer"],
            ValidAudience = builder.Configuration["AutenticacionService:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.ASCII.GetBytes(builder.Configuration["AutenticacionService:SecretForKey"]!))
        };
    })
    // Esquema aparte para los tokens temporales de MFA: otra audiencia, así el esquema
    // "Bearer" de la API de negocio los rechaza y estos solo sirven en /api/auth/mfa/*.
    .AddJwtBearer("MfaBearer", options =>
    {
        options.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["AutenticacionService:Issuer"],
            ValidAudience = builder.Configuration["AutenticacionService:Audience"] + AuthService.MfaAudienceSuffix,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.ASCII.GetBytes(builder.Configuration["AutenticacionService:SecretForKey"]!))
        };
    });

#region Repositories
builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IWorkdayRepository, WorkdayRepository>();
builder.Services.AddScoped<ILiquidationRepository, LiquidationRepository>();
builder.Services.AddScoped<IDetailLiquidationRepositoy, DetailLiquidationRepository>();
builder.Services.AddScoped<IPasswordResetRepository, PasswordResetRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<IRateHistoryRepository, RateHistoryRepository>();
#endregion

#region Services
builder.Services.Configure<AuthServiceOptions>(
    builder.Configuration.GetSection(
        AuthServiceOptions.AutenticacionService));
// Tarjeta 14 de api :v
builder.Services.AddHttpClient<IHolidayService, HolidayService>(client =>
{
    var holidayApiConfig = builder.Configuration.GetSection("HolidayApi");

    client.BaseAddress = new Uri(
        holidayApiConfig["BaseUrl"]!);

    client.Timeout = TimeSpan.FromSeconds(
        int.Parse(holidayApiConfig["TimeoutSeconds"]!));

    client.DefaultRequestHeaders.Add(
        "User-Agent",
        "PlataformaAutogestion");

    client.DefaultRequestHeaders.Add(
        "Accept",
        "application/json");
});

builder.Services.AddSingleton<MfaSecretProtector>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IWorkdayService, WorkdayService>();
builder.Services.AddScoped<ILiquidationService, LiquidationService>();
builder.Services.AddScoped<IDetailLiquidationService, DetailLiquidationService>();
builder.Services.AddScoped<IReporteService, ReporteService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IStatisticsService, StatisticsService>();

builder.Services.Configure<EmailServiceOptions>(
    builder.Configuration.GetSection(EmailServiceOptions.EmailService));
builder.Services.AddScoped<IEmailService, EmailService>();

#endregion

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        var origins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
            ?? (builder.Environment.IsDevelopment() ? new[] { "http://localhost:5173" } : Array.Empty<string>());
        var frontendUrl = builder.Configuration["FrontendUrl"];
        if (!string.IsNullOrWhiteSpace(frontendUrl))
            origins = origins.Append(frontendUrl.TrimEnd('/')).ToArray();

        if (origins.Length > 0)
            policy.WithOrigins(origins).AllowAnyHeader().AllowAnyMethod();
    });
});

var app = builder.Build();

// Asegura que la tabla para recuperación de contraseñas exista en la base de datos
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.ExecuteSqlRaw(@"
        CREATE TABLE IF NOT EXISTS ""PasswordResetTokens"" (
            ""Id"" integer GENERATED BY DEFAULT AS IDENTITY,
            ""UserId"" integer NOT NULL,
            ""TokenHash"" character varying(128) NOT NULL,
            ""ExpiresAt"" timestamp with time zone NOT NULL,
            ""CreatedAt"" timestamp with time zone NOT NULL,
            ""Used"" boolean NOT NULL,
            CONSTRAINT ""PK_PasswordResetTokens"" PRIMARY KEY (""Id""),
            CONSTRAINT ""FK_PasswordResetTokens_Users_UserId"" FOREIGN KEY (""UserId"") REFERENCES ""Users"" (""Id"") ON DELETE CASCADE
        );
        CREATE INDEX IF NOT EXISTS ""IX_PasswordResetTokens_TokenHash"" ON ""PasswordResetTokens"" (""TokenHash"");
        CREATE INDEX IF NOT EXISTS ""IX_PasswordResetTokens_UserId"" ON ""PasswordResetTokens"" (""UserId"");
    ");
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionMiddleware>();
// Render termina TLS y redirige HTTP a HTTPS antes de enviar la peticion al contenedor.
if (!string.Equals(builder.Configuration["RENDER"], "true", StringComparison.OrdinalIgnoreCase))
    app.UseHttpsRedirection();

app.UseCors("Frontend");


app.UseAuthentication();
app.UseAuthorization();
app.MapGet("/health", () => Results.Ok(new { status = "ok" })).AllowAnonymous();
app.MapControllers();
app.Run();
