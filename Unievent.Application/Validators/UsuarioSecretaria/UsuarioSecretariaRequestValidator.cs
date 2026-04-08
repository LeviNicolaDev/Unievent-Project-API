using FluentValidation;
using Unievent.Application.Dtos.UsuarioSecretaria;

namespace Unievent.Application.Validators.UsuarioSecretaria;

public class UsuarioSecretariaRequestValidator : AbstractValidator<UsuarioSecretariaRequest>
{
    public UsuarioSecretariaRequestValidator()
    {
        RuleFor(s => s.EmailUsuario)
        .EmailAddress().WithMessage("O email deve ser valido").Matches(@"^[a-zA-Z0-9._%+-]+@fatec\.sp\.gov\.br$").WithMessage("O email deve ser institucional")
        .NotEmpty().WithMessage("O email deve ser preenchido");
        RuleFor(s => s.NomeUsuario)
        .NotEmpty().WithMessage("O nome deve ser preenchido");
        RuleFor(s => s.RoleUsuario)
        .NotEmpty().WithMessage("A role do usuario deve ser preenchido");
        RuleFor(s => s.Senha)
        .NotEmpty().WithMessage("A senha deve ser preenchida").MinimumLength(6).WithMessage("A senha deve conter no minimo 6 caracteres");
    }
}
