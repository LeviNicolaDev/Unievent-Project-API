using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Moq;
using Unievent.Application.Dtos.Certificado;
using Unievent.Application.Interfaces.Repository;
using Unievent.Application.Interfaces.Services;
using Unievent.Application.Services;
using Xunit;

namespace Unievent.Tests.Application.Certificado;

public class CertificadoServiceTest
{
    private readonly Mock<ICertificadoRepository> _repositoryMock;
    private readonly Mock<IEventoRepository> _eventoRepositoryMock;
    private readonly Mock<IAlunoRepository> _alunoRepositoryMock;
    private readonly Mock<ILogger<CertificadoService>> _loggerMock;
    private readonly Mock<IValidator<CertificadoRequest>> _requestValidatorMock;
    private readonly Mock<IValidator<CertificadoUpdate>> _updateValidatorMock;
    private readonly ICertificadoService _service;

    public CertificadoServiceTest()
    {
        _repositoryMock = new Mock<ICertificadoRepository>();
        _loggerMock = new Mock<ILogger<CertificadoService>>();
        _requestValidatorMock = new Mock<IValidator<CertificadoRequest>>();
        _updateValidatorMock = new Mock<IValidator<CertificadoUpdate>>();
        _eventoRepositoryMock = new Mock<IEventoRepository>();
        _alunoRepositoryMock = new Mock<IAlunoRepository>();
        _service = new CertificadoService(_repositoryMock.Object, _loggerMock.Object, _requestValidatorMock.Object, _updateValidatorMock.Object, _alunoRepositoryMock.Object, _eventoRepositoryMock.Object);
    }

    public class CriarCertificado : CertificadoServiceTest
    {
        [Fact]
        public async Task Deve_Criar_Certificado_Com_Sucesso()
        {
            // Arrange
            var request = new CertificadoRequest
            {
                Texto = "Certificado de participação",
                DataCertifcado = DateTime.Now.AddDays(-1),
                AlunoId = 1,
                EventoId = 1
            };
            _requestValidatorMock.Setup(v => v.ValidateAsync(request, default)).ReturnsAsync(new FluentValidation.Results.ValidationResult());
            _alunoRepositoryMock.Setup(a => a.ListarAlunoById(request.AlunoId)).ReturnsAsync(new Domain.Entities.Aluno
            {
                Id = request.AlunoId,
                Nome = "Aluno Teste",
                DataNascimento = DateTime.Now.AddYears(-20),
                Email = "aluno@fatec.sp.gov.br",
                FotoPerfil = "foto.jpg",
                Senha = "senha123"
            });
            _eventoRepositoryMock.Setup(e => e.ListarEventoById(request.EventoId)).ReturnsAsync(new Domain.Entities.Evento
            {
                Id = request.EventoId,
                Nome = "Evento Teste",
                ResponsavelEventoId = 1,
                Capacidade = 40,
                Categoria = "Tecnologia",
                DataEvento = DateTime.Now.AddDays(10),
                Descricao = "Descrição do evento",
                HoraEvento = "14:00",
                Thumbnail = new List<string> { "thumbnail.jpg" },
            });
            _repositoryMock.Setup(r => r.CriarCertificado(It.IsAny<Domain.Entities.Certificado>())).ReturnsAsync((Domain.Entities.Certificado c) => c);
            _repositoryMock.Setup(r => r.SaveChangesAsync()).Returns(Task.FromResult(true));
            // Act
            var result = await _service.CriarCertificado(request);
            // Assert
            result.IsSuccess.Should().BeTrue();
            _repositoryMock.Verify(r => r.CriarCertificado(It.IsAny<Domain.Entities.Certificado>()), Times.Once);
            _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
            _alunoRepositoryMock.Verify(a => a.ListarAlunoById(request.AlunoId), Times.Once);
            _eventoRepositoryMock.Verify(e => e.ListarEventoById(request.EventoId), Times.Once);
        }

        [Fact]
        public async Task Deve_Falhar_Criar_Certificado_Quando_Id_Aluno_Nao_Existir()
        {
            // Arrange
            var request = new CertificadoRequest
            {
                Texto = "Certificado de participação",
                DataCertifcado = DateTime.Now.AddDays(-1),
                AlunoId = 999,
                EventoId = 1
            };
            _requestValidatorMock.Setup(v => v.ValidateAsync(request, default)).ReturnsAsync(new FluentValidation.Results.ValidationResult());
            _alunoRepositoryMock.Setup(a => a.ListarAlunoById(request.AlunoId)).ReturnsAsync((Domain.Entities.Aluno)null);
            // Act
            var result = await _service.CriarCertificado(request);
            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Be("Aluno não encontrado");
            _alunoRepositoryMock.Verify(a => a.ListarAlunoById(request.AlunoId), Times.Once);
            _eventoRepositoryMock.Verify(e => e.ListarEventoById(It.IsAny<int>()), Times.Never);

        }

        [Fact]
        public async Task Deve_Falhar_Criar_Certificado_Quando_Id_Evento_Nao_Existir()
        {
            // Arrange
            var request = new CertificadoRequest
            {
                Texto = "Certificado de participação",
                DataCertifcado = DateTime.Now.AddDays(-1),
                AlunoId = 1,
                EventoId = 999
            };
            _requestValidatorMock.Setup(v => v.ValidateAsync(request, default)).ReturnsAsync(new FluentValidation.Results.ValidationResult());
            _alunoRepositoryMock.Setup(a => a.ListarAlunoById(request.AlunoId)).ReturnsAsync(new Domain.Entities.Aluno
            {
                Id = request.AlunoId,
                Nome = "Aluno Teste",
                DataNascimento = DateTime.Now.AddYears(-20),
                Email = "aluno@fatec.sp.gov.br",
                FotoPerfil = "foto.jpg",
                Senha = "senha123"
            });
            _eventoRepositoryMock.Setup(e => e.ListarEventoById(request.EventoId)).ReturnsAsync((Domain.Entities.Evento)null);
            // Act
            var result = await _service.CriarCertificado(request);
            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Be("Evento não encontrado");
            _alunoRepositoryMock.Verify(a => a.ListarAlunoById(request.AlunoId), Times.Once);
            _eventoRepositoryMock.Verify(e => e.ListarEventoById(It.IsAny<int>()), Times.Once);
            _repositoryMock.Verify(r => r.CriarCertificado(It.IsAny<Domain.Entities.Certificado>()), Times.Never);
            _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);

        }

        [Fact]
        public async Task Deve_Falhar_Criar_Certificado_Quando_Validator_Falhar()
        {
            //Arrange
            var request = new CertificadoRequest
            {
                AlunoId = 1,
                EventoId = 1,
                DataCertifcado = DateTime.Now.AddDays(-1),
                Texto = ""

            };
            _requestValidatorMock.Setup(v => v.ValidateAsync(request, default)).ReturnsAsync(new FluentValidation.Results.ValidationResult
            {
                Errors = { new FluentValidation.Results.ValidationFailure("Texto", "Texto é obrigatório") }
            });
            //Act
            var result = await _service.CriarCertificado(request);
            //Assert
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Be("Dados inválidos");
            _requestValidatorMock.Verify(v => v.ValidateAsync(request, default), Times.Once);
            _alunoRepositoryMock.Verify(a => a.ListarAlunoById(It.IsAny<int>()), Times.Never);
            _eventoRepositoryMock.Verify(e => e.ListarEventoById(It.IsAny<int>()), Times.Never);
        }
    }
    public class AtualizarCertificado : CertificadoServiceTest
    {
        [Fact]
        public async Task Deve_Atualizar_Certificado_Com_Sucesso()
        {

            // Arrange
            var request = new CertificadoUpdate
            {
                Texto = "Certificado de participação",
                DataCertifcado = DateTime.Now.AddDays(-1),
                AlunoId = 1,
                EventoId = 1
            };
            _updateValidatorMock.Setup(v => v.ValidateAsync(request, default)).ReturnsAsync(new FluentValidation.Results.ValidationResult());
            _repositoryMock.Setup(r => r.ListarCertificadoById(1)).ReturnsAsync(new Domain.Entities.Certificado
            {
                Id = 1,
                Texto = "Certificado de participação",
                DataCertifcado = DateTime.Now.AddDays(-1),
                AlunoId = 1,
                EventoId = 1
            });
            _alunoRepositoryMock.Setup(a => a.ListarAlunoById(request.AlunoId.Value)).ReturnsAsync(new Domain.Entities.Aluno
            {
                Id = request.AlunoId.Value,
                Nome = "Aluno Teste",
                DataNascimento = DateTime.Now.AddYears(-20),
                Email = "teste@fatec.sp.gov.br",
                FotoPerfil = "foto.jpg",
                Senha = "senha123",
                IsAtivo = true
            });
            _eventoRepositoryMock.Setup(e => e.ListarEventoById(request.EventoId.Value)).ReturnsAsync(new Domain.Entities.Evento
            {
                Id = request.EventoId.Value,
                Nome = "Evento Teste",
                ResponsavelEventoId = 1,
                Capacidade = 40,
                Categoria = "Tecnologia",
                DataEvento = DateTime.Now.AddDays(10),
                Descricao = "Descrição do evento",
                HoraEvento = "14:00",
                Thumbnail = new List<string> { "thumbnail.jpg" },
            });
            _repositoryMock.Setup(r => r.AtualizarCertificado(It.IsAny<Domain.Entities.Certificado>())).ReturnsAsync((Domain.Entities.Certificado c) => c);
            _repositoryMock.Setup(r => r.SaveChangesAsync()).Returns(Task.FromResult(true));
            // Act
            var result = await _service.AtualizarCertificado(1, request);
            // Assert
            _repositoryMock.Verify(r => r.AtualizarCertificado(It.IsAny<Domain.Entities.Certificado>()), Times.Once);
            _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
            _alunoRepositoryMock.Verify(a => a.ListarAlunoById(request.AlunoId.Value), Times.Once);
            _eventoRepositoryMock.Verify(e => e.ListarEventoById(request.EventoId.Value), Times.Once);
            result.IsSuccess.Should().BeTrue();

        }

        public async Task Deve_Falhar_Atualizar_Certificado_Quando_Id_Certificado_Nao_Existir()
        {
            // Arrange
            var request = new CertificadoUpdate
            {
                Texto = "Certificado de participação",
                DataCertifcado = DateTime.Now.AddDays(-1),
                AlunoId = 1,
                EventoId = 1
            };
            _updateValidatorMock.Setup(v => v.ValidateAsync(request, default)).ReturnsAsync(new FluentValidation.Results.ValidationResult());
            _repositoryMock.Setup(r => r.ListarCertificadoById(999)).ReturnsAsync((Domain.Entities.Certificado)null);
            // Act
            var result = await _service.AtualizarCertificado(999, request);
            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Be("Certificado não encontrado");
            _repositoryMock.Verify(r => r.ListarCertificadoById(999), Times.Once);
            _alunoRepositoryMock.Verify(a => a.ListarAlunoById(It.IsAny<int>()), Times.Never);
            _eventoRepositoryMock.Verify(e => e.ListarEventoById(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task Deve_Falhar_Atualizar_Certificado_Quando_Validator_Falhar()
        {
            //Arrange
            var request = new CertificadoUpdate
            {
                AlunoId = 1,
                EventoId = 1,
                DataCertifcado = DateTime.Now.AddDays(-1),
                Texto = ""

            };
            _updateValidatorMock.Setup(v => v.ValidateAsync(request, default)).ReturnsAsync(new FluentValidation.Results.ValidationResult
            {
                Errors = { new FluentValidation.Results.ValidationFailure("Texto", "Texto é obrigatório") }
            });
            //Act
            var result = await _service.AtualizarCertificado(1, request);
            //Assert
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Be("Dados inválidos");
            _updateValidatorMock.Verify(v => v.ValidateAsync(request, default), Times.Once);
            _repositoryMock.Verify(r => r.ListarCertificadoById(It.IsAny<int>()), Times.Never);
            _alunoRepositoryMock.Verify(a => a.ListarAlunoById(It.IsAny<int>()), Times.Never);
            _eventoRepositoryMock.Verify(e => e.ListarEventoById(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task Deve_Falhar_Atualizar_Certificado_Quando_Id_Aluno_Nao_Existir()
        {
            // Arrange
            var request = new CertificadoUpdate
            {
                Texto = "Certificado de participação",
                DataCertifcado = DateTime.Now.AddDays(-1),
                AlunoId = 999,
                EventoId = 1
            };
            _updateValidatorMock.Setup(v => v.ValidateAsync(request, default)).ReturnsAsync(new FluentValidation.Results.ValidationResult());
            _repositoryMock.Setup(r => r.ListarCertificadoById(1)).ReturnsAsync(new Domain.Entities.Certificado
            {
                Id = 1,
                Texto = "Certificado de participação",
                DataCertifcado = DateTime.Now.AddDays(-1),
                AlunoId = 1,
                EventoId = 1
            });
            _alunoRepositoryMock.Setup(a => a.ListarAlunoById(request.AlunoId.Value)).ReturnsAsync((Domain.Entities.Aluno)null);
            // Act
            var result = await _service.AtualizarCertificado(1, request);
            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Be("Aluno não encontrado");
            _repositoryMock.Verify(r => r.ListarCertificadoById(1), Times.Once);
            _alunoRepositoryMock.Verify(a => a.ListarAlunoById(request.AlunoId.Value), Times.Once);
            _eventoRepositoryMock.Verify(e => e.ListarEventoById(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task Deve_Falhar_Atualizar_Certificado_Quando_Id_Evento_Nao_Existir()
        {
            //Arrange
            var request = new CertificadoUpdate
            {
                AlunoId = 1,
                EventoId = 999,
                DataCertifcado = DateTime.Now.AddDays(-1),
                Texto = "Certificado de participação"
            };
            _updateValidatorMock.Setup(v => v.ValidateAsync(request, default)).ReturnsAsync(new FluentValidation.Results.ValidationResult());
            _repositoryMock.Setup(r => r.ListarCertificadoById(1)).ReturnsAsync(new Domain.Entities.Certificado
            {
                Id = 1,
                AlunoId = 1,
                EventoId = 1,
                DataCertifcado = DateTime.Now.AddDays(-1),
                Texto = "Certificado de participação"
            });
            _alunoRepositoryMock.Setup(a => a.ListarAlunoById(request.AlunoId.Value)).ReturnsAsync(new Domain.Entities.Aluno
            {
                Id = request.AlunoId.Value,
                Nome = "Aluno Teste",
                DataNascimento = DateTime.Now.AddYears(-20),
                Email = "teste@fatec.sp.gov.br",
                FotoPerfil = "foto.jpg",
                Senha = "senha123",
                IsAtivo = true
            });
            _eventoRepositoryMock.Setup(e => e.ListarEventoById(request.EventoId.Value)).ReturnsAsync((Domain.Entities.Evento)null);
            // Act
            var result = await _service.AtualizarCertificado(1, request);
            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Be("Evento não encontrado");
            _repositoryMock.Verify(r => r.ListarCertificadoById(1), Times.Once);
            _alunoRepositoryMock.Verify(a => a.ListarAlunoById(request.AlunoId.Value), Times.Once);
            _eventoRepositoryMock.Verify(e => e.ListarEventoById(request.EventoId.Value), Times.Once);
            _repositoryMock.Verify(r => r.AtualizarCertificado(It.IsAny<Domain.Entities.Certificado>()), Times.Never);
            _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }
    }
    public class DeletarCertificado : CertificadoServiceTest
    {
        [Fact]
        public async Task Deve_Deletar_Certificado_Com_Sucesso()
        {
            // Arrange
            _repositoryMock.Setup(r => r.ListarCertificadoById(1)).ReturnsAsync(new Domain.Entities.Certificado
            {
                Id = 1,
                Texto = "Certificado de participação",
                DataCertifcado = DateTime.Now.AddDays(-1),
                AlunoId = 1,
                EventoId = 1
            });
            _repositoryMock.Setup(r => r.DeletarCertificado(It.IsAny<Domain.Entities.Certificado>())).Returns(Task.FromResult(true));
            _repositoryMock.Setup(r => r.SaveChangesAsync()).Returns(Task.FromResult(true));
            // Act
            var result = await _service.DeletarCertificado(1);
            // Assert
            result.IsSuccess.Should().BeTrue();
            _repositoryMock.Verify(r => r.ListarCertificadoById(1), Times.Once);
            _repositoryMock.Verify(r => r.DeletarCertificado(It.IsAny<Domain.Entities.Certificado>()), Times.Once);
            _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
            result.Message.Should().Be("Certificado deletado com sucesso");
        }

        [Fact]
        public async Task Deve_Falhar_Deletar_Certificado_Quando_Id_Certificado_Nao_Existir()
        {
            // Arrange
            _repositoryMock.Setup(r => r.ListarCertificadoById(999)).ReturnsAsync((Domain.Entities.Certificado)null);
            // Act
            var result = await _service.DeletarCertificado(999);
            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Be("Certificado não encontrado");
            _repositoryMock.Verify(r => r.ListarCertificadoById(999), Times.Once);
            _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }
    }
    public class ListarCertificadoById : CertificadoServiceTest
    {
        [Fact]
        public async Task Deve_Listar_Certificado_Com_Sucesso()
        {
            // Arrange
            _repositoryMock.Setup(r => r.ListarCertificadoById(1)).ReturnsAsync(new Domain.Entities.Certificado
            {
                Id = 1,
                Texto = "Certificado de participação",
                DataCertifcado = DateTime.Now.AddDays(-1),
                AlunoId = 1,
                EventoId = 1
            });
            // Act
            var result = await _service.ListarCertificadoById(1);
            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Id.Should().Be(1);
            result.Data.Texto.Should().Be("Certificado de participação");
            _repositoryMock.Verify(r => r.ListarCertificadoById(1), Times.Once);
        }

        [Fact]
        public async Task Deve_Falhar_Listar_Certificado_Quando_Id_Certificado_Nao_Existir()
        {
            // Arrange
            _repositoryMock.Setup(r => r.ListarCertificadoById(999)).ReturnsAsync((Domain.Entities.Certificado)null);
            // Act
            var result = await _service.ListarCertificadoById(999);
            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Be("Certificado não encontrado");
            _repositoryMock.Verify(r => r.ListarCertificadoById(999), Times.Once);
        }
    }

}
