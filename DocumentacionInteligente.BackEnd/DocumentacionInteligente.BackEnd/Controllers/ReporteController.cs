using DocumentacionInteligente.BackEnd.Models;
using DocumentacionInteligente.BackEnd.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Dapper;

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

        [HttpPost("reportenuevo")]
        public async Task<IActionResult> GuardarDocumentoAsync([FromBody] DocumentoDTO2 documento)
        {
            var reportService = new ReporteServices();
            await reportService.GuardarDocumentoAsync(documento);

            return Ok("Documento guardado correctamente");
        }

        [HttpPost("reportenuevoWord")]
        public async Task<IActionResult> GuardarYDescargarDocumentoWord([FromBody] DocumentoDTO2 documento)
        {
            using var memoryStream = new MemoryStream();
            using var doc = DocX.Create(memoryStream);

            doc.InsertParagraph("Documento Generado").FontSize(18).Bold().Alignment = Aligment.center;
            doc.InsertParagraph($"Título: {documento.TítuloDelDocumento}");
            doc.InsertParagraph($"Fecha de Edición: {documento.FechaDeEdición}");
            doc.InsertParagraph($"Versión: {documento.Version}");
            doc.InsertParagraph($"Código: {documento.CodigoDelDocumento}");
            doc.InsertParagraph($"Elaborado por: {documento.ElaboradoPor}");
            doc.InsertParagraph($"Revisado por: {documento.RevisadoPor}");
            doc.InsertParagraph($"I. Objetivo: {documento.IObjetivo}");
            doc.InsertParagraph($"II. Alcance: {documento.IIAlcance}");
            doc.InsertParagraph($"III. Responsabilidades: {JsonSerializer.Serialize(documento.IIIResponsabilidades)}");
            doc.InsertParagraph($"IV. Desarrollo: {JsonSerializer.Serialize(documento.IVDesarrollo)}");
            doc.InsertParagraph($"V. Vigencia: {documento.VVigencia}");
            doc.InsertParagraph($"VI. Referencias: {documento.VIReferenciasBibliográficas}");
            doc.InsertParagraph($"VII. Historial de cambios: {JsonSerializer.Serialize(documento.VIIHistorialDeCambioDeDocumentos)}");
            doc.InsertParagraph($"VIII. Firmas: {documento.VIIIFirmas}");

            doc.Save();

            // Retorna el documento Word como descarga
            return File(memoryStream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                        "DocumentoGenerado.docx");
        }


    }
}
