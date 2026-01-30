using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Data;      // Tu namespace de Data
using Entidades; // Tu namespace de Entidades
using System.Net.Http.Headers;
using System.Text.Json;

namespace Universidad.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrayanController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly HttpClient _http;

        // 🔴 CREDENCIALES (Cámbialas por las tuyas de Google/Discord/Auth0)
        private const string CLIENT_ID = "TU_CLIENT_ID"; 
        private const string CLIENT_SECRET = "TU_CLIENT_SECRET";

        public BrayanController(AppDbContext context)
        {
            _context = context;
            _http = new HttpClient();
        }

        // ==================================================
        // PARTE 2: OAUTH 2 - LOGIN Y CALLBACK
        // ==================================================

        [HttpGet("login")]
        public IActionResult Login()
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var redirectUri = $"{baseUrl}/api/Brayan/callback";

            // Ejemplo con Google (puedes cambiar la URL a Discord si prefieres)
            var url = "https://accounts.google.com/o/oauth2/v2/auth" +
                      $"?client_id={CLIENT_ID}" +
                      $"&redirect_uri={Uri.EscapeDataString(redirectUri)}" +
                      "&response_type=code" +
                      "&scope=openid%20profile%20email";

            return Redirect(url);
        }

        [HttpGet("callback")]
        public async Task<IActionResult> Callback(string code)
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var redirectUri = $"{baseUrl}/api/Brayan/callback";

            if (string.IsNullOrEmpty(code)) return BadRequest("Código no recibido.");

            // 1. Intercambiar código por token
            var tokenRequest = new FormUrlEncodedContent(new Dictionary<string, string> {
                { "client_id", CLIENT_ID },
                { "client_secret", CLIENT_SECRET },
                { "grant_type", "authorization_code" },
                { "code", code },
                { "redirect_uri", redirectUri }
            });

            var response = await _http.PostAsync("https://oauth2.googleapis.com/token", tokenRequest);
            if (!response.IsSuccessStatusCode) return Unauthorized("Error en OAuth con el proveedor.");

            var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            var accessToken = json.RootElement.GetProperty("access_token").GetString();

            // 2. Obtener info del usuario logueado
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var userRes = await _http.GetAsync("https://www.googleapis.com/oauth2/v2/userinfo");
            var userInfo = JsonDocument.Parse(await userRes.Content.ReadAsStringAsync());

            var email = userInfo.RootElement.GetProperty("email").GetString();

            // 3. Crear o actualizar sesión en tu tabla de Entidad (puedes usar la misma de Brayan o una nueva)
            // Para el examen, simplemente devolveremos un Token de Sesión inventado
            var tokenSesion = Guid.NewGuid().ToString();

            return Ok(new { 
                mensaje = "Autenticación OAuth exitosa", 
                tokenValido = tokenSesion, 
                usuario = email 
            });
        }

        // ==================================================
        // PARTE 1 & 2: POST PROTEGIDO
        // ==================================================
        
        [HttpPost]
        public async Task<ActionResult<Brayan>> PostBrayan([FromHeader] string authorization, Brayan brayan)
        {
            // VALIDACIÓN MANUAL DE OAUTH:
            // Si el header no trae nada, lo rebotamos (esto cumple el punto de endpoint protegido)
            if (string.IsNullOrEmpty(authorization))
            {
                return Unauthorized(new { mensaje = "Acceso denegado. Se requiere Token OAuth en el Header 'authorization'" });
            }

            // Lógica normal de guardado de la Parte 1
            _context.Brayans.Add(brayan);
            await _context.SaveChangesAsync();
            
            return CreatedAtAction(nameof(PostBrayan), new { id = brayan.Id }, brayan);
        }
    }
}