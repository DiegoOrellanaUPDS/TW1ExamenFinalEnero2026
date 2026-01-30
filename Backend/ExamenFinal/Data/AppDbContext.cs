
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
        public DbSet<UsuarioVictorCox> UsuariosVictorCox { get; set; }
    }

}

