using Data;
using Entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Controllers
{
    [ApiController]
    [Route("authjavier-aramayo")]
    public class JavierAramayoController : ControllerBase
    {
        private readonly IConfiguration configuration;
        IHttpClientFactory httpClientFactory;
        private readonly AppDbContext context;
        public JavierAramayoController (IConfiguration configuration,IHttpClientFactory httpClientFactory,AppDbContext context)
        {
            this.configuration = configuration;
            this.httpClientFactory = httpClientFactory;
            this.context = context;
        }
        [HttpGet("ListarUsuario-JavierAramayo")]
        public async Task<ActionResult<IEnumerable<JavierAramayo>>> GetUsuario()
        {
             return await context.JavierAramayos.ToListAsync();
            
        }

        [HttpPost("AnadirElUsuario-JavierAramayo")]
        public async Task<ActionResult<JavierAramayo>> PostUsuario(JavierAramayo usuario )
        {
            var xd = await (from u in context.JavierAramayos
                            where u.ci ==usuario.ci
                            select u).FirstOrDefaultAsync();
            if(xd != null)
            {
                return BadRequest("El usuario con este ci ya existe");
            }
        await context.JavierAramayos.AddAsync(usuario);
        await context.SaveChangesAsync();
        return Ok(usuario);
        }

        [HttpGet("login-JavierAramayoo")]
        public async Task<IActionResult>Login()
        {
            var ClientId = "1465001775135195361";
            var redirectUri = "http://localhost:5242/authjavier-aramayo/callback-javier-aramayo";
            
            var url =
                "https://discord.com/oauth2/authorize" +
                "?response_type=code" +
                $"&client_id={ClientId}" +
                $"&redirect_uri={redirectUri}" +
                "&scope=identify";

            return Ok(Redirect(url));
        }

        [HttpGet("callback-javier-aramayo")]
        public async Task<IActionResult> Callback([FromQuery] string code)
        {
            if (string.IsNullOrEmpty(code))
                return BadRequest("No se recibió el código de Discord.");

            var ClientId = "1465001775135195361";
            var ClientSecret = "koN5ftIJmyzcX1PjEZjtn26I1EWBt0hu";
            var redirectUri = "http://localhost:5242/authjavier-aramayo/callback-javier-aramayo"; 

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
            var usuario = await context.JavierAramayos
                .FirstOrDefaultAsync(u => u.usuarioDc == username);

            if (usuario == null)
            {
                usuario = new JavierAramayo
                {
                    nombre = "Javier Aramayo",
                    ci= "10691917",
                    edad = 21,
                    estado = "activo",
                    usuarioDc = username,
                    tokenDc = tokenSesion,
                    
                };
                context.JavierAramayos.Add(usuario);
            }
            else
            {
                usuario.tokenDc = tokenSesion; 
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