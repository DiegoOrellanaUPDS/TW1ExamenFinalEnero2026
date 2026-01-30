using Microsoft.AspNetCore.Mvc;
using Universidad.Entidades;

namespace Universidad.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdrianController : ControllerBase
    {
        // Simulación de base de datos en memoria
        private static List<Adrian> lista = new();

        // POST: api/Adrian
        [HttpPost]
        public IActionResult Crear([FromBody] Adrian adrian)
        {
            adrian.Id = lista.Count + 1;
            lista.Add(adrian);

            return Ok(adrian);
        }

        // GET: api/Adrian
        [HttpGet]
        public IActionResult Listar()
        {
            return Ok(lista);
        }
    }
}
