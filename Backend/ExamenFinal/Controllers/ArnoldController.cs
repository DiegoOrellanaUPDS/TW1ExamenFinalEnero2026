
using Data;
using Entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Controllers
{
    [ApiController]
    [Route("oauthArnold")]
    public class ArnoldController:ControllerBase
    {
        private readonly AppDbContext context;

        private readonly IHttpClientFactory httpClientFactory;
        private const string ClientId="1376422560436141";
        private const string ClientSecret="06bcbc0d76588c996632546c5184d3d6";
        public ArnoldController(AppDbContext context,IHttpClientFactory httpClientFactory)
        {
            this.context=context;
            this.httpClientFactory=httpClientFactory;
        }

        [HttpGet("login")]
        public async Task<IActionResult> LoginArnold()
        {


            
            var redirectUri = "http://localhost:5026/oauthArnold/callback-facebook";

            var url = "http://www.facebook.com/v18.0/dialog/oauth" +
              $"?client_id={ClientId}" +
              $"&redirect_uri={redirectUri}" +
              "&scope=public_profile"; 



            return Redirect(url);
        }



        [HttpGet("callback-facebook")]

        public async Task<IActionResult> CallbackArnold([FromQuery] string code)
        {

            var redirectUri = "http://localhost:5026/oauthArnold/callback-facebook";
            var client = httpClientFactory.CreateClient();

            // 1. Intercambio de Code por Token
            // Facebook usa GET para esto (a diferencia de Discord que usa POST)
            var tokenUrl = "https://graph.facebook.com/v18.0/oauth/access_token" +
                        $"?client_id={ClientId}" +
                        $"&redirect_uri={redirectUri}" +
                        $"&client_secret={ClientSecret}" +
                        $"&code={code}";

            var response = await client.GetAsync(tokenUrl);
            var body = await response.Content.ReadAsStringAsync();

            // Sacamos el token con tu técnica
            var token = body.Split(",").First(x => x.Contains("\"access_token\"")).Split(":")[1].Replace("\"", "").Trim();

            // 2. Pedir el nombre del usuario (Graph API)
            // En Facebook, especificamos qué campos queremos en la URL (?fields=name)
            var profileUrl = $"https://graph.facebook.com/me?fields=name&access_token={token}";
            var resProfile = await client.GetAsync(profileUrl);
            var bodyProfile = await resProfile.Content.ReadAsStringAsync();

            // En Facebook el campo se llama "name"
            var nombreFacebook = bodyProfile.Split(",").First(x => x.Contains("\"name\"")).Split(":")[1].Replace("\"", "").Replace("}", "").Trim();

            // 3. Guardar en PostgreSQL (Igual que los anteriores)
            var usuarioLocal = await context.Arnolds.FirstOrDefaultAsync(u => u.Nombre == nombreFacebook);

            if (usuarioLocal == null)
            {
                var nuevo = new Arnold { Nombre = nombreFacebook, Token = token,Edad=22 };
                context.Arnolds.Add(nuevo);
                await context.SaveChangesAsync();
                HttpContext.Session.SetString("UsuarioId", nuevo.Id.ToString());
            }
            else
            {
                usuarioLocal.Token = token;
                await context.SaveChangesAsync();
                HttpContext.Session.SetString("UsuarioId", usuarioLocal.Id.ToString());
            }

        return Ok("Conectado con Facebook correctamente");
    }




        [HttpPost]
        public async Task<IActionResult> PostArnold(Arnold user)
        {
            context.Arnolds.Add(user);
            await context.SaveChangesAsync();
            return Ok("Usuario guardado");
        }
    }
}