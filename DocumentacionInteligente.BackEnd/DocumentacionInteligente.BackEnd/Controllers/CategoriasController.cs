using Microsoft.AspNetCore.Mvc;
using DocumentacionInteligente.BackEnd.Data;
using DocumentacionInteligente.BackEnd.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace DocumentacionInteligente.BackEnd.Controllers
{
    [Authorize]
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
                NOMBRE = request.Nombre ?? string.Empty,
                DESCRIPCION = request.Descripcion ?? string.Empty,
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

            categoria.NOMBRE = request.Nombre ?? string.Empty;
            categoria.DESCRIPCION = request.Descripcion ?? string.Empty;

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

        // GET: api/categorias/con-documentos
        [HttpGet("con-documentos")]
        public async Task<ActionResult<IEnumerable<CategoriaDTO>>> GetCategoriasConDocumentos()
        {
            var categorias = await _context.CATEGORIAS
                .Include(c => c.DOCUMENTOS)
                .ToListAsync();

            var dto = categorias.Select(c => new CategoriaDTO
            {
                ID = c.ID,
                NOMBRE = c.NOMBRE,
                DESCRIPCION = c.DESCRIPCION,
                CREATE_DATE = c.CREATE_DATE,
                DOCUMENTOS = c.DOCUMENTOS.Select(d => new DocumentoDto
                {
                    Id = d.ID,
                    Titulo = d.TITULO,
                    Descripcion = d.DESCRIPCION,
                    Categoria = d.CATEGORIA_ID,
                    Estado = d.ESTADO,
                    CreadoIA = d.CREADO_IA ?? false,
                    CreateDate = d.CREATE_DATE,
                    VersionActual = d.VERSION_ACTUAL ?? 0,
                    RutaArchivo = d.RUTA_ARCHIVO
                }).ToList()
            });

            return Ok(dto);
        }
    }

    public class CategoriaRequest
    {
        public string? Nombre { get; set; }
        public string? Descripcion { get; set; }
    }
}
