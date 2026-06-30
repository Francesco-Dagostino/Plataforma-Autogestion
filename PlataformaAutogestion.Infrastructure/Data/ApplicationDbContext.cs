using Microsoft.AspNetCore.Http; // <-- AGREGADO
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
        private readonly IHttpContextAccessor _httpContextAccessor; // <-- AGREGADO

        // Modificado constructor para recibir IHttpContextAccessor
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options,
            IHttpContextAccessor httpContextAccessor, // <-- AGREGADO
            bool isTestingEnvironment = false) : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
            this.isTestingEnvironment = isTestingEnvironment;
        }

        public DbSet<Company> Companys { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Workday> Workdays { get; set; }
        public DbSet<Liquidation> Liquidations { get; set; }
        public DbSet<DetailLiquidation> Details { get; set; }

        // Método auxiliar para extraer el IdCompany del token JWT
        private int? GetCurrentCompanyId()
        {
            var claim = _httpContextAccessor.HttpContext?.User?.FindFirst("IdCompany")?.Value;
            Console.WriteLine($">>> IdCompany del token: {claim}");
            return int.TryParse(claim, out var id) ? id : 0;
        }
        private string? GetCurrentRole()
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de Filtros Globales
            modelBuilder.Entity<User>().HasQueryFilter(u => GetCurrentRole() == "SuperAdmin" || u.IdCompany == GetCurrentCompanyId());
            modelBuilder.Entity<Workday>().HasQueryFilter(w => w.IdCompany == GetCurrentCompanyId());
            modelBuilder.Entity<Liquidation>().HasQueryFilter(l => l.IdCompany == GetCurrentCompanyId());
            modelBuilder.Entity<DetailLiquidation>().HasQueryFilter(dl => dl.IdCompany == GetCurrentCompanyId());

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

                entity.HasOne(w => w.Company)
                    .WithMany(c => c.workdays)
                    .HasForeignKey(w => w.IdCompany);

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

            modelBuilder.Entity<Company>().HasData(CreateCompanyDataSeed());
            modelBuilder.Entity<User>().HasData(CreateUserDataSeed());

            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.NoAction;
            }
        }

        // ... (Tus métodos Seed se mantienen iguales)
        private Company[] CreateCompanyDataSeed() { /* ... */ return new Company[] { }; }
        private User[] CreateUserDataSeed() { /* ... */ return new User[] { }; }
    }

    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var directory = Directory.GetCurrentDirectory();
            var apiPath = Path.Combine(directory, "PlataformaAutogestion.Api");
            if (!Directory.Exists(apiPath)) apiPath = Path.Combine(directory, "../PlataformaAutogestion.Api");

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Path.GetFullPath(apiPath))
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));

            // Pasamos null para el IHttpContextAccessor en tiempo de diseño
            return new ApplicationDbContext(optionsBuilder.Options, null!, false);
        }
    }
}