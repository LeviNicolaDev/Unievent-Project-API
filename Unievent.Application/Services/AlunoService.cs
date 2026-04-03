using Microsoft.AspNetCore.Http;
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
    async Task<AlunoResponse> IAlunoService.AtualizarAluno(int id, AlunoUpdate update)
    {

        var aluno = await _repository.ListarAlunoById(id) ?? throw new Exception("Aluno não encontrado");
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
        return new AlunoResponse
        {
            Id = id,
            Nome = aluno.Nome,
            Email = aluno.Email,
            FotoPerfil = aluno.FotoPerfil,
            IsAtivo = aluno.IsAtivo,

            DataNascimento = aluno.DataNascimento
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

    async Task<AlunoResponse> IAlunoService.CriarAluno(AlunoRequest request)
    {
        var senha = BCrypt.Net.BCrypt.HashPassword(request.Senha);
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
        return new AlunoResponse
        {
            Id = aluno.Id,
            Nome = aluno.Nome,
            Email = aluno.Email,
            FotoPerfil = aluno.FotoPerfil,
            IsAtivo = aluno.IsAtivo,

            DataNascimento = aluno.DataNascimento
        };
    }

    async Task<bool> IAlunoService.DeletarAluno(int id)
    {
        var aluno = await _repository.ListarAlunoById(id) ?? throw new Exception("Aluno não encontrado");
        aluno.IsAtivo = false;
        await _repository.SaveChangesAsync();
        return true;
    }

    async Task<AlunoResponse> IAlunoService.ListarAlunoById(int id)
    {
        var aluno = await _repository.ListarAlunoById(id) ?? throw new Exception("Aluno não encontrado");

        return new AlunoResponse
        {
            Id = aluno.Id,
            Nome = aluno.Nome,
            Email = aluno.Email,
            FotoPerfil = aluno.FotoPerfil,
            IsAtivo = aluno.IsAtivo,
            DataNascimento = aluno.DataNascimento

        };
    }

    async Task<IList<AlunoResponse>> IAlunoService.ListarAlunos()
    {
        var alunos = await _repository.ListarAlunos();
        return alunos.Select(a => new AlunoResponse
        {
            Id = a.Id,
            Nome = a.Nome,
            Email = a.Email,
            FotoPerfil = a.FotoPerfil,
            IsAtivo = a.IsAtivo,
            DataNascimento = a.DataNascimento

        }).ToList();
    }
}
