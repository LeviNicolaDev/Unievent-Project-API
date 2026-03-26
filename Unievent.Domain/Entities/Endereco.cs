namespace Unievent.Domain.Entities;

public class Endereco : EntidadeBase
{
    public Endereco()
    {

    }
    public Endereco(string rua, string cidade, string bairro, string estado, string cep, string numero)
    {

        Rua = rua;
        Cidade = cidade;
        Bairro = bairro;
        Estado = estado;
        Cep = cep;
        Numero = numero;
    }


    public required string Rua { get; set; }
    public required string Cidade { get; set; }
    public required string Bairro { get; set; }
    public required string Estado { get; set; }
    public required string Cep { get; set; }
    public required string Numero { get; set; }

}
