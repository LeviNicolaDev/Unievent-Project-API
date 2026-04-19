using FluentValidation;
using Unievent.Application.Dtos.UsuarioSecretaria;

namespace Unievent.Application.Validators.UsuarioSecretaria;

public class UsuarioSecretariaRequestValidator : AbstractValidator<UsuarioSecretariaRequest>
{
    public UsuarioSecretariaRequestValidator()
    {
        RuleFor(s => s.EmailUsuario)
        .EmailAddress().WithMessage("O email deve ser valido")
        .NotEmpty().WithMessage("O email deve ser preenchido").Must(email => email.EndsWith("@fatec.sp.gov.br", StringComparison.CurrentCultureIgnoreCase))
        .WithMessage("O email deve ser institucional");
        RuleFor(s => s.NomeUsuario)
        .NotEmpty().WithMessage("O nome deve ser preenchido");
        RuleFor(s => s.RoleUsuario)
        .NotEmpty().WithMessage("A role do usuario deve ser preenchido");
        RuleFor(s => s.Senha)
        .NotEmpty().WithMessage("A senha deve ser preenchida").MinimumLength(6).WithMessage("A senha deve conter no minimo 6 caracteres");
    }
}
