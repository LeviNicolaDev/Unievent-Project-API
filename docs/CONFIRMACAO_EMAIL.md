# Confirmação de e-mail e ativação

| Perfil | Cadastro | Confirmação | Destino |
| --- | --- | --- | --- |
| Administrador global | POST /api/UsuarioUnievent/admin-inicial ou POST /api/UsuarioUnievent (admin autorizado) | Obrigatória, persistida em UsuarioUnievent.EmailConfirmado | /login |
| Público Geral | POST /api/Auth/cadastro-publico | Obrigatória, persistida em Aluno.EmailConfirmado | /entrar |
| Secretaria | Fluxo institucional existente | Confirmação e aprovação continuam necessárias | /login |
| Aluno mobile interno | Fluxo mobile existente | Confirmação institucional existente | Deep link do aplicativo |

## Cadastro e login

Nos novos cadastros de administrador e público, a API gera a chave aleatória, envia o e-mail e salva a conta pendente. Chaves enviadas pelo cliente não são usadas nesses cadastros. Se o envio retornar falha, o cadastro não é persistido e o front apresenta o erro para nova tentativa. O front não envia um segundo e-mail após cadastrar o administrador.

Credenciais corretas sem e-mail confirmado retornam HTTP 401, sem JWT. Confirmar o e-mail não altera IsAtivo nem concede aprovação institucional. O login administrativo também rejeita administrador inativo.

GET /api/Email/confirmar-conta?chave=... confirma e redireciona ao login do perfil. POST no mesmo endpoint recebe { "chave": "..." } e retorna mensagem, tipoUsuario e destino para a página web. Para Público Geral, tipoUsuario é Publico; aluno interno mantém Aluno. Chave inválida retorna 404; abrir novamente uma chave já confirmada mantém o sucesso.

## Reenvio automático no login

Uma tentativa de login com credenciais corretas para administrador ou público pendente envia automaticamente a confirmação. A resposta HTTP 401 contém `{ "code": "EMAIL_CONFIRMATION_SENT", "mensagem": "Enviamos um e-mail de confirmação para seu e-mail. Abra o link recebido para ativar sua conta." }`, sem JWT. O front exibe o aviso e não oferece botão de reenvio. O envio é assíncrono e aguardado antes de afirmar sucesso; falha de SMTP retorna erro. Senha incorreta não dispara envio. A chave existente é reutilizada para manter válidos os links anteriores; uma chave é gerada se estiver ausente.

### Endpoint de compatibilidade

POST /api/Email/reenviar-confirmacao recebe somente { "email": "pessoa@example.com" }. O endpoint continua disponível para clientes existentes; a interface web usa o envio automático pelo login. Nome e chave são definidos pela API. Para contas pendentes, uma nova chave substitui a anterior após envio bem-sucedido. Falha de envio preserva a chave anterior. Conta ausente ou já confirmada recebe a mesma mensagem genérica, sem revelar dados ou chave. Secretaria e aluno interno continuam com seus fluxos existentes.

## Configuração

Configure SMTP via EmailSettings (Host, Port, Email, Password, EnableSsl), sem versionar credenciais. Configure BaseUrl com a origem do front e ConfirmationBaseUrl com a origem da API; o template acrescenta /api quando necessário. Sem ConfirmationBaseUrl, o link aponta à página BaseUrl + /confirmar-email, que confirma usando a API configurada no front, em vez de presumir que a API está hospedada na origem do front. WebLoginUrl aponta para /login; PublicLoginUrl aponta para /entrar (fallback: BaseUrl + /entrar). MobileLoginDeepLink mantém a configuração do aplicativo.

Em containers, forneça EmailSettings__PublicLoginUrl se o destino público diferir do fallback. User Secrets atendem ao desenvolvimento; variáveis de ambiente têm precedência.

## Atualização de um banco existente

Execute na raiz da API, com ConnectionStrings__DefaultConnection apontando ao banco pretendido:

~~~powershell
dotnet ef database update --project Unievent.Infra --startup-project Unievent.Api
~~~

A migration ConfirmacaoEmailAdminPublico adiciona EmailConfirmado=false ao administrador e invalida suas chaves antigas. Contas externas existentes passam a EmailConfirmado=false, com chave nula, pois antes eram confirmadas automaticamente sem comprovação. Essas contas devem tentar fazer login com suas credenciais e abrir o novo link enviado automaticamente. Alunos internos e secretaria não são alterados. Não há envio em massa durante a migration.

A reversão remove a coluna do administrador; não restaura chaves nem confirmações anteriores das contas externas. Nenhuma tabela de usuários ou participação é removida.

## Verificação

Validar cadastro pendente, bloqueio de JWT, reenvio, chave inválida, ativação repetida e redirecionamento de cada perfil. Os testes automatizados usam e-mail simulado; a entrega SMTP real exige configuração do ambiente.

## Diagnóstico local de link recusado

Os perfis de projeto da API escutam em http://localhost:5227 (o perfil https também escuta em https://localhost:7128). IIS Express usa outras portas. No desenvolvimento, BaseUrl deve ser http://localhost:5173 e ConfirmationBaseUrl http://localhost:5227. O link explícito será http://localhost:5227/api/Email/confirmar-conta?chave=... . Reinicie a API após ajustar User Secrets ou variáveis de ambiente.

Inicie a API com `dotnet run --project Unievent.Api --launch-profile https` e o front com `npm run dev -- --port 5173`. Links em e-mails antigos não são corrigidos retroativamente: tente login novamente para receber um novo e-mail. Localhost exige que os serviços estejam rodando no mesmo computador em que o link é aberto; para celular ou outro computador, configure URLs acessíveis pela rede.
