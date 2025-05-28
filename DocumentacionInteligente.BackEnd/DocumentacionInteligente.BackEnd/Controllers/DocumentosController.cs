using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DocumentacionInteligente.BackEnd.Data;

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
    [HttpGet]
    public async Task<IActionResult> GetDocumentos()
    {
        var documentos = await _context.DOCUMENTOS
            .Include(d => d.CATEGORIA)
            .Include(d => d.VERSION)
            .ToListAsync();

        return Ok(documentos);
    }

    // GET: api/Documentos/5
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

    private bool DocumentoExiste(int id)
    {
        return _context.DOCUMENTOS.Any(e => e.ID == id);
    }
}
