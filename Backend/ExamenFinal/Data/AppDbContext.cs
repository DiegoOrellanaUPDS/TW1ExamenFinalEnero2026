<<<<<<< HEAD
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Entidades;
using ExamenFinal.Entidades;

namespace Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        // Entidades
        public DbSet<AdrianRojas> AdrianRojases { get; set; }
        public DbSet<AlejandroRivera> AlejandroRivera { get; set; }
        public DbSet<Arnold> Arnolds { get; set; }
        public DbSet<Brandon> Brandons { get; set; }
        public DbSet<CristhianAmador> CristhianAmadors { get; set; }
        public DbSet<Herberth> Herberth { get; set; }
        public DbSet<HoracioZenteno> HoracioZenteno { get; set; }
        public DbSet<JavierAramayo> JavierAramayos { get; set; }
        public DbSet<Marcelo> Marcelo { get; set; }
        public DbSet<MiguelAngelColqueCalizaya> MiguelAngelColqueCalizaya { get; set; }
        public DbSet<Persona> Personas { get; set; }
        public DbSet<RodrigoPorcel> RodrigoPorcel { get; set; }
        public DbSet<SergioVillarrubia> SergioVillarrubias { get; set; }
        public DbSet<Wilson> Wilson { get; set; }

        // NO BORRAR, compatibilidad DateTime con Postgres
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(DateTime) ||
                        property.ClrType == typeof(DateTime?))
                    {
                        property.SetColumnType("date");
                    }
                }
            }
        }
    }
}

=======

using ExamenFinal.Entidades;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace ExamenFinal.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
           
        }
        public DbSet<Persona> Personas {get;set;}

        public DbSet<VictorCox> VictorCoxs { get; set; }
    }

}

>>>>>>> feature/victorcox-entidad
