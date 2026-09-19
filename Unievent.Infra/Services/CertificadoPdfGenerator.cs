using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using MigraDoc.Rendering;
using PdfSharp.Fonts;
using Unievent.Application.Dtos.Certificado;
using Unievent.Application.Interfaces.Services;
using Unievent.Infra.Templates;

namespace Unievent.Infra.Services;

public sealed class CertificadoPdfGenerator : ICertificadoPdfGenerator
{
    private static readonly Lazy<bool> Fonts = new(() =>
    {
        GlobalFontSettings.FontResolver = new EmbeddedFontResolver();
        return true;
    });

    public CertificadoPdfArquivo Gerar(CertificadoPdfDados dados)
    {
        _ = Fonts.Value;
        var renderer = new PdfDocumentRenderer { Document = CertificadoPdfTemplate.Criar(dados) };
        renderer.RenderDocument();
        using var stream = new MemoryStream();
        using var pdf = renderer.PdfDocument;
        CertificadoPdfTemplate.AplicarAcabamento(pdf);
        pdf.Info.CreationDate = dados.EmitidoEmUtc;
        pdf.Save(stream, false);
        return new CertificadoPdfArquivo(stream.ToArray(),
            $"certificado-{Slug(dados.NomeEvento)}-{Slug(dados.NomeParticipante)}.pdf");
    }

    private static string Slug(string value)
    {
        var text = new string(value.Normalize(NormalizationForm.FormD)
            .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark).ToArray());
        text = Regex.Replace(text.ToLowerInvariant(), "[^a-z0-9]+", "-", RegexOptions.CultureInvariant).Trim('-');
        return text.Length == 0 ? "participacao" : text[..Math.Min(text.Length, 70)].TrimEnd('-');
    }

    private sealed class EmbeddedFontResolver : IFontResolver
    {
        public FontResolverInfo ResolveTypeface(string familyName, bool bold, bool italic) =>
            new(bold ? "LiberationSans-Bold" : "LiberationSans-Regular");

        public byte[] GetFont(string faceName)
        {
            using var stream = typeof(CertificadoPdfGenerator).Assembly.GetManifestResourceStream(
                $"Unievent.Infra.Templates.Fonts.{faceName}.ttf")
                ?? throw new InvalidOperationException("Fonte do certificado não encontrada.");
            using var buffer = new MemoryStream();
            stream.CopyTo(buffer);
            return buffer.ToArray();
        }
    }
}
