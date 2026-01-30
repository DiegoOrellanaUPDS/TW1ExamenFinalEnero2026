
using Data;
using Entidades;
using Microsoft.AspNetCore.Mvc;

namespace Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ArnoldController:ControllerBase
    {
        private readonly AppDbContext context;
        public ArnoldController(AppDbContext context)
        {
            this.context=context;
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