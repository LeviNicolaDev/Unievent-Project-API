using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Unievent.Application.Interfaces.Repository;
using Unievent.Domain.Entities;
using Unievent.Infra.Data;

namespace Unievent.Infra.Repository
{
    public class ResponsavelEventoRepository : IResponsavelEventoRepository
    {
        private readonly AppDbContext _context;
        public ResponsavelEventoRepository(AppDbContext context, IConfiguration config)
        {
            _context = context;
        }

        Task<ResponsavelEvento> IResponsavelEventoRepository.AtualizarResponsavelEvento(ResponsavelEvento responsavel)
        {
            _context.ResponsavelEvento.Update(responsavel);
            return Task.FromResult(responsavel);
        }

        async Task<ResponsavelEvento> IResponsavelEventoRepository.CriarResponsavelEvento(ResponsavelEvento responsavel)
        {
            await _context.ResponsavelEvento.AddAsync(responsavel);
            return responsavel;
        }

        Task<bool> IResponsavelEventoRepository.DeletarResponsavelEvento(ResponsavelEvento responsavelEvento)
        {
            _context.ResponsavelEvento.Remove(responsavelEvento);
            return Task.FromResult(true);
        }

        async Task<IEnumerable<ResponsavelEvento>> IResponsavelEventoRepository.ListarResponsaveisEvento()
        {
            return await _context.ResponsavelEvento.ToListAsync();
        }

        async Task<ResponsavelEvento> IResponsavelEventoRepository.ListarResponsavelEventoById(int id)
        {
            return await _context.ResponsavelEvento.FirstOrDefaultAsync(r => r.Id == id);
        }

        Task IResponsavelEventoRepository.SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }
    }
}
