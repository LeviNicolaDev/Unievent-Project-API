using FluentValidation;
using Unievent.Application.Dtos.Evento;

namespace Unievent.Application.Validators.Evento;

public class EventoUpdateValidator : AbstractValidator<EventoUpdate>
{
    public EventoUpdateValidator()
    {
        RuleFor(e => e.Nome)
              .NotEmpty().WithMessage("O campo 'Nome' é obrigatório.")
              .MaximumLength(200).WithMessage("O campo 'Nome' deve conter no máximo 200 caracteres.").When(e => !string.IsNullOrWhiteSpace(e.Nome));
        RuleFor(e => e.Descricao)
            .NotEmpty().WithMessage("O campo 'Descrição' é obrigatório.")
            .MaximumLength(1000).WithMessage("O campo 'Descrição' deve conter no máximo 1000 caracteres.").When(e => !string.IsNullOrWhiteSpace(e.Descricao));
        RuleFor(e => e.DataEvento)
            .NotEmpty().WithMessage("O campo 'Data' é obrigatório.")
            .GreaterThan(DateTime.Now).WithMessage("A data do evento deve ser futura.").When(e => e.DataEvento.HasValue);
        RuleFor(e => e.Categoria)
            .NotEmpty().WithMessage("O campo 'Categoria' é obrigatório.")
            .MaximumLength(100).WithMessage("O campo 'Categoria' deve conter no máximo 100 caracteres.").When(e => !string.IsNullOrWhiteSpace(e.Categoria));
        RuleFor(e => e.ResponsavelEventoId)
            .NotEmpty().WithMessage("O campo 'Responsável' é obrigatório.").When(e => e.ResponsavelEventoId.HasValue);
        RuleFor(e => e.Capacidade)
            .NotNull().WithMessage("O campo 'Capacidade' é obrigatório.").When(e => e.Capacidade.HasValue);
        RuleFor(e => e.Thumbnail)
            .NotEmpty().WithMessage("O campo 'Thumbnail' é obrigatório.").When(e => e.Thumbnail != null);
        RuleFor(e => e.HoraEvento)
            .NotEmpty().WithMessage("O campo 'Hora' é obrigatório.").When(e => !string.IsNullOrWhiteSpace(e.HoraEvento));

    }
}
