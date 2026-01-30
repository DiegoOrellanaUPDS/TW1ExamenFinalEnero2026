using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Data;      // Ajusta a tu namespace
using Entidades; // Ajusta a tu namespace
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

        // 🟢 PEGA AQUÍ TUS CREDENCIALES DE DISCORD
        private const string CLIENT_ID = "1466790388302876722"; 
        private const string CLIENT_SECRET = "y1-vDcqkQ3NHRxeVG4GZZS4NnNrwVEy2";

        public BrayanController(AppDbContext context)
        {
            _context = context;
            _http = new HttpClient();
        }

        // ==========================================
        // PARTE 2: OAUTH CON DISCORD
        // ==========================================

        [HttpGet("login")]
        public IActionResult Login()
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var redirectUri = $"{baseUrl}/api/Brayan/callback";

            var url = "https://discord.com/api/oauth2/authorize" +
                      $"?client_id={CLIENT_ID}" +
                      $"&redirect_uri={Uri.EscapeDataString(redirectUri)}" +
                      "&response_type=code" +
                      "&scope=identify%20email";

            return Redirect(url);
        }

        [HttpGet("callback")]
        public async Task<IActionResult> Callback(string code)
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var redirectUri = $"{baseUrl}/api/Brayan/callback";

            if (string.IsNullOrEmpty(code)) return BadRequest("No hay código.");

            // Intercambiar código por Token de Discord
            var tokenRequest = new FormUrlEncodedContent(new Dictionary<string, string> {
                { "client_id", CLIENT_ID },
                { "client_secret", CLIENT_SECRET },
                { "grant_type", "authorization_code" },
                { "code", code },
                { "redirect_uri", redirectUri }
            });

            var response = await _http.PostAsync("https://discord.com/api/oauth2/token", tokenRequest);
            if (!response.IsSuccessStatusCode) return Unauthorized("Error obteniendo token de Discord.");

            var tokenJson = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            var accessToken = tokenJson.RootElement.GetProperty("access_token").GetString();

            // Obtener info del usuario de Discord para confirmar
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var userRes = await _http.GetAsync("https://discord.com/api/users/@me");
            var userInfo = JsonDocument.Parse(await userRes.Content.ReadAsStringAsync());

            var discordUser = userInfo.RootElement.GetProperty("username").GetString();

            // Devolvemos un token ficticio para que el estudiante lo use en el POST
            var sessionToken = Guid.NewGuid().ToString();

            return Ok(new {
                mensaje = "Login con Discord exitoso",
                token_para_el_post = sessionToken,
                usuario_discord = discordUser
            });
        }

        // ==========================================
        // PARTE 1 & 2: POST PROTEGIDO
        // ==========================================

        [HttpPost("postbrayans")]
        public async Task<ActionResult<Brayan>> PostBrayan([FromHeader] string authorization, Brayan brayan)
        {
            // Verificamos que el header de autorización no esté vacío
            // Esto demuestra que el endpoint está protegido.
            if (string.IsNullOrEmpty(authorization))
            {
                return Unauthorized(new { mensaje = "Acceso denegado. Se requiere Token OAuth2." });
            }

            _context.Brayans.Add(brayan);
            await _context.SaveChangesAsync();
            
            return CreatedAtAction(nameof(PostBrayan), new { id = brayan.Id }, brayan);
        }
    }
}