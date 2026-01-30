using Microsoft.AspNetCore.Mvc;
using Universidad.Data;
using Universidad.Entidades;

namespace Universidad.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RodrigoController : ControllerBase
    {
        private readonly AppDbContext _db;
        public RodrigoController(AppDbContext db) => _db = db;

        // POST: api/Rodrigo
        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] Rodrigo item)
        {
            _db.Rodrigos.Add(item);
            await _db.SaveChangesAsync();
            return Ok(item);
        }
    }
}
