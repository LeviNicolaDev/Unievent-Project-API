using Unievent.Domain.Enuns;

namespace Unievent.Domain.Entities
{
    public class UsuarioSecretaria : EntidadeBase
    {
        public UsuarioSecretaria() { }
        public UsuarioSecretaria(string nomeUsuario, Role roleUsuario, string emailUsuario, string chave, string senha,
        int tentativasLogin, bool isAtivo)
        {
            NomeUsuario = nomeUsuario;
            RoleUsuario = roleUsuario;
            EmailUsuario = emailUsuario;
            Chave = chave;
            Senha = senha;
            TentativasLogin = tentativasLogin;
            IsAtivo = isAtivo;
        }

        public required string NomeUsuario { get; set; }
        public required Role RoleUsuario { get; set; }
        public required string EmailUsuario { get; set; }
        public string Chave { get; set; }
        public string Senha { get; set; }
        public int TentativasLogin { get; set; } = default;
        public bool IsAtivo { get; set; }

    }
}
