using FluentValidation;
using Unievent.Application.Dtos.ResponsavelEvento;

namespace Unievent.Application.Validators.ResponsavelEvento;

public class ResponsavelEventoRequestValidator : AbstractValidator<ResponsavelEventoRequest>
{
    public ResponsavelEventoRequestValidator()
    {
        RuleFor(r => r.Nome)
            .NotEmpty().WithMessage("O campo 'Nome' é obrigatório.")
            .MaximumLength(100).WithMessage("O campo 'Nome' deve conter no máximo 100 caracteres.");
        RuleFor(r => r.FotoPerfil)
            .NotEmpty().WithMessage("O campo 'Foto de Perfil' é obrigatório.");
    }
}
