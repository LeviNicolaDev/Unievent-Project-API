using FluentValidation;
using Unievent.Application.Dtos.Instituicao;

namespace Unievent.Application.Validators.Instituicao;

public class InstituicaoUpdateValidator : AbstractValidator<InstituicaoUpdate>
{
    public InstituicaoUpdateValidator()
    {
        RuleFor(i => i.Nome)
            .MaximumLength(160).WithMessage("O campo 'Nome' deve conter no máximo 160 caracteres.")
            .When(i => i.Nome != null);
        RuleFor(i => i.NomeAbreviado)
            .MaximumLength(40).WithMessage("O campo 'NomeAbreviado' deve conter no máximo 40 caracteres.")
            .When(i => i.NomeAbreviado != null);
        RuleFor(i => i.Codigo)
            .MaximumLength(60).WithMessage("O campo 'Codigo' deve conter no máximo 60 caracteres.")
            .When(i => i.Codigo != null);
        RuleFor(i => i.Cnpj)
            .NotEmpty().WithMessage("O campo 'CNPJ' é obrigatório.")
            .MaximumLength(20).WithMessage("O campo 'CNPJ' deve conter no máximo 20 caracteres.").When(i => i.Cnpj != null);
        RuleFor(i => i.FotoPerfil)
            .NotEmpty().WithMessage("O campo 'FotoPerfil' é obrigatório.").When(i => i.FotoPerfil != null);
        RuleFor(i => i.Site)
            .MaximumLength(200).WithMessage("O campo 'Site' deve conter no máximo 200 caracteres.")
            .When(i => i.Site != null);
        RuleFor(i => i.Rua)
            .NotEmpty().WithMessage("O campo 'Rua' é obrigatório.")
            .MaximumLength(200).WithMessage("O campo 'Rua' deve conter no máximo 200 caracteres.")
            .When(i => i.Rua != null);
        RuleFor(i => i.Numero)
            .NotEmpty().WithMessage("O campo 'Número' é obrigatório.")
            .MaximumLength(20).WithMessage("O campo 'Número' deve conter no máximo 20 caracteres.")
            .When(i => i.Numero != null);
        RuleFor(i => i.Bairro)
            .NotEmpty().WithMessage("O campo 'Bairro' é obrigatório.")
            .MaximumLength(100).WithMessage("O campo 'Bairro' deve conter no máximo 100 caracteres.")
            .When(i => i.Bairro != null);
        RuleFor(i => i.Cidade)
            .NotEmpty().WithMessage("O campo 'Cidade' é obrigatório.")
            .MaximumLength(100).WithMessage("O campo 'Cidade' deve conter no máximo 100 caracteres.")
            .When(i => i.Cidade != null);
        RuleFor(i => i.Estado)
            .NotEmpty().WithMessage("O campo 'Estado' é obrigatório.")
            .MaximumLength(50).WithMessage("O campo 'Estado' deve conter no máximo 50 caracteres.")
            .When(i => i.Estado != null);
        RuleFor(i => i.Cep)
            .NotEmpty().WithMessage("O campo 'CEP' é obrigatório.")
            .MaximumLength(8).WithMessage("O campo 'CEP' deve conter no máximo 8 caracteres.")
            .When(i => i.Cep != null);

    }

}
