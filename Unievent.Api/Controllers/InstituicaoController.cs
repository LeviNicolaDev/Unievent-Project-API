using Microsoft.AspNetCore.Authorization;
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
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CriarInstituicao([FromForm] InstituicaoRequest request)
    {
        var instituicao = await _service.CriarInstituicao(request);
        if (instituicao.IsFailure)
        {
            return BadRequest(instituicao.Message);
        }
        return Ok(instituicao.Data);


    }

    [HttpPatch("AtualizarInstituicao/{id}")]
    [Consumes("multipart/form-data")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AtualizarInstituicao([FromRoute] int id, [FromForm] InstituicaoUpdate update)
    {

        var instituicao = await _service.AtualizarInstituicao(id, update);
        if (instituicao.IsFailure)
        {
            return BadRequest(instituicao.Message);
        }
        return Ok(instituicao.Data);



    }

    [HttpGet("ListarInstituicoes")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ListarInstituicoes()
    {

        var instituicoes = await _service.ListarInstituicoes();
        if (instituicoes.IsFailure)
        {
            return NotFound(instituicoes.Message);
        }
        return Ok(instituicoes.Data);



    }

    [HttpGet("ListarInstituicaoById/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ListarInstituicaoById([FromRoute] int id)
    {

        var instituicao = await _service.ListarInstituicaoById(id);
        if (instituicao.IsFailure)
        {
            return NotFound(instituicao.Message);
        }
        return Ok(instituicao.Data);


    }

    [HttpDelete("DeletarInstituicao/{id}")]
    [Authorize(Roles = "Admin,Secretaria")]
    public async Task<IActionResult> DeletarInstituicao([FromRoute] int id)
    {
        var response = await _service.DeletarInstituicao(id);
        if (response.IsFailure)
        {
            return NotFound(response.Message);
        }
        return Ok(response.Message);

    }


}
