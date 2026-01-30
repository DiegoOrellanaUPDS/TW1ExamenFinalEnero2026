<<<<<<< HEAD
<<<<<<< HEAD
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
=======


>>>>>>> 6424321c55f6f875de374b0cbc34384b897c3fdb
using Entidades;
using ExamenFinal.Entidades;
using Microsoft.EntityFrameworkCore;

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
        public DbSet<Sebastian> Sebastians { get; set; }

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
using Microsoft.EntityFrameworkCore;
using Universidad.Entidades;  // Asegúrate de importar las entidades correctamente

namespace Universidad.Data
{
    public class AppDbContext : DbContext
    {
        // Constructor
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // DbSets para las entidades
        public DbSet<Rodrigo> Rodrigos { get; set; } = default!;
        public DbSet<Arnold> Arnolds { get; set; } = default!;
        public DbSet<Beymar> Beymars { get; set; } = default!;
        public DbSet<Brandon> Brandons { get; set; } = default!;
        public DbSet<Herberth> Herberths { get; set; } = default!;
        public DbSet<Joel> Joels { get; set; } = default!;
        public DbSet<Marcelo> Marcellos { get; set; } = default!;
        public DbSet<Persona> Personas { get; set; } = default!;
        public DbSet<SergioVillarrubia> SergioVillarrubias { get; set; } = default!;
    }
}
>>>>>>> eaa15fc0fc9cb389423af2eb39bac6bf2479b1f4
