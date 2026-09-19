using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Unievent.Api.Security;
using Unievent.Application.Dtos.Certificado;
using Unievent.Application.Interfaces.Services;
using Unievent.Infra.Data;

namespace Unievent.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CertificadoController : ControllerBase
{
    private readonly ICertificadoService _service;
    private readonly IParticipacaoService _participacaoService;
    private readonly AppDbContext _db;

    public CertificadoController(
        ICertificadoService service,
        IParticipacaoService participacaoService,
        AppDbContext db)
    {
        _service = service;
        _participacaoService = participacaoService;
        _db = db;
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Secretaria")]
    public async Task<IActionResult> CriarCertificado([FromBody] CertificadoRequest request)
    {
        if (!await PodeAcessarEvento(request.EventoId)) return Forbid();

        var certificado = await _service.CriarCertificado(request);
        if (certificado.IsFailure) return BadRequest(certificado.Errors);

        return Ok(certificado.Value);
    }

    [HttpGet("meus")]
    [Authorize(Roles = "Aluno")]
    public async Task<IActionResult> MeusCertificados(CancellationToken cancellationToken)
    {
        if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var alunoId)) return Unauthorized();
        var result = await _db.Participacao.AsNoTracking()
            .Where(p => p.AlunoId == alunoId)
            .OrderByDescending(p => p.Evento.DataEvento)
            .Select(p => new
            {
                p.Id, p.EventoId, NomeEvento = p.Evento.Nome,
                p.PresencaConfirmada, p.CertificadoEmitido, p.CodigoValidacao,
                p.CertificadoEnviadoPorEmail, p.StatusEnvioCertificado, p.ErroEnvioCertificadoEmail,
                p.DataGeracaoCertificado, p.DataEnvioCertificadoEmail,
                PdfDisponivel = p.CertificadoPdf != null,
                TemCertificado = p.CertificadoPdf != null || _db.Certificado.Any(c => c.EventoId == p.EventoId)
            }).ToListAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet("eventos/{eventoId}/pdf")]
    [Authorize(Roles = "Aluno")]
    public async Task<IActionResult> BaixarMeuPdf(int eventoId, CancellationToken cancellationToken)
    {
        if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var alunoId)) return Unauthorized();
        var arquivo = await _db.Participacao.AsNoTracking()
            .Where(p => p.AlunoId == alunoId && p.EventoId == eventoId && p.PresencaConfirmada && p.CertificadoEmitido)
            .Select(p => new { p.CertificadoPdf, p.NomeArquivoCertificado })
            .FirstOrDefaultAsync(cancellationToken);
        if (arquivo?.CertificadoPdf is null) return NotFound("Certificado PDF ainda não disponível.");
        Response.Headers.CacheControl = "private, no-store";
        return File(arquivo.CertificadoPdf, "application/pdf", arquivo.NomeArquivoCertificado ?? "certificado.pdf");
    }

    [HttpPatch("{id}")]
    [Authorize(Roles = "Admin,Secretaria")]
    public async Task<IActionResult> AtualizarCertificado([FromRoute] int id, [FromBody] CertificadoUpdate update)
    {
        if (!await PodeAcessarCertificado(id)) return Forbid();
        if (update.EventoId.HasValue && !await PodeAcessarEvento(update.EventoId.Value)) return Forbid();

        var certificado = await _service.AtualizarCertificado(id, update);
        if (certificado.IsFailure) return BadRequest(certificado.Errors);

        return Ok(certificado.Value);
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Secretaria")]
    public async Task<IActionResult> ListarCertificados()
    {
        var certificados = await _service.ListarCertificados();
        if (certificados.IsFailure) return NotFound(certificados.Errors);

        var resultado = certificados.Value ?? [];
        if (!User.IsGlobalAdmin())
        {
            var eventosPermitidos = await _db.Evento
                .AsNoTracking()
                .Where(e => e.InstituicaoId == User.GetInstituicaoId())
                .Select(e => e.Id)
                .ToListAsync();
            resultado = resultado.Where(c => eventosPermitidos.Contains(c.EventoId));
        }

        return Ok(resultado);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Secretaria")]
    public async Task<IActionResult> ListarCertificadoById([FromRoute] int id)
    {
        if (!await PodeAcessarCertificado(id)) return Forbid();

        var certificado = await _service.ListarCertificadoById(id);
        if (certificado.IsFailure) return NotFound(certificado.Errors);

        return Ok(certificado.Value);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeletarCertificado([FromRoute] int id)
    {
        if (!await PodeAcessarCertificado(id)) return Forbid();

        var response = await _service.DeletarCertificado(id);
        if (response.IsFailure) return NotFound(response.Errors);

        return Ok(response.Value);
    }

    [HttpPost("eventos/{eventoId}/alunos/{alunoId}/emitir")]
    [Authorize(Roles = "Admin,Secretaria")]
    public async Task<IActionResult> EmitirParaAluno([FromRoute] int eventoId, [FromRoute] int alunoId)
    {
        if (!await PodeAcessarEvento(eventoId)) return Forbid();

        var response = await _participacaoService.EmitirCertificadoAsync(alunoId, eventoId);
        if (response.IsFailure) return BadRequest(response.Errors);

        return Ok(response.Value);
    }

    private async Task<bool> PodeAcessarCertificado(int certificadoId)
    {
        var instituicaoId = await _db.Certificado
            .AsNoTracking()
            .Where(c => c.Id == certificadoId)
            .Select(c => c.Evento.InstituicaoId)
            .FirstOrDefaultAsync();

        return User.CanAccessInstituicao(instituicaoId);
    }

    private async Task<bool> PodeAcessarEvento(int eventoId)
    {
        var instituicaoId = await _db.Evento
            .AsNoTracking()
            .Where(e => e.Id == eventoId)
            .Select(e => e.InstituicaoId)
            .FirstOrDefaultAsync();

        return User.CanAccessInstituicao(instituicaoId);
    }
}
