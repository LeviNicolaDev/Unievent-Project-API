
using FluentAssertions;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Unievent.Application.Dtos.Aluno;
using Unievent.Application.Interfaces.Repository;
using Unievent.Application.Interfaces.Services;
using Unievent.Application.Services;
using Xunit;

namespace Unievent.Tests.Application.Aluno;

public class AlunoServiceTest
{
    private readonly Mock<IAlunoRepository> _repositoryMock;
    private readonly Mock<ILogger<AlunoService>> _loggerMock;
    private readonly Mock<IValidator<AlunoRequest>> _validatorRequest;
    private readonly Mock<IValidator<AlunoUpdate>> _validatorUpdate;
    private readonly IAlunoService _alunoService;
    public AlunoServiceTest()
    {
        _repositoryMock = new Mock<IAlunoRepository>();
        _loggerMock = new Mock<ILogger<AlunoService>>();
        _validatorRequest = new Mock<IValidator<AlunoRequest>>();
        _validatorUpdate = new Mock<IValidator<AlunoUpdate>>();
        _alunoService = new AlunoService(_repositoryMock.Object, _loggerMock.Object, _validatorRequest.Object, _validatorUpdate.Object);
    }

    public class CriarAluno : AlunoServiceTest
    {
        [Fact]
        public async Task Deve_Criar_Aluno_Com_Sucesso()
        {
            // Arrange
            using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes("conteudo fake"));
            var request = new AlunoRequest
            {
                Nome = "Ryan",
                Email = "ryan@fatec.sp.gov.br",
                DataNascimento = new DateTime(2000, 1, 1),
                Senha = "senha123",
                FotoPerfil = new FormFile(stream, 0, stream.Length, "FotoPerfil", "foto.jpg")
                {
                    Headers = new HeaderDictionary(),
                    ContentType = "image/jpeg"
                }
            };
            _validatorRequest.Setup(v => v.ValidateAsync(It.IsAny<AlunoRequest>(), default)).ReturnsAsync(new FluentValidation.Results.ValidationResult());
            _repositoryMock.Setup(r => r.ListarAlunoByEmail(It.IsAny<string>())).ReturnsAsync((Unievent.Domain.Entities.Aluno)null);
            _repositoryMock.Setup(r => r.CriarAluno(It.IsAny<Unievent.Domain.Entities.Aluno>())).ReturnsAsync((Unievent.Domain.Entities.Aluno a) => a);
            _repositoryMock.Setup(r => r.SaveChangesAsync()).Returns(Task.FromResult(true));
            // Act
            var result = await _alunoService.CriarAluno(request);
            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Nome.Should().Be(request.Nome);
            result.Value.Email.Should().Be(request.Email);
            result.Value.DataNascimento.Should().Be(request.DataNascimento);
            _repositoryMock.Verify(r => r.CriarAluno(It.IsAny<Unievent.Domain.Entities.Aluno>()), Times.Once);
        }

        [Fact]
        public async Task Deve_Falhar_Criar_Aluno_Quando_Email_Ja_Cadastrado()
        {
            // Arrange
            using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes("conteudo fake"));
            var request = new AlunoRequest
            {
                Nome = "Ryan",
                Email = "ryan@fatec.sp.gov.br",
                DataNascimento = new DateTime(2000, 1, 1),
                Senha = "senha123",
                FotoPerfil = new FormFile(stream, 0, stream.Length, "FotoPerfil", "foto.jpg")
                {
                    Headers = new HeaderDictionary(),
                    ContentType = "image/jpeg"
                }
            };
            _validatorRequest.Setup(v => v.ValidateAsync(It.IsAny<AlunoRequest>(), default)).ReturnsAsync(new FluentValidation.Results.ValidationResult());
            _repositoryMock.Setup(r => r.ListarAlunoByEmail(It.IsAny<string>())).ReturnsAsync(new Unievent.Domain.Entities.Aluno
            {
                Id = 2,
                Nome = "Outro Aluno",
                Email = "ryan@fatec.sp.gov.br",
                DataNascimento = new DateTime(1999, 1, 1),
                Senha = "outrasenha",
                FotoPerfil = "outraFoto.jpg",
                IsAtivo = true

            });
            // Act
            var result = await _alunoService.CriarAluno(request);
            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Contains("Email já cadastrado para outro aluno").Should().Be(true);
            _repositoryMock.Verify(r => r.CriarAluno(It.IsAny<Unievent.Domain.Entities.Aluno>()), Times.Never);
        }

        [Fact]
        public async Task Deve_Falhar_Criar_Aluno_Quando_Validator_Falhar()
        {
            // Arrange
            using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes("conteudo fake"));

            var request = new AlunoRequest
            {
                Nome = "Ryan",
                Email = "email@invalido.com",
                DataNascimento = new DateTime(2000, 1, 1),
                Senha = "senha123",
                FotoPerfil = new FormFile(stream, 0, stream.Length, "FotoPerfil", "foto.jpg")
                {
                    Headers = new HeaderDictionary(),
                    ContentType = "image/jpeg"
                }
            };

            _validatorRequest
                .Setup(v => v.ValidateAsync(It.IsAny<AlunoRequest>(), default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult(new[]
                {
            new FluentValidation.Results.ValidationFailure("Email", "O email deve ser válido")
                }));

            // Act
            var result = await _alunoService.CriarAluno(request);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().Contain("O email deve ser válido");

            _validatorRequest.Verify(
                v => v.ValidateAsync(It.IsAny<AlunoRequest>(), default),
                Times.Once
            );

            _repositoryMock.Verify(
                r => r.CriarAluno(It.IsAny<Domain.Entities.Aluno>()),
                Times.Never
            );
        }
        public class DeletarAluno : AlunoServiceTest
        {
            [Fact]
            public async Task Deve_Deletar_Aluno_Quando_Id_For_Valido()
            {
                // Arrange
                using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes("conteudo fake"));
                var alunoId = 1;
                _repositoryMock.Setup(r => r.ListarAlunoById(alunoId)).ReturnsAsync(new Unievent.Domain.Entities.Aluno
                {
                    Id = alunoId,
                    Nome = "Ryan",
                    Email = "aluno@fatec.sp.gov.br",
                    DataNascimento = new DateTime(2000, 1, 1),
                    Senha = "senha123",
                    FotoPerfil = "foto.jpg",
                    IsAtivo = true

                });
                _repositoryMock.Setup(r => r.SaveChangesAsync()).Returns(Task.FromResult(true));
                // Act
                var result = await _alunoService.DeletarAluno(alunoId);
                // Assert
                result.IsSuccess.Should().BeTrue();
                _repositoryMock.Verify(r => r.ListarAlunoById(alunoId), Times.Once);
                _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
            }

            [Fact]
            public async Task Deve_Falhar_Deletar_Aluno_Quando_Id_For_Invalido()
            {
                // Arrange
                var alunoId = 1;
                _repositoryMock.Setup(r => r.ListarAlunoById(alunoId)).ReturnsAsync((Unievent.Domain.Entities.Aluno)null);

                // Act
                var result = await _alunoService.DeletarAluno(alunoId);
                // Assert
                result.IsSuccess.Should().BeFalse();
                _repositoryMock.Verify(r => r.ListarAlunoById(alunoId), Times.Once);
                _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
                result.Errors.Should().Contain("Aluno não encontrado");
            }
        }
        public class ListarAluno : AlunoServiceTest
        {
            [Fact]
            public async Task Deve_Listar_Aluno_Quando_Id_For_Valido()
            {
                // Arrange
                using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes("conteudo fake"));
                var alunoId = 1;
                _repositoryMock.Setup(r => r.ListarAlunoById(alunoId)).ReturnsAsync(new Unievent.Domain.Entities.Aluno
                {
                    Id = alunoId,
                    Nome = "Ryan",
                    Email = "aluno@fatec.sp.gov.br",
                    DataNascimento = new DateTime(2000, 1, 1),
                    Senha = "senha123",
                    FotoPerfil = "foto.jpg",
                    IsAtivo = true

                });
                // Act
                var result = await _alunoService.ListarAlunoById(alunoId);
                // Assert
                result.IsSuccess.Should().BeTrue();
                _repositoryMock.Verify(r => r.ListarAlunoById(alunoId), Times.Once);
            }

            [Fact]
            public async Task Deve_Falhar_Listar_Aluno_Quando_Id_For_Invalido()
            {
                // Arrange
                using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes("conteudo fake"));
                var alunoId = 1;
                _repositoryMock.Setup(r => r.ListarAlunoById(alunoId)).ReturnsAsync((Unievent.Domain.Entities.Aluno)null);
                // Act
                var result = await _alunoService.ListarAlunoById(alunoId);
                // Assert
                result.IsSuccess.Should().BeFalse();
                _repositoryMock.Verify(r => r.ListarAlunoById(alunoId), Times.Once);
                result.Errors.Should().Contain("Aluno não encontrado");
            }

        }
        public class AtualizarAluno : AlunoServiceTest
        {
            [Fact]
            public async Task Deve_Atualizar_Aluno_Quando_Id_For_Valido()
            {
                // Arrange
                using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes("conteudo fake"));
                var alunoId = 1;
                var update = new AlunoUpdate
                {
                    Nome = "Ryan",
                    DataNascimento = new DateTime(2000, 1, 1),
                    Senha = "senha123",
                    FotoPerfil = new FormFile(stream, 0, stream.Length, "FotoPerfil", "foto.jpg")
                    {
                        Headers = new HeaderDictionary(),
                        ContentType = "image/jpeg"
                    }
                };
                _validatorUpdate.Setup(v => v.ValidateAsync(It.IsAny<AlunoUpdate>(), default)).ReturnsAsync(new FluentValidation.Results.ValidationResult());
                _repositoryMock.Setup(r => r.ListarAlunoById(It.IsAny<int>())).ReturnsAsync(new Unievent.Domain.Entities.Aluno
                {
                    Id = alunoId,
                    Nome = "Ryan",
                    Email = "aluno@fatec.sp.gov.br",
                    DataNascimento = new DateTime(2000, 1, 1),
                    Senha = "senha123",
                    FotoPerfil = "foto.jpg",
                    IsAtivo = true
                });
                _repositoryMock.Setup(r => r.AtualizarAluno(It.IsAny<Unievent.Domain.Entities.Aluno>())).ReturnsAsync((Unievent.Domain.Entities.Aluno a) => a);
                _repositoryMock.Setup(r => r.SaveChangesAsync()).Returns(Task.FromResult(true));
                // Act
                var result = await _alunoService.AtualizarAluno(alunoId, update);
                // Assert
                result.IsSuccess.Should().BeTrue();
                _repositoryMock.Verify(r => r.ListarAlunoById(alunoId), Times.Once);
                _repositoryMock.Verify(r => r.AtualizarAluno(It.IsAny<Unievent.Domain.Entities.Aluno>()), Times.Once);
                _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
            }

            [Fact]
            public async Task Deve_Falhar_Atualizar_Aluno_Quando_Id_For_Invalido()
            {
                // Arrange
                using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes("conteudo fake"));
                var alunoId = 1;
                var update = new AlunoUpdate
                {
                    Nome = "Ryan",
                    DataNascimento = new DateTime(2000, 1, 1),
                    Senha = "senha123",
                    FotoPerfil = new FormFile(stream, 0, stream.Length, "FotoPerfil", "foto.jpg")
                    {
                        Headers = new HeaderDictionary(),
                        ContentType = "image/jpeg"
                    }
                };
                _validatorUpdate.Setup(v => v.ValidateAsync(It.IsAny<AlunoUpdate>(), default)).ReturnsAsync(new FluentValidation.Results.ValidationResult());
                _repositoryMock.Setup(r => r.ListarAlunoById(It.IsAny<int>())).ReturnsAsync((Domain.Entities.Aluno)null);
                // Act
                var result = await _alunoService.AtualizarAluno(alunoId, update);
                // Assert
                result.IsSuccess.Should().BeFalse();
                _repositoryMock.Verify(r => r.ListarAlunoById(alunoId), Times.Once);
                _repositoryMock.Verify(r => r.AtualizarAluno(It.IsAny<Unievent.Domain.Entities.Aluno>()), Times.Never);
                _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
                result.Errors.Should().Contain("Aluno não encontrado");
            }

            [Fact]
            public async Task Deve_Falhar_Atualizar_Aluno_Quando_Validator_Falhar()
            {
                // Arrange
                using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes("conteudo fake"));
                var alunoId = 1;
                var request = new AlunoUpdate
                {
                    Nome = "Ryan",
                    DataNascimento = new DateTime(2000, 1, 1),
                    Senha = "senha123",
                    FotoPerfil = new FormFile(stream, 0, stream.Length, "FotoPerfil", "foto.jpg")
                    {
                        Headers = new HeaderDictionary(),
                        ContentType = "image/jpeg"
                    }
                };

                _validatorUpdate
                    .Setup(v => v.ValidateAsync(It.IsAny<AlunoUpdate>(), default))
                    .ReturnsAsync(new FluentValidation.Results.ValidationResult(new[]
                    {
            new FluentValidation.Results.ValidationFailure("Senha", "A senha deve ter no minimo 6 caracteres")
                    }));

                // Act
                var result = await _alunoService.AtualizarAluno(alunoId, request);

                // Assert
                result.IsSuccess.Should().BeFalse();
                result.Errors.Should().Contain("A senha deve ter no minimo 6 caracteres");

                _validatorUpdate.Verify(
                    v => v.ValidateAsync(It.IsAny<AlunoUpdate>(), default),
                    Times.Once
                );

                _repositoryMock.Verify(
                    r => r.AtualizarAluno(It.IsAny<Domain.Entities.Aluno>()),
                    Times.Never
                );
            }
        }
    }
}
