using FluentValidation;
using Unievent.Application.Dtos.ResponsavelEvento;

namespace Unievent.Application.Validators.ResponsavelEvento;

public class ResponsavelEventoUpdateValidator : AbstractValidator<ResponsavelEventoUpdate>
{
    public ResponsavelEventoUpdateValidator()
    {
        RuleFor(r => r.Nome)
            .NotEmpty().WithMessage("O campo 'Nome' é obrigatório.").When(r => r.Nome != null);
        RuleFor(r => r.FotoPerfil)
            .NotEmpty().WithMessage("O campo 'Foto de Perfil' é obrigatório.").When(r => r.FotoPerfil != null);
    }
}
