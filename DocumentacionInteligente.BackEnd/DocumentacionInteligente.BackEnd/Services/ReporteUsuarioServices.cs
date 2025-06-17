using System;
using System.Collections.Generic;
using System.IO;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Layout.Borders;
using iText.IO.Font.Constants;
using iText.Layout.Font;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Layout.Renderer;
using iText.Kernel.Pdf.Tagging;
using iText.Layout.Tagging;

namespace DocumentacionInteligente.BackEnd.Services
{
    using DocumentacionInteligente.BackEnd.Models;

    public class ReporteUsuarioService
    {
        public byte[] GenerarReporteDocumentosPorUsuario(List<DocumentoDto> documentos, string nombreUsuario)
        {
            using var ms = new MemoryStream();
            var writer = new PdfWriter(ms);
            var pdf = new PdfDocument(writer);
            var document = new Document(pdf);

            var boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
            var normalFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);

            // Título principal
            document.Add(new Paragraph("Reporte de Documentos")
                .SetFont(boldFont)
                .SetFontSize(20)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontColor(ColorConstants.DARK_GRAY)
                .SetMarginBottom(10));

            // Subtítulo con nombre del usuario
            document.Add(new Paragraph($"{nombreUsuario}")
                .SetFont(boldFont)
                .SetFontSize(14)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontColor(ColorConstants.BLUE)
                .SetMarginBottom(20));

            // Línea decorativa
            var line = new LineSeparator(new SolidLine(1f));
            line.SetMarginBottom(15);
            document.Add(line);

            // Tabla con columnas ampliadas y mejor proporción
            var table = new Table(new float[] { 1, 4, 5, 2, 2, 2, 3, 2 })
                .UseAllAvailableWidth();

            // Agregar encabezados con fondo gris y texto en blanco
            var headerBackground = ColorConstants.LIGHT_GRAY;
            var headerTextColor = ColorConstants.BLACK;

            string[] headers = new string[]
            {
                "ID", "Título", "Descripción", "Categoría", "Estado", "Creado IA", "Fecha Creación", "Versión"
            };

            foreach (var header in headers)
            {
                table.AddHeaderCell(new Cell()
                    .Add(new Paragraph(header).SetFont(boldFont).SetFontColor(headerTextColor))
                    .SetBackgroundColor(headerBackground)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetPadding(5));
            }

            // Agregar filas con alternancia de color para facilitar lectura
            bool shade = false;
            var rowShadeColor = new DeviceRgb(240, 240, 240);

            foreach (var doc in documentos)
            {
                var bgColor = shade ? rowShadeColor : ColorConstants.WHITE;
                shade = !shade;

                table.AddCell(new Cell().Add(new Paragraph(doc.Id.ToString()).SetFont(normalFont)).SetBackgroundColor(bgColor).SetPadding(4));
                table.AddCell(new Cell().Add(new Paragraph(doc.Titulo).SetFont(normalFont)).SetBackgroundColor(bgColor).SetPadding(4));
                table.AddCell(new Cell().Add(new Paragraph(doc.Descripcion).SetFont(normalFont)).SetBackgroundColor(bgColor).SetPadding(4));
                table.AddCell(new Cell().Add(new Paragraph(doc.NombreCategoria).SetFont(normalFont)).SetBackgroundColor(bgColor).SetPadding(4));
                table.AddCell(new Cell().Add(new Paragraph(doc.Estado).SetFont(normalFont)).SetBackgroundColor(bgColor).SetPadding(4));
                table.AddCell(new Cell().Add(new Paragraph(doc.CreadoIA ? "Sí" : "No").SetFont(normalFont)).SetBackgroundColor(bgColor).SetPadding(4));
                table.AddCell(new Cell().Add(new Paragraph(doc.CreateDate.ToString("dd/MM/yyyy")).SetFont(normalFont)).SetBackgroundColor(bgColor).SetPadding(4));
                table.AddCell(new Cell().Add(new Paragraph(doc.VersionActual.ToString()).SetFont(normalFont)).SetBackgroundColor(bgColor).SetPadding(4));
            }

            document.Add(table);

            // Total documentos al final
            document.Add(new Paragraph($"\nTotal de documentos: {documentos.Count}")
                .SetFont(boldFont)
                .SetFontSize(12)
                .SetTextAlignment(TextAlignment.RIGHT)
                .SetMarginTop(15));

            // Pie de página con fecha generación
            document.Add(new Paragraph($"Reporte generado el {DateTime.Now:dd/MM/yyyy HH:mm}")
                .SetFont(normalFont)
                .SetFontSize(8)
                .SetTextAlignment(TextAlignment.RIGHT)
                .SetMarginTop(5));

            document.Close();
            return ms.ToArray();
        }

    }
}
