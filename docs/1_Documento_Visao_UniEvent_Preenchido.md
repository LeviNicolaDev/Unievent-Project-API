# UniEvent - Documento de Visão

## 1. Índice

2. Objetivo
3. Necessidade do Negócio
4. Descrição do Escopo
5. Equipe
6. Especificações Técnicas
7. Riscos
8. Cronograma de Marcos Resumido
9. Orçamento Resumido

## 2. Objetivo

O objetivo do projeto UniEvent é desenvolver uma solução de software para divulgação, gerenciamento, inscrição, controle de presença e emissão de certificados de eventos acadêmicos e institucionais relacionados às Fatecs.

O sistema é composto por três frentes principais:

- API backend responsável pelas regras de negócio, autenticação, persistência e integração com banco de dados.
- Portal web com área pública para consulta de eventos e área administrativa para Admin UniEvent e usuários da secretaria.
- Aplicativo mobile exclusivo para alunos Fatec, permitindo cadastro institucional, login, consulta de eventos, inscrição, favoritos, ingresso e QR Code.
- Área Web `Meus Ingressos` para o Público Geral, com listagem própria, histórico, ingresso individual responsivo, QR Code e estado do check-in.

A proposta busca centralizar a gestão de eventos, reduzir controles manuais, facilitar a participação dos alunos e oferecer indicadores confiáveis para administradores e instituições.

## 3. Necessidade do Negócio

Instituições acadêmicas realizam com frequência palestras, workshops, feiras, semanas tecnológicas e outros eventos complementares à formação dos alunos. Em muitos cenários, a divulgação, inscrição, controle de presença e emissão de certificados são feitos por ferramentas separadas, planilhas ou processos manuais.

Essa forma de trabalho gera problemas como:

- dificuldade para divulgar eventos de forma organizada;
- baixa visibilidade para alunos de outras unidades;
- retrabalho no cadastro de participantes;
- inconsistência no controle de presença;
- demora na emissão de certificados;
- ausência de indicadores consolidados para acompanhamento institucional;
- risco de acesso indevido a dados de outras instituições em ambientes multi-institucionais.

O UniEvent atende essa necessidade ao oferecer uma plataforma integrada. A web pública permite que alunos e público geral visualizem eventos. O mobile é voltado exclusivamente para alunos Fatec, com cadastro institucional e confirmação de e-mail. A secretaria gerencia eventos vinculados à sua instituição, enquanto o Admin UniEvent administra instituições, usuários secretaria e indicadores globais.

Com essa solução, a instituição ganha padronização operacional, melhora a experiência dos alunos e passa a contar com dados mais confiáveis sobre inscrições, presenças e certificados emitidos.

## 4. Descrição do Escopo

O UniEvent será composto por módulos integrados de backend, web e mobile.

### 4.1. Itens dentro do escopo

| Módulo                    | Descrição                                                                                                                    |
| ------------------------- | ---------------------------------------------------------------------------------------------------------------------------- |
| Backend/API               | API em ASP.NET Core responsável por autenticação, regras de negócio, integrações, endpoints REST, validações e persistência. |
| Banco de dados            | Banco SQL Server com migrations do Entity Framework Core.                                                                    |
| Portal web público        | Páginas para landing page, descoberta de eventos públicos, filtro por instituição e detalhes do evento.                      |
| Portal web administrativo | Área autenticada para Admin UniEvent e Usuário Secretaria.                                                                   |
| Administração global      | Cadastro do Admin UniEvent, gestão de instituições, gestão de usuários secretaria, dashboard global e automações.            |
| Gestão institucional      | Dashboard da instituição, cadastro e manutenção de eventos, responsáveis, certificados e operação de check-in.               |
| Aplicativo mobile         | Aplicativo React Native/Expo exclusivo para alunos Fatec.                                                                    |
| Cadastro de aluno         | Cadastro institucional com seleção de instituição e confirmação de e-mail.                                                   |
| Inscrição em eventos      | Consulta de eventos disponíveis, regras de público permitido e inscrição do aluno.                                           |
| Ingresso e QR Code        | Exibição do ingresso do aluno no Mobile e do Público Geral na Web, usando o código único da participação na validação do check-in. |
| Check-in                  | Validação feita pela secretaria na web, com código do QR Code, evento e janela de horário permitida.                         |
| Certificados              | Emissão idempotente de PDF após presença confirmada, com armazenamento, download próprio e envio anexado para alunos FATEC e Público Geral. |
| E-mail                    | Envio SMTP de confirmação de conta e certificado PDF anexado, com estados de falha, retry limitado e auditoria.               |
| Docker                    | Ambiente com Docker Compose para banco, backend, web e mobile.                                                               |
| Testes                    | Testes unitários para serviços, validações e regras críticas.                                                                |

### 4.2. Itens fora do escopo inicial

| Item                                                  | Justificativa                                                                                                        |
| ----------------------------------------------------- | -------------------------------------------------------------------------------------------------------------------- |
| Pagamentos ou cobrança de inscrição                   | O projeto atual é voltado para eventos acadêmicos sem fluxo financeiro.                                              |
| Integração oficial com sistemas do Centro Paula Souza | A validação institucional é feita por e-mail e instituição selecionada; integração oficial pode ser evolução futura. |
| Publicação em lojas de aplicativos                    | O mobile está preparado em Expo, mas publicação em Play Store/App Store não faz parte do escopo inicial.             |
| Leitura de QR Code por câmera no web                  | O fluxo atual permite informar o código; leitura por câmera pode ser incremento posterior.                           |
| Notificações push completas                           | Preferências existem, mas provedor push, tokens de dispositivo e central de notificações ficam para evolução.        |
| Paginação, BI avançado e exportação de relatórios     | Dashboards principais existem, mas recursos analíticos avançados são evolução futura.                                |
| Controle de localização obrigatório para check-in     | O MVP valida o QR Code e a janela de horário; localização permanece como legado opcional/futuro.                     |
| Chat, fórum ou comunicação entre participantes        | O foco é gestão de eventos, inscrições, presença e certificados.                                                     |

## 5. Equipe

O repositório não contém uma lista formal de todos os integrantes. A tabela abaixo registra a equipe identificada e os papéis recomendados para o projeto, podendo ser ajustada com os nomes oficiais antes da entrega.

| Integrante  | Formação/experiência                              | Papel no projeto                               | Responsabilidades                                                                                         |
| ----------- | ------------------------------------------------- | ---------------------------------------------- | --------------------------------------------------------------------------------------------------------- |
| Ryan Souza  | Desenvolvimento de software em ambiente acadêmico | Desenvolvedor full stack / responsável técnico | Implementação do backend, frontend web, aplicativo mobile, banco de dados, Docker, testes e documentação. |
| Levi Nicola | Desenvolvimento de software em ambiente acadêmico | Desenvolvedor full stack / responsável técnico | Implementação do backend, frontend web, aplicativo mobile, banco de dados, Docker, testes e documentação. |

## 6. Especificações Técnicas

| Categoria                   | Especificação                                                                                                            |
| --------------------------- | ------------------------------------------------------------------------------------------------------------------------ |
| Arquitetura backend         | API em ASP.NET Core organizada em camadas: `Unievent.Api`, `Unievent.Application`, `Unievent.Domain` e `Unievent.Infra`. |
| Linguagem backend           | C# com .NET 10.                                                                                                          |
| Persistência                | Entity Framework Core com SQL Server.                                                                                    |
| Banco de dados              | Microsoft SQL Server 2022 em ambiente local/Docker.                                                                      |
| Autenticação                | JWT Bearer com perfis Admin, Secretaria e Aluno.                                                                         |
| Segurança por perfil        | Admin global sem instituição; Secretaria vinculada a uma instituição; Aluno com tipo de participante e instituição.      |
| Validações                  | FluentValidation, validações de domínio e regras nos serviços de aplicação.                                              |
| Criptografia de senha       | BCrypt.Net.                                                                                                              |
| Documentação da API         | OpenAPI, Scalar e Swagger/Swashbuckle.                                                                                   |
| Portal web                  | React 19 com Vite 7, React Router e lucide-react.                                                                        |
| Aplicativo mobile           | React Native 0.81, Expo 54 e React Navigation.                                                                           |
| QR Code                     | `react-native-qrcode-svg` no Mobile e `qrcode.react` na Web, ambos codificando `Participacao.CodigoIngresso`.             |
| Imagens e arquivos          | Upload e exposição de imagens via API/static files.                                                                      |
| PDF                         | PDFsharp/MigraDoc com template A4 paisagem e fontes Liberation Sans incorporadas.                                       |
| E-mail                      | SMTP configurável para confirmação de conta e envio de certificados PDF anexados.                                      |
| Containerização             | Docker e Docker Compose para banco, API, web e mobile.                                                                   |
| Testes                      | xUnit, Moq, FluentAssertions e Bogus.                                                                                    |
| Ambiente de desenvolvimento | API em porta configurável, web em Vite, mobile em Expo e SQL Server via Docker.                                          |

### 6.1. Principais entidades

| Entidade               | Finalidade                                                                                            |
| ---------------------- | ----------------------------------------------------------------------------------------------------- |
| UsuarioUnievent        | Representa o administrador global da plataforma.                                                      |
| UsuarioSecretaria      | Representa usuários institucionais vinculados a uma instituição.                                      |
| Instituicao            | Representa a Fatec/instituição organizadora, incluindo dados cadastrais e endereço.                   |
| Aluno                  | Representa o participante autenticado: aluno FATEC interno ou Público Geral externo.                   |
| Evento                 | Representa eventos acadêmicos com instituição, responsável, público permitido, local, data e horário. |
| ResponsavelEvento      | Representa pessoas responsáveis por eventos.                                                          |
| Participacao           | Registra inscrição ativa/cancelada, código único do ingresso, presença, PDF emitido e estado idempotente da entrega por e-mail. |
| Certificado            | Representa o template e a habilitação de certificado configurados para um evento.                      |
| PreferenciaNotificacao | Representa preferências do aluno relacionadas a recomendações e notificações.                         |

### 6.2. Regras importantes do produto

- O aplicativo mobile é exclusivo para alunos Fatec.
- A web pública pode ser visualizada por alunos e público geral.
- O Usuário Secretaria sempre atua vinculado a uma instituição.
- Ao criar evento como secretaria, a instituição é obtida pelo usuário logado.
- Eventos podem ter público permitido como público geral, todos alunos Fatec ou alunos da instituição.
- O check-in depende de QR Code/código válido, janela de horário e confirmação por operador ativo da instituição do evento.
- O Público Geral consulta somente ingressos cujo `AlunoId` corresponde ao `NameIdentifier` do JWT.
- O QR Code contém somente o `CodigoIngresso` aleatório da participação; inscrição cancelada não realiza check-in.
- A simples exibição/leitura do QR Code não confirma presença nem emite certificado.
- A emissão automática ocorre no máximo uma vez por participação com presença confirmada e certificado configurado.
- O mesmo processo atende aluno FATEC e Público Geral e envia o PDF oficial como anexo.
- Falha de PDF ou e-mail não desfaz a presença; falhas temporárias podem ser reprocessadas pelo worker.

## 7. Riscos

| ID  | Risco                                                                                    | Probabilidade | Impacto | Plano de mitigação/contingência                                                                    |
| --- | ---------------------------------------------------------------------------------------- | ------------- | ------- | -------------------------------------------------------------------------------------------------- |
| R01 | Indisponibilidade do banco SQL Server no ambiente local ou Docker.                       | Média         | Alto    | Usar Docker Compose, healthcheck do banco, documentação de portas e variáveis de ambiente.         |
| R02 | Falha no envio de e-mails por credenciais SMTP, bloqueio do provedor ou limite de envio. | Média         | Alto    | Persistir presença e PDF antes do SMTP; classificar falha, aplicar retry limitado e exigir análise quando o resultado for incerto. |
| R03 | Usuário secretaria acessar dados de outra instituição.                                   | Baixa/Média   | Alto    | Validar `instituicao_id` no JWT e aplicar filtros de acesso no backend.                            |
| R04 | Aluno tentar se cadastrar sem vínculo institucional válido.                              | Média         | Médio   | Exigir e-mail institucional Fatec, instituição ativa e confirmação de e-mail.                      |
| R05 | Check-in realizado fora do horário permitido.                                            | Média         | Médio   | Validar janela de horário no backend e exibir mensagens claras no frontend.                        |
| R06 | QR Code reutilizado ou informado para evento incorreto.                                  | Média         | Alto    | Validar código, evento e participação no backend; manter idempotência do check-in.                 |
| R07 | Divergência entre frontend e backend em contratos de API.                                | Média         | Médio   | Centralizar serviços de API no frontend e validar com testes/builds.                               |
| R08 | Crescimento do escopo durante o desenvolvimento.                                         | Alta          | Médio   | Separar MVP de evoluções futuras e manter itens fora do escopo documentados.                       |
| R09 | Dependência de versões recentes de frameworks.                                           | Média         | Médio   | Fixar versões em arquivos de projeto e package-lock, além de validar builds.                       |
| R10 | Perda de dados em ambiente local de desenvolvimento.                                     | Baixa/Média   | Alto    | Usar volumes Docker para SQL Server e migrations versionadas.                                      |
| R11 | Duplo clique, retry ou múltiplas instâncias enviarem o mesmo certificado.                 | Média         | Alto    | Reservar a participação com atualização condicional atômica e tornar estados finais inelegíveis ao processamento automático. |

## 8. Cronograma de Marcos Resumido

As datas abaixo são estimativas baseadas no estado atual do projeto e podem ser ajustadas conforme calendário acadêmico e disponibilidade da equipe.

| Marco                          | Data estimada | Entrega                                                                                |
| ------------------------------ | ------------- | -------------------------------------------------------------------------------------- |
| Início do Projeto              | 17/08/2026    | Definição da evolução do UniEvent e análise do estado inicial.                         |
| Especificação de Requisitos    | 20/08/2026    | Regras de mobile exclusivo para alunos Fatec, multi-instituição e perfis de acesso.    |
| Apresentação de Protótipos     | 21/08/2026    | Telas web e mobile revisadas para cadastro, eventos, check-in e dashboards.            |
| Modelagem da Iteração Inicial  | 24/08/2026    | Entidades, DTOs, serviços, migrations e diagrama de caso de uso.                       |
| Desenvolvimento do Backend     | 31/08/2026    | API, autenticação, regras institucionais, eventos, check-in, certificados e e-mail.    |
| Desenvolvimento do Portal Web  | 07/09/2026    | Área pública, Admin UniEvent e área institucional da secretaria.                       |
| Desenvolvimento do Mobile      | 14/09/2026    | Cadastro/login de aluno, eventos, inscrição, ingresso, QR Code, perfil e preferências. |
| Testes Unitários e Integração  | 21/09/2026    | Validação de serviços, regras de negócio, fluxos web/mobile e integração com API.      |
| Testes de Aceitação do Usuário | 28/09/2026    | Testes com cenários de aluno, secretaria e admin.                                      |
| Instalação/Containerização     | 30/09/2026    | Docker Compose para banco, backend, web e mobile.                                      |
| Entrega do Projeto Final       | 05/10/2026    | Documentação, código validado e apresentação final.                                    |

## 9. Orçamento Resumido

Os valores abaixo são estimativos para documentação acadêmica. Em um projeto real, o orçamento deve ser revisado conforme equipe contratada, infraestrutura de produção, hospedagem, domínio e volume de usuários.

### 9.1. Custos fixos

| Item                        | Valor estimado | Observação                                                        |
| --------------------------- | -------------: | ----------------------------------------------------------------- |
| Hardware de desenvolvimento |        R$ 0,00 | Uso de equipamento próprio/acadêmico.                             |
| Licenças de software        |        R$ 0,00 | Tecnologias principais possuem uso gratuito para desenvolvimento. |
| Banco de dados local        |        R$ 0,00 | SQL Server em ambiente local/Docker.                              |
| Treinamentos                |        R$ 0,00 | Considerado estudo da equipe durante o projeto acadêmico.         |
| Hospedagem inicial          |        R$ 0,00 | Ambiente local durante desenvolvimento.                           |

### 9.2. Custos variáveis simulados

| Item                         | Valor estimado | Observação                                                  |
| ---------------------------- | -------------: | ----------------------------------------------------------- |
| Desenvolvimento backend      |    R$ 6.000,00 | Estimativa acadêmica/profissional para API, banco e regras. |
| Desenvolvimento frontend web |    R$ 4.000,00 | Estimativa para área pública e administrativa.              |
| Desenvolvimento mobile       |    R$ 4.000,00 | Estimativa para aplicativo Expo/React Native.               |
| Testes e documentação        |    R$ 2.000,00 | Casos de uso, Documento Visão, testes e ajustes.            |
| Margem de contingência       |    R$ 2.400,00 | Aproximadamente 15% sobre custos variáveis.                 |

### 9.3. Total estimado

| Categoria                  |        Valor |
| -------------------------- | -----------: |
| Custos fixos               |      R$ 0,00 |
| Custos variáveis simulados | R$ 16.000,00 |
| Contingência               |  R$ 2.400,00 |
| Total estimado             | R$ 18.400,00 |

Para fins de Projeto Final de Curso, o custo financeiro direto pode ser considerado reduzido ou nulo quando o desenvolvimento for realizado pela própria equipe acadêmica, utilizando ferramentas gratuitas e infraestrutura local.
