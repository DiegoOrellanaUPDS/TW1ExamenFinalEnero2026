using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExamenFinal.Data;
using ExamenFinal.Entidades;

namespace ExamenFinal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VictorCoxController : ControllerBase
    {
        private readonly AppDbContext _context;

        public VictorCoxController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<VictorCox>> PostVictorCox(VictorCox entidad)
        {
            var estadosValidos = new List<string> { "soltero", "casado", "arrecho" };
            
            if (string.IsNullOrEmpty(entidad.Estado) || !estadosValidos.Contains(entidad.Estado.ToLower()))
            {
                return BadRequest("El estado debe ser: soltero, casado o arrecho.");
            }

            _context.VictorCoxs.Add(entidad);
            await _context.SaveChangesAsync();

            return StatusCode(201, entidad);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<VictorCox>> GetVictorCox(int id)
        {
            var entidad = await _context.VictorCoxs.FindAsync(id);
            if (entidad == null) return NotFound();
            return entidad;
        }
    }
}