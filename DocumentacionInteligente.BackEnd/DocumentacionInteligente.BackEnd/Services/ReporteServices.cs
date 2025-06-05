namespace DocumentacionInteligente.BackEnd.Services { 
using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using DocumentacionInteligente.BackEnd.Models;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;


    public class ReporteServices
{
    public byte[] GenerarReporteDocumento(DocumentoDTO doc)
    {
        using var ms = new MemoryStream();
        var writer = new PdfWriter(ms);
        var pdf = new PdfDocument(writer);
        var document = new Document(pdf);

        document.Add(new Paragraph("Documento Institucional")
            .SetTextAlignment(TextAlignment.CENTER)
            .SetFontSize(20)
            .SetMarginBottom(20));

        document.Add(new Paragraph($"Título: {doc.TítuloDelDocumento ?? "N/A"}"));
        document.Add(new Paragraph($"Fecha de Edición: {doc.FechaDeEdición?.ToString("dd/MM/yyyy") ?? "N/A"}"));
        document.Add(new Paragraph($"Versión: {doc.Version ?? "N/A"}"));
        document.Add(new Paragraph($"Código: {doc.CódigoDelDocumento ?? "N/A"}"));
        document.Add(new Paragraph($"Elaborado por: {doc.ElaboradoPor ?? "N/A"}"));
        document.Add(new Paragraph($"Revisado por: {doc.RevisadoPor ?? "N/A"}"));

        document.Add(new Paragraph("\nI. Objetivo"));
        document.Add(new Paragraph(doc.IObjetivo ?? "N/A"));

        document.Add(new Paragraph("\nII. Alcance"));
        document.Add(new Paragraph(doc.IIAlcance ?? "N/A"));

        document.Add(new Paragraph("\nIII. Responsabilidades"));
        document.Add(new Paragraph(doc.IIIResponsabilidades ?? "N/A"));

        document.Add(new Paragraph("\nIV. Desarrollo"));
        document.Add(new Paragraph(doc.IVDesarrollo ?? "N/A"));

        document.Add(new Paragraph("\nV. Vigencia"));
        document.Add(new Paragraph(doc.VVigencia ?? "N/A"));

        document.Add(new Paragraph("\nVI. Referencias Bibliográficas"));
        document.Add(new Paragraph(doc.VIReferenciasBibliográficas ?? "N/A"));

        // Historial de cambios    document.Add(new Paragraph("\nVII. Historial de Cambios").SetBold());
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
            document.Add(new Paragraph("Sin historial."));
        }

        document.Add(new Paragraph("\nVIII. Firmas"));
        document.Add(new Paragraph(doc.VIIIFirmas ?? "N/A"));

        document.Add(new Paragraph($"\n\nGenerado el {DateTime.Now:dd/MM/yyyy HH:mm}").SetFontSize(10).SetTextAlignment(TextAlignment.RIGHT));

        document.Close();
        return ms.ToArray();
    }

    //Documento 2 versión extendida.
    public byte[] GenerarReporteDocumento2(DocumentoDTO2 doc)
    {
        using var ms = new MemoryStream();
        var writer = new PdfWriter(ms);
        var pdf = new PdfDocument(writer);
        var document = new Document(pdf);

        //Titulo
        document.Add(new Paragraph("Documento Institucional").SetTextAlignment(TextAlignment.CENTER).SetFontSize(20).SetMarginBottom(20));

        // Información básica
        document.Add(new Paragraph($"Título: {doc.TítuloDelDocumento ?? "N/A"}"));
        document.Add(new Paragraph($"Fecha de Edición: {doc.FechaDeEdición?.ToString("dd/MM/yyyy") ?? "N/A"}"));
        document.Add(new Paragraph($"Versión: {doc.Version ?? "N/A"}"));
        document.Add(new Paragraph($"Código: {doc.CodigoDelDocumento ?? "N/A"}"));
        document.Add(new Paragraph($"Elaborado por: {doc.ElaboradoPor ?? "N/A"}"));
        document.Add(new Paragraph($"Revisado por: {doc.RevisadoPor ?? "N/A"}"));

        //Subtitulos
        // I. Objetivo
        document.Add(new Paragraph("\nI. Objetivo"));
        document.Add(new Paragraph(doc.IObjetivo ?? "N/A"));

        // II. Alcance
        document.Add(new Paragraph("\nII. Alcance"));
        document.Add(new Paragraph(doc.IIAlcance ?? "N/A"));

        // III. Responsabilidades
        document.Add(new Paragraph("\nIII. Responsabilidades"));
        if (doc.IIIResponsabilidades is JsonElement element && element.ValueKind == JsonValueKind.Object)
        {
            foreach (var propiedad in element.EnumerateObject())
            {
                document.Add(new Paragraph($"{propiedad.Name}: {propiedad.Value.ToString()}"));
            }
        }
        else
        {
            document.Add(new Paragraph(doc.IIIResponsabilidades?.ToString() ?? "N/A"));
        }

        // IV. Desarrollo
        document.Add(new Paragraph("\nIV. Desarrollo"));
        if (doc.IVDesarrollo != null && doc.IVDesarrollo.Any())
        {
            foreach (var subtema in doc.IVDesarrollo)
            {
                document.Add(new Paragraph($"- {subtema.Key}"));

                foreach (var paso in subtema.Value)
                {
                    document.Add(new Paragraph($"    {paso.Key}. {paso.Value}"));
                }
            }
        }
        else
        {
            document.Add(new Paragraph("N/A"));
        }

        // V. Vigencia
        document.Add(new Paragraph("\nV. Vigencia"));
        document.Add(new Paragraph(doc.VVigencia ?? "N/A"));

        // VI. Referencias
        document.Add(new Paragraph("\nVI. Referencias Bibliográficas"));
        document.Add(new Paragraph(doc.VIReferenciasBibliográficas ?? "N/A"));

        // VII. Historial de Cambios
        document.Add(new Paragraph("\nVII. Historial de Cambios"));
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
            document.Add(new Paragraph("Sin historial."));
        }

        // VIII. Firmas
        document.Add(new Paragraph("\nVIII. Firmas"));
        document.Add(new Paragraph(doc.VIIIFirmas ?? "N/A"));

        // Footer
        document.Add(new Paragraph($"\n\nGenerado el {DateTime.Now:dd/MM/yyyy HH:mm}").SetFontSize(10).SetTextAlignment(TextAlignment.RIGHT));

        document.Close();
        return ms.ToArray();
    }

}
}