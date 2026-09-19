using FluentValidation;
using Unievent.Application.Dtos.Auth;

namespace Unievent.Application.Validators.Auth;

public class PublicoGeralCadastroRequestValidator : AbstractValidator<PublicoGeralCadastroRequest>
{
    public PublicoGeralCadastroRequestValidator()
    {
        RuleFor(p => p.Nome)
            .NotEmpty().WithMessage("O nome é obrigatório");

        RuleFor(p => p.Email)
            .NotEmpty().WithMessage("O email deve ser preenchido")
            .EmailAddress().WithMessage("O email deve ser válido");

        RuleFor(p => p.Senha)
            .NotEmpty().WithMessage("Senha é obrigatória")
            .MinimumLength(6).WithMessage("A senha deve ter no mínimo 6 caracteres");

        RuleFor(p => p.ConfirmacaoSenha)
            .Equal(p => p.Senha).WithMessage("Senhas não conferem");
    }
}
