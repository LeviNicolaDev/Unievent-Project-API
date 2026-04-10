using System.Reflection.Metadata.Ecma335;
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
    public AlunoService(IAlunoRepository repository, ILogger<AlunoService> logger)
    {
        _repository = repository;
        _logger = logger;
    }
    async Task<ResultData<AlunoResponse>> IAlunoService.AtualizarAluno(int id, AlunoUpdate update)
    {
        try
        {
            _logger.LogInformation("Iniciando atualização do aluno com ID {AlunoId}", id);
            var aluno = await _repository.ListarAlunoById(id);
            if (aluno is null)
            {
                _logger.LogWarning("Aluno com ID {AlunoId} não encontrado para atualização", id);
                return ResultData<AlunoResponse>.Failure("Aluno não encontrado");
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
            return new ResultData<AlunoResponse>(new AlunoResponse
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

            return ResultData<AlunoResponse>.Failure($"Erro ao atualizar aluno");
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

    async Task<ResultData<AlunoResponse>> IAlunoService.CriarAluno(AlunoRequest request)
    {
        try
        {
            _logger.LogInformation("Iniciando criação de novo aluno com email {Email}", request.Email);
            var senha = BCrypt.Net.BCrypt.HashPassword(request.Senha);
            var imagem = await SalvarImagem(request.FotoPerfil);
            var emailExistente = await _repository.ListarAlunoByEmail(request.Email);
            if (emailExistente != null)
            {
                _logger.LogWarning("Tentativa de criar aluno com email já existente: {Email}", request.Email);
                return ResultData<AlunoResponse>.Failure("Email já cadastrado para outro aluno");
            }
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
            return ResultData<AlunoResponse>.Success(new AlunoResponse
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
            return ResultData<AlunoResponse>.Failure("Erro ao criar aluno");

        }
    }

    async Task<Result> IAlunoService.DeletarAluno(int id)
    {
        try
        {
            _logger.LogInformation("Iniciando deleção do aluno com ID {AlunoId}", id);
            var aluno = await _repository.ListarAlunoById(id);
            if (aluno is null)
            {
                _logger.LogWarning("Aluno com ID {AlunoId} não encontrado para deleção", id);
                return Result.Failure("Aluno não encontrado");
            }
            aluno.IsAtivo = false;
            await _repository.SaveChangesAsync();
            _logger.LogInformation("Aluno com ID {AlunoId} deletado com sucesso", id);
            return Result.Success("Aluno deletado com sucesso");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao deletar aluno com ID {AlunoId}", id);
            return Result.Failure("Erro ao deletar aluno");
        }
    }

    async Task<ResultData<AlunoResponse>> IAlunoService.ListarAlunoById(int id)
    {
        try
        {
            _logger.LogInformation("Iniciando busca do aluno com ID {AlunoId}", id);
            var aluno = await _repository.ListarAlunoById(id);
            if (aluno is null)
            {
                _logger.LogWarning("Aluno com ID {AlunoId} não encontrado", id);
                return ResultData<AlunoResponse>.Failure("Aluno não encontrado");
            }
            _logger.LogInformation("Aluno com ID {AlunoId} encontrado com sucesso", id);
            return ResultData<AlunoResponse>.Success(new AlunoResponse
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
            return ResultData<AlunoResponse>.Failure("Erro ao buscar aluno");
        }
    }

    async Task<ResultData<IEnumerable<AlunoResponse>>> IAlunoService.ListarAlunos()
    {
        try
        {
            _logger.LogInformation("Iniciando listagem de todos os alunos");
            var alunos = await _repository.ListarAlunos();
            _logger.LogInformation("Listagem de alunos realizada com sucesso. Total de alunos encontrados: {TotalAlunos}", alunos.Count());
            return ResultData<IEnumerable<AlunoResponse>>.Success(alunos.Select(a => new AlunoResponse
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
            return ResultData<IEnumerable<AlunoResponse>>.Failure("Erro ao listar alunos");
        }

    }
}
