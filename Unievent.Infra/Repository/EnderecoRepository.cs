namespace Unievent.Infra.Repository;

public class EnderecoRepository
{
    private AppDbContext _context;
    public EnderecoRepository(AppDbContext context)
    {
        _context = context;
    }
    Task<Endereco> IEnderecoRepository.AtualizarEndereco(Endereco endereco)
    {
        _context.Endereco.Update(endereco);
        return Task.FromResult(endereco);
    }

    async Task<Endereco> IEnderecoRepository.CriarEndereco(Endereco endereco)
    {
        await _context.Endereco.AddAsync(endereco);
        return endereco;
    }

    Task<bool> IEnderecoRepository.DeletarEndereco(Endereco endereco)
    {
        _context.Endereco.Remove(endereco);
        return Task.FromResult(true);
    }

    async Task<Endereco> IEnderecoRepository.ListarEnderecoById(int id)
    {
        return await _context.Endereco.FirstOrDefaultAsync(e => e.Id == id);
    }

    async Task<IEnumerable<Endereco>> IEnderecoRepository.ListarEnderecos()
    {
        return await _context.Endereco.ToListAsync();

    }

    Task IEnderecoRepository.SaveChangesAsync()
    {
        return _context.SaveChangesAsync();
    }
}
