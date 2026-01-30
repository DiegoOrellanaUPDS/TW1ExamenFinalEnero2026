using Entidades;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Persona> Personas { get; set; }
        public DbSet<Brandon> Brandons { get; set; }
        // Aquí puedes agregar más DbSet si necesitas otros modelos
        // public DbSet<Docente> Docentes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Recorre todas las entidades y propiedades DateTime para compatibilidad con PostgreSQL
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                    {
                        property.SetColumnType("date"); // Se guarda como "date" en PostgreSQL
                    }
                }
            }
        }
    }
}
