using Microsoft.AspNetCore.Mvc;
using DocumentacionInteligente.BackEnd.Data;
using DocumentacionInteligente.BackEnd.Models;

namespace DocumentacionInteligente.BackEnd.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategoriasController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/categorias
        [HttpGet]
        public IActionResult GetCategorias()
        {
            var categorias = _context.CATEGORIAS.ToList();
            return Ok(categorias);
        }

        // GET: api/categorias/5
        [HttpGet("{id}")]
        public IActionResult GetCategoria(int id)
        {
            var categoria = _context.CATEGORIAS.FirstOrDefault(c => c.ID == id);
            if (categoria == null)
                return NotFound("Categoría no encontrada.");

            return Ok(categoria);
        }

        // POST: api/categorias
        [HttpPost]
        public IActionResult CreateCategoria([FromBody] CategoriaRequest request)
        {
            var nuevaCategoria = new CATEGORIAS
            {
                NOMBRE = request.Nombre,
                DESCRIPCION = request.Descripcion,
                CREATE_DATE = DateTime.UtcNow
            };

            _context.CATEGORIAS.Add(nuevaCategoria);
            _context.SaveChanges();

            return Ok("Categoría creada exitosamente.");
        }

        // PUT: api/categorias/5
        [HttpPut("{id}")]
        public IActionResult UpdateCategoria(int id, [FromBody] CategoriaRequest request)
        {
            var categoria = _context.CATEGORIAS.FirstOrDefault(c => c.ID == id);
            if (categoria == null)
                return NotFound("Categoría no encontrada.");

            categoria.NOMBRE = request.Nombre;
            categoria.DESCRIPCION = request.Descripcion;

            _context.SaveChanges();

            return Ok("Categoría actualizada correctamente.");
        }

        // DELETE: api/categorias/5
        [HttpDelete("{id}")]
        public IActionResult DeleteCategoria(int id)
        {
            var categoria = _context.CATEGORIAS.FirstOrDefault(c => c.ID == id);
            if (categoria == null)
                return NotFound("Categoría no encontrada.");

            _context.CATEGORIAS.Remove(categoria);
            _context.SaveChanges();

            return Ok("Categoría eliminada correctamente.");
        }
    }

    public class CategoriaRequest
    {
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
    }
}
