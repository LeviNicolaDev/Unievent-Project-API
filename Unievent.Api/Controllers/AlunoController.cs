using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unievent.Application.Dtos.Aluno;
using Unievent.Application.Interfaces.Services;

namespace Unievent.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AlunoController : ControllerBase
{
    private readonly IAlunoService _service;
    public AlunoController(IAlunoService service)
    {
        _service = service;
    }

    [HttpPost("CriarAluno")]
    [Consumes("multipart/form-data")]
    [Authorize(Roles = "Admin,Secretaria")]
    public async Task<IActionResult> CriarAluno([FromForm] AlunoRequest request)
    {
        var aluno = await _service.CriarAluno(request);
        if (aluno.IsFailure)
        {
            return BadRequest(aluno.Message);
        }
        return Ok(aluno.Data);

    }

    [HttpPatch("AtualizarAluno/{id}")]
    [Consumes("multipart/form-data")]
    [Authorize(Roles = "Admin,Secretaria")]
    public async Task<IActionResult> AtualizarAluno([FromRoute] int id, [FromForm] AlunoUpdate update)
    {
        var aluno = await _service.AtualizarAluno(id, update);
        if (aluno.IsFailure)
        {
            return BadRequest(aluno.Message);
        }
        return Ok(aluno.Data);
    }

    [HttpGet("ListarAlunos")]
    [Authorize(Roles = "Admin,Secretaria")]
    public async Task<IActionResult> ListarAlunos()
    {
        var alunos = await _service.ListarAlunos();
        if (alunos.IsFailure)
        {
            return NotFound(alunos.Message);
        }
        return Ok(alunos.Data);

    }

    [HttpGet("ListarAlunoById/{id}")]
    [Authorize(Roles = "Admin,Secretaria")]
    public async Task<IActionResult> ListarAlunoById([FromRoute] int id)
    {
        var aluno = await _service.ListarAlunoById(id);
        if (aluno.IsFailure)
        {
            return NotFound(aluno.Message);
        }
        return Ok(aluno.Data);

    }
    [HttpDelete("DeletarAluno/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeletarAluno([FromRoute] int id)
    {

        var aluno = await _service.DeletarAluno(id);
        if (aluno.IsFailure)
        {
            return NotFound(aluno.Message);
        }
        return Ok(aluno.Message);

    }
}
