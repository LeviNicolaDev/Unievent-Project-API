using System.Data;
using FluentValidation;
using Unievent.Application.Dtos.Certificado;

namespace Unievent.Application.Validators.Certificado;

public class CertificadoRequestValidator : AbstractValidator<CertificadoRequest>
{

    public CertificadoRequestValidator()
    {
        RuleFor(x => x.Texto)
            .NotEmpty().WithMessage("O campo 'Texto' é obrigatório.")
            .MaximumLength(500).WithMessage("O campo 'Texto' deve conter no máximo 500 caracteres.");
        RuleFor(x => x.DataCertifcado)
            .NotEmpty().WithMessage("O campo 'DataCertifcado' é obrigatório.")
            .LessThanOrEqualTo(DateTime.Now).WithMessage("A data do certificado não pode ser futura.");
        RuleFor(x => x.EventoId)
            .NotEmpty().WithMessage("O campo 'EventoId' é obrigatório.")
            .GreaterThan(0).WithMessage("O campo 'EventoId' deve ser um número positivo.");
    }
}
