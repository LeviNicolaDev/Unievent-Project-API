using Unievent.Domain.Entities;

namespace Unievent.Application.Interfaces.Repository;

public interface IAlunoRepository
{
    public Task<Aluno> CriarAluno(Aluno aluno);
    public Task<Aluno> AtualizarAluno(Aluno aluno);
    public Task<IList<Aluno>> ListarAlunos();
    public Task<Aluno> ListarAlunoById(int id);
    public Task<bool> DeletarAluno(Aluno aluno);
    public Task SaveChangesAsync();
}
