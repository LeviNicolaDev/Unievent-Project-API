namespace Unievent.Domain.Entities
{
    public class UsuarioSecretaria : EntidadeBase
    {
        public UsuarioSecretaria() { }
        public UsuarioSecretaria(string nomeUsuario, Role roleUsuario, string emailUsuario, string chave,
        int tentativasLogin, bool isAtivo)
        {
            NomeUsuario = nomeUsuario;
            RoleUsuario = roleUsuario;
            EmailUsuario = emailUsuario;
            Chave = chave;
            TentativasLogin = tentativasLogin;
            IsAtivo = isAtivo;
        }

        public required string NomeUsuario { get; set; }
        public required Role RoleUsuario { get; set; }
        public required string EmailUsuario { get; set; }
        public string Chave { get; set; }
        public int TentativasLogin { get; set; }
        public bool IsAtivo { get; set; }

    }
}
