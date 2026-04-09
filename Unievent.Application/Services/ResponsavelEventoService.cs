using Microsoft.AspNetCore.Http;
using Unievent.Application.Common;
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
        async Task<ResultData<ResponsavelEventoResponse>> IResponsavelEventoService.AtualizarResponsavelEvento(int id, ResponsavelEventoUpdate responsavel)
        {
            var responsavelAntigo = await _repository.ListarResponsavelEventoById(id);
            if (responsavelAntigo is null)
            {
                return ResultData<ResponsavelEventoResponse>.Failure("Responsável não encontrado");
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
            return ResultData<ResponsavelEventoResponse>.Success(new ResponsavelEventoResponse
            {
                Id = id,
                Nome = responsavelAntigo.Nome,
                FotoPerfil = responsavelAntigo.FotoPerfil
            });
        }

        async Task<ResultData<ResponsavelEventoResponse>> IResponsavelEventoService.CriarResponsavelEvento(ResponsavelEventoRequest responsavel)
        {
            var imagem = await SalvarImagem(responsavel.FotoPerfil);
            var responsavelNovo = new ResponsavelEvento
            {
                Nome = responsavel.Nome,
                FotoPerfil = imagem
            };
            await _repository.CriarResponsavelEvento(responsavelNovo);
            await _repository.SaveChangesAsync();
            return ResultData<ResponsavelEventoResponse>.Success(new ResponsavelEventoResponse
            {
                Id = responsavelNovo.Id,
                Nome = responsavelNovo.Nome,
                FotoPerfil = responsavelNovo.FotoPerfil
            });
        }

        async Task<Result> IResponsavelEventoService.DeletarResponsavelEvento(int id)
        {
            var responsavel = await _repository.ListarResponsavelEventoById(id);
            if (responsavel is null)
            {
                return Result.Failure("Responsável do evento não encontrado");
            }
            await _repository.DeletarResponsavelEvento(responsavel);
            await _repository.SaveChangesAsync();
            return Result.Success("Responsável do evento deletado com sucesso");
        }

        async Task<ResultData<IEnumerable<ResponsavelEventoResponse>>> IResponsavelEventoService.ListarResponsaveisEvento()
        {
            var responsavels = await _repository.ListarResponsaveisEvento();
            return ResultData<IEnumerable<ResponsavelEventoResponse>>.Success(responsavels.Select(l => new ResponsavelEventoResponse
            {
                Id = l.Id,
                Nome = l.Nome,
                FotoPerfil = l.FotoPerfil
            }));
        }

        async Task<ResultData<ResponsavelEventoResponse>> IResponsavelEventoService.ListarResponsavelEventoById(int id)
        {
            var responsavel = await _repository.ListarResponsavelEventoById(id);
            if (responsavel == null)
            {
                return ResultData<ResponsavelEventoResponse>.Failure("Responsável do evento não encontrado");
            }
            return ResultData<ResponsavelEventoResponse>.Success(new ResponsavelEventoResponse
            {

                Id = responsavel.Id,
                Nome = responsavel.Nome,
                FotoPerfil = responsavel.FotoPerfil
            });
        }
        public async Task<string> SalvarImagem(IFormFile imagem)
        {
            var pasta = Path.Combine("wwwroot", "imagens");
            if (!Directory.Exists(pasta))
            {
                Directory.CreateDirectory(pasta);
            }


            var nomeArquivo = $"{Guid.NewGuid()}{Path.GetExtension(imagem.FileName)}";
            var caminho = Path.Combine(pasta, nomeArquivo);

            using var stream = new FileStream(caminho, FileMode.Create);
            await imagem.CopyToAsync(stream);

            return $"/imagens/{nomeArquivo}";
        }
    }
}
