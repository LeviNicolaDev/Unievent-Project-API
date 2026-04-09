using Microsoft.AspNetCore.Http;
using Unievent.Application.Common;
using Unievent.Application.Dtos.Evento;
using Unievent.Application.Interfaces.Repository;
using Unievent.Application.Interfaces.Services;
using Unievent.Domain.Entities;

namespace Unievent.Application.Services
{
    public class EventoService : IEventoService
    {

        private readonly IEventoRepository _repository;

        public EventoService(IEventoRepository repository)
        {
            _repository = repository;

        }

        async Task<ResultData<EventoResponse>> IEventoService.AtualizarEvento(int id, EventoUpdate update)
        {
            IList<string> imagens = new List<string>(); // Lista para armazenar os caminhos das imagens salvas

            var evento = await _repository.ListarEventoById(id);
            if (evento is null)
            {
                return ResultData<EventoResponse>.Failure("Evento não encontrado");
            }
            if (update.Capacidade.HasValue)
            {
                evento.Capacidade = update.Capacidade.Value;
            }
            //QUANDO CATEGORIA ERA UM ENUM
            /*if (!string.IsNullOrWhiteSpace(update.Categoria))
            {
                if (!Enum.TryParse<CategoriaEvento>(update.Categoria, ignoreCase: true, out var categoriaEnum))
                    throw new Exception($"Categoria '{update.Categoria}' inválida.");

                evento.Categoria = categoriaEnum;
            }*/

            if (!string.IsNullOrWhiteSpace(update.Categoria))
            {
                evento.Categoria = update.Categoria;
            }

            if (!string.IsNullOrWhiteSpace(update.DataEvento.ToString()))
            {
                evento.DataEvento = update.DataEvento.Value;
            }
            if (!string.IsNullOrWhiteSpace(update.Descricao))
            {
                evento.Descricao = update.Descricao;
            }
            if (update.ResponsavelEventoId != null)
            {
                var responsavel = await _repository.ListarEventoByResponsavel((int)update.ResponsavelEventoId);
                if (responsavel != null && responsavel.DataEvento == evento.DataEvento)
                {
                    return ResultData<EventoResponse>.Failure("O responsável já possui um evento cadastrado para esta data.");
                }
                else if (responsavel == null)
                {
                    return ResultData<EventoResponse>.Failure("Responsável não encontrado.");
                }
                evento.ResponsavelEventoId = (int)update.ResponsavelEventoId;
            }
            if (update.Thumbnail != null)
            {
                foreach (var imagem in update.Thumbnail)
                {
                    var img = await SalvarImagem(imagem);
                    imagens.Add(img);
                }
                evento.Thumbnail = imagens;
            }
            if (!string.IsNullOrWhiteSpace(update.Nome))
            {
                evento.Nome = update.Nome;
            }

            await _repository.AtualizarEvento(evento);
            await _repository.SaveChangesAsync();
            return ResultData<EventoResponse>.Success(new EventoResponse
            {
                Id = id,
                Capacidade = evento.Capacidade,
                Categoria = evento.Categoria,
                DataEvento = evento.DataEvento,
                HoraEvento = evento.HoraEvento,
                Descricao = evento.Descricao,
                IdResponsavelEvento = evento.ResponsavelEventoId,
                Thumbnail = evento.Thumbnail.ToList(),
                Nome = evento.Nome
            });
        }

        async Task<ResultData<EventoResponse>> IEventoService.CriarEvento(EventoRequest request)
        {
            var imagens = new List<string>();

            foreach (var imagem in request.Thumbnail)
            {
                var img = await SalvarImagem(imagem);
                imagens.Add(img);
            }
            var responsavel = await _repository.ListarEventoByResponsavel(request.ResponsavelEventoId);
            if (responsavel != null && responsavel.DataEvento == request.DataEvento)
            {
                return ResultData<EventoResponse>.Failure("O responsável já possui um evento cadastrado para esta data.");
            }



            var evento = new Evento
            {
                Capacidade = request.Capacidade,
                Categoria = request.Categoria,
                HoraEvento = request.HoraEvento,
                DataEvento = request.DataEvento,
                Descricao = request.Descricao,
                ResponsavelEventoId = request.ResponsavelEventoId,
                Nome = request.Nome,
                Thumbnail = imagens
            };
            await _repository.CriarEvento(evento);
            await _repository.SaveChangesAsync();
            return ResultData<EventoResponse>.Success(new EventoResponse
            {
                Id = evento.Id,
                Capacidade = evento.Capacidade,
                Categoria = evento.Categoria,
                DataEvento = evento.DataEvento,
                HoraEvento = evento.HoraEvento,
                Descricao = evento.Descricao,
                IdResponsavelEvento = evento.ResponsavelEventoId,
                Thumbnail = evento.Thumbnail.ToList(),
                Nome = evento.Nome
            });
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

        async Task<Result> IEventoService.DeletarEvento(int id)
        {
            var evento = await _repository.ListarEventoById(id);
            if (evento is null)
            {
                return Result.Failure("Evento não encontrado");
            }
            await _repository.DeletarEvento(evento);
            await _repository.SaveChangesAsync();
            return Result.Success("Evento deletado com sucesso");
        }

        async Task<ResultData<EventoResponse>> IEventoService.ListarEventoById(int id)
        {
            var evento = await _repository.ListarEventoById(id);
            if (evento is null)
            {
                return ResultData<EventoResponse>.Failure("Evento não encontrado");
            }
            return ResultData<EventoResponse>.Success(new EventoResponse
            {
                Id = evento.Id,
                Capacidade = evento.Capacidade,
                Categoria = evento.Categoria,
                DataEvento = evento.DataEvento,
                HoraEvento = evento.HoraEvento,
                Descricao = evento.Descricao,
                IdResponsavelEvento = evento.ResponsavelEventoId,
                Thumbnail = evento.Thumbnail.ToList(),
                Nome = evento.Nome
            });
        }

        async Task<ResultData<IEnumerable<EventoResponse>>> IEventoService.ListarEventos()
        {
            var eventos = await _repository.ListarEventos();
            return ResultData<IEnumerable<EventoResponse>>.Success(eventos.Select(e => new EventoResponse
            {
                Id = e.Id,
                Capacidade = e.Capacidade,
                Categoria = e.Categoria,
                DataEvento = e.DataEvento,
                HoraEvento = e.HoraEvento,
                Descricao = e.Descricao,
                IdResponsavelEvento = e.ResponsavelEventoId,
                Nome = e.Nome,
                Thumbnail = e.Thumbnail.ToList()
            }));
        }

        async Task<ResultData<IEnumerable<EventoResponse>>> IEventoService.ListarEventosByCategoria(string categoria)
        {
            var eventos = await _repository.ListarEventoByCategoria(categoria);

            return ResultData<IEnumerable<EventoResponse>>.Success(eventos.Select(e => new EventoResponse
            {
                Id = e.Id,
                Capacidade = e.Capacidade,
                Categoria = e.Categoria,
                DataEvento = e.DataEvento,
                HoraEvento = e.HoraEvento,
                Descricao = e.Descricao,
                IdResponsavelEvento = e.ResponsavelEventoId,
                Nome = e.Nome,
                Thumbnail = e.Thumbnail.ToList()
            }));
        }

        async Task<ResultData<EventoResponse>> IEventoService.ListarEventosByResponsavel(int responsavelId)
        {
            var evento = await _repository.ListarEventoByResponsavel(responsavelId);
            if (evento is null)
            {
                return ResultData<EventoResponse>.Failure("Evento não encontrado para o responsável informado");
            }
            return ResultData<EventoResponse>.Success(new EventoResponse
            {
                Id = evento.Id,
                Capacidade = evento.Capacidade,
                Categoria = evento.Categoria,
                DataEvento = evento.DataEvento,
                HoraEvento = evento.HoraEvento,
                Descricao = evento.Descricao,
                IdResponsavelEvento = evento.ResponsavelEventoId,
                Nome = evento.Nome,
                Thumbnail = evento.Thumbnail.ToList()
            });
        }
    }
}
