using Microsoft.EntityFrameworkCore;
using PlataformaAutogestion.Application.Interfaces;
using PlataformaAutogestion.Application.Services;
using PlataformaAutogestion.Domain.Interfaces;
using PlataformaAutogestion.Infrastructure.Data;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// 1. Agregar servicios para los controladores y la generación de Swagger
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    }); ;

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); // Agrega el generador de Swagger

//Postgress Conexion
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));


#region Repositories
builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
#endregion

#region Services
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<IUserService, UserService>();
#endregion

var app = builder.Build();

// 2. Configurar el pipeline HTTP para que muestre Swagger en desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();