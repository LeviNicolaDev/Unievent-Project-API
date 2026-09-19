using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Unievent.Application.Common;
using Unievent.Application.Dtos.Automacoes;
using Unievent.Application.Dtos.Participacao;
using Unievent.Application.Interfaces.Repository;
using Unievent.Application.Interfaces.Services;
using Unievent.Application.Services;
using Unievent.Domain.Entities;
using Unievent.Domain.Enuns;
using Xunit;
using AlunoEntity = Unievent.Domain.Entities.Aluno;

namespace Unievent.Tests.Application.Participacao;

public class ParticipacaoServiceTest
{
    private readonly Mock<IEventoRepository> _eventos = new();
    private readonly Mock<IParticipacaoRepository> _participacoes = new();
    private readonly Mock<IAlunoRepository> _alunos = new();
    private readonly Mock<ICertificadoAutomaticoService> _certificadosAutomaticos = new();
    private readonly IParticipacaoService _service;

    public ParticipacaoServiceTest()
    {
        _participacoes.Setup(r => r.TryConfirmarPresencaAsync(It.IsAny<Domain.Entities.Participacao>())).ReturnsAsync(true);
        _certificadosAutomaticos
            .Setup(s => s.ProcessarAposCheckInAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<CertificadoAutomaticoResult>.Success(
                new CertificadoAutomaticoResult(true, true, true, false, null)));

        _service = new ParticipacaoService(
            _eventos.Object,
            _participacoes.Object,
            _alunos.Object,
            Mock.Of<ILogger<ParticipacaoService>>(),
            _certificadosAutomaticos.Object);
    }

    [Fact]
    public async Task Deve_Rejeitar_CheckIn_Com_Localizacao_Imprecisa()
    {
        var participacao = new Domain.Entities.Participacao(1, 2)
        {
            Evento = new Domain.Entities.Evento
            {
                Id = 2,
                Nome = "Evento",
                Descricao = "Teste",
                Categoria = Categoria.Palestra,
                DataEvento = DateTime.UtcNow,
                ResponsavelEventoId = 1,
                Capacidade = 10,
                Thumbnail = new List<string>(),
                Latitude = -23.5400,
                Longitude = -46.3700,
                RaioCheckInMetros = 150
            }
        };
        _participacoes.Setup(r => r.GetByCodigoIngressoAsync(participacao.CodigoIngresso)).ReturnsAsync(participacao);

        var result = await _service.ValidarCheckInAsync(10, new CheckInRequest
        {
            CodigoIngresso = participacao.CodigoIngresso,
            Latitude = -23.5,
            Longitude = -46.4,
            PrecisaoMetros = 100
        });

        result.IsFailure.Should().BeTrue();
        result.Errors.Should().Contain("Localização imprecisa; aproxime-se do evento e tente novamente");
        _participacoes.Verify(r => r.GetByCodigoIngressoAsync(participacao.CodigoIngresso), Times.Once);
        _participacoes.Verify(r => r.Update(It.IsAny<Domain.Entities.Participacao>()), Times.Never);
        _certificadosAutomaticos.Verify(
            s => s.ProcessarAposCheckInAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Deve_Confirmar_CheckIn_Valido_Apenas_Com_Codigo_Do_Ingresso()
    {
        var participacao = new Domain.Entities.Participacao(1, 2)
        {
            Id = 7,
            Aluno = new AlunoEntity
            {
                Id = 1, Nome = "Participante", Email = "pessoa@example.com", Senha = "hash",
                FotoPerfil = "foto.png", IsAtivo = true, DataNascimento = new DateTime(2000, 1, 1),
                TipoParticipante = TipoParticipante.Externo
            },
            Evento = new Domain.Entities.Evento
            {
                Id = 2, Nome = "Evento", Descricao = "Teste", Categoria = Categoria.Palestra,
                DataEvento = DateTime.UtcNow, ResponsavelEventoId = 1, Capacidade = 10,
                Thumbnail = new List<string>()
            }
        };
        _participacoes.Setup(r => r.GetByCodigoIngressoAsync(participacao.CodigoIngresso)).ReturnsAsync(participacao);
        _participacoes.Setup(r => r.Update(participacao)).ReturnsAsync(participacao);

        var result = await _service.ValidarCheckInAsync(10, new CheckInRequest
        {
            CodigoIngresso = participacao.CodigoIngresso
        });

        result.IsSuccess.Should().BeTrue();
        result.Value!.PresencaGarantida.Should().BeTrue();
        _participacoes.Verify(r => r.TryConfirmarPresencaAsync(It.IsAny<Domain.Entities.Participacao>()), Times.Once);
        _certificadosAutomaticos.Verify(
            s => s.ProcessarAposCheckInAsync(7, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Deve_Manter_CheckIn_Confirmado_Quando_Email_Do_Certificado_Falhar()
    {
        var participacao = new Domain.Entities.Participacao(1, 2)
        {
            Id = 8,
            Aluno = new AlunoEntity
            {
                Id = 1, Nome = "Participante", Email = "pessoa@fatec.sp.gov.br", Senha = "hash",
                FotoPerfil = "foto.png", IsAtivo = true, DataNascimento = new DateTime(2000, 1, 1),
                TipoParticipante = TipoParticipante.Interno
            },
            Evento = new Domain.Entities.Evento
            {
                Id = 2, Nome = "Evento", Descricao = "Teste", Categoria = Categoria.Palestra,
                DataEvento = DateTime.UtcNow, ResponsavelEventoId = 1, Capacidade = 10,
                Thumbnail = new List<string>()
            }
        };
        _certificadosAutomaticos
            .Setup(s => s.ProcessarAposCheckInAsync(8, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<CertificadoAutomaticoResult>.Failure("Erro ao enviar e-mail"));
        _participacoes.Setup(r => r.GetByCodigoIngressoAsync(participacao.CodigoIngresso)).ReturnsAsync(participacao);
        _participacoes.Setup(r => r.Update(participacao)).ReturnsAsync(participacao);

        var result = await _service.ValidarCheckInAsync(10, new CheckInRequest
        {
            CodigoIngresso = participacao.CodigoIngresso
        });

        result.IsSuccess.Should().BeTrue();
        result.Value!.PresencaGarantida.Should().BeTrue();
        _participacoes.Verify(r => r.TryConfirmarPresencaAsync(It.IsAny<Domain.Entities.Participacao>()), Times.Once);
        _certificadosAutomaticos.Verify(
            s => s.ProcessarAposCheckInAsync(8, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Nao_Deve_Reprocessar_Certificado_Quando_CheckIn_Ja_Estava_Confirmado()
    {
        var participacao = new Domain.Entities.Participacao(1, 2)
        {
            Id = 9,
            Aluno = new AlunoEntity
            {
                Id = 1, Nome = "Participante", Email = "pessoa@fatec.sp.gov.br", Senha = "hash",
                FotoPerfil = "foto.png", IsAtivo = true, DataNascimento = new DateTime(2000, 1, 1),
                TipoParticipante = TipoParticipante.Interno
            },
            Evento = new Domain.Entities.Evento
            {
                Id = 2, Nome = "Evento", Descricao = "Teste", Categoria = Categoria.Palestra,
                DataEvento = DateTime.UtcNow, ResponsavelEventoId = 1, Capacidade = 10,
                Thumbnail = new List<string>()
            }
        };
        participacao.ConfirmarPresencaPorCodigo(10);
        _participacoes.Setup(r => r.GetByCodigoIngressoAsync(participacao.CodigoIngresso)).ReturnsAsync(participacao);

        var result = await _service.ValidarCheckInAsync(10, new CheckInRequest
        {
            CodigoIngresso = participacao.CodigoIngresso
        });

        result.IsSuccess.Should().BeTrue();
        _participacoes.Verify(r => r.SaveChangesAsync(), Times.Never);
        _certificadosAutomaticos.Verify(
            s => s.ProcessarAposCheckInAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Deve_Confirmar_CheckIn_Valido_Dentro_Do_Raio()
    {
        var participacao = new Domain.Entities.Participacao(1, 2)
        {
            Aluno = new AlunoEntity
            {
                Id = 1, Nome = "Participante", Email = "pessoa@example.com", Senha = "hash",
                FotoPerfil = "foto.png", IsAtivo = true, DataNascimento = new DateTime(2000, 1, 1),
                TipoParticipante = TipoParticipante.Externo
            },
            Evento = new Domain.Entities.Evento
            {
                Id = 2, Nome = "Evento", Descricao = "Teste", Categoria = Categoria.Palestra,
                DataEvento = DateTime.UtcNow, ResponsavelEventoId = 1, Capacidade = 10,
                Thumbnail = new List<string>(), Latitude = -23.5400, Longitude = -46.3700,
                RaioCheckInMetros = 150
            }
        };
        _participacoes.Setup(r => r.GetByCodigoIngressoAsync(participacao.CodigoIngresso)).ReturnsAsync(participacao);
        _participacoes.Setup(r => r.Update(participacao)).ReturnsAsync(participacao);

        var result = await _service.ValidarCheckInAsync(10, new CheckInRequest
        {
            CodigoIngresso = participacao.CodigoIngresso,
            Latitude = -23.5400,
            Longitude = -46.3700,
            PrecisaoMetros = 10
        });

        result.IsSuccess.Should().BeTrue();
        result.Value!.PresencaGarantida.Should().BeTrue();
        _participacoes.Verify(r => r.TryConfirmarPresencaAsync(It.IsAny<Domain.Entities.Participacao>()), Times.Once);
    }

    [Fact]
    public async Task Deve_Bloquear_Publico_Externo_Em_Evento_Apenas_Para_Alunos_Fatec()
    {
        _alunos.Setup(r => r.ListarAlunoById(1)).ReturnsAsync(new AlunoEntity
        {
            Id = 1, Nome = "Externo", Email = "externo@example.com", Senha = "hash",
            FotoPerfil = "foto.png", IsAtivo = true, DataNascimento = new DateTime(2000, 1, 1),
            TipoParticipante = TipoParticipante.Externo
        });
        _eventos.Setup(r => r.ListarEventoById(2)).ReturnsAsync(new Domain.Entities.Evento
        {
            Id = 2, Nome = "Interno", Descricao = "Teste", Categoria = Categoria.Palestra,
            DataEvento = DateTime.UtcNow.AddDays(1), ResponsavelEventoId = 1, Capacidade = 10,
            Thumbnail = new List<string>(), PublicoPermitido = PublicoPermitido.TodosAlunosFatec
        });

        var result = await _service.InscreverAsync(1, 2);

        result.IsFailure.Should().BeTrue();
        _participacoes.Verify(r => r.TryAddWithinCapacityAsync(It.IsAny<Domain.Entities.Participacao>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task Deve_Bloquear_Aluno_De_Outra_Instituicao_Em_Evento_Exclusivo()
    {
        _alunos.Setup(r => r.ListarAlunoById(1)).ReturnsAsync(new AlunoEntity
        {
            Id = 1, Nome = "Aluno Ferraz", Email = "aluno@fatec.sp.gov.br", Senha = "hash",
            FotoPerfil = "foto.png", IsAtivo = true, DataNascimento = new DateTime(2000, 1, 1),
            TipoParticipante = TipoParticipante.Interno, InstituicaoId = 1
        });
        _eventos.Setup(r => r.ListarEventoById(2)).ReturnsAsync(new Domain.Entities.Evento
        {
            Id = 2, Nome = "Evento exclusivo", Descricao = "Teste", Categoria = Categoria.Palestra,
            DataEvento = DateTime.UtcNow.AddDays(1), ResponsavelEventoId = 1, Capacidade = 10,
            Thumbnail = new List<string>(), PublicoPermitido = PublicoPermitido.AlunosDaInstituicao,
            Visibilidade = VisibilidadeEvento.Publico, InstituicaoId = 2,
            Instituicao = new Domain.Entities.Instituicao
            {
                Id = 2,
                Nome = "FATEC Zona Leste",
                FotoPerfil = "foto.png",
                Cnpj = "00.000.000/0001-00",
                Rua = "Rua A",
                Cidade = "São Paulo",
                Bairro = "Bairro",
                Estado = "SP",
                Cep = "00000000",
                Numero = "1"
            }
        });

        var result = await _service.InscreverAsync(1, 2);

        result.IsFailure.Should().BeTrue();
        result.Errors.Should().Contain("Este evento é exclusivo para alunos da FATEC Zona Leste.");
        _participacoes.Verify(r => r.TryAddWithinCapacityAsync(It.IsAny<Domain.Entities.Participacao>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task Deve_Permitir_Aluno_De_Outra_Instituicao_Em_Evento_Para_Todos_Alunos_Fatec()
    {
        _alunos.Setup(r => r.ListarAlunoById(1)).ReturnsAsync(new AlunoEntity
        {
            Id = 1, Nome = "Aluno Ferraz", Email = "aluno@fatec.sp.gov.br", Senha = "hash",
            FotoPerfil = "foto.png", IsAtivo = true, DataNascimento = new DateTime(2000, 1, 1),
            TipoParticipante = TipoParticipante.Interno, InstituicaoId = 1
        });
        _eventos.Setup(r => r.ListarEventoById(2)).ReturnsAsync(new Domain.Entities.Evento
        {
            Id = 2, Nome = "Workshop", Descricao = "Teste", Categoria = Categoria.Palestra,
            DataEvento = DateTime.UtcNow.AddDays(1), ResponsavelEventoId = 1, Capacidade = 10,
            Thumbnail = new List<string>(), PublicoPermitido = PublicoPermitido.TodosAlunosFatec,
            Visibilidade = VisibilidadeEvento.Publico, InstituicaoId = 2
        });
        _participacoes.Setup(r => r.CountByEventoIdAsync(2)).ReturnsAsync(0);
        _participacoes.Setup(r => r.ExistsAsync(1, 2)).ReturnsAsync(false);
        _participacoes.Setup(r => r.TryAddWithinCapacityAsync(It.IsAny<Domain.Entities.Participacao>(), 10))
            .ReturnsAsync(true);

        var result = await _service.InscreverAsync(1, 2);

        result.IsSuccess.Should().BeTrue();
        _participacoes.Verify(r => r.TryAddWithinCapacityAsync(It.IsAny<Domain.Entities.Participacao>(), 10), Times.Once);
    }

    [Fact]
    public async Task Deve_Permitir_Aluno_De_Outra_Instituicao_Em_Evento_Publico_Geral()
    {
        _alunos.Setup(r => r.ListarAlunoById(1)).ReturnsAsync(new AlunoEntity
        {
            Id = 1, Nome = "Aluno Ferraz", Email = "aluno@fatec.sp.gov.br", Senha = "hash",
            FotoPerfil = "foto.png", IsAtivo = true, DataNascimento = new DateTime(2000, 1, 1),
            TipoParticipante = TipoParticipante.Interno, InstituicaoId = 1
        });
        _eventos.Setup(r => r.ListarEventoById(2)).ReturnsAsync(new Domain.Entities.Evento
        {
            Id = 2, Nome = "Feira aberta", Descricao = "Teste", Categoria = Categoria.Feira,
            DataEvento = DateTime.UtcNow.AddDays(1), ResponsavelEventoId = 1, Capacidade = 10,
            Thumbnail = new List<string>(), PublicoPermitido = PublicoPermitido.PublicoGeral,
            Visibilidade = VisibilidadeEvento.Publico, InstituicaoId = 2
        });
        _participacoes.Setup(r => r.CountByEventoIdAsync(2)).ReturnsAsync(0);
        _participacoes.Setup(r => r.ExistsAsync(1, 2)).ReturnsAsync(false);
        _participacoes.Setup(r => r.TryAddWithinCapacityAsync(It.IsAny<Domain.Entities.Participacao>(), 10))
            .ReturnsAsync(true);

        var result = await _service.InscreverAsync(1, 2);

        result.IsSuccess.Should().BeTrue();
        _participacoes.Verify(r => r.TryAddWithinCapacityAsync(It.IsAny<Domain.Entities.Participacao>(), 10), Times.Once);
    }

    [Fact]
    public async Task Publico_Geral_Consegue_Se_Inscrever_Em_Evento_Publico()
    {
        _alunos.Setup(r => r.ListarAlunoById(1)).ReturnsAsync(new AlunoEntity
        {
            Id = 1, Nome = "Visitante", Email = "visitante@example.com", Senha = "hash",
            FotoPerfil = string.Empty, IsAtivo = true, DataNascimento = new DateTime(2000, 1, 1),
            TipoParticipante = TipoParticipante.Externo
        });
        _eventos.Setup(r => r.ListarEventoById(2)).ReturnsAsync(new Domain.Entities.Evento
        {
            Id = 2, Nome = "Feira aberta", Descricao = "Teste", Categoria = Categoria.Feira,
            DataEvento = DateTime.UtcNow.AddDays(1), ResponsavelEventoId = 1, Capacidade = 2,
            Thumbnail = new List<string>(), PublicoPermitido = PublicoPermitido.PublicoGeral,
            Visibilidade = VisibilidadeEvento.Publico
        });
        _participacoes.Setup(r => r.CountByEventoIdAsync(2)).ReturnsAsync(1);
        _participacoes.Setup(r => r.ExistsAsync(1, 2)).ReturnsAsync(false);
        _participacoes.Setup(r => r.TryAddWithinCapacityAsync(It.IsAny<Domain.Entities.Participacao>(), 2))
            .ReturnsAsync(true);

        var result = await _service.InscreverAsync(1, 2);

        result.IsSuccess.Should().BeTrue();
        _participacoes.Verify(r => r.TryAddWithinCapacityAsync(It.IsAny<Domain.Entities.Participacao>(), 2), Times.Once);
    }

    [Fact]
    public async Task Evento_Lotado_Bloqueia_Novas_Inscricoes()
    {
        _alunos.Setup(r => r.ListarAlunoById(1)).ReturnsAsync(new AlunoEntity
        {
            Id = 1, Nome = "Visitante", Email = "visitante@example.com", Senha = "hash",
            FotoPerfil = string.Empty, IsAtivo = true, DataNascimento = new DateTime(2000, 1, 1),
            TipoParticipante = TipoParticipante.Externo
        });
        _eventos.Setup(r => r.ListarEventoById(2)).ReturnsAsync(new Domain.Entities.Evento
        {
            Id = 2, Nome = "Feira lotada", Descricao = "Teste", Categoria = Categoria.Feira,
            DataEvento = DateTime.UtcNow.AddDays(1), ResponsavelEventoId = 1, Capacidade = 1,
            Thumbnail = new List<string>(), PublicoPermitido = PublicoPermitido.PublicoGeral,
            Visibilidade = VisibilidadeEvento.Publico
        });
        _participacoes.Setup(r => r.CountByEventoIdAsync(2)).ReturnsAsync(1);

        var result = await _service.InscreverAsync(1, 2);

        result.IsFailure.Should().BeTrue();
        result.Errors.Should().Contain("Evento lotado");
        _participacoes.Verify(r => r.TryAddWithinCapacityAsync(It.IsAny<Domain.Entities.Participacao>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task Inscricao_Duplicada_E_Bloqueada()
    {
        _alunos.Setup(r => r.ListarAlunoById(1)).ReturnsAsync(new AlunoEntity
        {
            Id = 1, Nome = "Visitante", Email = "visitante@example.com", Senha = "hash",
            FotoPerfil = string.Empty, IsAtivo = true, DataNascimento = new DateTime(2000, 1, 1),
            TipoParticipante = TipoParticipante.Externo
        });
        _eventos.Setup(r => r.ListarEventoById(2)).ReturnsAsync(new Domain.Entities.Evento
        {
            Id = 2, Nome = "Feira aberta", Descricao = "Teste", Categoria = Categoria.Feira,
            DataEvento = DateTime.UtcNow.AddDays(1), ResponsavelEventoId = 1, Capacidade = 10,
            Thumbnail = new List<string>(), PublicoPermitido = PublicoPermitido.PublicoGeral,
            Visibilidade = VisibilidadeEvento.Publico
        });
        _participacoes.Setup(r => r.CountByEventoIdAsync(2)).ReturnsAsync(1);
        _participacoes.Setup(r => r.ExistsAsync(1, 2)).ReturnsAsync(true);

        var result = await _service.InscreverAsync(1, 2);

        result.IsFailure.Should().BeTrue();
        result.Errors.Should().Contain("Aluno já inscrito nesse evento");
        _participacoes.Verify(r => r.TryAddWithinCapacityAsync(It.IsAny<Domain.Entities.Participacao>(), It.IsAny<int>()), Times.Never);
    }
}
