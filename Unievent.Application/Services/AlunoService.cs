using System.Reflection.Metadata.Ecma335;
using Microsoft.AspNetCore.Http;
using Unievent.Application.Common;
using Unievent.Application.Dtos.Aluno;
using Unievent.Application.Interfaces.Repository;
using Unievent.Application.Interfaces.Services;
using Unievent.Domain.Entities;

namespace Unievent.Application.Services;

public class AlunoService : IAlunoService

{
    private readonly IAlunoRepository _repository;
    public AlunoService(IAlunoRepository repository)
    {
        _repository = repository;
    }
    async Task<ResultData<AlunoResponse>> IAlunoService.AtualizarAluno(int id, AlunoUpdate update)
    {
        var aluno = await _repository.ListarAlunoById(id);
        if (aluno is null)
        {
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

        if (!string.IsNullOrWhiteSpace(update.DataNascimento.ToString()))
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
        var senha = BCrypt.Net.BCrypt.HashPassword(request.Senha);
        var imagem = await SalvarImagem(request.FotoPerfil);
        var emailExistente = await _repository.ListarAlunoByEmail(request.Email);
        if (emailExistente != null)
        {
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

    async Task<Result> IAlunoService.DeletarAluno(int id)
    {
        var aluno = await _repository.ListarAlunoById(id);
        if (aluno is null)
        {
            return Result.Failure("Aluno não encontrado");
        }
        aluno.IsAtivo = false;
        await _repository.SaveChangesAsync();
        return Result.Success("Aluno deletado com sucesso");
    }

    async Task<ResultData<AlunoResponse>> IAlunoService.ListarAlunoById(int id)
    {
        var aluno = await _repository.ListarAlunoById(id);
        if (aluno is null)
        {
            return ResultData<AlunoResponse>.Failure("Aluno não encontrado");
        }

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

    async Task<ResultData<IEnumerable<AlunoResponse>>> IAlunoService.ListarAlunos()
    {
        var alunos = await _repository.ListarAlunos();
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
}
