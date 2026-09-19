namespace Unievent.Domain.Entities;

public class NotificacaoEvento : EntidadeBase
{
    public int AlunoId { get; set; }
    public Aluno Aluno { get; set; } = null!;
    public int EventoId { get; set; }
    public Evento Evento { get; set; } = null!;
    public required string Tipo { get; set; }
    public required string Assunto { get; set; }
    public required string Mensagem { get; set; }
    public DateTime ProcessadaEmUtc { get; set; } = DateTime.UtcNow;
    public bool Enviada { get; set; }
    public string? Erro { get; set; }
}
