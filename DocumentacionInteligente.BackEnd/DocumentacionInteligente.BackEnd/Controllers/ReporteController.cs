using DocumentacionInteligente.BackEnd.Models;
using DocumentacionInteligente.BackEnd.Services;
using Microsoft.AspNetCore.Mvc;
using DocumentacionInteligente.BackEnd.Data;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace DocumentacionInteligente.BackEnd.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReporteController : ControllerBase
    {
        private readonly ReporteServices _reportService;
        private readonly ReporteUsuarioService _reporteUsuarioService;
        private readonly ReporteConsumoTokens _reporteConsumoTokens;

        private readonly AppDbContext _context;

        public ReporteController(AppDbContext context)
        {
            _context = context;
            _reportService = new ReporteServices();
            _reporteUsuarioService = new ReporteUsuarioService();
            _reporteConsumoTokens = new ReporteConsumoTokens();



        }

        [HttpPost("reporte-documento")]
        public IActionResult DescargarReporteDocumento([FromBody] DocumentoDTO documento)
        {
            // Aquí debes convertir documento a DocumentoDto si necesitas
            var documentos = new List<DocumentoDto>(); // ejemplo vacío
            var pdfBytes = _reporteUsuarioService.GenerarReporteDocumentosPorUsuario(documentos, "NombreUsuario");

            return File(pdfBytes, "application/pdf", "Documento.pdf");
        }

        [HttpPost("reporte-documento-word")]
        public IActionResult DescargarDocumentoWord([FromBody] DocumentoDTO documento)
        {
            var reportService = new ReporteServiceWord();
            var wordBytes = reportService.GenerarDocumentoWord(documento);
            return File(wordBytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "Documento.docx");
        }

        [HttpPost("reporte-usuario")]
        public IActionResult GenerarReporteUsuario([FromBody] DocumentoDTO documentoDto)
        {
            int userIdInt = documentoDto.UsuarioId;

            if (userIdInt <= 0)
            {
                return BadRequest("ID de usuario inválido");
            }

            var usuario = _context.USUARIOS.FirstOrDefault(u => u.ID == userIdInt);
            if (usuario == null)
            {
                return NotFound("Usuario no encontrado");
            }

            // Ajustar el filtrado por fechas si vienen valores válidos
            var documentosDelUsuarioQuery = _context.DOCUMENTOS
                .Include(d => d.CATEGORIA)
                .Where(d => d.USUARIO_ID == userIdInt);

            if (documentoDto.FechaInicio.HasValue)
            {
                documentosDelUsuarioQuery = documentosDelUsuarioQuery.Where(d => d.CREATE_DATE >= documentoDto.FechaInicio.Value);
            }
            if (documentoDto.FechaFin.HasValue)
            {
                documentosDelUsuarioQuery = documentosDelUsuarioQuery.Where(d => d.CREATE_DATE <= documentoDto.FechaFin.Value);
            }

            var documentosDelUsuario = documentosDelUsuarioQuery
                .Select(d => new DocumentoDto
                {
                    Id = d.ID,
                    Titulo = d.TITULO,
                    Descripcion = d.DESCRIPCION,
                    Categoria = d.CATEGORIA_ID,
                    NombreCategoria = d.CATEGORIA != null ? d.CATEGORIA.NOMBRE : "Sin categoría",
                    Estado = d.ESTADO,
                    CreadoIA = d.CREADO_IA ?? false,
                    CreateDate = d.CREATE_DATE,
                    VersionActual = d.VERSION_ACTUAL ?? 0,
                    RutaArchivo = d.RUTA_ARCHIVO,
                    UsuarioCreadorId = d.USUARIO_ID
                })
                .ToList();

            var pdf = _reporteUsuarioService.GenerarReporteDocumentosPorUsuario(documentosDelUsuario, $"Usuario: {usuario.NOMBRE}");

            return File(pdf, "application/pdf", "ReporteDocumentosUsuario.pdf");
        }

      [HttpPost("reporte-categoria")]
public IActionResult GenerarReporteCategoria([FromBody] FiltroReporteCategoriaDTO filtro)
{
    // Validar que venga categoría y fechas
    if (filtro.Categoria <= 0 || !filtro.FechaInicio.HasValue || !filtro.FechaFin.HasValue)
    {
        return BadRequest("Debe completar categoría, fecha de inicio y fecha de fin.");
    }

    // Ya no validar rol ni hacer autorizaciones especiales
    //var claims = HttpContext.User.Claims;
    //var rol = claims.FirstOrDefault(c => c.Type == "Rol")?.Value;

    //if (string.IsNullOrEmpty(rol))
    //{
    //    return Unauthorized("No se encontró rol en el token.");
    //}

    //if (rol != "Admin")
    //{
    //    return StatusCode(403, "Solo los administradores pueden generar reportes por categoría.");
    //}

    var categoria = _context.CATEGORIAS.FirstOrDefault(c => c.ID == filtro.Categoria);
    if (categoria == null)
    {
        return NotFound("Categoría no encontrada.");
    }

    var documentos = (from d in _context.DOCUMENTOS
                      join u in _context.USUARIOS on d.USUARIO_ID equals u.ID into joined
                      from u in joined.DefaultIfEmpty()
                      where d.CATEGORIA_ID == filtro.Categoria
                          && d.CREATE_DATE >= filtro.FechaInicio.Value
                          && d.CREATE_DATE <= filtro.FechaFin.Value
                      select new DocumentoDto
                      {
                          Id = d.ID,
                          Titulo = d.TITULO,
                          Descripcion = d.DESCRIPCION,
                          Categoria = d.CATEGORIA_ID,
                          NombreCategoria = d.CATEGORIA != null ? d.CATEGORIA.NOMBRE : "Sin categoría",
                          Estado = d.ESTADO,
                          CreadoIA = d.CREADO_IA ?? false,
                          CreateDate = d.CREATE_DATE,
                          VersionActual = d.VERSION_ACTUAL ?? 0,
                          RutaArchivo = d.RUTA_ARCHIVO,
                          UsuarioCreadorId = d.USUARIO_ID,
                          NombreUsuarioCreador = u != null ? u.NOMBRE : "Desconocido"
                      }).ToList();

    var pdf = _reporteUsuarioService.GenerarReporteDocumentosPorUsuario(documentos, $"Categoría: {categoria.NOMBRE}");

    return File(pdf, "application/pdf", "ReporteDocumentosCategoria.pdf");
}


[HttpPost("reporte-consumo-tokens")]
public IActionResult GenerarReporteConsumoTokens([FromBody] FiltroReporteTokensDTO filtro)
{
    // Validar rol Admin desde las claims del usuario autenticado
    var rol = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "Rol")?.Value;
    if (rol != "Admin")
    {
        return Forbid("Solo los administradores pueden generar este reporte.");
    }

    if (filtro.UsuarioId <= 0 || !filtro.FechaInicio.HasValue || !filtro.FechaFin.HasValue)
    {
        return BadRequest("Debe completar usuario, fecha de inicio y fecha de fin.");
    }

    var usuario = _context.USUARIOS.FirstOrDefault(u => u.ID == filtro.UsuarioId);
    if (usuario == null)
    {
        return NotFound("Usuario no encontrado.");
    }

    var historialTokens = _context.HISTORIALDOCUMENTOSIA
        .Where(h => h.USUARIO_ID == filtro.UsuarioId
                    && h.FECHA_GENERACION >= filtro.FechaInicio.Value
                    && h.FECHA_GENERACION <= filtro.FechaFin.Value)
        .OrderBy(h => h.FECHA_GENERACION)
        .ToList();

    var pdf = _reporteConsumoTokens.GenerarReporteConsumoTokens(historialTokens, usuario.NOMBRE);

    return File(pdf, "application/pdf", $"ReporteTokens_{usuario.NOMBRE}_{DateTime.Now:yyyyMMdd}.pdf");
}

    }

    

}
