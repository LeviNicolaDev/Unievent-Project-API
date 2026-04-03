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
        try
        {
            var endereco = await _service.CriarEndereco(request);
            return Ok(endereco);
        }
        catch (System.Exception)
        {

            throw new Exception("Erro ao criar endereco");
        }
    }

    [HttpPatch("AtualizarEndereco/{id}")]
    public async Task<IActionResult> AtualizarEndereco([FromRoute] int id, [FromBody] EnderecoUpdate update)
    {
        try
        {
            var endereco = await _service.AtualizarEndereco(id, update);
            return Ok(endereco);
        }
        catch (System.Exception)
        {

            throw new Exception("Erro ao atualizar endereco");
        }
    }

    [HttpGet("ListarEnderecos")]
    public async Task<IActionResult> ListarEnderecos()
    {
        try
        {
            var enderecos = await _service.ListarEnderecos();
            return Ok(enderecos);
        }
        catch (System.Exception)
        {

            throw new Exception("Erro ao listar enderecos");
        }
    }

    [HttpGet("ListarEnderecoById/{id}")]
    public async Task<IActionResult> ListarEnderecoById([FromRoute] int id)
    {
        try
        {
            var endereco = await _service.ListarEnderecoById(id);
            return Ok(endereco);
        }
        catch (System.Exception)
        {

            throw new Exception("Erro ao listar endereco");
        }
    }

    [HttpDelete("DeletarEndereco/{id}")]
    public async Task<IActionResult> DeletarEndereco([FromRoute] int id)
    {
        try
        {
            var response = await _service.DeletarEndereco(id);
            return Ok(response);
        }
        catch (System.Exception)
        {

            throw new Exception("Erro ao deletar endereco");
        }
    }

}
