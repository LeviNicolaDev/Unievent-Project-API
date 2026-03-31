namespace Unievent.Domain.Entities;

public class Instituicao : EntidadeBase
{
    public Instituicao()
    {

    }
    public Instituicao(int id, string emailLogin, string senhaLogin, string fotoPerfil, string cnpj, int idEndereco)
    {
        Id = id;
        EmailLogin = emailLogin;
        SenhaLogin = senhaLogin;
        FotoPerfil = fotoPerfil;
        Cnpj = cnpj;
        IdEndereco = idEndereco;
    }
    public int Id { get; set; }
    public required string EmailLogin { get; set; }
    public required string SenhaLogin { get; set; }
    public required string FotoPerfil { get; set; }
    public required string Cnpj { get; set; }
    public int IdEndereco { get; set; }
    public Endereco Endereco { get; set; }
}
