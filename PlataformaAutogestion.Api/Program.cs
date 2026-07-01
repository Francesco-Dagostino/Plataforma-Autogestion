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

string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;


builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString, sqlOptions => sqlOptions.EnableRetryOnFailure()));

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
    });

#region Repositories
builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IWorkdayRepository, WorkdayRepository>();
builder.Services.AddScoped<ILiquidationRepository, LiquidationRepository>();
builder.Services.AddScoped<IDetailLiquidationRepositoy, DetailLiquidationRepository>();
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

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IWorkdayService, WorkdayService>();
builder.Services.AddScoped<ILiquidationService, LiquidationService>();
builder.Services.AddScoped<IDetailLiquidationService, DetailLiquidationService>();
builder.Services.AddScoped<IReporteService, ReporteService>();

#endregion

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionMiddleware>();
app.UseHttpsRedirection();

app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());


app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();