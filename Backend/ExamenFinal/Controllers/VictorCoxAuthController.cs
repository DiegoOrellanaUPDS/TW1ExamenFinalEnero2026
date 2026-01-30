using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http; // Required for ISession extensions
using Microsoft.EntityFrameworkCore;
using ExamenFinal.Data;
using ExamenFinal.Entidades;

namespace ExamenFinal.Controllers
{
    // Usamos esta ruta para que coincida con tu captura de Discord
    [Route("api/auth/ExamenFinal")] 
    [ApiController]
    public class VictorCoxAuthController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context; // Agregado el contexto de DB

        // TUS CREDENCIALES DE DISCORD (Copiadas de tu imagen)
        private const string ClientId = "1466431254676242434";
        private const string ClientSecret = "TU_CLIENT_SECRET_AQUI"; // <--- PEGA AQUÍ EL SECRET LAAARGO
        
        // Esta URL debe ser IDÉNTICA a la de tu captura de pantalla
        private const string RedirectUri = "http://localhost:5000/api/auth/ExamenFinal/callback";

        public VictorCoxAuthController(IHttpClientFactory httpClientFactory, IConfiguration configuration, AppDbContext context)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _context = context;
        }

        // 1. ENDPOINT PARA INICIAR LOGIN (Redirige a Discord)
        [HttpGet("login")]
        public IActionResult Login()
        {
            // URL oficial de autorización de Discord
            var discordAuthUrl = "https://discord.com/api/oauth2/authorize" +
                                 $"?client_id={ClientId}" +
                                 $"&redirect_uri={Uri.EscapeDataString(RedirectUri)}" +
                                 "&response_type=code" +
                                 "&scope=identify%20email"; // Pedimos identidad y email

            return Redirect(discordAuthUrl);
        }

        // 2. CALLBACK (Discord nos devuelve aquí con un código)
        [HttpGet("callback")]
        public async Task<IActionResult> Callback([FromQuery] string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return BadRequest("Error: No se recibió el código de autorización de Discord.");
            }

            // A) Intercambiar el CÓDIGO por un TOKEN
            var client = _httpClientFactory.CreateClient();
            
            var tokenRequestParams = new Dictionary<string, string>
            {
                { "client_id", ClientId },
                { "client_secret", ClientSecret },
                { "grant_type", "authorization_code" },
                { "code", code },
                { "redirect_uri", RedirectUri }
            };

            var tokenResponse = await client.PostAsync("https://discord.com/api/oauth2/token", new FormUrlEncodedContent(tokenRequestParams));

            if (!tokenResponse.IsSuccessStatusCode)
            {
                var errorContent = await tokenResponse.Content.ReadAsStringAsync();
                return BadRequest($"Error al obtener token: {errorContent}");
            }

            var tokenJson = await tokenResponse.Content.ReadAsStringAsync();
            using var jsonDoc = JsonDocument.Parse(tokenJson);
            var accessToken = jsonDoc.RootElement.GetProperty("access_token").GetString();

            // B) Usar el TOKEN para pedir los DATOS DEL USUARIO
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var userResponse = await client.GetAsync("https://discord.com/api/users/@me");

            if (!userResponse.IsSuccessStatusCode)
            {
                return BadRequest("Error al obtener datos del usuario.");
            }

            var userBody = await userResponse.Content.ReadAsStringAsync();
            var userInfo = JsonDocument.Parse(userBody);
            
            var username = userInfo.RootElement.GetProperty("username").GetString() ?? "UsuarioDesconocido";
            var email = "no-email@discord.com"; // Discord a veces no envía email si no está verificado o scope
            if(userInfo.RootElement.TryGetProperty("email", out var emailProp) && emailProp.ValueKind != JsonValueKind.Null)
            {
                email = emailProp.GetString();
            }

            // C) LÓGICA DE BASE DE DATOS (Guardar Token)
            var usuario = await _context.UsuariosVictorCox
                .FirstOrDefaultAsync(u => u.Nombre == username); // Buscamos por nombre (o email si prefieres)

            if (usuario == null)
            {
                usuario = new UsuarioVictorCox
                {
                    Nombre = username,
                    Correo = email,
                    Rol = "Admin ExamenFinal",
                    Estado = "Activo"
                };
                _context.UsuariosVictorCox.Add(usuario);
            }

            // Generar nuevo token de sesión local (UUID)
            usuario.TokenSesion = Guid.NewGuid().ToString();
            await _context.SaveChangesAsync();
            
            // Guardar en Sesión también (por si acaso)
            HttpContext.Session.SetString("TokenDiscord", accessToken);
            HttpContext.Session.SetString("UsuarioData", userBody);
            HttpContext.Session.SetString("TokenSesionDB", usuario.TokenSesion);

            // Retornamos los datos
            return Ok(new 
            { 
                Mensaje = "¡Autenticación Exitosa! Token guardado en BD.", 
                TokenSesion = usuario.TokenSesion,
                Usuario = usuario.Nombre,
                Rol = usuario.Rol,
                DatosDiscord = userInfo.RootElement 
            });
        }

        // 3. VERIFICAR SESIÓN (Nuevo Endpoint)
        [HttpGet("verificar")]
        public async Task<IActionResult> VerificarSesion([FromQuery] string token)
        {
            var usuario = await _context.UsuariosVictorCox
                .FirstOrDefaultAsync(u => u.TokenSesion == token);

            if (usuario == null || string.IsNullOrEmpty(token))
                return Unauthorized("Sesión inválida o expirada.");

            return Ok(new { Mensaje = "Sesión válida", Usuario = usuario.Nombre, Rol = usuario.Rol });
        }

        // 4. LOGOUT (Limpiar BD)
        [HttpPost("logout")] // POST es más correcto para acciones
        public async Task<IActionResult> Logout([FromQuery] string token)
        {
            var usuario = await _context.UsuariosVictorCox
                .FirstOrDefaultAsync(u => u.TokenSesion == token);

            if (usuario != null)
            {
                usuario.TokenSesion = ""; // Invalidar token
                await _context.SaveChangesAsync();
            }

            HttpContext.Session.Clear();
            return Ok(new { Mensaje = "Sesión cerrada correctamente." });
        }
    }
}