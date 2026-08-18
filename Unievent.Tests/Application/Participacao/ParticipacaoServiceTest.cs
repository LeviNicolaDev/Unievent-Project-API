using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Unievent.Application.Dtos.Participacao;
using Unievent.Application.Interfaces.Repository;
using Unievent.Application.Interfaces.Services;
using Unievent.Application.Services;
using Unievent.Domain.Entities;
using Unievent.Domain.Enuns;
using Xunit;

namespace Unievent.Tests.Application.Participacao;

public class ParticipacaoServiceTest
{
    private readonly Mock<IEventoRepository> _eventos = new();
    private readonly Mock<IParticipacaoRepository> _participacoes = new();
    private readonly Mock<IAlunoRepository> _alunos = new();
    private readonly IParticipacaoService _service;

    public ParticipacaoServiceTest()
    {
        _service = new ParticipacaoService(
            _eventos.Object,
            _participacoes.Object,
            _alunos.Object,
            Mock.Of<ILogger<ParticipacaoService>>());
    }

    [Fact]
    public async Task Deve_Rejeitar_CheckIn_Com_Localizacao_Imprecisa()
    {
        var result = await _service.ValidarCheckInAsync(10, new CheckInRequest
        {
            CodigoIngresso = "codigo",
            Latitude = -23.5,
            Longitude = -46.4,
            PrecisaoMetros = 100
        });

        result.IsFailure.Should().BeTrue();
        _participacoes.Verify(r => r.GetByCodigoIngressoAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Deve_Confirmar_CheckIn_Valido_Dentro_Do_Raio()
    {
        var participacao = new Domain.Entities.Participacao(1, 2)
        {
            Aluno = new Aluno
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
        _participacoes.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Deve_Bloquear_Publico_Externo_Em_Evento_Interno()
    {
        _alunos.Setup(r => r.ListarAlunoById(1)).ReturnsAsync(new Aluno
        {
            Id = 1, Nome = "Externo", Email = "externo@example.com", Senha = "hash",
            FotoPerfil = "foto.png", IsAtivo = true, DataNascimento = new DateTime(2000, 1, 1),
            TipoParticipante = TipoParticipante.Externo
        });
        _eventos.Setup(r => r.ListarEventoById(2)).ReturnsAsync(new Domain.Entities.Evento
        {
            Id = 2, Nome = "Interno", Descricao = "Teste", Categoria = Categoria.Palestra,
            DataEvento = DateTime.UtcNow.AddDays(1), ResponsavelEventoId = 1, Capacidade = 10,
            Thumbnail = new List<string>(), PublicoPermitido = PublicoPermitido.SomenteInternos
        });

        var result = await _service.InscreverAsync(1, 2);

        result.IsFailure.Should().BeTrue();
        _participacoes.Verify(r => r.TryAddWithinCapacityAsync(It.IsAny<Domain.Entities.Participacao>(), It.IsAny<int>()), Times.Never);
    }
}
