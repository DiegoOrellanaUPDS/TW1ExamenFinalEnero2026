u	sing Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Discord;
using Microsoft.AspNetCore.Authorization;
using ExamenFinal.Entidades; // Verifica que este sea el namespace correcto
using ExamenFinal.Data;

namespace ExamenFinal.Controllers
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

        // 1. ENDPOINT PARA INICIAR LOGIN
        [HttpGet("login-discord")]
        public IActionResult Login()
        {
            // Esto dispara el flujo de Discord
            return Challenge(new AuthenticationProperties { RedirectUri = "/api/AdrianRojas/perfil" }, "Discord");
        }

        // 2. ENDPOINT PROTEGIDO (Cumple con el punto del examen)
        [Authorize]
        [HttpGet("perfil")]
        public IActionResult GetPerfil()
        {
            // Si el usuario llega aquí, es que OAuth funcionó
            var usuario = User.Identity?.Name;
            return Ok(new { mensaje = $"Bienvenido {usuario}, estás autenticado con Discord" });
        }

        // 3. TU POST (De la parte 1, ahora protegido)
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Post(AdrianRojas entidad)
        {
            _context.AdrianRojases.Add(entidad);
            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Entidad guardada con éxito", data = entidad });
        }
    }
}
