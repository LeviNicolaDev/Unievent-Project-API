using System.Linq.Expressions;
using Unievent.Domain.Entities;
using Unievent.Domain.Enuns;

namespace Unievent.Application.Rules;

public static class EventoRules
{
    public static bool PodeVisualizar(Evento evento, TipoParticipante? tipoParticipante)
    {
        return PodeVisualizarNoCatalogo(evento, tipoParticipante, null);
    }

    public static bool PodeVisualizar(Evento evento, Aluno aluno)
    {
        return PodeVisualizarNoCatalogo(evento, aluno.TipoParticipante, aluno.InstituicaoId);
    }

    public static bool PodeVisualizarNoCatalogo(Evento evento, TipoParticipante? tipoParticipante, int? instituicaoId)
    {
        return CatalogoVisivelExpression(tipoParticipante, instituicaoId).Compile()(evento);
    }

    public static Expression<Func<Evento, bool>> CatalogoVisivelExpression(TipoParticipante? tipoParticipante, int? instituicaoId)
    {
        if (tipoParticipante == TipoParticipante.Interno && instituicaoId.HasValue)
        {
            return evento =>
                evento.Visibilidade == VisibilidadeEvento.Publico ||
                evento.Visibilidade == VisibilidadeEvento.Privado &&
                evento.InstituicaoId.HasValue &&
                evento.InstituicaoId == instituicaoId;
        }

        return evento => evento.Visibilidade == VisibilidadeEvento.Publico;
    }

    public static Expression<Func<Evento, bool>> FiltroPublicoExpression(FiltroPublicoEvento filtro)
    {
        return filtro switch
        {
            FiltroPublicoEvento.PublicoGeral =>
                evento => evento.PublicoPermitido == PublicoPermitido.PublicoGeral,
            FiltroPublicoEvento.Restrito =>
                evento => evento.PublicoPermitido == PublicoPermitido.AlunosDaInstituicao ||
                          evento.PublicoPermitido == PublicoPermitido.TodosAlunosFatec,
            _ => evento => true
        };
    }

    public static bool PodeParticipar(Evento evento, Aluno aluno)
    {
        if (evento.Visibilidade == VisibilidadeEvento.Privado)
        {
            return aluno.TipoParticipante == TipoParticipante.Interno &&
                   evento.InstituicaoId.HasValue &&
                   aluno.InstituicaoId == evento.InstituicaoId;
        }

        return PublicoAceito(evento, aluno.TipoParticipante, aluno);
    }

    public static bool InscricoesAbertas(Evento evento, DateTime agoraUtc, out string? erro)
    {
        if (evento.InicioInscricoes.HasValue && agoraUtc < evento.InicioInscricoes.Value.ToUniversalTime())
        {
            erro = "As inscrições ainda não começaram";
            return false;
        }

        if (evento.FimInscricoes.HasValue && agoraUtc > evento.FimInscricoes.Value.ToUniversalTime())
        {
            erro = "As inscrições foram encerradas";
            return false;
        }

        erro = null;
        return true;
    }

    private static bool PublicoAceito(Evento evento, TipoParticipante? tipoParticipante, Aluno? aluno)
    {
        return evento.PublicoPermitido switch
        {
            PublicoPermitido.PublicoGeral => true,
            PublicoPermitido.TodosAlunosFatec =>
                tipoParticipante == TipoParticipante.Interno,
            PublicoPermitido.AlunosDaInstituicao =>
                aluno is { TipoParticipante: TipoParticipante.Interno } &&
                evento.InstituicaoId.HasValue &&
                aluno.InstituicaoId == evento.InstituicaoId,
            _ => false
        };
    }
}
