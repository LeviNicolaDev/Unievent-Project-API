using Microsoft.AspNetCore.Authorization;
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
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CriarEndereco([FromBody] EnderecoRequest request)
    {

        var endereco = await _service.CriarEndereco(request);
        if (endereco.IsFailure)
        {
            return BadRequest(endereco.Errors);
        }
        return Ok(endereco.Value);

    }

    [HttpPatch("AtualizarEndereco/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AtualizarEndereco([FromRoute] int id, [FromBody] EnderecoUpdate update)
    {

        var endereco = await _service.AtualizarEndereco(id, update);
        if (endereco.IsFailure)
        {
            return NotFound(endereco.Errors);
        }
        return Ok(endereco.Value);

    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ListarEnderecos()
    {


        var enderecos = await _service.ListarEnderecos();
        if (enderecos.IsFailure)
        {
            return NotFound(enderecos.Errors);
        }
        return Ok(enderecos.Value);


    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ListarEnderecoById([FromRoute] int id)
    {
        var endereco = await _service.ListarEnderecoById(id);
        if (endereco.IsFailure)
        {
            return NotFound(endereco.Errors);
        }
        return Ok(endereco.Value);

    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeletarEndereco([FromRoute] int id)
    {

        var response = await _service.DeletarEndereco(id);
        if (response.IsFailure)
        {
            return BadRequest(response.Errors);
        }
        return Ok(response.Value);


    }

}
