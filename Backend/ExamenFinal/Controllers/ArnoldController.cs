using Microsoft.AspNetCore.Mvc;
using Universidad.Data;        // Asegúrate de que este namespace esté bien importado
using Universidad.Entidades;   // Asegúrate de que este namespace esté bien importado

namespace Backend.ExamenFinal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ArnoldController : ControllerBase
    {
        private readonly AppDbContext _context;

        // Constructor que inyecta el DbContext
        public ArnoldController(AppDbContext context)
        {
            _context = context;
        }

        // Endpoint para agregar un nuevo "Arnold"
        [HttpPost]
        public async Task<IActionResult> PostArnold(Arnold user)
        {
            _context.Arnolds.Add(user);
            await _context.SaveChangesAsync();
            return Ok("Usuario guardado");
        }
    }
}
