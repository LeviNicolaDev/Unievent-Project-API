using Microsoft.AspNetCore.Http;
using Unievent.Application.Dtos.Instituicao;
using Unievent.Application.Interfaces.Repository;
using Unievent.Application.Interfaces.Services;
using Elekto.BrazilianDocuments;
using Unievent.Domain.Entities;

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
    async Task<InstituicaoResponse> IInstituicaoService.AtualizarInstituicao(int id, InstituicaoUpdate update)
    {
        var instituicao = await _repository.ListarInstituicaoById(id) ?? throw new Exception("Instituicao não encontrada");
        if (update.EnderecoId.HasValue)
        {
            var endereco = await _repositoryEndereco.ListarEnderecoById(update.EnderecoId.Value) ?? throw new Exception("Endereço não encontrado para ser atualizado");
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

        if (!string.IsNullOrWhiteSpace(update.EmailLogin))
        {
            instituicao.EmailLogin = update.EmailLogin;
        }

        if (!string.IsNullOrWhiteSpace(update.Cnpj) && Cnpj.TryParse(update.Cnpj, out var cnpj))
        {

            instituicao.Cnpj = cnpj.ToString();
        }
        await _repository.AtualizarInstituicao(instituicao);
        await _repository.SaveChangesAsync();
        return new InstituicaoResponse
        {
            Id = id,
            Cnpj = instituicao.Cnpj,
            EmailLogin = instituicao.EmailLogin,
            FotoPerfil = instituicao.FotoPerfil,
            EnderecoId = instituicao.EnderecoId
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



    async Task<InstituicaoResponse> IInstituicaoService.CriarInstituicao(InstituicaoRequest request)
    {
        if (!Cnpj.TryParse(request.Cnpj, out var cnpj))
        {
            throw new Exception("Digte um CNPJ valido");
        }
        var endereco = await _repositoryEndereco.ListarEnderecoById(request.EnderecoId) ?? throw new Exception("Endereço não encontrado para ser associado à instituição");
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
        return new InstituicaoResponse
        {
            Id = instituicao.Id,
            Cnpj = instituicao.Cnpj,
            EmailLogin = instituicao.EmailLogin,
            FotoPerfil = instituicao.FotoPerfil,

            EnderecoId = instituicao.EnderecoId
        };
    }
    async Task<bool> IInstituicaoService.DeletarInstituicao(int id)
    {
        var instituicao = await _repository.ListarInstituicaoById(id) ?? throw new Exception("Instituicao não encontrada");
        await _repository.DeletarInstituicao(instituicao);
        await _repository.SaveChangesAsync();
        return true;
    }

    async Task<InstituicaoResponse> IInstituicaoService.ListarInstituicaoById(int id)
    {
        var instituicao = await _repository.ListarInstituicaoById(id) ?? throw new Exception("Instituicao não encontrada");
        return new InstituicaoResponse
        {
            Id = id,
            Cnpj = instituicao.Cnpj,
            EmailLogin = instituicao.EmailLogin,
            FotoPerfil = instituicao.FotoPerfil,

            EnderecoId = instituicao.EnderecoId
        };
    }

    async Task<IList<InstituicaoResponse>> IInstituicaoService.ListarInstituicoes()
    {
        var instituicoes = await _repository.ListarInstituicoes();
        return instituicoes.Select(instituicao => new InstituicaoResponse
        {
            Id = instituicao.Id,
            Cnpj = instituicao.Cnpj,
            EmailLogin = instituicao.EmailLogin,
            FotoPerfil = instituicao.FotoPerfil,
            EnderecoId = instituicao.EnderecoId
        }).ToList();
    }
}
