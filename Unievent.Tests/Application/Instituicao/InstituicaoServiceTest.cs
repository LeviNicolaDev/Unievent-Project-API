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
using Xunit.Sdk;

namespace Unievent.Tests.Application.Instituicao;

public class InstituicaoServiceTest
{
    private readonly Mock<IValidator<InstituicaoRequest>> _mockRequestValidator;
    private readonly Mock<IValidator<InstituicaoUpdate>> _mockUpdateValidator;
    private readonly Mock<IInstituicaoRepository> _mockRepository;
    private readonly Mock<IEnderecoRepository> _mockEnderecoRepository;
    private readonly Mock<ILogger<InstituicaoService>> _mockLogger;
    private readonly IInstituicaoService _service;


    public InstituicaoServiceTest()
    {
        _mockRepository = new Mock<IInstituicaoRepository>();
        _mockEnderecoRepository = new Mock<IEnderecoRepository>();
        _mockLogger = new Mock<ILogger<InstituicaoService>>();
        _mockRequestValidator = new Mock<IValidator<InstituicaoRequest>>();
        _mockUpdateValidator = new Mock<IValidator<InstituicaoUpdate>>();
        _service = new InstituicaoService(_mockRepository.Object, _mockEnderecoRepository.Object, _mockLogger.Object, _mockRequestValidator.Object, _mockUpdateValidator.Object);

    }

    public class CriarInstituicao : InstituicaoServiceTest
    {
        [Fact]
        public async Task Deve_Criar_Instituicao_Com_Sucesso()
        {
            //Arrange
            using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes("conteudo fake"));
            var request = new InstituicaoRequest
            {
                Cnpj = "16203686000158",
                SenhaLogin = "senhaaa",
                EmailLogin = "email@fatec.sp.gov.br",
                EnderecoId = 1,
                FotoPerfil = new FormFile(stream, 0, stream.Length, "FotoPerfil", "foto.jpg")
                {
                    Headers = new HeaderDictionary(),

                    ContentType = "image/jpeg"
                }
            };

            _mockRequestValidator.Setup(v => v.ValidateAsync(It.IsAny<InstituicaoRequest>())).ReturnsAsync(new FluentValidation.Results.ValidationResult());
            _mockEnderecoRepository.Setup(r => r.ListarEnderecoById(request.EnderecoId)).ReturnsAsync(new Domain.Entities.Endereco
            {
                Id = 1,
                Bairro = "bairro",
                Cep = "12345678",
                Cidade = "sao paulo",
                Estado = "sp",
                Numero = "12",
                Rua = "rua"

            });
            _mockRepository.Setup(r => r.ListarInstituicaoByEmail(It.IsAny<string>())).ReturnsAsync((Domain.Entities.Instituicao)null);
            _mockRepository.Setup(r => r.CriarInstituicao(It.IsAny<Domain.Entities.Instituicao>())).ReturnsAsync((Domain.Entities.Instituicao i) => i);
            _mockRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.FromResult(true));

            //act
            var result = await _service.CriarInstituicao(request);
            //Assert
            result.IsSuccess.Should().BeTrue();
            _mockRepository.Verify(r => r.CriarInstituicao(It.IsAny<Domain.Entities.Instituicao>()), Times.Once);
        }

        [Fact]
        public async Task Deve_Falhar_Criar_Instituicao_Quando_Endereco_Nao_Existir()
        {
            //Arrange
            using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes("conteudo fake"));
            var request = new InstituicaoRequest
            {
                Cnpj = "16203686000158",
                SenhaLogin = "senhaaa",
                EmailLogin = "email@fatec.sp.gov.br",
                EnderecoId = 99999,
                FotoPerfil = new FormFile(stream, 0, stream.Length, "FotoPerfil", "foto.jpg")
                {
                    Headers = new HeaderDictionary(),

                    ContentType = "image/jpeg"
                }
            };

            _mockRequestValidator.Setup(v => v.ValidateAsync(It.IsAny<InstituicaoRequest>())).ReturnsAsync(new FluentValidation.Results.ValidationResult());
            _mockEnderecoRepository.Setup(r => r.ListarEnderecoById(request.EnderecoId)).ReturnsAsync((Domain.Entities.Endereco)null);

            //act
            var result = await _service.CriarInstituicao(request);
            //Assert
            result.IsSuccess.Should().BeFalse();
            _mockRepository.Verify(r => r.CriarInstituicao(It.IsAny<Domain.Entities.Instituicao>()), Times.Never);
            _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
            result.Errors.Should().Contain("Endereço não encontrado para ser associado à instituição");
        }

        public async Task Deve_Falhar_Criar_Instituicao_Quando_Validator_Falhar()
        {
            //Arrange
            using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes("conteudo fake"));
            var request = new InstituicaoRequest
            {
                Cnpj = "16203686000158",
                SenhaLogin = "senhaaa",
                EmailLogin = "email@fatec.sp.gov.br",
                EnderecoId = 99999,
                FotoPerfil = new FormFile(stream, 0, stream.Length, "FotoPerfil", "foto.jpg")
                {
                    Headers = new HeaderDictionary(),

                    ContentType = "image/jpeg"
                }
            };

            _mockRequestValidator.Setup(v => v.ValidateAsync(It.IsAny<InstituicaoRequest>())).ReturnsAsync(new FluentValidation.Results.ValidationResult(new[]
            {
                new FluentValidation.Results.ValidationFailure("Dados", "Dados Inválidos")
            }));

            //act
            var result = await _service.CriarInstituicao(request);
            //Assert
            result.IsSuccess.Should().BeFalse();
            _mockRequestValidator.Verify(v => v.ValidateAsync(It.IsAny<InstituicaoRequest>()), Times.Once);
            _mockRepository.Verify(r => r.CriarInstituicao(It.IsAny<Domain.Entities.Instituicao>()), Times.Never);
            _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

    }
    public class AtualizarInstituicao : InstituicaoServiceTest
    {
        [Fact]
        public async Task Deve_Atualizar_Instituicao_Com_Sucesso()
        {
            //Arrange
            using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes("conteudo fake"));
            var instituicaoId = 1;
            var request = new InstituicaoUpdate
            {
                Cnpj = "16203686000158",
                SenhaLogin = "senhaaa",
                EmailLogin = "email@fatec.sp.gov.br",
                EnderecoId = 1,
                FotoPerfil = new FormFile(stream, 0, stream.Length, "FotoPerfil", "foto.jpg")
                {
                    Headers = new HeaderDictionary(),

                    ContentType = "image/jpeg"
                }
            };

            _mockUpdateValidator.Setup(v => v.ValidateAsync(It.IsAny<InstituicaoUpdate>())).ReturnsAsync(new FluentValidation.Results.ValidationResult());
            _mockRepository.Setup(r => r.ListarInstituicaoById(instituicaoId)).ReturnsAsync(new Domain.Entities.Instituicao
            {
                Id = instituicaoId,
                Cnpj = "16203686000158",
                SenhaLogin = "senhaaa",
                EmailLogin = "email@fatec.sp.gov.br",
                EnderecoId = 1,
                FotoPerfil = "foto.jpg"
            });
            _mockEnderecoRepository.Setup(r => r.ListarEnderecoById(request.EnderecoId.Value)).ReturnsAsync(new Domain.Entities.Endereco
            {
                Id = 1,
                Bairro = "bairro",
                Cep = "12345678",
                Cidade = "sao paulo",
                Estado = "sp",
                Numero = "12",
                Rua = "rua"

            });
            _mockRepository.Setup(r => r.ListarInstituicaoByEmail(request.EmailLogin)).ReturnsAsync((Domain.Entities.Instituicao)null);
            _mockRepository.Setup(r => r.AtualizarInstituicao(It.IsAny<Domain.Entities.Instituicao>())).ReturnsAsync((Domain.Entities.Instituicao i) => i);
            _mockRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.FromResult(true));

            //act
            var result = await _service.AtualizarInstituicao(instituicaoId, request);
            //Assert
            result.IsSuccess.Should().BeTrue();
            _mockRepository.Verify(r => r.AtualizarInstituicao(It.IsAny<Domain.Entities.Instituicao>()), Times.Once);
            _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Deve_Falhar_Atualizar_Instituicao_Quando_Id_Instituicao_Invalido()
        {
            //Arrange
            using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes("conteudo fake"));
            var instituicaoId = 999;
            var request = new InstituicaoUpdate
            {
                Cnpj = "16203686000158",
                SenhaLogin = "senhaaa",
                EmailLogin = "email@fatec.sp.gov.br",
                EnderecoId = 1,
                FotoPerfil = new FormFile(stream, 0, stream.Length, "FotoPerfil", "foto.jpg")
                {
                    Headers = new HeaderDictionary(),

                    ContentType = "image/jpeg"
                }
            };

            _mockUpdateValidator.Setup(v => v.ValidateAsync(It.IsAny<InstituicaoUpdate>())).ReturnsAsync(new FluentValidation.Results.ValidationResult());
            _mockRepository.Setup(r => r.ListarInstituicaoById(instituicaoId)).ReturnsAsync((Domain.Entities.Instituicao)null);
            //act
            var result = await _service.AtualizarInstituicao(instituicaoId, request);
            //Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().Contain("Instituição não encontrada");
            _mockRepository.Verify(r => r.ListarInstituicaoById(It.IsAny<int>()), Times.Once);
            _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        }
        [Fact]
        public async Task Deve_Falhar_Atualizar_Instituicao_Quando_Email_Instituicao_Ja_Existe()
        {
            //Arrange
            using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes("conteudo fake"));
            var instituicaoId = 1;
            var request = new InstituicaoUpdate
            {
                Cnpj = "16203686000158",
                SenhaLogin = "senhaaa",
                EmailLogin = "email@fatec.sp.gov.br",
                EnderecoId = 1,
                FotoPerfil = new FormFile(stream, 0, stream.Length, "FotoPerfil", "foto.jpg")
                {
                    Headers = new HeaderDictionary(),

                    ContentType = "image/jpeg"
                }
            };

            _mockUpdateValidator.Setup(v => v.ValidateAsync(It.IsAny<InstituicaoUpdate>())).ReturnsAsync(new FluentValidation.Results.ValidationResult());
            _mockRepository.Setup(r => r.ListarInstituicaoById(instituicaoId)).ReturnsAsync(new Domain.Entities.Instituicao
            {
                Id = instituicaoId,
                Cnpj = "16203686000158",
                SenhaLogin = "senhaaa",
                EmailLogin = "email@fatec.sp.gov.br",
                EnderecoId = 1,
                FotoPerfil = "foto.jpg"
            });
            _mockEnderecoRepository.Setup(r => r.ListarEnderecoById(request.EnderecoId.Value)).ReturnsAsync(new Domain.Entities.Endereco
            {
                Id = 1,
                Bairro = "bairro",
                Cep = "12345678",
                Cidade = "sao paulo",
                Estado = "sp",
                Numero = "12",
                Rua = "rua"

            });
            _mockRepository.Setup(r => r.ListarInstituicaoByEmail(request.EmailLogin)).ReturnsAsync(new Domain.Entities.Instituicao
            {
                Id = instituicaoId,
                Cnpj = "16203686000158",
                SenhaLogin = "senhaaa",
                EmailLogin = "email@existente.com",
                EnderecoId = 1,
                FotoPerfil = "foto.jpg"
            });
            //act
            var result = await _service.AtualizarInstituicao(instituicaoId, request);
            //Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().Contain("Email já cadastrado para outra instituição");
            _mockRepository.Verify(r => r.AtualizarInstituicao(It.IsAny<Domain.Entities.Instituicao>()), Times.Never);
            _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        public async Task Deve_Falhar_Atualizar_Instituicao_Quando_Endereco_Instituicao_Nao_Existe()
        {
            //Arrange
            using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes("conteudo fake"));
            var instituicaoId = 1;
            var request = new InstituicaoUpdate
            {
                Cnpj = "16203686000158",
                SenhaLogin = "senhaaa",
                EmailLogin = "email@fatec.sp.gov.br",
                EnderecoId = 1,
                FotoPerfil = new FormFile(stream, 0, stream.Length, "FotoPerfil", "foto.jpg")
                {
                    Headers = new HeaderDictionary(),

                    ContentType = "image/jpeg"
                }
            };

            _mockUpdateValidator.Setup(v => v.ValidateAsync(It.IsAny<InstituicaoUpdate>())).ReturnsAsync(new FluentValidation.Results.ValidationResult());
            _mockRepository.Setup(r => r.ListarInstituicaoById(instituicaoId)).ReturnsAsync(new Domain.Entities.Instituicao
            {
                Id = instituicaoId,
                Cnpj = "16203686000158",
                SenhaLogin = "senhaaa",
                EmailLogin = "email@fatec.sp.gov.br",
                EnderecoId = 1,
                FotoPerfil = "foto.jpg"
            });
            _mockEnderecoRepository.Setup(r => r.ListarEnderecoById(request.EnderecoId.Value)).ReturnsAsync(new Domain.Entities.Endereco
            {
                Id = 1,
                Bairro = "bairro",
                Cep = "12345678",
                Cidade = "sao paulo",
                Estado = "sp",
                Numero = "12",
                Rua = "rua"

            });
            //act
            var result = await _service.AtualizarInstituicao(instituicaoId, request);
            //Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().Contain("Endereço não encontrado para ser associado à instituição");
            _mockRepository.Verify(r => r.AtualizarInstituicao(It.IsAny<Domain.Entities.Instituicao>()), Times.Never);
            _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        }
        public async Task Deve_Falhar_Atualizar_Instituicao_Quando_Validator_Falha()
        {
            //Arrange
            using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes("conteudo fake"));
            var instituicaoId = 1;
            var request = new InstituicaoUpdate
            {
                Cnpj = "16203686000158",
                SenhaLogin = "senhaaa",
                EmailLogin = "email@fatec.sp.gov.br",
                EnderecoId = 1,
                FotoPerfil = new FormFile(stream, 0, stream.Length, "FotoPerfil", "foto.jpg")
                {
                    Headers = new HeaderDictionary(),

                    ContentType = "image/jpeg"
                }
            };

            _mockUpdateValidator.Setup(v => v.ValidateAsync(It.IsAny<InstituicaoUpdate>())).ReturnsAsync(new FluentValidation.Results.ValidationResult(new[]
            {
                new FluentValidation.Results.ValidationFailure("Dados", "Dados Inválidos")
            }));
            //act
            var result = await _service.AtualizarInstituicao(instituicaoId, request);
            //Assert
            result.IsSuccess.Should().BeFalse();
            _mockRepository.Verify(r => r.AtualizarInstituicao(It.IsAny<Domain.Entities.Instituicao>()), Times.Never);
            _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        }
    }
    public class DeletarInstituicao : InstituicaoServiceTest
    {
        [Fact]
        public async Task Deve_Deletar_Instituicao_Com_Sucesso()
        {
            //Arrange
            var instituicaoId = 1;
            _mockRepository.Setup(r => r.ListarInstituicaoById(instituicaoId)).ReturnsAsync(new Domain.Entities.Instituicao
            {
                Id = instituicaoId,
                Cnpj = "16203686000158",
                SenhaLogin = "senhaaa",
                EmailLogin = "email@fatec.sp.gov.br",
                EnderecoId = 1,
                FotoPerfil = "foto.jpg"
            });
            _mockRepository.Setup(r => r.DeletarInstituicao(It.IsAny<Domain.Entities.Instituicao>())).ReturnsAsync(true);
            _mockRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.FromResult(true));
            //Act
            var result = await _service.DeletarInstituicao(instituicaoId);
            //Assert
            result.IsSuccess.Should().BeTrue();

            _mockRepository.Verify(r => r.DeletarInstituicao(It.IsAny<Domain.Entities.Instituicao>()), Times.Once);
        }

        [Fact]
        public async Task Deve_Falhar_Deletar_Instituicao_Com_Id_Invalido()
        {
            //Arrange
            var instituicaoId = 999;
            _mockRepository.Setup(r => r.ListarInstituicaoById(instituicaoId)).ReturnsAsync((Domain.Entities.Instituicao)null);
            //Act
            var result = await _service.DeletarInstituicao(instituicaoId);
            //Assert
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
            //Arrange
            var instituicaoId = 1;
            _mockRepository.Setup(r => r.ListarInstituicaoById(instituicaoId)).ReturnsAsync(new Domain.Entities.Instituicao
            {
                Id = instituicaoId,
                Cnpj = "16203686000158",
                SenhaLogin = "senhaaa",
                EmailLogin = "email@fatec.sp.gov.br",
                EnderecoId = 1,
                FotoPerfil = "foto.jpg"
            });
            //Act
            var result = await _service.ListarInstituicaoById(instituicaoId);
            //Assert
            result.IsSuccess.Should().BeTrue();
            _mockRepository.Verify(r => r.ListarInstituicaoById(It.IsAny<int>()), Times.Once);
        }


        [Fact]
        public async Task Deve_Falhar_Listar_Instituicao_Por_Id_Quando_Id_Invalido()
        {
            //Arrange
            var instituicaoId = 1;
            _mockRepository.Setup(r => r.ListarInstituicaoById(instituicaoId)).ReturnsAsync((Domain.Entities.Instituicao)null);
            //Act
            var result = await _service.ListarInstituicaoById(instituicaoId);
            //Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().Contain("Instituição não encontrada");
            _mockRepository.Verify(r => r.ListarInstituicaoById(It.IsAny<int>()), Times.Once);
        }
    }
}
