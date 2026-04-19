using FluentValidation;
using Unievent.Application.Dtos.UsuarioSecretaria;

namespace Unievent.Application.Validators.UsuarioSecretaria;

public class UsuarioSecretariaUpdateValidator : AbstractValidator<UsuarioSecretariaUpdate>
{
    public UsuarioSecretariaUpdateValidator()
    {
        RuleFor(s => s.EmailUsuario)
       .EmailAddress().WithMessage("O email deve ser valido")
       .Must(email => email.EndsWith("@fatec.sp.gov.br", StringComparison.CurrentCultureIgnoreCase))
       .WithMessage("O email deve ser institucional")
       .When(s => !string.IsNullOrWhiteSpace(s.EmailUsuario));
        RuleFor(s => s.NomeUsuario)
        .NotEmpty()
        .WithMessage("O nome deve ser preenchido")
        .When(s => s.NomeUsuario != null);
        RuleFor(s => s.Senha)
        .MinimumLength(6).WithMessage("A senha deve conter no minimo 6 caracteres").When(s => !string.IsNullOrWhiteSpace(s.Senha));
        RuleFor(s => s.Role)
        .NotEmpty().WithMessage("O cargo deve ser preenchido").When(s => s.Role != null);
    }
}
