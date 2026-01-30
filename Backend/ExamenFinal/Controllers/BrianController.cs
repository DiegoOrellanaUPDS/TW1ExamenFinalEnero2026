using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Data;
using Entidades;

namespace ExamenFinal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrianController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BrianController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<Brian>> PostBrian(Brian brian)
        {
            _context.Brians.Add(brian);
            await _context.SaveChangesAsync();
            return Ok(brian);
        }
    }
}