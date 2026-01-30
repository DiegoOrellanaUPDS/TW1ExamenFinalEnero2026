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
