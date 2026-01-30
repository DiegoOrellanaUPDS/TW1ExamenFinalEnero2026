using Data;
using Entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Controllers
{
    [ApiController]
[Route("api/[controller]")]
public class FabricioController : ControllerBase
{
    private readonly AppDbContext _context;

    public FabricioController(AppDbContext context)
    {
        _context = context;
    }

    // POST: api/Fabricio
    [HttpPost]
    public async Task<IActionResult> Post(Fabricio fabricio)
    {
        _context.Fabricios.Add(fabricio);
        await _context.SaveChangesAsync();
        return Ok(fabricio);
    }

    // GET: api/Fabricio
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Fabricio>>> Get()
    {
        var fabricios = await _context.Fabricios.ToListAsync();
        return Ok(fabricios);
    }

    // GET: api/Fabricio/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Fabricio>> GetById(int id)
    {
        var fabricio = await _context.Fabricios.FindAsync(id);
        if (fabricio == null)
        {
            return NotFound();
        }
        return Ok(fabricio);
    }
}


}