using Microsoft.AspNetCore.Http;
using Unievent.Application.Dtos.Instituicao;
using Unievent.Application.Interfaces.Repository;
using Unievent.Application.Interfaces.Services;
using Elekto.BrazilianDocuments;
using Unievent.Domain.Entities;
using Unievent.Application.Common;
using Microsoft.Extensions.Logging;
using FluentValidation;

namespace Unievent.Application.Services;

public class InstituicaoService : IInstituicaoService
{
    private readonly IInstituicaoRepository _repository;
    private readonly IValidator<InstituicaoRequest> _requestValidator;
    private readonly IValidator<InstituicaoUpdate> _updateValidator;
    private readonly ILogger<InstituicaoService> _logger;
    public InstituicaoService(IInstituicaoRepository repository, ILogger<InstituicaoService> logger,
    IValidator<InstituicaoRequest> requestValidator, IValidator<InstituicaoUpdate> updateValidator)
    {
        _repository = repository;
        _logger = logger;
        _requestValidator = requestValidator;
        _updateValidator = updateValidator;
    }
    async Task<Result<InstituicaoResponse>> IInstituicaoService.AtualizarInstituicao(int id, InstituicaoUpdate update)
    {
        try
        {
            var validationResult = await _updateValidator.ValidateAsync(update);
            if (!validationResult.IsValid)
            {
                _logger.LogInformation("Dados invalidos para atualização da instituição com ID {InstituicaoId}", id);
                return Result<InstituicaoResponse>.Failure(validationResult.Errors.Select(e => e.ErrorMessage).ToList());
            }
            _logger.LogInformation("Iniciando atualização da instituição com ID {InstituicaoId}", id);
            var instituicao = await _repository.ListarInstituicaoById(id);
            if (instituicao is null)
            {
                _logger.LogWarning("Instituição com ID {InstituicaoId} não encontrada para atualização", id);
                return Result<InstituicaoResponse>.Failure("Instituição não encontrada");
            }

            if (update.Nome is not null) instituicao.Nome = update.Nome;
            if (update.NomeAbreviado is not null) instituicao.NomeAbreviado = update.NomeAbreviado;
            if (update.Codigo is not null) instituicao.Codigo = update.Codigo;
            if (update.Rua is not null) instituicao.Rua = update.Rua;
            if (update.Numero is not null) instituicao.Numero = update.Numero;
            if (update.Bairro is not null) instituicao.Bairro = update.Bairro;
            if (update.Cidade is not null) instituicao.Cidade = update.Cidade;
            if (update.Estado is not null) instituicao.Estado = update.Estado;
            if (update.Cep is not null) instituicao.Cep = update.Cep;
            if (update.Telefone is not null) instituicao.Telefone = update.Telefone;
            if (update.Site is not null) instituicao.Site = update.Site;
            if (update.IsAtivo.HasValue) instituicao.IsAtivo = update.IsAtivo.Value;
            instituicao.AtualizadoEmUtc = DateTime.UtcNow;

            if (update.FotoPerfil != null)
            {
                var imagem = await SalvarImagem(update.FotoPerfil);

                instituicao.FotoPerfil = imagem;
            }

            if (!string.IsNullOrWhiteSpace(update.Cnpj) && Cnpj.TryParse(update.Cnpj, out var cnpj))
            {
                instituicao.Cnpj = cnpj.ToString();
            }
            else if (!string.IsNullOrWhiteSpace(update.Cnpj))
            {
                _logger.LogWarning("CNPJ {Cnpj} inválido para atualização", update.Cnpj);
                return Result<InstituicaoResponse>.Failure("Digite um CNPJ válido");
            }
            await _repository.AtualizarInstituicao(instituicao);
            await _repository.SaveChangesAsync();
            _logger.LogInformation("Instituição com ID {InstituicaoId} atualizada com sucesso", id);
            return Result<InstituicaoResponse>.Success(MapearResponse(instituicao));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar instituição com ID {InstituicaoId}", id);
            return Result<InstituicaoResponse>.Failure($"Erro ao atualizar instituição");
        }


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



    async Task<Result<InstituicaoResponse>> IInstituicaoService.CriarInstituicao(InstituicaoRequest request)
    {
        try
        {
            var validationResult = await _requestValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                _logger.LogInformation("Dados invalidos para criação da instituição");
                return Result<InstituicaoResponse>.Failure(validationResult.Errors.Select(e => e.ErrorMessage).ToList());
            }
            _logger.LogInformation("Iniciando criação de nova instituição");
            if (!Cnpj.TryParse(request.Cnpj, out var cnpj))
            {
                _logger.LogWarning("CNPJ {Cnpj} inválido para criação", request.Cnpj);
                return Result<InstituicaoResponse>.Failure("Digite um CNPJ válido");
            }
            var cnpjValido = cnpj.ToString();
            var imagem = await SalvarImagem(request.FotoPerfil);
            var instituicao = new Instituicao
            {
                Nome = request.Nome,
                NomeAbreviado = request.NomeAbreviado,
                Codigo = request.Codigo,
                Cnpj = cnpjValido,
                Rua = request.Rua,
                Numero = request.Numero,
                Bairro = request.Bairro,
                Cidade = request.Cidade,
                Estado = request.Estado,
                Cep = request.Cep,
                FotoPerfil = imagem,
                Telefone = request.Telefone,
                Site = request.Site,
                IsAtivo = true
            };
            await _repository.CriarInstituicao(instituicao);
            await _repository.SaveChangesAsync();
            _logger.LogInformation("Instituição criada com sucesso");
            return Result<InstituicaoResponse>.Success(MapearResponse(instituicao));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar instituição");
            return Result<InstituicaoResponse>.Failure("Erro ao criar instituição");
        }

    }
    async Task<Result<bool>> IInstituicaoService.DeletarInstituicao(int id)
    {
        try
        {
            _logger.LogInformation("Iniciando deleção da instituição com ID {InstituicaoId}", id);
            var instituicao = await _repository.ListarInstituicaoById(id);
            if (instituicao is null)
            {
                _logger.LogWarning("Instituição com ID {InstituicaoId} não encontrada para deleção", id);
                return Result<bool>.Failure("Instituição não encontrada");
            }
            instituicao.IsAtivo = false;
            instituicao.AtualizadoEmUtc = DateTime.UtcNow;
            await _repository.AtualizarInstituicao(instituicao);
            await _repository.SaveChangesAsync();
            _logger.LogInformation("Instituição com ID {InstituicaoId} desativada com sucesso", id);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao deletar instituição com ID {InstituicaoId}", id);
            return Result<bool>.Failure("Erro ao deletar instituição");
        }
    }

    async Task<Result<InstituicaoResponse>> IInstituicaoService.ListarInstituicaoById(int id)
    {
        try
        {
            _logger.LogInformation("Iniciando busca da instituição com ID {InstituicaoId}", id);
            var instituicao = await _repository.ListarInstituicaoById(id);
            if (instituicao is null)
            {
                _logger.LogWarning("Instituição com ID {InstituicaoId} não encontrada", id);
                return Result<InstituicaoResponse>.Failure("Instituição não encontrada");
            }
            _logger.LogInformation("Instituição com ID {InstituicaoId} encontrada com sucesso", id);
            return Result<InstituicaoResponse>.Success(MapearResponse(instituicao));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar instituição com ID {InstituicaoId}", id);
            return Result<InstituicaoResponse>.Failure("Erro ao buscar instituição");
        }
    }

    async Task<Result<IEnumerable<InstituicaoResponse>>> IInstituicaoService.ListarInstituicoes()
    {
        try
        {
            _logger.LogInformation("Iniciando listagem de instituições");
            var instituicoes = await _repository.ListarInstituicoes();
            _logger.LogInformation("Instituições listadas com sucesso {InstituicoesCount}", instituicoes.Count());

            return Result<IEnumerable<InstituicaoResponse>>.Success(instituicoes.Select(MapearResponse));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar instituições");
            return Result<IEnumerable<InstituicaoResponse>>.Failure("Erro ao listar instituições");
        }
    }

    async Task<Result<InstituicaoResponse>> IInstituicaoService.AlterarStatusInstituicao(int id, bool ativo)
    {
        var instituicao = await _repository.ListarInstituicaoById(id);
        if (instituicao is null)
            return Result<InstituicaoResponse>.Failure("Instituição não encontrada");

        instituicao.IsAtivo = ativo;
        instituicao.AtualizadoEmUtc = DateTime.UtcNow;
        await _repository.AtualizarInstituicao(instituicao);
        await _repository.SaveChangesAsync();

        return Result<InstituicaoResponse>.Success(MapearResponse(instituicao));
    }

    private static InstituicaoResponse MapearResponse(Instituicao instituicao)
    {
        return new InstituicaoResponse
        {
            Id = instituicao.Id,
            Nome = instituicao.Nome,
            NomeAbreviado = instituicao.NomeAbreviado,
            Codigo = instituicao.Codigo,
            Cnpj = instituicao.Cnpj,
            FotoPerfil = instituicao.FotoPerfil,
            Rua = instituicao.Rua,
            Numero = instituicao.Numero,
            Bairro = instituicao.Bairro,
            Cidade = instituicao.Cidade,
            Estado = instituicao.Estado,
            Cep = instituicao.Cep,
            Telefone = instituicao.Telefone,
            Site = instituicao.Site,
            IsAtivo = instituicao.IsAtivo
        };
    }
}
