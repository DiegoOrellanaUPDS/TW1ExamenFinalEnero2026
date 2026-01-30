using Entidades;
using Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CrsthianAmador.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CristhianAmadorController : ControllerBase
    {
        private readonly AppDbContext context;

        public CristhianAmadorController(AppDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CristhianAmador>>> GetCristhianAmador()
        {
            return Ok(await context.CristhianAmadors.Where(e => e.Estado != false).ToListAsync());
        }

        [HttpGet("{ci}")]
        public async Task<IActionResult> GetEstudiante(int ci)
        {
            var persona = await (from cris in context.CristhianAmadors
                                   where cris.Ci == ci && cris.Estado != false
                                   select cris).FirstOrDefaultAsync();
            if (persona == null)
                return NotFound();
            return Ok(persona);
        }
        [HttpPost]
        public async Task<IActionResult> CreateEstudiante(CristhianAmador persona)
        {
            var e = await (from cris in context.CristhianAmadors
                           where cris.Ci == persona.Ci && cris.Estado != false
                           select cris).FirstOrDefaultAsync();
            if (e != null)
                return BadRequest("El estudiante ya existe.");
        
            await context.CristhianAmadors.AddAsync(persona);
            await context.SaveChangesAsync();
            return Ok(persona);
        }
    }
}