namespace Unievent.Application.Templates
{
    public static class EmailTemplates
    {
        public static string ConfirmacaoConta(string nome, string chave, string baseUrl)
        {
            var link = $"{baseUrl}/confirmar-email?chave={chave}";

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
    }
}