// Archivo: Controllers/AuthController.cs
using Data;
using Entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase 
    {
        private readonly AppDbContext context;
        private readonly IHttpClientFactory httpClientFactory;
        
        // Configuración de Discord
        private readonly string DiscordClientId = "1465246687399252010";
        private readonly string DiscordClientSecret = "tWfNr2H_yXNHl0XoY_q_VSudljwOTZSD";
        private readonly string DiscordRedirectUri = "http://localhost:5026/api/auth/discord_callback_snaider";
        private const string DiscordAuthUrl = "https://discord.com/api/oauth2/authorize";
        private const string DiscordTokenUrl = "https://discord.com/api/oauth2/token";
        private const string DiscordUserUrl = "https://discord.com/api/users/@me";
        
        public AuthController(AppDbContext context, IHttpClientFactory httpClientFactory)
        {
            this.context = context;
            this.httpClientFactory = httpClientFactory;
        }
        
        // GET: api/auth/discord_login_snaider
        // Solo redirige a Discord para autenticación
        [HttpGet("discord_login_snaider")]
        public IActionResult DiscordLoginSnaider()
        {
            var state = Guid.NewGuid().ToString();
            
            // Guardar state en session por seguridad
            HttpContext.Session.SetString("oauth_state_snaider", state);
            
            var authUrl = $"{DiscordAuthUrl}?" +
                         $"client_id={DiscordClientId}" +
                         $"&redirect_uri={Uri.EscapeDataString(DiscordRedirectUri)}" +
                         $"&response_type=code" +
                         $"&scope=identify%20email" +
                         $"&state={state}";
            
            return Ok(new { 
                success = true, 
                authUrl = authUrl,
                message = "Usa esta URL para autenticarte con Discord" 
            });
        }
        
        // GET: api/auth/discord_callback_snaider
        // Callback simple: verifica login y genera token
        [HttpGet("discord_callback_snaider")]
        public async Task<IActionResult> DiscordCallbackSnaider([FromQuery] string code, [FromQuery] string state)
        {
            try
            {
                // Validar state
                var storedState = HttpContext.Session.GetString("oauth_state_snaider");
                if (state != storedState)
                    return BadRequest(new { success = false, message = "State inválido" });
                
                // 1. Obtener token de acceso de Discord
                var tokenResponse = await GetDiscordToken(code);
                if (tokenResponse == null || string.IsNullOrEmpty(tokenResponse.access_token))
                    return BadRequest(new { success = false, message = "Error con Discord OAuth" });
                
                // 2. Obtener información básica del usuario
                var discordUser = await GetDiscordUserInfo(tokenResponse.access_token);
                if (discordUser == null)
                    return BadRequest(new { success = false, message = "Error obteniendo información de Discord" });
                
                // 3. Generar token simple para nuestra app
                var appToken = GenerateSimpleToken(discordUser.id);
                
                // 4. Opcional: Guardar/actualizar usuario en nuestra DB
                await SaveOrUpdateUser(discordUser, tokenResponse);
                
                // 5. Limpiar session
                HttpContext.Session.Remove("oauth_state_snaider");
                
                return Ok(new {
                    success = true,
                    message = "Autenticación exitosa",
                    token = appToken,  // Token para nuestra app
                    user = new {
                        discordId = discordUser.id,
                        username = $"{discordUser.username}#{discordUser.discriminator}",
                        email = discordUser.email,
                        avatar = discordUser.avatar
                    },
                    expiresIn = 3600 // 1 hora en segundos
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    success = false, 
                    message = "Error interno",
                    error = ex.Message 
                });
            }
        }
        
        // POST: api/auth/verify_token_snaider
        // Verifica si un token es válido (solo para verificación)
        [HttpPost("verify_token_snaider")]
        public IActionResult VerifyTokenSnaider([FromBody] VerifyTokenRequest request)
        {
            if (string.IsNullOrEmpty(request?.Token))
                return BadRequest(new { success = false, message = "Token requerido" });
            
            try
            {
                var isValid = VerifySimpleToken(request.Token);
                
                return Ok(new {
                    success = true,
                    valid = isValid,
                    message = isValid ? "Token válido" : "Token inválido o expirado"
                });
            }
            catch
            {
                return Ok(new {
                    success = true,
                    valid = false,
                    message = "Token inválido"
                });
            }
        }
        
        // Métodos auxiliares privados
        
        private async Task<DiscordTokenResponse> GetDiscordToken(string code)
        {
            var client = httpClientFactory.CreateClient();
            
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("client_id", DiscordClientId),
                new KeyValuePair<string, string>("client_secret", DiscordClientSecret),
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
                new KeyValuePair<string, string>("code", code),
                new KeyValuePair<string, string>("redirect_uri", DiscordRedirectUri),
                new KeyValuePair<string, string>("scope", "identify email")
            });
            
            content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
            
            var response = await client.PostAsync(DiscordTokenUrl, content);
            
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<DiscordTokenResponse>(json);
            }
            
            return null;
        }
        
        private async Task<DiscordUserInfo> GetDiscordUserInfo(string accessToken)
        {
            var client = httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            
            var response = await client.GetAsync(DiscordUserUrl);
            
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<DiscordUserInfo>(json);
            }
            
            return null;
        }
        
        private string GenerateSimpleToken(string discordId)
        {
            // Token simple: discordId + timestamp + random
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var random = new Random().Next(1000, 9999);
            var tokenData = $"{discordId}|{timestamp}|{random}";
            
            // En Base64 para que se vea como token
            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(tokenData));
        }
        
        private bool VerifySimpleToken(string token)
        {
            try
            {
                var decoded = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(token));
                var parts = decoded.Split('|');
                
                if (parts.Length != 3)
                    return false;
                
                // Verificar timestamp (token válido por 24 horas)
                var timestamp = long.Parse(parts[1]);
                var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                var maxAge = 24 * 60 * 60; // 24 horas en segundos
                
                return (now - timestamp) < maxAge;
            }
            catch
            {
                return false;
            }
        }
        
        private async Task SaveOrUpdateUser(DiscordUserInfo discordUser, DiscordTokenResponse tokenResponse)
        {
            try
            {
                var existingUser = await context.Snaiders
                    .FirstOrDefaultAsync(s => s.DiscordId == discordUser.id);
                
                if (existingUser == null)
                {
                    // Crear nuevo usuario
                    var newUser = new Snaider
                    {
                        DiscordId = discordUser.id,
                        DiscordUsername = $"{discordUser.username}#{discordUser.discriminator}",
                        DiscordEmail = discordUser.email,
                        DiscordAvatar = discordUser.avatar,
                        DiscordVerified = discordUser.verified,
                        Nombre = discordUser.username, // Usar username como nombre por defecto
                        Estado = "Activo",
                        FechaRegistro = DateTime.UtcNow,
                        UltimoLogin = DateTime.UtcNow
                    };
                    
                    await context.Snaiders.AddAsync(newUser);
                }
                else
                {
                    // Actualizar último login
                    existingUser.UltimoLogin = DateTime.UtcNow;
                    existingUser.DiscordUsername = $"{discordUser.username}#{discordUser.discriminator}";
                    existingUser.DiscordAvatar = discordUser.avatar;
                }
                
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Solo loguear el error, no fallar la autenticación
                Console.WriteLine($"Error guardando usuario: {ex.Message}");
            }
        }
        
        // Clases para deserialización
        private class DiscordTokenResponse
        {
            public string access_token { get; set; }
            public string token_type { get; set; }
            public int expires_in { get; set; }
            public string refresh_token { get; set; }
            public string scope { get; set; }
        }
        
        private class DiscordUserInfo
        {
            public string id { get; set; }
            public string username { get; set; }
            public string discriminator { get; set; }
            public string email { get; set; }
            public bool verified { get; set; }
            public string avatar { get; set; }
            public string locale { get; set; }
        }
        
        public class VerifyTokenRequest
        {
            public string Token { get; set; }
        }
    }
}