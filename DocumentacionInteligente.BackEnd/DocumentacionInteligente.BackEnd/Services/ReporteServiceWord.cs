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

            // Crea el documento Word
            using (var wordDocument = WordprocessingDocument.Create(ms, DocumentFormat.OpenXml.WordprocessingDocumentType.Document, true))
            {
                var mainPart = wordDocument.AddMainDocumentPart();
                mainPart.Document = new Document();
                var body = new Body();

                // Encabezado principal
                body.Append(CreateHeading("Documento Institucional"));

                body.Append(CreateParagraph($"Título del Documento: {doc.TítuloDelDocumento ?? "N/A"}"));
                body.Append(CreateParagraph($"Fecha de Edición: {doc.FechaDeEdición?.ToString("dd/MM/yyyy") ?? "N/A"}"));
                body.Append(CreateParagraph($"Versión: {doc.Version ?? "N/A"}"));
                body.Append(CreateParagraph($"Código: {doc.CódigoDelDocumento ?? "N/A"}"));
                body.Append(CreateParagraph($"Elaborado por: {doc.ElaboradoPor ?? "N/A"}"));
                body.Append(CreateParagraph($"Revisado por: {doc.RevisadoPor ?? "N/A"}"));

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

        private Paragraph CreateParagraph(string text)
        {
            return new Paragraph(new Run(new Text(text)));
        }

        private Paragraph CreateHeading(string text)
        {
            return new Paragraph(
                new Run(
                    new Text(text)
                )
            )
            {
                ParagraphProperties = new ParagraphProperties(
                    new ParagraphStyleId() { Val = "Heading1" }
                )
            };
        }
    };
}

        
    
