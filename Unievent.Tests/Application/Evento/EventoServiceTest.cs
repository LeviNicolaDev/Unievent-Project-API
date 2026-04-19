using FluentAssertions;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Unievent.Application.Dtos.Evento;
using Unievent.Application.Interfaces.Repository;
using Unievent.Application.Interfaces.Services;
using Unievent.Application.Services;
using Xunit;

namespace Unievent.Tests.Application.Evento;

public class EventoServiceTest
{
    private readonly Mock<IEventoRepository> _repository;
    private readonly Mock<ILogger<EventoService>> _logger;
    private readonly Mock<IValidator<EventoRequest>> _validatorRequest;
    private readonly Mock<IValidator<EventoUpdate>> _validatorUpdate;
    private readonly IEventoService _service;

    public EventoServiceTest()
    {
        _repository = new Mock<IEventoRepository>();
        _logger = new Mock<ILogger<EventoService>>();
        _validatorRequest = new Mock<IValidator<EventoRequest>>();
        _validatorUpdate = new Mock<IValidator<EventoUpdate>>();
        _service = new EventoService(_repository.Object, _logger.Object, _validatorRequest.Object, _validatorUpdate.Object);
    }

    public class CriarEvento : EventoServiceTest
    {
        [Fact]
        public async Task Deve_Criar_Evento_Com_Sucesso()
        {
            //Arrange
            var request = new EventoRequest
            {
                Nome = "Evento de Teste",
                Descricao = "Descrição do evento de teste",
                DataEvento = DateTime.Now.AddDays(10),
                Categoria = "Categoria de Teste",
                ResponsavelEventoId = 1,
                Capacidade = 100,
                Thumbnail = new List<IFormFile> { new FormFile(new MemoryStream(), 0, 0, "Thumbnail", "thumbnail.jpg") },
                HoraEvento = "18:00"
            };

            _validatorRequest.Setup(v => v.ValidateAsync(request, default)).ReturnsAsync(new FluentValidation.Results.ValidationResult());
            _repository.Setup(r => r.ListarEventoByResponsavel(It.IsAny<int>())).ReturnsAsync(new Domain.Entities.Evento
            {
                Id = 1,
                Nome = "Evento Existente",
                DataEvento = DateTime.Now.AddDays(10),
                ResponsavelEventoId = 1,
                Categoria = "Categoria Existente",
                Capacidade = 50,
                Thumbnail = new List<string> { "thumbnail.jpg" },
                HoraEvento = "18:00",
                Descricao = "Descrição do evento existente"
            });
            _repository.Setup(r => r.CriarEvento(It.IsAny<Domain.Entities.Evento>())).ReturnsAsync((Domain.Entities.Evento e) => e);
            _repository.Setup(r => r.SaveChangesAsync()).Returns(Task.FromResult(true));
            //Act
            var result = await _service.CriarEvento(request);
            //Assert
            result.IsSuccess.Should().BeTrue();
            _validatorRequest.Verify(v => v.ValidateAsync(request, default), Times.Once);
            _repository.Verify(r => r.ListarEventoByResponsavel(request.ResponsavelEventoId), Times.Once);
            _repository.Verify(r => r.CriarEvento(It.IsAny<Domain.Entities.Evento>()), Times.Once);
            _repository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Deve_Falhar_Criar_Evento_Quando_Responsavel_Nao_Existir()
        {
            //Arrange
            var request = new EventoRequest
            {
                Nome = "Evento de Teste",
                Descricao = "Descrição do evento de teste",
                DataEvento = DateTime.Now.AddDays(10),
                Categoria = "Categoria de Teste",
                ResponsavelEventoId = 1,
                Capacidade = 100,
                Thumbnail = new List<IFormFile> { new FormFile(new MemoryStream(), 0, 0, "Thumbnail", "thumbnail.jpg") },
                HoraEvento = "18:00"
            };

            _validatorRequest.Setup(v => v.ValidateAsync(request, default)).ReturnsAsync(new FluentValidation.Results.ValidationResult());
            _repository.Setup(r => r.ListarEventoByResponsavel(It.IsAny<int>())).ReturnsAsync((Domain.Entities.Evento)null);
            //Act
            var result = await _service.CriarEvento(request);
            //Assert
            result.IsFailure.Should().BeTrue();
            result.Message.Should().Be("Responsável não encontrado.");
            _validatorRequest.Verify(v => v.ValidateAsync(request, default), Times.Once);
            _repository.Verify(r => r.ListarEventoByResponsavel(request.ResponsavelEventoId), Times.Once);
            _repository.Verify(r => r.CriarEvento(It.IsAny<Domain.Entities.Evento>()), Times.Never);
            _repository.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task Deve_Falhar_Criar_Evento_Quando_Validator_Falhar()
        {
            //Arrange
            var request = new EventoRequest
            {
                Nome = "Evento de Teste",
                Descricao = "Descrição do evento de teste",
                DataEvento = DateTime.Now.AddDays(10),
                Categoria = "Categoria de Teste",
                ResponsavelEventoId = 1,
                Capacidade = 100,
                Thumbnail = new List<IFormFile> { new FormFile(new MemoryStream(), 0, 0, "Thumbnail", "thumbnail.jpg") },
                HoraEvento = "18:00"
            };

            _validatorRequest.Setup(v => v.ValidateAsync(request, default)).ReturnsAsync(new FluentValidation.Results.ValidationResult(new[]
            {
                    new FluentValidation.Results.ValidationFailure("Nome", "O campo 'Nome' é obrigatório.")
            }));
            //Act
            var result = await _service.CriarEvento(request);
            //Assert
            result.IsFailure.Should().BeTrue();
            result.Message.Should().Be("Dados inválidos");
            _validatorRequest.Verify(v => v.ValidateAsync(request, default), Times.Once);
            _repository.Verify(r => r.ListarEventoByResponsavel(It.IsAny<int>()), Times.Never);
            _repository.Verify(r => r.CriarEvento(It.IsAny<Domain.Entities.Evento>()), Times.Never);
            _repository.Verify(r => r.SaveChangesAsync(), Times.Never);
        }
    }
    public class AtualizarEvento : EventoServiceTest
    {
        [Fact]
        public async Task Deve_Atualizar_Evento_Com_Sucesso()
        {
            //Arrange
            var request = new EventoUpdate
            {

                Nome = "Evento de Teste Atualizado",
                Descricao = "Descrição do evento de teste atualizado",
                DataEvento = DateTime.Now.AddDays(15),
                Categoria = "Categoria de Teste Atualizada",
                ResponsavelEventoId = 1,
                Capacidade = 150,
                Thumbnail = new List<IFormFile> { new FormFile(new MemoryStream(), 0, 0, "Thumbnail", "thumbnail.jpg") },
                HoraEvento = "19:00"
            };

            _validatorUpdate.Setup(v => v.ValidateAsync(request, default)).ReturnsAsync(new FluentValidation.Results.ValidationResult());
            _repository.Setup(r => r.ListarEventoById(1)).ReturnsAsync(new Domain.Entities.Evento
            {
                Id = 1,
                Nome = "Evento Existente",
                DataEvento = DateTime.Now.AddDays(10),
                ResponsavelEventoId = 1,
                Categoria = "Categoria Existente",
                Capacidade = 50,
                Thumbnail = new List<string> { "thumbnail.jpg" },
                HoraEvento = "18:00",
                Descricao = "Descrição do evento existente"
            });
            _repository.Setup(r => r.ListarEventoByResponsavel(request.ResponsavelEventoId.Value)).ReturnsAsync(new Domain.Entities.Evento
            {
                Id = 2,
                Nome = "Outro Evento Existente",
                DataEvento = DateTime.Now.AddDays(20),
                ResponsavelEventoId = 1,
                Categoria = "Categoria Existente",
                Capacidade = 100,
                Thumbnail = new List<string> { "thumbnail.jpg" },
                HoraEvento = "20:00",
                Descricao = "Descrição do outro evento existente"
            });
            _repository.Setup(r => r.AtualizarEvento(It.IsAny<Domain.Entities.Evento>())).ReturnsAsync((Domain.Entities.Evento e) => e);
            _repository.Setup(r => r.SaveChangesAsync()).Returns(Task.FromResult(true));
            //Act
            var result = await _service.AtualizarEvento(1, request);
            //Assert
            result.IsSuccess.Should().BeTrue();
            _validatorUpdate.Verify(v => v.ValidateAsync(request, default), Times.Once);
            _repository.Verify(r => r.ListarEventoById(1), Times.Once);
            _repository.Verify(r => r.AtualizarEvento(It.IsAny<Domain.Entities.Evento>()), Times.Once);
            _repository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Deve_Falhar_Atualizar_Evento_Quando_Evento_Nao_Existir()
        {
            //Arrange
            var request = new EventoUpdate
            {

                Nome = "Evento de Teste Atualizado",
                Descricao = "Descrição do evento de teste atualizado",
                DataEvento = DateTime.Now.AddDays(15),
                Categoria = "Categoria de Teste Atualizada",
                ResponsavelEventoId = 1,
                Capacidade = 150,
                Thumbnail = new List<IFormFile> { new FormFile(new MemoryStream(), 0, 0, "Thumbnail", "thumbnail.jpg") },
                HoraEvento = "19:00"
            };

            _validatorUpdate.Setup(v => v.ValidateAsync(request, default)).ReturnsAsync(new FluentValidation.Results.ValidationResult());
            _repository.Setup(r => r.ListarEventoById(1)).ReturnsAsync((Domain.Entities.Evento)null);
            //Act
            var result = await _service.AtualizarEvento(1, request);
            //Assert
            result.IsFailure.Should().BeTrue();
            result.Message.Should().Be("Evento não encontrado");
            _validatorUpdate.Verify(v => v.ValidateAsync(request, default), Times.Once);
            _repository.Verify(r => r.ListarEventoById(1), Times.Once);
            _repository.Verify(r => r.AtualizarEvento(It.IsAny<Domain.Entities.Evento>()), Times.Never);
            _repository.Verify(r => r.SaveChangesAsync(), Times.Never);
        }
        [Fact]
        public async Task Deve_Falhar_Atualizar_Evento_Quando_Responsavel_Nao_Existir()
        {
            //Arrange
            var request = new EventoUpdate
            {

                Nome = "Evento de Teste Atualizado",
                Descricao = "Descrição do evento de teste atualizado",
                DataEvento = DateTime.Now.AddDays(15),
                Categoria = "Categoria de Teste Atualizada",
                ResponsavelEventoId = 1,
                Capacidade = 150,
                Thumbnail = new List<IFormFile> { new FormFile(new MemoryStream(), 0, 0, "Thumbnail", "thumbnail.jpg") },
                HoraEvento = "19:00"
            };

            _validatorUpdate.Setup(v => v.ValidateAsync(request, default)).ReturnsAsync(new FluentValidation.Results.ValidationResult());
            _repository.Setup(r => r.ListarEventoById(1)).ReturnsAsync(new Domain.Entities.Evento
            {
                Id = 1,
                Nome = "Evento Existente",
                DataEvento = DateTime.Now.AddDays(10),
                ResponsavelEventoId = 1,
                Categoria = "Categoria Existente",
                Capacidade = 50,
                Thumbnail = new List<string> { "thumbnail.jpg" },
                HoraEvento = "18:00",
                Descricao = "Descrição do evento existente"
            });
            _repository.Setup(r => r.ListarEventoByResponsavel(request.ResponsavelEventoId.Value)).ReturnsAsync((Domain.Entities.Evento)null);
            //Act
            var result = await _service.AtualizarEvento(1, request);
            //Assert
            result.IsFailure.Should().BeTrue();
            result.Message.Should().Be("Responsável não encontrado.");
            _validatorUpdate.Verify(v => v.ValidateAsync(request, default), Times.Once);
            _repository.Verify(r => r.ListarEventoById(1), Times.Once);
            _repository.Verify(r => r.ListarEventoByResponsavel(request.ResponsavelEventoId.Value), Times.Once);
            _repository.Verify(r => r.AtualizarEvento(It.IsAny<Domain.Entities.Evento>()), Times.Never);
            _repository.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task Deve_Falhar_Atualizar_Evento_Quando_Validator_Falhar()
        {
            //Arrange
            var request = new EventoUpdate
            {

                Nome = "",
                Descricao = "Descrição do evento de teste atualizado",
                DataEvento = DateTime.Now.AddDays(15),
                Categoria = "Categoria de Teste Atualizada",
                ResponsavelEventoId = 1,
                Capacidade = 150,
                Thumbnail = new List<IFormFile> { new FormFile(new MemoryStream(), 0, 0, "Thumbnail", "thumbnail.jpg") },
                HoraEvento = "19:00"
            };

            _validatorUpdate.Setup(v => v.ValidateAsync(request, default)).ReturnsAsync(new FluentValidation.Results.ValidationResult(new[]
            {
                    new FluentValidation.Results.ValidationFailure("Nome", "O campo 'Nome' é obrigatório.")
            }));
            //Act
            var result = await _service.AtualizarEvento(1, request);
            //Assert
            result.IsFailure.Should().BeTrue();
            result.Message.Should().Be("Dados inválidos");
            _validatorUpdate.Verify(v => v.ValidateAsync(request, default), Times.Once);
            _repository.Verify(r => r.ListarEventoById(It.IsAny<int>()), Times.Never);
            _repository.Verify(r => r.AtualizarEvento(It.IsAny<Domain.Entities.Evento>()), Times.Never);
            _repository.Verify(r => r.SaveChangesAsync(), Times.Never);
        }
    }
    public class DeletarEvento : EventoServiceTest
    {
        [Fact]
        public async Task Deve_Deletar_Evento_Com_Sucesso()
        {
            //Arrange
            _repository.Setup(r => r.ListarEventoById(1)).ReturnsAsync(new Domain.Entities.Evento
            {
                Id = 1,
                Nome = "Evento Existente",
                DataEvento = DateTime.Now.AddDays(10),
                ResponsavelEventoId = 1,
                Categoria = "Categoria Existente",
                Capacidade = 50,
                Thumbnail = new List<string> { "thumbnail.jpg" },
                HoraEvento = "18:00",
                Descricao = "Descrição do evento existente"
            });
            _repository.Setup(r => r.DeletarEvento(It.IsAny<Domain.Entities.Evento>())).ReturnsAsync(true);
            _repository.Setup(r => r.SaveChangesAsync()).Returns(Task.FromResult(true));
            //Act
            var result = await _service.DeletarEvento(1);
            //Assert
            result.IsSuccess.Should().BeTrue();
            _repository.Verify(r => r.ListarEventoById(1), Times.Once);
            _repository.Verify(r => r.DeletarEvento(It.IsAny<Domain.Entities.Evento>()), Times.Once);
            _repository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Deve_Falhar_Deletar_Evento_Quando_Evento_Nao_Existir()
        {
            //Arrange
            _repository.Setup(r => r.ListarEventoById(1)).ReturnsAsync((Domain.Entities.Evento)null);
            //Act
            var result = await _service.DeletarEvento(1);
            //Assert
            result.IsFailure.Should().BeTrue();
            result.Message.Should().Be("Evento não encontrado");
            _repository.Verify(r => r.ListarEventoById(1), Times.Once);
            _repository.Verify(r => r.DeletarEvento(It.IsAny<Domain.Entities.Evento>()), Times.Never);
            _repository.Verify(r => r.SaveChangesAsync(), Times.Never);
        }
    }

    public class ListarEventoById : EventoServiceTest
    {
        [Fact]
        public async Task Deve_Listar_Evento_Por_Id_Com_Sucesso()
        {
            //Arrange
            _repository.Setup(r => r.ListarEventoById(1)).ReturnsAsync(new Domain.Entities.Evento
            {
                Id = 1,
                Nome = "Evento Existente",
                DataEvento = DateTime.Now.AddDays(10),
                ResponsavelEventoId = 1,
                Categoria = "Categoria Existente",
                Capacidade = 50,
                Thumbnail = new List<string> { "thumbnail.jpg" },
                HoraEvento = "18:00",
                Descricao = "Descrição do evento existente"
            });
            //Act
            var result = await _service.ListarEventoById(1);
            //Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Id.Should().Be(1);
            _repository.Verify(r => r.ListarEventoById(1), Times.Once);
        }

        [Fact]
        public async Task Deve_Falhar_Listar_Evento_Quando_Evento_Nao_Existir()
        {
            //Arrange
            _repository.Setup(r => r.ListarEventoById(1)).ReturnsAsync((Domain.Entities.Evento)null);
            //Act
            var result = await _service.ListarEventoById(1);
            //Assert
            result.IsFailure.Should().BeTrue();
            result.Message.Should().Be("Evento não encontrado");
            _repository.Verify(r => r.ListarEventoById(1), Times.Once);
        }
    }

}

