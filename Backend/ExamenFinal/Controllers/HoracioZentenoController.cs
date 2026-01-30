using Data;
using Entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HoracioZentenoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public HoracioZentenoController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("gethoracio")]
        public async Task<IActionResult> GetHoracio()
        {
            var lista = await _context.HoracioZenteno.ToListAsync();
            return Ok(lista);
        }

        [HttpPost("posthoracio")]
        public async Task<IActionResult> PostHoracio([FromBody] HoracioZenteno data)
        {
            if (data == null)
                return BadRequest();

            _context.HoracioZenteno.Add(data);
            await _context.SaveChangesAsync();

            return Ok(data);
        }
    }
}
