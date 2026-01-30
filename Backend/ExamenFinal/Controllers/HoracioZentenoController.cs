using Data;
using Entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HoracioZentenoController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _http;

        public HoracioZentenoController(AppDbContext context, IConfiguration config, IHttpClientFactory http)
        {
            _context = context;
            _config = config;
            _http = http;
        }

        // ================= LOGIN DISCORD =================
        [HttpGet("loginHoracio")]
        public IActionResult Login()
        {
            var clientId = _config["Discord:ClientId"];
            var redirectUri = "http://localhost:5081/api/HoracioZenteno/callback";
            var scope = "identify";

            var url = $"https://discord.com/api/oauth2/authorize?client_id={clientId}" +
                      $"&redirect_uri={Uri.EscapeDataString(redirectUri)}&response_type=code&scope={scope}";

            return Redirect(url);
        }

        [HttpGet("callback")]
        public async Task<IActionResult> Callback(string code)
        {
            var client = _http.CreateClient();

            var values = new Dictionary<string, string>
            {
                ["client_id"] = _config["Discord:ClientId"],
                ["client_secret"] = _config["Discord:ClientSecret"],
                ["grant_type"] = "authorization_code",
                ["code"] = code,
                ["redirect_uri"] = "http://localhost:5081/api/HoracioZenteno/callback"
            };

            var response = await client.PostAsync("https://discord.com/api/oauth2/token",
                new FormUrlEncodedContent(values));

            var tokenJson = await response.Content.ReadAsStringAsync();
            var tokenData = JsonSerializer.Deserialize<DiscordToken>(tokenJson);

            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {tokenData.access_token}");
            var userJson = await client.GetStringAsync("https://discord.com/api/users/@me");
            var discordUser = JsonSerializer.Deserialize<DiscordUser>(userJson);

            var usuario = await _context.HoracioZenteno
                .FirstOrDefaultAsync(u => u.DiscordId == discordUser.id);

            if (usuario == null)
            {
                usuario = new HoracioZenteno
                {
                    Nombre = discordUser.username,
                    Edad = 0,
                    Estado = "Activo",
                    DiscordId = discordUser.id,
                    Token = tokenData.access_token
                };
                _context.Add(usuario);
            }
            else
            {
                usuario.Token = tokenData.access_token;
            }

            await _context.SaveChangesAsync();

            HttpContext.Session.SetString("Token", tokenData.access_token);
            HttpContext.Session.SetInt32("UserId", usuario.Id);

            return Ok("Login correcto");
        }

        // ================= PROTECCIÓN =================
        private bool SesionValida()
        {
            var token = HttpContext.Session.GetString("Token");
            var id = HttpContext.Session.GetInt32("UserId");
            return !string.IsNullOrEmpty(token) && id.HasValue;
        }

        [HttpGet("gethoracio")]
        public async Task<IActionResult> GetHoracio()
        {
            if (!SesionValida()) return Unauthorized("Debes iniciar sesión");

            return Ok(await _context.HoracioZenteno.ToListAsync());
        }

        [HttpPost("posthoracio")]
        public async Task<IActionResult> PostHoracio([FromBody] HoracioZenteno data)
        {
            if (!SesionValida()) return Unauthorized("Debes iniciar sesión");

            _context.Add(data);
            await _context.SaveChangesAsync();
            return Ok(data);
        }
    }

    public class DiscordToken { public string access_token { get; set; } }
    public class DiscordUser { public string id { get; set; } public string username { get; set; } }
}
