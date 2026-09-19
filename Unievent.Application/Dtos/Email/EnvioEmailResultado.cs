namespace Unievent.Application.Dtos.Email;

public enum SituacaoEnvioEmail { Enviado, FalhaTemporaria, FalhaPermanente, Incerto }

public record EnvioEmailResultado(SituacaoEnvioEmail Situacao, string? Erro = null);

public record EmailAnexo(byte[] Conteudo, string NomeArquivo, string ContentType = "application/pdf");
