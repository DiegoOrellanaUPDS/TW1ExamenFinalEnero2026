using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Entidades;
using ExamenFinal.Entidades;

namespace Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        //Entidades y Modelos AQUÍ
        //public DbSet<Docente> Docentes { get; set; }
        //crea bien tu entidad joel
        // public DbSet<Joel> Joeles {get;set;}

        // Entidades y Modelos
        public DbSet<Wilson> Wilson {get;set;}
        public DbSet<Arnold> Arnolds { get; set; }
        public DbSet<Persona> Personas { get; set; }
        public DbSet<Brandon> Brandons { get; set; }
        public DbSet<SergioVillarrubia> SergioVillarrubias { get; set; }
        public DbSet<Persona> Persona { get; set; }
        public DbSet<Marcelo> Marcelo { get; set; }
        public DbSet<JavierAramayo> JavierAramayos {get;set;}
        public DbSet<Herberth> Herberth { get; set; }
        public DbSet<HoracioZenteno> HoracioZenteno { get; set; }
        // public DbSet<Docente> Docentes { get; set; } // puedes descomentar si lo necesitas

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

