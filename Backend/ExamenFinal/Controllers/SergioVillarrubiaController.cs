using Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Entidades;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SergioVillarrubiasController : ControllerBase
    {
        private AppDbContext context;
        public SergioVillarrubiasController(AppDbContext context)
        {
            this.context = context;
        }
        private static List<SergioVillarrubia> sergioVillarrubias = new List<SergioVillarrubia>();


        [HttpGet]
        public async Task<ActionResult<IEnumerable<SergioVillarrubia>>> GetSergioVillarrubias()
        {
            return Ok(await context.SergioVillarrubias.ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> CreateSergioVillarrubia(SergioVillarrubia sergioVillarrubia)
        {
            var s = await (from sva in context.SergioVillarrubias
                           where sva.Ci == sergioVillarrubia.Ci && sva.Estado != "Borrado"
                           select sva).FirstOrDefaultAsync();
            if (s != null)
            {
                return BadRequest("El Gersio ya existe.");
            }
            await context.SergioVillarrubias.AddAsync(sergioVillarrubia);
            await context.SaveChangesAsync();
            return Ok(sergioVillarrubia);
        }

    }
}