using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Data;
using Entidades;

namespace ExamenFinal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RicardoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RicardoController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("postricardo")]
        public async Task<ActionResult<Ricardo>> PostRicardo(Ricardo Ricardo)
        {
            _context.Ricardos.Add(Ricardo);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetRicardo), new { id = Ricardo.Id }, Ricardo);
        }

        [HttpGet("getricardo/{id}")]
        public async Task<ActionResult<Ricardo>> GetRicardo(int id)
        {
        var ricardo = await _context.Ricardos.FindAsync(id);
        if (ricardo == null)
        {
            return NotFound(); 
        }
        return ricardo;
        }
    }
}