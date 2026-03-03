using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unievent.Domain.Entities
{
    public class Evento : EntidadeBase
    {
        public Evento() { }
        public Evento(string nome, string descricao, string categoriaEvento, string horaEvento, DateTime dataEvento, int capacidade, IList<string> thumbnail)
        {
            Nome = nome;
            Descricao = descricao;
            CategoriaEvento = categoriaEvento;
            HoraEvento = horaEvento;
            DataEvento = dataEvento;
            Capacidade = capacidade;
            Thumbnail = thumbnail;
        }

        public required string Nome { get; set; }
        public required string Descricao { get; set; }
        public required string CategoriaEvento { get; set; }
        public required string HoraEvento { get; set; }
        public required DateTime DataEvento { get; set; }
        public required int Capacidade { get; set; }
        public required IList<string> Thumbnail { get; set; } 
    }
}
