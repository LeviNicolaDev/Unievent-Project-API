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
    private readonly IEnderecoRepository _repositoryEndereco;
    private readonly IValidator<InstituicaoRequest> _requestValidator;
    private readonly IValidator<InstituicaoUpdate> _updateValidator;
    private readonly ILogger<InstituicaoService> _logger;
    public InstituicaoService(IInstituicaoRepository repository, IEnderecoRepository repositoryEndereco, ILogger<InstituicaoService> logger,
    IValidator<InstituicaoRequest> requestValidator,IValidator<InstituicaoUpdate> updateValidator)
    {
        _repository = repository;
        _repositoryEndereco = repositoryEndereco;
        _logger = logger;
        _requestValidator = requestValidator;
        _updateValidator = updateValidator;
    }
    async Task<ResultData<InstituicaoResponse>> IInstituicaoService.AtualizarInstituicao(int id, InstituicaoUpdate update)
    {
        try
        {
            var validationResult = await _updateValidator.ValidateAsync(update);
            if (!validationResult.IsValid)
            {
            _logger.LogInformation("Dados invalidos para atualização da instituição com ID {InstituicaoId}", id);
            return ResultData<InstituicaoResponse>.Failure("Dados Inválidos");    
            }
            _logger.LogInformation("Iniciando atualização da instituição com ID {InstituicaoId}", id);
            var emailExistente = await _repository.ListarInstituicaoByEmail(update.EmailLogin);
            var instituicao = await _repository.ListarInstituicaoById(id);
            if (instituicao is null)
            {
                _logger.LogWarning("Instituição com ID {InstituicaoId} não encontrada para atualização", id);
                return ResultData<InstituicaoResponse>.Failure("Instituição não encontrada");
            }

            if (update.EnderecoId.HasValue)
            {
                var endereco = await _repositoryEndereco.ListarEnderecoById(update.EnderecoId.Value);
                if (endereco is null)
                {
                    _logger.LogWarning("Endereço com ID {EnderecoId} não encontrado para ser associado à instituição", update.EnderecoId.Value);
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
                _logger.LogWarning("Email {EmailLogin} já cadastrado para outra instituição", update.EmailLogin);
                return ResultData<InstituicaoResponse>.Failure("Email já cadastrado para outra instituição");
            }

            if (!string.IsNullOrWhiteSpace(update.Cnpj) && Cnpj.TryParse(update.Cnpj, out var cnpj))
            {
                instituicao.Cnpj = cnpj.ToString();
            }
            else
            {
                _logger.LogWarning("CNPJ {Cnpj} inválido para atualização", update.Cnpj);
                return ResultData<InstituicaoResponse>.Failure("Digite um CNPJ válido");
            }
            await _repository.AtualizarInstituicao(instituicao);
            await _repository.SaveChangesAsync();
            _logger.LogInformation("Instituição com ID {InstituicaoId} atualizada com sucesso", id);
            return ResultData<InstituicaoResponse>.Success(new InstituicaoResponse
            {
                Id = id,
                Cnpj = instituicao.Cnpj,
                EmailLogin = instituicao.EmailLogin,
                FotoPerfil = instituicao.FotoPerfil,
                EnderecoId = instituicao.EnderecoId
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar instituição com ID {InstituicaoId}", id);
            return ResultData<InstituicaoResponse>.Failure($"Erro ao atualizar instituição");
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



    async Task<ResultData<InstituicaoResponse>> IInstituicaoService.CriarInstituicao(InstituicaoRequest request)
    {
        try
        {
            var validationResult = await _requestValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
            _logger.LogInformation("Dados invalidos para criação da instituição com email {EmailLogin}", request.EmailLogin);
            return ResultData<InstituicaoResponse>.Failure("Dados Inválidos");    
            }
            _logger.LogInformation("Iniciando criação de nova instituição com email {EmailLogin}", request.EmailLogin);
            if (!Cnpj.TryParse(request.Cnpj, out var cnpj))
            {
                _logger.LogWarning("CNPJ {Cnpj} inválido para criação", request.Cnpj);
                return ResultData<InstituicaoResponse>.Failure("Digite um CNPJ válido");
            }
            var endereco = await _repositoryEndereco.ListarEnderecoById(request.EnderecoId);
            if (endereco is null)
            {
                _logger.LogWarning("Endereço com ID {EnderecoId} não encontrado para ser associado à instituição", request.EnderecoId);
                return ResultData<InstituicaoResponse>.Failure("Endereço não encontrado para ser associado à instituição");
            }
            var emailExistente = await _repository.ListarInstituicaoByEmail(request.EmailLogin);
            if (emailExistente != null)
            {
                _logger.LogWarning("Email {EmailLogin} já cadastrado para outra instituição", request.EmailLogin);
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
            _logger.LogInformation("Instituição criada com sucesso com email {EmailLogin}", request.EmailLogin);
            return ResultData<InstituicaoResponse>.Success(new InstituicaoResponse
            {
                Id = instituicao.Id,
                Cnpj = instituicao.Cnpj,
                EmailLogin = instituicao.EmailLogin,
                FotoPerfil = instituicao.FotoPerfil,
                EnderecoId = instituicao.EnderecoId
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar instituição com email {EmailLogin}", request.EmailLogin);
            return ResultData<InstituicaoResponse>.Failure("Erro ao criar instituição");
        }

    }
    async Task<Result> IInstituicaoService.DeletarInstituicao(int id)
    {
        try
        {
            _logger.LogInformation("Iniciando deleção da instituição com ID {InstituicaoId}", id);
            var instituicao = await _repository.ListarInstituicaoById(id);
            if (instituicao is null)
            {
                _logger.LogWarning("Instituição com ID {InstituicaoId} não encontrada para deleção", id);
                return Result.Failure("Instituição não encontrada");
            }
            await _repository.DeletarInstituicao(instituicao);
            await _repository.SaveChangesAsync();
            _logger.LogInformation("Instituição com ID {InstituicaoId} deletada com sucesso", id);
            return Result.Success("Instituicao deletada com sucesso");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao deletar instituição com ID {InstituicaoId}", id);
            return Result.Failure("Erro ao deletar instituição");
        }
    }

    async Task<ResultData<InstituicaoResponse>> IInstituicaoService.ListarInstituicaoById(int id)
    {
        try
        {
            _logger.LogInformation("Iniciando busca da instituição com ID {InstituicaoId}", id);
            var instituicao = await _repository.ListarInstituicaoById(id);
            if (instituicao is null)
            {
                _logger.LogWarning("Instituição com ID {InstituicaoId} não encontrada", id);
                return ResultData<InstituicaoResponse>.Failure("Instituição não encontrada");
            }
            _logger.LogInformation("Instituição com ID {InstituicaoId} encontrada com sucesso", id);
            return ResultData<InstituicaoResponse>.Success(new InstituicaoResponse
            {
                Id = id,
                Cnpj = instituicao.Cnpj,
                EmailLogin = instituicao.EmailLogin,
                FotoPerfil = instituicao.FotoPerfil,

                EnderecoId = instituicao.EnderecoId
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar instituição com ID {InstituicaoId}", id);
            return ResultData<InstituicaoResponse>.Failure("Erro ao buscar instituição");
        }
    }

    async Task<ResultData<IEnumerable<InstituicaoResponse>>> IInstituicaoService.ListarInstituicoes()
    {
        try
        {
            _logger.LogInformation("Iniciando listagem de instituições");
            var instituicoes = await _repository.ListarInstituicoes();
            _logger.LogInformation("Instituições listadas com sucesso {InstituicoesCount}", instituicoes.Count());

            return ResultData<IEnumerable<InstituicaoResponse>>.Success(instituicoes.Select(instituicao => new InstituicaoResponse
            {
                Id = instituicao.Id,
                Cnpj = instituicao.Cnpj,
                EmailLogin = instituicao.EmailLogin,
                FotoPerfil = instituicao.FotoPerfil,
                EnderecoId = instituicao.EnderecoId
            }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar instituições");
            return ResultData<IEnumerable<InstituicaoResponse>>.Failure("Erro ao listar instituições");
        }
    }
}
