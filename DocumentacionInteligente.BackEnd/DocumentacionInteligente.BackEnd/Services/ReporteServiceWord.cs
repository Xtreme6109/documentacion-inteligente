namespace DocumentacionInteligente.BackEnd.Services
{
    using DocumentacionInteligente.BackEnd.Models;
    using DocumentFormat.OpenXml.Packaging;
    using DocumentFormat.OpenXml.Wordprocessing;
    using System.IO;
    using System.Text;

    public class ReporteServiceWord
    {
        public byte[] GenerarDocumentoWord(DocumentoDTO doc)
        {
            using var ms = new MemoryStream();

            using (var wordDocument = WordprocessingDocument.Create(ms, DocumentFormat.OpenXml.WordprocessingDocumentType.Document, true))
            {
                var mainPart = wordDocument.AddMainDocumentPart();
                mainPart.Document = new Document();
                var body = new Body();

                // Título principal centrado
                var titulo = CreateParagraph("Documentación inteligente", true, true);
                body.Append(titulo);

                // Espaciado después del título
                body.Append(new Paragraph(new Run(new Text(""))));

                // Tabla de encabezado con 2 columnas
                var headerTable = new Table();

                //Lado de la tabla
                new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center };
                // Bordes de la tabla
                TableProperties tableProperties = new TableProperties(
                    new TableWidth() { Width = "100%", Type = TableWidthUnitValues.Pct },
                    new TableBorders(
                        new TopBorder { Val = BorderValues.Single, Size = 4 },
                        new BottomBorder { Val = BorderValues.Single, Size = 4 },
                        new LeftBorder { Val = BorderValues.Single, Size = 4 },
                        new RightBorder { Val = BorderValues.Single, Size = 4 },
                        new InsideHorizontalBorder { Val = BorderValues.Single, Size = 4 },
                        new InsideVerticalBorder { Val = BorderValues.Single, Size = 4 }
    )
);
                headerTable.AppendChild(tableProperties);

                void AddRow(string label, string value)
                {
                    var tr = new TableRow();

                    var labelCell = new TableCell(new Paragraph(new Run(new Text(label))))
                    {
                        TableCellProperties = new TableCellProperties(new TableCellWidth { Type = TableWidthUnitValues.Pct })
                    };

                    var valueCell = new TableCell(new Paragraph(new Run(new Text(value ?? "N/A"))))
                    {
                        TableCellProperties = new TableCellProperties(new TableCellWidth { Type = TableWidthUnitValues.Pct })
                    };

                    tr.Append(labelCell, valueCell);
                    headerTable.Append(tr);
                }

                AddRow("Fecha de Edición:", doc.FechaDeEdición?.ToString("dd/MM/yyyy") ?? "N/A");
                AddRow("Versión:", doc.Version ?? "N/A");
                AddRow("Código:", doc.CódigoDelDocumento ?? "N/A");
                AddRow("Elaborado por:", doc.ElaboradoPor ?? "N/A");
                AddRow("Revisado por:", doc.RevisadoPor ?? "N/A");

                body.Append(headerTable);

                // Espaciado después de la tabla
                body.Append(new Paragraph(new Run(new Text(""))));

                // Secciones del documento
                body.Append(CreateHeading("I. Objetivo"));
                body.Append(CreateParagraph(doc.IObjetivo ?? "N/A"));

                body.Append(CreateHeading("II. Alcance"));
                body.Append(CreateParagraph(doc.IIAlcance ?? "N/A"));

                body.Append(CreateHeading("III. Responsabilidades"));
                body.Append(CreateParagraph(doc.IIIResponsabilidades ?? "N/A"));

                body.Append(CreateHeading("IV. Desarrollo"));
                body.Append(CreateParagraph(doc.IVDesarrollo ?? "N/A"));

                body.Append(CreateHeading("V. Vigencia"));
                body.Append(CreateParagraph(doc.VVigencia ?? "N/A"));

                body.Append(CreateHeading("VI. Referencias Bibliográficas"));
                body.Append(CreateParagraph(doc.VIReferenciasBibliográficas ?? "N/A"));

                body.Append(CreateHeading("VII. Historial de cambio de Documentos"));

                if (doc.VIIHistorialDeCambioDeDocumentos != null && doc.VIIHistorialDeCambioDeDocumentos.Any())
                {
                    foreach (var cambio in doc.VIIHistorialDeCambioDeDocumentos)
                    {
                        body.Append(CreateParagraph($"N°: {cambio.Number ?? 0} | Fecha: {cambio.Date?.ToString("dd/MM/yyyy") ?? "N/A"} | Descripción: {cambio.Description ?? "N/A"}"));
                    }
                }
                else
                {
                    body.Append(CreateParagraph("Sin historial."));
                }

                body.Append(CreateHeading("VIII. Firmas"));
                body.Append(CreateParagraph(doc.VIIIFirmas ?? "N/A"));

                mainPart.Document.Append(body);
                mainPart.Document.Save();
            }

            return ms.ToArray();
        }

        // Párrafo genérico
        private Paragraph CreateParagraph(string text, bool bold = false, bool center = false)
        {
            var run = new Run(new Text(text));

            if (bold)
                run.RunProperties = new RunProperties(new Bold());

            var para = new Paragraph(run);

            if (center)
                para.ParagraphProperties = new ParagraphProperties(new Justification { Val = JustificationValues.Center });

            return para;
        }

        // Encabezado con estilo de título y negrita
        private Paragraph CreateHeading(string text)
        {
            var run = new Run(new Text(text))
            {
                RunProperties = new RunProperties(new Bold())
            };

            return new Paragraph(
                new ParagraphProperties(new ParagraphStyleId() { Val = "Heading1" }),
                run
            );
        }
    };
}
