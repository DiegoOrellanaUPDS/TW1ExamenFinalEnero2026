<<<<<<< HEAD
﻿using Microsoft.EntityFrameworkCore;
using TW1ExamenFinalEnero2026.Entidades;
=======
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Entidades;
>>>>>>> f317bb4e6222bbb794096311a6ffa757a3d3e52d

namespace Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

<<<<<<< HEAD
        }
        //Entidades y Modelos AQUÍ
        //public DbSet<Docente> Docentes { get; set; }
        public DbSet<Joel> Joeles { get; set; }
=======
        // Entidades y Modelos
        public DbSet<Arnold> Arnolds { get; set; }
        public DbSet<Persona> Personas { get; set; }
        public DbSet<Brandon> Brandons { get; set; }
        public DbSet<SergioVillarrubia> SergioVillarrubias { get; set; }
        public DbSet<Herberth> Herberth { get; set; }
        // public DbSet<Docente> Docentes { get; set; } // puedes descomentar si lo necesitas
>>>>>>> f317bb4e6222bbb794096311a6ffa757a3d3e52d

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
