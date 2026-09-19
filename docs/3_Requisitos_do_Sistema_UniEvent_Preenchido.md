# UniEvent - Documento de Requisitos

## 1. Índice

2. Objetivo  
3. Técnicas Utilizadas na Elicitação de Requisitos  
4. Requisitos Funcionais  
4.1. Grupo: Portal web público  
4.1.1. RF001 - Acessar landing page  
4.1.2. RF002 - Descobrir eventos públicos  
4.1.3. RF003 - Filtrar eventos por instituição  
4.1.4. RF004 - Visualizar detalhes de evento público  
4.2. Grupo: Autenticação, perfis e usuários  
4.2.1. RF005 - Cadastrar admin inicial  
4.2.2. RF006 - Realizar login web  
4.2.3. RF007 - Solicitar cadastro de usuário secretaria  
4.2.4. RF008 - Aprovar, bloquear ou recusar secretaria  
4.2.5. RF009 - Confirmar e-mail de conta  
4.2.6. RF010 - Cadastrar aluno institucional no mobile  
4.2.7. RF011 - Realizar login mobile  
4.2.8. RF012 - Gerenciar perfil do aluno  
4.3. Grupo: Instituições  
4.3.1. RF013 - Gerenciar instituições  
4.3.2. RF014 - Listar instituições ativas para seleção  
4.4. Grupo: Eventos e responsáveis  
4.4.1. RF015 - Gerenciar eventos da instituição  
4.4.2. RF016 - Cadastrar evento  
4.4.3. RF017 - Editar evento  
4.4.4. RF018 - Excluir evento  
4.4.5. RF019 - Definir público permitido do evento  
4.4.6. RF020 - Gerenciar responsáveis por eventos  
4.5. Grupo: Experiência do aluno no mobile  
4.5.1. RF021 - Consultar eventos disponíveis no mobile  
4.5.2. RF022 - Visualizar detalhes de evento no mobile  
4.5.3. RF023 - Favoritar evento  
4.5.4. RF024 - Inscrever-se em evento  
4.5.5. RF025 - Visualizar meus eventos  
4.5.6. RF026 - Visualizar ingresso e QR Code  
4.5.7. RF027 - Configurar preferências  
4.5.8. RF028 - Consultar eventos recomendados  
4.6. Grupo: Check-in e certificados  
4.6.1. RF029 - Validar check-in  
4.6.2. RF030 - Verificar janela de horário do check-in  
4.6.3. RF031 - Emitir certificado após presença  
4.6.4. RF032 - Enviar certificado por e-mail  
4.6.5. RF033 - Executar automações de eventos  
4.6.6. RF039 - Consultar e baixar certificado próprio  
4.7. Grupo: Dashboards e indicadores  
4.7.1. RF034 - Consultar dashboard administrativo  
4.7.2. RF035 - Consultar dashboard institucional  
4.7.3. RF036 - Consultar dashboard individual do evento  
4.8. Grupo: Público Geral na Web  
4.8.1. RF037 - Cadastrar e autenticar Público Geral  
4.8.2. RF038 - Inscrever Público Geral em evento público  
4.8.3. RF040 - Consultar Meus Ingressos e apresentar QR Code  
5. Requisitos Não Funcionais  
5.1. Grupo: Segurança e privacidade  
5.1.1. RNF001 - Autenticação com JWT  
5.1.2. RNF002 - Autorização por perfil e instituição  
5.1.3. RNF003 - Senhas protegidas por hash  
5.1.4. RNF004 - Confirmação de e-mail  
5.1.5. RNF005 - Configurações sensíveis por ambiente  
5.2. Grupo: Usabilidade e interface  
5.2.1. RNF006 - Interface web responsiva  
5.2.2. RNF007 - Experiência mobile dedicada  
5.2.3. RNF008 - Mensagens claras de erro  
5.3. Grupo: Confiabilidade e integridade  
5.3.1. RNF009 - Migrations versionadas  
5.3.2. RNF010 - Idempotência no check-in  
5.3.3. RNF011 - Falha de e-mail não desfaz presença  
5.3.4. RNF018 - Capacidade consistente sob concorrência  
5.3.5. RNF019 - Idempotência da certificação distribuída  
5.3.6. RNF020 - PDF válido e portátil  
5.3.7. RNF021 - Resultado SMTP incerto não gera retry cego  
5.4. Grupo: Desempenho e disponibilidade  
5.4.1. RNF012 - Consultas filtráveis  
5.4.2. RNF013 - Ambiente containerizado  
5.4.3. RNF014 - Healthcheck do banco  
5.5. Grupo: Manutenibilidade e suporte  
5.5.1. RNF015 - Arquitetura em camadas  
5.5.2. RNF016 - Documentação OpenAPI  
5.5.3. RNF017 - Testes automatizados  
6. Regras de Negócio  
7. Matriz de relacionamento Requisitos Funcionais x Regras de Negócio  

## 2. Objetivo

Este documento tem como objetivo especificar os requisitos funcionais, requisitos não funcionais e regras de negócio do sistema UniEvent. A especificação foi elaborada com base no projeto existente, no Documento de Visão, nos Casos de Uso, nos Diagramas de Atividades e na análise dos módulos backend, web e mobile.

O UniEvent é uma plataforma para divulgação, gestão, inscrição, check-in e certificação de eventos acadêmicos e institucionais, composta por API ASP.NET Core, portal web React/Vite e aplicativo mobile React Native/Expo exclusivo para alunos Fatec.

## 3. Técnicas Utilizadas na Elicitação de Requisitos

As técnicas utilizadas para identificar e detalhar os requisitos foram:

- Análise documental dos templates acadêmicos, Documento de Visão, Casos de Uso e Atividades do Negócio.
- Inspeção do código-fonte do backend, frontend web e aplicativo mobile.
- Mapeamento dos atores e fluxos do diagrama de caso de uso.
- Derivação dos requisitos a partir dos 46 casos de uso documentados.
- Análise das regras de autenticação, autorização, instituição, público permitido, inscrição, check-in e certificados.
- Priorização por valor de negócio, impacto no fluxo principal e dependência técnica.

## 4. Requisitos Funcionais

Os requisitos funcionais foram organizados por módulos do sistema: portal web público, autenticação e usuários, instituições, eventos, experiência mobile, check-in/certificados e dashboards.

### 4.1. Grupo: Portal web público

#### 4.1.1. RF001 - Acessar landing page

**Prioridade:** Alta.  
**Especificação da Regra de Negócio:** A web pública deve permitir que visitantes acessem a página inicial do UniEvent sem autenticação.  
**Detalhes da implementação prevista:** Disponibilizar rota pública `/` no frontend web, exibindo informações gerais do projeto, identidade visual, chamada para descoberta de eventos e atalhos para login/cadastro.

#### 4.1.2. RF002 - Descobrir eventos públicos

**Prioridade:** Alta.  
**Especificação da Regra de Negócio:** O sistema deve permitir que visitantes consultem eventos com visibilidade pública.  
**Detalhes da implementação prevista:** Consumir a API pública de eventos e exibir cards com nome, data, horário, categoria, instituição, local e imagem do evento.

#### 4.1.3. RF003 - Filtrar eventos por instituição

**Prioridade:** Alta.  
**Especificação da Regra de Negócio:** A listagem pública deve permitir filtro por instituições cadastradas e ativas.  
**Detalhes da implementação prevista:** Carregar instituições pelo endpoint público e aplicar o identificador selecionado como filtro na consulta de eventos.

#### 4.1.4. RF004 - Visualizar detalhes de evento público

**Prioridade:** Alta.  
**Especificação da Regra de Negócio:** O usuário público deve conseguir abrir os detalhes de eventos disponíveis.  
**Detalhes da implementação prevista:** Criar rota de detalhes com consulta por ID do evento e exibição de descrição, data, horário, instituição, local, categoria, público permitido e imagem.

### 4.2. Grupo: Autenticação, perfis e usuários

#### 4.2.1. RF005 - Cadastrar admin inicial

**Prioridade:** Alta.  
**Especificação da Regra de Negócio:** O primeiro Admin UniEvent deve ser cadastrado sem vínculo institucional.  
**Detalhes da implementação prevista:** Implementar formulário web e endpoint `UsuarioUnievent/admin-inicial`, validando existência prévia de admin, nome, e-mail e senha.

#### 4.2.2. RF006 - Realizar login web

**Prioridade:** Alta.  
**Especificação da Regra de Negócio:** Admin UniEvent e Usuário Secretaria devem autenticar-se pela web com e-mail e senha.  
**Detalhes da implementação prevista:** Usar endpoint de autenticação web, gerar JWT com role, tipo de usuário, status e `instituicao_id` quando houver.

#### 4.2.3. RF007 - Solicitar cadastro de usuário secretaria

**Prioridade:** Alta.  
**Especificação da Regra de Negócio:** Usuários secretaria são institucionais e devem estar vinculados a uma instituição.  
**Detalhes da implementação prevista:** Permitir cadastro/solicitação com nome, e-mail institucional, senha, chave e instituição, mantendo status pendente quando aplicável.

#### 4.2.4. RF008 - Aprovar, bloquear ou recusar secretaria

**Prioridade:** Alta.  
**Especificação da Regra de Negócio:** Somente Admin UniEvent pode alterar o status de acesso de usuários secretaria.  
**Detalhes da implementação prevista:** Disponibilizar ação administrativa para status ativo, pendente, recusado ou bloqueado e bloquear login de contas não ativas.

#### 4.2.5. RF009 - Confirmar e-mail de conta

**Prioridade:** Alta.  
**Especificação da Regra de Negócio:** Aluno e usuário secretaria devem confirmar e-mail antes de concluir o fluxo de acesso.  
**Detalhes da implementação prevista:** Gerar chave de confirmação, enviar link por e-mail e validar chave em endpoint centralizado de confirmação.

#### 4.2.6. RF010 - Cadastrar aluno institucional no mobile

**Prioridade:** Alta.  
**Especificação da Regra de Negócio:** O mobile é exclusivo para alunos Fatec; o cadastro deve exigir vínculo institucional.  
**Detalhes da implementação prevista:** Implementar formulário mobile com nome, e-mail institucional, senha, data de nascimento, foto e instituição ativa selecionada.

#### 4.2.7. RF011 - Realizar login mobile

**Prioridade:** Alta.  
**Especificação da Regra de Negócio:** Apenas alunos Fatec com e-mail confirmado devem acessar o aplicativo mobile.  
**Detalhes da implementação prevista:** Validar credenciais, role Aluno, tipo participante Interno, instituição vinculada e e-mail confirmado antes de liberar sessão.

#### 4.2.8. RF012 - Gerenciar perfil do aluno

**Prioridade:** Média.  
**Especificação da Regra de Negócio:** O aluno autenticado deve conseguir visualizar e atualizar seus próprios dados.  
**Detalhes da implementação prevista:** Consumir endpoint autenticado de aluno, exibir nome, e-mail, instituição, data de nascimento e foto, permitindo atualização de campos editáveis.

### 4.3. Grupo: Instituições

#### 4.3.1. RF013 - Gerenciar instituições

**Prioridade:** Alta.  
**Especificação da Regra de Negócio:** Somente Admin UniEvent deve cadastrar, editar, ativar ou desativar instituições.  
**Detalhes da implementação prevista:** Disponibilizar telas web e endpoints para criar, listar, editar e alterar status de instituições com dados cadastrais e endereço.

#### 4.3.2. RF014 - Listar instituições ativas para seleção

**Prioridade:** Alta.  
**Especificação da Regra de Negócio:** Alunos e secretarias devem selecionar apenas instituições cadastradas e ativas.  
**Detalhes da implementação prevista:** Criar endpoint público de instituições ativas com dados mínimos, usado por cadastro mobile e formulários web.

### 4.4. Grupo: Eventos e responsáveis

#### 4.4.1. RF015 - Gerenciar eventos da instituição

**Prioridade:** Alta.  
**Especificação da Regra de Negócio:** Secretaria deve gerenciar somente eventos da instituição vinculada; Admin pode gerenciar eventos globais.  
**Detalhes da implementação prevista:** Aplicar escopo institucional no backend e usar a instituição do JWT para secretaria ao criar/listar/editar eventos.

#### 4.4.2. RF016 - Cadastrar evento

**Prioridade:** Alta.  
**Especificação da Regra de Negócio:** Todo evento novo deve possuir instituição, responsável, datas, horários, local e público permitido.  
**Detalhes da implementação prevista:** Implementar formulário web com campos do evento, upload de imagem, validação de responsável e associação automática à instituição correta.

#### 4.4.3. RF017 - Editar evento

**Prioridade:** Alta.  
**Especificação da Regra de Negócio:** Usuários autorizados podem alterar dados de eventos dentro do próprio escopo institucional.  
**Detalhes da implementação prevista:** Consultar evento por ID, validar acesso à instituição do evento e salvar alterações permitidas.

#### 4.4.4. RF018 - Excluir evento

**Prioridade:** Média.  
**Especificação da Regra de Negócio:** Usuários autorizados podem remover eventos quando não houver restrição de vínculo.  
**Detalhes da implementação prevista:** Implementar exclusão por ID com validação de permissão e tratamento de vínculos que impeçam remoção.

#### 4.4.5. RF019 - Definir público permitido do evento

**Prioridade:** Alta.  
**Especificação da Regra de Negócio:** Eventos devem controlar quem pode visualizar ou participar: público geral, todos alunos Fatec ou alunos da instituição.  
**Detalhes da implementação prevista:** Persistir enum de público permitido e aplicar regra em listagens, detalhes e inscrição.

#### 4.4.6. RF020 - Gerenciar responsáveis por eventos

**Prioridade:** Média.  
**Especificação da Regra de Negócio:** Responsáveis devem ser cadastrados e associados à instituição/evento correto.  
**Detalhes da implementação prevista:** Disponibilizar CRUD de responsáveis com nome, foto e instituição, validando vínculo antes da associação ao evento.

### 4.5. Grupo: Experiência do aluno no mobile

#### 4.5.1. RF021 - Consultar eventos disponíveis no mobile

**Prioridade:** Alta.  
**Especificação da Regra de Negócio:** Aluno Fatec autenticado deve visualizar eventos compatíveis com seu perfil.  
**Detalhes da implementação prevista:** Consumir endpoint autenticado de eventos, aplicar regras de público permitido e permitir filtro por instituição/categoria quando disponível.

#### 4.5.2. RF022 - Visualizar detalhes de evento no mobile

**Prioridade:** Alta.  
**Especificação da Regra de Negócio:** Aluno deve consultar informações completas do evento antes de se inscrever.  
**Detalhes da implementação prevista:** Exibir dados detalhados do evento e ações como favoritar, inscrever-se ou visualizar ingresso.

#### 4.5.3. RF023 - Favoritar evento

**Prioridade:** Baixa.  
**Especificação da Regra de Negócio:** Aluno deve conseguir marcar eventos de interesse.  
**Detalhes da implementação prevista:** Atualizar estado de favorito na interface mobile e manter possibilidade de integração persistente conforme evolução.

#### 4.5.4. RF024 - Inscrever-se em evento

**Prioridade:** Alta.  
**Especificação da Regra de Negócio:** Aluno só pode se inscrever se atender ao público permitido e ainda não estiver inscrito.  
**Detalhes da implementação prevista:** Criar participação vinculando aluno e evento, gerar código de ingresso e retornar confirmação ao mobile.

#### 4.5.5. RF025 - Visualizar meus eventos

**Prioridade:** Alta.  
**Especificação da Regra de Negócio:** Aluno deve consultar eventos em que possui participação registrada.  
**Detalhes da implementação prevista:** Listar participações do aluno com dados do evento, status de presença e disponibilidade de ingresso/certificado.

#### 4.5.6. RF026 - Visualizar ingresso e QR Code

**Prioridade:** Alta.  
**Especificação da Regra de Negócio:** Aluno FATEC ou Público Geral inscrito deve apresentar ingresso com QR Code para validação do check-in.  
**Detalhes da implementação prevista:** Recuperar o `CodigoIngresso` da participação e renderizar exatamente esse valor no aplicativo mobile ou na área Web `Meus Ingressos`.

#### 4.5.7. RF027 - Configurar preferências

**Prioridade:** Média.  
**Especificação da Regra de Negócio:** Aluno deve ajustar preferências de uso, tema e opções relacionadas a notificações/recomendações.  
**Detalhes da implementação prevista:** Exibir tela de configurações e persistir preferências quando houver integração disponível no backend.

#### 4.5.8. RF028 - Consultar eventos recomendados

**Prioridade:** Média.  
**Especificação da Regra de Negócio:** Aluno pode receber recomendações conforme preferências e eventos disponíveis.  
**Detalhes da implementação prevista:** Disponibilizar endpoint de recomendações autenticado e respeitar regras de elegibilidade e opt-out.

### 4.6. Grupo: Check-in e certificados

#### 4.6.1. RF029 - Validar check-in

**Prioridade:** Alta.  
**Especificação da Regra de Negócio:** A presença deve ser confirmada por Secretaria, operador de check-in ou Admin autorizado, com código do ingresso e evento correto. Aluno e Público Geral não podem confirmar a própria presença.  
**Detalhes da implementação:** O endpoint valida JWT, usuário ainda ativo/aprovado, role atual, instituição, evento, participação, código e status. A confirmação usa atualização atômica e só dispara certificação na primeira mudança de estado.

#### 4.6.2. RF030 - Verificar janela de horário do check-in

**Prioridade:** Alta.  
**Especificação da Regra de Negócio:** O check-in só deve ser aceito dentro da janela de horário permitida do evento.  
**Detalhes da implementação:** Interpretar a data civil do evento no fuso `America/Sao_Paulo`, comparar com o instante atual e retornar erro claro quando estiver fora da janela.

#### 4.6.3. RF031 - Emitir certificado após presença

**Prioridade:** Alta.  
**Especificação da Regra de Negócio:** Certificado PDF só deve ser emitido para participação com presença confirmada e evento com template de certificado configurado, tanto para aluno FATEC quanto para Público Geral.  
**Detalhes da implementação:** O serviço automático reserva a participação, gera o PDF com dados do participante/evento, persiste bytes, nome, destinatário, emissão e código de validação antes de iniciar o SMTP.

#### 4.6.4. RF032 - Enviar certificado por e-mail

**Prioridade:** Alta.  
**Especificação da Regra de Negócio:** O PDF oficial deve ser enviado como anexo ao e-mail cadastrado do aluno FATEC ou Público Geral.  
**Detalhes da implementação:** Usar SMTP configurável, conteúdo HTML escapado e anexo `application/pdf`; registrar destinatário, data, status e erro sem desfazer a presença confirmada.

#### 4.6.5. RF033 - Executar automações de eventos

**Prioridade:** Média.  
**Especificação da Regra de Negócio:** Admin pode acionar rotinas automáticas e o backend pode processar certificados pendentes.  
**Detalhes da implementação:** O endpoint manual é restrito ao Admin global. O worker processa pendências e falhas temporárias elegíveis; falhas permanentes ou de resultado incerto exigem análise e não são reenviadas automaticamente.

#### 4.6.6. RF039 - Consultar e baixar certificado próprio

**Prioridade:** Alta.  
**Especificação da Regra de Negócio:** O participante autenticado deve consultar os estados das próprias inscrições e baixar somente o PDF associado à sua participação confirmada.  
**Detalhes da implementação:** Expor `GET /api/Certificado/meus` e `GET /api/Certificado/eventos/{eventoId}/pdf` para role `Aluno`, cruzando o identificador do JWT com `Participacao.AlunoId` e respondendo o PDF sem cache.

### 4.7. Grupo: Dashboards e indicadores

#### 4.7.1. RF034 - Consultar dashboard administrativo

**Prioridade:** Média.  
**Especificação da Regra de Negócio:** Admin UniEvent deve visualizar indicadores globais da plataforma.  
**Detalhes da implementação prevista:** Implementar API e tela web com totais de instituições, secretarias, alunos, eventos, inscrições, presenças e certificados.

#### 4.7.2. RF035 - Consultar dashboard institucional

**Prioridade:** Média.  
**Especificação da Regra de Negócio:** Usuário Secretaria deve visualizar indicadores restritos à sua instituição.  
**Detalhes da implementação prevista:** Usar `instituicao_id` do JWT para filtrar métricas institucionais, sem aceitar instituição por parâmetro aberto.

#### 4.7.3. RF036 - Consultar dashboard individual do evento

**Prioridade:** Alta.  
**Especificação da Regra de Negócio:** Usuário Secretaria deve consultar indicadores de um evento individual pertencente à instituição vinculada ao seu JWT.  
**Detalhes da implementação prevista:** Disponibilizar endpoint `GET /api/Evento/{eventId}/dashboard`, validando `Evento.InstituicaoId` contra `instituicao_id` e retornando DTO com capacidade, inscrições, vagas, check-ins, ausentes, ocupação, presença, status, certificado e distribuição por tipo de participante.

### 4.8. Grupo: Público Geral na Web

#### 4.8.1. RF037 - Cadastrar e autenticar Público Geral

**Prioridade:** Alta.  
**Especificação da Regra de Negócio:** Pessoas do Público Geral devem criar conta e autenticar-se pela aplicação web pública sem exigência de e-mail institucional.  
**Detalhes da implementação prevista:** Criar endpoints `POST /api/Auth/cadastro-publico` e `POST /api/Auth/login-publico`, gerando JWT para participante externo modelado como `Aluno` com `TipoParticipante.Externo`.

#### 4.8.2. RF038 - Inscrever Público Geral em evento público

**Prioridade:** Alta.  
**Especificação da Regra de Negócio:** Público Geral só pode se inscrever em evento com `PublicoPermitido = PublicoGeral`, respeitando período de inscrição, duplicidade e capacidade.  
**Detalhes da implementação prevista:** Reutilizar o endpoint de inscrição existente, aplicar `EventoRules.PodeParticipar` no backend, calcular vagas por `Participacao` e indicar lotação/dados de inscrição no frontend público.

#### 4.8.3. RF040 - Consultar Meus Ingressos e apresentar QR Code

**Prioridade:** Alta.  
**Especificação da Regra de Negócio:** Público Geral autenticado deve consultar somente as próprias participações, visualizar um ingresso por evento e apresentar o QR Code compatível com o check-in institucional.  
**Detalhes da implementação:** A rota Web `/meus-ingressos` consome `GET /api/Evento/meus-ingressos`, que obtém `AlunoId` exclusivamente do `NameIdentifier` do JWT. O detalhe amplia o QR Code, mantém o código textual como alternativa e informa inscrição ativa/cancelada, evento realizado e check-in pendente/confirmado.

## 5. Requisitos Não Funcionais

Os requisitos não funcionais foram organizados por categorias de qualidade: segurança, usabilidade, confiabilidade, desempenho, disponibilidade, manutenibilidade e suporte.

### 5.1. Grupo: Segurança e privacidade

#### 5.1.1. RNF001 - Autenticação com JWT

**Prioridade:** Alta.  
**Descrição:** O sistema deve autenticar usuários por token JWT e validar emissor, audiência, tempo de expiração e assinatura.

#### 5.1.2. RNF002 - Autorização por perfil e instituição

**Prioridade:** Alta.  
**Descrição:** O sistema deve restringir ações conforme role Admin, Secretaria ou Aluno e conforme instituição vinculada.

#### 5.1.3. RNF003 - Senhas protegidas por hash

**Prioridade:** Alta.  
**Descrição:** As senhas devem ser armazenadas usando BCrypt, nunca em texto puro.

#### 5.1.4. RNF004 - Confirmação de e-mail

**Prioridade:** Alta.  
**Descrição:** Contas institucionais devem passar por confirmação de e-mail para reduzir cadastros inválidos.

#### 5.1.5. RNF005 - Configurações sensíveis por ambiente

**Prioridade:** Alta.  
**Descrição:** Credenciais de banco, JWT e e-mail devem ser configuradas por variáveis de ambiente ou user secrets.

### 5.2. Grupo: Usabilidade e interface

#### 5.2.1. RNF006 - Interface web responsiva

**Prioridade:** Média.  
**Descrição:** O portal web deve funcionar em navegadores modernos e adaptar-se a diferentes tamanhos de tela.

#### 5.2.2. RNF007 - Experiência mobile dedicada

**Prioridade:** Alta.  
**Descrição:** O aplicativo mobile deve priorizar fluxo de aluno Fatec, com navegação simples para eventos, inscrição, ingresso e perfil.

#### 5.2.3. RNF008 - Mensagens claras de erro

**Prioridade:** Alta.  
**Descrição:** Erros de validação, login, check-in e confirmação de e-mail devem apresentar mensagens compreensíveis ao usuário.

### 5.3. Grupo: Confiabilidade e integridade

#### 5.3.1. RNF009 - Migrations versionadas

**Prioridade:** Alta.  
**Descrição:** Alterações de banco devem ser controladas por migrations do Entity Framework Core.

#### 5.3.2. RNF010 - Idempotência no check-in

**Prioridade:** Alta.  
**Descrição:** Check-in repetido não deve duplicar presença nem reenviar certificado indevidamente.

#### 5.3.3. RNF011 - Falha de e-mail não desfaz presença

**Prioridade:** Alta.  
**Descrição:** Problemas no envio de e-mail devem ser registrados sem cancelar check-in ou certificado emitido.

#### 5.3.4. RNF018 - Capacidade consistente sob concorrência

**Prioridade:** Alta.  
**Descrição:** Inscrições simultâneas não devem permitir que o total de participações de um evento ultrapasse `Evento.Capacidade`.

#### 5.3.5. RNF019 - Idempotência da certificação distribuída

**Prioridade:** Alta.  
**Descrição:** Chamadas repetidas e concorrentes, inclusive em várias instâncias da API e do worker, devem gerar e enviar automaticamente no máximo um certificado por participação.

#### 5.3.6. RNF020 - PDF válido e portátil

**Prioridade:** Alta.  
**Descrição:** O certificado deve ser um PDF válido, abrir em leitores comuns e ser gerado de forma consistente em Linux, Docker e ambiente de desenvolvimento.

#### 5.3.7. RNF021 - Resultado SMTP incerto não gera retry cego

**Prioridade:** Alta.  
**Descrição:** Quando não for possível determinar se o SMTP aceitou a mensagem, o sistema deve registrar estado incerto e aguardar análise explícita, evitando possível duplicidade.

### 5.4. Grupo: Desempenho e disponibilidade

#### 5.4.1. RNF012 - Consultas filtráveis

**Prioridade:** Média.  
**Descrição:** Listagens de eventos e instituições devem permitir filtros para reduzir volume de dados retornados.

#### 5.4.2. RNF013 - Ambiente containerizado

**Prioridade:** Alta.  
**Descrição:** O sistema deve poder subir backend, banco, web e mobile por Docker Compose em ambiente local.

#### 5.4.3. RNF014 - Healthcheck do banco

**Prioridade:** Média.  
**Descrição:** A API deve aguardar o banco ficar saudável no ambiente Docker antes de iniciar.

### 5.5. Grupo: Manutenibilidade e suporte

#### 5.5.1. RNF015 - Arquitetura em camadas

**Prioridade:** Alta.  
**Descrição:** O backend deve separar API, aplicação, domínio e infraestrutura para facilitar manutenção.

#### 5.5.2. RNF016 - Documentação OpenAPI

**Prioridade:** Média.  
**Descrição:** Endpoints devem ser documentados por OpenAPI/Scalar para facilitar testes e integração.

#### 5.5.3. RNF017 - Testes automatizados

**Prioridade:** Média.  
**Descrição:** Regras críticas devem possuir testes unitários e de validação.

## 6. Regras de Negócio

| Código | Regra de Negócio | Descrição |
| --- | --- | --- |
| RN001 | Mobile exclusivo para alunos Fatec | O aplicativo mobile deve aceitar apenas alunos internos da Fatec. |
| RN002 | Web pública aberta | A área pública web pode ser visualizada por alunos e público geral. |
| RN003 | E-mail institucional | Aluno e usuário secretaria devem usar e-mail institucional quando o fluxo exigir vínculo institucional. |
| RN004 | Instituição ativa para cadastro | Cadastros de aluno e secretaria devem usar instituição existente e ativa. |
| RN005 | Secretaria vinculada à instituição | Usuário Secretaria sempre atua dentro da instituição vinculada ao JWT. |
| RN006 | Admin global sem instituição | UsuarioUnievent representa administrador global e não depende de instituição. |
| RN007 | Evento sempre institucional | Evento novo deve possuir instituição; secretaria usa a instituição do usuário logado. |
| RN008 | Público permitido do evento | Eventos podem ser Público Geral, Todos Alunos Fatec ou Alunos da Instituição. |
| RN009 | Elegibilidade do aluno | Aluno só pode se inscrever em evento compatível com público permitido e instituição quando aplicável. |
| RN010 | Inscrição única | Aluno não deve possuir participação duplicada no mesmo evento. |
| RN011 | Ingresso por participação | Cada participação possui código/QR Code usado na validação do check-in. |
| RN012 | Check-in por evento e código | Check-in deve validar identificador do evento e código de verificação do ingresso. |
| RN013 | Janela de horário | Check-in só deve ser confirmado dentro da janela permitida do evento. |
| RN014 | Certificado após presença | Certificado só é emitido para participação com presença confirmada e certificado configurado. |
| RN015 | Falha de e-mail não cancela presença | Erro no envio de certificado deve ser registrado sem desfazer check-in. |
| RN016 | Status de secretaria | Secretaria pendente, recusada ou bloqueada não pode acessar o portal. |
| RN017 | Isolamento institucional | Secretaria não pode listar, editar ou excluir recursos de outra instituição. |
| RN018 | Capacidade calculada por participações | Vagas disponíveis são calculadas por `Evento.Capacidade - Participacao.Count`, sem armazenar saldo redundante. |
| RN019 | Público Geral externo | Conta pública web é participante externo e não exige e-mail institucional nem vínculo com FATEC. |
| RN020 | Certificação dos dois tipos de participante | Aluno FATEC e Público Geral podem receber certificado pela mesma participação quando estiverem inscritos e com presença confirmada. |
| RN021 | Emissão automática única | A emissão e o envio automáticos devem ocorrer no máximo uma vez para cada participação, mesmo sob repetição ou concorrência. |
| RN022 | PDF oficial anexado | O certificado persistido deve ser um PDF e os mesmos bytes devem ser anexados ao e-mail do participante. |
| RN023 | Separação entre presença e entrega | Falhas de geração ou envio devem preservar o check-in confirmado e ficar registradas para retry seguro ou análise. |
| RN024 | Autorização institucional do check-in | Somente Admin ou Secretaria/operador ativo e autorizado para a instituição do evento pode confirmar presença. |
| RN025 | Download restrito ao titular | Aluno FATEC ou Público Geral só pode consultar e baixar certificados associados às próprias participações. |
| RN026 | Ingressos restritos ao titular | A API de `Meus Ingressos` deve obter o participante do JWT e nunca aceitar `userId` informado pelo cliente. |
| RN027 | Inscrição ativa para check-in | Participação cancelada permanece no histórico, não consome vaga e não pode confirmar presença. |
| RN028 | Mesmo código no QR e no check-in | O QR Code deve codificar somente o `CodigoIngresso` aleatório da participação, sem JWT, CPF, e-mail ou ID interno previsível. |

## 7. Matriz de relacionamento Requisitos Funcionais x Regras de Negócio

| Requisito Funcional | Regras de Negócio Relacionadas |
| --- | --- |
| RF001 - Acessar landing page | RN002 |
| RF002 - Descobrir eventos públicos | RN002, RN008 |
| RF003 - Filtrar eventos por instituição | RN002, RN004 |
| RF004 - Visualizar detalhes de evento público | RN002, RN008 |
| RF005 - Cadastrar admin inicial | RN006 |
| RF006 - Realizar login web | RN005, RN006, RN016 |
| RF007 - Solicitar cadastro de usuário secretaria | RN003, RN004, RN005 |
| RF008 - Aprovar, bloquear ou recusar secretaria | RN006, RN016 |
| RF009 - Confirmar e-mail de conta | RN003 |
| RF010 - Cadastrar aluno institucional no mobile | RN001, RN003, RN004 |
| RF011 - Realizar login mobile | RN001, RN003 |
| RF012 - Gerenciar perfil do aluno | RN001 |
| RF013 - Gerenciar instituições | RN006, RN004 |
| RF014 - Listar instituições ativas para seleção | RN004 |
| RF015 - Gerenciar eventos da instituição | RN005, RN017 |
| RF016 - Cadastrar evento | RN005, RN007, RN008 |
| RF017 - Editar evento | RN005, RN017 |
| RF018 - Excluir evento | RN005, RN017 |
| RF019 - Definir público permitido do evento | RN008, RN009 |
| RF020 - Gerenciar responsáveis por eventos | RN005, RN017 |
| RF021 - Consultar eventos disponíveis no mobile | RN001, RN008, RN009 |
| RF022 - Visualizar detalhes de evento no mobile | RN001, RN008 |
| RF023 - Favoritar evento | RN001 |
| RF024 - Inscrever-se em evento | RN009, RN010, RN011 |
| RF025 - Visualizar meus eventos | RN010, RN011 |
| RF026 - Visualizar ingresso e QR Code | RN011, RN012 |
| RF027 - Configurar preferências | RN001 |
| RF028 - Consultar eventos recomendados | RN001, RN008, RN009 |
| RF029 - Validar check-in | RN011, RN012, RN013, RN017, RN024 |
| RF030 - Verificar janela de horário do check-in | RN013 |
| RF031 - Emitir certificado após presença | RN014, RN020, RN021, RN022, RN023 |
| RF032 - Enviar certificado por e-mail | RN014, RN015, RN020, RN021, RN022, RN023 |
| RF033 - Executar automações de eventos | RN014, RN015, RN021, RN023 |
| RF039 - Consultar e baixar certificado próprio | RN014, RN020, RN022, RN025 |
| RF034 - Consultar dashboard administrativo | RN006 |
| RF035 - Consultar dashboard institucional | RN005, RN017 |
| RF036 - Consultar dashboard individual do evento | RN005, RN017 |
| RF037 - Cadastrar e autenticar Público Geral | RN019 |
| RF038 - Inscrever Público Geral em evento público | RN008, RN009, RN010, RN018, RN019 |
| RF040 - Consultar Meus Ingressos e apresentar QR Code | RN011, RN012, RN024, RN026, RN027, RN028 |
