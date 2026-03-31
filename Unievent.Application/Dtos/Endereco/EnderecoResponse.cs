namespace Unievent.Application.Dtos.Endereco;

public record EnderecoResponse
{
    public int Id { get; set; }
    public string Rua { get; set; }
    public string Cidade { get; set; }
    public string Bairro { get; set; }
    public string Estado { get; set; }
    public string Cep { get; set; }
    public string Numero { get; set; }
}
