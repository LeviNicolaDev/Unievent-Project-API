# Deploy da API pela pipeline

O deploy da API usa exclusivamente GitHub Actions, GHCR e Azure Container Apps, pelo workflow `.github/workflows/cd.yml`. Nenhuma ferramenta de provisionamento é necessária para publicar uma nova versão.

## Recursos existentes

A pipeline atualiza o Container App já criado. Ela não cria nem exclui PostgreSQL, rede, Container Apps Environment ou outros recursos Azure. Remover os antigos arquivos de infraestrutura do repositório não altera recursos existentes nem revoga identidades configuradas.

## Configuração do GitHub

Em Settings → Secrets and variables → Actions → Variables, mantenha:

| Variável | Uso |
| --- | --- |
| AZURE_CLIENT_ID | Identidade usada pelo login OIDC |
| AZURE_TENANT_ID | Tenant Azure |
| AZURE_SUBSCRIPTION_ID | Assinatura |
| AZURE_RESOURCE_GROUP | Grupo do Container App existente |
| AZURE_CONTAINER_APP | Nome do Container App existente |
| WEB_URL | Opcional: origem HTTPS do front, padrão https://unievent-web.vercel.app |

O login usa OIDC, com `id-token: write`; mantenha a credencial federada associada ao repositório e à branch de produção. Não é necessário criar uma senha Azure para substituir o OIDC. `GITHUB_TOKEN` é fornecido pelo Actions para publicar a imagem no GHCR.

## Fluxo

1. Push para main, ou execução manual do workflow na main.
2. Restore, build Release e testes .NET.
3. Build Docker e publicação no GHCR com a tag do commit e latest.
4. Login Azure via OIDC.
5. Consulta ao ingress HTTPS do Container App existente.
6. Atualização da imagem do commit e das variáveis de destino do front/e-mail/CORS.
7. Verificação de provisioningState. Esse estado não substitui o teste funcional da API após o deploy.

A opção manual `publish_only` publica a imagem sem atualizar o Container App. Não há deploy Azure de branches diferentes de main. Push na main pressupõe que os gates de revisão e CI já foram respeitados.

## Variáveis gerenciadas pela pipeline

- EmailSettings__BaseUrl = WEB_URL
- EmailSettings__ConfirmationBaseUrl = origem HTTPS consultada no ingress da API
- EmailSettings__WebLoginUrl = WEB_URL + /login
- EmailSettings__PublicLoginUrl = WEB_URL + /entrar
- Cors__AllowedOrigins__0 = WEB_URL

`WEB_URL` deve ser somente a origem HTTPS, sem caminho. A pipeline remove uma barra final. A atualização usa `--set-env-vars`, preservando as demais variáveis e referências a secrets já configuradas no Container App.

## Configurações que precisam continuar no Container App

Conexão PostgreSQL, chave/issuer/audience/expiração JWT, Host/Port/Email/Password/EnableSsl do SMTP, deep link mobile, ambiente ASP.NET, migrations e configuração do worker continuam no recurso existente. A pipeline não imprime seus valores nem os substitui por arquivos locais. Se já funcionavam, não precisam de nova senha por causa desta remoção.

O Container App deve conseguir baixar a imagem publicada no GHCR. Preserve sua configuração de acesso ao registry. A API precisa de rede e permissões adequadas para o PostgreSQL existente.

## Migrations

As migrations são compiladas na imagem. Se `Database__MigrateOnStartup=true` estiver configurado e o usuário do banco tiver permissão, a API aplica as pendentes ao iniciar. Confirme nos logs, especialmente `ConfirmacaoEmailAdminPublico` e `AddPasswordRecovery`, e faça backup antes de alterações sensíveis. A pipeline não executa um segundo `database update` separado.

Não recrie o banco nem a infraestrutura para aplicar uma nova versão. Para mudanças que exijam job de migration dedicado, acrescente uma etapa controlada ao workflow e mantenha compatibilidade entre revisões.

## Front e verificação

O front continua publicado na Vercel por sua integração Git, com `VITE_API_BASE_URL` apontando à API pública e `vercel.json` versionado. A pipeline de API não publica o front.

Após o deploy, teste a API, a busca pública, login, confirmação, recuperação de senha, ingressos e certificados. Acessar /entrar ou /redefinir-senha diretamente deve funcionar no front. Confirme também os logs da revisão ativa do Container App.

## Reverter

Reimplante uma imagem de commit anterior ou ajuste a revisão ativa pelo processo operacional adotado. Alterações de schema exigem plano próprio; não remova tabelas ou dados automaticamente para reverter uma imagem.
