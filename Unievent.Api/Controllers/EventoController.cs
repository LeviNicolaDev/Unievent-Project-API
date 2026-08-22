using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Unievent.Api.Data;
using Unievent.Api.Security;
using Unievent.Application.Dtos.Common;
using Unievent.Application.Dtos.Evento;
using Unievent.Application.Interfaces.Services;
using Unievent.Application.Rules;
using Unievent.Domain.Entities;
using Unievent.Domain.Enuns;
using Unievent.Infra.Data;

namespace Unievent.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventoController(IEventoService _service, IParticipacaoService participacaoService, AppDbContext db) : ControllerBase
    {
        [HttpPost]
        [Consumes("multipart/form-data")]
        [Authorize(Roles = "Admin,Secretaria")]
        public async Task<IActionResult> CriarEvento([FromForm] EventoRequest request)
        {
            if (!AplicarInstituicaoPermitida(request))
                return Forbid();
            if (!await ResponsavelPertenceAoEscopo(request.ResponsavelEventoId, request.InstituicaoId))
                return BadRequest("Responsável não pertence à instituição informada");

            var response = await _service.CriarEvento(request);
            if (response.IsFailure)
            {
                return BadRequest(response.Errors);
            }
            return Ok(response.Value);
        }

        [HttpPut("{id}")]
        [Consumes("multipart/form-data")]
        [Authorize(Roles = "Admin,Secretaria")]
        public async Task<IActionResult> AtualizarEvento(int id, [FromForm] EventoUpdate update)
        {
            var eventoAtual = await db.Evento.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
            if (eventoAtual is null) return NotFound("Evento não encontrado");
            if (!User.CanAccessInstituicao(eventoAtual.InstituicaoId)) return Forbid();
            if (update.InstituicaoId.HasValue && !User.CanAccessInstituicao(update.InstituicaoId)) return Forbid();
            if (!User.IsGlobalAdmin())
            {
                update.InstituicaoId = eventoAtual.InstituicaoId ?? User.GetInstituicaoId();
            }
            if (update.ResponsavelEventoId.HasValue &&
                !await ResponsavelPertenceAoEscopo(update.ResponsavelEventoId.Value, update.InstituicaoId ?? eventoAtual.InstituicaoId))
                return BadRequest("Responsável não pertence à instituição informada");

            var response = await _service.AtualizarEvento(id, update);
            if (response.IsFailure)
            {
                return NotFound(response.Errors);
            }
            return Ok(response.Value);
        }

        [HttpGet]

        public async Task<IActionResult> ListarEventos([FromQuery] EventoBuscaRequest filtros)
        {
            if (Request.Query.Count > 0)
            {
                var busca = await BuscarEventos(filtros);
                return Ok(busca);
            }

            var administrativo = User.IsInRole(Role.Admin.ToString()) || User.IsInRole(Role.Secretaria.ToString());
            TipoParticipante? tipo = Enum.TryParse<TipoParticipante>(
                User.FindFirst("tipo_participante")?.Value, out var tipoClaim) ? tipoClaim : null;
            var response = administrativo
                ? await _service.ListarEventos()
                : await _service.ListarEventosDisponiveis(tipo);
            if (response.IsFailure)
            {
                return NotFound(response.Errors);
            }
            var eventos = response.Value ?? [];
            if (administrativo && !User.IsGlobalAdmin())
                eventos = eventos.Where(e => User.CanAccessInstituicao(e.InstituicaoId));

            return Ok(eventos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ListarEventoById(int id)
        {
            var response = await _service.ListarEventoById(id);
            if (response.IsFailure)
            {
                return NotFound(response.Errors);
            }
            if (!PodeVisualizarEventoResponse(response.Value!)) return Forbid();
            return Ok(response.Value);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Secretaria")]
        public async Task<IActionResult> DeletarEvento(int id)
        {

            var evento = await db.Evento.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
            if (evento is null) return NotFound("Evento não encontrado");
            if (!User.CanAccessInstituicao(evento.InstituicaoId)) return Forbid();

            var response = await _service.DeletarEvento(id);
            if (response.IsFailure)
            {
                return NotFound(response.Errors);
            }
            return Ok(response.Value);
        }

        [HttpGet("categorias")]
        public async Task<ActionResult<IEnumerable<string>>> GetCategorias()
        {
            var result = Enum.GetNames(typeof(Categoria));
            return Ok(result);
        }

        [HttpGet("eventos")]
        [Authorize(Roles = "Admin,Secretaria")]
        public async Task<ActionResult<IList<EventoResponse>>> GetEventosByCategoria([FromQuery] Categoria? categoria)
        {
            if (categoria is null)
                return BadRequest("Categoria é obrigatória");

            var result = await _service.ListarEventosByCategoria(categoria.Value);

            if (result.IsFailure)
                return BadRequest(result.Errors);

            var eventos = result.Value ?? [];
            if (!User.IsGlobalAdmin())
                eventos = eventos.Where(e => User.CanAccessInstituicao(e.InstituicaoId)).ToList();

            return Ok(eventos);
        }

        [HttpPost("{id}/inscrever-se")]
        [Authorize(Roles = "Aluno")]
        public async Task<ActionResult> InscreverEvento([FromRoute] int id)
        {
            var aluno = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (aluno is null) return BadRequest("Aluno não autenticado");
            var result = await participacaoService.InscreverAsync(int.Parse(aluno), id);
            if (result.IsFailure)
            {
                return BadRequest(result.Errors);
            }
            return Ok(result.Value);
        }

        [HttpGet("{id}/ingresso")]
        [Authorize(Roles = "Aluno")]
        public async Task<ActionResult> ObterIngresso([FromRoute] int id)
        {
            var aluno = int.Parse(
        User.FindFirst(ClaimTypes.NameIdentifier)!.Value
    );
            var result = await participacaoService.ObterIngressoAsync(aluno, id);
            if (result.IsFailure)
            {
                return BadRequest(result.Errors);
            }
            return Ok(result.Value);
        }

        [HttpPost("check-in")]
        [Authorize(Roles = "Admin,Secretaria,OperadorCheckIn")]
        public async Task<ActionResult> ValidarCheckIn([FromBody] Application.Dtos.Participacao.CheckInRequest request)
        {
            if (!User.IsGlobalAdmin())
            {
                if (string.IsNullOrWhiteSpace(request.CodigoIngresso))
                    return BadRequest("Código do ingresso é obrigatório");

                var ingresso = await db.Participacao
                    .AsNoTracking()
                    .Where(p => p.CodigoIngresso == request.CodigoIngresso)
                    .Select(p => new
                    {
                        p.EventoId,
                        p.Evento.InstituicaoId
                    })
                    .FirstOrDefaultAsync();

                if (ingresso is null) return BadRequest("Ingresso inválido");
                if (request.EventoId.HasValue && request.EventoId.Value != ingresso.EventoId)
                    return BadRequest("Ingresso não pertence ao evento informado");
                if (!User.CanAccessInstituicao(ingresso.InstituicaoId)) return Forbid();
            }

            var operador = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await participacaoService.ValidarCheckInAsync(operador, request);
            return result.IsFailure ? BadRequest(result.Errors) : Ok(result.Value);
        }

        private bool AplicarInstituicaoPermitida(EventoRequest request)
        {
            if (User.IsGlobalAdmin()) return request.InstituicaoId.HasValue;

            var instituicaoId = User.GetInstituicaoId();
            if (!instituicaoId.HasValue) return false;
            if (request.InstituicaoId.HasValue && request.InstituicaoId != instituicaoId) return false;

            request.InstituicaoId = instituicaoId.Value;
            return true;
        }

        private async Task<bool> ResponsavelPertenceAoEscopo(int responsavelId, int? instituicaoId)
        {
            var responsavel = await db.ResponsavelEvento.AsNoTracking().FirstOrDefaultAsync(r => r.Id == responsavelId);
            if (responsavel is null) return false;
            return User.IsGlobalAdmin() ||
                   responsavel.InstituicaoId == instituicaoId;
        }

        private bool PodeVisualizarEventoResponse(EventoResponse evento)
        {
            if (User.IsInRole(Role.Admin.ToString()) || User.IsInRole(Role.Secretaria.ToString()))
                return User.CanAccessInstituicao(evento.InstituicaoId);

            var tipo = Enum.TryParse<TipoParticipante>(User.FindFirst("tipo_participante")?.Value, out var tipoClaim)
                ? tipoClaim
                : (TipoParticipante?)null;
            var instituicaoId = User.GetInstituicaoId();
            return evento.Visibilidade == VisibilidadeEvento.Publico ||
                   tipo == TipoParticipante.Interno &&
                   instituicaoId.HasValue &&
                   evento.Visibilidade == VisibilidadeEvento.Privado &&
                   evento.InstituicaoId == instituicaoId;
        }

        private async Task<PagedResponse<EventoResponse>> BuscarEventos(EventoBuscaRequest filtros)
        {
            var page = Math.Max(filtros.Page, 1);
            var pageSize = Math.Clamp(filtros.PageSize, 1, 50);
            var query = db.Evento
                .AsNoTracking()
                .Include(e => e.ResponsavelEvento)
                .Include(e => e.Instituicao)
                .AsQueryable();

            if (User.IsInRole(Role.Admin.ToString()) || User.IsInRole(Role.Secretaria.ToString()))
            {
                if (!User.IsGlobalAdmin())
                    query = query.Where(e => e.InstituicaoId == User.GetInstituicaoId());
                else if (filtros.InstituicaoId.HasValue)
                    query = query.Where(e => e.InstituicaoId == filtros.InstituicaoId);
            }
            else
            {
                var tipo = Enum.TryParse<TipoParticipante>(User.FindFirst("tipo_participante")?.Value, out var tipoClaim)
                    ? tipoClaim
                    : (TipoParticipante?)null;
                var instituicaoId = User.GetInstituicaoId();
                query = query.Where(EventoRules.CatalogoVisivelExpression(tipo, instituicaoId));

                if (filtros.InstituicaoId.HasValue)
                    query = query.Where(e => e.InstituicaoId == filtros.InstituicaoId);
            }

            if (!filtros.InstituicaoId.HasValue && !string.IsNullOrWhiteSpace(filtros.InstituicaoCodigo))
            {
                var catalogo = FatecInstitutionCatalog.FindByCode(filtros.InstituicaoCodigo);
                query = catalogo is null
                    ? query.Where(e => false)
                    : query.Where(e => e.Instituicao != null &&
                                       (e.Instituicao.Codigo == catalogo.Codigo ||
                                        e.Instituicao.Nome == catalogo.Nome ||
                                        e.Instituicao.NomeAbreviado == catalogo.Nome));
            }

            if (filtros.Categoria.HasValue) query = query.Where(e => e.Categoria == filtros.Categoria);
            if (filtros.Publico.HasValue) query = query.Where(EventoRules.FiltroPublicoExpression(filtros.Publico.Value));
            if (filtros.Inicio.HasValue) query = query.Where(e => e.DataEvento >= filtros.Inicio.Value);
            if (filtros.Fim.HasValue) query = query.Where(e => e.DataEvento <= filtros.Fim.Value);
            if (!string.IsNullOrWhiteSpace(filtros.Cidade))
                query = query.Where(e => e.Instituicao != null && e.Instituicao.Cidade.Contains(filtros.Cidade));
            if (!string.IsNullOrWhiteSpace(filtros.Search))
                query = query.Where(e => e.Nome.Contains(filtros.Search) || e.Descricao.Contains(filtros.Search));
            if (!string.IsNullOrWhiteSpace(filtros.Curso))
                query = query.Where(e => e.Nome.Contains(filtros.Curso) || e.Descricao.Contains(filtros.Curso));

            var total = await query.CountAsync();
            var eventos = await query
                .OrderBy(e => e.DataEvento)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResponse<EventoResponse>
            {
                Items = eventos.Select(MapEvento).ToList(),
                Page = page,
                PageSize = pageSize,
                TotalItems = total,
                TotalPages = total == 0 ? 0 : (int)Math.Ceiling(total / (double)pageSize)
            };
        }

        private static EventoResponse MapEvento(Evento e)
        {
            return new EventoResponse
            {
                Id = e.Id,
                Nome = e.Nome,
                Descricao = e.Descricao,
                Local = e.Local,
                Categoria = e.Categoria,
                DataEvento = e.DataEvento,
                Capacidade = e.Capacidade,
                VagasDisponiveis = null,
                Thumbnail = e.Thumbnail.ToList(),
                IdResponsavelEvento = e.ResponsavelEventoId,
                Responsavel = e.ResponsavelEvento?.Nome ?? string.Empty,
                InstituicaoId = e.InstituicaoId,
                InstituicaoNome = e.Instituicao?.Nome ?? e.Instituicao?.NomeAbreviado,
                Cidade = e.Instituicao?.Cidade,
                Estado = e.Instituicao?.Estado,
                Visibilidade = e.Visibilidade,
                PublicoPermitido = e.PublicoPermitido,
                InicioInscricoes = e.InicioInscricoes,
                FimInscricoes = e.FimInscricoes,
                Latitude = e.Latitude,
                Longitude = e.Longitude,
                RaioCheckInMetros = e.RaioCheckInMetros
            };
        }
    }
}
