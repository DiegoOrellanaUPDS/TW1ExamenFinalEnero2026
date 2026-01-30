// Archivo: Data/AppDbContext.cs
using Microsoft.EntityFrameworkCore;
using Entidades;

namespace Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Persona> Personas { get; set; }
        public DbSet<Snaider> Snaiders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración para Snaider
            modelBuilder.Entity<Snaider>(entity =>
            {
                // DiscordId es único y requerido
                entity.HasIndex(e => e.DiscordId).IsUnique();
                
                // CI puede ser nulo pero si existe debe ser único
                entity.HasIndex(e => e.Ci).IsUnique();
                entity.Property(e => e.Ci).IsRequired(false);
                
                // Configuraciones de campos
                entity.Property(e => e.DiscordId)
                    .IsRequired()
                    .HasMaxLength(500);
                    
                entity.Property(e => e.DiscordUsername)
                    .IsRequired()
                    .HasMaxLength(100);
                    
                entity.Property(e => e.DiscordEmail)
                    .HasMaxLength(200);
                    
                entity.Property(e => e.Nombre)
                    .HasMaxLength(100);
                    
                entity.Property(e => e.Estado)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasDefaultValue("Activo");
                    
                entity.Property(e => e.DiscordToken)
                    .HasMaxLength(2000);
                    
                entity.Property(e => e.DiscordRefreshToken)
                    .HasMaxLength(2000);
                    
                entity.Property(e => e.DiscordAvatar)
                    .HasMaxLength(500);
                    
                entity.Property(e => e.DiscordLocale)
                    .HasMaxLength(50);
                    
                // Valores por defecto
                entity.Property(e => e.FechaRegistro)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
                    
                entity.Property(e => e.Estado)
                    .HasDefaultValue("Activo");
            });

            // Configuración para Persona (manteniendo compatibilidad)
            modelBuilder.Entity<Persona>(entity =>
            {
                entity.HasIndex(e => e.Ci).IsUnique();
            });
        }
    }
}