using FluentValidation;
using Unievent.Application.Dtos.Aluno;

namespace Unievent.Application.Validators.Aluno;

public class AlunoRequestValidator : AbstractValidator<AlunoRequest>
{
    public AlunoRequestValidator()
    {
        RuleFor(a => a.Email)
         .EmailAddress().WithMessage("O email deve ser válido").Matches(@"^[a-zA-Z0-9._%+-]+@fatec\.sp\.gov\.br$")
         .WithMessage("O email deve ser institucional").NotEmpty()
         .WithMessage("O email deve ser preenchido").NotNull().WithMessage("Email não pode ser nulo");
        RuleFor(a => a.DataNascimento)
        .NotEmpty().WithMessage("Data de nascimento é obrigatoria").LessThan(DateTime.Now).WithMessage("A data de nascimento não pode ser futura");
        RuleFor(a => a.Senha)
        .NotEmpty().WithMessage("Senha é obrigatoria").MinimumLength(6).WithMessage("A senha deve ter no minimo 6 caracteres").NotNull()
        .WithMessage("Senha não pode ser nula");
        RuleFor(a => a.FotoPerfil)
        .NotEmpty().WithMessage("Foto de perfil é obrigatoria").NotNull().WithMessage("Foto de perfil não pode ser nula");
        RuleFor(a => a.Nome)
       .NotEmpty().WithMessage("O nome é obrigatoria").NotNull().WithMessage("Nome não pode ser nulo");
    }
}
