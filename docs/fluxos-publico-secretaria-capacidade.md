# Fluxos: Público Geral, Secretaria E Capacidade

## Fluxo Público Geral

```plantuml
@startuml
start
:Cadastro na web pública;
:Login como Público Geral;
:Visualização dos eventos;
:Detalhes do evento;
if (Evento permite Público Geral?) then (não)
  :Inscrição bloqueada;
  stop
else (sim)
endif
if (Há vagas disponíveis?) then (não)
  :Evento lotado;
  stop
else (sim)
endif
if (Usuário já está inscrito?) then (sim)
  :Exibir inscrição existente;
  stop
else (não)
endif
:Registrar Participacao;
:Gerar ingresso;
:Disponibilizar em Meus Ingressos;
:Exibir QR Code com o CodigoIngresso;
:Check-in por operador autorizado;
:Acompanhar estado da certificação;
:Baixar PDF próprio quando disponível;
stop
@enduml
```

O Público Geral é armazenado como `Aluno` com `TipoParticipante.Externo`. Isso permite reutilizar `Participacao`, ingresso, QR Code, check-in e certificados sem criar um fluxo paralelo de inscrição.

## Fluxo De Meus Ingressos E Check-in Web

```plantuml
@startuml
start
:Público Geral acessa a Web;
if (Autenticado?) then (não)
  :Direcionar ao login;
  stop
else (sim)
endif
:Abrir Meus Ingressos;
:API obtém AlunoId do JWT;
:Consultar participações do titular;
:Separar próximos eventos e histórico;
:Selecionar ingresso;
if (Inscrição ativa?) then (não)
  :Informar inscrição cancelada;
  stop
else (sim)
endif
:Renderizar QR Code com CodigoIngresso;
:Secretaria lê o QR Code;
:Backend valida operador e instituição;
:Backend valida código, evento, inscrição e horário;
if (Válido?) then (não)
  :Negar check-in e informar motivo;
  stop
else (sim)
endif
if (Check-in já realizado?) then (sim)
  :Retornar presença existente;
else (não)
  :Confirmar presença atomicamente;
endif
:Meus Ingressos exibe Check-in realizado;
stop
@enduml
```

O QR Code não contém dados pessoais, JWT ou o ID numérico da participação. Seu conteúdo é somente `Participacao.CodigoIngresso`, um valor aleatório gerado por inscrição e protegido por índice único. O Web usa `qrcode.react`; o Mobile mantém `react-native-qrcode-svg`; ambos codificam o mesmo valor entregue pela API. O backend continua sendo a fonte da verdade para titularidade, status da inscrição, evento, horário, operador e instituição.

## Fluxo Secretaria

```plantuml
@startuml
start
:Login Secretaria;
:JWT com instituicao_id;
:Eventos da instituição;
:Selecionar evento;
:Dashboard do evento;
:Visualizar inscrições, vagas, check-ins e métricas;
:Validar check-in quando necessário;
stop
@enduml
```

A Secretaria só consulta dashboards de eventos vinculados à própria instituição. O backend compara `Evento.InstituicaoId` com a claim `instituicao_id` do JWT por meio de `CanAccessInstituicao`.

No check-in, o backend também relê o usuário institucional. Tokens emitidos antes de um bloqueio ou revogação não bastam: a conta precisa continuar ativa, aprovada e com role operacional compatível.

## Fluxo De Certificação Automática

```plantuml
@startuml
start
:Secretaria/operador confirma presença;
if (Autorizado para a instituição?) then (não)
  :Negar acesso;
  stop
else (sim)
endif
if (Participação, código e janela válidos?) then (não)
  :Não confirmar presença;
  stop
else (sim)
endif
:Confirmar presença atomicamente;
if (Primeira confirmação?) then (não)
  :Retornar sucesso sem novo envio;
  stop
else (sim)
endif
if (Evento possui template?) then (não)
  :Finalizar check-in;
  stop
else (sim)
endif
:Reservar processamento no banco;
:Gerar e persistir PDF oficial;
:Enviar os mesmos bytes como anexo;
if (SMTP confirmou entrega?) then (sim)
  :Registrar Enviado;
else (não)
  :Classificar falha ou resultado incerto;
  :Agendar apenas retry seguro e limitado;
endif
:Preservar presença confirmada;
stop
@enduml
```

Aluno FATEC e Público Geral percorrem o mesmo fluxo porque ambos usam `Aluno` e `Participacao`. A configuração `Certificado` pertence ao evento; o PDF emitido e seu estado de entrega pertencem à participação. Veja detalhes de arquitetura e dados em [CERTIFICACAO_AUTOMATICA.md](CERTIFICACAO_AUTOMATICA.md).

## Regra De Capacidade

```plantuml
@startuml
start
:Solicitação de inscrição;
:Buscar aluno e evento;
:Validar período de inscrições;
:Validar público permitido;
:Verificar duplicidade;
:Abrir transação Serializable;
:Contar participações do evento;
if (Inscrições >= capacidade?) then (sim)
  :Recusar inscrição;
  stop
else (não)
endif
:Inserir Participacao;
:Commit;
stop
@enduml
```

Fonte da verdade:

`Vagas disponíveis = Evento.Capacidade - quantidade de Participacao ativa do evento`

Não existe campo persistido para vagas restantes. A API calcula a disponibilidade ao listar ou detalhar eventos e na dashboard por evento.

## Endpoints Envolvidos

- `POST /api/Auth/cadastro-publico`: cria conta do Público Geral.
- `POST /api/Auth/login-publico`: autentica Público Geral e retorna JWT.
- `GET /api/Evento`: lista eventos e calcula vagas disponíveis.
- `GET /api/Evento/{id}`: detalha evento e calcula vagas disponíveis.
- `POST /api/Evento/{id}/inscrever-se`: registra inscrição respeitando público, duplicidade, período e capacidade.
- `GET /api/Evento/{id}/minha-inscricao`: informa se o participante autenticado já está inscrito.
- `GET /api/Evento/meus-ingressos`: lista ingressos usando exclusivamente o `NameIdentifier` do JWT.
- `GET /api/Evento/{id}/ingresso`: detalha somente o ingresso do participante autenticado naquele evento.
- `GET /api/Evento/{eventId}/dashboard`: dashboard individual do evento para Admin/Secretaria autorizados.
- `POST /api/Evento/check-in`: confirma presença administrativamente e dispara a certificação na primeira confirmação.
- `GET /api/Certificado/meus`: retorna somente as participações do participante autenticado.
- `GET /api/Certificado/eventos/{eventoId}/pdf`: retorna somente o PDF próprio com presença confirmada.
- `POST /api/Automacoes/processar-agora`: processamento manual restrito ao Admin global.

## Segurança

- Público Geral não acessa endpoints administrativos porque mantém role técnica `Aluno`.
- Aluno FATEC e Público Geral não possuem operação para confirmar a própria presença.
- Secretaria/operador não valida check-in de outra instituição porque o backend cruza evento, claim e usuário institucional ativo.
- Trocar o identificador do evento não contorna o vínculo institucional nem a correspondência do código do ingresso.
- A API de ingressos não aceita `userId`; alterar o identificador do evento só retorna um ingresso quando a participação pertence ao usuário autenticado.
- Inscrição com `StatusInscricao.Cancelada` permanece no histórico, não consome vaga e é recusada pelo check-in.
- Check-in repetido é idempotente: a atualização condicional confirma presença uma vez e não gera múltiplos registros.
- O download cruza o identificador do JWT com a participação e não revela certificados de terceiros.
- Regras de público permitido são verificadas no backend por `EventoRules.PodeParticipar`.
- Inscrição duplicada é bloqueada por regra de serviço e índice único `(AlunoId, EventoId)`.
- A última vaga é protegida por transação com isolamento `Serializable`.
