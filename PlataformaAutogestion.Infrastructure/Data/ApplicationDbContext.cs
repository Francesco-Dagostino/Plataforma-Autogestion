using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using PlataformaAutogestion.Domain.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;

namespace PlataformaAutogestion.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        private readonly bool isTestingEnvironment;
        // Permite obtener información de la request actual, como usuario logueado y claims.
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options,
            // Acceso al HttpContext para leer datos del usuario autenticado.
            IHttpContextAccessor httpContextAccessor,
            bool isTestingEnvironment = false) : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
            this.isTestingEnvironment = isTestingEnvironment;
        }

        //Tablas
        public DbSet<Company> Companys { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Workday> Workdays { get; set; }
        public DbSet<Liquidation> Liquidations { get; set; }
        public DbSet<DetailLiquidation> Details { get; set; }

        // Método para extraer el IdCompany del token JWT.
        private int? GetCurrentCompanyId()
        {
            var claim = _httpContextAccessor.HttpContext?.User?.FindFirst("IdCompany")?.Value;
            // Muestra por consola la empresa detectada, útil para depuración.
            Console.WriteLine($">>> IdCompany del token: {claim}");
            // Convierte el claim a int; si no existe o no es válido, devuelve 0.
            return int.TryParse(claim, out var id) ? id : 0;
        }

        // Método para obtener el rol del usuario desde el token JWT.
        private string? GetCurrentRole()
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value;
        }

        //Entidades a tablas
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //filtros
            modelBuilder.Entity<User>().HasQueryFilter(u => GetCurrentRole() == "SuperAdmin" || u.IdCompany == GetCurrentCompanyId());
            // Filtra jornadas por la empresa del usuario logueado.
            modelBuilder.Entity<Workday>().HasQueryFilter(w => w.IdCompany == GetCurrentCompanyId());
            // Filtra liquidaciones por la empresa del usuario logueado.
            modelBuilder.Entity<Liquidation>().HasQueryFilter(l => l.IdCompany == GetCurrentCompanyId());
            // Filtra detalles de liquidación por la empresa del usuario logueado.
            modelBuilder.Entity<DetailLiquidation>().HasQueryFilter(dl => dl.IdCompany == GetCurrentCompanyId());

            //// Configuración de las entidades.
            modelBuilder.Entity<Company>(entity =>
            {
                entity.ToTable("Companys");
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Name).IsRequired().HasMaxLength(150);
                entity.Property(c => c.Cuit).IsRequired();
                entity.Property(c => c.DateHigh).IsRequired();
                entity.Property(c => c.ParameterSystem).IsRequired();
            });

            
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Name).IsRequired().HasMaxLength(100);
                entity.Property(u => u.Email).IsRequired().HasMaxLength(150);
                entity.Property(u => u.UserName).IsRequired().HasMaxLength(50);
                entity.Property(u => u.Password).IsRequired().HasMaxLength(255);
                entity.Property(u => u.CreationDate).IsRequired();
                entity.Property(u => u.role).IsRequired();

                // Relaciónes
                entity.HasOne(u => u.Company)
                    .WithMany(c => c.users)
                    .HasForeignKey(u => u.IdCompany)
                    .IsRequired(false);
            });


            modelBuilder.Entity<Workday>(entity =>
            {
                entity.ToTable("Workdays");
                entity.HasKey(w => w.Id);
                entity.Property(w => w.Id).HasMaxLength(50);
                entity.Property(w => w.HoursWorked).IsRequired();
                entity.Property(w => w.DateEntry).IsRequired();
                entity.Property(w => w.DateApproval).IsRequired(false);
                entity.Property(w => w.Estado).IsRequired();

                // Relaciónes
                entity.HasOne(w => w.Company)
                    .WithMany(c => c.workdays)
                    .HasForeignKey(w => w.IdCompany);

                // Relación con usuario.
                entity.HasOne(w => w.Usuario)
                    .WithMany(u => u.workdays)
                    .HasForeignKey(w => w.IdUser);
            });

            modelBuilder.Entity<Liquidation>(entity =>
            {
                entity.ToTable("Liquidations");
                entity.HasKey(l => l.Id);
                entity.Property(l => l.LiquidationDate).IsRequired();
                entity.Property(l => l.Total).IsRequired();

                // Relaciónes
                entity.HasOne(l => l.Company)
                    .WithMany(c => c.liquidations)
                    .HasForeignKey(l => l.IdCompany);
            });

            modelBuilder.Entity<DetailLiquidation>(entity =>
            {
                entity.ToTable("Details");
                entity.HasKey(dl => dl.Id);
                entity.Property(dl => dl.TotalHours).IsRequired();
                entity.Property(dl => dl.Amount).IsRequired();

                // Relaciónes
                entity.HasOne(dl => dl.Liquidation)
                    .WithMany(l => l.detailLiquidations)
                    .HasForeignKey(dl => dl.IdLiquidation);

                entity.HasOne(dl => dl.Company)
                    .WithMany(c => c.detailLiquidations)
                    .HasForeignKey(dl => dl.IdCompany);

                entity.HasOne(dl => dl.User)
                    .WithMany(u => u.detailLiquidations)
                    .HasForeignKey(dl => dl.IdUser);
            });

            // Inserta datos iniciales.
            modelBuilder.Entity<Company>().HasData(CreateCompanyDataSeed());
            modelBuilder.Entity<User>().HasData(CreateUserDataSeed());

           
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                // Evita borrados en cascada automáticos para no eliminar datos relacionados sin control.
                relationship.DeleteBehavior = DeleteBehavior.NoAction;
            }
        }

        //metodo para cargar empresas y usuarios
        private Company[] CreateCompanyDataSeed() { /* ... */ return new Company[] { }; }
        private User[] CreateUserDataSeed() { /* ... */ return new User[] { }; }
    }

    // comandos de migracion
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            // Obtiene la carpeta actual desde donde se ejecuta el comando.
            var directory = Directory.GetCurrentDirectory();
            // Busca la carpeta del proyecto API para encontrar appsettings.json.
            var apiPath = Path.Combine(directory, "PlataformaAutogestion.Api");
            // Si no encuentra la API en la carpeta actual, prueba una ruta relativa alternativa.
            if (!Directory.Exists(apiPath)) apiPath = Path.Combine(directory, "../PlataformaAutogestion.Api");

            // Lee appsettings.json para obtener la connection string.
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Path.GetFullPath(apiPath))
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            // Construye las opciones del DbContext.
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            // Configura SQL Server usando la connection string DefaultConnection.
            optionsBuilder.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"));

            // En migraciones no hay una request HTTP real, por eso no hay usuario logueado.
            return new ApplicationDbContext(optionsBuilder.Options, null!, false);
        }
    }
}
