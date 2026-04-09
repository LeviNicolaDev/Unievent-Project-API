using Microsoft.AspNetCore.Mvc;
using Unievent.Application.Dtos.Endereco;
using Unievent.Application.Interfaces.Services;

namespace Unievent.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EnderecoController : ControllerBase
{
    private readonly IEnderecoService _service;
    public EnderecoController(IEnderecoService service)
    {
        _service = service;
    }

    [HttpPost("CriarEndereco")]
    public async Task<IActionResult> CriarEndereco([FromBody] EnderecoRequest request)
    {

        var endereco = await _service.CriarEndereco(request);
        if (endereco.IsFailure)
        {
            return BadRequest(endereco.Message);
        }
        return Ok(endereco.Data);

    }

    [HttpPatch("AtualizarEndereco/{id}")]
    public async Task<IActionResult> AtualizarEndereco([FromRoute] int id, [FromBody] EnderecoUpdate update)
    {

        var endereco = await _service.AtualizarEndereco(id, update);
        if (endereco.IsFailure)
        {
            return NotFound(endereco.Message);
        }
        return Ok(endereco.Data);

    }

    [HttpGet("ListarEnderecos")]
    public async Task<IActionResult> ListarEnderecos()
    {


        var enderecos = await _service.ListarEnderecos();
        if (enderecos.IsFailure)
        {
            return NotFound(enderecos.Message);
        }
        return Ok(enderecos.Data);


    }

    [HttpGet("ListarEnderecoById/{id}")]
    public async Task<IActionResult> ListarEnderecoById([FromRoute] int id)
    {
        var endereco = await _service.ListarEnderecoById(id);
        if (endereco.IsFailure)
        {
            return NotFound(endereco.Message);
        }
        return Ok(endereco.Data);

    }

    [HttpDelete("DeletarEndereco/{id}")]
    public async Task<IActionResult> DeletarEndereco([FromRoute] int id)
    {

        var response = await _service.DeletarEndereco(id);
        if (response.IsFailure)
        {
            return NotFound(response.Message);
        }
        return Ok(response.Message);


    }

}
