using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Unievent.Api.Data;
using Unievent.Api.Security;
using Unievent.Application.Dtos.Instituicao;
using Unievent.Application.Interfaces.Services;
using Unievent.Domain.Enuns;
using Unievent.Infra.Data;

namespace Unievent.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InstituicaoController : ControllerBase
{
    private readonly IInstituicaoService _service;
    private readonly AppDbContext _db;
    public InstituicaoController(IInstituicaoService service, AppDbContext db)
    {
        _service = service;
        _db = db;
    }
    [HttpPost]
    [Consumes("multipart/form-data")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CriarInstituicao([FromForm] InstituicaoRequest request)
    {
        if (!User.IsGlobalAdmin()) return Forbid();
        var instituicao = await _service.CriarInstituicao(request);
        if (instituicao.IsFailure)
        {
            return BadRequest(instituicao.Errors);
        }
        return Ok(instituicao.Value);


    }


    [HttpPatch("{id}")]
    [Consumes("multipart/form-data")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AtualizarInstituicao([FromRoute] int id, [FromForm] InstituicaoUpdate update)
    {
        if (!User.CanAccessInstituicao(id)) return Forbid();

        var instituicao = await _service.AtualizarInstituicao(id, update);
        if (instituicao.IsFailure)
        {
            return BadRequest(instituicao.Errors);
        }
        return Ok(instituicao.Value);



    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ListarInstituicoes()
    {

        var instituicoes = await _service.ListarInstituicoes();
        if (instituicoes.IsFailure)
        {
            return NotFound(instituicoes.Errors);
        }
        var resultado = instituicoes.Value ?? [];
        if (!User.IsGlobalAdmin())
            resultado = resultado.Where(i => User.CanAccessInstituicao(i.Id));

        return Ok(resultado);



    }

    [HttpGet("publicas")]
    [AllowAnonymous]
    public async Task<IActionResult> ListarInstituicoesPublicas()
    {
        var instituicoes = await _service.ListarInstituicoes();
        if (instituicoes.IsFailure)
        {
            return NotFound(instituicoes.Errors);
        }

        var cadastradas = (instituicoes.Value ?? []).ToList();
        var cadastradasPorCodigo = cadastradas
            .Where(i => !string.IsNullOrWhiteSpace(i.Codigo))
            .GroupBy(i => FatecInstitutionCatalog.NormalizeKey(i.Codigo))
            .ToDictionary(g => g.Key, g => g.First());
        var cadastradasPorNome = cadastradas
            .Where(i => !string.IsNullOrWhiteSpace(i.Nome ?? i.NomeAbreviado))
            .GroupBy(i => FatecInstitutionCatalog.NormalizeKey(i.Nome ?? i.NomeAbreviado))
            .ToDictionary(g => g.Key, g => g.First());
        var eventosPorInstituicao = await _db.Evento
            .AsNoTracking()
            .Where(e => e.InstituicaoId.HasValue &&
                        e.Visibilidade == VisibilidadeEvento.Publico)
            .GroupBy(e => e.InstituicaoId!.Value)
            .Select(g => new { InstituicaoId = g.Key, Total = g.Count() })
            .ToDictionaryAsync(g => g.InstituicaoId, g => g.Total);
        var idsIncluidos = new HashSet<int>();
        var resultado = new List<InstituicaoPublicaResponse>();

        foreach (var item in FatecInstitutionCatalog.Items)
        {
            cadastradasPorCodigo.TryGetValue(FatecInstitutionCatalog.NormalizeKey(item.Codigo), out var cadastrada);
            cadastrada ??= cadastradasPorNome.GetValueOrDefault(FatecInstitutionCatalog.NormalizeKey(item.Nome));
            if (cadastrada is not null) idsIncluidos.Add(cadastrada.Id);

            var totalEventos = cadastrada is null ? 0 : eventosPorInstituicao.GetValueOrDefault(cadastrada.Id);
            resultado.Add(new InstituicaoPublicaResponse
            {
                Id = cadastrada?.Id,
                Codigo = item.Codigo,
                Nome = item.Nome,
                NomeAbreviado = cadastrada?.NomeAbreviado,
                Cidade = cadastrada?.Cidade ?? item.Cidade,
                Estado = cadastrada?.Estado ?? item.Estado,
                TemEventos = totalEventos > 0,
                TotalEventos = totalEventos
            });
        }

        foreach (var instituicao in cadastradas.Where(i => !idsIncluidos.Contains(i.Id)))
        {
            var codigo = !string.IsNullOrWhiteSpace(instituicao.Codigo)
                ? instituicao.Codigo
                : FatecInstitutionCatalog.NormalizeKey(instituicao.Nome ?? instituicao.NomeAbreviado ?? $"instituicao-{instituicao.Id}");
            var totalEventos = eventosPorInstituicao.GetValueOrDefault(instituicao.Id);
            resultado.Add(new InstituicaoPublicaResponse
            {
                Id = (int?)instituicao.Id,
                Codigo = codigo,
                Nome = instituicao.Nome ?? instituicao.NomeAbreviado ?? $"Instituição #{instituicao.Id}",
                NomeAbreviado = instituicao.NomeAbreviado,
                Cidade = instituicao.Cidade,
                Estado = instituicao.Estado,
                TemEventos = totalEventos > 0,
                TotalEventos = totalEventos
            });
        }

        return Ok(resultado.OrderBy(i => i.Cidade).ThenBy(i => i.Nome));
    }

    [HttpGet("ativas")]
    [AllowAnonymous]
    public async Task<IActionResult> ListarInstituicoesAtivas()
    {
        var instituicoes = await _db.Instituicao
            .AsNoTracking()
            .OrderBy(i => i.Nome)
            .Select(i => new InstituicaoOpcaoResponse
            {
                Id = i.Id,
                Nome = i.Nome ?? i.NomeAbreviado ?? $"Instituição #{i.Id}",
                Sigla = i.NomeAbreviado ?? i.Codigo
            })
            .ToListAsync();

        return Ok(instituicoes);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ListarInstituicaoById([FromRoute] int id)
    {
        if (!User.CanAccessInstituicao(id)) return Forbid();

        var instituicao = await _service.ListarInstituicaoById(id);
        if (instituicao.IsFailure)
        {
            return NotFound(instituicao.Errors);
        }
        return Ok(instituicao.Value);


    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeletarInstituicao([FromRoute] int id)
    {
        if (!User.IsGlobalAdmin()) return Forbid();
        var response = await _service.DeletarInstituicao(id);
        if (response.IsFailure)
        {
            return NotFound(response.Errors);
        }
        return Ok(response.Value);

    }

    [HttpPatch("{id}/ativar")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AtivarInstituicao([FromRoute] int id)
    {
        if (!User.IsGlobalAdmin()) return Forbid();
        var response = await _service.AlterarStatusInstituicao(id, true);
        if (response.IsFailure) return NotFound(response.Errors);
        return Ok(response.Value);
    }

    [HttpPatch("{id}/desativar")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DesativarInstituicao([FromRoute] int id)
    {
        if (!User.IsGlobalAdmin()) return Forbid();
        var response = await _service.AlterarStatusInstituicao(id, false);
        if (response.IsFailure) return NotFound(response.Errors);
        return Ok(response.Value);
    }


}
