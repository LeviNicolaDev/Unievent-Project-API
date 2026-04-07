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
    public async Task<IActionResult> CriarAluno([FromForm] AlunoRequest request)
    {
        try
        {
            var aluno = await _service.CriarAluno(request);
            return Ok(aluno);
        }
        catch (System.Exception)
        {

            throw new Exception("Erro ao criar aluno");
        }
    }

    [HttpPatch("AtualizarAluno/{id}")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> AtualizarAluno([FromRoute] int id, [FromForm] AlunoUpdate update)
    {
        try
        {
            var aluno = await _service.AtualizarAluno(id, update);
            return Ok(aluno);
        }
        catch (System.Exception)
        {

            throw new Exception("Erro ao atualizar aluno");
        }
    }

    [HttpGet("ListarAlunos")]
    public async Task<IActionResult> ListarAlunos()
    {
        try
        {
            var alunos = await _service.ListarAlunos();
            return Ok(alunos);
        }
        catch (System.Exception)
        {

            throw new Exception("Erro ao listar alunos");
        }
    }

    [HttpGet("ListarAlunoById/{id}")]
    public async Task<IActionResult> ListarAlunoById([FromRoute] int id)
    {
        try
        {
            var aluno = await _service.ListarAlunoById(id);
            return Ok(aluno);
        }
        catch (System.Exception)
        {

            throw new Exception("Erro ao listar aluno por ID");
        }
    }
    [HttpDelete("DeletarAluno/{id}")]
    public async Task<IActionResult> DeletarAluno([FromRoute] int id)
    {
        try
        {
            await _service.DeletarAluno(id);
            return Ok();
        }
        catch (System.Exception)
        {

            throw new Exception("Erro ao deletar aluno");
        }
    }
}
