using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Data;
using ExamenFinal.Entidades;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace TW1ExamenFinalEnero2026.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class JoelController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<JoelController> _logger;

        public JoelController(AppDbContext context, ILogger<JoelController> logger)
        {
            _context = context;
            _logger = logger;
        }
        // Endpoint público para login
        [AllowAnonymous]
        [HttpGet("login")]
        public IActionResult Login()
       {
          return Challenge(new AuthenticationProperties { RedirectUri = "/api/joel" }, "Google");
     }

        // PARTE 1: POST para registrar datos (5 puntos)
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CrearJoel([FromBody] Joel joel)
        {
            try
            {
                _logger.LogInformation("Creando nuevo registro Joel: {Nombre}", joel.Nombre);
                
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Modelo inválido: {Errors}", ModelState.Values);
                    return BadRequest(ModelState);
                }

                // Validar duplicados (opcional)
                var existe = await _context.Joel
                    .AnyAsync(j => j.Nombre == joel.Nombre && j.Edad == joel.Edad);
                
                if (existe)
                {
                    return Conflict(new { message = "Ya existe un registro con esos datos" });
                }

                joel.FechaCreacion = DateTime.UtcNow;
                _context.Joel.Add(joel);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Joel creado con ID: {Id}", joel.Id);
                
                return CreatedAtAction(
                    nameof(ObtenerJoelPorId), 
                    new { id = joel.Id }, 
                    new { 
                        message = "Registro Joel creado exitosamente",
                        data = joel 
                    }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear registro Joel");
                return StatusCode(500, new { error = "Error interno del servidor" });
            }
        }

        // Endpoint adicional para GET (verificación)
        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerJoelPorId(int id)
        {
            var joel = await _context.Joel.FindAsync(id);
            if (joel == null)
            {
                return NotFound(new { message = $"Joel con ID {id} no encontrado" });
            }
            
            return Ok(joel);
        }

        // Listar todos (para pruebas)
        [HttpGet]
        public async Task<IActionResult> ListarJoeles()
        {
            var joeles = await _context.Joel.ToListAsync();
            return Ok(new { 
                total = joeles.Count, 
                datos = joeles 
            });
        }
         // Endpoint para logout
         [Authorize]
         [HttpGet("logout")]
          public async Task<IActionResult> Logout()
        {
          await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
          return Ok(new { message = "Sesión cerrada" });
        }


        // Health check
        [HttpGet("health")]
        public IActionResult HealthCheck()
        {
            return Ok(new { 
                status = "OK", 
                controller = "JoelController",
                estudiante = "Joel Cerrogrande",
                email = "tj.joel.cerrogrande.o@upds.net.bo",
                timestamp = DateTime.UtcNow
            });
        }
        // Endpoint para ver usuario autenticado
    [Authorize]
    [HttpGet("me")]
    public IActionResult GetCurrentUser()
    {
        var user = new
        {
            Name = User.FindFirst(ClaimTypes.Name)?.Value,
            Email = User.FindFirst(ClaimTypes.Email)?.Value,
            Provider = User.FindFirst(ClaimTypes.NameIdentifier)?.Issuer            
        };
        return Ok(user);
    }
    }
}