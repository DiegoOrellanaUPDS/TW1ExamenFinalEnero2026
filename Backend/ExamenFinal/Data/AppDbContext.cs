using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Entidades;
using ExamenFinal.Entidades;

namespace Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            
        }

        //Entidades y Modelos AQUÍ
        //public DbSet<Docente> Docentes { get; set; }
        public DbSet<Entidades.Joel> Joeles { get; set; }
        public DbSet<Entidades.Wilson> Wilson { get; set; }
        public DbSet<Entidades.Arnold> Arnolds { get; set; }
        public DbSet<Entidades.Persona> Personas { get; set; }
        public DbSet<Entidades.Brandon> Brandons { get; set; }
        public DbSet<Entidades.SergioVillarrubia> SergioVillarrubias { get; set; }
        public DbSet<Entidades.Marcelo> Marcelo { get; set; }
        public DbSet<Entidades.JavierAramayo> JavierAramayos { get; set; }
        public DbSet<Entidades.Herberth> Herberth { get; set; }
        public DbSet<Entidades.RodrigoPorcel> RodrigoPorcel { get; set; }

        //NO BORRAR, COMPATIBILIDAD DateTime con Postgres
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
