using Data;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;
using Entidades;

namespace Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SebastianOAuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly HttpClient _http;

        private const string DISCORD_CLIENT_ID = "1466791126957817898";
        private const string DISCORD_CLIENT_SECRET = "LIzBL7-G2mDYLCdwvi99XYDEMQkx1bP3";
        private const string REDIRECT_URI = "http://localhost:5242/api/sebastianoauth/callback";

        public SebastianOAuthController(AppDbContext context)
        {
            _context = context;
            _http = new HttpClient();
        }

        // Redirige al login de Discord
        [HttpGet("login")]
        public IActionResult Login()
        {
            var url = $"https://discord.com/api/oauth2/authorize?client_id={DISCORD_CLIENT_ID}&redirect_uri={REDIRECT_URI}&response_type=code&scope=identify%20email";
            return Redirect(url);
        }

        // Callback de Discord después del login
        [HttpGet("callback")]
        public async Task<IActionResult> Callback(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return BadRequest(new { mensaje = "Código de autorización no recibido" });
            }

            try
            {
                // Intercambiar código por access token
                var tokenRequest = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "client_id", DISCORD_CLIENT_ID },
                    { "client_secret", DISCORD_CLIENT_SECRET },
                    { "grant_type", "authorization_code" },
                    { "code", code },
                    { "redirect_uri", REDIRECT_URI }
                });

                var tokenResponse = await _http.PostAsync("https://discord.com/api/oauth2/token", tokenRequest);
                tokenResponse.EnsureSuccessStatusCode();
                
                var tokenJson = JsonDocument.Parse(await tokenResponse.Content.ReadAsStringAsync());
                var accessToken = tokenJson.RootElement.GetProperty("access_token").GetString();

                // Obtener info del usuario de Discord
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var userInfo = await _http.GetFromJsonAsync<JsonElement>("https://discord.com/api/users/@me");
                
                var discordId = userInfo.GetProperty("id").GetString();
                var username = userInfo.GetProperty("username").GetString();
                var discriminator = userInfo.GetProperty("discriminator").GetString();
                
                // Email puede ser null
                string email;
                try
                {
                    email = userInfo.GetProperty("email").GetString() ?? $"{username}@discord.com";
                }
                catch
                {
                    email = $"{username}@discord.com";
                }

                // Crear token de sesión
                var tokenSesion = Guid.NewGuid().ToString();

                return Ok(new 
                { 
                    mensaje = "Login exitoso con Discord - Sebastian",
                    token = tokenSesion,
                    discordId = discordId,
                    username = username,
                    email = email,
                    discriminator = discriminator
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error en autenticación", error = ex.Message });
            }
        }

        // Verificar sesión con token
        [HttpGet("verificar-sesion")]
        public IActionResult VerificarSesion([FromQuery] string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized(new { mensaje = "Token no proporcionado" });
            }

            return Ok(new { mensaje = "Token válido", valido = true });
        }

        // Logout
        [HttpPost("logout")]
        public IActionResult Logout([FromHeader] string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized(new { mensaje = "Token no proporcionado" });
            }

            return Ok(new { mensaje = "Sesión cerrada exitosamente" });
        }

        // Obtener info del usuario autenticado
        [HttpGet("user-info")]
        public IActionResult GetUserInfo([FromQuery] string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized(new { mensaje = "Token no proporcionado" });
            }

            return Ok(new 
            { 
                mensaje = "Usuario autenticado con Discord",
                authenticated = true,
                proveedor = "Discord"
            });
        }
    }
}