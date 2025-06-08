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
        public byte[] GenerarReportexCategoria(List<DOCUMENTOS> documentos, string nombreCategoria)
        {
            using var ms = new MemoryStream();
            var writer = new PdfWriter(ms);
            var pdf = new PdfDocument(writer);
            var document = new Document(pdf);
            document.SetMargins(40, 40, 40, 40);

            var boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
            var regularFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);

            // Título general
            document.Add(new Paragraph($"Reporte de Documentos - Categoría: {nombreCategoria}")
                .SetFont(boldFont)
                .SetFontSize(14)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetMarginBottom(20));

            foreach (var doc in documentos)
            {
                // Sección por documento
                var docTable = new Table(UnitValue.CreatePercentArray(new float[] { 1, 2 }))
                    .UseAllAvailableWidth()
                    .SetMarginBottom(10);

                docTable.AddCell(new Cell().Add(new Paragraph("Título").SetFont(boldFont)));
                docTable.AddCell(new Cell().Add(new Paragraph(doc.TITULO).SetFont(regularFont)));

                docTable.AddCell(new Cell().Add(new Paragraph("Descripción").SetFont(boldFont)));
                docTable.AddCell(new Cell().Add(new Paragraph(doc.DESCRIPCION).SetFont(regularFont)));

                docTable.AddCell(new Cell().Add(new Paragraph("Usuario").SetFont(boldFont)));
                docTable.AddCell(new Cell().Add(new Paragraph(doc.USUARIO_ID.ToString()).SetFont(regularFont))); // Reemplaza por nombre real si lo incluyes con Include

                docTable.AddCell(new Cell().Add(new Paragraph("Categoría").SetFont(boldFont)));
                docTable.AddCell(new Cell().Add(new Paragraph(doc.CATEGORIA?.NOMBRE ?? "N/A").SetFont(regularFont)));

                docTable.AddCell(new Cell().Add(new Paragraph("Fecha creación").SetFont(boldFont)));
                docTable.AddCell(new Cell().Add(new Paragraph(doc.CREATE_DATE.ToString("yyyy-MM-dd")).SetFont(regularFont)));

                docTable.AddCell(new Cell().Add(new Paragraph("Estado").SetFont(boldFont)));
                docTable.AddCell(new Cell().Add(new Paragraph(doc.ESTADO).SetFont(regularFont)));

                docTable.AddCell(new Cell().Add(new Paragraph("Ruta archivo").SetFont(boldFont)));
                docTable.AddCell(new Cell().Add(new Paragraph(doc.RUTA_ARCHIVO).SetFont(regularFont)));

                document.Add(docTable);
                document.Add(new Paragraph("\n").SetFontSize(6)); // Espaciado
            }

            int pageCount = pdf.GetNumberOfPages();
            for (int i = 1; i <= pageCount; i++)
            {
                var page = pdf.GetPage(i);
                var canvas = new PdfCanvas(page.NewContentStreamAfter(), page.GetResources(), pdf);
                var footer = $"Página {i} de {pageCount}";
                new Canvas(canvas, page.GetPageSize())
                    .ShowTextAligned(new Paragraph(footer).SetFont(regularFont).SetFontSize(8),
                        520, 20, TextAlignment.RIGHT);
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