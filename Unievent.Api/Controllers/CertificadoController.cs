using Microsoft.AspNetCore.Mvc;
using Unievent.Application.Dtos.Certificado;
using Unievent.Application.Interfaces.Services;

namespace Unievent.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CertificadoController : ControllerBase
{

    private readonly ICertificadoService _service;
    public CertificadoController(ICertificadoService service)
    {
        _service = service;
    }

    [HttpPost("CriarCertificado")]
    public async Task<IActionResult> CriarCertificado([FromBody] CertificadoRequest request)
    {
        try
        {
            var certificado = await _service.CriarCertificado(request);
            return Ok(certificado);
        }
        catch (System.Exception)
        {

            throw new Exception("Erro ao criar certificado");
        }
    }

    [HttpPatch("AtualizarCertificado/{id}")]
    public async Task<IActionResult> AtualizarCertificado([FromRoute] int id, [FromBody] CertificadoUpdate update)
    {
        try
        {
            var certificado = await _service.AtualizarCertificado(id, update);
            return Ok(certificado);
        }
        catch (System.Exception)
        {

            throw new Exception("Erro ao atualizar certificado");
        }
    }

    [HttpGet("ListarCertificados")]
    public async Task<IActionResult> ListarCertificados()
    {
        try
        {
            var certificados = await _service.ListarCertificados();
            return Ok(certificados);
        }
        catch (System.Exception)
        {

            throw new Exception("Erro ao listar certificados");
        }
    }

    [HttpGet("ListarCertificadoById/{id}")]
    public async Task<IActionResult> ListarCertificadoById([FromRoute] int id)
    {
        try
        {
            var certificado = await _service.ListarCertificadoById(id);
            return Ok(certificado);
        }
        catch (System.Exception)
        {

            throw new Exception("Erro ao listar certificado por id");
        }
    }

    [HttpDelete("DeletarCertificado/{id}")]
    public async Task<IActionResult> DeletarCertificado([FromRoute] int id)
    {
        try
        {
            var response = await _service.DeletarCertificado(id);
            return Ok(response);
        }
        catch (System.Exception)
        {

            throw new Exception("Erro ao deletar certificado");
        }
    }


}
