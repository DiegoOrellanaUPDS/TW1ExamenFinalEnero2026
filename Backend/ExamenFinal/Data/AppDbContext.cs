using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        //Entidades y Modelos AQUÍ
        //public DbSet<Docente> Docentes { get; set; }

        //NO BORRAR, COMPATIBILIDAD DateTime con Postgres
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // DataTime (C#) == Date (PostreSQL)
            // Recorre todas las entidades y propiedades DateTime
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    // Si la propiedad es DateTime o DateTime?
                    if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                    {
                        property.SetColumnType("date"); // Se guarda como "date" en PostgreSQL
                    }
                }
            }
        }

    }

}

