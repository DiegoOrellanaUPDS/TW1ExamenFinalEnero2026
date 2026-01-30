using Microsoft.AspNetCore.Mvc;
using Data;
using ExamenFinal.Entidades;

namespace ExamenFinal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdrianaController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AdrianaController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<Adriana>> PostAdriana(Adriana adriana)
        {
            _context.Adrianas.Add(adriana);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(PostAdriana), new { id = adriana.Id }, adriana);
        }
    }
}
