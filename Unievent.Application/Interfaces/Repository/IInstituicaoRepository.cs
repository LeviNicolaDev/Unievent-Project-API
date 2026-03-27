using Unievent.Domain.Entities;

namespace Unievent.Application.Interfaces.Repository;

public interface IInstituicaoRepository
{
    public Task<Instituicao> CriarInstituicao(Instituicao instituicao);
    public Task<Instituicao> AtualizarInstituicao(Instituicao instituicao);
    public Task<bool> DeletarInstituicao(Instituicao instituicao);
    public Task<IEnumerable<Instituicao>> ListarInstituicoes();
    public Task<Instituicao> ListarInstituicaoById(int id);
    public Task SaveChangesAsync();
}
