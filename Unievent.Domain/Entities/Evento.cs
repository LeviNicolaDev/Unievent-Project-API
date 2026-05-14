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
        public required Categoria Categoria { get; set; }
        public required DateTime DataEvento { get; set; }
        public required int ResponsavelEventoId { get; set; }
        public ResponsavelEvento ResponsavelEvento { get; set; }
        public required int Capacidade { get; set; }
        public required IList<string> Thumbnail { get; set; }
    }
}
