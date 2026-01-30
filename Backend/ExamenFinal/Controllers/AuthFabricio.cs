using Data;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;
using Entidades;

namespace Universidad.Controllers
{
    [ApiController]
    [Route("api/auth/fabricio")]
    public class AuthFabricioController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly AppDbContext _context;
        private readonly HttpClient _http;

        public AuthFabricioController(IConfiguration config, AppDbContext context)
        {
            _config = config;
            _context = context;
            _http = new HttpClient();
        }

        // REDIRIGE AL LOGIN DE DISCORD
        // http://localhost:5026/api/auth/fabricio/login
        [HttpGet("login")]
        public IActionResult Login()
        {
            var clientId = _config["DiscordOAuth:ClientId"];
            var redirectUri = "http://localhost:5026/api/auth/fabricio/callback"; // Cambié el puerto
            var url = $"https://discord.com/api/oauth2/authorize?client_id={clientId}&redirect_uri={redirectUri}&response_type=code&scope=identify";
            return Redirect(url);
        }

        // CALLBACK DE DISCORD DESPUÉS DEL LOGIN
        [HttpGet("callback")]
        public async Task<IActionResult> Callback(string code)
        {
            var clientId = _config["DiscordOAuth:ClientId"];
            var clientSecret = _config["DiscordOAuth:ClientSecret"];
            var redirectUri = "http://localhost:5026/api/auth/fabricio/callback"; // Cambié el puerto

            var tokenRequest = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "client_id", clientId },
                { "client_secret", clientSecret },
                { "grant_type", "authorization_code" },
                { "code", code },
                { "redirect_uri", redirectUri }
            });

            var tokenResponse = await _http.PostAsync("https://discord.com/api/oauth2/token", tokenRequest);
            tokenResponse.EnsureSuccessStatusCode();
            var tokenJson = JsonDocument.Parse(await tokenResponse.Content.ReadAsStringAsync());
            var accessToken = tokenJson.RootElement.GetProperty("access_token").GetString();

            // Obtener info del usuario
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var userInfo = await _http.GetFromJsonAsync<JsonElement>("https://discord.com/api/users/@me");
            var username = userInfo.GetProperty("username").GetString();

            // Revisar si ya existe en la DB
            var persona = _context.Fabricios.FirstOrDefault(p => p.Nombre == username);
            if (persona == null)
            {
                persona = new Fabricio
                {
                    Nombre = username,
                    Edad = 18,
                    Estado = true
                };
                _context.Fabricios.Add(persona);
                await _context.SaveChangesAsync();
            }

            // Crear token de sesión temporal
            var tokenSesion = Guid.NewGuid().ToString();

            return Ok(new { token = tokenSesion, persona.Nombre, persona.Edad, persona.Estado });
        }

        // VERIFICAR SESIÓN
        [HttpGet("verificarSesion")]
        public IActionResult VerificarSesion([FromQuery] string token)
        {
            if (string.IsNullOrEmpty(token))
                return Unauthorized();

            return Ok(new { mensaje = "Token válido" });
        }

        // LOGOUT
        [HttpPost("logout")]
        public IActionResult Logout([FromHeader] string token)
        {
            if (string.IsNullOrEmpty(token))
                return Unauthorized();

            return Ok(new { mensaje = "Sesión cerrada" });
        }
    }
}
