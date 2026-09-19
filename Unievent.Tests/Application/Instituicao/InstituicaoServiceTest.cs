using FluentAssertions;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Unievent.Application.Dtos.Instituicao;
using Unievent.Application.Interfaces.Repository;
using Unievent.Application.Interfaces.Services;
using Unievent.Application.Services;
using Xunit;

namespace Unievent.Tests.Application.Instituicao;

public class InstituicaoServiceTest
{
    private readonly Mock<IValidator<InstituicaoRequest>> _mockRequestValidator;
    private readonly Mock<IValidator<InstituicaoUpdate>> _mockUpdateValidator;
    private readonly Mock<IInstituicaoRepository> _mockRepository;
    private readonly IInstituicaoService _service;

    public InstituicaoServiceTest()
    {
        _mockRepository = new Mock<IInstituicaoRepository>();
        _mockRequestValidator = new Mock<IValidator<InstituicaoRequest>>();
        _mockUpdateValidator = new Mock<IValidator<InstituicaoUpdate>>();
        var mockLogger = new Mock<ILogger<InstituicaoService>>();
        _service = new InstituicaoService(_mockRepository.Object, mockLogger.Object, _mockRequestValidator.Object, _mockUpdateValidator.Object);
    }

    public class CriarInstituicao : InstituicaoServiceTest
    {
        [Fact]
        public async Task Deve_Criar_Instituicao_Com_Sucesso()
        {
            var request = CriarRequest();

            _mockRequestValidator.Setup(v => v.ValidateAsync(It.IsAny<InstituicaoRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());
            _mockRepository.Setup(r => r.CriarInstituicao(It.IsAny<Domain.Entities.Instituicao>()))
                .ReturnsAsync((Domain.Entities.Instituicao i) => i);
            _mockRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.FromResult(true));

            var result = await _service.CriarInstituicao(request);

            result.IsSuccess.Should().BeTrue();
            result.Value!.Rua.Should().Be(request.Rua);
            result.Value.Cidade.Should().Be(request.Cidade);
            _mockRepository.Verify(r => r.CriarInstituicao(It.Is<Domain.Entities.Instituicao>(i =>
                i.Rua == request.Rua &&
                i.Numero == request.Numero &&
                i.Bairro == request.Bairro &&
                i.Cidade == request.Cidade &&
                i.Estado == request.Estado &&
                i.Cep == request.Cep)), Times.Once);
        }

        [Fact]
        public async Task Deve_Falhar_Criar_Instituicao_Quando_Validator_Falhar()
        {
            var request = CriarRequest();

            _mockRequestValidator.Setup(v => v.ValidateAsync(It.IsAny<InstituicaoRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult(new[]
                {
                    new FluentValidation.Results.ValidationFailure("Dados", "Dados Inválidos")
                }));

            var result = await _service.CriarInstituicao(request);

            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().Contain("Dados Inválidos");
            _mockRepository.Verify(r => r.CriarInstituicao(It.IsAny<Domain.Entities.Instituicao>()), Times.Never);
            _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        }
    }

    public class AtualizarInstituicao : InstituicaoServiceTest
    {
        [Fact]
        public async Task Deve_Atualizar_Instituicao_Com_Sucesso()
        {
            var instituicaoId = 1;
            var request = new InstituicaoUpdate
            {
                Cnpj = "16203686000158",
                Rua = "Rua Atualizada",
                Numero = "34",
                Bairro = "Centro",
                Cidade = "Sao Paulo",
                Estado = "SP",
                Cep = "12345678",
                FotoPerfil = CriarArquivo()
            };

            _mockUpdateValidator.Setup(v => v.ValidateAsync(It.IsAny<InstituicaoUpdate>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());
            _mockRepository.Setup(r => r.ListarInstituicaoById(instituicaoId))
                .ReturnsAsync(CriarEntidade(instituicaoId));
            _mockRepository.Setup(r => r.AtualizarInstituicao(It.IsAny<Domain.Entities.Instituicao>()))
                .ReturnsAsync((Domain.Entities.Instituicao i) => i);
            _mockRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.FromResult(true));

            var result = await _service.AtualizarInstituicao(instituicaoId, request);

            result.IsSuccess.Should().BeTrue();
            result.Value!.Rua.Should().Be(request.Rua);
            _mockRepository.Verify(r => r.AtualizarInstituicao(It.Is<Domain.Entities.Instituicao>(i =>
                i.Rua == request.Rua &&
                i.Numero == request.Numero &&
                i.Bairro == request.Bairro &&
                i.Cidade == request.Cidade &&
                i.Estado == request.Estado &&
                i.Cep == request.Cep)), Times.Once);
            _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Deve_Falhar_Atualizar_Instituicao_Quando_Id_Instituicao_Invalido()
        {
            var instituicaoId = 999;
            var request = new InstituicaoUpdate
            {
                Rua = "Rua Atualizada"
            };

            _mockUpdateValidator.Setup(v => v.ValidateAsync(It.IsAny<InstituicaoUpdate>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());
            _mockRepository.Setup(r => r.ListarInstituicaoById(instituicaoId))
                .ReturnsAsync((Domain.Entities.Instituicao)null);

            var result = await _service.AtualizarInstituicao(instituicaoId, request);

            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().Contain("Instituição não encontrada");
            _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task Deve_Falhar_Atualizar_Instituicao_Quando_Validator_Falha()
        {
            var request = new InstituicaoUpdate { Nome = "FATEC Teste" };

            _mockUpdateValidator.Setup(v => v.ValidateAsync(It.IsAny<InstituicaoUpdate>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult(new[]
                {
                    new FluentValidation.Results.ValidationFailure("Dados", "Dados Inválidos")
                }));

            var result = await _service.AtualizarInstituicao(1, request);

            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().Contain("Dados Inválidos");
            _mockRepository.Verify(r => r.AtualizarInstituicao(It.IsAny<Domain.Entities.Instituicao>()), Times.Never);
            _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        }
    }

    public class DeletarInstituicao : InstituicaoServiceTest
    {
        [Fact]
        public async Task Deve_Deletar_Instituicao_Com_Sucesso()
        {
            var instituicaoId = 1;
            _mockRepository.Setup(r => r.ListarInstituicaoById(instituicaoId))
                .ReturnsAsync(CriarEntidade(instituicaoId));
            _mockRepository.Setup(r => r.AtualizarInstituicao(It.IsAny<Domain.Entities.Instituicao>()))
                .ReturnsAsync((Domain.Entities.Instituicao i) => i);
            _mockRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.FromResult(true));

            var result = await _service.DeletarInstituicao(instituicaoId);

            result.IsSuccess.Should().BeTrue();
            _mockRepository.Verify(r => r.AtualizarInstituicao(It.Is<Domain.Entities.Instituicao>(i => i.IsAtivo == false)), Times.Once);
            _mockRepository.Verify(r => r.DeletarInstituicao(It.IsAny<Domain.Entities.Instituicao>()), Times.Never);
        }

        [Fact]
        public async Task Deve_Falhar_Deletar_Instituicao_Com_Id_Invalido()
        {
            var instituicaoId = 999;
            _mockRepository.Setup(r => r.ListarInstituicaoById(instituicaoId))
                .ReturnsAsync((Domain.Entities.Instituicao)null);

            var result = await _service.DeletarInstituicao(instituicaoId);

            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().Contain("Instituição não encontrada");
            _mockRepository.Verify(r => r.DeletarInstituicao(It.IsAny<Domain.Entities.Instituicao>()), Times.Never);
        }
    }

    public class ListarInstituicaoById : InstituicaoServiceTest
    {
        [Fact]
        public async Task Deve_Listar_Instituicao_Por_Id_Com_Sucesso()
        {
            var instituicaoId = 1;
            _mockRepository.Setup(r => r.ListarInstituicaoById(instituicaoId))
                .ReturnsAsync(CriarEntidade(instituicaoId));

            var result = await _service.ListarInstituicaoById(instituicaoId);

            result.IsSuccess.Should().BeTrue();
            result.Value!.Cidade.Should().Be("Sao Paulo");
            _mockRepository.Verify(r => r.ListarInstituicaoById(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task Deve_Falhar_Listar_Instituicao_Por_Id_Quando_Id_Invalido()
        {
            var instituicaoId = 1;
            _mockRepository.Setup(r => r.ListarInstituicaoById(instituicaoId))
                .ReturnsAsync((Domain.Entities.Instituicao)null);

            var result = await _service.ListarInstituicaoById(instituicaoId);

            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().Contain("Instituição não encontrada");
            _mockRepository.Verify(r => r.ListarInstituicaoById(It.IsAny<int>()), Times.Once);
        }
    }

    private static InstituicaoRequest CriarRequest()
    {
        return new InstituicaoRequest
        {
            Nome = "FATEC Teste",
            NomeAbreviado = "FATEC Teste",
            Codigo = "fatec-teste",
            Cnpj = "16203686000158",
            Rua = "Rua Carlos Barattino",
            Numero = "908",
            Bairro = "Vila Romanopolis",
            Cidade = "Sao Paulo",
            Estado = "SP",
            Cep = "12345678",
            FotoPerfil = CriarArquivo()
        };
    }

    private static Domain.Entities.Instituicao CriarEntidade(int id)
    {
        return new Domain.Entities.Instituicao
        {
            Id = id,
            Nome = "FATEC Teste",
            NomeAbreviado = "FATEC Teste",
            Codigo = "fatec-teste",
            Cnpj = "16203686000158",
            Rua = "Rua Carlos Barattino",
            Numero = "908",
            Bairro = "Vila Romanopolis",
            Cidade = "Sao Paulo",
            Estado = "SP",
            Cep = "12345678",
            FotoPerfil = "foto.jpg",
            IsAtivo = true
        };
    }

    private static IFormFile CriarArquivo()
    {
        var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes("conteudo fake"));
        return new FormFile(stream, 0, stream.Length, "FotoPerfil", "foto.jpg")
        {
            Headers = new HeaderDictionary(),
            ContentType = "image/jpeg"
        };
    }
}
