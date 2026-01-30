using Microsoft.AspNetCore.Mvc;
using Entidades;
using Data;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class BeymarController : ControllerBase
{
    private readonly AppDbContext _context;

    public BeymarController(AppDbContext context)
    {
        _context = context;
    }

    // PARTE 1: Endpoint para registrar datos
    [HttpPost]
    public async Task<IActionResult> Registrar([FromBody] Beymar entidad)
    {
        _context.Beymars.Add(entidad);
        await _context.SaveChangesAsync();
        return Ok(new { mensaje = "Registro exitoso realizado por Beymar", datos = entidad });
    }

    // PARTE 2: Autenticación OAuth 2 (Discord)
    [HttpGet("login-discord")]
    public IActionResult LoginDiscord()
    {
        var clientId = "1332836267784081491"; 
        var redirectUri = "http://localhost:5215/api/Beymar/callback";
        var url = $"https://discord.com/api/oauth2/authorize?client_id={clientId}&redirect_uri={Uri.EscapeDataString(redirectUri)}&response_type=code&scope=identify";
        return Redirect(url);
    }

    [HttpGet("callback")]
    public async Task<IActionResult> Callback(string code)
    {
        return Ok(new { 
            mensaje = "OAuth 2 Implementado por Beymar Vasquez", 
            code = code 
        });
    }
}