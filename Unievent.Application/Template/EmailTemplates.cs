namespace Unievent.Application.Templates
{
    public static class EmailTemplates
    {
        public static string ConfirmacaoConta(string nome, string chave, string baseUrl, string confirmationBaseUrl = "")
        {
            var origemConfirmacao = string.IsNullOrWhiteSpace(confirmationBaseUrl)
                ? baseUrl
                : confirmationBaseUrl;
            var apiBaseUrl = origemConfirmacao.TrimEnd('/');
            if (!apiBaseUrl.EndsWith("/api", StringComparison.OrdinalIgnoreCase))
            {
                apiBaseUrl = $"{apiBaseUrl}/api";
            }
            var link = $"{apiBaseUrl}/Email/confirmar-conta?chave={Uri.EscapeDataString(chave)}";

            return $@"
            <html>
            <head>
                <meta charset='UTF-8'>
                <title>Confirmação de Conta - Unievent</title>
            </head>
            <body>
                <h1>Olá, {nome}!</h1>

                <p>
                    Seja bem-vindo(a) ao <strong>Unievent</strong>.
                </p>

                <p>
                    Recebemos sua solicitação de cadastro e estamos felizes em tê-lo(a) conosco.
                    Para garantir a segurança da sua conta e concluir o processo de registro,
                    é necessário confirmar o seu endereço de e-mail.
                </p>

                <p>
                    Clique no botão ou link abaixo para validar sua conta:
                </p>

                <p>
                    <a href='{link}' style='display: inline-block; padding: 10px 20px; font-size: 16px; color: #fff; background-color: #007bff; text-decoration: none; border-radius: 5px;'>
                        Confirmar Minha Conta
                    </a>
                </p>

                <p>
                    Após a confirmação, você poderá acessar todos os recursos disponíveis
                    na plataforma e participar das atividades e eventos oferecidos.
                </p>

                <p>
                    Caso o link não funcione, copie e cole o endereço abaixo em seu navegador:
                </p>

                <p>
                    {link}
                </p>

                <hr />

                <p>
                    Se você não realizou este cadastro, nenhuma ação é necessária.
                    Basta desconsiderar esta mensagem.
                </p>

                <p>
                    Por motivos de segurança, o Unievent nunca solicita senhas,
                    códigos de acesso, documentos pessoais ou arquivos anexados por e-mail.
                </p>

                <p>
                    Em caso de dúvidas ou dificuldades para acessar sua conta,
                    entre em contato com nossa equipe de suporte.
                </p>

                <p>
                    Atenciosamente,<br />
                    <strong>Equipe Unievent</strong>
                </p>
            </body>
            </html>";  // TODO: Colocar URL real do front-end para confirmação de email
        }

        public static string CertificadoDisponivel(string nome, string evento, string texto, string codigo, string instituicao = "")
        {
            nome = System.Net.WebUtility.HtmlEncode(nome);
            evento = System.Net.WebUtility.HtmlEncode(evento);
            texto = System.Net.WebUtility.HtmlEncode(texto);
            codigo = System.Net.WebUtility.HtmlEncode(codigo);
            instituicao = System.Net.WebUtility.HtmlEncode(instituicao);
            var origemEvento = string.IsNullOrWhiteSpace(instituicao)
                ? "pelo UniEvent"
                : $"pela {instituicao}";

            return $@"
            <html>
            <head>
                <meta charset='UTF-8'>
                <title>Certificado disponível - Unievent</title>
            </head>
            <body>
                <h1>Olá, {nome}!</h1>
                <p>Sua presença no evento <strong>{evento}</strong>, realizado {origemEvento}, foi confirmada.</p>
                <p>Seu certificado de participação em PDF está anexado a este e-mail.</p>
                <p>{texto}</p>
                <p>Código de validação: <strong>{codigo}</strong></p>
                <p>Atenciosamente,<br /><strong>Equipe Unievent</strong></p>
            </body>
            </html>";
        }

        public static string AlertaEvento(string nome, string evento, string data, string motivo)
        {
            return $@"
            <html>
            <head>
                <meta charset='UTF-8'>
                <title>Evento recomendado - Unievent</title>
            </head>
            <body>
                <h1>Olá, {nome}!</h1>
                <p>Encontramos um evento que pode combinar com você:</p>
                <p><strong>{evento}</strong></p>
                <p>Data: {data}</p>
                <p>{motivo}</p>
                <p>Atenciosamente,<br /><strong>Equipe Unievent</strong></p>
            </body>
            </html>";
        }
    }
}
