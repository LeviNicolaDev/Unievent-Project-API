using FluentValidation;
using Unievent.Application.Dtos.Instituicao;

namespace Unievent.Application.Validators.Instituicao;

public class InstituicaoUpdateValidator : AbstractValidator<InstituicaoUpdate>
{
    public InstituicaoUpdateValidator()
    {
        RuleFor(i => i.Cnpj)
            .NotEmpty().WithMessage("O campo 'CNPJ' é obrigatório.")
            .MaximumLength(20).WithMessage("O campo 'CNPJ' deve conter no máximo 20 caracteres.").When(i => !string.IsNullOrWhiteSpace(i.Cnpj));
        RuleFor(i => i.EmailLogin)
            .NotEmpty().WithMessage("O campo 'EmailLogin' é obrigatório.")
            .MaximumLength(100).WithMessage("O campo 'EmailLogin' deve conter no máximo 100 caracteres.")
            .EmailAddress().WithMessage("O campo 'EmailLogin' deve ser um endereço de email válido.")
            .Matches(@"^[a-zA-Z0-9._%+-]+@fatec\.sp\.gov\.br$")
            .WithMessage("O email deve ser institucional").When(i => !string.IsNullOrWhiteSpace(i.EmailLogin));
        RuleFor(i => i.SenhaLogin)
            .NotEmpty().WithMessage("O campo 'SenhaLogin' é obrigatório.")
            .MinimumLength(6).WithMessage("O campo 'SenhaLogin' deve conter no mínimo 6 caracteres.").When(i => !string.IsNullOrWhiteSpace(i.SenhaLogin));
        RuleFor(i => i.FotoPerfil)
            .NotEmpty().WithMessage("O campo 'FotoPerfil' é obrigatório.").When(i => i.FotoPerfil != null);

    }

}
