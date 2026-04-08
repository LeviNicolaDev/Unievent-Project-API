using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Unievent.Application.Interfaces.Repository;
using Unievent.Domain.Entities;
using Unievent.Infra.Data;

namespace Unievent.Infra.Repository
{
    public class EventoRepository : IEventoRepository
    {
        private readonly AppDbContext _context;
        public EventoRepository(AppDbContext context, IConfiguration config)
        {
            _context = context;
        }
        Task<Evento> IEventoRepository.AtualizarEvento(Evento evento)
        {
            _context.Evento.Update(evento);
            return Task.FromResult(evento);
        }

        async Task<Evento> IEventoRepository.CriarEvento(Evento evento)
        {
            await _context.Evento.AddAsync(evento);
            return evento;

        }

        Task<bool> IEventoRepository.DeletarEvento(Evento evento)
        {
            _context.Evento.Remove(evento);
            return Task.FromResult(true);
        }

        async Task<IEnumerable<Evento>> IEventoRepository.ListarEventoByCategoria(string categoria)
        {
            var eventos = await _context.Evento.Where(e => e.Categoria == categoria).ToListAsync();
            return eventos;
        }

        async Task<Evento> IEventoRepository.ListarEventoById(int id)
        {
            return await _context.Evento.Where(e => e.Id == id).FirstOrDefaultAsync();
        }

        async Task<Evento> IEventoRepository.ListarEventoByResponsavel(int responsavelId)
        {
            return await _context.Evento.Where(r => r.ResponsavelEventoId == responsavelId).FirstOrDefaultAsync();
        }

        async Task<IEnumerable<Evento>> IEventoRepository.ListarEventos()
        {
            return await _context.Evento.ToListAsync();
        }

        Task IEventoRepository.SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }
    }
}
