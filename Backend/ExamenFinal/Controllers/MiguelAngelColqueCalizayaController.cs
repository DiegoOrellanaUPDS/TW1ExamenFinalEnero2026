using Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using TuProyecto.Entities;

namespace TuProyecto.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MiguelAngelColqueCalizayaController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MiguelAngelColqueCalizayaController(AppDbContext context)
        {
            _context = context;
        }


        // POST: api/MiguelAngelColqueCalizaya
        [HttpPost("postMiguelAngelColqueCalizaya")]
        public async Task<IActionResult> Post([FromBody] MiguelAngelColqueCalizaya miguel)
        {
            try
            {
                // Validaciones
                if (miguel == null)
                    return BadRequest("Los datos son requeridos");

                if (string.IsNullOrWhiteSpace(miguel.NombreMiguelColque))
                    return BadRequest("El nombre es obligatorio");

                if (miguel.EdadMiguelColque < 1 || miguel.EdadMiguelColque > 100)
                    return BadRequest("La edad debe estar entre 1 y 100 años");

                if (miguel.EstadoMiguelColque != "soltero" && miguel.EstadoMiguelColque != "universitario")
                    return BadRequest("El estado debe ser 'soltero' o 'universitario'");

                // Generar token automático si no viene
                if (string.IsNullOrEmpty(miguel.tokenMiguelColque))
                    miguel.tokenMiguelColque = $"TOKEN_{Guid.NewGuid():N}";

                // Guardar en BD
                _context.MiguelAngelColqueCalizaya.Add(miguel);
                await _context.SaveChangesAsync();

                // Respuesta exitosa
                return Ok(new
                {
                    success = true,
                    message = "Registro creado exitosamente",
                    data = new
                    {
                        miguel.IdMiguelColque,
                        miguel.NombreMiguelColque,
                        miguel.EdadMiguelColque,
                        miguel.EstadoMiguelColque,
                        miguel.tokenMiguelColque
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Error interno: {ex.Message}" });
            }
        }
        // GET: api/MiguelAngelColqueCalizaya
        [HttpGet("GetMiguelAngelColqueCalizaya")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var registros = await _context.MiguelAngelColqueCalizaya.ToListAsync();

                return Ok(new
                {
                    success = true,
                    count = registros.Count,
                    data = registros.Select(m => new
                    {
                        m.IdMiguelColque,
                        m.NombreMiguelColque,
                        m.EdadMiguelColque,
                        m.EstadoMiguelColque,
                        m.tokenMiguelColque
                    })
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Error interno: {ex.Message}" });
            }
        }
    }
}