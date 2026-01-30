using Data;
using Entidades;
using Microsoft.AspNetCore.Mvc;

namespace ExamenFinal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BrandonController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BrandonController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult Post([FromBody] Brandon brandon)
        {
            _context.Brandons.Add(brandon);
            _context.SaveChanges();
            return Ok(brandon);
        }
    }
}
