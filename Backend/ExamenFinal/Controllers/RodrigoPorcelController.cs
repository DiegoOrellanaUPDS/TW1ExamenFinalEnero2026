using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Data;
using Entidades;

namespace Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RodrigoPorcelController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RodrigoPorcelController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/RodrigoPorcel
        [HttpPost]
        public async Task<ActionResult<RodrigoPorcel>> PostRodrigoPorcel(RodrigoPorcel rodrigoPorcel)
        {
            _context.RodrigoPorcel.Add(rodrigoPorcel);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRodrigoPorcel), new { id = rodrigoPorcel.Id }, rodrigoPorcel);
        }

        // GET: api/RodrigoPorcel/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RodrigoPorcel>> GetRodrigoPorcel(int id)
        {
            var rodrigoPorcel = await _context.RodrigoPorcel.FindAsync(id);

            if (rodrigoPorcel == null)
            {
                return NotFound();
            }

            return rodrigoPorcel;
        }
    }
}
