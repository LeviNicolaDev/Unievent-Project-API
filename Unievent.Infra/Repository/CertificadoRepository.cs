using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unievent.Application.Interfaces.Repository;
using Unievent.Domain.Entities;
using Unievent.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace Unievent.Infra.Repository
{
    public class CertificadoRepository : ICertificadoRepository
    {
        private readonly AppDbContext _context;
        public CertificadoRepository(AppDbContext context)
        {
            _context = context;
        }
        Task<Certificado> ICertificadoRepository.AtualizarCertificado(Certificado certificado)
        {
            _context.Certificado.Update(certificado);
            return Task.FromResult(certificado);
        }

        async Task<Certificado> ICertificadoRepository.CriarCertificado(Certificado certificado)
        {
            await _context.Certificado.AddAsync(certificado);
            return certificado;
        }

        Task<bool> ICertificadoRepository.DeletarCertificado(Certificado certificado)
        {
            _context.Certificado.Remove(certificado);
            return Task.FromResult(true);
        }

        async Task<Certificado> ICertificadoRepository.ListarCertificadoById(int id)
        {
            return await _context.Certificado.FirstOrDefaultAsync(c => c.Id == id);
        }

        async Task<IEnumerable<Certificado>> ICertificadoRepository.ListarCertificados()
        {
            return await _context.Certificado.ToListAsync();
        }

        Task ICertificadoRepository.SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }
    }
}