using FluentAssertions;
using Unievent.Application.Templates;
using Xunit;

namespace Unievent.Tests.Application.Email;

public class EmailTemplatesTest
{
    [Fact]
    public void ConfirmacaoConta_Deve_Apontar_Link_Para_Api_De_Confirmacao()
    {
        var html = EmailTemplates.ConfirmacaoConta(
            "Aluno",
            "chave com espaco",
            "http://localhost:5173",
            "http://localhost:5227");

        html.Should().Contain("http://localhost:5227/api/Email/confirmar-conta");
        html.Should().Contain("chave%20com%20espaco");
    }

    [Fact]
    public void ConfirmacaoConta_Nao_Deve_Duplicar_Prefixo_Api()
    {
        var html = EmailTemplates.ConfirmacaoConta(
            "Secretaria",
            "abc",
            "http://localhost:5173",
            "http://localhost:5227/api");

        html.Should().Contain("http://localhost:5227/api/Email/confirmar-conta");
        html.Should().NotContain("http://localhost:5227/api/api/Email");
    }
}
