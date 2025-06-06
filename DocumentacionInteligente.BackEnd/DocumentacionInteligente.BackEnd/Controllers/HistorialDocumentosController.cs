using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using DocumentacionInteligente.BackEnd.Data;
using DocumentacionInteligente.BackEnd.Models;

[ApiController]
[Route("api/[controller]")]
public class HistorialDocumentosController : ControllerBase
{
    private readonly AppDbContext _context;

    public HistorialDocumentosController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost("registrar")]
    public async Task<IActionResult> RegistrarHistorial([FromBody] HistorialDocumentoCreateDto dto)
    {
        if (dto.UsuarioId <= 0)
            return BadRequest("UsuarioId debe ser mayor que 0.");

        var nuevoRegistro = new HISTORIALDOCUMENTOSIA
        {
            USUARIO_ID = dto.UsuarioId,
            FECHA_GENERACION = DateTime.Now,
            PROMPT = dto.Prompt,
            CANTIDAD_PALABRAS_PROMPT = dto.CantidadPalabrasPrompt,
            TOKENS_ENTRADA = dto.TokensEntrada,
            TOKENS_SALIDA = dto.TokensSalida,
            RESULTADO = dto.Resultado
        };

        try
        {

            Console.WriteLine("DTO recibido:");
            Console.WriteLine($"Prompt: {dto.Prompt}");
            Console.WriteLine($"TokensSalida: {dto.TokensSalida}");
            Console.WriteLine($"Resultado: {dto.Resultado}");

            _context.HISTORIALDOCUMENTOSIA.Add(nuevoRegistro);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                id = nuevoRegistro.ID,
                message = "Historial registrado correctamente."
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, error = ex.Message });
        }
    }
}
