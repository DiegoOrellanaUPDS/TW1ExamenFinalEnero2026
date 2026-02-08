using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Data;
using ExamenFinal.Entidades;
using Microsoft.AspNetCore.Mvc;

namespace ExamenFinal.Controllers
{
    [ApiController]
    [Route("wilson")]
    public class WilsonController : ControllerBase
    {
        private readonly AppDbContext context;
        private readonly IHttpClientFactory httpClientFactory;

        private readonly string _tenantId = "56de9580-2613-4024-9e29-a0aea78f7ade";
        private readonly string _clientId = "37b6cd21-bc3a-4556-bc9d-7511cfbd356e";
        private readonly string _clientSecret = "ONE8Q~R3W.rboaI95LimUMmyJ-OaAqW2eJPM7bvP";
        private readonly string _redirectUri = "http://localhost:5026/wilson/auth/callback";

        public WilsonController(AppDbContext context,IHttpClientFactory httpClientFactory)
        {
            this.context=context;
            this.httpClientFactory = httpClientFactory;
        }
        [HttpPost("crear")]
        public async Task<IActionResult> PostWilson(Wilson e)
        {        
            e.estado = "Activo";
            await context.Wilson.AddAsync(e);
            await context.SaveChangesAsync();
            return Ok(e);
        }
        [HttpGet("login")]
        public IActionResult Login()
        {
            var url =
                $"https://login.microsoftonline.com/{_tenantId}/oauth2/v2.0/authorize" +
                $"?client_id={_clientId}" +
                $"&response_type=code" +
                $"&redirect_uri={_redirectUri}" +
                $"&response_mode=query" +
                $"&scope=openid profile email";

            return Redirect(url);
        }

        // ======================
        // CALLBACK
        // ======================
        [HttpGet("auth/callback")]
        public async Task<IActionResult> Callback([FromQuery] string code)
        {
            if (string.IsNullOrEmpty(code))
                return BadRequest("No llegó el code.");

            var client = httpClientFactory.CreateClient();

            var tokenRequest = new Dictionary<string, string>
            {
                { "client_id", _clientId },
                { "scope", "openid profile email" },
                { "code", code },
                { "redirect_uri", _redirectUri },
                { "grant_type", "authorization_code" },
                { "client_secret", _clientSecret }
            };

            var response = await client.PostAsync(
                $"https://login.microsoftonline.com/{_tenantId}/oauth2/v2.0/token",
                new FormUrlEncodedContent(tokenRequest)
            );

            if (!response.IsSuccessStatusCode)
                return BadRequest("Error al obtener token.");

            var json = await response.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(json);

            var accessToken = doc.RootElement.GetProperty("access_token").GetString();

            return Ok(new
            {
                mensaje = "Autenticación correcta",
                accessToken
            });
        }
    }
}