using FluentAssertions;
using Unievent.Application.Rules;
using Unievent.Domain.Entities;
using Unievent.Domain.Enuns;
using Xunit;

namespace Unievent.Tests.Application.Evento;

public class EventoRulesTest
{
    [Fact]
    public void Aluno_De_Outra_Instituicao_Nao_Participa_De_Evento_Restrito_A_Instituicao()
    {
        var evento = CriarEvento(PublicoPermitido.AlunosDaInstituicao, instituicaoId: 2);
        var aluno = CriarAluno(instituicaoId: 1);

        var podeParticipar = EventoRules.PodeParticipar(evento, aluno);

        podeParticipar.Should().BeFalse();
    }

    [Fact]
    public void Aluno_De_Outra_Instituicao_Visualiza_Evento_Publico_Restrito_A_Instituicao()
    {
        var evento = CriarEvento(PublicoPermitido.AlunosDaInstituicao, instituicaoId: 2);
        var aluno = CriarAluno(instituicaoId: 1);

        var podeVisualizar = EventoRules.PodeVisualizar(evento, aluno);

        podeVisualizar.Should().BeTrue();
    }

    [Fact]
    public void Aluno_Interno_Visualiza_Catalogo_Publico_De_Todas_As_Instituicoes()
    {
        var evento = CriarEvento(PublicoPermitido.AlunosDaInstituicao, instituicaoId: 2);

        var podeVisualizar = EventoRules.PodeVisualizar(evento, TipoParticipante.Interno);

        podeVisualizar.Should().BeTrue();
    }

    [Fact]
    public void Aluno_De_Outra_Instituicao_Participa_De_Evento_Para_Todos_Alunos_Fatec()
    {
        var evento = CriarEvento(PublicoPermitido.TodosAlunosFatec, instituicaoId: 2);
        var aluno = CriarAluno(instituicaoId: 1);

        var podeParticipar = EventoRules.PodeParticipar(evento, aluno);

        podeParticipar.Should().BeTrue();
    }

    [Fact]
    public void Publico_Geral_Aparece_Para_Consulta_Anonima()
    {
        var evento = CriarEvento(PublicoPermitido.PublicoGeral, instituicaoId: 2);

        var podeVisualizar = EventoRules.PodeVisualizar(evento, (TipoParticipante?)null);

        podeVisualizar.Should().BeTrue();
    }

    [Fact]
    public void Todos_Alunos_Fatec_Aparece_Para_Consulta_Anonima()
    {
        var evento = CriarEvento(PublicoPermitido.TodosAlunosFatec, instituicaoId: 2);

        var podeVisualizar = EventoRules.PodeVisualizar(evento, (TipoParticipante?)null);

        podeVisualizar.Should().BeTrue();
    }

    [Fact]
    public void Evento_Publico_Restrito_A_Instituicao_Aparece_Para_Consulta_Anonima()
    {
        var evento = CriarEvento(PublicoPermitido.AlunosDaInstituicao, instituicaoId: 2);

        var podeVisualizar = EventoRules.PodeVisualizar(evento, (TipoParticipante?)null);

        podeVisualizar.Should().BeTrue();
    }

    [Fact]
    public void Catalogo_Global_Retorna_Eventos_Publicos_De_Instituicoes_Diferentes()
    {
        var eventoFerraz = CriarEvento(PublicoPermitido.AlunosDaInstituicao, instituicaoId: 1);
        var eventoZonaLeste = CriarEvento(PublicoPermitido.TodosAlunosFatec, instituicaoId: 2);
        var eventos = new[] { eventoFerraz, eventoZonaLeste };

        var filtrados = eventos
            .AsQueryable()
            .Where(EventoRules.CatalogoVisivelExpression(null, null))
            .ToList();

        filtrados.Should().Contain(eventoFerraz);
        filtrados.Should().Contain(eventoZonaLeste);
    }

    [Fact]
    public void Catalogo_Global_Filtra_Por_Instituicao_Quando_Solicitado()
    {
        var eventoFerraz = CriarEvento(PublicoPermitido.AlunosDaInstituicao, instituicaoId: 1);
        var eventoZonaLeste = CriarEvento(PublicoPermitido.TodosAlunosFatec, instituicaoId: 2);
        var eventos = new[] { eventoFerraz, eventoZonaLeste };

        var filtrados = eventos
            .AsQueryable()
            .Where(EventoRules.CatalogoVisivelExpression(null, null))
            .Where(e => e.InstituicaoId == 1)
            .ToList();

        filtrados.Should().ContainSingle().Which.Should().Be(eventoFerraz);
    }

    [Fact]
    public void Evento_Privado_Nao_Aparece_Para_Consulta_Anonima()
    {
        var evento = CriarEvento(PublicoPermitido.AlunosDaInstituicao, instituicaoId: 2);
        evento.Visibilidade = VisibilidadeEvento.Privado;

        var podeVisualizar = EventoRules.PodeVisualizar(evento, (TipoParticipante?)null);

        podeVisualizar.Should().BeFalse();
    }

    [Fact]
    public void Filtro_Publico_Geral_Retorna_Somente_Eventos_Publico_Geral()
    {
        var publicoGeral = CriarEvento(PublicoPermitido.PublicoGeral, instituicaoId: 1);
        var todosAlunos = CriarEvento(PublicoPermitido.TodosAlunosFatec, instituicaoId: 1);
        var somenteInstituicao = CriarEvento(PublicoPermitido.AlunosDaInstituicao, instituicaoId: 1);
        var eventos = new[] { publicoGeral, todosAlunos, somenteInstituicao };

        var filtrados = eventos
            .AsQueryable()
            .Where(EventoRules.FiltroPublicoExpression(FiltroPublicoEvento.PublicoGeral))
            .ToList();

        filtrados.Should().ContainSingle().Which.Should().Be(publicoGeral);
    }

    [Fact]
    public void Filtro_Restrito_Retorna_Somente_Eventos_De_Alunos_Fatec_Ou_Instituicao()
    {
        var publicoGeral = CriarEvento(PublicoPermitido.PublicoGeral, instituicaoId: 1);
        var todosAlunos = CriarEvento(PublicoPermitido.TodosAlunosFatec, instituicaoId: 1);
        var somenteInstituicao = CriarEvento(PublicoPermitido.AlunosDaInstituicao, instituicaoId: 1);
        var eventos = new[] { publicoGeral, todosAlunos, somenteInstituicao };

        var filtrados = eventos
            .AsQueryable()
            .Where(EventoRules.FiltroPublicoExpression(FiltroPublicoEvento.Restrito))
            .ToList();

        filtrados.Should().NotContain(publicoGeral);
        filtrados.Should().Contain(todosAlunos);
        filtrados.Should().Contain(somenteInstituicao);
    }

    [Fact]
    public void Filtro_Publico_Pode_Ser_Combinado_Com_Instituicao()
    {
        var ferrazPublico = CriarEvento(PublicoPermitido.PublicoGeral, instituicaoId: 1);
        var ferrazRestrito = CriarEvento(PublicoPermitido.AlunosDaInstituicao, instituicaoId: 1);
        var zonaLestePublico = CriarEvento(PublicoPermitido.PublicoGeral, instituicaoId: 2);
        var eventos = new[] { ferrazPublico, ferrazRestrito, zonaLestePublico };

        var filtrados = eventos
            .AsQueryable()
            .Where(EventoRules.FiltroPublicoExpression(FiltroPublicoEvento.PublicoGeral))
            .Where(e => e.InstituicaoId == 1)
            .ToList();

        filtrados.Should().ContainSingle().Which.Should().Be(ferrazPublico);
    }

    private static Domain.Entities.Evento CriarEvento(PublicoPermitido publicoPermitido, int instituicaoId) => new()
    {
        Id = 10,
        Nome = "Evento multi-instituição",
        Descricao = "Evento de teste",
        Categoria = Categoria.Palestra,
        DataEvento = DateTime.UtcNow.AddDays(10),
        ResponsavelEventoId = 1,
        Capacidade = 100,
        Thumbnail = new List<string> { "thumbnail.jpg" },
        InstituicaoId = instituicaoId,
        Visibilidade = VisibilidadeEvento.Publico,
        PublicoPermitido = publicoPermitido
    };

    private static Domain.Entities.Aluno CriarAluno(int instituicaoId) => new()
    {
        Id = 20,
        Nome = "Aluno Fatec",
        Email = "aluno@fatec.sp.gov.br",
        Senha = "hash",
        FotoPerfil = "foto.jpg",
        IsAtivo = true,
        Role = Role.Aluno,
        DataNascimento = DateTime.UtcNow.AddYears(-20),
        TipoParticipante = TipoParticipante.Interno,
        InstituicaoId = instituicaoId
    };
}
