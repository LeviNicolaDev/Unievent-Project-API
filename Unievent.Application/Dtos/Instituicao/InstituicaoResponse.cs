namespace Unievent.Application.Dtos.Instituicao;

public class InstituicaoResponse
{
    public int Id { get; set; }
    public string EmailLogin { get; set; }

    public string FotoPerfil { get; set; }
    public string Cnpj { get; set; }
    public int EnderecoId { get; set; }
}
