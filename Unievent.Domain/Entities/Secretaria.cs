namespace Unievent.Domain.Entities
{
    public class Secretaria : EntidadeBase
    {
        public Secretaria() { }
        public Secretaria(string nome, string email, string senha)
        {
            Nome = nome;
            Email = email;
            Senha = senha;
        }
        public required string Nome { get; set; }
        public required string Email { get; set; }
        public required string Senha { get; set; }
    }
}
