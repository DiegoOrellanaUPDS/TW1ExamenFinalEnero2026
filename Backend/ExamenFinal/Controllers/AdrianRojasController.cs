using Microsoft.AspNetCore.Mvc;
using Entidades;
using Data; // Ajusta esto según el nombre de tu carpeta de datos

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdrianRojasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AdrianRojasController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Post(AdrianRojas entidad)
        {
            _context.AdrianRojases.Add(entidad);
            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Datos guardados correctamente", data = entidad });
        }
    }
}
