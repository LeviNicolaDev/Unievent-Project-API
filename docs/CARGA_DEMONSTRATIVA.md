# Carga demonstrativa no deploy

A pipeline cd.yml define DemoSeed__Enabled=true. Na inicialização da API, após as migrations quando habilitadas, Deployment/DemoDataSeed.cs cria seis eventos públicos e três responsáveis fictícios, usando a imagem fornecida em wwwroot/imagens/school-placeholder.png.

As datas ficam entre 7 e 22 dias após a primeira execução, às 18h UTC. As inscrições abrem imediatamente e encerram uma hora antes de cada evento. Todos os registros são identificados como demonstração.

São utilizadas até três instituições ativas existentes, em ordem de ID. Se nenhuma existir, é criada uma instituição demonstrativa com dados fictícios e CNPJ composto de zeros. A carga não cria contas, inscrições, presenças ou certificados, nem envia e-mails.

## Execução única

O marcador demo-eventos-v1 na tabela PostgreSQL "__DeploymentSeeds" impede repetição por banco. Todos os inserts e o marcador fazem parte da mesma transação. A trava pg_advisory_xact_lock serializa réplicas concorrentes. Em caso de erro, a transação é revertida e a inicialização falha.

Novos deploys e reinícios não duplicam dados nem renovam as datas. Excluir manualmente eventos não faz a carga recriá-los. Não apague o marcador para corrigir falhas, pois isso pode duplicar dados. Uma base diferente recebe sua própria carga uma vez.

O banco precisa estar atualizado e o usuário deve ter permissão para criar a tabela de controle e inserir dados. Mantenha Database__MigrateOnStartup=true se as migrations forem aplicadas no início da API. Fora da pipeline, DemoSeed:Enabled é desativado por padrão.

Para desabilitar, altere DemoSeed__Enabled=false no workflow. Os dados existentes são preservados. Alterar somente o portal não é persistente, pois o próximo deploy define a variável novamente.

## Verificação

Procure nos logs "Carga demo-eventos-v1 aplicada: 6 eventos e 3 responsáveis." ou "já aplicada; nenhum registro alterado.". Abra o catálogo público e /imagens/school-placeholder.png no domínio da API.

DemoDataSeedTest verifica repetição, preservação de exclusões e concorrência em bancos temporários isolados. Defina UNIEVENT_TEST_POSTGRESQL para um servidor PostgreSQL de teste com permissão para criar bancos; não use produção.
