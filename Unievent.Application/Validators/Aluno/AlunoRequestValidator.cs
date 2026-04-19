using FluentValidation;
using Unievent.Application.Dtos.Aluno;

namespace Unievent.Application.Validators.Aluno;

public class AlunoRequestValidator : AbstractValidator<AlunoRequest>
{
    public AlunoRequestValidator()
    {
        RuleFor(a => a.Email)
     .NotEmpty().WithMessage("O email deve ser preenchido")
     .EmailAddress().WithMessage("O email deve ser válido")
     .Must(email => email.EndsWith("@fatec.sp.gov.br", StringComparison.CurrentCultureIgnoreCase))
     .WithMessage("O email deve ser institucional");

        RuleFor(a => a.DataNascimento)
            .NotEmpty().WithMessage("Data de nascimento é obrigatória")
            .LessThan(DateTime.Now).WithMessage("A data de nascimento não pode ser futura");

        RuleFor(a => a.Senha)
            .NotEmpty().WithMessage("Senha é obrigatória")
            .MinimumLength(6).WithMessage("A senha deve ter no mínimo 6 caracteres");

        RuleFor(a => a.FotoPerfil)
            .NotEmpty().WithMessage("Foto de perfil é obrigatória");

        RuleFor(a => a.Nome)
            .NotEmpty().WithMessage("O nome é obrigatório");
    }
}
