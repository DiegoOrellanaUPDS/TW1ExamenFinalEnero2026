using Microsoft.EntityFrameworkCore;
using Entidades;

namespace Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        // ============================
        // Entidades y Modelos AQUÍ
        // ============================

        // EXISTENTE
        public DbSet<Persona> Personas { get; set; }

        // TU ENTIDAD DEL EXAMEN
        public DbSet<Gerson> Gersones { get; set; }

        // ============================
        // NO BORRAR, COMPATIBILIDAD DateTime con Postgres
        // ============================
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // DataTime (C#) == Date (PostreSQL)
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                    {
                        property.SetColumnType("date");
                    }
                }
            }
        }
    }
}
