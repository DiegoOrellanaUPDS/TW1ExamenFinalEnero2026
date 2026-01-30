using Data;
using Entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HerberthController : ControllerBase
    {
        private readonly AppDbContext context;
        public HerberthController(AppDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetHerberth()
        {
            var herberth = await (from h in context.Herberth select h).ToListAsync();
            return Ok(herberth);
        }

        [HttpPost]
        public async Task<IActionResult> PostHerberth(Herberth herberth)
        {
            var existing = await (from h in context.Herberth where herberth.Id == h.Id select h).FirstOrDefaultAsync();
            if (existing != null) return BadRequest("Ya existe");

            return Ok("Se creo correctamente");
        }
    }
}