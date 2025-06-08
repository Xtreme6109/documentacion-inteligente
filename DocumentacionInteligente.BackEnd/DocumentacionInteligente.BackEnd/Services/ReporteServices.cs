namespace DocumentacionInteligente.BackEnd.Services
{
    using System;
    using iText.IO.Font.Constants;
    using iText.Kernel.Colors;
    using iText.Kernel.Font;
    using iText.Kernel.Pdf;
    using iText.Kernel.Pdf.Canvas;
    using iText.Layout;
    using iText.Layout.Element;
    using iText.Layout.Properties;
    using iText.Layout.Borders;
    using DocumentacionInteligente.BackEnd.Models;
    using System.Text.Json;
    using Microsoft.Data.SqlClient;
    using Dapper;

    public class ReporteServices
    {
        public byte[] GenerarReporteDocumento2(DocumentoDTO2 doc)
        {
            using var ms = new MemoryStream();
            var writer = new PdfWriter(ms);
            var pdf = new PdfDocument(writer);

            var document = new Document(pdf);
            document.SetMargins(40, 40, 40, 40);

            var boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
            var regularFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);

            var headerTable = new Table(UnitValue.CreatePercentArray(new float[] { 1, 1 }))
                .UseAllAvailableWidth()
                .SetMarginBottom(10);

            headerTable.AddCell(new Cell(1, 2)
                .Add(new Paragraph("Pasos para Enamorar el Corazón de una IA").SetFont(boldFont))
                .SetTextAlignment(TextAlignment.CENTER)
                .SetBorder(Border.NO_BORDER));

            headerTable.AddCell(new Cell().Add(new Paragraph("Fecha Edición:").SetFont(boldFont)).SetBorder(new SolidBorder(1)));
            headerTable.AddCell(new Cell().Add(new Paragraph($"{doc.FechaDeEdición?.ToString("yyyy-MM-dd") ?? "N/A"}")).SetBorder(new SolidBorder(1)));

            headerTable.AddCell(new Cell().Add(new Paragraph("Versión:").SetFont(boldFont)).SetBorder(new SolidBorder(1)));
            headerTable.AddCell(new Cell().Add(new Paragraph($"{doc.Version ?? "N/A"}")).SetBorder(new SolidBorder(1)));

            headerTable.AddCell(new Cell().Add(new Paragraph("Código:").SetFont(boldFont)).SetBorder(new SolidBorder(1)));
            headerTable.AddCell(new Cell().Add(new Paragraph($"{doc.CodigoDelDocumento ?? "N/A"}")).SetBorder(new SolidBorder(1)));

            headerTable.AddCell(new Cell().Add(new Paragraph("Elaboró:").SetFont(boldFont)).SetBorder(new SolidBorder(1)));
            headerTable.AddCell(new Cell().Add(new Paragraph($"{doc.ElaboradoPor ?? "N/A"}")).SetBorder(new SolidBorder(1)));

            headerTable.AddCell(new Cell().Add(new Paragraph("Revisó:").SetFont(boldFont)).SetBorder(new SolidBorder(1)));
            headerTable.AddCell(new Cell().Add(new Paragraph($"{doc.RevisadoPor ?? "N/A"}")).SetBorder(new SolidBorder(1)));

            document.Add(headerTable);

            document.Add(new Paragraph("\nI. Objetivo").SetFont(boldFont));
            document.Add(new Paragraph(doc.IObjetivo ?? "N/A").SetFont(regularFont));

            document.Add(new Paragraph("\nII. Alcance").SetFont(boldFont));
            document.Add(new Paragraph(doc.IIAlcance ?? "N/A").SetFont(regularFont));

            document.Add(new Paragraph("\nIII. Responsabilidades").SetFont(boldFont));
            if (doc.IIIResponsabilidades is JsonElement element && element.ValueKind == JsonValueKind.Object)
            {
                foreach (var propiedad in element.EnumerateObject())
                {
                    document.Add(new Paragraph($"{propiedad.Name}: {propiedad.Value.ToString()}").SetFont(regularFont));
                }
            }
            else
            {
                document.Add(new Paragraph(doc.IIIResponsabilidades?.ToString() ?? "N/A").SetFont(regularFont));
            }

            document.Add(new Paragraph("\nIV. Desarrollo").SetFont(boldFont));
            if (doc.IVDesarrollo != null && doc.IVDesarrollo.Any())
            {
                foreach (var subtema in doc.IVDesarrollo)
                {
                    document.Add(new Paragraph($"- {subtema.Key}").SetFont(boldFont));

                    foreach (var paso in subtema.Value)
                    {
                        document.Add(new Paragraph($"    {paso.Key}. {paso.Value}").SetFont(regularFont));
                    }
                }
            }
            else
            {
                document.Add(new Paragraph("N/A").SetFont(regularFont));
            }

            document.Add(new Paragraph("\nV. Vigencia").SetFont(boldFont));
            document.Add(new Paragraph(doc.VVigencia ?? "N/A").SetFont(regularFont));

            document.Add(new Paragraph("\nVI. Referencias Bibliográficas").SetFont(boldFont));
            document.Add(new Paragraph(doc.VIReferenciasBibliográficas ?? "N/A").SetFont(regularFont));

            document.Add(new Paragraph("\nVII. Historial de Cambios").SetFont(boldFont));
            if (doc.VIIHistorialDeCambioDeDocumentos != null && doc.VIIHistorialDeCambioDeDocumentos.Any())
            {
                var table = new Table(3).UseAllAvailableWidth();
                table.AddHeaderCell("N°").AddHeaderCell("Fecha").AddHeaderCell("Descripción");

                foreach (var cambio in doc.VIIHistorialDeCambioDeDocumentos)
                {
                    table.AddCell(cambio.Number?.ToString() ?? "N/A");
                    table.AddCell(cambio.Date?.ToString("dd/MM/yyyy") ?? "N/A");
                    table.AddCell(cambio.Description ?? "N/A");
                }
                document.Add(table);
            }
            else
            {
                document.Add(new Paragraph("Sin historial.").SetFont(regularFont));
            }

            document.Add(new Paragraph("\nVIII. Firmas").SetFont(boldFont));
            document.Add(new Paragraph(doc.VIIIFirmas ?? "N/A").SetFont(regularFont));

            int pageCount = pdf.GetNumberOfPages();
            for (int i = 1; i <= pageCount; i++)
            {
                var page = pdf.GetPage(i);
                var canvas = new PdfCanvas(page.NewContentStreamAfter(), page.GetResources(), pdf);
                var pageText = $"Página {i} de {pageCount}";
                new Canvas(canvas, page.GetPageSize())
                .ShowTextAligned(new Paragraph(pageText), 520, 20, TextAlignment.RIGHT);
            }

            document.Close();
            return ms.ToArray();
        }

        public byte[] GenerarReporteConsumoTokens(List<HISTORIALDOCUMENTOSIA> historialIA)
        {
            using var ms = new MemoryStream();
            var writer = new PdfWriter(ms);
            var pdf = new PdfDocument(writer);
            var document = new Document(pdf);
            document.SetMargins(40, 40, 40, 40);

            var boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
            var regularFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);

            // Título
            document.Add(new Paragraph("Informe de Consumo de Tokens").SetFont(boldFont).SetFontSize(16).SetTextAlignment(TextAlignment.CENTER));
            document.Add(new Paragraph($"Fecha de generación: {DateTime.Now:dd/MM/yyyy HH:mm:ss}").SetFont(regularFont).SetTextAlignment(TextAlignment.RIGHT));
            document.Add(new Paragraph("\n"));

            // Tabla de historial
            var table = new Table(UnitValue.CreatePercentArray(new float[] { 1, 2, 1, 1, 1, 1 }))
                .UseAllAvailableWidth();

            table.AddHeaderCell("ID");
            table.AddHeaderCell("Prompt");
            table.AddHeaderCell("Palabras");
            table.AddHeaderCell("Tokens Entrada");
            table.AddHeaderCell("Tokens Salida");
            table.AddHeaderCell("Fecha");

            foreach (var item in historialIA)
            {
                table.AddCell(item.ID.ToString());
                table.AddCell(new Paragraph(item.PROMPT ?? "N/A").SetFontSize(8));
                table.AddCell(item.CANTIDAD_PALABRAS_PROMPT.ToString());
                table.AddCell(item.TOKENS_ENTRADA.ToString());
                table.AddCell(item.TOKENS_SALIDA.ToString());
                table.AddCell(item.FECHA_GENERACION.ToString("dd/MM/yyyy HH:mm"));
            }

            document.Add(table);

            // Totales
            int totalEntrada = historialIA.Sum(h => h.TOKENS_ENTRADA);
            int totalSalida = historialIA.Sum(h => h.TOKENS_SALIDA);
            int totalPalabras = historialIA.Sum(h => h.CANTIDAD_PALABRAS_PROMPT);

            document.Add(new Paragraph($"\nTotal Palabras: {totalPalabras}").SetFont(boldFont));
            document.Add(new Paragraph($"Total Tokens Entrada: {totalEntrada}").SetFont(boldFont));
            document.Add(new Paragraph($"Total Tokens Salida: {totalSalida}").SetFont(boldFont));

            // Pie de página con número de página
            int pageCount = pdf.GetNumberOfPages();
            for (int i = 1; i <= pageCount; i++)
            {
                var page = pdf.GetPage(i);
                var canvas = new PdfCanvas(page.NewContentStreamAfter(), page.GetResources(), pdf);
                var pageText = $"Página {i} de {pageCount}";
                new Canvas(canvas, page.GetPageSize())
                    .ShowTextAligned(new Paragraph(pageText).SetFontSize(8), 520, 20, TextAlignment.RIGHT);
            }

            document.Close();
            return ms.ToArray();
        }

        public byte[] GenerarReportexCategoria(List<DOCUMENTOS> documentos, string nombreCategoria)
        {
            using var ms = new MemoryStream();
            var writer = new PdfWriter(ms);
            var pdf = new PdfDocument(writer);
            var document = new Document(pdf);
            document.SetMargins(40, 40, 40, 40);

            var boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
            var regularFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);

            document.Add(new Paragraph($"REPORTE DE DOCUMENTOS POR CATEGORÍA: {nombreCategoria}")
                .SetFont(boldFont)
                .SetFontSize(16)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetMarginBottom(20));

            var table = new Table(UnitValue.CreatePercentArray(new float[] { 1, 3, 3, 3, 2 }))
                .UseAllAvailableWidth();

            table.AddHeaderCell("ID");
            table.AddHeaderCell("Título");
            table.AddHeaderCell("Descripción");
            table.AddHeaderCell("Ruta");
            table.AddHeaderCell("Fecha");

            foreach (var doc in documentos)
            {
                table.AddCell(doc.ID.ToString());
                table.AddCell(doc.TITULO);
                table.AddCell(doc.DESCRIPCION);
                table.AddCell(doc.RUTA_ARCHIVO);
                table.AddCell(doc.CREATE_DATE.ToString("yyyy-MM-dd"));
            }

            document.Add(table);

            document.Close();
            return ms.ToArray();
        }

        public byte[] GenerarReportexUsuario(List<DOCUMENTOS> documentos, int usuarioId)
        {
            using var ms = new MemoryStream();
            var writer = new PdfWriter(ms);
            var pdf = new PdfDocument(writer);
            var document = new Document(pdf);
            document.SetMargins(40, 40, 40, 40);

            var boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
            var regularFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);

            document.Add(new Paragraph($"REPORTE DE DOCUMENTOS DEL USUARIO ID: {usuarioId}")
                .SetFont(boldFont)
                .SetFontSize(16)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetMarginBottom(20));

            var table = new Table(UnitValue.CreatePercentArray(new float[] { 1, 3, 3, 3, 2 }))
                .UseAllAvailableWidth();

            table.AddHeaderCell("ID");
            table.AddHeaderCell("Título");
            table.AddHeaderCell("Descripción");
            table.AddHeaderCell("Ruta");
            table.AddHeaderCell("Fecha");

            foreach (var doc in documentos)
            {
                table.AddCell(doc.ID.ToString());
                table.AddCell(doc.TITULO);
                table.AddCell(doc.DESCRIPCION);
                table.AddCell(doc.RUTA_ARCHIVO);
                table.AddCell(doc.CREATE_DATE.ToString("yyyy-MM-dd"));
            }

            document.Add(table);

            document.Close();
            return ms.ToArray();
        }

    }
}