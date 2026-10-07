# UniEvent: Azure API e frontend Vercel

O Terraform cria Container Apps (0,25 vCPU / 0,5 GiB), PostgreSQL 16 privado
(B_Standard_B1ms, 32 GiB), VNet, DNS privado e Log Analytics. A região padrão
é canadacentral. O frontend é publicado separadamente pela integração Git da Vercel.
Não há Static Web App na Azure.

## Primeira publicação

1. Envie o workflow para main. Em Actions, execute CI/CD - Azure API manualmente
   em main com publish_only marcado para publicar a imagem inicial sem deploy.
   No GitHub Packages, torne o pacote público. O Container App não tem credenciais GHCR.
   Um push sem as variáveis Azure ainda publica a imagem, mas o job de deploy falha
   explicitamente até concluir a configuração abaixo.
2. Na Vercel, obtenha o domínio de produção do frontend.
3. No PowerShell, a partir de infra/terraform:

```powershell
az login
$env:TF_VAR_subscription_id = az account show --query id -o tsv
$env:TF_VAR_container_image = 'ghcr.io/levinicoladev/unievent-project-api:latest'
$env:TF_VAR_web_url = Read-Host 'Cole a origem HTTPS de producao da Vercel, sem barra final'
$env:TF_VAR_location = 'canadacentral'
terraform init
terraform validate
terraform plan -out=tfplan
# Revise o plano antes de criar os recursos.
terraform apply tfplan
terraform output -raw api_url
```

Alternativamente, copie terraform.tfvars.example para terraform.tfvars e preencha
os valores. tfvars tem precedência sobre TF_VAR; evite manter valores conflitantes.
A configuração não lê .env. SMTP é opcional: informe email_smtp_address e
email_smtp_password juntos. Banco e chave JWT são gerados automaticamente.

Se já existir um plano anterior, gere-o novamente. Se um Static Web App já estiver
no state, sua remoção será apresentada no plano. A mudança de região de recursos
existentes pode exigir substituição: revise o plano e faça backup do banco antes.

## Credencial GitHub/Azure sem senha (OIDC)

Execute uma vez após criar a infraestrutura, no diretório infra/terraform.
É necessário ter permissão para registrar aplicações no tenant e atribuir papéis
no resource group; o tenant institucional pode exigir ajuda do administrador.

```powershell
$subscriptionId = az account show --query id -o tsv
$tenantId = az account show --query tenantId -o tsv
$resourceGroup = terraform output -raw resource_group_name
$containerApp = terraform output -raw container_app_name
$repository = Read-Host 'Repositorio GitHub no formato proprietario/repositorio'
$clientId = az ad app create --display-name 'unievent-github-deploy' --query appId -o tsv
if ($LASTEXITCODE -ne 0) { throw 'Falha ao criar aplicacao Entra.' }
$principalId = az ad sp create --id $clientId --query id -o tsv
if ($LASTEXITCODE -ne 0) { throw 'Falha ao criar service principal.' }
$scope = "/subscriptions/$subscriptionId/resourceGroups/$resourceGroup"
az role assignment create --assignee-object-id $principalId --assignee-principal-type ServicePrincipal --role Contributor --scope $scope --output none
if ($LASTEXITCODE -ne 0) { throw 'Falha ao atribuir papel.' }
$federation = @{
    name = 'github-main'
    issuer = 'https://token.actions.githubusercontent.com'
    subject = "repo:${repository}:ref:refs/heads/main"
    audiences = @('api://AzureADTokenExchange')
} | ConvertTo-Json
$federationFile = Join-Path ([System.IO.Path]::GetTempPath()) 'unievent-github-federation.json'
[System.IO.File]::WriteAllText($federationFile, $federation)
az ad app federated-credential create --id $clientId --parameters "@$federationFile" --output none
if ($LASTEXITCODE -ne 0) { throw 'Falha ao criar federacao OIDC.' }
[pscustomobject]@{
    AZURE_CLIENT_ID = $clientId
    AZURE_TENANT_ID = $tenantId
    AZURE_SUBSCRIPTION_ID = $subscriptionId
    AZURE_RESOURCE_GROUP = $resourceGroup
    AZURE_CONTAINER_APP = $containerApp
} | Format-List
```

Cadastre os cinco valores exibidos em GitHub → Settings → Secrets and variables →
Actions → Variables (variáveis do repositório). Não há senha/client secret Azure.
A federação autoriza somente a branch main desse repositório. Não configure um
GitHub Environment sem adaptar o subject da federação.

Referência: https://docs.github.com/en/actions/how-tos/secure-your-work/security-harden-deployments/oidc-in-azure

## CI/CD

PRs para main continuam executando ci.yml. Cada push em main executa cd.yml:
restore → build → testes → build/push GHCR → login OIDC → atualização do Container
App com tag do commit. Se testes ou publicação falharem, não há deploy. Deploys
são serializados; a opção publish_only existe apenas para o bootstrap manual.
O workflow atualiza uma infraestrutura já criada: ele não executa Terraform.

Terraform gerencia infraestrutura, configuração, CORS e secrets. Após a criação,
a imagem é gerenciada pelo CD (ignore_changes no campo image), impedindo que um
terraform apply reverta a versão implantada. Alterar container_image no Terraform
não atualiza uma aplicação existente; para rollback, execute:

```powershell
$resourceGroup = terraform output -raw resource_group_name
$containerApp = terraform output -raw container_app_name
$image = Read-Host 'Imagem GHCR com a tag de um commit anterior'
az containerapp update --resource-group $resourceGroup --name $containerApp --image $image --output none
```

## Vercel e mobile

Na Vercel, defina VITE_API_BASE_URL com terraform output -raw api_url e faça novo
build/deploy do frontend. Configure EXPO_PUBLIC_API_URL com a mesma URL no mobile.
web_url deve ser o domínio estável de produção; URLs de preview não são autorizadas
pelo CORS. Ao mudar o domínio, atualize web_url e gere/aplique um novo plano.

## Custos e state

A assinatura estudantil usa créditos limitados. PostgreSQL, rede, logs e a API
ativa podem consumir créditos; esta infraestrutura não tem custo zero garantido.
container_min_replicas = 0 permite escalar a API a zero, mas pausa as automações
em processo durante a inatividade. O padrão 1 mantém essas automações ativas.

O state contém senha do banco, JWT e SMTP. Preserve-o em local protegido e nunca
o versione; o plano também pode conter secrets. PostgreSQL só é acessível na VNet.
Para remover, faça backup dos dados e execute terraform destroy: isso exclui o banco.
