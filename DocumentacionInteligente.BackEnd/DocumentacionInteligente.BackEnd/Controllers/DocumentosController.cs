using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DocumentacionInteligente.BackEnd.Data;
using System.IO;
using System.Text;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;


[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DocumentosController : ControllerBase
{
    private readonly IWebHostEnvironment _env;
    private readonly AppDbContext _context;

    public DocumentosController(AppDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
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
                RutaArchivo = d.RUTA_ARCHIVO,
                UsuarioCreadorId = d.UsuarioCreadorId,
                NombreCategoria = d.CATEGORIA != null ? d.CATEGORIA.NOMBRE : "Desconocido"
            })

            .ToListAsync();


        Console.WriteLine($"Cantidad documentos: {documentos.Count}");
        foreach (var doc in documentos)
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

    [HttpGet("TextoDocumento/{id}")]
    public IActionResult ObtenerTextoDocumento(int id)
    {
        var documento = _context.DOCUMENTOS.Find(id);
        if (documento == null)
            return NotFound("Documento no encontrado");

        string carpetaBase = Path.Combine(Directory.GetCurrentDirectory());
        Console.WriteLine($"Carpeta base para archivos: {carpetaBase}");

        string rutaRelativaCorregida = documento.RUTA_ARCHIVO.Replace('/', Path.DirectorySeparatorChar);
        Console.WriteLine($"Ruta corregida: {rutaRelativaCorregida}");

        string rutaCompleta = Path.Combine(carpetaBase, rutaRelativaCorregida);
        Console.WriteLine($"Ruta completa corregida: {rutaCompleta}");

        if (!System.IO.File.Exists(rutaCompleta))
        {
            Console.WriteLine("Archivo no encontrado en el sistema de archivos");
            return NotFound("Archivo no encontrado");
        }

        var extension = Path.GetExtension(rutaCompleta).ToLowerInvariant();

        if (extension == ".pdf")
        {
            try
            {
                var textoPdf = ExtraerTextoDePdf(rutaCompleta);
                return Content(textoPdf, "text/plain");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error leyendo PDF: {ex.Message}");
                return StatusCode(500, "Error al procesar el archivo PDF.");
            }
        }
        else
        {
            return BadRequest("Solo se soportan archivos PDF para extracción de texto.");
        }
    }

    private string ExtraerTextoDePdf(string ruta)
    {
        var sb = new StringBuilder();

        using var pdfReader = new PdfReader(ruta);
        using var pdfDoc = new PdfDocument(pdfReader);

        for (int i = 1; i <= pdfDoc.GetNumberOfPages(); i++)
        {
            var page = pdfDoc.GetPage(i);
            var texto = PdfTextExtractor.GetTextFromPage(page);
            sb.AppendLine(texto);
        }

        return sb.ToString();
    }



}
