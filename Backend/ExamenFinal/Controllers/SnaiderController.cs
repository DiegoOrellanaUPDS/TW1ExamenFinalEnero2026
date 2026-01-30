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

        // GET: api/snaider/get_all_snaider
        [HttpGet("get_all_snaider")]
        public async Task<IActionResult> GetAllSnaider()
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
                    s.DiscordEmail
                })
                .ToListAsync();
            return Ok(snaiders);
        }

        // GET: api/snaider/get_by_id_snaider/{id}
        [HttpGet("get_by_id_snaider/{id}")]
        public async Task<IActionResult> GetByIdSnaider(int id)
        {
            var snaider = await context.Snaiders
                .Where(s => s.Id == id && s.Estado != "Borrado")
                .Select(s => new {
                    s.Id,
                    s.Ci,
                    s.Nombre,
                    s.Edad,
                    s.Estado,
                    s.DiscordUsername,
                    s.DiscordEmail
                })
                .FirstOrDefaultAsync();
            
            if (snaider == null) return NotFound();
            return Ok(snaider);
        }

        // GET: api/snaider/get_by_ci_snaider/{ci}
        [HttpGet("get_by_ci_snaider/{ci}")]
        public async Task<IActionResult> GetByCiSnaider(int ci)
        {
            var snaider = await context.Snaiders
                .Where(s => s.Ci == ci && s.Estado != "Borrado")
                .Select(s => new {
                    s.Id,
                    s.Ci,
                    s.Nombre,
                    s.Edad,
                    s.Estado,
                    s.DiscordUsername,
                    s.DiscordEmail
                })
                .FirstOrDefaultAsync();
            
            if (snaider == null) return NotFound();
            return Ok(snaider);
        }

        // POST: api/snaider/create_snaider
        [HttpPost("create_snaider")]
        public async Task<IActionResult> CreateSnaider(Snaider snaider)
        {
            if (string.IsNullOrEmpty(snaider.Nombre))
                return BadRequest("El nombre es requerido");
            
            if (snaider.Edad <= 0)
                return BadRequest("La edad debe ser mayor a 0");
            
            if (snaider.Ci <= 0)
                return BadRequest("El CI debe ser un número válido");

            var ciExists = await context.Snaiders
                .AnyAsync(s => s.Ci == snaider.Ci && s.Estado != "Borrado");
            
            if (ciExists)
                return BadRequest("Ya existe un registro con este CI");

            if (string.IsNullOrEmpty(snaider.Estado))
                snaider.Estado = "Activo";

            snaider.DiscordToken = null;
            snaider.DiscordRefreshToken = null;
            snaider.DiscordTokenExpiry = null;

            await context.Snaiders.AddAsync(snaider);
            await context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetByIdSnaider), new { id = snaider.Id }, new {
                snaider.Id,
                snaider.Ci,
                snaider.Nombre,
                snaider.Edad,
                snaider.Estado
            });
        }

        // PUT: api/snaider/update_snaider/{id}
        [HttpPut("update_snaider/{id}")]
        public async Task<IActionResult> UpdateSnaider(int id, Snaider snaider)
        {
            if (id != snaider.Id) 
                return BadRequest("El ID de la URL no coincide con el del cuerpo");

            var existing = await context.Snaiders
                .FirstOrDefaultAsync(s => s.Id == id && s.Estado != "Borrado");
            
            if (existing == null) return NotFound();

            if (existing.Ci != snaider.Ci)
            {
                var ciExists = await context.Snaiders
                    .AnyAsync(s => s.Ci == snaider.Ci && s.Id != id && s.Estado != "Borrado");
                
                if (ciExists)
                    return BadRequest("Ya existe otro registro con este CI");
            }

            existing.Ci = snaider.Ci;
            existing.Nombre = snaider.Nombre;
            existing.Edad = snaider.Edad;
            existing.Estado = snaider.Estado;

            try
            {   
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(500, "Error de concurrencia");
            }

            return NoContent();
        }

        // DELETE: api/snaider/delete_snaider/{id}
        [HttpDelete("delete_snaider/{id}")]
        public async Task<IActionResult> DeleteSnaider(int id)
        {
            var existing = await context.Snaiders
                .FirstOrDefaultAsync(s => s.Id == id && s.Estado != "Borrado");
            
            if (existing == null) return NotFound();

            existing.Estado = "Borrado";
            
            await context.SaveChangesAsync();
            return Ok(new { message = "Eliminado correctamente" });
        }

        // POST: api/snaider/register_with_discord_snaider
        [HttpPost("register_with_discord_snaider")]
        public async Task<IActionResult> RegisterWithDiscordSnaider([FromBody] RegisterWithDiscordRequest request)
        {
            if (string.IsNullOrEmpty(request.Nombre))
                return BadRequest("El nombre es requerido");
            
            if (request.Edad <= 0)
                return BadRequest("La edad debe ser mayor a 0");
            
            if (request.Ci <= 0)
                return BadRequest("El CI debe ser un número válido");
            
            if (string.IsNullOrEmpty(request.DiscordId))
                return BadRequest("El Discord ID es requerido");

            var ciExists = await context.Snaiders
                .AnyAsync(s => s.Ci == request.Ci && s.Estado != "Borrado");
            
            if (ciExists)
                return BadRequest("Ya existe un registro con este CI");
            
            var discordExists = await context.Snaiders
                .AnyAsync(s => s.DiscordId == request.DiscordId && s.Estado != "Borrado");
            
            if (discordExists)
                return BadRequest("Esta cuenta de Discord ya está registrada");

            var snaider = new Snaider
            {
                Ci = request.Ci,
                Nombre = request.Nombre,
                Edad = request.Edad,
                Estado = "Activo",
                DiscordId = request.DiscordId,
                DiscordUsername = request.DiscordUsername,
                DiscordEmail = request.DiscordEmail,
                DiscordToken = request.DiscordToken,
                DiscordRefreshToken = request.DiscordRefreshToken,
                DiscordTokenExpiry = DateTime.UtcNow.AddSeconds(request.TokenExpiresIn)
            };

            await context.Snaiders.AddAsync(snaider);
            await context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetByIdSnaider), new { id = snaider.Id }, new {
                snaider.Id,
                snaider.Ci,
                snaider.Nombre,
                snaider.Edad,
                snaider.DiscordUsername,
                message = "Usuario registrado y vinculado a Discord exitosamente"
            });
        }

        // GET: api/snaider/get_by_discord_snaider/{discordId}
        [HttpGet("get_by_discord_snaider/{discordId}")]
        public async Task<IActionResult> GetByDiscordSnaider(string discordId)
        {
            var snaider = await context.Snaiders
                .Where(s => s.DiscordId == discordId && s.Estado != "Borrado")
                .Select(s => new {
                    s.Id,
                    s.Ci,
                    s.Nombre,
                    s.Edad,
                    s.Estado,
                    s.DiscordUsername,
                    s.DiscordEmail
                })
                .FirstOrDefaultAsync();
            
            if (snaider == null) return NotFound();
            return Ok(snaider);
        }

        public class RegisterWithDiscordRequest
        {
            public int Ci { get; set; }
            public string Nombre { get; set; }
            public int Edad { get; set; }
            public string DiscordId { get; set; }
            public string DiscordUsername { get; set; }
            public string DiscordEmail { get; set; }
            public string DiscordToken { get; set; }
            public string DiscordRefreshToken { get; set; }
            public int TokenExpiresIn { get; set; }
        }
    }
}