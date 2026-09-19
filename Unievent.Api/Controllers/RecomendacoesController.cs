using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Unievent.Application.Rules;
using Unievent.Domain.Enuns;
using Unievent.Infra.Data;

namespace Unievent.Api.Controllers;

[ApiController]
[Route("api/eventos/recomendados")]
[Authorize(Roles = "Aluno")]
public class RecomendacoesController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult> Listar([FromQuery] int limite = 20)
    {
        limite = Math.Clamp(limite, 1, 50);
        var alunoId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var aluno = await db.Aluno.AsNoTracking().SingleOrDefaultAsync(a => a.Id == alunoId);
        if (aluno is null) return Unauthorized();
        var preferencia = await db.PreferenciaNotificacao.AsNoTracking().SingleOrDefaultAsync(p => p.AlunoId == alunoId);
        if (preferencia is { Recomendacoes: false }) return Ok(Array.Empty<object>());

        var agora = DateTime.UtcNow;
        var candidatos = await db.Evento.AsNoTracking()
            .Include(e => e.ResponsavelEvento)
            .Where(e => e.DataEvento > agora)
            .OrderBy(e => e.DataEvento).Take(200).ToListAsync();
        candidatos = candidatos.Where(e => EventoRules.PodeVisualizar(e, aluno)).ToList();

        var categorias = (preferencia?.Categorias ?? string.Empty).Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var recomendados = candidatos.Select(e =>
        {
            var score = categorias.Contains(e.Categoria.ToString(), StringComparer.OrdinalIgnoreCase) ? 40 : 0;
            var motivo = score > 0 ? $"Combina com seu interesse em {e.Categoria}" : "Evento próximo na agenda";
            double? distanciaKm = null;
            if (preferencia is { UsarLocalizacao: true, LatitudeAproximada: not null, LongitudeAproximada: not null } && e.Latitude.HasValue && e.Longitude.HasValue)
            {
                distanciaKm = DistanciaKm(preferencia.LatitudeAproximada.Value, preferencia.LongitudeAproximada.Value, e.Latitude.Value, e.Longitude.Value);
                if (distanciaKm <= preferencia.RaioKm) { score += 25; motivo += $" e está a {distanciaKm:0.#} km"; }
                else score -= 20;
            }
            if (e.DataEvento <= agora.AddDays(14)) score += 10;
            return new { evento = e, score, motivo, distanciaKm };
        }).OrderByDescending(x => x.score).ThenBy(x => x.evento.DataEvento).Take(limite);

        return Ok(recomendados);
    }

    private static double DistanciaKm(double lat1, double lon1, double lat2, double lon2)
    {
        const double r = 6371;
        static double Rad(double value) => value * Math.PI / 180;
        var dLat = Rad(lat2 - lat1);
        var dLon = Rad(lon2 - lon1);
        var a = Math.Pow(Math.Sin(dLat / 2), 2) + Math.Cos(Rad(lat1)) * Math.Cos(Rad(lat2)) * Math.Pow(Math.Sin(dLon / 2), 2);
        return r * 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
    }
}
