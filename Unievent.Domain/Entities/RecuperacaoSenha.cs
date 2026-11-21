namespace Unievent.Domain.Entities;

public class RecuperacaoSenha
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string TokenHash { get; set; }
    public required string TipoConta { get; set; }
    public int UsuarioId { get; set; }
    public required string SenhaFingerprint { get; set; }
    public DateTime CriadoEmUtc { get; set; }
    public DateTime ExpiraEmUtc { get; set; }
    public DateTime? UsadoEmUtc { get; set; }
}
