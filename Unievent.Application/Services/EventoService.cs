using Microsoft.AspNetCore.Http;
using Unievent.Application.Dtos.Evento;
using Unievent.Application.Interfaces.Repository;
using Unievent.Application.Interfaces.Services;
using Unievent.Domain.Entities;

namespace Unievent.Application.Services
{
    public class EventoService : IEventoService
    {

        private readonly IEventoRepository _repository;
        private readonly IResponsavelEventoRepository _repositoryResponsavel;
        public EventoService(IEventoRepository repository, IResponsavelEventoRepository responsavelEventoRepository)
        {
            _repository = repository;
            _repositoryResponsavel = responsavelEventoRepository;
        }

        async Task<EventoResponse> IEventoService.AtualizarEvento(int id, EventoUpdate update)
        {
            var evento = await _repository.ListarEventoById(id) ?? throw new Exception("Evento não encontrado");
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
            if (update.IdResponsavelEvento != null)
            {
                var responsavel = await _repositoryResponsavel.ListarResponsavelEventoById((int)update.IdResponsavelEvento);
                if (responsavel is null)
                {
                    throw new Exception("Responsavel do evento não encontrado para ser atualizado");
                }
                evento.IdResponsavelEvento = (int)update.IdResponsavelEvento;
            }
            if (update.Thumbnail != null)
            {
                var imagens = await SalvarImagem(update.Thumbnail);
                evento.Thumbnail = imagens;
            }
            if (!string.IsNullOrWhiteSpace(update.Nome))
            {
                evento.Nome = update.Nome;
            }

            await _repository.AtualizarEvento(evento);
            await _repository.SaveChangesAsync();
            return new EventoResponse
            {
                Id = id,
                Capacidade = evento.Capacidade,
                Categoria = evento.Categoria,
                DataEvento = evento.DataEvento,
                HoraEvento = evento.HoraEvento,
                Descricao = evento.Descricao,
                IdResponsavelEvento = evento.IdResponsavelEvento,
                Thumbnail = evento.Thumbnail.ToList(),
                Nome = evento.Nome
            };
        }

        async Task<EventoResponse> IEventoService.CriarEvento(EventoRequest request)
        {
            var imagens = new List<string>();
            // QUANDO CATEGORIA ERA UM ENUM
            // var categoria = Enum.TryParse(request.Categoria, out CategoriaEvento categoriaEvento) ? categoriaEvento : CategoriaEvento.palestra;
            foreach (var imagem in request.Thumbnail)
            {
                var img = await SalvarImagem(imagem);
            }
            ;
            var evento = new Evento
            {
                Capacidade = request.Capacidade,
                Categoria = request.Categoria,
                HoraEvento = request.HoraEvento,
                DataEvento = request.DataEvento,
                Descricao = request.Descricao,
                IdResponsavelEvento = request.IdResponsavelEvento,
                Nome = request.Nome,
                Thumbnail = imagens
            };
            await _repository.CriarEvento(evento);
            await _repository.SaveChangesAsync();
            return new EventoResponse
            {
                Id = evento.Id,
                Capacidade = evento.Capacidade,
                Categoria = evento.Categoria,
                DataEvento = evento.DataEvento,
                HoraEvento = evento.HoraEvento,
                Descricao = evento.Descricao,
                IdResponsavelEvento = evento.IdResponsavelEvento,
                Thumbnail = evento.Thumbnail.ToList(),
                Nome = evento.Nome
            };
        }

        public async Task<string> SalvarImagem(IFormFile imagem)
        {
            var pasta = Path.Combine("wwwroot", "imagens");
            Directory.CreateDirectory(pasta);

            var nomeArquivo = $"{Guid.NewGuid()}{Path.GetExtension(imagem.FileName)}";
            var caminho = Path.Combine(pasta, nomeArquivo);

            using var stream = new FileStream(caminho, FileMode.Create);
            await imagem.CopyToAsync(stream);

            return $"/imagens/{nomeArquivo}";
        }

        async Task<bool> IEventoService.DeletarEvento(int id)
        {
            var evento = await _repository.ListarEventoById(id) ?? throw new Exception("Evento não encontrado");
            await _repository.DeletarEvento(evento);
            await _repository.SaveChangesAsync();
            return true;
        }

        async Task<EventoResponse> IEventoService.ListarEventoById(int id)
        {

            var evento = await _repository.ListarEventoById(id) ?? throw new Exception("Evento não encontrado");
            return new EventoResponse
            {
                Id = evento.Id,
                Capacidade = evento.Capacidade,
                Categoria = evento.Categoria,
                DataEvento = evento.DataEvento,
                HoraEvento = evento.HoraEvento,
                Descricao = evento.Descricao,
                IdResponsavelEvento = evento.IdResponsavelEvento,
                Thumbnail = evento.Thumbnail.ToList(),
                Nome = evento.Nome
            };
        }

        async Task<IEnumerable<EventoResponse>> ListarEventos()
        {
            var eventos = await _repository.ListarEventos();
            return eventos.Select(e => new EventoResponse
            {
                Id = e.Id,
                Capacidade = e.Capacidade,
                Categoria = e.Categoria,
                DataEvento = e.DataEvento,
                HoraEvento = e.HoraEvento,
                Descricao = e.Descricao,
                IdResponsavelEvento = e.IdResponsavelEvento,
                Nome = e.Nome,
                Thumbnail = e.Thumbnail.ToList()
            });
        }
    }
}
