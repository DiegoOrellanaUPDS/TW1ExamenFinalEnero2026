using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data;
using ExamenFinal.Entidades;
using Microsoft.AspNetCore.Mvc;

namespace ExamenFinal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WilsonController : ControllerBase
    {
        private readonly AppDbContext context;
        public WilsonController(AppDbContext context)
        {
            this.context=context;
        }
        [HttpPost("wilson/crear")]
        public async Task<IActionResult> PostWilson(Wilson e)
        {        
            e.estado = "Activo";
            await context.Wilson.AddAsync(e);
            await context.SaveChangesAsync();
            return Ok(e);
        }
    }
}