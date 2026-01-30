using Microsoft.AspNetCore.Mvc;
using Data;
using Entidades;

namespace Universidad.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BrunoController : ControllerBase
    {
        private readonly AppDbContext _db;
        public BrunoController(AppDbContext db) => _db = db;

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] Bruno item)
        {
            _db.Brunos.Add(item);
            await _db.SaveChangesAsync();
            return Ok(item);
        }
    }
}




