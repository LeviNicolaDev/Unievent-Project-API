using Microsoft.AspNetCore.Mvc;
using Unievent.Application.Dtos.Instituicao;
using Unievent.Application.Interfaces.Services;

namespace Unievent.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InstituicaoController : ControllerBase
{
    private readonly IInstituicaoService _service;
    public InstituicaoController(IInstituicaoService service)
    {
        _service = service;
    }
    [HttpPost("CriarInstituicao")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> CriarInstituicao([FromForm] InstituicaoRequest request)
    {
        try
        {
            var instituicao = await _service.CriarInstituicao(request);
            return Ok(instituicao);
        }
        catch (System.Exception ex)
        {

            throw new Exception("Erro ao criar instituicao", ex);
        }
    }

    [HttpPatch("AtualizarInstituicao/{id}")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> AtualizarInstituicao([FromRoute] int id, [FromForm] InstituicaoUpdate update)
    {
        try
        {
            var instituicao = await _service.AtualizarInstituicao(id, update);
            return Ok(instituicao);
        }
        catch (System.Exception)
        {

            throw new Exception("Erro ao atualizar instituicao");
        }
    }

    [HttpGet("ListarInstituicoes")]
    public async Task<IActionResult> ListarInstituicoes()
    {
        try
        {
            var instituicoes = await _service.ListarInstituicoes();
            return Ok(instituicoes);
        }
        catch (System.Exception)
        {

            throw new Exception("Erro ao listar instituicoes");
        }
    }

    [HttpGet("ListarInstituicaoById/{id}")]
    public async Task<IActionResult> ListarInstituicaoById([FromRoute] int id)
    {
        try
        {
            var instituicao = await _service.ListarInstituicaoById(id);
            return Ok(instituicao);
        }
        catch (System.Exception)
        {

            throw new Exception("Erro ao listar instituicao");
        }
    }

    [HttpDelete("DeletarInstituicao/{id}")]
    public async Task<IActionResult> DeletarInstituicao([FromRoute] int id)
    {
        try
        {
            var response = await _service.DeletarInstituicao(id);
            return Ok(response);
        }
        catch (System.Exception)
        {

            throw new Exception("Erro ao deletar instituicao");
        }
    }


}
