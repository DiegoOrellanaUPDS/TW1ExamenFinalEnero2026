using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Data;   // <--- REVISA QUE COINCIDA CON TU PROYECTO
using Entidades; // <--- REVISA QUE COINCIDA CON TU PROYECTO

namespace Universidad.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrayanController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BrayanController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/Brayan
        [HttpPost]
        public async Task<ActionResult<Brayan>> PostBrayan(Brayan brayan)
        {
            _context.Brayans.Add(brayan);
            await _context.SaveChangesAsync();
            
            // Retorna 201 Created y el objeto
            return CreatedAtAction(nameof(PostBrayan), new { id = brayan.Id }, brayan);
        }
    }
}