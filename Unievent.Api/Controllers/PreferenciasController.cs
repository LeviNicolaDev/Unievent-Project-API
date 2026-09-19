using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Unievent.Domain.Entities;
using Unievent.Infra.Data;

namespace Unievent.Api.Controllers;

[ApiController]
[Route("api/me/preferencias")]
[Authorize(Roles = "Aluno")]
public class PreferenciasController(AppDbContext db) : ControllerBase
{
    private int AlunoId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<ActionResult> Obter()
    {
        var item = await db.PreferenciaNotificacao.AsNoTracking().SingleOrDefaultAsync(x => x.AlunoId == AlunoId);
        return Ok(item ?? new PreferenciaNotificacao { AlunoId = AlunoId });
    }

    [HttpPut]
    public async Task<ActionResult> Atualizar([FromBody] PreferenciaRequest request)
    {
        if (request.RaioKm is < 1 or > 500) return BadRequest("O raio deve estar entre 1 e 500 km");
        if (request.LatitudeAproximada is < -90 or > 90 || request.LongitudeAproximada is < -180 or > 180)
            return BadRequest("Localização inválida");

        var item = await db.PreferenciaNotificacao.SingleOrDefaultAsync(x => x.AlunoId == AlunoId)
            ?? new PreferenciaNotificacao { AlunoId = AlunoId };
        item.LembretesEventos = request.LembretesEventos;
        item.AlertasCertificados = request.AlertasCertificados;
        item.Recomendacoes = request.Recomendacoes;
        item.UsarLocalizacao = request.UsarLocalizacao;
        item.Categorias = string.Join(',', request.Categorias.Distinct(StringComparer.OrdinalIgnoreCase));
        item.LatitudeAproximada = request.UsarLocalizacao ? request.LatitudeAproximada : null;
        item.LongitudeAproximada = request.UsarLocalizacao ? request.LongitudeAproximada : null;
        item.RaioKm = request.RaioKm;
        item.AtualizadoEmUtc = DateTime.UtcNow;
        if (item.Id == 0) db.PreferenciaNotificacao.Add(item);
        await db.SaveChangesAsync();
        return Ok(item);
    }
}

public record PreferenciaRequest(
    bool LembretesEventos,
    bool AlertasCertificados,
    bool Recomendacoes,
    bool UsarLocalizacao,
    string[] Categorias,
    double? LatitudeAproximada,
    double? LongitudeAproximada,
    int RaioKm = 30);
