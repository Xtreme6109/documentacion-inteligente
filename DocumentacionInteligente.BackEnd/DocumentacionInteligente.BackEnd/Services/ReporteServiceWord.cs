namespace DocumentacionInteligente.BackEnd.Services
{
    using DocumentacionInteligente.BackEnd.Models;
    using DocumentFormat.OpenXml.Packaging;
    using DocumentFormat.OpenXml.Wordprocessing;
    using DocumentFormat.OpenXml;
    using System.IO;
    using System.Linq;

    public class ReporteServiceWord
    {
        private void AgregarEstilosNumeracion(MainDocumentPart mainPart)
        {
            var numberingPart = mainPart.AddNewPart<NumberingDefinitionsPart>();

            numberingPart.Numbering = new Numbering(
                new AbstractNum(
                    new Level(
                        new NumberingFormat() { Val = NumberFormatValues.Bullet },
                        new LevelText() { Val = "•" },
                        new LevelJustification() { Val = LevelJustificationValues.Left },
                        new ParagraphProperties(
                            new Indentation() { Left = "720", Hanging = "360" }
                        )
                    )
                    { LevelIndex = 0 }
                )
                { AbstractNumberId = 1 },

                new AbstractNum(
                    new Level(
                        new NumberingFormat() { Val = NumberFormatValues.Bullet },
                        new LevelText() { Val = "◦" },
                        new LevelJustification() { Val = LevelJustificationValues.Left },
                        new ParagraphProperties(
                            new Indentation() { Left = "720", Hanging = "360" }
                        )
                    )
                    { LevelIndex = 0 }
                )
                { AbstractNumberId = 2 },

                new NumberingInstance(
                    new AbstractNumId() { Val = 1 }
                ) { NumberID = 1 },

                new NumberingInstance(
                    new AbstractNumId() { Val = 2 }
                ) { NumberID = 2 }
            );
        }

        
        public byte[] GenerarDocumentoWord(DocumentoDTO doc)
        {
            using var ms = new MemoryStream();

            using (var wordDocument = WordprocessingDocument.Create(ms, DocumentFormat.OpenXml.WordprocessingDocumentType.Document, true))
            {
                var mainPart = wordDocument.AddMainDocumentPart();
                mainPart.Document = new Document();
                AgregarEstilosNumeracion(mainPart);
                var body = new Body();

                // Tabla de encabezado con 2 columnas
                var headerTable = new Table();

                // Propiedades de la tabla
                TableProperties tableProperties = new TableProperties(
                    new TableWidth { Width = "100%", Type = TableWidthUnitValues.Pct },
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

                // Método para insertar imagen (aquí con texto temporal, puedes reemplazarlo con la imagen real)
                void InsertImageCell(TableCell cell)
                {
                    cell.Append(new Paragraph(new Run(new Text("[Logo Aquí]"))));
                }

                // Primera fila
                var tr1 = new TableRow();

                // Celda con imagen (rowspan=3)
                var cellImg = new TableCell();
                cellImg.Append(new TableCellProperties(
                    new VerticalMerge() { Val = MergedCellValues.Restart },
                    new TableCellWidth() { Width = "3000", Type = TableWidthUnitValues.Dxa }
                ));
                InsertImageCell(cellImg);
                tr1.Append(cellImg);

                // Celda título con rowspan=2 y colspan=2
                var cellTitle = new TableCell();
                cellTitle.Append(new TableCellProperties(
                    new GridSpan() { Val = 2 },
                    new VerticalMerge() { Val = MergedCellValues.Restart },
                    new TableCellWidth() { Width = "7000", Type = TableWidthUnitValues.Dxa }
                ));
                cellTitle.Append(new Paragraph(new Run(new Text(doc.Titulo ?? "Título"))));
                tr1.Append(cellTitle);

                // Celda "Página x de y"
                var cellPage = new TableCell();
                cellPage.Append(new TableCellProperties(
                    new TableCellWidth() { Width = "3000", Type = TableWidthUnitValues.Dxa }
                ));
                cellPage.Append(new Paragraph(new Run(new Text($"Página {doc.Hoja} de {doc.TotalHojas}"))));
                tr1.Append(cellPage);

                headerTable.Append(tr1);

                // Segunda fila
                var tr2 = new TableRow();

                // Celda imagen rowspan continue
                var cellImgCont1 = new TableCell();
                cellImgCont1.Append(new TableCellProperties(
                    new VerticalMerge() { Val = MergedCellValues.Continue }
                ));
                tr2.Append(cellImgCont1);

                // Celda título rowspan continue y colspan=2 vacía (se mantiene por la fusión)
                var cellTitleCont = new TableCell();
                cellTitleCont.Append(new TableCellProperties(
                    new GridSpan() { Val = 2 },
                    new VerticalMerge() { Val = MergedCellValues.Continue }
                ));
                tr2.Append(cellTitleCont);

                // Celda código
                var cellCode = new TableCell();
                cellCode.Append(new TableCellProperties(
                    new TableCellWidth() { Width = "3000", Type = TableWidthUnitValues.Dxa }
                ));
                cellCode.Append(new Paragraph(new Run(new Text($"Código: {doc.CódigoDelDocumento ?? "N/A"}"))));
                tr2.Append(cellCode);

                headerTable.Append(tr2);

                // Tercera fila
                var tr3 = new TableRow();

                // Celda imagen rowspan continue
                var cellImgCont2 = new TableCell();
                cellImgCont2.Append(new TableCellProperties(
                    new VerticalMerge() { Val = MergedCellValues.Continue }
                ));
                tr3.Append(cellImgCont2);

                // Celda Fecha de Edición
                var cellEditDate = new TableCell();
                cellEditDate.Append(new TableCellProperties(
                    new TableCellWidth() { Width = "3000", Type = TableWidthUnitValues.Dxa }
                ));
                cellEditDate.Append(new Paragraph(new Run(new Text($"Fecha Edición: {doc.FechaDeEdición?.ToString("dd/MM/yyyy") ?? "N/A"}"))));
                tr3.Append(cellEditDate);

                // Celda Versión
                var cellVersion = new TableCell();
                cellVersion.Append(new TableCellProperties(
                    new TableCellWidth() { Width = "3000", Type = TableWidthUnitValues.Dxa }
                ));
                cellVersion.Append(new Paragraph(new Run(new Text($"Versión: {doc.Version ?? "N/A"}"))));
                tr3.Append(cellVersion);

                // Celda Fecha de Divulgación vacía
                var cellDivulgacion = new TableCell();
                cellDivulgacion.Append(new TableCellProperties(
                    new TableCellWidth() { Width = "3000", Type = TableWidthUnitValues.Dxa }
                ));
                cellDivulgacion.Append(new Paragraph(new Run(new Text("Fecha de Divulgación: "))));
                tr3.Append(cellDivulgacion);

                headerTable.Append(tr3);

                // Cuarta fila
                var tr4 = new TableRow();

                // Celda Elaboró
                var cellElaboro = new TableCell();
                cellElaboro.Append(new TableCellProperties(
                    new TableCellWidth() { Width = "3000", Type = TableWidthUnitValues.Dxa }
                ));
                cellElaboro.Append(new Paragraph(new Run(new Text($"Elaboró: {doc.ElaboradoPor ?? "N/A"}"))));
                tr4.Append(cellElaboro);

                // Celda Revisó colspan=2
                var cellReviso = new TableCell();
                cellReviso.Append(new TableCellProperties(
                    new GridSpan() { Val = 2 },
                    new TableCellWidth() { Width = "6000", Type = TableWidthUnitValues.Dxa }
                ));
                cellReviso.Append(new Paragraph(new Run(new Text($"Revisó: {doc.RevisadoPor ?? "N/A"}"))));
                tr4.Append(cellReviso);

                // Celda Autorizó
                var cellAutorizo = new TableCell();
                cellAutorizo.Append(new TableCellProperties(
                    new TableCellWidth() { Width = "3000", Type = TableWidthUnitValues.Dxa }
                ));
                cellAutorizo.Append(new Paragraph(new Run(new Text($"Autorizó: {doc.AutorizadoPor ?? "N/A"}"))));
                tr4.Append(cellAutorizo);

                headerTable.Append(tr4);

                // Agrega la tabla al body
                body.Append(headerTable);

                // Espaciado después de la tabla
                body.Append(new Paragraph(new Run(new Text(""))));

                // Secciones del documento
                body.Append(CreateHeading("I. Objetivo"));
                body.Append(CreateParagraph(doc.IObjetivo ?? "N/A"));

                body.Append(CreateHeading("II. Alcance"));
                body.Append(CreateParagraph(doc.IIAlcance ?? "N/A"));

                body.Append(CreateHeading("III. Responsabilidades"));
                if (doc.IIIResponsabilidades != null && doc.IIIResponsabilidades.Any())
                {
                    foreach (var item in doc.IIIResponsabilidades)
                    {
                        body.Append(CreateBulletParagraph(item.Key, item.Value, 1)); // Usa ID 1
                    }
                }


                body.Append(CreateHeading("IV. Desarrollo"));
                if (doc.IVDesarrollo != null && doc.IVDesarrollo.Any())
                {
                    foreach (var item in doc.IVDesarrollo)
                    {
                        body.Append(CreateBulletParagraph(item.Key, item.Value, 2)); // Usa ID 2
                    }
                }


                body.Append(CreateHeading("V. Vigencia"));
                body.Append(CreateParagraph(doc.VVigencia ?? "N/A"));

                body.Append(CreateHeading("VI. Referencias Bibliográficas"));
                body.Append(CreateParagraph(doc.VIReferenciasBibliográficas ?? "N/A"));

                body.Append(CreateHeading("VII. Historial de cambio de Documentos"));

                if (doc.VIIHistorialDeCambioDeDocumentos != null && doc.VIIHistorialDeCambioDeDocumentos.Any())
                {
                    var changeTable = new Table();

                    // Usa las mismas propiedades de tabla que headerTable para bordes y ancho
                    changeTable.AppendChild(tableProperties.CloneNode(true));

                    var headerRow = new TableRow();
                    headerRow.Append(
                        CreateTableCell("NÚMERO DE REVISIÓN", true),
                        CreateTableCell("FECHA", true),
                        CreateTableCell("DESCRIPCIÓN DEL CAMBIO", true)
                    );
                    changeTable.Append(headerRow);

                    foreach (var cambio in doc.VIIHistorialDeCambioDeDocumentos)
                    {
                        var row = new TableRow();
                        row.Append(
                            CreateTableCell(cambio.Number.ToString()),
                            CreateTableCell(cambio.Date?.ToString("yyyy-MM-dd") ?? "N/A"),
                            CreateTableCell(cambio.Description ?? "N/A")
                        );
                        changeTable.Append(row);
                    }
                    body.Append(changeTable);
                }
                else
                {
                    body.Append(CreateParagraph("Sin historial."));
                }

                // Espaciado después de la tabla historial
                body.Append(new Paragraph(new Run(new Text(""))));

                body.Append(CreateHeading("VIII. Firmas"));
                body.Append(CreateParagraph(doc.VIIIFirmas ?? "N/A"));

                mainPart.Document.Append(body);
                AddFooterWithPageNumber(mainPart);
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
                new ParagraphProperties(
                    new ParagraphStyleId() { Val = "Heading1" },
                    new SpacingBetweenLines { Before = "200", After = "200" }
                ),
                run
            );
        }

        // Método auxiliar para crear celdas con texto y opción de negrita
        private TableCell CreateTableCell(string text, bool bold = false)
        {
            var run = new Run(new Text(text));
            if (bold)
                run.RunProperties = new RunProperties(new Bold());

            var para = new Paragraph(run);

            return new TableCell(para);
        }

        private static Paragraph CreateBulletParagraph(string title, string content, int numberingId)
        {
            return new Paragraph(
                new ParagraphProperties(
                    new NumberingProperties(
                        new NumberingLevelReference() { Val = 0 },
                        new NumberingId() { Val = numberingId }
                    )
                ),
                new Run(
                    new RunProperties(new Bold()),
                    new Text($"{title}: ")
                ),
                new Run(new Text(content))
            );
        }

       private void AddFooterWithPageNumber(MainDocumentPart mainPart)
        {
            var footerPart = mainPart.AddNewPart<FooterPart>();
            string footerPartId = mainPart.GetIdOfPart(footerPart);

            var paragraph = new Paragraph(
                new ParagraphProperties(
                    new Justification { Val = JustificationValues.Center }
                ),

                // "Página " con espacio incluido y preservado
                new Run(new Text("Página ") { Space = SpaceProcessingModeValues.Preserve }),

                // Campo PAGE
                new Run(new FieldChar { FieldCharType = FieldCharValues.Begin }),
                new Run(new FieldCode(" PAGE ")),
                new Run(new FieldChar { FieldCharType = FieldCharValues.Separate }),
                new Run(new Text("1")),
                new Run(new FieldChar { FieldCharType = FieldCharValues.End }),

                // " de " con espacios incluidos y preservados
                new Run(new Text(" de ") { Space = SpaceProcessingModeValues.Preserve }),

                // Campo NUMPAGES
                new Run(new FieldChar { FieldCharType = FieldCharValues.Begin }),
                new Run(new FieldCode(" NUMPAGES ")),
                new Run(new FieldChar { FieldCharType = FieldCharValues.Separate }),
                new Run(new Text("1")),
                new Run(new FieldChar { FieldCharType = FieldCharValues.End })
            );

            // Crea el Footer con el párrafo
            var footer = new Footer(paragraph);
            footerPart.Footer = footer;
            footerPart.Footer.Save();

            // Agrega el FooterReference al documento
            var sectionProps = mainPart.Document.Body.Elements<SectionProperties>().LastOrDefault();
            if (sectionProps == null)
            {
                sectionProps = new SectionProperties();
                mainPart.Document.Body.Append(sectionProps);
            }

            sectionProps.RemoveAllChildren<FooterReference>();
            sectionProps.Append(new FooterReference() { Id = footerPartId, Type = HeaderFooterValues.Default });
        }

    }
}
