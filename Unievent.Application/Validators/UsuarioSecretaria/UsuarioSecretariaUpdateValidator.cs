using FluentValidation;
using Unievent.Application.Dtos.UsuarioSecretaria;

namespace Unievent.Application.Validators.UsuarioSecretaria;

public class UsuarioSecretariaUpdateValidator : AbstractValidator<UsuarioSecretariaUpdate>
{
    public UsuarioSecretariaUpdateValidator()
    {
        RuleFor(s => s.EmailUsuario)
       .EmailAddress().WithMessage("O email deve ser valido").Matches(@"^[a-zA-Z0-9._%+-]+@fatec\.sp\.gov\.br$").WithMessage("O email deve ser institucional")
       .NotEmpty().WithMessage("O email deve ser preenchido").When(s => !string.IsNullOrWhiteSpace(s.EmailUsuario));
        RuleFor(s => s.NomeUsuario)
        .NotEmpty().WithMessage("O nome deve ser preenchido").When(s => !string.IsNullOrWhiteSpace(s.NomeUsuario));
        RuleFor(s => s.Senha)
        .NotEmpty().WithMessage("A senha deve ser preenchida").MinimumLength(6).WithMessage("A senha deve conter no minimo 6 caracteres").When(s => !string.IsNullOrWhiteSpace(s.Senha));
    }
}
