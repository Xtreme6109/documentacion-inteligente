using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using DocumentacionInteligente.BackEnd.Models;
using System.IO;
using System.Threading.Tasks;
using DocumentacionInteligente.BackEnd.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class DocumentosSubidaController : ControllerBase
{
    private readonly IWebHostEnvironment _env;
    private readonly AppDbContext _context;

    public DocumentosSubidaController(IWebHostEnvironment env, AppDbContext context)
    {
        _env = env;
        _context = context;
    }

    [HttpPost("subir")]
    public async Task<IActionResult> SubirDocumento([FromForm] DocumentoSubidaDto dto)
    {
        if (dto.Archivo == null || dto.Archivo.Length == 0)
            return BadRequest("Archivo no válido.");

        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UsuarioId");
        if (userIdClaim == null)
            return Unauthorized("No se pudo determinar el usuario desde el token.");

        if (!int.TryParse(userIdClaim.Value, out int usuarioId))
            return BadRequest("ID de usuario no válido en el token.");

        var basePath = Directory.GetCurrentDirectory();
        var carpeta = Path.Combine(basePath, "Documentos");

        if (!Directory.Exists(carpeta))
        {
            Directory.CreateDirectory(carpeta);
        }

        var nombreArchivo = $"{Guid.NewGuid()}_{Path.GetFileName(dto.Archivo.FileName)}";
        var rutaArchivoCompleta = Path.Combine(carpeta, nombreArchivo);

        using (var stream = new FileStream(rutaArchivoCompleta, FileMode.Create))
        {
            await dto.Archivo.CopyToAsync(stream);
        }

        var rutaRelativa = Path.Combine("Documentos", nombreArchivo).Replace("\\", "/");

        int? versionMayor = await _context.DOCUMENTOS
        .Where(d => d.TITULO == dto.Titulo && d.VERSION_ACTUAL != null)
        .MaxAsync(d => (int?)d.VERSION_ACTUAL);

        int nuevaVersion = (versionMayor ?? 0) + 1;


        var documento = new DOCUMENTOS
        {
            TITULO = dto.Titulo,
            DESCRIPCION = dto.Descripcion,
            RUTA_ARCHIVO = rutaRelativa,
            CATEGORIA_ID = dto.CategoriaId,
            ESTADO = dto.Estado,
            CREADO_IA = dto.CreadoIA,
            CREATE_DATE = DateTime.UtcNow,
            USUARIO_ID = usuarioId,
            VERSION_ACTUAL = nuevaVersion
        };

        try
        {
            _context.DOCUMENTOS.Add(documento);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error al guardar en la base de datos: {ex.Message}");
        }

        return Ok(new { mensaje = "Documento subido exitosamente", documentoId = documento.ID });
    }

}
