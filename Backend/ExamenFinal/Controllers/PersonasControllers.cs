using ExamenFinal.Data;
using ExamenFinal.Entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExamenFinal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PersonasController : ControllerBase 
    {
        private readonly AppDbContext context;
        public PersonasController(AppDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetPersonas()
        {
            var personas = await context.Personas
                .Where(per => per.Estado != "Borrado")
                .ToListAsync();
            return Ok(personas);
        }

        [HttpGet("{ci}")]
        public async Task<IActionResult> GetPersona(int ci)
        {
            var persona = await context.Personas
                .FirstOrDefaultAsync(per => per.Ci == ci && per.Estado != "Borrado");
            
            if (persona == null) return NotFound();
            return Ok(persona);
        }
        
        [HttpPost]
        public async Task<IActionResult> PostPersona(Persona persona)
        {
            var exists = await context.Personas.AnyAsync(p => p.Ci == persona.Ci);
            if (exists) return BadRequest("La Persona ya existe");

            await context.Personas.AddAsync(persona);
            await context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPersona), new { ci = persona.Ci }, persona);
        }

        [HttpPut("{ci}")]
        public async Task<IActionResult> PutPersona(int ci, Persona persona)
        {
            if (ci != persona.Ci) return BadRequest("El CI de la URL no coincide con el del cuerpo");

            var existing = await context.Personas
                .FirstOrDefaultAsync(per => per.Ci == ci && per.Estado != "Borrado");
            
            if (existing == null) return NotFound();

            existing.Nombre = persona.Nombre;
            existing.Apellido = persona.Apellido;
            existing.FechaNacimiento = persona.FechaNacimiento;
            existing.Estado = persona.Estado;

            try
            {   
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(500, "Error de concurrencia");
            }

            return NoContent();
        }

        [HttpDelete("{ci}")]
        public async Task<IActionResult> DeletePersona(int ci)
        {
            var existing = await context.Personas
                .FirstOrDefaultAsync(per => per.Ci == ci && per.Estado != "Borrado");
            
            if (existing == null) return NotFound();

            existing.Estado = "Borrado";
            
            await context.SaveChangesAsync();
            return Ok(new { message = "Eliminado correctamente" });
        }
    }
}