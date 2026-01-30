using ExamenFinal.Entidades;
using Entidades;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
           
        }
        public DbSet<Persona> Personas {get;set;}
        public DbSet<Adriana> Adrianas {get; set;}

    }

}

