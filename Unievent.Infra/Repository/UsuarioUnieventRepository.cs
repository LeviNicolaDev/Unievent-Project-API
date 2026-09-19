using Microsoft.EntityFrameworkCore;
using Unievent.Application.Interfaces.Repository;
using Unievent.Domain.Entities;
using Unievent.Infra.Data;

namespace Unievent.Infra.Repository;

public class UsuarioUnieventRepository : IUsuarioUnieventRepository
{
    private readonly AppDbContext _context;

    public UsuarioUnieventRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<UsuarioUnievent> CriarUsuarioUnievent(UsuarioUnievent usuarioUnievent)
    {
        await _context.UsuarioUnievent.AddAsync(usuarioUnievent);
        return usuarioUnievent;
    }

    public async Task<UsuarioUnievent?> ListarUsuarioUnieventByEmail(string email)
    {
        return await _context.UsuarioUnievent.FirstOrDefaultAsync(u => u.EmailUsuario == email);
    }

    public async Task<bool> ExisteUsuarioUnievent()
    {
        return await _context.UsuarioUnievent.AnyAsync();
    }

    public Task SaveChangesAsync()
    {
        return _context.SaveChangesAsync();
    }
}
