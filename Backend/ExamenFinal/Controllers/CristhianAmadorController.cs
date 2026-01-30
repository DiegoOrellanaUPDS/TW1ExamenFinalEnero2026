using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Data;
using Entidades;

namespace CrsthianAmador.Controllers
{
    [ApiController]
    [Route("outh-Cristhian-Amador")]
    public class CristhianAmadorController : ControllerBase
    {
        private readonly AppDbContext context;
        private readonly IConfiguration config;
        private readonly IHttpClientFactory httpClientFactory;

        public CristhianAmadorController(AppDbContext context, IConfiguration config, IHttpClientFactory httpClientFactory)
        {
            this.context = context;
            this.config = config;
            this.httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CristhianAmador>>> GetCristhianAmador()
        {
            return Ok(await context.CristhianAmadors.Where(e => e.Estado != false).ToListAsync());
        }

        [HttpGet("{ci}")]
        public async Task<IActionResult> GetEstudiante(int ci)
        {
            var persona = await (from cris in context.CristhianAmadors
                                   where cris.Ci == ci && cris.Estado != false
                                   select cris).FirstOrDefaultAsync();
            if (persona == null)
                return NotFound();
            return Ok(persona);
        }
        [HttpPost]
        public async Task<IActionResult> CreateEstudiante(CristhianAmador persona)
        {
            var e = await (from cris in context.CristhianAmadors
                           where cris.Ci == persona.Ci && cris.Estado != false
                           select cris).FirstOrDefaultAsync();
            if (e != null)
                return BadRequest("El estudiante ya existe.");
        
            await context.CristhianAmadors.AddAsync(persona);
            await context.SaveChangesAsync();
            return Ok(persona);
        }

        [HttpGet("login")]
        public IActionResult Login()
        {
            var clientId = "1464997516956401684";
            var redirectUri = "http://localhost:5242/outh-Cristhian-Amador/callback-cristhian-amador";
            var scopes = "identify email";

            var url = $"https://discord.com/oauth2/authorize?" +
                    $"response_type=code&" +
                    $"client_id={clientId}&" +
                    $"redirect_uri={Uri.EscapeDataString(redirectUri)}&" +
                    $"scope={Uri.EscapeDataString(scopes)}";

            return Ok(Redirect(url));
        }



        [HttpGet("callback-cristhian-amador")]
        public async Task<IActionResult> Callback([FromQuery] string code)
        {
            if (string.IsNullOrEmpty(code))
                return BadRequest("No se recibió el código de Discord.");

            var ClientId = "1464997516956401684";
            var ClientSecret = "tttuvUAjV1vIyuauVJ07Jjur106SJUKB";
            var redirectUri = "http://localhost:5242/outh-Cristhian-Amador/callback-cristhian-amador"; 

            var client = httpClientFactory.CreateClient();

            var tokenResponse = await client.PostAsync(
                "https://discord.com/api/oauth2/token",
                new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["client_id"] = ClientId,
                    ["client_secret"] = ClientSecret,
                    ["grant_type"] = "authorization_code",
                    ["code"] = code,
                    ["redirect_uri"] = redirectUri
                })
            );

            if (!tokenResponse.IsSuccessStatusCode)
                return BadRequest("Error obteniendo token de Discord.");

            var tokenJson = await tokenResponse.Content.ReadAsStringAsync();
            var tokenData = System.Text.Json.JsonDocument.Parse(tokenJson).RootElement;
            string accessToken = tokenData.GetProperty("access_token").GetString();

            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

            var userResponse = await client.GetAsync("https://discord.com/api/users/@me");
            if (!userResponse.IsSuccessStatusCode)
                return BadRequest("Error obteniendo datos del usuario de Discord.");

            var userJson = await userResponse.Content.ReadAsStringAsync();
            var userData = System.Text.Json.JsonDocument.Parse(userJson).RootElement;

            string username = userData.GetProperty("username").GetString();

            string tokenSesion = Guid.NewGuid().ToString();
            var usuario = await context.CristhianAmadors
                .FirstOrDefaultAsync(u => u.Usuario == username);

            if (usuario == null)
            {
                usuario = new CristhianAmador
                {
                    Nombre = "Cristhian Amador",
                    Ci= 10628105,
                    Edad = 21,
                    Usuario = username,
                    Token = tokenSesion,
                    Estado = true,
                    
                };
                context.CristhianAmadors.Add(usuario);
            }
            else
            {
                usuario.Token = tokenSesion; 
            }

            await context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Usuario autenticado correctamente",
                token = tokenSesion
            });
        }
    }
}