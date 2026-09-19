namespace Unievent.Application.Dtos.Certificado;

public record CertificadoPdfDados(
    string NomeParticipante,
    string NomeEvento,
    string Instituicao,
    DateTime DataEvento,
    string Texto,
    string Codigo,
    DateTime EmitidoEmUtc,
    string? Responsavel);

public record CertificadoPdfArquivo(byte[] Conteudo, string NomeArquivo);
