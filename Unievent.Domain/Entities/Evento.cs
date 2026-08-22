using Unievent.Domain.Enuns;

namespace Unievent.Domain.Entities
{
    public class Evento : EntidadeBase
    {
        public Evento() { }
        public Evento(string nome, string descricao, Categoria categoria, DateTime dataEvento, int capacidade, IList<string> thumbnail, int idResponsavelEvento)
        {
            Nome = nome;
            Descricao = descricao;
            Categoria = categoria;
            DataEvento = dataEvento;
            Capacidade = capacidade;
            Thumbnail = thumbnail;
            ResponsavelEventoId = idResponsavelEvento;
        }

        public required string Nome { get; set; }
        public required string Descricao { get; set; }
        public string? Local { get; set; }
        public required Categoria Categoria { get; set; }
        public required DateTime DataEvento { get; set; }
        public required int ResponsavelEventoId { get; set; }
        public ResponsavelEvento ResponsavelEvento { get; set; }
        public required int Capacidade { get; set; }
        public required IList<string> Thumbnail { get; set; }
        public int? InstituicaoId { get; set; }
        public Instituicao? Instituicao { get; set; }
        public VisibilidadeEvento Visibilidade { get; set; } = VisibilidadeEvento.Publico;
        public PublicoPermitido PublicoPermitido { get; set; } = PublicoPermitido.PublicoGeral;
        public DateTime? InicioInscricoes { get; set; }
        public DateTime? FimInscricoes { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public int RaioCheckInMetros { get; set; } = 150;
        public int ToleranciaCheckInMinutos { get; set; } = 60;
    }
}
