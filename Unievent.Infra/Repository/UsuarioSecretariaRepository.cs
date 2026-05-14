using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Unievent.Application.Interfaces.Repository;
using Unievent.Domain.Entities;
using Unievent.Infra.Data;

namespace Unievent.Infra.Repository
{
    public class UsuarioSecretariaRepository : IUsuarioSecretariaRepository
    {
        private readonly AppDbContext _context;
        public UsuarioSecretariaRepository(AppDbContext context, IConfiguration config)
        {
            _context = context;
        }
        Task<UsuarioSecretaria> IUsuarioSecretariaRepository.AtualizarUsuarioSecretaria(UsuarioSecretaria usuarioSecretaria)
        {
            _context.UsuarioSecretaria.Update(usuarioSecretaria);
            return Task.FromResult(usuarioSecretaria);
        }

        async Task<UsuarioSecretaria> IUsuarioSecretariaRepository.CriarUsuarioSecretaria(UsuarioSecretaria usuarioSecretaria)
        {
            await _context.UsuarioSecretaria.AddAsync(usuarioSecretaria);
            return usuarioSecretaria;
        }

        Task<bool> IUsuarioSecretariaRepository.DeletarUsuarioSecretaria(UsuarioSecretaria usuarioSecretaria)
        {
            _context.UsuarioSecretaria.Remove(usuarioSecretaria);
            return Task.FromResult(true);
        }

        async Task<UsuarioSecretaria> IUsuarioSecretariaRepository.ListarUsuarioSecretariaByEmail(string email)
        {
            return await _context.UsuarioSecretaria.FirstOrDefaultAsync(s => s.EmailUsuario == email);

        }

        async Task<UsuarioSecretaria> IUsuarioSecretariaRepository.ListarUsuarioSecretariaById(int id)
        {
            return await _context.UsuarioSecretaria.Where(s => s.Id == id).FirstOrDefaultAsync();
        }

        async Task<IEnumerable<UsuarioSecretaria>> IUsuarioSecretariaRepository.ListarUsuarioSecretarias()
        {
            return await _context.UsuarioSecretaria.ToListAsync();
        }

        Task IUsuarioSecretariaRepository.SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }




    }
}
