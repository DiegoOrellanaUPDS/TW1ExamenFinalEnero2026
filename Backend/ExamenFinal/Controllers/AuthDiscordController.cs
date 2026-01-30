using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using System.Security.Claims;

namespace ExamenFinal.Controllers
{
    [ApiController]
    [Route("api/auth/discord")]
    public class AuthDiscordController : ControllerBase
    {
        [HttpGet("login")]
        public IActionResult Login()
        {
            var redirectUrl = Url.Action("Callback", "AuthDiscord");
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, "Discord");
        }

        [HttpGet("callback")]
        public IActionResult Callback()
        {
            return Ok("Login con Discord exitoso");
        }
    }
}
