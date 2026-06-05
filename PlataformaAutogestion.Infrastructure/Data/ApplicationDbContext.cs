using Microsoft.EntityFrameworkCore;
using PlataformaAutogestion.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlataformaAutogestion.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) 
        { 
        }

        public DbSet<Company> Companys { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Workday> Workdays { get; set; }
        public DbSet<Liquidation> Liquidations { get; set; }
        public DbSet<DetailLiquidation> Details { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); //aca se configura relaciones, restricciones, etc.
        }
        
    }
}
