namespace Unievent.Infra.Repository;

public class InstituicaoRepository
{
    private AppDbContext _context;
    public InstituicaoRepository(AppDbContext context)
    {
        _context = context;
    }
    Task<Instituicao> IInstituicaoRepository.AtualizarInstituicao(Instituicao instituicao)
    {
        _context.Instituicao.Update(instituicao);
        return Task.FromResult(instituicao);
    }

    async Task<Instituicao> IInstituicaoRepository.CriarInstituicao(Instituicao instituicao)
    {
        await _context.AddAsync(instituicao);
        return instituicao;
    }

    Task<bool> IInstituicaoRepository.DeletarInstituicao(Instituicao instituicao)
    {
        _context.Instituicao.Remove(instituicao);
        return Task.FromResult(true);
    }

    async Task<Instituicao> IInstituicaoRepository.ListarInstituicaoById(int id)
    {
        return await _context.Instituicao.FirstOrDefaultAsync(i => i.Id == id);
    }

    async Task<IEnumerable<Instituicao>> IInstituicaoRepository.ListarInstituicoes()
    {
        return await _context.Instituicao.ToListAsync();
    }

    Task IInstituicaoRepository.SaveChangesAsync()
    {
        return _context.SaveChangesAsync();
    }
}
