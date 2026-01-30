using Data;
using Entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Text.Json;

namespace ExamenFinal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MarceloAuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly AppDbContext _context;

        public MarceloAuthController(IConfiguration config, AppDbContext context)
        {
            _config = config;
            _context = context;
        }

        // ======================================================
        // CRUD MARCELO
        // ======================================================

        // GET: api/marcelo
        [HttpGet]
        public async Task<ActionResult<List<Marcelo>>> GetMarcelo()
        {
            var marcelo = await _context.Marcelo
                .AsNoTracking()
                .Where(m => m.Estado == "Activo")
                .ToListAsync();

            return Ok(marcelo);
        }

        // GET: api/marcelo/{token}
        [HttpGet("{token}")]
        public async Task<ActionResult<Marcelo>> GetMarcelo(string token)
        {
            var marcelo = await _context.Marcelo
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Token == token && m.Estado == "Activo");

            if (marcelo == null)
                return NotFound();

            return Ok(marcelo);
        }

        // GET: api/marcelo/inactivos
        [HttpGet("inactivos")]
        public async Task<ActionResult<List<Marcelo>>> GetMarceloInactivos()
        {
            var marcelo = await _context.Marcelo
                .AsNoTracking()
                .Where(m => m.Estado == "Inactivo")
                .ToListAsync();

            return Ok(marcelo);
        }

        // POST: api/marcelo
        [HttpPost]
        public async Task<ActionResult<Marcelo>> PostMarcelo(
            string nombre,
            int edad,
            string? rol)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                return BadRequest("El nombre es obligatorio");

            var marcelo = new Marcelo
            {
                Nombre = nombre,
                Edad = edad,
                Rol = string.IsNullOrWhiteSpace(rol) ? "user" : rol,
                Estado = "Activo",
                Token = Guid.NewGuid().ToString()
            };

            _context.Marcelo.Add(marcelo);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMarcelo),
                new { token = marcelo.Token }, marcelo);
        }

        // PUT: api/marcelo/{token}
        [HttpPut("{token}")]
        public async Task<ActionResult<Marcelo>> PutMarcelo(
            string token,
            string nombre,
            int edad,
            string rol)
        {
            var marcelo = await _context.Marcelo
                .FirstOrDefaultAsync(m => m.Token == token && m.Estado == "Activo");

            if (marcelo == null)
                return NotFound();

            marcelo.Nombre = nombre;
            marcelo.Edad = edad;
            marcelo.Rol = rol;

            await _context.SaveChangesAsync();

            return Ok(marcelo);
        }

        // DELETE: api/marcelo/{token}
        [HttpDelete("{token}")]
        public async Task<ActionResult<Marcelo>> DeleteMarcelo(string token)
        {
            var marcelo = await _context.Marcelo
                .FirstOrDefaultAsync(m => m.Token == token && m.Estado == "Activo");

            if (marcelo == null)
                return NotFound();

            marcelo.Estado = "Inactivo";
            await _context.SaveChangesAsync();

            return Ok(marcelo);
        }

        // PUT: api/marcelo/habilitar/{token}
        [HttpPut("habilitar/{token}")]
        public async Task<ActionResult<Marcelo>> HabilitarMarcelo(string token)
        {
            var marcelo = await _context.Marcelo
                .FirstOrDefaultAsync(m => m.Token == token && m.Estado == "Inactivo");

            if (marcelo == null)
                return NotFound();

            marcelo.Estado = "Activo";
            await _context.SaveChangesAsync();

            return Ok(marcelo);
        }
        
        // ======================================================
        // AUTH DISCORD (OAuth)
        // ======================================================

        // GET: api/marceloauth/discord-login
        [HttpGet("discord-login")]
        public IActionResult DiscordLogin()
        {
            var clientId = _config["DiscordOAuth:ClientId"];
            var redirectUri = _config["DiscordOAuth:RedirectUri"];
            var scope = "identify email";

            var url = $"https://discord.com/api/oauth2/authorize" +
                      $"?client_id={clientId}" +
                      $"&redirect_uri={Uri.EscapeDataString(redirectUri)}" +
                      $"&response_type=code" +
                      $"&scope={scope}";

            return Ok(url);
        }

        // GET: api/marceloauth/discord-callback
        [HttpGet("discord-callback")]
        public async Task<IActionResult> DiscordCallback([FromQuery] string code)
        {
            if (string.IsNullOrEmpty(code))
                return BadRequest("Código OAuth no recibido");

            using var client = new HttpClient();

            var form = new Dictionary<string, string>
            {
                { "client_id", _config["DiscordOAuth:ClientId"] },
                { "client_secret", _config["DiscordOAuth:ClientSecret"] },
                { "grant_type", "authorization_code" },
                { "code", code },
                { "redirect_uri", _config["DiscordOAuth:RedirectUri"] }
            };

            var tokenResponse = await client.PostAsync(
                "https://discord.com/api/oauth2/token",
                new FormUrlEncodedContent(form));

            if (!tokenResponse.IsSuccessStatusCode)
                return Unauthorized("Error al obtener access token");

            var tokenJson = JsonDocument.Parse(
                await tokenResponse.Content.ReadAsStringAsync());

            var accessToken = tokenJson.RootElement
                .GetProperty("access_token").GetString();

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);

            var userJson = await client.GetStringAsync(
                "https://discord.com/api/users/@me");

            var discordUser = JsonDocument.Parse(userJson);

            var username =
                discordUser.RootElement.GetProperty("username").GetString() +
                "#" +
                discordUser.RootElement.GetProperty("discriminator").GetString();

            var sessionToken = Guid.NewGuid().ToString();

            var usuario = await _context.Marcelo
                .FirstOrDefaultAsync(u => u.Nombre == username);

            if (usuario == null)
            {
                usuario = new Marcelo
                {
                    Nombre = username,
                    Edad = 0,
                    Rol = "user",
                    Token = sessionToken,
                    Estado = "Activo"
                };
                _context.Marcelo.Add(usuario);
            }
            else
            {
                usuario.Token = sessionToken;
                usuario.Estado = "Activo";
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Login exitoso",
                token = sessionToken,
                usuario = username
            });
        }

        // ======================================================
        // SESIÓN
        // ======================================================

        // GET: api/marceloauth/verificarSesion/{token}
        [HttpGet("verificarSesion/{token}")]
        public IActionResult VerificarSesion(string token)
        {
            var usuario = _context.Marcelo
                .FirstOrDefault(u => u.Token == token && u.Estado == "Activo");

            if (usuario == null)
                return Unauthorized("Sesión inválida");

            return Ok(new
            {
                mensaje = "Sesión válida",
                usuario = usuario.Nombre,
                rol = usuario.Rol
            });
        }

        // POST: api/marceloauth/logout/{token}
        [HttpPost("logout/{token}")]
        public IActionResult Logout(string token)
        {
            var usuario = _context.Marcelo
                .FirstOrDefault(u => u.Token == token);

            if (usuario == null)
                return Unauthorized("Usuario inexistente");

            usuario.Token = "-1";
            _context.SaveChanges();

            return Ok("Sesión cerrada correctamente");
        }
    }
}

