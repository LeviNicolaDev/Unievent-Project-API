using Unievent.Application.Common;
using Unievent.Application.Dtos.Aluno;

namespace Unievent.Application.Interfaces.Services;

public interface IAlunoService
{
    public Task<Result<AlunoResponse>> CriarAluno(AlunoRequest request);
    public Task<Result<AlunoResponse>> AtualizarAluno(int id, AlunoUpdate update);
    public Task<Result<bool>> DeletarAluno(int id);
    public Task<Result<IEnumerable<AlunoResponse>>> ListarAlunos();
    public Task<Result<AlunoResponse>> ListarAlunoById(int id);
}
