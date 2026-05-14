using System.Reflection.Metadata.Ecma335;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Unievent.Application.Common;
using Unievent.Application.Dtos.Aluno;
using Unievent.Application.Interfaces.Repository;
using Unievent.Application.Interfaces.Services;
using Unievent.Domain.Entities;

namespace Unievent.Application.Services;

public class AlunoService : IAlunoService

{
    private readonly IAlunoRepository _repository;
    private readonly ILogger<AlunoService> _logger;
    private readonly IValidator<AlunoRequest> _validatorRequest;
    private readonly IValidator<AlunoUpdate> _validatorUpdate;
    public AlunoService(IAlunoRepository repository, ILogger<AlunoService> logger, IValidator<AlunoRequest> validatorRequest, IValidator<AlunoUpdate> validatorUpdate)
    {
        _repository = repository;
        _logger = logger;
        _validatorRequest = validatorRequest;
        _validatorUpdate = validatorUpdate;
    }
    async Task<Result<AlunoResponse>> IAlunoService.AtualizarAluno(int id, AlunoUpdate update)
    {
        try
        {
            _logger.LogInformation("Iniciando atualização do aluno com ID {AlunoId}", id);
            var validationResult = await _validatorUpdate.ValidateAsync(update);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Dados inválidos para atualização do aluno com ID {AlunoId}", id);
                return Result<AlunoResponse>.Failure(validationResult.Errors.Select(e => e.ErrorMessage).ToList());
            }
            var aluno = await _repository.ListarAlunoById(id);
            if (aluno is null)
            {
                _logger.LogWarning("Aluno com ID {AlunoId} não encontrado para atualização", id);
                return Result<AlunoResponse>.Failure("Aluno não encontrado");
            }
            if (!string.IsNullOrWhiteSpace(update.Nome))
            {
                aluno.Nome = update.Nome;
            }

            if (!string.IsNullOrWhiteSpace(update.Senha))
            {
                var senhaHash = BCrypt.Net.BCrypt.HashPassword(update.Senha);
                aluno.Senha = senhaHash;
            }

            if (update.DataNascimento.HasValue)
            {
                aluno.DataNascimento = update.DataNascimento.Value;
            }

            if (update.FotoPerfil != null)
            {
                var imagem = await SalvarImagem(update.FotoPerfil);
                aluno.FotoPerfil = imagem;
            }
            await _repository.AtualizarAluno(aluno);
            await _repository.SaveChangesAsync();
            _logger.LogInformation("Aluno com ID {AlunoId} atualizado com sucesso", id);
            return Result<AlunoResponse>.Success(new AlunoResponse
            {
                Id = id,
                Nome = aluno.Nome,
                Email = aluno.Email,
                FotoPerfil = aluno.FotoPerfil,
                IsAtivo = aluno.IsAtivo,

                DataNascimento = aluno.DataNascimento
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar aluno com ID {AlunoId}", id);

            return Result<AlunoResponse>.Failure($"Erro ao atualizar aluno");
        }

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

    async Task<Result<AlunoResponse>> IAlunoService.CriarAluno(AlunoRequest request)
    {
        try
        {
            var validationResult = await _validatorRequest.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Dados inválidos para criação de aluno com email {Email}", request.Email);
                return Result<AlunoResponse>.Failure(validationResult.Errors.Select(e => e.ErrorMessage).ToList());
            }
            _logger.LogInformation("Iniciando criação de novo aluno com email {Email}", request.Email);
            var senha = BCrypt.Net.BCrypt.HashPassword(request.Senha);

            var emailExistente = await _repository.ListarAlunoByEmail(request.Email);
            if (emailExistente != null)
            {
                _logger.LogWarning("Tentativa de criar aluno com email já existente: {Email}", request.Email);
                return Result<AlunoResponse>.Failure("Email já cadastrado para outro aluno");
            }
            var imagem = await SalvarImagem(request.FotoPerfil);

            var aluno = new Aluno
            {
                Nome = request.Nome,
                Email = request.Email,
                FotoPerfil = imagem,
                DataNascimento = request.DataNascimento,
                IsAtivo = true,
                Senha = senha
            };
            await _repository.CriarAluno(aluno);
            await _repository.SaveChangesAsync();
            _logger.LogInformation("Aluno criado com sucesso com ID {AlunoId}", aluno.Id);
            return Result<AlunoResponse>.Success(new AlunoResponse
            {
                Id = aluno.Id,
                Nome = aluno.Nome,
                Email = aluno.Email,
                FotoPerfil = aluno.FotoPerfil,
                IsAtivo = aluno.IsAtivo,

                DataNascimento = aluno.DataNascimento
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar aluno");
            return Result<AlunoResponse>.Failure("Erro ao criar aluno");

        }
    }

    async Task<Result<bool>> IAlunoService.DeletarAluno(int id)
    {
        try
        {
            _logger.LogInformation("Iniciando deleção do aluno com ID {AlunoId}", id);
            var aluno = await _repository.ListarAlunoById(id);
            if (aluno is null)
            {
                _logger.LogWarning("Aluno com ID {AlunoId} não encontrado para deleção", id);
                return Result<bool>.Failure("Aluno não encontrado");
            }
            aluno.IsAtivo = false;
            await _repository.SaveChangesAsync();
            _logger.LogInformation("Aluno com ID {AlunoId} deletado com sucesso", id);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao deletar aluno com ID {AlunoId}", id);
            return Result<bool>.Failure("Erro ao deletar aluno");
        }
    }

    async Task<Result<AlunoResponse>> IAlunoService.ListarAlunoById(int id)
    {
        try
        {
            _logger.LogInformation("Iniciando busca do aluno com ID {AlunoId}", id);
            var aluno = await _repository.ListarAlunoById(id);
            if (aluno is null)
            {
                _logger.LogWarning("Aluno com ID {AlunoId} não encontrado", id);
                return Result<AlunoResponse>.Failure("Aluno não encontrado");
            }
            _logger.LogInformation("Aluno com ID {AlunoId} encontrado com sucesso", id);
            return Result<AlunoResponse>.Success(new AlunoResponse
            {
                Id = aluno.Id,
                Nome = aluno.Nome,
                Email = aluno.Email,
                FotoPerfil = aluno.FotoPerfil,
                IsAtivo = aluno.IsAtivo,
                DataNascimento = aluno.DataNascimento

            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar aluno com ID {AlunoId}", id);
            return Result<AlunoResponse>.Failure("Erro ao buscar aluno");
        }
    }

    async Task<Result<IEnumerable<AlunoResponse>>> IAlunoService.ListarAlunos()
    {
        try
        {
            _logger.LogInformation("Iniciando listagem de todos os alunos");
            var alunos = await _repository.ListarAlunos();
            _logger.LogInformation("Listagem de alunos realizada com sucesso. Total de alunos encontrados: {TotalAlunos}", alunos.Count());
            return Result<IEnumerable<AlunoResponse>>.Success(alunos.Select(a => new AlunoResponse
            {
                Id = a.Id,
                Nome = a.Nome,
                Email = a.Email,
                FotoPerfil = a.FotoPerfil,
                IsAtivo = a.IsAtivo,
                DataNascimento = a.DataNascimento

            }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar alunos");
            return Result<IEnumerable<AlunoResponse>>.Failure("Erro ao listar alunos");
        }

    }
}
