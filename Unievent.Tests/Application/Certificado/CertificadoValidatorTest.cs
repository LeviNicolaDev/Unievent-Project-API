using FluentAssertions;
using Unievent.Application.Dtos.Certificado;
using Unievent.Application.Validators.Certificado;
using Xunit;

namespace Unievent.Tests.Application.Certificado;

public class CertificadoValidatorTest
{
    private readonly CertificadoRequestValidator _requestValidator;
    private readonly CertificadoUpdateValidator _updateValidator;
    public CertificadoValidatorTest()
    {
        _requestValidator = new CertificadoRequestValidator();
        _updateValidator = new CertificadoUpdateValidator();
    }

    [Fact]
    public async Task Deve_Validar_Dados_Com_Sucesso()
    {
        var request = new CertificadoRequest
        {
            Texto = "Certificado de participação",
            DataCertifcado = DateTime.Now.AddDays(-1),

            EventoId = 1
        };

        var result = await _requestValidator.ValidateAsync(request);
        result.IsValid.Should().BeTrue();
    }


    [Fact]
    public async Task Deve_Falhar_Quando_Dados_Foram_Informados_Incorretamente()
    {
        var request = new CertificadoRequest
        {
            Texto = "Certificado de participação",
            DataCertifcado = DateTime.Now.AddDays(+1),

            EventoId = 1
        };

        var result = await _requestValidator.ValidateAsync(request);
        result.IsValid.Should().BeFalse();

    }


}

