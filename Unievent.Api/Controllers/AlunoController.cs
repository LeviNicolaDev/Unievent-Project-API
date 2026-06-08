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

    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> CriarAluno([FromForm] AlunoRequest request)
    {
        var aluno = await _service.CriarAluno(request);
        if (aluno.IsFailure)
        {
            return BadRequest(aluno.Errors);
        }
        return Ok(aluno.Value);

    }

    [HttpPatch("{id}")]
    [Consumes("multipart/form-data")]

    public async Task<IActionResult> AtualizarAluno([FromRoute] int id, [FromForm] AlunoUpdate update)
    {
        var aluno = await _service.AtualizarAluno(id, update);
        if (aluno.IsFailure)
        {
            return BadRequest(aluno.Errors);
        }
        return Ok(aluno.Value);
    }

    [HttpGet]

    public async Task<IActionResult> ListarAlunos()
    {
        var alunos = await _service.ListarAlunos();
        if (alunos.IsFailure)
        {
            return NotFound(alunos.Errors);
        }
        return Ok(alunos.Value);

    }

    [HttpGet("{id}")]

    public async Task<IActionResult> ListarAlunoById([FromRoute] int id)
    {
        var aluno = await _service.ListarAlunoById(id);
        if (aluno.IsFailure)
        {
            return NotFound(aluno.Errors);
        }
        return Ok(aluno.Value);

    }
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeletarAluno([FromRoute] int id)
    {

        var aluno = await _service.DeletarAluno(id);
        if (aluno.IsFailure)
        {
            return NotFound(aluno.Errors);
        }
        return Ok(aluno.Value);

    }
}
