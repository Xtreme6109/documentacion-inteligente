using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DocumentacionInteligente.BackEnd.Data;
using System.IO;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DocumentosController : ControllerBase
{
    private readonly AppDbContext _context;

    public DocumentosController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/Documentos
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetDocumentos()
    {
        try
        {
            var documentos = await _context.DOCUMENTOS
                .Include(d => d.CATEGORIA)
                .Include(d => d.VERSION)
                .ToListAsync();

            return Ok(documentos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno: {ex.Message}");
        }
    }

    [HttpGet("AllDocuments")]
    public async Task<ActionResult<List<DocumentoDto>>> GetAllDocuments()
    {
        var documentos = await _context.DOCUMENTOS
            .Select(d => new DocumentoDto
            {
                Id = d.ID,
                Titulo = d.TITULO,
                Descripcion = d.DESCRIPCION,
                Categoria = d.CATEGORIA_ID,
                Estado = d.ESTADO,
                CreadoIA = d.CREADO_IA ?? false,
                CreateDate = d.CREATE_DATE,
                VersionActual = d.VERSION_ACTUAL ?? 1,
                RutaArchivo = d.RUTA_ARCHIVO
            })

            .ToListAsync();

            
Console.WriteLine($"Cantidad documentos: {documentos.Count}");
foreach(var doc in documentos)
    Console.WriteLine($"ID: {doc.Id}, Titulo: {doc.Titulo}");

        return Ok(documentos);
    }

    [HttpGet("titulos")]
    public async Task<IActionResult> GetTitulosUnicos()
    {
        var titulos = await _context.DOCUMENTOS
            .Select(d => d.TITULO)
            .Distinct()
            .ToListAsync();

        return Ok(titulos);
    }

    // GET: api/Documentos/5
    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetDocumento(int id)
    {
        var documento = await _context.DOCUMENTOS
            .Include(d => d.CATEGORIA)
            .Include(d => d.VERSION)
            .FirstOrDefaultAsync(d => d.ID == id);

        if (documento == null)
        {
            return NotFound();
        }

        return Ok(documento);
    }



    // GET: api/Documentos/version-mayor?titulo=xxx
    [HttpGet("version-mayor")]
    public async Task<IActionResult> GetDocumentoVersionMayor([FromQuery] string titulo)
    {
        if (string.IsNullOrEmpty(titulo))
            return BadRequest("El título es requerido.");

        var documento = await _context.DOCUMENTOS
            .Where(d => d.TITULO == titulo && d.VERSION_ACTUAL != null)
            .OrderByDescending(d => d.VERSION_ACTUAL)
            .FirstOrDefaultAsync();

        if (documento == null)
            return NotFound();

        return Ok(new { VERSION = documento.VERSION_ACTUAL });
    }


    // POST: api/Documentos
    [HttpPost]
    [Authorize(Policy = "Admin")]
    public async Task<IActionResult> CrearDocumento([FromBody] DOCUMENTOS documento)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _context.DOCUMENTOS.Add(documento);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetDocumento), new { id = documento.ID }, documento);
    }

    // PUT: api/Documentos/5
    [HttpPut("{id}")]
    [Authorize(Policy = "Admin")]
    public async Task<IActionResult> EditarDocumento(int id, [FromBody] DOCUMENTOS documento)
    {
        if (id != documento.ID)
        {
            return BadRequest("El ID no coincide.");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _context.Entry(documento).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!DocumentoExiste(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }



    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> EliminarDocumento(int id)
    {
        var documento = await _context.DOCUMENTOS.FindAsync(id);
        if (documento == null)
        {
            return NotFound();
        }

        // Ruta física completa del archivo
        var rutaArchivo = Path.Combine(Directory.GetCurrentDirectory(), documento.RUTA_ARCHIVO);

        if (System.IO.File.Exists(rutaArchivo))
        {
            try
            {
                System.IO.File.Delete(rutaArchivo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al eliminar el archivo físico: {ex.Message}");
            }
        }

        _context.DOCUMENTOS.Remove(documento);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error al eliminar el documento: {ex.Message}");
        }

        return NoContent();
    }


    private bool DocumentoExiste(int id)
    {
        return _context.DOCUMENTOS.Any(e => e.ID == id);
    }




}
