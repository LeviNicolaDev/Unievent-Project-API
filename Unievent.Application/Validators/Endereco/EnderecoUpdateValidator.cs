using FluentValidation;
using Unievent.Application.Dtos.Endereco;

namespace Unievent.Application.Validators.Endereco;

public class EnderecoUpdateValidator : AbstractValidator<EnderecoUpdate>
{
    public EnderecoUpdateValidator()
    {
        RuleFor(e => e.Rua)
            .NotEmpty().WithMessage("O campo 'Rua' é obrigatório.")
            .MaximumLength(200).WithMessage("O campo 'Rua' deve conter no máximo 200 caracteres.").When(e => !string.IsNullOrWhiteSpace(e.Rua));
        RuleFor(e => e.Numero)
            .NotEmpty().WithMessage("O campo 'Número' é obrigatório.")
            .MaximumLength(20).WithMessage("O campo 'Número' deve conter no máximo 20 caracteres.").When(e => !string.IsNullOrWhiteSpace(e.Numero));
        RuleFor(e => e.Bairro)
            .NotEmpty().WithMessage("O campo 'Bairro' é obrigatório.")
            .MaximumLength(100).WithMessage("O campo 'Bairro' deve conter no máximo 100 caracteres.").When(e => !string.IsNullOrWhiteSpace(e.Bairro));
        RuleFor(e => e.Cidade)
            .NotEmpty().WithMessage("O campo 'Cidade' é obrigatório.")
            .MaximumLength(100).WithMessage("O campo 'Cidade' deve conter no máximo 100 caracteres.").When(e => !string.IsNullOrWhiteSpace(e.Cidade));
        RuleFor(e => e.Estado)
            .NotEmpty().WithMessage("O campo 'Estado' é obrigatório.")
            .MaximumLength(50).WithMessage("O campo 'Estado' deve conter no máximo 50 caracteres.").When(e => !string.IsNullOrWhiteSpace(e.Estado));
        RuleFor(e => e.Cep)
            .NotEmpty().WithMessage("O campo 'CEP' é obrigatório.").MaximumLength(8)
            .WithMessage("O campo 'CEP' deve conter no máximo 8 caracteres.").When(e => !string.IsNullOrWhiteSpace(e.Cep));


    }
}
