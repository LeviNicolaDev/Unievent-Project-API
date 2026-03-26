using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Unievent.Infra.Repository
{
    public class AlunoRepository
    {
        private readonly AppDbContext _context;
        public AlunoRepository(AppDbContext context)
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