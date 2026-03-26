namespace Unievent.Infra.Repository
{
    public class EventoRepository
    {
        private readonly AppDbContext _context;
        public EventoRepository(AppDbContext context)
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

        async Task<Evento> IEventoRepository.ListarEventoById(int id)
        {
            return await _context.Evento.Where(e => e.Id == id).FirstOrDefaultAsync();
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
