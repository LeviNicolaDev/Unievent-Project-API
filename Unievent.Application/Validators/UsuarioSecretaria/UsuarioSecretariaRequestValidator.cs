using FluentValidation;
using Unievent.Application.Dtos.UsuarioSecretaria;
using Unievent.Domain.Enuns;

namespace Unievent.Application.Validators.UsuarioSecretaria;

public class UsuarioSecretariaRequestValidator : AbstractValidator<UsuarioSecretariaRequest>
{
    public UsuarioSecretariaRequestValidator()
    {
        RuleFor(s => s.EmailUsuario)
       .Cascade(CascadeMode.Stop)
       .NotEmpty().WithMessage("O email deve ser preenchido")
       .EmailAddress().WithMessage("O email deve ser válido")
       .Must(email =>
           email.EndsWith("@fatec.sp.gov.br",
           StringComparison.CurrentCultureIgnoreCase))
       .WithMessage("O email deve ser institucional (@fatec.sp.gov.br)");
        RuleFor(s => s.NomeUsuario)
        .NotEmpty().WithMessage("O nome deve ser preenchido");
        RuleFor(s => s.RoleUsuario)
        .IsInEnum().WithMessage("A role do usuario deve ser preenchido");
        RuleFor(s => s.RoleUsuario)
        .Equal(Role.Secretaria).WithMessage("UsuarioSecretaria deve usar o perfil Secretaria");
        RuleFor(s => s.InstituicaoId)
        .NotNull().WithMessage("A instituição é obrigatória para usuário Secretaria")
        .When(s => s.RoleUsuario == Role.Secretaria);
        RuleFor(s => s.Senha)
        .NotEmpty().WithMessage("A senha deve ser preenchida").MinimumLength(6).WithMessage("A senha deve conter no minimo 6 caracteres");
    }
}
