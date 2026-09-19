using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Unievent.Api.Security;
using Unievent.Application.Dtos.Aluno;
using Unievent.Application.Interfaces.Services;
using Unievent.Domain.Enuns;
using Unievent.Infra.Data;

namespace Unievent.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AlunoController : ControllerBase
{
    private readonly IAlunoService _service;
    private readonly AppDbContext _db;
    public AlunoController(IAlunoService service, AppDbContext db)
    {
        _service = service;
        _db = db;
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> CriarAluno([FromForm] AlunoRequest request)
    {
        if (request.TipoParticipante == TipoParticipante.Interno)
        {
            if (!request.InstituicaoId.HasValue)
            {
                return BadRequest("Selecione uma instituição ativa para realizar o cadastro.");
            }

            var instituicaoAtivaExiste = await _db.Instituicao
                .AsNoTracking()
                .AnyAsync(i => i.Id == request.InstituicaoId.Value);
            if (!instituicaoAtivaExiste)
            {
                return BadRequest("Instituição não encontrada ou inativa.");
            }
        }

        var aluno = await _service.CriarAluno(request);
        if (aluno.IsFailure)
        {
            return BadRequest(aluno.Errors);
        }
        return Ok(aluno.Value);

    }

    [HttpPatch("{id}")]
    [Consumes("multipart/form-data")]
    [Authorize]
    public async Task<IActionResult> AtualizarAluno([FromRoute] int id, [FromForm] AlunoUpdate update)
    {
        if (!await PodeAcessarAluno(id)) return Forbid();
        var aluno = await _service.AtualizarAluno(id, update);
        if (aluno.IsFailure)
        {
            return BadRequest(aluno.Errors);
        }
        return Ok(aluno.Value);
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Secretaria")]
    public async Task<IActionResult> ListarAlunos()
    {
        var alunos = await _service.ListarAlunos();
        if (alunos.IsFailure)
        {
            return NotFound(alunos.Errors);
        }
        var resultado = alunos.Value ?? [];
        if (!User.IsGlobalAdmin())
            resultado = resultado.Where(a => User.CanAccessInstituicao(a.InstituicaoId));

        return Ok(resultado);

    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> ListarAlunoById([FromRoute] int id)
    {
        if (!await PodeAcessarAluno(id)) return Forbid();
        var aluno = await _service.ListarAlunoById(id);
        if (aluno.IsFailure)
        {
            return NotFound(aluno.Errors);
        }
        return Ok(aluno.Value);

    }

    [HttpGet("me")]
    [Authorize(Roles = "Aluno")]
    public async Task<IActionResult> ObterMeuPerfil()
    {
        var alunoId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(alunoId, out var id)) return BadRequest("Aluno não autenticado");

        var aluno = await _db.Aluno
            .AsNoTracking()
            .Include(a => a.Instituicao)
            .FirstOrDefaultAsync(a => a.Id == id);
        if (aluno is null) return NotFound("Aluno não encontrado");

        return Ok(new AlunoResponse
        {
            Id = aluno.Id,
            Nome = aluno.Nome,
            Email = aluno.Email,
            EmailConfirmado = aluno.EmailConfirmado,
            FotoPerfil = aluno.FotoPerfil,
            IsAtivo = aluno.IsAtivo,
            Role = aluno.Role,
            DataNascimento = aluno.DataNascimento,
            TipoParticipante = aluno.TipoParticipante,
            InstituicaoId = aluno.InstituicaoId,
            InstituicaoNome = aluno.Instituicao?.Nome ?? aluno.Instituicao?.NomeAbreviado
        });
    }
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeletarAluno([FromRoute] int id)
    {
        if (!await PodeAcessarAluno(id)) return Forbid();

        var aluno = await _service.DeletarAluno(id);
        if (aluno.IsFailure)
        {
            return NotFound(aluno.Errors);
        }
        return Ok(aluno.Value);

    }

    private async Task<bool> PodeAcessarAluno(int alunoId)
    {
        if (User.IsInRole(Role.Aluno.ToString()))
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value == alunoId.ToString();

        var aluno = await _db.Aluno.AsNoTracking().FirstOrDefaultAsync(a => a.Id == alunoId);
        return aluno is not null && User.CanAccessInstituicao(aluno.InstituicaoId);
    }
}
