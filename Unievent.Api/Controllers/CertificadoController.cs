using Microsoft.AspNetCore.Authorization;
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

    [HttpPost]
    [Authorize(Roles = "Admin,Secretaria")]
    public async Task<IActionResult> CriarCertificado([FromBody] CertificadoRequest request)
    {
        var certificado = await _service.CriarCertificado(request);
        if (certificado.IsFailure)
        {
            return BadRequest(certificado.Errors);
        }

        return Ok(certificado.Value);


    }

    [HttpPatch("{id}")]
    [Authorize(Roles = "Admin,Secretaria")]
    public async Task<IActionResult> AtualizarCertificado([FromRoute] int id, [FromBody] CertificadoUpdate update)
    {
        var certificado = await _service.AtualizarCertificado(id, update);
        if (certificado.IsFailure)
        {
            return BadRequest(certificado.Errors);
        }


        return Ok(certificado.Value);

    }

    [HttpGet]

    public async Task<IActionResult> ListarCertificados()
    {
        var certificados = await _service.ListarCertificados();
        if (certificados.IsFailure)
        {
            return NotFound(certificados.Errors);
        }

        return Ok(certificados.Value);


    }

    [HttpGet("{id}")]

    public async Task<IActionResult> ListarCertificadoById([FromRoute] int id)
    {
        var certificado = await _service.ListarCertificadoById(id);
        if (certificado.IsFailure)
        {
            return NotFound(certificado.Errors);
        }

        return Ok(certificado.Value);


    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeletarCertificado([FromRoute] int id)
    {
        var response = await _service.DeletarCertificado(id);
        if (response.IsFailure)
        {
            return NotFound(response.Errors);
        }
        return Ok(response.Value);



    }


}
