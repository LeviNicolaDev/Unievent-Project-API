using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Unievent.Application.Common;
using Unievent.Application.Dtos.Evento;
using Unievent.Application.Interfaces.Repository;
using Unievent.Application.Interfaces.Services;
using Unievent.Domain.Entities;
using Unievent.Domain.Enuns;

namespace Unievent.Application.Services
{
    public class EventoService : IEventoService
    {
        private readonly ILogger<EventoService> _logger;
        private readonly IValidator<EventoRequest> _validatorRequest;
        private readonly IValidator<EventoUpdate> _validatorUpdate;
        private readonly IEventoRepository _repository;
        private readonly IResponsavelEventoRepository _responsavelEventoRepository;

        public EventoService(IEventoRepository repository, ILogger<EventoService> logger, IValidator<EventoRequest> validatorRequest, IValidator<EventoUpdate> validatorUpdate, IResponsavelEventoRepository responsavelEventoRepository)
        {
            _repository = repository;
            _logger = logger;
            _validatorRequest = validatorRequest;
            _validatorUpdate = validatorUpdate;
            _responsavelEventoRepository = responsavelEventoRepository;
        }

        async Task<Result<EventoResponse>> IEventoService.AtualizarEvento(int id, EventoUpdate update)
        {
            try
            {
                var validationResult = await _validatorUpdate.ValidateAsync(update);
                if (!validationResult.IsValid)
                {
                    _logger.LogWarning("Dados inválidos para atualização do evento com ID {EventoId}", id);
                    return Result<EventoResponse>.Failure(validationResult.Errors.Select(e => e.ErrorMessage).ToList());
                }

                _logger.LogInformation("Iniciando atualização do evento com ID {EventoId}", id);

                var evento = await _repository.ListarEventoById(id);
                if (evento is null)
                {
                    _logger.LogWarning("Evento com ID {EventoId} não encontrado para atualização", id);
                    return Result<EventoResponse>.Failure("Evento não encontrado");
                }

                if (update.ResponsavelEventoId != null)
                {
                    var responsavel = await _responsavelEventoRepository
                        .ListarResponsavelEventoById(update.ResponsavelEventoId.Value);

                    if (responsavel == null)
                    {
                        _logger.LogWarning("Responsável com ID {ResponsavelId} não encontrado.", update.ResponsavelEventoId);
                        return Result<EventoResponse>.Failure("Responsável não encontrado.");
                    }

                    evento.ResponsavelEventoId = update.ResponsavelEventoId.Value;
                }

                if (update.DataEvento.HasValue)
                {
                    var responsavelIdFinal = update.ResponsavelEventoId ?? evento.ResponsavelEventoId;
                    var responsavelJaTemEvento = await _repository.ListarEventoByResponsavel(responsavelIdFinal);

                    if (responsavelJaTemEvento != null &&
                        responsavelJaTemEvento.Id != id &&
                        responsavelJaTemEvento.DataEvento.Date == update.DataEvento.Value.Date)
                    {
                        _logger.LogWarning("O responsável com ID {ResponsavelId} já possui um evento cadastrado para esta data.", responsavelIdFinal);
                        return Result<EventoResponse>.Failure("O responsável já possui um evento cadastrado para esta data.");
                    }

                    evento.DataEvento = update.DataEvento.Value;
                }

                if (update.Capacidade.HasValue)
                    evento.Capacidade = update.Capacidade.Value;

                if (update.Categoria.HasValue)
                    evento.Categoria = update.Categoria.Value;

                if (!string.IsNullOrWhiteSpace(update.Descricao))
                    evento.Descricao = update.Descricao;

                if (!string.IsNullOrWhiteSpace(update.Nome))
                    evento.Nome = update.Nome;

                if (update.Thumbnail != null)
                {
                    var imagens = new List<string>();
                    foreach (var imagem in update.Thumbnail)
                    {
                        var img = await SalvarImagem(imagem);
                        imagens.Add(img);
                    }
                    evento.Thumbnail = imagens;
                }

                await _repository.AtualizarEvento(evento);
                await _repository.SaveChangesAsync();

                _logger.LogInformation("Evento com ID {EventoId} atualizado com sucesso", id);

                return Result<EventoResponse>.Success(new EventoResponse
                {
                    Id = id,
                    Capacidade = evento.Capacidade,
                    Categoria = evento.Categoria,
                    DataEvento = evento.DataEvento,
                    Descricao = evento.Descricao,
                    IdResponsavelEvento = evento.ResponsavelEventoId,
                    Thumbnail = evento.Thumbnail.ToList(),
                    Nome = evento.Nome
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar evento com ID {EventoId}", id);
                return Result<EventoResponse>.Failure("Erro ao atualizar evento");
            }
        }

        async Task<Result<EventoResponse>> IEventoService.CriarEvento(EventoRequest request)
        {
            try
            {
                var validationResult = await _validatorRequest.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    _logger.LogWarning("Dados inválidos para criação do evento com nome {EventoNome}", request.Nome);
                    return Result<EventoResponse>.Failure(validationResult.Errors.Select(e => e.ErrorMessage).ToList());
                }
                _logger.LogInformation("Iniciando criação de evento para responsável ID {ResponsavelId} e data {DataEvento}", request.ResponsavelEventoId, request.DataEvento);

                var responsavel = await _responsavelEventoRepository.ListarResponsavelEventoById(request.ResponsavelEventoId);


                if (responsavel == null)
                {
                    _logger.LogWarning("Responsável com ID {ResponsavelId} não encontrado.", request.ResponsavelEventoId);
                    return Result<EventoResponse>.Failure("Responsável não encontrado.");
                }

                var responsavelJaTemEvento = await _repository.ListarEventoByResponsavel(request.ResponsavelEventoId);

                if (responsavelJaTemEvento != null && responsavelJaTemEvento.DataEvento.Date == request.DataEvento.Date)
                {
                    _logger.LogWarning("O responsável com ID {ResponsavelId} já possui um evento cadastrado para esta data.", request.ResponsavelEventoId);
                    return Result<EventoResponse>.Failure("O responsável já possui um evento cadastrado para esta data.");
                }

                var imagens = new List<string>();

                foreach (var imagem in request.Thumbnail)
                {
                    var img = await SalvarImagem(imagem);
                    imagens.Add(img);
                }
                var evento = new Evento
                {
                    Capacidade = request.Capacidade,
                    Categoria = request.Categoria,
                    DataEvento = request.DataEvento,
                    Descricao = request.Descricao,
                    ResponsavelEventoId = request.ResponsavelEventoId,
                    Nome = request.Nome,
                    Thumbnail = imagens
                };
                await _repository.CriarEvento(evento);
                await _repository.SaveChangesAsync();
                _logger.LogInformation("Evento criado com sucesso com ID {EventoId}", evento.Id);

                return Result<EventoResponse>.Success(new EventoResponse
                {
                    Id = evento.Id,
                    Capacidade = evento.Capacidade,
                    Categoria = evento.Categoria,
                    DataEvento = evento.DataEvento,
                    Descricao = evento.Descricao,
                    IdResponsavelEvento = evento.ResponsavelEventoId,
                    Thumbnail = evento.Thumbnail.ToList(),
                    Nome = evento.Nome
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar evento");
                return Result<EventoResponse>.Failure("Erro ao criar evento");
            }
        }

        public async Task<string> SalvarImagem(IFormFile imagem)
        {

            var pasta = Path.Combine("wwwroot", "imagens");
            if (!Directory.Exists(pasta)) Directory.CreateDirectory(pasta);

            var nomeArquivo = $"{Guid.NewGuid()}{Path.GetExtension(imagem.FileName)}";
            var caminho = Path.Combine(pasta, nomeArquivo);

            using var stream = new FileStream(caminho, FileMode.Create);
            await imagem.CopyToAsync(stream);

            return $"/imagens/{nomeArquivo}";
        }

        async Task<Result<bool>> IEventoService.DeletarEvento(int id)
        {
            try
            {
                _logger.LogInformation("Iniciando deleção do evento com ID {EventoId}", id);
                var evento = await _repository.ListarEventoById(id);
                if (evento is null)
                {
                    _logger.LogWarning("Evento com ID {EventoId} não encontrado para deleção", id);
                    return Result<bool>.Failure("Evento não encontrado");
                }
                await _repository.DeletarEvento(evento);
                await _repository.SaveChangesAsync();
                _logger.LogInformation("Evento com ID {EventoId} deletado com sucesso", id);
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao deletar evento com ID {EventoId}", id);
                return Result<bool>.Failure("Erro ao deletar evento");
            }
        }

        async Task<Result<EventoResponse>> IEventoService.ListarEventoById(int id)
        {
            try
            {
                _logger.LogInformation("Iniciando busca do evento com ID {EventoId}", id);
                var evento = await _repository.ListarEventoById(id);
                if (evento is null)
                {
                    _logger.LogWarning("Evento com ID {EventoId} não encontrado", id);
                    return Result<EventoResponse>.Failure("Evento não encontrado");
                }
                _logger.LogInformation("Evento com ID {EventoId} encontrado com sucesso", id);
                return Result<EventoResponse>.Success(new EventoResponse
                {
                    Id = evento.Id,
                    Capacidade = evento.Capacidade,
                    Categoria = evento.Categoria,
                    DataEvento = evento.DataEvento,
                    Descricao = evento.Descricao,
                    IdResponsavelEvento = evento.ResponsavelEventoId,
                    Thumbnail = evento.Thumbnail.ToList(),
                    Nome = evento.Nome
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar evento com ID {EventoId}", id);
                return Result<EventoResponse>.Failure("Erro ao buscar evento");
            }
        }

        async Task<Result<IEnumerable<EventoResponse>>> IEventoService.ListarEventos()
        {
            try
            {
                _logger.LogInformation("Iniciando listagem de eventos");
                var eventos = await _repository.ListarEventos();
                _logger.LogInformation("Eventos listados com sucesso {EventosCount}", eventos.Count());
                return Result<IEnumerable<EventoResponse>>.Success(eventos.Select(e => new EventoResponse
                {
                    Id = e.Id,
                    Capacidade = e.Capacidade,
                    Categoria = e.Categoria,
                    DataEvento = e.DataEvento,
                    Descricao = e.Descricao,
                    Responsavel = e.ResponsavelEvento.Nome,
                    Nome = e.Nome,
                    Thumbnail = e.Thumbnail.ToList()
                }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar eventos");
                return Result<IEnumerable<EventoResponse>>.Failure("Erro ao listar eventos");
            }
        }

        async Task<Result<IList<EventoResponse>>> IEventoService.ListarEventosByCategoria(Categoria categoria)
        {
            try
            {
                _logger.LogInformation("Iniciando busca de eventos com categoria {Categoria}", categoria);
                var eventos = await _repository.ListarEventoByCategoria(categoria);
                _logger.LogInformation("Busca de eventos com categoria {Categoria} realizada com sucesso. Total de eventos encontrados: {EventosCount}", categoria, eventos.Count());
                return Result<IList<EventoResponse>>.Success(eventos.Select(e => new EventoResponse
                {
                    Id = e.Id,
                    Capacidade = e.Capacidade,
                    Categoria = e.Categoria,
                    DataEvento = e.DataEvento,
                    Descricao = e.Descricao,
                    IdResponsavelEvento = e.ResponsavelEventoId,
                    Nome = e.Nome,
                    Thumbnail = e.Thumbnail.ToList()
                }).ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar eventos com categoria {Categoria}", categoria);
                return Result<IList<EventoResponse>>.Failure("Erro ao buscar eventos por categoria");
            }
        }

        async Task<Result<EventoResponse>> IEventoService.ListarEventosByResponsavel(int responsavelId)
        {
            try
            {
                _logger.LogInformation("Iniciando busca de evento para responsável ID {ResponsavelId}", responsavelId);
                var evento = await _repository.ListarEventoByResponsavel(responsavelId);
                if (evento is null)
                {
                    _logger.LogWarning("Evento para responsável ID {ResponsavelId} não encontrado", responsavelId);
                    return Result<EventoResponse>.Failure("Evento não encontrado para o responsável informado");
                }
                _logger.LogInformation("Evento para responsável ID {ResponsavelId} encontrado com sucesso", responsavelId);
                return Result<EventoResponse>.Success(new EventoResponse
                {
                    Id = evento.Id,
                    Capacidade = evento.Capacidade,
                    Categoria = evento.Categoria,
                    DataEvento = evento.DataEvento,
                    Descricao = evento.Descricao,
                    IdResponsavelEvento = evento.ResponsavelEventoId,
                    Nome = evento.Nome,
                    Thumbnail = evento.Thumbnail.ToList()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar evento para responsável ID {ResponsavelId}", responsavelId);
                return Result<EventoResponse>.Failure("Erro ao buscar evento por responsável");
            }
        }
    }
}
