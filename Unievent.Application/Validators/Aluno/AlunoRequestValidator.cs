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
     .Must(email => email is not null && System.Text.RegularExpressions.Regex.IsMatch(
         email, @"^[^@\s]+@(fatec\.sp\.gov\.br|aluno\.cps\.sp\.gov\.br)$",
         System.Text.RegularExpressions.RegexOptions.IgnoreCase))
     .WithMessage("Participantes internos devem usar @fatec.sp.gov.br ou @aluno.cps.sp.gov.br")
     .When(a => a.TipoParticipante == Unievent.Domain.Enuns.TipoParticipante.Interno);

        RuleFor(a => a.InstituicaoId)
            .NotNull().WithMessage("A instituição é obrigatória para participantes internos")
            .When(a => a.TipoParticipante == Unievent.Domain.Enuns.TipoParticipante.Interno);

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
