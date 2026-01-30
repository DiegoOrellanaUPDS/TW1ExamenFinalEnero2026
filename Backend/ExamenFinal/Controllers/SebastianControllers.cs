using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Data;
using Entidades;
using ExamenFinal.Entidades;

namespace ExamenFinal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SebastianController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SebastianController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<Sebastian>> PostSebastian(Sebastian item)
        {
            _context.Sebastians.Add(item);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSebastian), new { id = item.Id }, item);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Sebastian>> GetSebastian(int id)
        {
            var item = await _context.Sebastians.FindAsync(id);

            if (item == null)
            {
                return NotFound();
            }

            return item;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Sebastian>>> GetSebastians()
        {
            return await _context.Sebastians.ToListAsync();
        }
    }
}