using DocumentacionInteligente.BackEnd.Models;
using DocumentacionInteligente.BackEnd.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Dapper;
using DocumentFormat.OpenXml.InkML;

namespace DocumentacionInteligente.BackEnd.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReporteController : ControllerBase
    {

        private readonly ReporteServices _reportService;
       // private readonly AppDbContext _context;

        public ReporteController()
        {
            // _context = context;
            _reportService = new ReporteServices();

        }


        [HttpPost("reporte-documento2")]
        public IActionResult DescargarReporteDocumento2([FromBody] DocumentoDTO2 documento2)
        {
            var reportService = new ReporteServices();
            var pdfBytes = reportService.GenerarReporteDocumento2(documento2);

            return File(pdfBytes, "application/pdf", "Documento.pdf");
        }

        [HttpPost("reporte-documento-word")]
          public IActionResult DescargarDocumentoWord([FromBody] DocumentoDTO documento)
          {
              var reportService = new ReporteServiceWord();
              var wordBytes = reportService.GenerarDocumentoWord(documento);
              return File(wordBytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "Documento.docx");
          }

        [HttpPost("reporteconsumotokens")]
        public IActionResult Tokens([FromBody] List<HISTORIALDOCUMENTOSIA> historialIA)
        {
            var reportService = new ReporteServices();
            var pdfBytes = reportService.GenerarReporteConsumoTokens(historialIA);

            return File(pdfBytes, "application/pdf", "TokensGenerado.pdf");
        }

        [HttpPost("reportexcategoria")]
        public IActionResult ReportePorCategoria([FromBody] CATEGORIAS dto)
        {
            var documentos = _context.DOCUMENTOS
                .Where(d => d.CATEGORIA_ID == dto.ID)
                .ToList();

            var pdf = _reportService.GenerarReportexCategoria(documentos, dto.NOMBRE);

            return File(pdf, "application/pdf", $"Reporte_Categoria_{dto.NOMBRE}.pdf");
        }

        [HttpPost("reportexusuario")]
        public IActionResult ReportePorUsuario([FromBody] int USUARIO_ID)
        {
            var documentos = _reportService.GenerarReportexUsuario(USUARIO_ID);
            /*    .Where(d => d.USUARIO_ID == USUARIO_ID)
                .ToList();*/

            var pdf = _reportService.GenerarReportexUsuario(documentos, USUARIO_ID);

            return File(pdf, "application/pdf", $"Reporte_Usuario_{USUARIO_ID}.pdf");
        }




    }
}
