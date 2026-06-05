using Microsoft.EntityFrameworkCore;
using PlataformaAutogestion.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. Agregar servicios para los controladores y la generación de Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); // Agrega el generador de Swagger

//Postgress Conexion
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));


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