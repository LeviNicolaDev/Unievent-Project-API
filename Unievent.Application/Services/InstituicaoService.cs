using Microsoft.AspNetCore.Http;
using Unievent.Application.Dtos.Instituicao;
using Unievent.Application.Interfaces.Repository;
using Unievent.Application.Interfaces.Services;
using Elekto.BrazilianDocuments;
using Unievent.Domain.Entities;
using Unievent.Application.Common;

namespace Unievent.Application.Services;

public class InstituicaoService : IInstituicaoService
{
    private readonly IInstituicaoRepository _repository;
    private readonly IEnderecoRepository _repositoryEndereco;
    public InstituicaoService(IInstituicaoRepository repository, IEnderecoRepository repositoryEndereco)
    {
        _repository = repository;
        _repositoryEndereco = repositoryEndereco;
    }
    async Task<ResultData<InstituicaoResponse>> IInstituicaoService.AtualizarInstituicao(int id, InstituicaoUpdate update)
    {
        var emailExistente = await _repository.ListarInstituicaoByEmail(update.EmailLogin);
        var instituicao = await _repository.ListarInstituicaoById(id);
        if (instituicao is null)
        {
            return ResultData<InstituicaoResponse>.Failure("Instituição não encontrada");
        }

        if (update.EnderecoId.HasValue)
        {
            var endereco = await _repositoryEndereco.ListarEnderecoById(update.EnderecoId.Value);
            if (endereco is null)
            {
                return ResultData<InstituicaoResponse>.Failure("Endereço não encontrado para ser associado à instituição");
            }
            instituicao.EnderecoId = endereco.Id;
        }

        if (update.FotoPerfil != null)
        {
            var imagem = await SalvarImagem(update.FotoPerfil);

            instituicao.FotoPerfil = imagem;
        }

        if (!string.IsNullOrWhiteSpace(update.SenhaLogin))
        {
            var senha = BCrypt.Net.BCrypt.HashPassword(update.SenhaLogin);
            instituicao.SenhaLogin = senha;
        }

        if (!string.IsNullOrWhiteSpace(update.EmailLogin) && emailExistente == null)
        {
            instituicao.EmailLogin = update.EmailLogin;
        }
        else if (!string.IsNullOrWhiteSpace(update.EmailLogin) && emailExistente != null)
        {
            return ResultData<InstituicaoResponse>.Failure("Email já cadastrado para outra instituição");
        }

        if (!string.IsNullOrWhiteSpace(update.Cnpj) && Cnpj.TryParse(update.Cnpj, out var cnpj))
        {

            instituicao.Cnpj = cnpj.ToString();
        }
        await _repository.AtualizarInstituicao(instituicao);
        await _repository.SaveChangesAsync();
        return ResultData<InstituicaoResponse>.Success(new InstituicaoResponse
        {
            Id = id,
            Cnpj = instituicao.Cnpj,
            EmailLogin = instituicao.EmailLogin,
            FotoPerfil = instituicao.FotoPerfil,
            EnderecoId = instituicao.EnderecoId
        });

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



    async Task<ResultData<InstituicaoResponse>> IInstituicaoService.CriarInstituicao(InstituicaoRequest request)
    {
        if (!Cnpj.TryParse(request.Cnpj, out var cnpj))
        {
            return ResultData<InstituicaoResponse>.Failure("Digite um CNPJ válido");
        }
        var endereco = await _repositoryEndereco.ListarEnderecoById(request.EnderecoId);
        if (endereco is null)
        {
            return ResultData<InstituicaoResponse>.Failure("Endereço não encontrado para ser associado à instituição");
        }
        var emailExistente = await _repository.ListarInstituicaoByEmail(request.EmailLogin);
        if (emailExistente != null)
        {
            return ResultData<InstituicaoResponse>.Failure("Email já cadastrado para outra instituição");
        }
        var cnpjValido = cnpj.ToString();
        var imagem = await SalvarImagem(request.FotoPerfil);
        var senha = BCrypt.Net.BCrypt.HashPassword(request.SenhaLogin);
        var instituicao = new Instituicao
        {
            Cnpj = cnpjValido,
            EmailLogin = request.EmailLogin,
            EnderecoId = endereco.Id,
            FotoPerfil = imagem,
            SenhaLogin = senha
        };
        await _repository.CriarInstituicao(instituicao);
        await _repository.SaveChangesAsync();
        return ResultData<InstituicaoResponse>.Success(new InstituicaoResponse
        {
            Id = instituicao.Id,
            Cnpj = instituicao.Cnpj,
            EmailLogin = instituicao.EmailLogin,
            FotoPerfil = instituicao.FotoPerfil,

            EnderecoId = instituicao.EnderecoId
        });
    }
    async Task<Result> IInstituicaoService.DeletarInstituicao(int id)
    {
        var instituicao = await _repository.ListarInstituicaoById(id) ?? throw new Exception("Instituicao não encontrada");
        await _repository.DeletarInstituicao(instituicao);
        await _repository.SaveChangesAsync();
        return Result.Success("Instituicao deletada com sucesso");
    }

    async Task<ResultData<InstituicaoResponse>> IInstituicaoService.ListarInstituicaoById(int id)
    {
        var instituicao = await _repository.ListarInstituicaoById(id);
        if (instituicao is null)
        {
            return ResultData<InstituicaoResponse>.Failure("Instituição não encontrada");
        }
        return ResultData<InstituicaoResponse>.Success(new InstituicaoResponse
        {
            Id = id,
            Cnpj = instituicao.Cnpj,
            EmailLogin = instituicao.EmailLogin,
            FotoPerfil = instituicao.FotoPerfil,

            EnderecoId = instituicao.EnderecoId
        });
    }

    async Task<ResultData<IEnumerable<InstituicaoResponse>>> IInstituicaoService.ListarInstituicoes()
    {
        var instituicoes = await _repository.ListarInstituicoes();
        return ResultData<IEnumerable<InstituicaoResponse>>.Success(instituicoes.Select(instituicao => new InstituicaoResponse
        {
            Id = instituicao.Id,
            Cnpj = instituicao.Cnpj,
            EmailLogin = instituicao.EmailLogin,
            FotoPerfil = instituicao.FotoPerfil,
            EnderecoId = instituicao.EnderecoId
        }));
    }
}
