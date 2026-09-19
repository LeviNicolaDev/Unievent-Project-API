using FluentValidation;
using Unievent.Application.Dtos.Certificado;

namespace Unievent.Application.Validators.Certificado;

public class CertificadoUpdateValidator : AbstractValidator<CertificadoUpdate>
{
    public CertificadoUpdateValidator()
    {
        RuleFor(x => x.Texto)
            .NotEmpty().WithMessage("O campo 'Texto' é obrigatório.")
            .MaximumLength(500).WithMessage("O campo 'Texto' deve conter no máximo 500 caracteres.").When(x => !string.IsNullOrWhiteSpace(x.Texto));
        RuleFor(x => x.DataCertifcado)
            .LessThanOrEqualTo(DateTime.Now).WithMessage("A data do certificado não pode ser futura.").When(x => x.DataCertifcado.HasValue);
        RuleFor(x => x.EventoId)
            .GreaterThan(0).WithMessage("O campo 'EventoId' deve ser um número positivo.").When(x => x.EventoId.HasValue);

    }
}
