using Microsoft.AspNetCore.Mvc;
using Data;
using Entidades;

namespace ExamenFinal.Controllers
{
    [ApiController]
    [Route("api/gerson")]
    public class GersonController : ControllerBase
    {
        private readonly AppDbContext _context;

        public GersonController(AppDbContext context)
        {
            _context = context;
        }

        // ============================
        // POST: Registrar Gerson
        // ============================
        [HttpPost]
        public IActionResult Registrar([FromBody] Gerson gerson)
        {
            _context.Gersones.Add(gerson);
            _context.SaveChanges();

            return Ok(gerson);
        }
    }
}
