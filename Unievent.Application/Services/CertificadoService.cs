using Unievent.Application.Dtos.Certificado;
using Unievent.Application.Interfaces.Repository;
using Unievent.Application.Interfaces.Services;
using Unievent.Domain.Entities;

namespace Unievent.Application.Services;

public class CertificadoService : ICertificadoService
{
    private readonly ICertificadoRepository _repository;
    public CertificadoService(ICertificadoRepository repository)
    {
        _repository = repository;
    }
    async Task<CertificadoResponse> ICertificadoService.AtualizarCertificado(int id, CertificadoUpdate update)
    {
        var certificado = await _repository.ListarCertificadoById(id) ?? throw new Exception("Certificado não encontrado");
        if (!string.IsNullOrWhiteSpace(update.Texto))
        {
            certificado.Texto = update.Texto;
        }
        if (!string.IsNullOrWhiteSpace(update.DataCertifcado.ToString()))
        {
            certificado.DataCertifcado = update.DataCertifcado.Value;
        }
        if (update.IdAluno.HasValue)
        {
            certificado.IdAluno = update.IdAluno.Value;
        }
        if (update.IdEvento.HasValue)
        {
            certificado.IdEvento = update.IdEvento.Value;
        }
        await _repository.AtualizarCertificado(certificado);
        await _repository.SaveChangesAsync();
        return new CertificadoResponse
        {
            Id = id,
            DataCertifcado = certificado.DataCertifcado,
            IdAluno = certificado.IdAluno,
            IdEvento = certificado.IdEvento,
            Texto = certificado.Texto
        };
    }


    async Task<CertificadoResponse> ICertificadoService.CriarCertificado(CertificadoRequest request)
    {
        var certificado = new Certificado
        {
            DataCertifcado = request.DataCertifcado,
            IdAluno = request.IdAluno,
            IdEvento = request.IdEvento,
            Texto = request.Texto
        };
        await _repository.CriarCertificado(certificado);
        await _repository.SaveChangesAsync();
        return new CertificadoResponse
        {
            Id = certificado.Id,
            DataCertifcado = certificado.DataCertifcado,
            IdAluno = certificado.IdAluno,
            IdEvento = certificado.IdEvento,
            Texto = certificado.Texto
        };
    }

    async Task<bool> ICertificadoService.DeletarCertificado(int id)
    {
        var certificado = await _repository.ListarCertificadoById(id) ?? throw new Exception("Certificado não encontrado");
        await _repository.DeletarCertificado(certificado);
        await _repository.SaveChangesAsync();
        return true;
    }

    async Task<CertificadoResponse> ICertificadoService.ListarCertificadoById(int id)
    {
        var certificado = await _repository.ListarCertificadoById(id) ?? throw new Exception("Certificado não encontrado");
        return new CertificadoResponse
        {
            Id = certificado.Id,
            DataCertifcado = certificado.DataCertifcado,
            IdAluno = certificado.IdAluno,
            IdEvento = certificado.IdEvento,
            Texto = certificado.Texto
        };
    }

    async Task<IList<CertificadoResponse>> ICertificadoService.ListarCertificados()
    {
        var certificados = await _repository.ListarCertificados();
        return certificados.Select(certificado => new CertificadoResponse
        {
            Id = certificado.Id,
            DataCertifcado = certificado.DataCertifcado,
            IdAluno = certificado.IdAluno,
            IdEvento = certificado.IdEvento,
            Texto = certificado.Texto
        }).ToList();
    }
}
