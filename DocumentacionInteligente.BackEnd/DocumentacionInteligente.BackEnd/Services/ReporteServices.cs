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

    public class ReporteServices
    {
        public byte[] GenerarReporteDocumento(DocumentoDTO doc)
        {
            using var ms = new MemoryStream();


            var writer = new PdfWriter(ms);
            var pdf = new PdfDocument(writer);
            var document = new Document(pdf);

            // Fuente para negrita
            PdfFont boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
            PdfFont normalFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);

            // Tabla de encabezado con 6 columnas (como en OpenXML: columnas combinadas)
            var table = new Table(new float[] { 4, 6, 4, 4 });
            table.SetWidth(UnitValue.CreatePercentValue(100));
            table.SetBorder(Border.NO_BORDER);

            // Fila 1
            var cellImg = new Cell(3, 1)
                .Add(new Paragraph("[Logo Aquí]").SetFont(boldFont).SetTextAlignment(TextAlignment.CENTER))
                .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                .SetBorder(new SolidBorder(1));
            table.AddCell(cellImg);

            var cellTitle = new Cell(2, 2)
                .Add(new Paragraph(doc.Titulo ?? "Título").SetFont(boldFont).SetFontSize(14))
                .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetBorder(new SolidBorder(1));
            table.AddCell(cellTitle);

            var cellPage = new Cell(1, 1)
                .Add(new Paragraph($"Página {doc.Hoja} de {doc.TotalHojas}").SetFont(normalFont))
                .SetTextAlignment(TextAlignment.CENTER)
                .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                .SetBorder(new SolidBorder(1));
            table.AddCell(cellPage);

            // Fila 2
            var cellCode = new Cell(1, 1)
                .Add(new Paragraph($"Código: {doc.CódigoDelDocumento ?? "N/A"}").SetFont(normalFont))
                .SetTextAlignment(TextAlignment.CENTER)
                .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                .SetBorder(new SolidBorder(1));
            table.AddCell(cellCode);

            // Fila 3
            var cellFecha = new Cell()
                .Add(new Paragraph($"Fecha Edición: {doc.FechaDeEdición?.ToString("dd/MM/yyyy") ?? "N/A"}").SetFont(normalFont))
                .SetTextAlignment(TextAlignment.CENTER)
                .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                .SetBorder(new SolidBorder(1));
            table.AddCell(cellFecha);

            var cellVersion = new Cell()
                .Add(new Paragraph($"Versión: {doc.Version ?? "N/A"}").SetFont(normalFont))
                .SetTextAlignment(TextAlignment.CENTER)
                .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                .SetBorder(new SolidBorder(1));
            table.AddCell(cellVersion);

            var cellDivulgacion = new Cell()
                .Add(new Paragraph("Fecha de Divulgación:").SetFont(normalFont))
                .SetTextAlignment(TextAlignment.CENTER)
                .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                .SetBorder(new SolidBorder(1));
            table.AddCell(cellDivulgacion);

            // Fila 4
            var cellElaboro = new Cell()
                .Add(new Paragraph($"Elaboró: {doc.ElaboradoPor ?? "N/A"}").SetFont(normalFont))
                .SetBorder(new SolidBorder(1));
            table.AddCell(cellElaboro);

            var cellReviso = new Cell(1, 2)
                .Add(new Paragraph($"Revisó: {doc.RevisadoPor ?? "N/A"}").SetFont(normalFont))
                .SetBorder(new SolidBorder(1));
            table.AddCell(cellReviso);

            var cellAutorizo = new Cell()
                .Add(new Paragraph($"Autorizó: {doc.AutorizadoPor ?? "N/A"}").SetFont(normalFont))
                .SetBorder(new SolidBorder(1));
            table.AddCell(cellAutorizo);

            document.Add(table);

            // Salto de línea
            document.Add(new Paragraph(" "));

            // Agregar secciones con encabezado y texto
            void AddSection(string title, string content)
            {
                document.Add(new Paragraph(title).SetFont(boldFont).SetFontSize(12).SetMarginTop(10));
                document.Add(new Paragraph(content ?? "N/A").SetFont(normalFont));
            }

            AddSection("I. Objetivo", doc.IObjetivo);
            AddSection("II. Alcance", doc.IIAlcance);

            // III. Responsabilidades
            document.Add(new Paragraph("III. Responsabilidades")
                .SetFont(boldFont)
                .SetFontSize(12)
                .SetMarginTop(10));

            if (doc.IIIResponsabilidades != null)
            {
                var list = new iText.Layout.Element.List()
                    .SetSymbolIndent(15)
                    .SetListSymbol("\u2022")
                    .SetMarginLeft(20);

                foreach (var item in doc.IIIResponsabilidades)
                {
                    var content = new Paragraph()
                        .Add(new Text($"{item.Key}: ").SetFont(boldFont))
                        .Add(new Text(item.Value).SetFont(normalFont));

                    var listItem = new ListItem();
                    listItem.Add(content);
                    list.Add(listItem);
                }

                document.Add(list);
            }
            else
            {
                document.Add(new Paragraph("N/A").SetMarginLeft(20));
            }

            // IV. Desarrollo
            document.Add(new Paragraph("IV. Desarrollo")
                .SetFont(boldFont)
                .SetFontSize(12)
                .SetMarginTop(10));

            if (doc.IVDesarrollo != null)
            {
                var list = new iText.Layout.Element.List()
                    .SetSymbolIndent(15)
                    .SetListSymbol("\u2022")
                    .SetMarginLeft(20);

                foreach (var item in doc.IVDesarrollo)
                {
                    var content = new Paragraph()
                        .Add(new Text($"{item.Key}: ").SetFont(boldFont))
                        .Add(new Text(item.Value).SetFont(normalFont));

                    var listItem = new ListItem();
                    listItem.Add(content);
                    list.Add(listItem);
                }

                document.Add(list);
            }
            else
            {
                document.Add(new Paragraph("N/A").SetMarginLeft(20));
            }



            AddSection("V. Vigencia", doc.VVigencia);
            AddSection("VI. Referencias Bibliográficas", doc.VIReferenciasBibliográficas);

            document.Add(new Paragraph("VII. Historial de cambio de Documentos").SetFont(boldFont).SetFontSize(12).SetMarginTop(10));

            if (doc.VIIHistorialDeCambioDeDocumentos != null && doc.VIIHistorialDeCambioDeDocumentos.Count > 0)
            {
                var histTable = new Table(new float[] { 3, 3, 6 }).SetWidth(UnitValue.CreatePercentValue(100));
                histTable.AddHeaderCell(new Cell().Add(new Paragraph("NÚMERO DE REVISIÓN").SetFont(boldFont)));
                histTable.AddHeaderCell(new Cell().Add(new Paragraph("FECHA").SetFont(boldFont)));
                histTable.AddHeaderCell(new Cell().Add(new Paragraph("DESCRIPCIÓN DEL CAMBIO").SetFont(boldFont)));

                foreach (var cambio in doc.VIIHistorialDeCambioDeDocumentos)
                {
                    histTable.AddCell(new Cell().Add(new Paragraph(cambio.Number.ToString()).SetFont(normalFont)));
                    histTable.AddCell(new Cell().Add(new Paragraph(cambio.Date?.ToString("yyyy-MM-dd") ?? "N/A").SetFont(normalFont)));
                    histTable.AddCell(new Cell().Add(new Paragraph(cambio.Description ?? "N/A").SetFont(normalFont)));
                }

                document.Add(histTable);
            }
            else
            {
                document.Add(new Paragraph("Sin historial.").SetFont(normalFont));
            }

            document.Add(new Paragraph(" "));

            AddSection("VIII. Firmas", doc.VIIIFirmas);

            // Agregar pie de página con número de página
            pdf.GetPage(1).GetPdfObject().Put(PdfName.Rotate, new PdfNumber(0));
            document.ShowTextAligned(new Paragraph($"Página 1 de 1").SetFont(normalFont),
                                    550, 20, 1,
                                    TextAlignment.RIGHT, VerticalAlignment.BOTTOM, 0);

            document.Close();

            return ms.ToArray();
        }
    }
}
