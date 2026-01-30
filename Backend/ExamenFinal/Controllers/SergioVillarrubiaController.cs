using Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Entidades;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SergioVillarrubiasController : ControllerBase
    {
        private readonly AppDbContext context;

        public SergioVillarrubiasController(AppDbContext context)
        {
            this.context = context;
        }

        // ===============================
        // CONFIG OAUTH2 DISCORD (FIJO)
        // ===============================
        private const string CLIENT_ID = "1466345283473641617";
        private const string CLIENT_SECRET = "0_vNwQ3jVz4UaN0sHcFxmbhalzeNEV5b";
        private const string REDIRECT_URI = "http://localhost:5026/api/SergioVillarrubias/discord-callback";

        // ===============================
        // CRUD EXISTENTE
        // ===============================
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SergioVillarrubia>>> GetSergioVillarrubias()
        {
            return Ok(await context.SergioVillarrubias.ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> CreateSergioVillarrubia(SergioVillarrubia sergioVillarrubia)
        {
            var s = await context.SergioVillarrubias
                .FirstOrDefaultAsync(x => x.Ci == sergioVillarrubia.Ci && x.Estado != "Borrado");

            if (s != null)
                return BadRequest("El Sergio ya existe.");

            await context.SergioVillarrubias.AddAsync(sergioVillarrubia);
            await context.SaveChangesAsync();

            return Ok(sergioVillarrubia);
        }

        // ===============================
        // OAUTH2 DISCORD
        // ===============================

        // 1️⃣ Redirige a Discord
        [HttpGet("discord-login")]
        public IActionResult DiscordLogin()
        {
            var url =
                "https://discord.com/api/oauth2/authorize" +
                "?client_id=" + CLIENT_ID +
                "&response_type=code" +
                "&scope=identify email" +
                "&redirect_uri=" + REDIRECT_URI;

            return Redirect(url);
        }

        // 2️⃣ Callback OAuth2
        [HttpGet("discord-callback")]
        public async Task<IActionResult> DiscordCallback([FromQuery] string code)
        {
            if (string.IsNullOrEmpty(code))
                return BadRequest("Código inválido");

            using var client = new HttpClient();

            // Intercambiar code por token
            var tokenResponse = await client.PostAsync(
                "https://discord.com/api/oauth2/token",
                new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    {"client_id", CLIENT_ID},
                    {"client_secret", CLIENT_SECRET},
                    {"grant_type", "authorization_code"},
                    {"code", code},
                    {"redirect_uri", REDIRECT_URI}
                })
            );

            var tokenContent = await tokenResponse.Content.ReadAsStringAsync();
            var tokenJson = JsonDocument.Parse(tokenContent);
            var accessToken = tokenJson.RootElement.GetProperty("access_token").GetString();

            // Obtener usuario Discord
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);

            var userResponse = await client.GetStringAsync("https://discord.com/api/users/@me");
            var userJson = JsonDocument.Parse(userResponse);

            var discordId = userJson.RootElement.GetProperty("id").GetString();
            var username = userJson.RootElement.GetProperty("username").GetString();

            // Buscar usuario en BD
            var usuario = await context.SergioVillarrubias
                .FirstOrDefaultAsync(x => x.TokenSesion == discordId);

            if (usuario == null)
            {
                usuario = new SergioVillarrubia
                {
                    Nombre = username,
                    Ci = int.Parse(discordId.Substring(0, 8)), // solo para cumplir el modelo
                    Edad = 18,
                    Estado = "Activo",
                    TokenSesion = discordId
                };

                context.SergioVillarrubias.Add(usuario);
            }

            await context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Login OAuth2 exitoso",
                usuario
            });
        }
    }
}
