using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using Unievent.Application.Common;
using Unievent.Application.Dtos.Aluno;
using Unievent.Application.Dtos.Auth;
using Unievent.Application.Interfaces.Auth;
using Unievent.Application.Interfaces.Repository;
using Unievent.Application.Services;
using Unievent.Domain.Enuns;
using Xunit;

namespace Unievent.Tests.Application.Auth;

public class AuthServiceTest
{
    private readonly Mock<IUsuarioUnieventRepository> _admins = new();
    private readonly Mock<IUsuarioSecretariaRepository> _secretarias = new();
    private readonly Mock<IAlunoRepository> _alunos = new();
    private readonly Mock<IJwtTokenGenerator> _tokens = new();
    private readonly Mock<IValidator<PublicoGeralCadastroRequest>> _validator = new();
    private readonly AuthService _service;

    public AuthServiceTest()
    {
        _validator
            .Setup(v => v.ValidateAsync(It.IsAny<PublicoGeralCadastroRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _service = new AuthService(
            _admins.Object,
            _secretarias.Object,
            _tokens.Object,
            _alunos.Object,
            Mock.Of<ILogger<AuthService>>(),
            _validator.Object);
    }

    [Fact]
    public async Task Publico_Geral_Consegue_Criar_Conta()
    {
        Domain.Entities.Aluno? alunoCriado = null;
        _alunos.Setup(r => r.ListarAlunoByEmail("ana@example.com")).ReturnsAsync((Domain.Entities.Aluno)null);
        _secretarias.Setup(r => r.ListarUsuarioSecretariaByEmail("ana@example.com")).ReturnsAsync((Domain.Entities.UsuarioSecretaria)null);
        _admins.Setup(r => r.ListarUsuarioUnieventByEmail("ana@example.com")).ReturnsAsync((Domain.Entities.UsuarioUnievent?)null);
        _alunos.Setup(r => r.CriarAluno(It.IsAny<Domain.Entities.Aluno>()))
            .Callback<Domain.Entities.Aluno>(a => alunoCriado = a)
            .ReturnsAsync((Domain.Entities.Aluno a) => a);

        var result = await _service.CadastrarPublicoGeral(new PublicoGeralCadastroRequest
        {
            Nome = "Ana Publica",
            Email = "ana@example.com",
            Senha = "senha123",
            ConfirmacaoSenha = "senha123"
        });

        result.IsSuccess.Should().BeTrue();
        alunoCriado.Should().NotBeNull();
        alunoCriado!.TipoParticipante.Should().Be(TipoParticipante.Externo);
        alunoCriado.InstituicaoId.Should().BeNull();
        alunoCriado.EmailConfirmado.Should().BeTrue();
        _alunos.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Publico_Geral_Consegue_Fazer_Login()
    {
        var senha = BCrypt.Net.BCrypt.HashPassword("senha123");
        _alunos.Setup(r => r.ListarAlunoByEmail("ana@example.com")).ReturnsAsync(new Domain.Entities.Aluno
        {
            Id = 10,
            Nome = "Ana Publica",
            Email = "ana@example.com",
            Senha = senha,
            FotoPerfil = string.Empty,
            IsAtivo = true,
            EmailConfirmado = true,
            DataNascimento = DateTime.UtcNow.Date,
            Role = Role.Aluno,
            TipoParticipante = TipoParticipante.Externo
        });
        _tokens.Setup(t => t.GerarToken(10, "ana@example.com", Role.Aluno, TipoParticipante.Externo, null))
            .Returns("token-publico");

        var result = await _service.LoginPublicoGeral(new LoginRequest
        {
            Email = "ana@example.com",
            Senha = "senha123"
        });

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("token-publico");
    }

    [Fact]
    public async Task Publico_Geral_Nao_Usa_Login_Publico_Com_Conta_Interna()
    {
        var senha = BCrypt.Net.BCrypt.HashPassword("senha123");
        _alunos.Setup(r => r.ListarAlunoByEmail("aluno@fatec.sp.gov.br")).ReturnsAsync(new Domain.Entities.Aluno
        {
            Id = 11,
            Nome = "Aluno Fatec",
            Email = "aluno@fatec.sp.gov.br",
            Senha = senha,
            FotoPerfil = "foto.png",
            IsAtivo = true,
            EmailConfirmado = true,
            DataNascimento = DateTime.UtcNow.Date,
            Role = Role.Aluno,
            TipoParticipante = TipoParticipante.Interno,
            InstituicaoId = 1
        });

        var result = await _service.LoginPublicoGeral(new LoginRequest
        {
            Email = "aluno@fatec.sp.gov.br",
            Senha = "senha123"
        });

        result.IsFailure.Should().BeTrue();
        _tokens.Verify(t => t.GerarToken(
            It.IsAny<int>(),
            It.IsAny<string>(),
            It.IsAny<Role>(),
            It.IsAny<TipoParticipante>(),
            It.IsAny<int?>()), Times.Never);
    }
}
