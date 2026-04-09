using Unievent.Application.Common;
using Unievent.Application.Dtos.Aluno;

namespace Unievent.Application.Interfaces.Services;

public interface IAlunoService
{
    public Task<ResultData<AlunoResponse>> CriarAluno(AlunoRequest request);
    public Task<ResultData<AlunoResponse>> AtualizarAluno(int id, AlunoUpdate update);
    public Task<Result> DeletarAluno(int id);
    public Task<ResultData<IEnumerable<AlunoResponse>>> ListarAlunos();
    public Task<ResultData<AlunoResponse>> ListarAlunoById(int id);
}
