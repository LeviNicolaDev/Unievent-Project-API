namespace Unievent.Domain.Entities
{
    public class ResponsavelEvento : EntidadeBase
    {
        public ResponsavelEvento() { }
        public ResponsavelEvento(string nome, string fotoPerfil)
        {
            Nome = nome;
            FotoPerfil = fotoPerfil;
        }
        public required string Nome { get; set; }
        public required string FotoPerfil { get; set; }
    }
}
