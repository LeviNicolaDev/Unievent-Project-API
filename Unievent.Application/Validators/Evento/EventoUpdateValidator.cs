using FluentValidation;
using Unievent.Application.Dtos.Evento;

namespace Unievent.Application.Validators.Evento;

public class EventoUpdateValidator : AbstractValidator<EventoUpdate>
{
    public EventoUpdateValidator()
    {
        RuleFor(e => e.Nome)
              .NotEmpty().WithMessage("O campo 'Nome' é obrigatório.")
              .MaximumLength(200).WithMessage("O campo 'Nome' deve conter no máximo 200 caracteres.").When(e => e.Nome != null);
        RuleFor(e => e.Descricao)
            .NotEmpty().WithMessage("O campo 'Descrição' é obrigatório.")
            .MaximumLength(1000).WithMessage("O campo 'Descrição' deve conter no máximo 1000 caracteres.").When(e => e.Descricao != null);
        RuleFor(e => e.DataEvento)
            .NotEmpty().WithMessage("O campo 'Data' é obrigatório.")
            .GreaterThan(DateTime.Now).WithMessage("A data do evento deve ser futura.").When(e => e.DataEvento.HasValue);
        RuleFor(e => e.Categoria)
            .IsInEnum().WithMessage("O campo 'Categoria' deve ser um dos valores permitidos.").When(e => e.Categoria.HasValue);
        RuleFor(e => e.ResponsavelEventoId)
            .NotEmpty().WithMessage("O campo 'Responsável' é obrigatório.").When(e => e.ResponsavelEventoId.HasValue);
        RuleFor(e => e.Capacidade)
            .NotEmpty().WithMessage("O campo 'Capacidade' é obrigatório.").GreaterThanOrEqualTo(1).WithMessage("A capacidade do evento deve ser um valor positivo.").When(e => e.Capacidade.HasValue);
        RuleFor(e => e.Thumbnail)
            .NotEmpty().WithMessage("O campo 'Thumbnail' é obrigatório.").When(e => e.Thumbnail != null);


    }
}
