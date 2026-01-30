// Archivo: Controllers/SnaiderController.cs
using Data;
using Entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SnaiderController : ControllerBase 
    {
        private readonly AppDbContext context;
        
        public SnaiderController(AppDbContext context)
        {
            this.context = context;
        }

        // ENDPOINT POST REQUERIDO: api/snaider
        // Permite registrar datos de la entidad Snaider en la base de datos
        [HttpPost]
        public async Task<IActionResult> PostSnaider([FromBody] Snaider snaider)
        {
            try
            {
                // Validaciones básicas
                if (snaider == null)
                    return BadRequest(new { 
                        success = false, 
                        message = "Los datos del Snaider son requeridos" 
                    });
                
                // Si no se especifica estado, poner "Activo" por defecto
                if (string.IsNullOrEmpty(snaider.Estado))
                    snaider.Estado = "Activo";
                
                // Si no se especifica fecha de registro, usar la actual
                if (snaider.FechaRegistro == default)
                    snaider.FechaRegistro = DateTime.UtcNow;
                
                // Validar CI si se proporciona
                if (snaider.Ci.HasValue)
                {
                    var ciExists = await context.Snaiders
                        .AnyAsync(s => s.Ci == snaider.Ci.Value && s.Estado != "Borrado");
                    
                    if (ciExists)
                        return BadRequest(new { 
                            success = false, 
                            message = "Ya existe un registro con este CI" 
                        });
                }
                
                // Validar DiscordId si se proporciona
                if (!string.IsNullOrEmpty(snaider.DiscordId))
                {
                    var discordExists = await context.Snaiders
                        .AnyAsync(s => s.DiscordId == snaider.DiscordId && s.Estado != "Borrado");
                    
                    if (discordExists)
                        return BadRequest(new { 
                            success = false, 
                            message = "Ya existe un registro con este Discord ID" 
                        });
                }
                
                // Guardar en la base de datos
                await context.Snaiders.AddAsync(snaider);
                await context.SaveChangesAsync();
                
                // Retornar respuesta exitosa
                return CreatedAtAction(nameof(GetSnaiderById), new { id = snaider.Id }, new {
                    success = true,
                    message = "Snaider registrado exitosamente",
                    data = new {
                        snaider.Id,
                        snaider.Ci,
                        snaider.Nombre,
                        snaider.Edad,
                        snaider.Estado,
                        snaider.DiscordId,
                        snaider.DiscordUsername,
                        snaider.DiscordEmail,
                        snaider.FechaRegistro
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new {
                    success = false,
                    message = "Error interno del servidor",
                    error = ex.Message
                });
            }
        }

        // Método auxiliar para CreatedAtAction
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSnaiderById(int id)
        {
            var snaider = await context.Snaiders
                .Where(s => s.Id == id && s.Estado != "Borrado")
                .Select(s => new {
                    s.Id,
                    s.Ci,
                    s.Nombre,
                    s.Edad,
                    s.Estado,
                    s.DiscordId,
                    s.DiscordUsername,
                    s.DiscordEmail,
                    s.DiscordAvatar,
                    s.DiscordVerified,
                    s.FechaRegistro,
                    s.UltimoLogin
                })
                .FirstOrDefaultAsync();
            
            if (snaider == null)
                return NotFound(new { success = false, message = "Snaider no encontrado" });
            
            return Ok(new { success = true, data = snaider });
        }

        // GET: api/snaider (opcional - para listar todos)
        [HttpGet]
        public async Task<IActionResult> GetSnaiders()
        {
            var snaiders = await context.Snaiders
                .Where(s => s.Estado != "Borrado")
                .Select(s => new {
                    s.Id,
                    s.Ci,
                    s.Nombre,
                    s.Edad,
                    s.Estado,
                    s.DiscordUsername,
                    s.FechaRegistro
                })
                .ToListAsync();
            
            return Ok(new { success = true, data = snaiders, count = snaiders.Count });
        }
    }
}