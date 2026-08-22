using System.Globalization;
using System.Text;

namespace Unievent.Api.Data;

public sealed record FatecInstitutionCatalogItem(string Codigo, string Nome, string Cidade, string Estado = "SP");

public static class FatecInstitutionCatalog
{
    public static readonly IReadOnlyList<FatecInstitutionCatalogItem> Items =
    [
        new("fatec-adamantina", "Fatec Adamantina", "Adamantina"),
        new("fatec-americana-ministro-ralph-biasi", "Fatec Americana - Ministro Ralph Biasi", "Americana"),
        new("fatec-aracatuba-prof-fernando-amaral-de-almeida-prado", "Fatec Araçatuba - Prof. Fernando Amaral de Almeida Prado", "Araçatuba"),
        new("fatec-araraquara-prof-jose-arana-varela", "Fatec Araraquara - Prof. José Arana Varela", "Araraquara"),
        new("fatec-araras-antonio-brambilla", "Fatec Araras - Antonio Brambilla", "Araras"),
        new("fatec-assis-prof-dr-jose-luiz-guimaraes", "Fatec Assis - Prof. Dr. José Luiz Guimarães", "Assis"),
        new("fatec-atibaia", "Fatec Atibaia", "Atibaia"),
        new("fatec-baixada-santista-rubens-lara", "Fatec Baixada Santista - Rubens Lara", "Santos"),
        new("fatec-barretos-profa-edi-salvi-lima", "Fatec Barretos - Profa. Édi Salvi Lima", "Barretos"),
        new("fatec-barueri-padre-danilo-jose-de-oliveira-ohl", "Fatec Barueri - Padre Danilo José de Oliveira Ohl", "Barueri"),
        new("fatec-bauru", "Fatec Bauru", "Bauru"),
        new("fatec-bebedouro-jorge-caram-sabbag", "Fatec Bebedouro - Jorge Caram Sabbag", "Bebedouro"),
        new("fatec-benedicto-pagliato-votorantim", "Fatec Benedicto Pagliato - Votorantim", "Votorantim"),
        new("fatec-botucatu", "Fatec Botucatu", "Botucatu"),
        new("fatec-braganca-paulista-jornalista-omair-fagundes-de-oliveira", "Fatec Bragança Paulista - Jornalista Omair Fagundes de Oliveira", "Bragança Paulista"),
        new("fatec-campinas", "Fatec Campinas", "Campinas"),
        new("fatec-capao-bonito", "Fatec Capão Bonito", "Capão Bonito"),
        new("fatec-carapicuiba", "Fatec Carapicuíba", "Carapicuíba"),
        new("fatec-catanduva", "Fatec Catanduva", "Catanduva"),
        new("fatec-cotia", "Fatec Cotia", "Cotia"),
        new("fatec-cruzeiro-prof-waldomiro-may", "Fatec Cruzeiro - Prof. Waldomiro May", "Cruzeiro"),
        new("fatec-de-embu-das-artes", "Fatec de Embu das Artes", "Embu das Artes"),
        new("fatec-de-limeira", "Fatec de Limeira", "Limeira"),
        new("fatec-de-sao-roque-dr-bernardino-de-campos", "Fatec de São Roque - Dr. Bernardino de Campos", "São Roque"),
        new("fatec-diadema-luigi-papaiz", "Fatec Diadema - Luigi Papaiz", "Diadema"),
        new("fatec-esportes", "Fatec Esportes", "São Paulo"),
        new("fatec-ferraz-de-vasconcelos", "Fatec Ferraz de Vasconcelos", "Ferraz de Vasconcelos"),
        new("fatec-franca-dr-thomaz-novelino", "Fatec Franca - Dr. Thomaz Novelino", "Franca"),
        new("fatec-franco-da-rocha-giuliano-cecchettini", "Fatec Franco da Rocha - Giuliano Cecchettini", "Franco da Rocha"),
        new("fatec-garca-deputado-julio-julinho-marcondes-de-moura", "Fatec Garça - Deputado Julio Julinho Marcondes de Moura", "Garça"),
        new("fatec-guaratingueta-prof-joao-mod", "Fatec Guaratinguetá - Prof. João Mod", "Guaratinguetá"),
        new("fatec-guarulhos", "Fatec Guarulhos", "Guarulhos"),
        new("fatec-ilha-solteira", "Fatec Ilha Solteira", "Ilha Solteira"),
        new("fatec-indaiatuba-dr-archimedes-lammoglia", "Fatec Indaiatuba - Dr. Archimedes Lammoglia", "Indaiatuba"),
        new("fatec-ipiranga-pastor-eneas-tognini", "Fatec Ipiranga - Pastor Enéas Tognini", "São Paulo"),
        new("fatec-itapetininga-prof-antonio-belizandro-barbosa-rezende", "Fatec Itapetininga - Prof. Antonio Belizandro Barbosa Rezende", "Itapetininga"),
        new("fatec-itapevi", "Fatec Itapevi", "Itapevi"),
        new("fatec-itapira-ogari-de-castro-pacheco", "Fatec Itapira - Ogari de Castro Pacheco", "Itapira"),
        new("fatec-itaquaquecetuba", "Fatec Itaquaquecetuba", "Itaquaquecetuba"),
        new("fatec-itaquera-prof-miguel-reale", "Fatec Itaquera - Prof. Miguel Reale", "São Paulo"),
        new("fatec-itatiba-maria-eunice-amadeo-de-almeida", "Fatec Itatiba - Maria Eunice Amadeo de Almeida", "Itatiba"),
        new("fatec-itu-dom-amaury-castanho", "Fatec Itu - Dom Amaury Castanho", "Itu"),
        new("fatec-jaboticabal-nilo-de-stefani", "Fatec Jaboticabal - Nilo de Stéfani", "Jaboticabal"),
        new("fatec-jacarei-professor-francisco-de-moura", "Fatec Jacareí - Professor Francisco de Moura", "Jacareí"),
        new("fatec-jahu", "Fatec Jahu", "Jaú"),
        new("fatec-jales-professor-jose-camargo", "Fatec Jales - Professor José Camargo", "Jales"),
        new("fatec-jundiai-deputado-ary-fossen", "Fatec Jundiaí - Deputado Ary Fossen", "Jundiaí"),
        new("fatec-lins-prof-antonio-seabra", "Fatec Lins - Prof. Antonio Seabra", "Lins"),
        new("fatec-marilia-estudante-rafael-almeida-camarinha", "Fatec Marília - Estudante Rafael Almeida Camarinha", "Marília"),
        new("fatec-matao-luiz-marchesan", "Fatec Matão - Luiz Marchesan", "Matão"),
        new("fatec-maua", "Fatec Mauá", "Mauá"),
        new("fatec-mococa", "Fatec Mococa", "Mococa"),
        new("fatec-mogi-das-cruzes", "Fatec Mogi das Cruzes", "Mogi das Cruzes"),
        new("fatec-mogi-mirim-arthur-de-azevedo", "Fatec Mogi Mirim - Arthur de Azevedo", "Mogi Mirim"),
        new("fatec-olimpia", "Fatec Olímpia", "Olímpia"),
        new("fatec-osasco-prefeito-hirant-sanazar", "Fatec Osasco - Prefeito Hirant Sanazar", "Osasco"),
        new("fatec-ourinhos", "Fatec Ourinhos", "Ourinhos"),
        new("fatec-paulinia", "Fatec Paulínia", "Paulínia"),
        new("fatec-pindamonhangaba", "Fatec Pindamonhangaba", "Pindamonhangaba"),
        new("fatec-piracicaba-deputado-roque-trevisan", "Fatec Piracicaba - Deputado Roque Trevisan", "Piracicaba"),
        new("fatec-pompeia-shunji-nishimura", "Fatec Pompeia - Shunji Nishimura", "Pompeia"),
        new("fatec-porto-ferreira", "Fatec Porto Ferreira", "Porto Ferreira"),
        new("fatec-praia-grande", "Fatec Praia Grande", "Praia Grande"),
        new("fatec-presidente-prudente", "Fatec Presidente Prudente", "Presidente Prudente"),
        new("fatec-registro", "Fatec Registro", "Registro"),
        new("fatec-ribeirao-preto", "Fatec Ribeirão Preto", "Ribeirão Preto"),
        new("fatec-rio-claro-prof-alvares-gracioli", "Fatec Rio Claro - Prof. Álvares Gracioli", "Rio Claro"),
        new("fatec-santana-de-parnaiba", "Fatec Santana de Parnaíba", "Santana de Parnaíba"),
        new("fatec-santo-andre", "Fatec Santo André", "Santo André"),
        new("fatec-sao-bernardo-do-campo-adib-moises-dib", "Fatec São Bernardo do Campo - Adib Moisés Dib", "São Bernardo do Campo"),
        new("fatec-sao-caetano-do-sul-antonio-russo", "Fatec São Caetano do Sul - Antonio Russo", "São Caetano do Sul"),
        new("fatec-sao-carlos", "Fatec São Carlos", "São Carlos"),
        new("fatec-sao-jose-do-rio-preto", "Fatec São José do Rio Preto", "São José do Rio Preto"),
        new("fatec-sao-jose-dos-campos-prof-jessen-vidal", "Fatec São José dos Campos - Prof. Jessen Vidal", "São José dos Campos"),
        new("fatec-sao-paulo", "Fatec São Paulo", "São Paulo"),
        new("fatec-sao-sebastiao", "Fatec São Sebastião", "São Sebastião"),
        new("fatec-sebrae", "Fatec Sebrae", "São Paulo"),
        new("fatec-sertaozinho-deputado-waldyr-alceu-trigo", "Fatec Sertãozinho - Deputado Waldyr Alceu Trigo", "Sertãozinho"),
        new("fatec-sorocaba-jose-crespo-gonzales", "Fatec Sorocaba - José Crespo Gonzales", "Sorocaba"),
        new("fatec-sumare", "Fatec Sumaré", "Sumaré"),
        new("fatec-suzano", "Fatec Suzano", "Suzano"),
        new("fatec-taquaritinga", "Fatec Taquaritinga", "Taquaritinga"),
        new("fatec-tatuape-victor-civita", "Fatec Tatuapé - Victor Civita", "São Paulo"),
        new("fatec-tatui-prof-wilson-roberto-ribeiro-de-camargo", "Fatec Tatuí - Prof. Wilson Roberto Ribeiro de Camargo", "Tatuí"),
        new("fatec-taubate", "Fatec Taubaté", "Taubaté"),
        new("fatec-zona-leste", "Fatec Zona Leste", "São Paulo"),
        new("fatec-zona-sul-dom-paulo-evaristo-arns", "Fatec Zona Sul - Dom Paulo Evaristo Arns", "São Paulo")
    ];

    public static FatecInstitutionCatalogItem? FindByCode(string? codigo)
    {
        if (string.IsNullOrWhiteSpace(codigo)) return null;
        return Items.FirstOrDefault(item => string.Equals(item.Codigo, codigo, StringComparison.OrdinalIgnoreCase));
    }

    public static string NormalizeKey(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return string.Empty;

        var normalized = value.Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder(normalized.Length);

        foreach (var character in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(character) == UnicodeCategory.NonSpacingMark) continue;
            if (char.IsLetterOrDigit(character)) builder.Append(char.ToLowerInvariant(character));
        }

        return builder.ToString();
    }
}
