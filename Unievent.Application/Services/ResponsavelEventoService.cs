using Microsoft.AspNetCore.Http;
using Unievent.Application.Dtos.ResponsavelEvento;
using Unievent.Application.Interfaces.Repository;
using Unievent.Application.Interfaces.Services;
using Unievent.Domain.Entities;

namespace Unievent.Application.Services
{
    public class ResponsavelEventoService : IResponsavelEventoService
    {
        private readonly IResponsavelEventoRepository _repository;
        public ResponsavelEventoService(IResponsavelEventoRepository repository)
        {
            _repository = repository;
        }
        async Task<ResponsavelEventoResponse> IResponsavelEventoService.AtualizarResponsavelEvento(int id, ResponsavelEventoUpdate responsavel)
        {
            var responsavelAntigo = await _repository.ListarResponsavelEventoById(id);
            if (responsavelAntigo is null)
            {
                throw new Exception("Responsavel não encontrado");
            }
            if (!string.IsNullOrWhiteSpace(responsavel.Nome))
            {
                responsavelAntigo.Nome = responsavel.Nome;
            }
            if (responsavel.FotoPerfil != null)
            {
                var imagem = await SalvarImagem(responsavel.FotoPerfil);
                responsavelAntigo.FotoPerfil = imagem;
            }
            return new ResponsavelEventoResponse
            {
                Id = id,
                Nome = responsavelAntigo.Nome,
                FotoPerfil = responsavelAntigo.FotoPerfil
            };
        }

        async Task<ResponsavelEventoResponse> IResponsavelEventoService.CriarResponsavelEvento(ResponsavelEventoRequest responsavel)
        {
            var imagem = await SalvarImagem(responsavel.FotoPerfil);
            var responsavelNovo = new ResponsavelEvento
            {
                Nome = responsavel.Nome,
                FotoPerfil = imagem
            };
            await _repository.CriarResponsavelEvento(responsavelNovo);
            await _repository.SaveChangesAsync();
            return new ResponsavelEventoResponse
            {
                Id = responsavelNovo.Id,
                Nome = responsavelNovo.Nome,
                FotoPerfil = responsavelNovo.FotoPerfil
            };
        }

        async Task<bool> IResponsavelEventoService.DeletarResponsavelEvento(int id)
        {
            var responsavel = await _repository.ListarResponsavelEventoById(id);
            if (responsavel is null)
            {
                throw new Exception("Responsavel do evento não encontrado");
            }
            _repository.DeletarResponsavelEvento(responsavel);
            await _repository.SaveChangesAsync();
            return true;
        }

        async Task<IEnumerable<ResponsavelEventoResponse>> IResponsavelEventoService.ListarResponsaveisEvento()
        {
            var responsavels = await _repository.ListarResponsaveisEvento();
            return responsavels.Select(l => new ResponsavelEventoResponse
            {
                Id = l.Id,
                Nome = l.Nome,
                FotoPerfil = l.FotoPerfil
            });
        }

        async Task<ResponsavelEventoResponse> IResponsavelEventoService.ListarResponsavelEventoById(int id)
        {
            var responsavel = await _repository.ListarResponsavelEventoById(id);
            if (responsavel == null)
            {
                throw new Exception("Resposavel do evento não encontrado");
            }
            return new ResponsavelEventoResponse
            {

                Id = responsavel.Id,
                Nome = responsavel.Nome,
                FotoPerfil = responsavel.FotoPerfil
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
    }
}
