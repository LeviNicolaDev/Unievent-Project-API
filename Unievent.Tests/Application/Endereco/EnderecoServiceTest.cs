using System.Net.Http.Headers;
using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Moq;
using Unievent.Application.Dtos.Endereco;
using Unievent.Application.Interfaces.Repository;
using Unievent.Application.Interfaces.Services;
using Unievent.Application.Services;
using Xunit;

namespace Unievent.Tests.Application.Endereco;

public class EnderecoServiceTest
{
    private readonly Mock<IEnderecoRepository> _repositoryMock;
    private readonly Mock<ILogger<EnderecoService>> _loggerMock;
    private readonly Mock<IValidator<EnderecoRequest>> _requestValidator;
    private readonly Mock<IValidator<EnderecoUpdate>> _updateValidator;
    private readonly IEnderecoService _service;
    public EnderecoServiceTest()
    {
        _repositoryMock = new Mock<IEnderecoRepository>();
        _loggerMock = new Mock<ILogger<EnderecoService>>();
        _requestValidator = new Mock<IValidator<EnderecoRequest>>();
        _updateValidator = new Mock<IValidator<EnderecoUpdate>>();
        _service = new EnderecoService(_repositoryMock.Object, _loggerMock.Object, _requestValidator.Object, _updateValidator.Object);
    }

    public class CriarEndereco : EnderecoServiceTest
    {
        [Fact]
        public async Task Deve_Criar_Endereco_Com_Sucesso()
        {
            // Arrange
            var request = new EnderecoRequest
            {
                Rua = "Rua Exemplo",
                Numero = "123",
                Bairro = "Bairro Exemplo",
                Cidade = "Cidade Exemplo",
                Estado = "Estado Exemplo",
                Cep = "1234567"
            };
            _requestValidator.Setup(v => v.ValidateAsync(request, default)).ReturnsAsync(new FluentValidation.Results.ValidationResult());
            _repositoryMock.Setup(r => r.CriarEndereco(It.IsAny<Domain.Entities.Endereco>())).ReturnsAsync((Domain.Entities.Endereco e) => e);
            _repositoryMock.Setup(r => r.SaveChangesAsync()).Returns(Task.FromResult(true));
            // Act
            var result = await _service.CriarEndereco(request);
            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Rua.Should().Be(request.Rua);
            _repositoryMock.Verify(r => r.CriarEndereco(It.IsAny<Domain.Entities.Endereco>()), Times.Once);
        }
        [Fact]
        public async Task Deve_Falhar_Criar_Endereco_Quando_Validator_Falhar()
        {
            // Arrange
            var request = new EnderecoRequest
            {
                Rua = "Rua Exemplo",
                Numero = "123",
                Bairro = "Bairro Exemplo",
                Cidade = "Cidade Exemplo",
                Estado = "Estado Exemplo",
                Cep = "12345678"
            };
            _requestValidator.Setup(v => v.ValidateAsync(request, default)).ReturnsAsync(new FluentValidation.Results.ValidationResult(new[]
            {
                new FluentValidation.Results.ValidationFailure("Rua", "A rua é obrigatória.")
            }));
            // Act
            var result = await _service.CriarEndereco(request);
            // Assert
            result.IsSuccess.Should().BeFalse();
            _repositoryMock.Verify(r => r.CriarEndereco(It.IsAny<Domain.Entities.Endereco>()), Times.Never);
        }

    }
    public class AtualizarEndereco : EnderecoServiceTest
    {
        [Fact]
        public async Task Deve_Atualizar_Endereco_Com_Sucesso()
        {
            // Arrange
            var enderecoExistente = new Domain.Entities.Endereco
            {
                Id = 1,
                Rua = "Rua Antiga",
                Numero = "123",
                Bairro = "Bairro Antigo",
                Cidade = "Cidade Antiga",
                Estado = "Estado Antigo",
                Cep = "1234567"
            };
            var request = new EnderecoUpdate
            {
                Rua = "Rua Nova",
                Numero = "456",
                Bairro = "Bairro Novo",
                Cidade = "Cidade Nova",
                Estado = "Estado Novo",
                Cep = "7654321"
            };
            _updateValidator.Setup(v => v.ValidateAsync(request, default)).ReturnsAsync(new FluentValidation.Results.ValidationResult());
            _repositoryMock.Setup(r => r.ListarEnderecoById(enderecoExistente.Id)).ReturnsAsync(enderecoExistente);
            _repositoryMock.Setup(r => r.AtualizarEndereco(It.IsAny<Domain.Entities.Endereco>())).ReturnsAsync((Domain.Entities.Endereco e) => e);
            _repositoryMock.Setup(r => r.SaveChangesAsync()).Returns(Task.FromResult(true));
            // Act
            var result = await _service.AtualizarEndereco(enderecoExistente.Id, request);
            // Assert
            result.IsSuccess.Should().BeTrue();
            enderecoExistente.Rua.Should().Be(request.Rua);
            enderecoExistente.Numero.Should().Be(request.Numero);
            enderecoExistente.Bairro.Should().Be(request.Bairro);
            enderecoExistente.Cidade.Should().Be(request.Cidade);
            enderecoExistente.Estado.Should().Be(request.Estado);
            enderecoExistente.Cep.Should().Be(request.Cep);
            _repositoryMock.Verify(r => r.AtualizarEndereco(It.IsAny<Domain.Entities.Endereco>()), Times.Once);
        }

        [Fact]
        public async Task Deve_Falhar_Atualizar_Endereco_Quando_Id_Endereco_Nao_Existir()
        {
            // Arrange
            var enderecoExistente = new Domain.Entities.Endereco
            {
                Id = 1,
                Rua = "Rua Antiga",
                Numero = "123",
                Bairro = "Bairro Antigo",
                Cidade = "Cidade Antiga",
                Estado = "Estado Antigo",
                Cep = "1234567"
            };
            var request = new EnderecoUpdate
            {
                Rua = "Rua Nova",
                Numero = "456",
                Bairro = "Bairro Novo",
                Cidade = "Cidade Nova",
                Estado = "Estado Novo",
                Cep = "76543218"
            };
            _updateValidator.Setup(v => v.ValidateAsync(It.IsAny<EnderecoUpdate>(), default)).ReturnsAsync(new FluentValidation.Results.ValidationResult());
            _repositoryMock.Setup(r => r.ListarEnderecoById(It.IsAny<int>())).ReturnsAsync((Domain.Entities.Endereco)null);

            //Act
            var result = await _service.AtualizarEndereco(1, request);
            //Assert

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain("Endereco não encontrado");
            _repositoryMock.Verify(r => r.AtualizarEndereco(It.IsAny<Domain.Entities.Endereco>()), Times.Never);
        }

        [Fact]
        public async Task Deve_Falhar_Atualizar_Endereco_Quando_Validator_Falhar()
        {
            // Arrange
            var enderecoExistente = new Domain.Entities.Endereco
            {
                Id = 1,
                Rua = "Rua Antiga",
                Numero = "123",
                Bairro = "Bairro Antigo",
                Cidade = "Cidade Antiga",
                Estado = "Estado Antigo",
                Cep = "1234567"
            };
            var request = new EnderecoUpdate
            {
                Rua = "Rua Nova",
                Numero = "456",
                Bairro = "Bairro Novo",
                Cidade = "Cidade Nova",
                Estado = "Estado Novo",
                Cep = "7654321"
            };
            _requestValidator.Setup(v => v.ValidateAsync(It.IsAny<EnderecoRequest>())).ReturnsAsync(new FluentValidation.Results.ValidationResult(new[]
            {
                new FluentValidation.Results.ValidationFailure("Dados", "Dados Inválidos")
            }));


            //Act
            var result = await _service.AtualizarEndereco(enderecoExistente.Id, request);
            //Assert

            result.IsFailure.Should().BeTrue();
            _repositoryMock.Verify(r => r.AtualizarEndereco(It.IsAny<Domain.Entities.Endereco>()), Times.Never);
        }
    }
    public class DeletarEndereco : EnderecoServiceTest
    {
        [Fact]
        public async Task Deve_Deletar_Com_Sucesso()
        {
            //Arrange
            _repositoryMock.Setup(r => r.ListarEnderecoById(1)).ReturnsAsync(new Domain.Entities.Endereco
            {
                Id = 1,
                Rua = "Rua Antiga",
                Numero = "123",
                Bairro = "Bairro Antigo",
                Cidade = "Cidade Antiga",
                Estado = "Estado Antigo",
                Cep = "1234567"
            });
            _repositoryMock.Setup(r => r.DeletarEndereco(It.IsAny<Domain.Entities.Endereco>())).ReturnsAsync(true);
            _repositoryMock.Setup(r => r.SaveChangesAsync()).Returns(Task.FromResult(true));
            //Act
            var result = await _service.DeletarEndereco(1);
            //Assert 
            result.IsSuccess.Should().BeTrue();
            _repositoryMock.Verify(r => r.ListarEnderecoById(1), Times.Once);
            _repositoryMock.Verify(r => r.DeletarEndereco(It.IsAny<Domain.Entities.Endereco>()), Times.Once);
        }

        [Fact]
        public async Task Deve_Falhar_Deletar_Endereco_Quando_Id_Endereco_Invalido()
        {
            //Arrange
            _repositoryMock.Setup(r => r.ListarEnderecoById(1)).ReturnsAsync((Domain.Entities.Endereco)null);
            //Act
            var result = await _service.DeletarEndereco(1);
            //Assert 
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().Contain("Endereco não encontrado");
            _repositoryMock.Verify(r => r.ListarEnderecoById(1), Times.Once);
            _repositoryMock.Verify(r => r.DeletarEndereco(It.IsAny<Domain.Entities.Endereco>()), Times.Never);
        }
    }
    public class ListarEnderecoById : EnderecoServiceTest
    {

        [Fact]
        public async Task Deve_Listar_Com_Sucesso()
        {
            //Arrange
            _repositoryMock.Setup(r => r.ListarEnderecoById(1)).ReturnsAsync(new Domain.Entities.Endereco
            {
                Id = 1,
                Rua = "Rua Antiga",
                Numero = "123",
                Bairro = "Bairro Antigo",
                Cidade = "Cidade Antiga",
                Estado = "Estado Antigo",
                Cep = "1234567"
            });
            //Act
            var result = await _service.ListarEnderecoById(1);
            //Assert 
            result.IsSuccess.Should().BeTrue();
            _repositoryMock.Verify(r => r.ListarEnderecoById(1), Times.Once);
        }

        [Fact]
        public async Task Deve_Falhar_Listar_Endereco_Quando_Id_Endereco_Invalido()
        {
            //Arrange
            _repositoryMock.Setup(r => r.ListarEnderecoById(1)).ReturnsAsync((Domain.Entities.Endereco)null);
            //Act
            var result = await _service.ListarEnderecoById(1);
            //Assert 
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().Contain("Endereco não encontrado");
            _repositoryMock.Verify(r => r.ListarEnderecoById(1), Times.Once);
        }
    }

}
