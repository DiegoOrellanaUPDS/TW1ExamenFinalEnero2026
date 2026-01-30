using Data;
using Entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AlejandroRiveraController : ControllerBase
    {
        private readonly AppDbContext context;

        public AlejandroRiveraController(AppDbContext context)
        {
            this.context = context;
        }

        // GET: api/AlejandroRivera/getalejandrorivera
        [HttpGet("getalejandrorivera")]
        public async Task<IActionResult> GetAlejandroRivera()
        {
            var registros = await context.AlejandroRivera.ToListAsync();
            return Ok(registros);
        }

        // POST: api/AlejandroRivera/postalejandrorivera
        [HttpPost("postalejandrorivera")]
        public async Task<IActionResult> PostAlejandroRivera(AlejandroRivera entidad)
        {
            var exists = await context.AlejandroRivera.AnyAsync(r => r.Ci == entidad.Ci);
            if (exists) return BadRequest("El registro con ese CI ya existe");

            await context.AlejandroRivera.AddAsync(entidad);
            await context.SaveChangesAsync();

            return Ok(entidad);
        }
    }
}
