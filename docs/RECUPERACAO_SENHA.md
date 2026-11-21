# Recuperação de senha

O login administrativo e o login público oferecem **Esqueci minha senha**. O pedido informa e-mail e perfil, retorna uma mensagem genérica e envia um link de 30 minutos apenas se a conta ativa existir. O token é aleatório de 256 bits; somente seu SHA-256 é persistido. O link é de uso único. Solicitações novas não invalidam imediatamente links anteriores: todos são invalidados após a primeira redefinição bem-sucedida.

## Endpoints anônimos

`POST /api/Auth/solicitar-recuperacao-senha`

```json
{ "email": "pessoa@example.com", "perfil": "publico" }
```

Perfis: `publico` (Aluno Externo), `administrativo` (administrador global ou secretaria), `mobile` (Aluno Interno). O perfil é usado para localizar a conta; não concede permissões. E-mail é normalizado para a consulta. Conta ausente/inativa, falha SMTP e envio aceito recebem a mesma resposta HTTP 200:

```json
{ "mensagem": "Se houver uma conta com esse e-mail, enviaremos um link para redefinir sua senha." }
```

`POST /api/Auth/redefinir-senha`

```json
{ "token": "TOKEN_RECEBIDO_NO_EMAIL", "senha": "novaSenha123", "confirmacaoSenha": "novaSenha123" }
```

Senhas devem ser iguais, ter pelo menos 6 caracteres e no máximo 72 bytes UTF-8 (limite do BCrypt). Token ausente, inválido, expirado ou consumido retorna 400. Sucesso retorna mensagem, tipoUsuario e destino de login; não emite JWT nem autentica automaticamente.

A transação atualiza a senha condicionalmente e consome o token, evitando reutilização concorrente. Alteração da senha fora deste fluxo invalida o fingerprint dos links antigos. Confirmar e-mail, ativar conta e aprovação da secretaria continuam independentes e não são alterados.

## Sessões e limites

Após redefinir, a validação JWT rejeita sessões emitidas antes da troca, incluindo tokens antigos sem o novo claim `emitido_em_utc`. Novos logins emitem esse claim e continuam válidos. Sem redefinição registrada, sessões antigas continuam respeitando sua expiração normal. O front remove a sessão ao receber 401.

Há limite de 10 chamadas por minuto por IP e instância, nos dois endpoints, com HTTP 429. Há também intervalo de um minuto entre solicitações persistidas da mesma conta. SMTP recusado remove a solicitação para permitir uma nova tentativa; detalhes ficam no log sem token ou senha. Em ambiente com proxy, confira o encaminhamento confiável do IP do cliente. Os registros consumidos são usados para revogar JWTs; não remova esse histórico antes da expiração de todas as sessões anteriores.

## Telas

- `/recuperar-senha?perfil=publico`
- `/recuperar-senha?perfil=administrativo`
- `/recuperar-senha?perfil=mobile`
- `/redefinir-senha?token=...&perfil=...`

O perfil acompanha o link para que uma nova solicitação após expiração use o fluxo correto. Após sucesso o token é retirado da URL. A Vercel configura `no-referrer`, `no-store` e `noindex` para a página de redefinição. Usuários públicos voltam a `/entrar`, administradores e secretaria a `/login`; alunos internos recebem orientação e link para abrir o aplicativo, quando `MobileLoginDeepLink` estiver configurado.

A API atende o perfil mobile; adicionar o botão ao código do aplicativo móvel é uma integração separada deste front web.

## SMTP e deploy

Use a configuração SMTP existente: `EmailSettings__Host`, `Port`, `Email`, `Password` e `EnableSsl`. O link usa `EmailSettings__BaseUrl`, que em produção deve ser `https://unievent-web.vercel.app`. Localmente pode usar `http://localhost:5173`; para abrir em outro dispositivo use uma origem HTTPS acessível. A origem do link não é derivada de cabeçalhos enviados pelo cliente.

Antes de iniciar a nova API, aplique a migration:

```powershell
dotnet ef database update --project Unievent.Infra --startup-project Unievent.Api
```

`AddPasswordRecovery` cria somente a tabela de solicitações e índices. Ela é necessária também para a verificação de sessões autenticadas. Se `Database:MigrateOnStartup` estiver ativo, o processo aplica migrations ao iniciar; confirme a configuração e permissão do ambiente antes do deploy. Publique a API com a migration e o front com as novas rotas. Nenhuma nova credencial SMTP é exigida se o envio existente já funciona.

## Validação

Os testes usam SQLite temporário e SMTP simulado. Cobrem quatro perfis, senha nova e antiga, uso único, expiração, cooldown, e-mail inexistente, erro SMTP, validação de senha, invalidação de outros links e sessões. Playwright cobre solicitação, formulário de redefinição, confirmação divergente, token ausente/expirado e retorno ao login. Entrega SMTP real e o deploy devem ser verificados no ambiente publicado.
