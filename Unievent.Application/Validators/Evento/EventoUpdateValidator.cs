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
        RuleFor(e => e.Local)
            .MaximumLength(200).WithMessage("O campo 'Local' deve conter no máximo 200 caracteres.")
            .When(e => e.Local != null);
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
        RuleFor(e => e.Visibilidade).IsInEnum().When(e => e.Visibilidade.HasValue);
        RuleFor(e => e.PublicoPermitido).IsInEnum().When(e => e.PublicoPermitido.HasValue);
        RuleFor(e => e.FimInscricoes)
            .GreaterThan(e => e.InicioInscricoes)
            .When(e => e.InicioInscricoes.HasValue && e.FimInscricoes.HasValue);
        RuleFor(e => e.RaioCheckInMetros).InclusiveBetween(25, 5000).When(e => e.RaioCheckInMetros.HasValue);
        RuleFor(e => e.Latitude).InclusiveBetween(-90, 90).When(e => e.Latitude.HasValue);
        RuleFor(e => e.Longitude).InclusiveBetween(-180, 180).When(e => e.Longitude.HasValue);


    }
}
