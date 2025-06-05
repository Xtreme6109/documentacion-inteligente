using DocumentacionInteligente.BackEnd.Models;
using DocumentacionInteligente.BackEnd.Services;
using Microsoft.AspNetCore.Mvc;

namespace DocumentacionInteligente.BackEnd.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReporteController : ControllerBase
    {

        private readonly ReporteServices _reportService;

        public ReporteController()
        {
            _reportService = new ReporteServices();
        }

        //Reporte controller documento 1
        [HttpPost("reporte-documento")]
        public IActionResult DescargarReporteDocumento([FromBody] DocumentoDTO documento)
        {
            var reportService = new ReporteServices();
            var pdfBytes = reportService.GenerarReporteDocumento(documento);

            return File(pdfBytes, "application/pdf", "Documento.pdf");
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

    }
}
