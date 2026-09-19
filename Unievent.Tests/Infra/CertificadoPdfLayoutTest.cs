using System.Text.RegularExpressions;
using FluentAssertions;
using Unievent.Application.Dtos.Certificado;
using Unievent.Infra.Services;
using UglyToad.PdfPig;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;
using Xunit;

namespace Unievent.Tests.Infra;

public class CertificadoPdfLayoutTest
{
    [Theory]
    [InlineData(null)]
    [InlineData("Professora Érica de Oliveira")]
    public void Preserva_Textos_Longos_Em_Uma_Pagina_A4_Sem_Recortes(string? responsavel)
    {
        var dados = new CertificadoPdfDados(
            "Maria Eduarda de Albuquerque Vasconcelos e Silva dos Santos Oliveira",
            "Encontro Interinstitucional de Ciência, Tecnologia, Inovação e Desenvolvimento Sustentável da Comunidade Acadêmica",
            "Faculdade de Tecnologia de Ferraz de Vasconcelos — Centro Estadual de Educação Tecnológica Paula Souza",
            new DateTime(2026, 9, 20),
            string.Join(" ", Enumerable.Repeat("Atividades acadêmicas de ciência, tecnologia e inovação.", 9)),
            "UE-2026-93HD2A4BC92844AFB701E97D05B913AC",
            new DateTime(2026, 9, 21, 12, 0, 0, DateTimeKind.Utc), responsavel);

        var arquivo = new CertificadoPdfGenerator().Gerar(dados);
        using var pdf = PdfDocument.Open(arquivo.Conteudo);
        pdf.NumberOfPages.Should().Be(1);
        var page = pdf.GetPage(1);
        page.Width.Should().BeApproximately(841.89, 1);
        page.Height.Should().BeApproximately(595.28, 1);
        var text = Regex.Replace(ContentOrderTextExtractor.GetText(page), @"\s+", " ");
        text.Should().Contain(dados.NomeParticipante).And.Contain(dados.NomeEvento)
            .And.Contain(dados.Instituicao).And.Contain(dados.Texto).And.Contain(dados.Codigo)
            .And.Contain("20/09/2026").And.Contain("21/09/2026 12:00 UTC");
        if (responsavel is not null) text.Should().Contain(responsavel);
        text.Should().NotContain("Responsável: ");
        page.Letters.Should().OnlyContain(letter =>
            letter.GlyphRectangle.Left >= 24 && letter.GlyphRectangle.Right <= page.Width - 24 &&
            letter.GlyphRectangle.Bottom >= 24 && letter.GlyphRectangle.Top <= page.Height - 24);
    }
}
