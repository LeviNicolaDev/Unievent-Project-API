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
using Unievent.Application.Interfaces.Services;
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
    private readonly Mock<IEmailService> _email = new();
    private readonly AuthService _service;

    public AuthServiceTest()
    {
        _validator
            .Setup(v => v.ValidateAsync(It.IsAny<PublicoGeralCadastroRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _email.Setup(e => e.EnviarEmailConfirmacaoConta(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(Result<bool>.Success(true));
        _service = new AuthService(
            _admins.Object,
            _secretarias.Object,
            _tokens.Object,
            _alunos.Object,
            Mock.Of<ILogger<AuthService>>(),
            _validator.Object, _email.Object);
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
        alunoCriado.EmailConfirmado.Should().BeFalse();
        alunoCriado.ChaveConfirmacaoEmail.Should().NotBeNullOrWhiteSpace();
        _email.Verify(e => e.EnviarEmailConfirmacaoConta(alunoCriado.Email, alunoCriado.Nome, alunoCriado.ChaveConfirmacaoEmail!), Times.Once);
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

    [Theory]
    [InlineData(false, false)]
    [InlineData(true, true)]
    public async Task Admin_Exige_Email_Confirmado(bool confirmado, bool sucesso)
    {
        _admins.Setup(r => r.ListarUsuarioUnieventByEmail("admin@example.com"))
            .ReturnsAsync(new Domain.Entities.UsuarioUnievent {
                Id = 1, NomeUsuario = "Admin", EmailUsuario = "admin@example.com",
                Senha = BCrypt.Net.BCrypt.HashPassword("senha123"), EmailConfirmado = confirmado,
                IsAtivo = true, RoleUsuario = Role.Admin
            });
        _tokens.Setup(t => t.GerarToken(1, "admin@example.com", Role.Admin)).Returns("jwt-admin");
        var result = await _service.Login(new Unievent.Application.Dtos.UsuarioSecretaria.UsuarioSecretariaLoginRequest {
            Email = "admin@example.com", Senha = "senha123"
        });
        result.IsSuccess.Should().Be(sucesso);
        if (!confirmado) result.ErrorCode.Should().Be("EMAIL_CONFIRMATION_SENT");
        _email.Verify(e => e.EnviarEmailConfirmacaoConta(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), confirmado ? Times.Never() : Times.Once());
        _tokens.Verify(t => t.GerarToken(1, "admin@example.com", Role.Admin), confirmado ? Times.Once() : Times.Never());
    }

    [Fact]
    public async Task Publico_Pendente_Nao_Recebe_Jwt()
    {
        _alunos.Setup(r => r.ListarAlunoByEmail("publico@example.com"))
            .ReturnsAsync(new Domain.Entities.Aluno {
                Nome = "Publico", Email = "publico@example.com", FotoPerfil = "",
                Senha = BCrypt.Net.BCrypt.HashPassword("senha123"),
                EmailConfirmado = false, DataNascimento = new DateTime(2000, 1, 1), TipoParticipante = TipoParticipante.Externo
            });
        var result = await _service.LoginPublicoGeral(new LoginRequest { Email = "publico@example.com", Senha = "senha123" });
        result.IsFailure.Should().BeTrue();
        _tokens.Verify(t => t.GerarToken(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Role>(), It.IsAny<TipoParticipante>(), It.IsAny<int?>()), Times.Never);
    }

    [Fact]
    public async Task Falha_De_Email_Mantem_Cadastro_Publico_Salvo()
    {
        _email.Setup(e => e.EnviarEmailConfirmacaoConta(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(Result<bool>.Failure("SMTP indisponível"));
        var result = await _service.CadastrarPublicoGeral(new PublicoGeralCadastroRequest {
            Nome = "Publico", Email = "publico@example.com", Senha = "senha123", ConfirmacaoSenha = "senha123"
        });
        result.IsFailure.Should().BeTrue();
        _alunos.Verify(r => r.CriarAluno(It.IsAny<Domain.Entities.Aluno>()), Times.Once);
        _alunos.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Credencial_Incorreta_Nao_Reenvia_Confirmacao(bool admin)
    {
        if (admin)
            _admins.Setup(r => r.ListarUsuarioUnieventByEmail("pessoa@example.com")).ReturnsAsync(new Domain.Entities.UsuarioUnievent {
                NomeUsuario = "Pessoa", EmailUsuario = "pessoa@example.com", Senha = BCrypt.Net.BCrypt.HashPassword("correta"), EmailConfirmado = false
            });
        else
            _alunos.Setup(r => r.ListarAlunoByEmail("pessoa@example.com")).ReturnsAsync(new Domain.Entities.Aluno {
                Nome = "Pessoa", Email = "pessoa@example.com", FotoPerfil = "", DataNascimento = new DateTime(2000, 1, 1),
                Senha = BCrypt.Net.BCrypt.HashPassword("correta"), EmailConfirmado = false, TipoParticipante = TipoParticipante.Externo
            });
        var result = admin
            ? await _service.Login(new Unievent.Application.Dtos.UsuarioSecretaria.UsuarioSecretariaLoginRequest { Email = "pessoa@example.com", Senha = "errada" })
            : await _service.LoginPublicoGeral(new LoginRequest { Email = "pessoa@example.com", Senha = "errada" });
        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().BeNull();
        _email.Verify(e => e.EnviarEmailConfirmacaoConta(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Login_Pendente_Com_Falha_Smtp_Nao_Afirma_Envio()
    {
        _alunos.Setup(r => r.ListarAlunoByEmail("pessoa@example.com")).ReturnsAsync(new Domain.Entities.Aluno {
            Nome = "Pessoa", Email = "pessoa@example.com", FotoPerfil = "", DataNascimento = new DateTime(2000, 1, 1),
            Senha = BCrypt.Net.BCrypt.HashPassword("correta"), EmailConfirmado = false, TipoParticipante = TipoParticipante.Externo
        });
        _email.Setup(e => e.EnviarEmailConfirmacaoConta(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(Result<bool>.Failure("SMTP indisponível"));
        var result = await _service.LoginPublicoGeral(new LoginRequest { Email = "pessoa@example.com", Senha = "correta" });
        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().BeNull();
        result.Errors.Should().Contain("Não foi possível enviar a confirmação de e-mail. Tente novamente.");
        _alunos.Verify(r => r.SaveChangesAsync(), Times.Never);
    }
}
