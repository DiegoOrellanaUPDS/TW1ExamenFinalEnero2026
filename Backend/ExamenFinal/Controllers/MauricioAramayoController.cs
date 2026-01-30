using Data;
using ExamenFinal.Entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ExamenFinal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MauricioAramayoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MauricioAramayoController(AppDbContext context)
        {
            _context = context;
        }

        // 🔓 GET público – Swagger verde
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_context.MauricioAramayos.ToList());
        }

        // 🔒 POST protegido con OAuth
        [Authorize]
        [HttpPost]
        public IActionResult Post(MauricioAramayo data)
        {
            _context.MauricioAramayos.Add(data);
            _context.SaveChanges();
            return Ok(data);
        }
    }
}
