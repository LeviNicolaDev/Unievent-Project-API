using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Unievent.Application.Interfaces.Repository;
using Unievent.Domain.Entities;
using Unievent.Infra.Data;

namespace Unievent.Infra.Repository
{
    public class AlunoRepository : IAlunoRepository
    {
        private readonly AppDbContext _context;
        public AlunoRepository(AppDbContext context, IConfiguration config)
        {
            _context = context;
        }
        Task<Aluno> IAlunoRepository.AtualizarAluno(Aluno aluno)
        {
            _context.Aluno.Update(aluno);
            return Task.FromResult(aluno);
        }

        async Task<Aluno> IAlunoRepository.CriarAluno(Aluno aluno)
        {
            await _context.Aluno.AddAsync(aluno);
            return aluno;
        }

        Task<bool> IAlunoRepository.DeletarAluno(Aluno aluno)
        {
            _context.Aluno.Remove(aluno);
            return Task.FromResult(true);
        }

        async Task<Aluno> IAlunoRepository.ListarAlunoByEmail(string email)
        {
            return await _context.Aluno.FirstOrDefaultAsync(a => a.Email == email);
        }

        async Task<Aluno> IAlunoRepository.ListarAlunoById(int id)
        {
            return await _context.Aluno.Where(a => a.Id == id).FirstOrDefaultAsync();
        }

        async Task<IEnumerable<Aluno>> IAlunoRepository.ListarAlunos()
        {
            return await _context.Aluno.ToListAsync();
        }

        Task IAlunoRepository.SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }
    }
}