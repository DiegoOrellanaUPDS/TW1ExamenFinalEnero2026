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
        
        private readonly string DiscordClientId = "1465246687399252010";
        private readonly string DiscordClientSecret = "tWfNr2H_yXNHl0XoY_q_VSudljwOTZSD";
        private readonly string DiscordRedirectUri = "http://localhost:5024/api/auth/discord_callback_snaider";
        private const string DiscordAuthUrl = "https://discord.com/api/oauth2/authorize";
        private const string DiscordTokenUrl = "https://discord.com/api/oauth2/token";
        private const string DiscordUserUrl = "https://discord.com/api/users/@me";
        
        public AuthController(AppDbContext context, IHttpClientFactory httpClientFactory)
        {
            this.context = context;
            this.httpClientFactory = httpClientFactory;
        }
        
        // GET: api/auth/discord_login_snaider
        [HttpGet("discord_login_snaider")]
        public IActionResult DiscordLoginSnaider([FromQuery] int? ci = null)
        {
            var state = Guid.NewGuid().ToString();
            
            HttpContext.Session.SetString("oauth_state_snaider", state);
            if (ci.HasValue)
            {
                HttpContext.Session.SetInt32("oauth_ci_snaider", ci.Value);
            }
            
            var authUrl = $"{DiscordAuthUrl}?" +
                         $"client_id={DiscordClientId}" +
                         $"&redirect_uri={Uri.EscapeDataString(DiscordRedirectUri)}" +
                         $"&response_type=code" +
                         $"&scope=identify%20email" +
                         $"&state={state}" +
                         $"&prompt=consent";
            
            return Redirect(authUrl);
        }
        
        // GET: api/auth/discord_callback_snaider
        [HttpGet("discord_callback_snaider")]
        public async Task<IActionResult> DiscordCallbackSnaider([FromQuery] string code, [FromQuery] string state)
        {
            var storedState = HttpContext.Session.GetString("oauth_state_snaider");
            if (state != storedState)
                return BadRequest("State inválido");
            
            var tokenResponse = await ExchangeCodeForTokenSnaider(code);
            if (tokenResponse == null)
                return BadRequest("Error obteniendo token de Discord");
            
            var discordUser = await GetDiscordUserInfoSnaider(tokenResponse.AccessToken);
            if (discordUser == null)
                return BadRequest("Error obteniendo información de usuario");
            
            var ci = HttpContext.Session.GetInt32("oauth_ci_snaider");
            
            if (ci.HasValue)
            {
                return await LinkDiscordToExistingUserSnaider(ci.Value, discordUser, tokenResponse);
            }
            else
            {
                var existingUser = await context.Snaiders
                    .FirstOrDefaultAsync(s => s.DiscordId == discordUser.Id && s.Estado != "Borrado");
                
                if (existingUser != null)
                {
                    return await UpdateDiscordInfoSnaider(existingUser, discordUser, tokenResponse);
                }
                else
                {
                    return Ok(new {
                        message = "No tienes una cuenta registrada. Por favor regístrate primero.",
                        discordInfo = new {
                            id = discordUser.Id,
                            username = discordUser.Username,
                            email = discordUser.Email,
                            discriminator = discordUser.Discriminator
                        },
                        registerUrl = "/api/snaider/create_snaider"
                    });
                }
            }
        }
        
        private async Task<IActionResult> LinkDiscordToExistingUserSnaider(int ci, DiscordUserInfoSnaider discordUser, DiscordTokenResponseSnaider tokenResponse)
        {
            var snaider = await context.Snaiders
                .FirstOrDefaultAsync(s => s.Ci == ci && s.Estado != "Borrado");
            
            if (snaider == null)
                return NotFound($"No se encontró usuario con CI: {ci}");
            
            var discordInUse = await context.Snaiders
                .AnyAsync(s => s.DiscordId == discordUser.Id && s.Id != snaider.Id && s.Estado != "Borrado");
            
            if (discordInUse)
                return BadRequest("Esta cuenta de Discord ya está vinculada a otro usuario");
            
            snaider.DiscordId = discordUser.Id;
            snaider.DiscordUsername = $"{discordUser.Username}#{discordUser.Discriminator}";
            snaider.DiscordEmail = discordUser.Email;
            snaider.DiscordToken = tokenResponse.AccessToken;
            snaider.DiscordRefreshToken = tokenResponse.RefreshToken;
            snaider.DiscordTokenExpiry = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn);
            
            await context.SaveChangesAsync();
            
            HttpContext.Session.Remove("oauth_state_snaider");
            HttpContext.Session.Remove("oauth_ci_snaider");
            
            return Ok(new {
                message = "Cuenta de Discord vinculada exitosamente",
                user = new {
                    id = snaider.Id,
                    ci = snaider.Ci,
                    nombre = snaider.Nombre,
                    edad = snaider.Edad,
                    discordUsername = snaider.DiscordUsername
                },
                token = GenerateJwtTokenSnaider(snaider.Id.ToString())
            });
        }
        
        private async Task<IActionResult> UpdateDiscordInfoSnaider(Snaider snaider, DiscordUserInfoSnaider discordUser, DiscordTokenResponseSnaider tokenResponse)
        {
            snaider.DiscordUsername = $"{discordUser.Username}#{discordUser.Discriminator}";
            snaider.DiscordEmail = discordUser.Email;
            snaider.DiscordToken = tokenResponse.AccessToken;
            snaider.DiscordRefreshToken = tokenResponse.RefreshToken;
            snaider.DiscordTokenExpiry = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn);
            
            await context.SaveChangesAsync();
            
            return Ok(new {
                message = "Sesión iniciada con Discord",
                user = new {
                    id = snaider.Id,
                    ci = snaider.Ci,
                    nombre = snaider.Nombre,
                    edad = snaider.Edad,
                    discordUsername = snaider.DiscordUsername
                },
                token = GenerateJwtTokenSnaider(snaider.Id.ToString())
            });
        }
        
        // POST: api/auth/unlink_discord_snaider/{ci}
        [HttpPost("unlink_discord_snaider/{ci}")]
        public async Task<IActionResult> UnlinkDiscordSnaider(int ci)
        {
            var snaider = await context.Snaiders
                .FirstOrDefaultAsync(s => s.Ci == ci && s.Estado != "Borrado");
            
            if (snaider == null)
                return NotFound("Usuario no encontrado");
            
            snaider.DiscordId = null;
            snaider.DiscordUsername = null;
            snaider.DiscordEmail = null;
            snaider.DiscordToken = null;
            snaider.DiscordRefreshToken = null;
            snaider.DiscordTokenExpiry = null;
            
            await context.SaveChangesAsync();
            
            return Ok(new {
                message = "Cuenta de Discord desvinculada exitosamente",
                user = new {
                    id = snaider.Id,
                    ci = snaider.Ci,
                    nombre = snaider.Nombre
                }
            });
        }
        
        // GET: api/auth/me_snaider
        [HttpGet("me_snaider")]
        public async Task<IActionResult> GetCurrentUserSnaider()
        {
            var authHeader = Request.Headers["Authorization"].ToString();
            
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                return Unauthorized();
            
            return Ok(new {
                message = "Endpoint protegido - Aquí iría la información del usuario Snaider"
            });
        }
        
        private async Task<DiscordTokenResponseSnaider?> ExchangeCodeForTokenSnaider(string code)
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
                return JsonSerializer.Deserialize<DiscordTokenResponseSnaider>(json);
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error obteniendo token: {error}");
            }
            
            return null;
        }
        
        private async Task<DiscordUserInfoSnaider?> GetDiscordUserInfoSnaider(string accessToken)
        {
            var client = httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            
            var response = await client.GetAsync(DiscordUserUrl);
            
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<DiscordUserInfoSnaider>(json);
            }
            
            return null;
        }
        
        private string GenerateJwtTokenSnaider(string userId)
        {
            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{userId}:{DateTime.UtcNow.Ticks}"));
        }
        
        // Clases para deserialización (con extensión _snaider)
        public class DiscordTokenResponseSnaider
        {
            [JsonPropertyName("access_token")]
            public string AccessToken { get; set; }
            
            [JsonPropertyName("token_type")]
            public string TokenType { get; set; }
            
            [JsonPropertyName("expires_in")]
            public int ExpiresIn { get; set; }
            
            [JsonPropertyName("refresh_token")]
            public string RefreshToken { get; set; }
            
            [JsonPropertyName("scope")]
            public string Scope { get; set; }
        }
        
        public class DiscordUserInfoSnaider
        {
            [JsonPropertyName("id")]
            public string Id { get; set; }
            
            [JsonPropertyName("username")]
            public string Username { get; set; }
            
            [JsonPropertyName("discriminator")]
            public string Discriminator { get; set; }
            
            [JsonPropertyName("email")]
            public string Email { get; set; }
            
            [JsonPropertyName("verified")]
            public bool Verified { get; set; }
            
            [JsonPropertyName("avatar")]
            public string Avatar { get; set; }
        }
    }
}