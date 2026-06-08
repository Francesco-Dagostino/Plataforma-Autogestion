using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;
using PlataformaAutogestion.Domain.Entities;

namespace PlataformaAutogestion.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        private readonly bool isTestingEnvironment;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, bool isTestingEnvironment = false) : base(options)
        {
            this.isTestingEnvironment = isTestingEnvironment;
        }

        public DbSet<Company> Companys { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Workday> Workdays { get; set; }
        public DbSet<Liquidation> Liquidations { get; set; }
        public DbSet<DetailLiquidation> Details { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

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
                    .HasForeignKey(u => u.IdCompany);
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
                entity.Property(dl => dl.amount).IsRequired();

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

            /*
             * las comiteo para no tener un id 1 ya creado en company
            // modelBuilder.Entity<Company>().HasData(CreateCompanyDataSeed());
            // modelBuilder.Entity<User>().HasData(CreateUserDataSeed());
            */

            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.NoAction;
            }
        }

        private Company[] CreateCompanyDataSeed()
        {
            var fechaAlta = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);


            if (isTestingEnvironment)
            {
                return new[]
                {
                    new Company { Id = 1, Name = "Empresa Testing S.A.", Cuit = 11111111, DateHigh = fechaAlta, ParameterSystem = 1 }
                };
            }

            return new[]
            {
                new Company { Id = 1, Name = "Mi Primera Empresa PYME", Cuit = 2030405060, DateHigh = fechaAlta, ParameterSystem = 1 }
            };
        }

        private User[] CreateUserDataSeed()
        {
            var fechaCreacion = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);


            if (isTestingEnvironment)
            {
                return new[]
                {
                    new User
                    {
                        Id = 1,
                        Name = "Testing Admin",
                        Email = "test@empresa.com",
                        UserName = "admin_test",
                        Password = "hashed_password_placeholder",
                        CreationDate = fechaCreacion,
                        IdCompany = 1,
                        role = 0
                    }
                };
            }

            return new[]
            {
                new User
                {
                    Id = 1,
                    Name = "Administrador Sistema",
                    Email = "admin@empresa.com",
                    UserName = "admin",
                    Password = "hashed_password_placeholder",
                    CreationDate = fechaCreacion,
                    IdCompany = 1,
                    role = 0
                }
            };
        }
    }

    // Clase para que EF Tools pueda instanciar el contexto en design-time
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            // Busca appsettings.json subiendo directorios hasta encontrarlo
            var directory = Directory.GetCurrentDirectory();
            var apiPath = Path.Combine(directory, "PlataformaAutogestion.Api");

            if (!Directory.Exists(apiPath))
                apiPath = Path.Combine(directory, "../PlataformaAutogestion.Api");

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Path.GetFullPath(apiPath))
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection")
            );

            return new ApplicationDbContext(optionsBuilder.Options, false);
        }
    }
}