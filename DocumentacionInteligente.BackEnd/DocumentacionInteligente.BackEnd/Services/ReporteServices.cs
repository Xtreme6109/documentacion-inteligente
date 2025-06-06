namespace DocumentacionInteligente.BackEnd.Services { 
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

    }
}