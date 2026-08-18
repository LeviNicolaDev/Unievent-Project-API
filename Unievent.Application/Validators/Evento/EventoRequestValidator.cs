using System.Data;
using FluentValidation;
using Unievent.Application.Dtos.Evento;

namespace Unievent.Application.Validators.Evento;

public class EventoRequestValidator : AbstractValidator<EventoRequest>
{
    public EventoRequestValidator()
    {
        RuleFor(e => e.Nome)
            .NotEmpty().WithMessage("O campo 'Nome' é obrigatório.")
            .MaximumLength(200).WithMessage("O campo 'Nome' deve conter no máximo 200 caracteres.");
        RuleFor(e => e.Descricao)
            .NotEmpty().WithMessage("O campo 'Descrição' é obrigatório.")
            .MaximumLength(1000).WithMessage("O campo 'Descrição' deve conter no máximo 1000 caracteres.");
        RuleFor(e => e.DataEvento)
            .NotEmpty().WithMessage("O campo 'Data' é obrigatório.")
            .GreaterThan(DateTime.Now).WithMessage("A data do evento deve ser futura.");
        RuleFor(e => e.Categoria)
            .IsInEnum().WithMessage("O campo 'Categoria' deve ser um dos valores permitidos.");
        RuleFor(e => e.ResponsavelEventoId)
            .NotEmpty().WithMessage("O campo 'Responsável' é obrigatório.");
        RuleFor(e => e.Capacidade)
            .NotEmpty().WithMessage("O campo 'Capacidade' é obrigatório.")
            .GreaterThanOrEqualTo(1).WithMessage("A capacidade do evento deve ser um valor positivo.");
        RuleFor(e => e.Thumbnail)
            .NotEmpty().WithMessage("O campo 'Thumbnail' é obrigatório.");
        RuleFor(e => e.Visibilidade).IsInEnum();
        RuleFor(e => e.PublicoPermitido).IsInEnum();
        RuleFor(e => e.FimInscricoes)
            .GreaterThan(e => e.InicioInscricoes)
            .When(e => e.InicioInscricoes.HasValue && e.FimInscricoes.HasValue)
            .WithMessage("O fim das inscrições deve ser posterior ao início.");
        RuleFor(e => e.RaioCheckInMetros).InclusiveBetween(25, 5000);
        RuleFor(e => e.Latitude).InclusiveBetween(-90, 90).When(e => e.Latitude.HasValue);
        RuleFor(e => e.Longitude).InclusiveBetween(-180, 180).When(e => e.Longitude.HasValue);

    }
}
