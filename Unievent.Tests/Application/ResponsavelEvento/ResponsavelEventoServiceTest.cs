using FluentAssertions;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Unievent.Application.Dtos.ResponsavelEvento;
using Unievent.Application.Interfaces.Repository;
using Unievent.Application.Interfaces.Services;
using Unievent.Application.Services;
using Unievent.Domain.Entities;
using Xunit;

namespace Unievent.Tests.Application.ResponsavelEvento;

public class ResponsavelEventoServiceTest
{
    private readonly IResponsavelEventoService _service;
    private readonly Mock<IResponsavelEventoRepository> _repositoryMock;
    private readonly Mock<ILogger<ResponsavelEventoService>> _loggerMock;
    private readonly Mock<IValidator<ResponsavelEventoRequest>> _validatorRequestMock;
    private readonly Mock<IValidator<ResponsavelEventoUpdate>> _validatorUpdateMock;
    public ResponsavelEventoServiceTest()
    {
        _repositoryMock = new Mock<IResponsavelEventoRepository>();
        _loggerMock = new Mock<ILogger<ResponsavelEventoService>>();
        _validatorRequestMock = new Mock<IValidator<ResponsavelEventoRequest>>();
        _validatorUpdateMock = new Mock<IValidator<ResponsavelEventoUpdate>>();
        _service = new ResponsavelEventoService(_repositoryMock.Object, _loggerMock.Object, _validatorRequestMock.Object, _validatorUpdateMock.Object);

    }

    public class CriarResponsavel : ResponsavelEventoServiceTest
    {
        [Fact]
        public async Task Deve_Criar_Responsavel_Evento_Com_Sucesso()
        {
            using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes("conteudo fake"));

            var request = new ResponsavelEventoRequest
            {
                Nome = "ryan",
                FotoPerfil = new FormFile(stream, 0, stream.Length, "FotoPerfil", "foto.jpg")
                {
                    Headers = new HeaderDictionary(),
                    ContentType = "image/jpeg"
                }
            };
            _validatorRequestMock.Setup(v => v.ValidateAsync(It.IsAny<ResponsavelEventoRequest>())).ReturnsAsync(new FluentValidation.Results.ValidationResult());
            _repositoryMock.Setup(r => r.CriarResponsavelEvento(It.IsAny<Domain.Entities.ResponsavelEvento>())).ReturnsAsync((Domain.Entities.ResponsavelEvento e) => e);
            _repositoryMock.Setup(r => r.SaveChangesAsync()).Returns(Task.FromResult(true));

            // Act
            var result = await _service.CriarResponsavelEvento(request);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Nome.Should().Be(request.Nome);
            _repositoryMock.Verify(r => r.CriarResponsavelEvento(It.IsAny<Domain.Entities.ResponsavelEvento>()), Times.Once);
            _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Deve_Falhar_Criar_Responsavel_Quando_Validator_Falhar()
        {
            using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes("conteudo fake"));

            var request = new ResponsavelEventoRequest
            {
                Nome = "",
                FotoPerfil = new FormFile(stream, 0, stream.Length, "FotoPerfil", "foto.jpg")
                {
                    Headers = new HeaderDictionary(),
                    ContentType = "image/jpeg"
                }
            };
            _validatorRequestMock.Setup(v => v.ValidateAsync(It.IsAny<ResponsavelEventoRequest>())).ReturnsAsync(new FluentValidation.Results.ValidationResult(new[]
            {
                new FluentValidation.Results.ValidationFailure("Nome", "Nome é obrigatório")
            }));

            // Act
            var result = await _service.CriarResponsavelEvento(request);

            // Assert
            result.IsSuccess.Should().BeFalse();
            _repositoryMock.Verify(r => r.CriarResponsavelEvento(It.IsAny<Domain.Entities.ResponsavelEvento>()), Times.Never);
            _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }
    }
    public class AtualizarResponsavel : ResponsavelEventoServiceTest
    {
        [Fact]
        public async Task Deve_Atualizar_Responsavel_Com_Sucesso()
        {
            using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes("conteudo fake"));
            var responsavelId = 1;
            var request = new ResponsavelEventoUpdate
            {
                Nome = "ryan",
                FotoPerfil = new FormFile(stream, 0, stream.Length, "FotoPerfil", "foto.jpg")
                {
                    Headers = new HeaderDictionary(),
                    ContentType = "image/jpeg"
                }
            };

            _validatorUpdateMock.Setup(v => v.ValidateAsync(It.IsAny<ResponsavelEventoUpdate>())).ReturnsAsync(new FluentValidation.Results.ValidationResult());
            _repositoryMock.Setup(r => r.ListarResponsavelEventoById(It.IsAny<int>())).ReturnsAsync(new Domain.Entities.ResponsavelEvento
            {
                Id = responsavelId,
                Nome = "Antigo",
                FotoPerfil = "url_antiga.jpg"
            });

            _repositoryMock.Setup(r => r.AtualizarResponsavelEvento(It.IsAny<Domain.Entities.ResponsavelEvento>())).ReturnsAsync((Unievent.Domain.Entities.ResponsavelEvento e) => e);
            _repositoryMock.Setup(r => r.SaveChangesAsync()).Returns(Task.FromResult(true));

            // Act
            var result = await _service.AtualizarResponsavelEvento(1, request);

            // Assert
            result.IsSuccess.Should().BeTrue();
            _repositoryMock.Verify(r => r.ListarResponsavelEventoById(It.IsAny<int>()), Times.Once);
            _repositoryMock.Verify(r => r.AtualizarResponsavelEvento(It.IsAny<Domain.Entities.ResponsavelEvento>()), Times.Once);
            _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Deve_Falhar_Atualizar_Responsavel_Quando_Id_Invalido()
        {
            using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes("conteudo fake"));

            var request = new ResponsavelEventoUpdate
            {
                Nome = "ryan",
                FotoPerfil = new FormFile(stream, 0, stream.Length, "FotoPerfil", "foto.jpg")
                {
                    Headers = new HeaderDictionary(),
                    ContentType = "image/jpeg"
                }
            };
            _validatorUpdateMock.Setup(v => v.ValidateAsync(It.IsAny<ResponsavelEventoUpdate>())).ReturnsAsync(new FluentValidation.Results.ValidationResult());
            _repositoryMock.Setup(r => r.ListarResponsavelEventoById(It.IsAny<int>())).ReturnsAsync((Unievent.Domain.Entities.ResponsavelEvento)null);

            // Act
            var result = await _service.AtualizarResponsavelEvento(1, request);

            // Assert
            result.IsSuccess.Should().BeFalse();
            _repositoryMock.Verify(r => r.ListarResponsavelEventoById(It.IsAny<int>()), Times.Once);
            _repositoryMock.Verify(r => r.AtualizarResponsavelEvento(It.IsAny<Domain.Entities.ResponsavelEvento>()), Times.Never);
            _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task Deve_Falhar_Atualizar_Responsavel_Quando_Validator_Falhar()
        {
            using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes("conteudo fake"));

            var request = new ResponsavelEventoUpdate
            {
                Nome = "",
                FotoPerfil = new FormFile(stream, 0, stream.Length, "FotoPerfil", "foto.jpg")
                {
                    Headers = new HeaderDictionary(),
                    ContentType = "image/jpeg"
                }
            };
            _validatorUpdateMock.Setup(v => v.ValidateAsync(It.IsAny<ResponsavelEventoUpdate>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult(new[]
            {
                new FluentValidation.Results.ValidationFailure("Nome", "Nome é obrigatório")
            }));


            // Act
            var result = await _service.AtualizarResponsavelEvento(1, request);

            // Assert
            result.IsSuccess.Should().BeFalse();
            _repositoryMock.Verify(r => r.ListarResponsavelEventoById(It.IsAny<int>()), Times.Never);
            _repositoryMock.Verify(r => r.AtualizarResponsavelEvento(It.IsAny<Domain.Entities.ResponsavelEvento>()), Times.Never);
            _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }
    }
    public class DeletarResponsavel : ResponsavelEventoServiceTest
    {
        [Fact]
        public async Task Deve_Deletar_Responsavel_Com_Sucesso()
        {
            var responsavelId = 1;
            _repositoryMock.Setup(r => r.ListarResponsavelEventoById(It.IsAny<int>())).ReturnsAsync(new Domain.Entities.ResponsavelEvento
            {
                Id = responsavelId,
                Nome = "João Silva",
                FotoPerfil = "url_foto.jpg"
            });
            _repositoryMock.Setup(r => r.DeletarResponsavelEvento(It.IsAny<Domain.Entities.ResponsavelEvento>())).Returns(Task.FromResult(true));
            _repositoryMock.Setup(r => r.SaveChangesAsync()).Returns(Task.FromResult(true));

            // Act
            var result = await _service.DeletarResponsavelEvento(responsavelId);

            // Assert
            result.IsSuccess.Should().BeTrue();
            _repositoryMock.Verify(r => r.ListarResponsavelEventoById(It.IsAny<int>()), Times.Once);
            _repositoryMock.Verify(r => r.DeletarResponsavelEvento(It.IsAny<Domain.Entities.ResponsavelEvento>()), Times.Once);
            _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);

        }

        [Fact]
        public async Task Deve_Falhar_Deletar_Responsavel_Quando_Id_Invalido()
        {
            _repositoryMock.Setup(r => r.ListarResponsavelEventoById(It.IsAny<int>())).ReturnsAsync((Unievent.Domain.Entities.ResponsavelEvento)null);

            // Act
            var result = await _service.DeletarResponsavelEvento(1);

            // Assert
            result.IsSuccess.Should().BeFalse();
            _repositoryMock.Verify(r => r.ListarResponsavelEventoById(It.IsAny<int>()), Times.Once);
            _repositoryMock.Verify(r => r.DeletarResponsavelEvento(It.IsAny<Domain.Entities.ResponsavelEvento>()), Times.Never);
            _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

    }
    public class ListarResponsavel : ResponsavelEventoServiceTest
    {
        [Fact]
        public async Task Deve_Listar_Responsavel_Com_Sucesso()
        {
            var responsavel = new Domain.Entities.ResponsavelEvento
            {
                Id = 1,
                Nome = "João Silva",
                FotoPerfil = "url_foto.jpg"
            };
            _repositoryMock.Setup(r => r.ListarResponsavelEventoById(It.IsAny<int>())).ReturnsAsync(responsavel);

            // Act
            var result = await _service.ListarResponsavelEventoById(1);

            // Assert
            result.IsSuccess.Should().BeTrue();
            _repositoryMock.Verify(r => r.ListarResponsavelEventoById(It.IsAny<int>()), Times.Once);
        }
        [Fact]
        public async Task Deve_Falhar_Listar_Por_Id_Quando_Id_Invalido()
        {
            var responsavelId = 1;
            _repositoryMock.Setup(r => r.ListarResponsavelEventoById(It.IsAny<int>())).ReturnsAsync((Domain.Entities.ResponsavelEvento)null);

            // Act
            var result = await _service.ListarResponsavelEventoById(responsavelId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            _repositoryMock.Verify(r => r.ListarResponsavelEventoById(It.IsAny<int>()), Times.Once);
            result.Errors.Should().Contain("Responsável do evento não encontrado");
        }
    }
}



