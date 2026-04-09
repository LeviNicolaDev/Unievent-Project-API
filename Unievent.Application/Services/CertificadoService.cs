using Unievent.Application.Common;
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
    async Task<ResultData<CertificadoResponse>> ICertificadoService.AtualizarCertificado(int id, CertificadoUpdate update)
    {
        var certificado = await _repository.ListarCertificadoById(id);
        if (certificado is null)
        {
            return ResultData<CertificadoResponse>.Failure("Certificado não encontrado");
        }
        if (!string.IsNullOrWhiteSpace(update.Texto))
        {
            certificado.Texto = update.Texto;
        }
        if (!string.IsNullOrWhiteSpace(update.DataCertifcado.ToString()))
        {
            certificado.DataCertifcado = update.DataCertifcado.Value;
        }
        if (update.AlunoId.HasValue)
        {
            certificado.AlunoId = update.AlunoId.Value;
        }
        if (update.EventoId.HasValue)
        {
            certificado.EventoId = update.EventoId.Value;
        }
        await _repository.AtualizarCertificado(certificado);
        await _repository.SaveChangesAsync();
        return ResultData<CertificadoResponse>.Success(new CertificadoResponse
        {
            Id = id,
            DataCertifcado = certificado.DataCertifcado,
            AlunoId = certificado.AlunoId,
            EventoId = certificado.EventoId,
            Texto = certificado.Texto
        });
    }


    async Task<ResultData<CertificadoResponse>> ICertificadoService.CriarCertificado(CertificadoRequest request)
    {
        var certificado = new Certificado
        {
            DataCertifcado = request.DataCertifcado,
            AlunoId = request.AlunoId,
            EventoId = request.EventoId,
            Texto = request.Texto
        };
        await _repository.CriarCertificado(certificado);
        await _repository.SaveChangesAsync();
        return ResultData<CertificadoResponse>.Success(new CertificadoResponse
        {
            Id = certificado.Id,
            DataCertifcado = certificado.DataCertifcado,
            AlunoId = certificado.AlunoId,
            EventoId = certificado.EventoId,
            Texto = certificado.Texto
        });
    }

    async Task<Result> ICertificadoService.DeletarCertificado(int id)
    {
        var certificado = await _repository.ListarCertificadoById(id);
        if (certificado is null)
        {
            return Result.Failure("Certificado não encontrado");
        }
        await _repository.DeletarCertificado(certificado);
        await _repository.SaveChangesAsync();
        return Result.Success("Certificado deletado com sucesso");
    }

    async Task<ResultData<CertificadoResponse>> ICertificadoService.ListarCertificadoById(int id)
    {
        var certificado = await _repository.ListarCertificadoById(id);
        if (certificado is null)
        {
            return ResultData<CertificadoResponse>.Failure("Certificado não encontrado");
        }
        return ResultData<CertificadoResponse>.Success(new CertificadoResponse
        {
            Id = certificado.Id,
            DataCertifcado = certificado.DataCertifcado,
            AlunoId = certificado.AlunoId,
            EventoId = certificado.EventoId,
            Texto = certificado.Texto
        });
    }

    async Task<ResultData<IEnumerable<CertificadoResponse>>> ICertificadoService.ListarCertificados()
    {
        var certificados = await _repository.ListarCertificados();
        return ResultData<IEnumerable<CertificadoResponse>>.Success(certificados.Select(certificado => new CertificadoResponse
        {
            Id = certificado.Id,
            DataCertifcado = certificado.DataCertifcado,
            AlunoId = certificado.AlunoId,
            EventoId = certificado.EventoId,
            Texto = certificado.Texto
        }));
    }
}
