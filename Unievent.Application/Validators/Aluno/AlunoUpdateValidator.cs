using FluentValidation;
using Unievent.Application.Dtos.Aluno;

namespace Unievent.Application.Validators.Aluno;

public class AlunoUpdateValidator : AbstractValidator<AlunoUpdate>

{
    public AlunoUpdateValidator()
    {
        RuleFor(a => a.DataNascimento)
        .LessThan(DateTime.Now).WithMessage("A data de nascimento nao pode futura").When(a => a.DataNascimento.HasValue);
        RuleFor(a => a.Senha)
        .MinimumLength(6).WithMessage("A senha deve ter no minimo 6 caracteres").When(a => !string.IsNullOrWhiteSpace(a.Senha));
        RuleFor(a => a.FotoPerfil)
        .NotEmpty().WithMessage("A foto de perfil deve ser obrigatoria").When(a => a.FotoPerfil != null);
        RuleFor(a => a.Nome)
        .NotEmpty().WithMessage("O nome dever ser obrigatorio").When(a => a.Nome != null);

    }
}
