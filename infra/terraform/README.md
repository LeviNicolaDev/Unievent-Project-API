# UniEvent na Azure

Esta configuração provisiona os serviços Azure necessários para a API, o portal web e o cliente mobile:

- **API:** Azure Container Apps no plano Consumption, com container de 0,25 vCPU / 0,5 GiB.
- **Web:** Azure Static Web Apps no plano Free.
- **Banco:** Azure Database for PostgreSQL Flexible Server, menor SKU burstable como padrão, 32 GiB, backup de 7 dias, sem redundância entre zonas e rede privada.
- **Logs:** Log Analytics com limite de ingestão de 0,1 GiB por dia.
- **Mobile:** não precisa de servidor. O app Expo usa a URL da API como variável de build.

A API e o PostgreSQL compartilham uma rede virtual; o banco não tem endpoint público. A senha do banco e a chave JWT, geradas pelo Terraform, são armazenadas como secrets do Container App. O state do Terraform contém esses valores e o token de deploy do Static Web Apps: não versione, compartilhe ou armazene esse state em um backend desprotegido.

## Antes de aplicar

1. Instale Terraform 1.8+ e Azure CLI. Entre na conta e selecione a assinatura estudantil:

   ```powershell
   az login
   az account set --subscription "<subscription-id>"
   az account show --query id -o tsv
   ```

2. Faça o push da API para `main`; o workflow publica a imagem no GitHub Container Registry (GHCR). Na primeira publicação, altere a visibilidade do pacote para **pública** em GitHub Packages. Assim não é necessário pagar pelo Azure Container Registry.
3. Copie `terraform.tfvars.example` para `terraform.tfvars` e informe a assinatura e a imagem. Mantenha o arquivo privado.
4. Confirme que a região escolhida oferece PostgreSQL Flexible Server `B_Standard_B1ms` e é permitida pela assinatura estudantil. O padrão usa `eastus` para compute/banco e `eastus2` para Static Web Apps; ajuste `location` se necessário.

## Criar os recursos

Execute neste diretório:

```powershell
terraform init
terraform plan -out=tfplan
terraform apply tfplan
```

Revise o plano antes de aplicar. O Azure for Students tem créditos limitados; PostgreSQL e uma réplica da API sempre ativa podem gerar custos mesmo com pouco tráfego. O menor servidor burstable e o site Free reduzem o custo, mas **nem todos os recursos são necessariamente gratuitos**. Confira os preços e limites atuais no portal Azure. Para reduzir o uso do Container Apps, defina `container_min_replicas = 0`; a API poderá escalar a zero, mas as automações executadas em processo não funcionarão enquanto não houver réplica. O Flexible Server continua gerando custos até ser destruído.

O servidor PostgreSQL é privado e a API está conectada à VNet. Nenhuma regra de firewall público é criada. Os backups duram 7 dias e a redundância geográfica fica desativada para reduzir custos. O limite de ingestão controla o consumo de logs; ao atingi-lo, novos logs deixam de ser ingeridos até o próximo período.

## Conectar os clientes

Após aplicar:

```powershell
terraform output -raw api_url
terraform output -raw web_url
terraform output -raw web_deployment_token
```

- Gere o web com `VITE_API_BASE_URL` apontando para a API e publique a pasta `dist` no Static Web Apps usando o token de deploy protegido. Configure esse token como secret de CI no repositório web; não o versione.
- Defina `EXPO_PUBLIC_API_URL` com a URL da API ao gerar/executar o Expo. Não há hospedagem Azure para o mobile; distribua o app pelo fluxo existente do Expo/EAS.
- SMTP é opcional. Defina `email_smtp_address` e `email_smtp_password` juntos para habilitar e-mails de conta/certificado. A senha é sensível, mas permanece no state do Terraform.

`terraform output -raw web_deployment_token` exibe uma credencial de deploy no terminal; use somente em uma sessão confiável. Se o token vazar, revogue-o no Azure.

## Atualizar e remover

Após cada publicação, atualize `container_image` com a tag do commit publicado e rode `terraform apply` para criar uma nova revisão. Para remover a infraestrutura, faça backup dos dados necessários e rode `terraform destroy` neste diretório. Isso também exclui o banco.
