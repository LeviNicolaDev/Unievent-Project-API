using FluentValidation;
using Unievent.Application.Dtos.ResponsavelEvento;

namespace Unievent.Application.Validators.ResponsavelEvento;

public class ResponsavelEventoRequestValidator : AbstractValidator<ResponsavelEventoRequest>
{
    public ResponsavelEventoRequestValidator()
    {
        RuleFor(r => r.Nome)
            .NotEmpty().WithMessage("O campo 'Nome' é obrigatório.");
        RuleFor(r => r.FotoPerfil)
            .NotEmpty().WithMessage("O campo 'Foto de Perfil' é obrigatório.");
    }
}
