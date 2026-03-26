namespace Unievent.Application.Interfaces.Services;

public interface IAlunoService
{
    public Task<AlunoResponse> CriarAluno(AlunoRequest request);
    public Task<AlunoResponse> AtualizarAluno(int id, AlunoUpdate update);
    public Task<bool> DeletarAluno(int id);
    public Task<IList<AlunoResponse>> ListarAlunos();
    public Task<AlunoResponse> ListarAlunoById(int id);
}
