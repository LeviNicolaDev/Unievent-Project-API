using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using Unievent.Application.Dtos.Certificado;

namespace Unievent.Infra.Templates;

public static class CertificadoPdfTemplate
{
    private static readonly Color Ink = Color.FromRgb(39, 39, 39);
    private static readonly Color Muted = Color.FromRgb(101, 101, 101);
    private static readonly Color Orange = Color.FromRgb(194, 77, 12);
    private static readonly Color Line = Color.FromRgb(228, 221, 215);
    private static readonly Unit ContentWidth = Unit.FromCentimeter(25.3);

    public static Document Criar(CertificadoPdfDados dados)
    {
        var document = new Document();
        document.Info.Title = $"Certificado - {dados.NomeEvento}";
        document.Info.Author = "UniEvent";
        document.Info.Subject = "Certificado de participação em evento";
        var normal = document.Styles[StyleNames.Normal]!;
        normal.Font.Name = "UniEvent Sans";
        normal.Font.Size = 11;
        normal.Font.Color = Ink;
        normal.ParagraphFormat.Alignment = ParagraphAlignment.Center;
        normal.ParagraphFormat.SpaceAfter = Unit.FromPoint(6);

        var section = document.AddSection();
        section.PageSetup.PageFormat = PageFormat.A4;
        section.PageSetup.Orientation = Orientation.Landscape;
        section.PageSetup.TopMargin = Unit.FromCentimeter(1.5);
        section.PageSetup.BottomMargin = Unit.FromCentimeter(3.1);
        section.PageSetup.LeftMargin = Unit.FromCentimeter(2.2);
        section.PageSetup.RightMargin = Unit.FromCentimeter(2.2);
        section.PageSetup.FooterDistance = Unit.FromCentimeter(1.1);

        var header = section.AddTable();
        ConfigureColumns(header, 0.5);
        var headerRow = header.AddRow();
        headerRow.VerticalAlignment = VerticalAlignment.Center;
        var brand = headerRow[0].AddParagraph("UniEvent");
        Format(brand, 21, true, Orange, ParagraphAlignment.Left);
        var documentType = headerRow[1].AddParagraph("CERTIFICADO DIGITAL");
        Format(documentType, 8, true, Muted, ParagraphAlignment.Right);
        Spacer(section, 18);

        Add(section, "CERTIFICADO", 32, true, Ink, 2);
        Add(section, "D E   P A R T I C I P A Ç Ã O", 10, true, Orange, 18);
        Add(section, "Certificamos que", 11, false, Muted, 7);
        Add(section, dados.NomeParticipante, dados.NomeParticipante.Length > 65 ? 23 : 28,
            true, Orange, 10);
        Add(section, "participou do evento", 11, false, Muted, 6);
        Add(section, dados.NomeEvento, dados.NomeEvento.Length > 100 ? 15 : 18,
            true, Ink, 18);

        var details = section.AddTable();
        ConfigureColumns(details, 0.75);
        details.Shading.Color = Color.FromRgb(252, 247, 242);
        details.Borders.Color = Line;
        details.Borders.Width = Unit.FromPoint(0.5);
        details.LeftPadding = details.RightPadding = Unit.FromPoint(14);
        var detailsRow = details.AddRow();
        detailsRow.TopPadding = detailsRow.BottomPadding = Unit.FromPoint(11);
        detailsRow.VerticalAlignment = VerticalAlignment.Center;
        AddField(detailsRow[0], "INSTITUIÇÃO ORGANIZADORA", dados.Instituicao);
        AddField(detailsRow[1], "DATA DO EVENTO", dados.DataEvento.ToString("dd/MM/yyyy"));
        Spacer(section, 14);

        // O conteúdo permanece em fluxo para acomodar nomes e textos sem recortes.
        var description = Add(section, dados.Texto, 11, false, Muted, 0);
        description.Format.LineSpacingRule = LineSpacingRule.OnePtFive;
        description.Format.LeftIndent = description.Format.RightIndent = Unit.FromPoint(18);

        AddFooter(section, dados);
        return document;
    }

    public static void AplicarAcabamento(PdfDocument document)
    {
        foreach (var page in document.Pages)
        {
            // Elementos vetoriais ficam atrás do texto e preservam a nitidez na impressão.
            using var graphics = XGraphics.FromPdfPage(page, XGraphicsPdfPageOptions.Prepend);
            var width = page.Width.Point;
            var height = page.Height.Point;
            var accent = new XSolidBrush(XColor.FromArgb(245, 111, 34));
            var soft = new XSolidBrush(XColor.FromArgb(255, 243, 232));
            graphics.DrawRectangle(new XPen(XColor.FromArgb(228, 221, 215), 0.7),
                24, 24, width - 48, height - 48);
            graphics.DrawPolygon(soft,
                [new XPoint(width - 134, 25), new XPoint(width - 25, 25), new XPoint(width - 25, 134)],
                XFillMode.Winding);
            graphics.DrawRectangle(accent, 24, 24, 112, 4);
            graphics.DrawRectangle(accent, width - 136, height - 28, 112, 4);
        }
    }

    private static void AddFooter(Section section, CertificadoPdfDados dados)
    {
        var footer = section.Footers.Primary.AddTable();
        ConfigureColumns(footer, 0.6);
        footer.Borders.Top.Color = Line;
        footer.Borders.Top.Width = Unit.FromPoint(0.7);
        var row = footer.AddRow();
        row.TopPadding = Unit.FromPoint(10);
        var hasResponsible = !string.IsNullOrWhiteSpace(dados.Responsavel);
        var responsible = row[0].AddParagraph(hasResponsible ? dados.Responsavel! : dados.Instituicao);
        Format(responsible, 10, true, Ink, ParagraphAlignment.Left);
        var responsibleLabel = row[0].AddParagraph(hasResponsible ? "Responsável pelo evento" : "Instituição organizadora");
        Format(responsibleLabel, 8, false, Muted, ParagraphAlignment.Left);

        var issuedLabel = row[1].AddParagraph("EMISSÃO");
        Format(issuedLabel, 8, true, Muted, ParagraphAlignment.Right);
        var issued = row[1].AddParagraph($"{dados.EmitidoEmUtc:dd/MM/yyyy HH:mm} UTC");
        Format(issued, 9, false, Ink, ParagraphAlignment.Right);

        var code = section.Footers.Primary.AddParagraph($"Código do certificado: {dados.Codigo}");
        Format(code, 8, false, Muted, ParagraphAlignment.Left);
        code.Format.SpaceBefore = Unit.FromPoint(8);
    }

    private static void ConfigureColumns(Table table, double firstColumnRatio)
    {
        table.Rows.LeftIndent = Unit.Zero;
        table.LeftPadding = table.RightPadding = Unit.Zero;
        table.AddColumn(Unit.FromPoint(ContentWidth.Point * firstColumnRatio));
        table.AddColumn(Unit.FromPoint(ContentWidth.Point * (1 - firstColumnRatio)));
    }

    private static void AddField(Cell cell, string label, string value)
    {
        var caption = cell.AddParagraph(label);
        Format(caption, 8, true, Orange, ParagraphAlignment.Left);
        caption.Format.SpaceAfter = Unit.FromPoint(5);
        var content = cell.AddParagraph(value);
        Format(content, 11, true, Ink, ParagraphAlignment.Left);
    }

    private static Paragraph Add(Section section, string text, int size, bool bold, Color color, int spaceAfter)
    {
        var paragraph = section.AddParagraph(text);
        Format(paragraph, size, bold, color, ParagraphAlignment.Center);
        paragraph.Format.SpaceAfter = Unit.FromPoint(spaceAfter);
        return paragraph;
    }

    private static void Format(Paragraph paragraph, int size, bool bold, Color color, ParagraphAlignment alignment)
    {
        paragraph.Format.Font.Size = size;
        paragraph.Format.Font.Bold = bold;
        paragraph.Format.Font.Color = color;
        paragraph.Format.Alignment = alignment;
        paragraph.Format.SpaceAfter = Unit.Zero;
    }

    private static void Spacer(Section section, int height)
    {
        var paragraph = section.AddParagraph();
        paragraph.Format.Font.Size = 1;
        paragraph.Format.LineSpacingRule = LineSpacingRule.Exactly;
        paragraph.Format.LineSpacing = Unit.FromPoint(height);
        paragraph.Format.SpaceAfter = Unit.Zero;
    }
}
