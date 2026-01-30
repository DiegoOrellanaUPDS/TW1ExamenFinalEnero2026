using System.Net.Http.Headers;
using System.Text.Json;
using Data;
using Entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HerberthController : ControllerBase
    {
        private readonly AppDbContext context;
        private readonly IHttpClientFactory httpClientFactory;
        public HerberthController(AppDbContext context, IHttpClientFactory httpClientFactory)
        {
            this.context = context;
            this.httpClientFactory = httpClientFactory;
        }

        private const string ClientId = "179933675790-u1r512l74hffep6a541su8p88lte80bf.apps.googleusercontent.com"; 
        private const string ClientSecret = "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";
        private const string RedirectUri = "http://localhost:5026/api/Herberth/callback";

        
        

        [HttpGet]
        public async Task<IActionResult> GetHerberth()
        {
            var herberth = await (from h in context.Herberth select h).ToListAsync();
            return Ok(herberth);
        }

        [HttpPost]
        public async Task<IActionResult> PostHerberth(Herberth herberth)
        {
            var existing = await (from h in context.Herberth where herberth.Id == h.Id select h).FirstOrDefaultAsync();
            if (existing != null) return BadRequest("Ya existe");

            return Ok("Se creo correctamente");
        }



        [HttpGet("login")]
        public IActionResult Login()
        {
            var url = "https://accounts.google.com/o/oauth2/v2/auth" +
                      $"?client_id={ClientId}" +
                      $"&redirect_uri={RedirectUri}" +
                      "&response_type=code" +
                      "&scope=openid%20profile%20email" +
                      "&access_type=offline";

            return Redirect(url);
        }

        // --- OAUTH GOOGLE: CALLBACK ---
        [HttpGet("callback")]
        public async Task<IActionResult> Callback([FromQuery] string code)
        {
            if (string.IsNullOrEmpty(code)) return BadRequest("No se recibió el código");

            var client = httpClientFactory.CreateClient();

            // 1. Intercambio de código por token
            var tokenResponse = await client.PostAsync("https://oauth2.googleapis.com/token", new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["client_id"] = ClientId,
                ["client_secret"] = ClientSecret,
                ["code"] = code,
                ["grant_type"] = "authorization_code",
                ["redirect_uri"] = RedirectUri
            }));

            var tokenBody = await tokenResponse.Content.ReadAsStringAsync();
            using var jsonDoc = JsonDocument.Parse(tokenBody);
            var accessToken = jsonDoc.RootElement.GetProperty("access_token").GetString();

            // 2. Obtener información del usuario (Nombre)
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var userResponse = await client.GetAsync("https://www.googleapis.com/oauth2/v3/userinfo");
            var userBody = await userResponse.Content.ReadAsStringAsync();
            using var userJson = JsonDocument.Parse(userBody);
            
            var googleNombre = userJson.RootElement.GetProperty("name").GetString();

            // 3. Buscar si el usuario ya existe en la tabla Herberth
            var registro = await context.Herberth.FirstOrDefaultAsync(h => h.Nombre == googleNombre);

            if (registro == null)
            {
                // Si no existe, lo creamos
                registro = new Herberth
                {
                    Nombre = googleNombre ?? "Usuario Google",
                    Edad = "0", // Google no da la edad por defecto
                    Token = accessToken,
                    Estado = "Activo"
                };
                context.Herberth.Add(registro);
            }
            else
            {
                // Si existe, actualizamos su token y estado
                registro.Token = accessToken;
                registro.Estado = "Activo";
            }

            await context.SaveChangesAsync();

            // Guardamos en sesión
            HttpContext.Session.SetString("GoogleToken", accessToken);
            HttpContext.Session.SetString("UsuarioNombre", googleNombre!);

            return Ok(new { mensaje = "Login exitoso", usuario = registro });
        }


    }
}