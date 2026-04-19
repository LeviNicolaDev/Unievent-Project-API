

using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Moq;
using Unievent.Application.Dtos.UsuarioSecretaria;
using Unievent.Application.Interfaces.Repository;
using Unievent.Application.Interfaces.Services;
using Unievent.Application.Services;
using Unievent.Domain.Entities;

using Xunit;

namespace Unievent.Tests.Application;

public class UsuarioSecretariaServiceTest
{
    private readonly Mock<IUsuarioSecretariaRepository> _repositoryMock;
    private readonly Mock<ILogger<UsuarioSecretariaService>> _loggerMock;
    private readonly Mock<IValidator<UsuarioSecretariaUpdate>> _updateValidator;
    private readonly Mock<IValidator<UsuarioSecretariaRequest>> _requestValidator;
    private readonly IUsuarioSecretariaService _service;

    public UsuarioSecretariaServiceTest()
    {
        _repositoryMock = new Mock<IUsuarioSecretariaRepository>();
        _loggerMock = new Mock<ILogger<UsuarioSecretariaService>>();
        _updateValidator = new Mock<IValidator<UsuarioSecretariaUpdate>>();
        _requestValidator = new Mock<IValidator<UsuarioSecretariaRequest>>();

        _service = new UsuarioSecretariaService(_repositoryMock.Object, _loggerMock.Object, _requestValidator.Object, _updateValidator.Object);
    }

    public class CriarUsuario : UsuarioSecretariaServiceTest
    {

        [Theory]
        [InlineData("Ryan", "Admin", "ryan@fatec.sp.gov.br", "te", "senha123")]
        [InlineData("Maria", "Secretaria", "maria@fatec.sp.gov.br", "ab", "outraSenha")]
        public async Task Criar_Usuario_Quando_Dados_Validos_Retorna_Sucesso(
        string nome, string role, string email, string chave, string senha)
        {
            // Arrange
            var request = new UsuarioSecretariaRequest
            {
                NomeUsuario = nome,
                RoleUsuario = role,
                EmailUsuario = email,
                Chave = chave,
                Senha = senha
            };
            _requestValidator.Setup(v => v.ValidateAsync(It.IsAny<UsuarioSecretariaRequest>(), default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());

            _repositoryMock.Setup(r => r.ListarUsuarioSecretariaByEmail(It.IsAny<string>()))
                .ReturnsAsync((UsuarioSecretaria)null);

            _repositoryMock
                .Setup(r => r.CriarUsuarioSecretaria(It.IsAny<UsuarioSecretaria>()))
                .ReturnsAsync((UsuarioSecretaria u) => u);

            _repositoryMock.Setup(r => r.SaveChangesAsync()).Returns(Task.FromResult(true));
            // Act
            var result = await _service.CriarUsuarioSecretaria(request);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.NomeUsuario.Should().Be(nome);
            result.Data.RoleUsuario.Should().Be(role);
            result.Data.EmailUsuario.Should().Be(email);
            result.Data.Chave.Should().Be(chave);
            _repositoryMock.Verify(r =>
                r.CriarUsuarioSecretaria(It.IsAny<UsuarioSecretaria>()), Times.Once);
        }
        [Fact]
        public async Task Deve_Falhar_Quando_Validator_Retorna_Erro()
        {
            // Arrange
            var request = new UsuarioSecretariaRequest
            {
                NomeUsuario = "Ryan",
                RoleUsuario = "Admin",
                EmailUsuario = "ryan@fatec.sp.gov.br",
                Chave = "chave",
                Senha = "1265"
            };
            _requestValidator.Setup(v => v.ValidateAsync(It.IsAny<UsuarioSecretariaRequest>(), default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult(new[] { new FluentValidation.Results.ValidationFailure
            ("Senha", "A senha deve ter no minimo 6 caracteres") }));


            // Act
            var result = await _service.CriarUsuarioSecretaria(request);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Message.Should().Be("A senha deve ter no minimo 6 caracteres");
            _repositoryMock.Verify(r =>
                r.CriarUsuarioSecretaria(It.IsAny<UsuarioSecretaria>()), Times.Never);
        }
        [Fact]
        public async Task Deve_Falhar_Quando_Email_Ja_Existe()
        {
            //Arrange
            var request = new UsuarioSecretariaRequest
            {
                NomeUsuario = "Ryan",
                RoleUsuario = "Admin",
                EmailUsuario = "ryan@fatec.sp.gov.br",
                Senha = "123456",
                Chave = "abc"
            };
            _requestValidator.Setup(v => v.ValidateAsync(It.IsAny<UsuarioSecretariaRequest>(), default))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

            _repositoryMock.Setup(r => r.ListarUsuarioSecretariaByEmail(It.IsAny<string>()))
            .ReturnsAsync(new UsuarioSecretaria
            {
                NomeUsuario = "QualquerNome",
                EmailUsuario = "ryan@fatec.sp.gov.br",
                Senha = "hash",
                RoleUsuario = Domain.Enuns.Role.Admin,
                Chave = "abc",
                IsAtivo = true
            });


            //Act
            var result = await _service.CriarUsuarioSecretaria(request);
            //Assert
            result.IsFailure.Should().BeTrue();
            result.Message.Should().Be("Email já cadastrado para outro usuário da secretaria");
            _repositoryMock.Verify(r => r.CriarUsuarioSecretaria(It.IsAny<UsuarioSecretaria>()), Times.Never);

        }

        public class DeletarUsuario : UsuarioSecretariaServiceTest
        {
            [Fact]
            public async Task Deve_Deletar_Usuario_Quando_Id_Valido()
            {
                // Arrange
                var usuario = new UsuarioSecretaria
                {
                    Id = 1,
                    NomeUsuario = "Ryan",
                    RoleUsuario = Domain.Enuns.Role.Admin,
                    EmailUsuario = "ryan@fatec.sp.gov.br",
                    Senha = "senhavalida",
                    Chave = "abc",
                    IsAtivo = true,
                    TentativasLogin = 0

                };

                _repositoryMock.Setup(r => r.ListarUsuarioSecretariaById(It.IsAny<int>())).ReturnsAsync(usuario);

                _repositoryMock.Setup(r => r.SaveChangesAsync()).Returns(Task.FromResult(true));

                //Act
                var result = await _service.DeletarUsuarioSecretaria(1);
                //Assert
                result.IsSuccess.Should().BeTrue();
                _repositoryMock.Verify(r => r.DeletarUsuarioSecretaria(It.IsAny<UsuarioSecretaria>()), Times.Never);
                result.Message.Should().Be("UsuarioSecretaria deletado com sucesso");
            }
        }

        [Fact]
        public async Task Deve_Falhar_Deletar_Usuario_Quando_Id_Invalido()
        {
            // Arrange
            var usuario = new UsuarioSecretaria
            {
                Id = 1,
                NomeUsuario = "Ryan",
                RoleUsuario = Domain.Enuns.Role.Admin,
                EmailUsuario = "ryan@fatec.sp.gov.br",
                Senha = "senhavalida",
                Chave = "abc",
                IsAtivo = true,
                TentativasLogin = 0

            };

            _repositoryMock.Setup(r => r.ListarUsuarioSecretariaById(It.IsAny<int>())).ReturnsAsync((UsuarioSecretaria)null);


            //Act
            var result = await _service.DeletarUsuarioSecretaria(1);
            //Assert
            result.IsSuccess.Should().BeFalse();
            _repositoryMock.Verify(r => r.DeletarUsuarioSecretaria(It.IsAny<UsuarioSecretaria>()), Times.Never);
            result.Message.Should().Be("UsuarioSecretaria não encontrado");
        }

        public class AtualizarUsuario : UsuarioSecretariaServiceTest
        {
            [Fact]
            public async Task Deve_Atualizar_Usuario_Quando_Dados_Validos()
            {
                //Arrange
                var usuarioExistente = new UsuarioSecretaria
                {
                    Id = 1,
                    NomeUsuario = "Ryan",
                    RoleUsuario = Domain.Enuns.Role.Admin,
                    EmailUsuario = "ryan@fatec.sp.gov.br",
                    Senha = "senhavalida",
                    Chave = "abc",
                    IsAtivo = true,
                    TentativasLogin = 0
                };
                _updateValidator.Setup(v => v.ValidateAsync(It.IsAny<UsuarioSecretariaUpdate>(), default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());
                _repositoryMock.Setup(r => r.ListarUsuarioSecretariaById(It.IsAny<int>())).ReturnsAsync(usuarioExistente);
                _repositoryMock.Setup(r => r.ListarUsuarioSecretariaByEmail(It.IsAny<string>())).ReturnsAsync((UsuarioSecretaria)null);
                _repositoryMock.Setup(r => r.AtualizarUsuarioSecretaria(It.IsAny<UsuarioSecretaria>())).ReturnsAsync((UsuarioSecretaria u) => u);
                _repositoryMock.Setup(r => r.SaveChangesAsync()).Returns(Task.FromResult(true));
                var request = new UsuarioSecretariaUpdate
                {
                    NomeUsuario = "Ryan Updated",
                    Role = "Secretaria",
                    EmailUsuario = "teste@fatec.sp.gov.br",
                    Senha = "novasenha"
                };
                //Act
                var result = await _service.AtualizarUsuarioSecretaria(1, request);
                //Assert
                result.IsSuccess.Should().BeTrue();
                result.Data.NomeUsuario.Should().Be(request.NomeUsuario);
                result.Data.RoleUsuario.Should().Be(request.Role);
                result.Data.EmailUsuario.Should().Be(request.EmailUsuario);
                result.Data.Chave.Should().Be(usuarioExistente.Chave);
                _repositoryMock.Verify(r => r.AtualizarUsuarioSecretaria(It.IsAny<UsuarioSecretaria>()), Times.Once);


            }

            [Fact]
            public async Task Deve_Lancar_Erro_Quando_Email_Update_Ja_Estiver_Cadastrado()
            {
                //Arrange
                var usuarioExistente = new UsuarioSecretaria
                {
                    Id = 1,
                    NomeUsuario = "Ryan",
                    RoleUsuario = Domain.Enuns.Role.Admin,
                    EmailUsuario = "ryan@fatec.sp.gov.br",
                    Senha = "senhavalida",
                    Chave = "abc",
                    IsAtivo = true,
                    TentativasLogin = 0
                };
                _updateValidator.Setup(v => v.ValidateAsync(It.IsAny<UsuarioSecretariaUpdate>(), default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());
                _repositoryMock.Setup(r => r.ListarUsuarioSecretariaById(It.IsAny<int>())).ReturnsAsync(usuarioExistente);
                _repositoryMock.Setup(r => r.ListarUsuarioSecretariaByEmail(It.IsAny<string>())).ReturnsAsync(new UsuarioSecretaria
                {
                    Id = 2,
                    NomeUsuario = "Outro Usuario",
                    RoleUsuario = Domain.Enuns.Role.Secretaria,
                    EmailUsuario = "emailexiste@fatec.sp.gov.br",
                    Chave = "def",
                    Senha = "outrasenha",
                    IsAtivo = true,
                    TentativasLogin = 0
                });
                var request = new UsuarioSecretariaUpdate
                {
                    NomeUsuario = "Ryan Updated",
                    Role = "Secretaria",
                    EmailUsuario = "teste@fatec.sp.gov.br",
                    Senha = "novasenha"
                };
                //Act
                var result = await _service.AtualizarUsuarioSecretaria(1, request);
                //Assert
                result.IsSuccess.Should().BeFalse();
                result.Message.Should().Be("Email já cadastrado para outro usuário da secretaria");
                _repositoryMock.Verify(r => r.AtualizarUsuarioSecretaria(It.IsAny<UsuarioSecretaria>()), Times.Never);


            }

            [Fact]
            public async Task Deve_Lancar_Erro_Quando_Validator_Falhar()
            {
                //Arrange
                var usuarioExistente = new UsuarioSecretaria
                {
                    Id = 1,
                    NomeUsuario = "Ryan",
                    RoleUsuario = Domain.Enuns.Role.Admin,
                    EmailUsuario = "ryan@fatec.sp.gov.br",
                    Senha = "senhavalida",
                    Chave = "abc",
                    IsAtivo = true,
                    TentativasLogin = 0
                };

                var request = new UsuarioSecretariaUpdate
                {
                    NomeUsuario = "Ryan Updated",
                    Role = "Secretaria",
                    EmailUsuario = "teste@fatec.sp.gov.br",
                    Senha = "saa"
                };
                _updateValidator.Setup(v => v.ValidateAsync(It.IsAny<UsuarioSecretariaUpdate>(), default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult(new[]{
                new FluentValidation.Results.ValidationFailure("Senha", "Senha deve ter no minimo 6 caracteres")
                    }));
                //Act
                var result = await _service.AtualizarUsuarioSecretaria(1, request);
                //Assert
                result.IsSuccess.Should().BeFalse();

                _repositoryMock.Verify(r => r.AtualizarUsuarioSecretaria(It.IsAny<UsuarioSecretaria>()), Times.Never);


            }
        }

        public class ListarUsuario : UsuarioSecretariaServiceTest
        {
            [Fact]
            public async Task Deve_Listar_Usuario_Por_Id()
            {
                //Arrange
                var usuario = new UsuarioSecretaria
                {
                    Id = 1,
                    NomeUsuario = "Ryan",
                    RoleUsuario = Domain.Enuns.Role.Admin,
                    EmailUsuario = "ryan@fatec.sp.gov.br",
                    Chave = "abc",
                    Senha = "senhavalida",
                    IsAtivo = true,
                    TentativasLogin = 0
                };

                _repositoryMock.Setup(r => r.ListarUsuarioSecretariaById(It.IsAny<int>())).ReturnsAsync(new UsuarioSecretaria
                {
                    Id = 1,
                    NomeUsuario = "Ryan",
                    RoleUsuario = Domain.Enuns.Role.Admin,
                    EmailUsuario = "ryan@fatec.sp.gov.br",
                    Chave = "abc",
                    Senha = "senhavalida",
                    IsAtivo = true,
                    TentativasLogin = 0

                });
                //Act
                var result = await _service.ListarUsuarioSecretariaById(1);
                //Assert
                result.IsSuccess.Should().BeTrue();
                result.Data.Id.Should().Be(1);

            }

            [Fact]
            public async Task Deve_Lancar_Erro_Ao_Listar_Usuario_Por_Id()
            {
                //Arrange
                var usuario = new UsuarioSecretaria
                {
                    Id = 1,
                    NomeUsuario = "Ryan",
                    RoleUsuario = Domain.Enuns.Role.Admin,
                    EmailUsuario = "ryan@fatec.sp.gov.br",
                    Chave = "abc",
                    Senha = "senhavalida",
                    IsAtivo = true,
                    TentativasLogin = 0
                };

                _repositoryMock.Setup(r => r.ListarUsuarioSecretariaById(It.IsAny<int>())).ReturnsAsync((UsuarioSecretaria)null);
                //Act
                var result = await _service.ListarUsuarioSecretariaById(1);
                //Assert
                result.IsSuccess.Should().BeFalse();
                result.Message.Should().Be("UsuarioSecretaria não encontrado");

            }
        }
    }
}
