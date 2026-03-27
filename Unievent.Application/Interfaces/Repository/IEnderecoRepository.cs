using Unievent.Domain.Entities;

namespace Unievent.Application.Interfaces.Repository;

public interface IEnderecoRepository
{
    public Task<Endereco> CriarEndereco(Endereco endereco);
    public Task<Endereco> AtualizarEndereco(Endereco endereco);
    public Task<bool> DeletarEndereco(Endereco endereco);
    public Task<IList<Endereco>> ListarEnderecos();
    public Task<Endereco> ListarEnderecoById(int id);
    public Task SaveChangesAsync();
}
