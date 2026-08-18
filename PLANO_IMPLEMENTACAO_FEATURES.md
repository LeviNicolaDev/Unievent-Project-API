# Plano de implementação — evolução do Unievent

## 1. Objetivo

Evoluir o Unievent para atender público interno e externo, recomendar e notificar eventos por interesse e proximidade, oferecer indicadores reais à administração, validar presença com QR Code e localização e controlar eventos públicos ou restritos.

Este plano foi elaborado a partir do estado atual dos três clientes/camadas do repositório:

- API ASP.NET Core (`Unievent.Api`, `Unievent.Application`, `Unievent.Domain` e `Unievent.Infra`);
- portal administrativo React/Vite (`Unievent-Project-Web-Frontend`);
- aplicativo React Native/Expo (`Unievent-Project-Mobile/biased-orange-donut`);
- testes unitários existentes em `Unievent.Tests`.

### 1.1 Status da implementação iniciada em 17/08/2026

Legenda: ✅ implementado no código; 🟡 implementação parcial; ⛔ pendente/bloqueado.

| Frente | Status | Entrega realizada / pendência |
|---|---:|---|
| Público externo | 🟡 | `Aluno` agora diferencia `Interno` e `Externo`; cadastro mobile envia o tipo e vínculo opcional. A unificação definitiva em `Usuario`, confirmação de e-mail e validação real do vínculo institucional continuam pendentes. |
| Evento público/restrito | 🟡 | Domínio, DTOs e formulário web receberam `Visibilidade` e `PublicoPermitido`; inscrição aplica elegibilidade e o catálogo público filtra eventos privados/incompatíveis usando claim do participante. Convites para eventos privados continuam pendentes. |
| Capacidade/período | 🟡 | Serviço bloqueia evento lotado e fora da janela; a gravação final usa transação `Serializable` para proteger a última vaga. Ainda falta lista de espera e teste de carga/concorrência. |
| QR + localização | 🟡 | Ingresso aleatório por participação, endpoint de ingresso e check-in autorizado com janela, precisão, raio, Haversine e idempotência foram adicionados. O auto-check-in foi removido da tela mobile e o portal ganhou operação manual por código + geolocalização. Ainda faltam assinatura/rotação do token, leitura por câmera, exceção auditada e isolamento por instituição. |
| Preferências/notificações | 🟡 | Preferências e consentimento de localização passaram a ter entidade e endpoints persistentes; os toggles mobile agora carregam e salvam as opções no servidor. Envio push, tokens de dispositivo, outbox/worker e central de notificações dependem de provedor/credenciais e não foram implementados. |
| Recomendações | 🟡 | Endpoint autenticado implementa ranking inicial explicável por categoria, proximidade e data, respeitando opt-out e elegibilidade. Ainda faltam favoritos persistentes, job/notificação proativa, paginação avançada e métricas de conversão. |
| Dashboard | 🟡 | API de resumo/por evento e cards web com eventos, inscrições, presença e público foram implementados. Filtros avançados, gráficos, exportação e escopo institucional permanecem pendentes. |
| Migração de banco | ⛔ | Alterações foram mapeadas no `AppDbContext`, mas a migration não pôde ser gerada porque o SDK .NET não está instalado no ambiente. Não implantar a API antes de gerar/revisar a migration. |
| Validação | ⛔ | Foram adicionados testes unitários de check-in, precisão e elegibilidade, e `git diff --check` passou. Builds/testes não rodaram: não há SDK .NET nem Node/npm instalados nesta máquina. |

Arquivos centrais alterados: entidades `Aluno`, `Evento`, `Participacao` e `PreferenciaNotificacao`; enums de público; DTOs/serviços/repositório de participação; claims JWT; `EventoController`; novos `DashboardController`, `PreferenciasController` e `RecomendacoesController`; cadastro/ingresso mobile; formulário, dashboard e operação de check-in web.

> **Atenção:** o estado atual é uma implementação intermediária, não uma release pronta para produção. Os itens ⛔ e as validações da seção 16 são bloqueadores de implantação.

## 2. Diagnóstico do estado atual

### 2.1 O que já existe e pode ser aproveitado

- autenticação JWT para `Admin`, `Secretaria` e `Aluno`;
- cadastro de aluno, instituição, responsável, evento, endereço e certificado;
- inscrição de aluno em evento por meio de `Participacao`;
- estado inicial de presença e emissão de certificado na entidade `Participacao`;
- filtros por categoria, busca e favoritos no aplicativo;
- tela de configurações com opções visuais para lembretes, certificados e sugestões;
- QR Code exibido no ingresso mobile;
- portal administrativo com rotas de eventos, responsáveis e certificados;
- testes unitários de serviços e validadores.

### 2.2 Lacunas e riscos que precisam ser tratados primeiro

1. **Modelo de usuário limitado:** a participação referencia diretamente `Aluno`; não existe usuário externo. O e-mail institucional aparece no SQL, mas não é uma regra consistente do domínio.
2. **Banco divergente do EF Core:** `bdunievent.sql` contém campos e relações ausentes das entidades (instituição do aluno, endereço do evento, hora do evento e tabela instituição-evento), enquanto não há migrações versionadas no repositório.
3. **Presença insegura:** o participante confirma a própria presença; o QR contém texto previsível (`UNIEVENT-{evento}-{titulo}`), sem assinatura, expiração ou vínculo seguro com a inscrição; não há leitura administrativa nem localização.
4. **Capacidade não aplicada:** a inscrição não verifica lotação, período de inscrição, cancelamento ou concorrência.
5. **Dashboard não analítico:** a tela atual apresenta atalhos estáticos e não consome métricas.
6. **Notificações não implementadas:** os toggles mobile não são persistidos no servidor; não há token de dispositivo, consentimento de localização, motor de recomendação ou fila de envio.
7. **Favoritos e parte do estado de participação ficam no dispositivo:** isso impede recomendações e métricas confiáveis entre dispositivos.
8. **Autorização incompleta:** há endpoints de leitura/alteração de aluno sem proteção e o portal valida basicamente a existência de token, sem controle fino por papel e instituição.
9. **Escopo institucional ausente:** as entidades administrativas e os eventos não estão consistentemente isolados por `InstituicaoId`, criando risco de acesso cruzado.
10. **Observabilidade e testes insuficientes para os novos fluxos:** não há testes de integração/E2E para inscrição, concorrência de vagas, QR, geolocalização ou notificações.

## 3. Decisões de negócio propostas

Estas decisões eliminam ambiguidades das sugestões e devem ser validadas com o responsável pelo produto antes do desenvolvimento.

### 3.1 Perfis de acesso

Adotar uma identidade única `Usuario` e diferenciar o vínculo acadêmico por tipo:

- `Administrador`: administra toda a plataforma, se esse papel global for realmente necessário;
- `Secretaria`: administra somente dados da própria instituição;
- `ParticipanteInterno`: possui vínculo validado com uma instituição (aluno atual);
- `ParticipanteExterno`: não possui vínculo acadêmico, mas pode criar conta com e-mail comum;
- `OperadorCheckIn`: lê ingressos e valida presença, sem permissão para alterar eventos ou usuários.

Preferência técnica: substituir gradualmente `Aluno` como identidade por `Usuario`, mantendo uma tabela opcional `VinculoInstitucional`. Isso evita duplicar autenticação, inscrição, preferências e notificações para interno e externo.

### 3.2 Cadastro de público externo

- Nome, e-mail, senha e data de nascimento são obrigatórios; foto é opcional.
- E-mail deve ser único e confirmado antes de inscrição.
- Usuário interno informa instituição e identificador acadêmico (RA/matrícula); o vínculo deve ser confirmado por domínio institucional, código enviado ao e-mail ou aprovação da secretaria.
- Usuário externo nasce sem instituição e com tipo `ParticipanteExterno`.
- A classificação interno/externo não pode depender apenas do texto do e-mail no front-end.
- A migração deve converter todos os `Aluno` existentes em usuários internos sem invalidar seus IDs, inscrições e tokens durante a transição.

### 3.3 Visibilidade e elegibilidade dos eventos

Separar dois conceitos:

- `Visibilidade`: `Publico` (aparece para qualquer pessoa) ou `Privado` (somente usuários elegíveis conseguem descobrir/detalhar);
- `PublicoPermitido`: `Todos`, `SomenteInternos`, `SomenteExternos` ou `ListaDeConvidados`.

Regras:

- evento público + `Todos`: visível e inscrevível por internos e externos;
- evento público + `SomenteInternos`: pode aparecer externamente, mas deve indicar restrição e bloquear inscrição externa;
- evento privado: somente elegíveis, convidados ou administradores da instituição podem consultá-lo;
- convite deve usar código/token aleatório com expiração e uso auditável, nunca ID sequencial;
- mudança de público após inscrições exige confirmação administrativa e tratamento explícito dos já inscritos;
- certificados dependem de presença válida, não do tipo de participante.

### 3.4 Inscrição

- Uma participação única por `(UsuarioId, EventoId)`.
- Só eventos publicados, futuros, dentro do período de inscrição e elegíveis aceitam inscrição.
- A vaga deve ser reservada atomicamente no banco; duas últimas inscrições simultâneas não podem exceder a capacidade.
- Ao lotar, o sistema pode oferecer lista de espera; cancelamento promove o próximo usuário e gera notificação.
- Estados sugeridos: `Inscrito`, `ListaEspera`, `Cancelado`, `CheckInRealizado`, `Ausente`.

### 3.5 Notificações e recomendação

- Notificações são opt-in e separadas por tipo: lembretes de inscrições, certificados, alterações/cancelamentos e recomendações.
- Localização precisa de consentimento específico, revogável, e não deve ser coletada continuamente para recomendação.
- Preferir cidade/CEP ou uma posição aproximada armazenada com baixa precisão; localização exata deve ser usada somente no momento do check-in.
- O primeiro motor de recomendação será determinístico e explicável, sem machine learning: categoria/favoritos + distância + data + vagas + elegibilidade.
- Eventos privados ou incompatíveis com o público do usuário nunca entram em recomendações.
- Evitar spam com frequência máxima configurável, deduplicação e registro de envio.

Pontuação inicial sugerida:

`score = interesseCategoria(0..40) + favoritoRelacionado(0..20) + proximidade(0..25) + recencia(0..10) + disponibilidade(0..5)`.

### 3.6 Check-in por QR Code e localização

- Cada `Participacao` confirmada recebe um ingresso com identificador aleatório e QR assinado pelo servidor.
- O QR deve conter apenas token opaco ou payload assinado; não deve expor dados pessoais.
- A leitura é feita no aplicativo/tela de `OperadorCheckIn`, nunca confirmada por botão do próprio participante.
- O servidor valida: assinatura, expiração, evento, inscrição ativa, janela de check-in, ausência de uso anterior e permissão do operador.
- O dispositivo leitor envia latitude, longitude, precisão e horário. O servidor calcula a distância até o evento.
- Check-in automático requer QR válido **e** distância dentro do raio configurado (sugestão inicial: 150 m, ajustável por evento) **e** precisão aceitável (sugestão: até 50 m).
- Localização indisponível ou imprecisa não confirma automaticamente. Um operador autorizado pode executar exceção com motivo obrigatório; toda exceção é auditada.
- O endpoint deve ser idempotente: nova leitura do mesmo ingresso devolve o resultado existente, sem duplicar presença.
- Guardar a distância calculada e a precisão, evitando retenção desnecessária da coordenada bruta; definir prazo de retenção conforme política LGPD.

## 4. Arquitetura e modelo de dados alvo

### 4.1 Entidades novas ou alteradas

| Entidade | Alterações principais |
|---|---|
| `Usuario` | Id, nome, e-mail normalizado único, hash de senha, tipo, status, confirmação de e-mail, foto opcional, nascimento, timestamps |
| `VinculoInstitucional` | UsuarioId, InstituicaoId, matrícula/RA, tipo, status de validação, data de validação |
| `Evento` | InstituicaoId, EnderecoId, status, visibilidade, público permitido, início/fim, período de inscrição, capacidade, raio de check-in, coordenadas do local, publicação |
| `Participacao` | UsuarioId, EventoId, status, data de inscrição/cancelamento/check-in e origem |
| `Ingresso` | ParticipacaoId único, identificador público aleatório, hash/JTI do token, validade, revogação, versão |
| `CheckIn` | ParticipacaoId único, operador, instante do servidor, distância, precisão, resultado, motivo de exceção, metadados mínimos de auditoria |
| `PreferenciaUsuario` | categorias, cidade/posição aproximada, raio, tipos de notificação e consentimentos/versionamento |
| `DispositivoPush` | UsuarioId, plataforma, token protegido, ativo, último uso |
| `Favorito` | UsuarioId + EventoId, criado em; substitui persistência exclusivamente local |
| `Notificacao` | usuário, tipo, título, corpo/dados, canal, estado, agendamento, envio, leitura e chave de deduplicação |
| `ConviteEvento` | evento, token em hash, destinatário opcional, validade, limite/uso |
| `Auditoria` | ator, instituição, ação, recurso, resultado, data e metadados sem segredo |

### 4.2 Índices e restrições obrigatórios

- índices únicos em e-mail normalizado, `(UsuarioId, EventoId)` de participação, `ParticipacaoId` de ingresso/check-in e token de dispositivo;
- índices para busca de eventos por status, visibilidade, instituição, categoria, data e localização;
- `rowversion`/controle otimista ou transação serializável na reserva de vagas;
- FKs explícitas no EF Core e comportamento de exclusão definido;
- datas em UTC no banco, com conversão para o fuso do evento na apresentação;
- coordenadas com tipo espacial do SQL Server (`geography`) quando possível; caso contrário, latitude/longitude validadas e cálculo geodésico no servidor.

### 4.3 Migração segura

1. Criar migrações EF Core como fonte de verdade e reconciliar o schema com `bdunievent.sql`.
2. Criar novas tabelas/campos inicialmente opcionais.
3. Migrar alunos e suas participações para `Usuario`/`UsuarioId` com tabela de correspondência.
4. Preencher `InstituicaoId` e `EnderecoId` de eventos; bloquear publicação dos registros incompletos.
5. Executar leitura compatível durante uma versão; depois remover colunas/relações antigas.
6. Fazer backup, ensaio em cópia da base, validação de contagens e plano de rollback antes da produção.

## 5. Contratos de API planejados

Rotas representativas; nomes finais devem seguir uma convenção única em português ou inglês.

### Identidade e preferências

- `POST /api/auth/cadastro` — cadastra participante interno ou externo.
- `POST /api/auth/confirmar-email` — confirma e-mail por token de uso único.
- `POST /api/vinculos-institucionais` — solicita vínculo interno.
- `GET/PUT /api/me/preferencias` — consulta/altera categorias, raio, local aproximado e consentimentos.
- `POST/DELETE /api/me/dispositivos` — registra ou revoga token push.

### Eventos, favoritos e inscrições

- `GET /api/eventos` — paginação e filtros; aplica visibilidade/elegibilidade no servidor.
- `GET /api/eventos/recomendados` — retorna score e motivo resumido da recomendação.
- `POST/DELETE /api/eventos/{id}/favorito` — sincroniza favoritos.
- `POST /api/eventos/{id}/inscricoes` — inscrição transacional.
- `DELETE /api/eventos/{id}/inscricoes/me` — cancelamento.
- `GET /api/me/participacoes` — fonte de verdade dos ingressos e estados.
- CRUD administrativo de evento recebe visibilidade, público, endereço/coordenadas, janela e raio de check-in.

### Check-in

- `GET /api/participacoes/{id}/ingresso` — devolve QR assinado/renovável ao dono.
- `POST /api/check-ins/validar` — operador envia token do QR e leitura de localização.
- `POST /api/check-ins/excecao` — confirmação manual com motivo e autorização adicional.
- `GET /api/eventos/{id}/check-ins` — lista/pagina presença para operação e dashboard.

### Dashboard

- `GET /api/admin/dashboard/resumo?inicio=&fim=&instituicaoId=` — cards agregados.
- `GET /api/admin/dashboard/eventos` — inscrições, ocupação, check-ins e ausências por evento.
- `GET /api/admin/dashboard/publico` — internos versus externos.
- `GET /api/admin/dashboard/categorias` — interesse/inscrição por categoria.
- `GET /api/admin/dashboard/notificacoes` — entregas, falhas, aberturas quando disponíveis.
- Exportação CSV assíncrona ou limitada por período, sempre respeitando escopo institucional.

Todos os endpoints administrativos devem validar papel **e** instituição do recurso. DTOs públicos não devem expor hashes, tokens, localização bruta ou dados pessoais desnecessários.

## 6. Plano por fases

### Fase 0 — saneamento e fundação

- escolher EF Core migrations como fonte única do schema;
- reconciliar entidades, contexto e SQL;
- corrigir encoding dos arquivos/mensagens para UTF-8;
- centralizar tratamento de erros com `ProblemDetails` e códigos estáveis;
- revisar proteção de endpoints de aluno, eventos, certificados e instituição;
- adicionar `InstituicaoId` aos recursos administrativos e claims necessários ao JWT;
- substituir senhas por hash forte com salt e preparar refresh/revogação de sessão;
- criar testes de integração com SQL Server em container.

**Saída:** base consistente, autorização multi-instituição e pipeline de migração/testes confiável.

### Fase 1 — público externo, visibilidade e inscrição correta

- implementar identidade `Usuario` e vínculo institucional;
- criar cadastro/ativação de externos no mobile;
- adaptar login, perfil e contexto de autenticação sem chamar todo participante de “aluno”;
- adicionar visibilidade/público/status e local completo ao evento;
- atualizar formulário, preview e listagem administrativa;
- filtrar catálogo e detalhes conforme elegibilidade;
- tornar inscrição transacional, aplicar capacidade, cancelamento e opcionalmente lista de espera;
- migrar favoritos e participações locais para APIs, mantendo cache offline apenas como cache;
- migrar registros existentes.

**Critérios de aceite:** externo confirmado acessa eventos permitidos; restrito não vaza em busca/recomendação; interno mantém dados e inscrições; lotação nunca é excedida sob concorrência.

### Fase 2 — ingresso seguro e check-in geográfico

- criar `Ingresso`, serviço de token assinado, rotação/revogação e QR não previsível;
- alterar `TicketQrScreen` para apenas apresentar ingresso retornado pela API;
- criar modo/tela de operador com câmera (`expo-camera`) e permissão de localização (`expo-location`);
- implementar cálculo server-side, janela, raio, precisão, idempotência e exceção auditada;
- remover o botão de auto-confirmação do participante;
- liberar certificado somente após check-in válido;
- oferecer lista operacional de presentes/pendentes e busca manual segura.

**Critérios de aceite:** screenshot/copiar payload fora da janela/local não confirma; participante não confirma a si próprio; releitura é idempotente; exceção registra operador e motivo; certificado sem presença é rejeitado.

### Fase 3 — notificações e recomendações

- persistir preferências, favoritos, consentimentos e tokens de dispositivo;
- integrar `expo-notifications` no aplicativo e um provedor de envio encapsulado por interface na API;
- criar fila/worker (inicialmente `BackgroundService` + outbox; migrar para broker se o volume exigir);
- disparar notificações transacionais: alteração, cancelamento, promoção da espera, lembrete e certificado;
- implementar job periódico de recomendação determinística por categoria/local/data/elegibilidade;
- criar central de notificações no app com lida/não lida e deep link para o evento;
- adicionar unsubscribe, limite de frequência, retry com backoff, dead-letter e métricas.

**Critérios de aceite:** opt-out é respeitado; evento restrito nunca é enviado ao inelegível; envio não duplica; usuário entende por que recebeu a sugestão; ausência de localização não bloqueia recomendações por conteúdo.

### Fase 4 — dashboard administrativo analítico

- substituir/complementar atalhos atuais por cards: eventos publicados, inscrições, taxa de ocupação, check-ins, comparecimento e externos/internos;
- gráficos por período/categoria e ranking de eventos;
- filtros por data, instituição, evento, categoria e público;
- visão operacional do evento em andamento e falhas de check-in;
- exportação CSV com colunas mínimas e controle de permissão;
- cache curto dos agregados e consultas projetadas (`AsNoTracking`), evitando carregar entidades completas;
- registrar acesso a relatórios sensíveis.

**Critérios de aceite:** números conciliam com consultas de referência; secretaria só vê sua instituição; filtros são refletidos em todos os componentes; estados de loading/vazio/erro funcionam; exportação respeita os mesmos filtros.

### Fase 5 — endurecimento e lançamento

- testes E2E mobile/API/web dos cinco fluxos;
- teste de carga para catálogo, inscrição concorrente, check-in na entrada e dashboard;
- revisão LGPD, acessibilidade, segurança e retenção;
- telemetria com correlação, métricas de fila e alertas;
- feature flags por instituição e rollout gradual;
- documentação operacional, treinamento de operadores e plano de contingência para check-in sem internet.

## 7. Alterações esperadas por projeto

### Backend

- `Unievent.Domain`: novas entidades, estados e regras invariantes; evitar regras críticas somente em controller/UI.
- `Unievent.Application`: casos de uso separados para inscrição, elegibilidade, recomendação, ingresso, check-in, notificações e métricas.
- `Unievent.Infra`: mapeamentos EF explícitos, migrations, queries agregadas, outbox, push provider, cálculo espacial e auditoria.
- `Unievent.Api`: controllers finos, policies por recurso/instituição, DTOs versionáveis, rate limiting em autenticação/convites/check-in e documentação OpenAPI.

### Portal web administrativo

- dashboard consumindo agregados reais;
- formulário de evento com visibilidade, elegibilidade, endereço/mapa, datas, janela e raio;
- gestão de convidados e lista de espera;
- leitor ou painel de check-in para dispositivo compatível, além de operação manual autorizada;
- rota e componentes protegidos por papel e instituição, não apenas pela existência de token.

### Aplicativo mobile

- cadastro interno/externo e confirmação de e-mail;
- preferências sincronizadas;
- catálogo filtrado por elegibilidade e recomendações explicadas;
- favoritos/inscrições vindos do servidor;
- central e permissões de notificações;
- QR de ingresso retornado pelo backend;
- modo de operador separado, com câmera e geolocalização, disponível apenas para papel autorizado.

## 8. Estratégia de testes

### Unidade

- matriz de elegibilidade por visibilidade/público/perfil;
- score de recomendação, limites e exclusões;
- geração/validação/expiração/revogação do QR;
- distância geográfica, precisão, raio e janela;
- transições válidas de participação e emissão de certificado;
- agregações do dashboard.

### Integração

- migrações em banco vazio e atualização de base legada;
- isolamento entre instituições;
- inscrição concorrente na última vaga;
- idempotência de check-in e outbox/notificação;
- filtros do catálogo e recomendações sem vazamento de evento privado.

### E2E

- externo: cadastro → confirmação → descoberta → inscrição → ingresso;
- interno: vínculo validado → evento restrito → inscrição;
- operador: login → leitura → localização → presença;
- administrador: criação de evento → acompanhamento no dashboard → exportação;
- cenários negados: QR adulterado/expirado, fora do raio, usuário inelegível e operador de outra instituição.

## 9. Segurança, privacidade e LGPD

- coletar localização somente com finalidade e consentimento claros;
- manter alternativa operacional para quem negar permissão, sem confirmação automática;
- minimizar precisão e retenção; documentar base legal e prazo de descarte;
- criptografar transporte, proteger tokens push e armazenar somente hash de tokens de convite/ingresso quando aplicável;
- não registrar JWT, QR completo, senha, token push ou coordenadas exatas em logs;
- rate limit e auditoria para login, convite, emissão e validação de ingresso;
- autorização sempre no servidor e vinculada à instituição;
- termos e política devem explicar recomendações automatizadas e permitir desativá-las;
- permitir exportação/correção/exclusão ou anonimização dos dados conforme obrigações legais e retenção de auditoria.

## 10. Observabilidade e indicadores de sucesso

### Técnicos

- taxa/latência de inscrição e check-in;
- contenção/erros na última vaga;
- QR inválido, expirado, duplicado e fora do raio;
- fila de notificações, entrega, retry e falha definitiva;
- latência e taxa de erro das consultas do dashboard.

### Produto

- conversão de cadastro externo e confirmação de e-mail;
- inscrições e comparecimento por público interno/externo;
- ocupação média e no-show por evento;
- conversão de recomendação em visualização/inscrição;
- opt-in/opt-out de notificações;
- tempo médio de check-in e taxa de exceção manual.

## 11. Dependências e escolhas técnicas

- Mobile: `expo-camera`, `expo-location`, `expo-notifications` e configuração EAS para credenciais push.
- API: abstração `INotificationSender`, worker/outbox, biblioteca oficial/validada para token QR e suporte espacial do SQL Server.
- Web: biblioteca de gráficos acessível e leve; escolher após protótipo e teste de bundle.
- Operação: credenciais push, HTTPS público, política de retenção, mapas/geocodificação caso o endereço não seja convertido manualmente em coordenadas.

Não acoplar as regras a um fornecedor: push, mapas e armazenamento de imagem devem ficar atrás de interfaces.

## 12. Backlog priorizado e estimativa relativa

| Ordem | Épico | Tamanho relativo | Dependência |
|---:|---|---|---|
| 1 | Saneamento do schema, segurança e multi-instituição | XL | nenhuma |
| 2 | Identidade única e público externo | XL | 1 |
| 3 | Visibilidade/elegibilidade e inscrição transacional | L | 1–2 |
| 4 | Ingresso seguro e check-in por QR/localização | XL | 1–3 |
| 5 | Preferências, favoritos e consentimentos no servidor | M | 2–3 |
| 6 | Notificações transacionais | L | 5 |
| 7 | Recomendações por conteúdo/local | L | 5–6 |
| 8 | Dashboard analítico e exportação | L | 1–4 |
| 9 | Hardening, carga, LGPD e rollout | L | todos |

Estimativas de calendário devem ser feitas somente após refinamento com equipe, definição do provedor de push/mapas, estratégia de migração e disponibilidade de UX/QA. Os tamanhos acima servem para dependência e priorização, não como prazo.

## 13. Definition of Done global

Uma história só está concluída quando:

- regra de negócio está no backend e documentada;
- autorização e isolamento institucional foram testados;
- migration e rollback foram revisados;
- contrato OpenAPI e clientes web/mobile foram atualizados;
- testes unitários e de integração cobrem sucesso, negação e concorrência quando aplicável;
- interface possui loading, vazio, erro, acessibilidade e textos em UTF-8;
- logs/métricas não expõem dados sensíveis;
- critérios de LGPD e retenção foram atendidos;
- documentação de operação e suporte foi atualizada.

## 14. Questões que exigem decisão do produto

1. Eventos “fechados” ficam invisíveis ou visíveis com bloqueio de inscrição? O plano suporta ambos por separar visibilidade de elegibilidade.
2. Como comprovar vínculo interno: domínio de e-mail, integração acadêmica, importação ou aprovação manual?
3. Haverá lista de espera já no primeiro release?
4. Qual o raio padrão de check-in e quem pode alterá-lo?
5. O sistema precisa operar check-in offline? Se sim, será necessário pacote de ingressos/allowlist com validade curta e sincronização de conflitos.
6. Qual provedor de push/mapas será usado e qual orçamento esperado?
7. `Admin` é global ou também pertence a uma instituição?
8. Quais métricas e dados pessoais podem ser exportados por cada papel?

## 15. Sequência recomendada de entrega de valor

Entregar primeiro público externo + eventos públicos/restritos sobre uma base corrigida; em seguida fechar o risco de fraude com ingresso/check-in seguro; depois ativar notificações e recomendações; por último ampliar o dashboard com dados já confiáveis. Essa ordem evita que o dashboard e as recomendações sejam alimentados por favoritos, inscrições e presenças que hoje ainda vivem parcialmente no dispositivo ou podem ser confirmadas sem validação.

## 16. Plano de implantação e rollback

### 16.1 Pré-requisitos obrigatórios

1. Instalar .NET SDK 10 compatível com os `TargetFramework` atuais e Node.js LTS com npm.
2. Disponibilizar SQL Server de homologação a partir de backup anonimizado da produção.
3. Definir URLs HTTPS da API, web e deep links mobile.
4. Configurar segredos fora do repositório: conexão SQL, chave/issuer/audience JWT, SMTP e futuramente credenciais Expo Push/mapas.
5. Definir quem é `Admin` global, como `Secretaria` se relaciona à instituição e como operadores são provisionados.
6. Aprovar política LGPD para localização, retenção de check-in e recomendações.

### 16.2 Geração e ensaio da migration — bloqueador atual

Após instalar o SDK:

```powershell
dotnet restore Unievent-Project.slnx
dotnet ef migrations add PublicoCheckInPreferencias --project Unievent.Infra --startup-project Unievent.Api
dotnet ef migrations script --idempotent --project Unievent.Infra --startup-project Unievent.Api --output artifacts/PublicoCheckInPreferencias.sql
```

Antes de aplicar, revisar especialmente:

- nomes reais das tabelas/colunas diante da divergência entre `bdunievent.sql` e EF;
- criação e preenchimento de `CodigoIngresso` único para participações antigas;
- defaults de `TipoParticipante`, `Visibilidade`, `PublicoPermitido`, raio e tolerância;
- FKs opcionais de instituição/endereço e comportamento `Restrict`;
- índice único `(AlunoId, EventoId)`: procurar/remover duplicidades mediante relatório aprovado, nunca silenciosamente;
- tamanho das colunas de enums e código do ingresso;
- rollback gerado e tempo de lock dos índices.

Executar o script primeiro em cópia de homologação, comparar contagens antes/depois e validar pelo menos uma participação histórica, um certificado e um evento de cada categoria.

### 16.3 Correções obrigatórias antes da release candidata

- aplicar autorização em todos os endpoints de aluno e leituras administrativas;
- adicionar `InstituicaoId` ao usuário administrativo/JWT e filtrar Dashboard, check-in e CRUD por instituição;
- impedir consulta de evento privado por usuário inelegível, inclusive `GET /api/Evento` e `GET /api/Evento/{id}`;
- validar por teste de concorrência a reserva atômica implementada com isolamento `Serializable`;
- armazenar somente hash do token do ingresso ou usar payload assinado com expiração/rotação;
- concluir provisionamento do papel de operador e leitura por câmera; o portal já oferece fallback manual com geolocalização do navegador;
- corrigir atualização do mobile após check-in realizado em outro dispositivo;
- validar existência de `InstituicaoId` informado no cadastro interno;
- implementar confirmação de e-mail antes de permitir inscrição;
- versionar os contratos que mudaram e normalizar respostas com `ProblemDetails`.

### 16.4 Pipeline de validação

```powershell
dotnet build Unievent-Project.slnx --configuration Release
dotnet test Unievent-Project.slnx --configuration Release --no-build
```

```powershell
Set-Location Unievent-Project-Web-Frontend
npm ci
npm run build
```

```powershell
Set-Location Unievent-Project-Mobile/biased-orange-donut
npm ci
npx expo-doctor
npx expo export --platform web
```

Além do pipeline, executar testes de integração com SQL Server e os E2E definidos na seção 8. A release não deve avançar com erro de compilação, migration pendente ou cenário de autorização falhando.

### 16.5 Ordem de publicação

1. Ativar modo de manutenção apenas se o ensaio indicar locks relevantes.
2. Gerar backup verificado e registrar versão/horário.
3. Aplicar a migration idempotente.
4. Publicar API compatível com o cliente antigo; executar smoke tests de login, catálogo, inscrição, ingresso, dashboard e preferências.
5. Publicar portal web e invalidar cache/CDN.
6. Distribuir mobile primeiro em canal interno/preview; depois rollout gradual nas lojas.
7. Ativar por feature flag, nesta ordem: tipo externo → público de evento → novo ingresso → check-in por operador → dashboard.
8. Manter notificações e recomendações desligadas até que outbox, provedor e opt-in estejam completos.
9. Monitorar por pelo menos um ciclo real de evento antes de ampliar o rollout.

### 16.6 Smoke tests de produção

- login administrativo e de participante existente;
- cadastro de externo e bloqueio de interno com domínio/vínculo inválido;
- evento `Todos`, `SomenteInternos` e `SomenteExternos`;
- bloqueio de lotação e janela de inscrição;
- emissão do ingresso somente para o próprio inscrito;
- check-in válido, duplicado, fora do raio, localização imprecisa e fora da janela;
- dashboard conciliado com consulta SQL de referência;
- preferências salvas e localização removida ao retirar consentimento;
- impossibilidade de acesso cruzado entre instituições (após implementar o bloqueador).

### 16.7 Monitoramento pós-release

- erros HTTP por endpoint e papel;
- latência/erros de inscrição e check-in;
- divergência entre capacidade e inscrições;
- percentual de QR inválido/duplicado/fora do raio;
- consultas lentas e uso de CPU/IO do dashboard;
- falhas de autenticação e tentativas de acesso entre instituições;
- crashes do mobile na tela do ingresso e, futuramente, câmera/localização.

### 16.8 Rollback

- Desativar primeiro as feature flags, preservando dados novos.
- Reverter web/mobile para a versão anterior sem apagar colunas.
- Reverter API somente se a versão anterior tolerar o schema expandido.
- Preferir roll-forward para erros de aplicação. Usar o `Down`/script reverso apenas se ele não apagar dados já coletados.
- Se houver corrupção ou migration incompleta, interromper escrita, guardar logs, restaurar o backup em instância separada e validar antes de trocar a conexão.
- Nunca remover participações/check-ins criados na janela sem exportação e aprovação do responsável.

### 16.9 Critério de go/no-go

**Go** somente com migration ensaiada, builds verdes, isolamento institucional validado, inscrição concorrente testada, QR não reutilizável fora das regras, política LGPD aprovada e rollback exercitado. Caso qualquer item falhe, manter as flags desligadas e registrar evidência/correção nesta seção.
