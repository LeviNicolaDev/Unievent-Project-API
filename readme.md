# UniEvent

Plataforma para descoberta, gestão, inscrição, check-in e certificação de eventos institucionais.

## Arquitetura

- `Unievent.Api`: API ASP.NET Core com autenticação JWT, controllers e configuração do EF Core.
- `Unievent.Application`: DTOs, services, validators, interfaces e regras de negócio.
- `Unievent.Domain`: entidades e enums do domínio.
- `Unievent.Infra`: `AppDbContext`, repositories, migrations e serviços de infraestrutura.
- `Unievent-Project-Web-Frontend`: portal web React/Vite, com área pública e área administrativa.
- `Unievent-Project-Mobile/biased-orange-donut`: aplicativo mobile React Native/Expo para alunos FATEC.
- `Unievent.Tests`: testes automatizados xUnit.

## Perfis

- `Admin`: administrador global do UniEvent, sem vínculo obrigatório com instituição.
- `Secretaria`: usuário administrativo vinculado a uma instituição/FATEC.
- `Aluno`: participante autenticado. Quando `TipoParticipante = Interno`, representa aluno FATEC; quando `TipoParticipante = Externo`, representa Público Geral.
- `OperadorCheckIn`: operador autorizado a validar ingressos/check-in.

### Sessões simultâneas na Web

O portal Web mantém uma sessão independente para cada área no mesmo perfil do navegador:

- `unievent.auth.admin`: Admin global do UniEvent;
- `unievent.auth.secretaria`: Secretaria vinculada a uma instituição;
- `unievent.auth.publico`: participante do Público Geral.

O `AuthContext` seleciona a sessão pela área acessada, e o cliente HTTP envia somente o token correspondente à rota atual. Entrar ou sair em um portal não substitui nem encerra as sessões dos outros portais. Alterações feitas em outra aba são sincronizadas pelo evento `storage` do navegador.

Na primeira execução após a atualização, uma sessão antiga armazenada em `authToken` e `authUser` é migrada automaticamente para o escopo identificado pelas claims do usuário e as chaves antigas são removidas.

## Dashboard Por Evento

Usuários `Secretaria` acessam uma dashboard individual por evento da própria instituição em:

- Web: `Secretaria > Eventos > Visualizar dashboard`
- API: `GET /api/Evento/{eventId}/dashboard`

A dashboard retorna nome, data, local, instituição, responsável, público permitido, capacidade total, inscrições, vagas restantes, check-ins, ausentes, percentual de ocupação, percentual de presença, status do evento, certificado e distribuição entre alunos FATEC e público geral.

A autorização é aplicada no backend. Uma Secretaria só consegue consultar eventos cujo `Evento.InstituicaoId` seja igual ao `instituicao_id` presente no JWT.

## Público Geral

A página pública web permite:

- criar conta em `/criar-conta`;
- entrar em `/entrar`;
- descobrir eventos em `/descobrir-eventos`;
- visualizar detalhes do evento;
- ver vagas disponíveis e estado de lotação;
- inscrever-se em eventos com `PublicoPermitido = PublicoGeral`;
- consultar `Meus Ingressos`, incluindo próximos eventos e histórico;
- abrir o ingresso individual com QR Code responsivo e código textual;
- acompanhar inscrição ativa/cancelada e check-in pendente/realizado;
- sair da sessão.

O Público Geral é modelado como `Aluno` com `TipoParticipante = Externo`, mantendo o fluxo existente de `Participacao`, ingresso, check-in e certificado.

## Meus Ingressos

Depois da inscrição, o ingresso fica disponível em `/meus-ingressos`. A Web chama `GET /api/Evento/meus-ingressos`; o endpoint não recebe `userId` e obtém o participante pelo `NameIdentifier` do JWT. Cada item retorna evento, instituição, data, horário, local, status da inscrição, status da presença e o código da participação.

A listagem separa próximos eventos do histórico e mantém ingressos de eventos já realizados. O detalhe em `/meus-ingressos/{eventoId}` amplia o QR Code para apresentação em celular. Depois do check-in, o ingresso continua acessível e passa a mostrar `Check-in realizado`. Uma inscrição cancelada permanece no histórico sem QR Code utilizável.

## QR Code Do Ingresso

O fluxo é:

```text
Inscrição
→ Meus Ingressos
→ QR Code
→ leitura pela Secretaria
→ validação no backend
→ check-in
```

Cada `Participacao` recebe um `CodigoIngresso` aleatório de 32 caracteres hexadecimais, gerado por `Guid.NewGuid().ToString("N")` e protegido por índice único no banco. O QR Code contém somente esse código; não inclui JWT, senha, CPF, e-mail ou ID numérico previsível. O Mobile e a Web usam o mesmo valor e os mesmos endpoints, embora renderizem a imagem com bibliotecas próprias de cada plataforma.

Ao receber o código em `POST /api/Evento/check-in`, o backend busca a participação e valida inscrição ativa, correspondência opcional do evento, janela de horário e autorização do operador para a instituição organizadora. A presença é atualizada condicionalmente apenas quando ainda não estava confirmada. Leituras repetidas retornam o estado já confirmado e não criam nova presença nem repetem a certificação.

## Sistema De Vagas

A fonte da verdade das vagas é:

`Vagas disponíveis = Evento.Capacidade - quantidade de Participacao ativa do evento`

As inscrições ativas de alunos FATEC e Público Geral consomem a mesma capacidade. Participações canceladas permanecem no histórico e deixam de consumir vaga. Não há campo persistido para vagas restantes, evitando divergência entre capacidade e inscrições reais.

A inscrição usa transação com isolamento `Serializable` em `ParticipacaoRepository.TryAddWithinCapacityAsync`, além do índice único por `(AlunoId, EventoId)`, para evitar duplicidade e impedir que duas inscrições simultâneas ultrapassem a última vaga.

## Regras De Público

- `PublicoGeral`: aceita alunos internos e público geral externo.
- `TodosAlunosFatec`: aceita somente participantes internos FATEC.
- `AlunosDaInstituicao`: aceita somente participantes internos cuja instituição seja a instituição organizadora do evento.

Essas regras são verificadas no backend por `EventoRules.PodeParticipar`.

## Certificação Automática

O fluxo oficial de certificação é:

```text
Participante se inscreve
→ apresenta o ingresso/QR Code
→ Secretaria confirma a presença
→ UniEvent verifica a configuração do evento
→ gera e persiste o PDF
→ envia o arquivo como anexo por e-mail
```

O mesmo fluxo atende alunos FATEC (`TipoParticipante.Interno`) e Público Geral (`TipoParticipante.Externo`) por meio da mesma entidade `Participacao`. A leitura ou exibição do QR Code não emite certificado. A emissão começa somente quando um usuário autorizado confirma a presença no endpoint de check-in.

Quando o evento não possui um `Certificado` configurado, a presença é confirmada normalmente e o processamento termina. Quando existe configuração, o backend gera um PDF A4 em orientação paisagem com participante, evento, instituição, data, responsável, texto livre do template, código de validação e data de emissão. O arquivo fica armazenado na participação e também pode ser baixado novamente pelo próprio participante.

### PDF e template

O backend utiliza `PDFsharp-MigraDoc` 6.2.4. O layout está em `Unievent.Infra/Templates/CertificadoPdfTemplate.cs`; as fontes Liberation Sans são incorporadas ao assembly para manter a geração compatível com Linux e Docker. A licença das fontes está em `Unievent.Infra/Templates/Fonts/LICENSE.txt`.

O nome do anexo é sanitizado no formato `certificado-{evento}-{participante}.pdf`. O mobile baixa o mesmo PDF oficial da API e usa `expo-file-system`; ele não cria uma segunda versão local do certificado.

### E-mail e tratamento de falhas

`EmailService` usa SMTP e envia o PDF com MIME `application/pdf`. A presença e o PDF são persistidos antes da tentativa de envio. Assim, uma indisponibilidade do SMTP nunca desfaz o check-in.

Falhas temporárias recebem nova tentativa pelo `AutomacaoEventosService`, com espera exponencial e limite configurável. Rejeições permanentes ficam disponíveis para análise. Se a conexão cair depois que o envio começou, o estado fica `EnvioIncerto`: o sistema não repete automaticamente porque o servidor pode ter aceitado a mensagem. Um processamento abandonado durante o SMTP segue a mesma regra.

### Idempotência

Cada `Participacao` mantém o PDF, código, destinatário, datas, contador de tentativas, estado do envio e uma reserva de processamento. A reserva é adquirida por atualização condicional atômica no SQL Server. Chamadas repetidas ou concorrentes não conseguem reservar a mesma participação ao mesmo tempo, e participações marcadas como enviadas não entram novamente no fluxo automático.

Os estados são `Pendente`, `Preparando`, `Enviando`, `Enviado`, `FalhaTemporaria`, `FalhaPermanente` e `EnvioIncerto`. Um reenvio futuro deve ser uma ação administrativa explícita; o fluxo automático não reenvia estados finais.

### Endpoints do participante

- `GET /api/Evento/meus-ingressos`: lista somente os ingressos do participante obtido pelo JWT.
- `GET /api/Evento/{eventoId}/ingresso`: retorna o ingresso daquele evento somente se pertencer ao participante autenticado.
- `GET /api/Certificado/meus`: lista somente as inscrições e estados de certificado do participante autenticado.
- `GET /api/Certificado/eventos/{eventoId}/pdf`: baixa o PDF somente quando pertence ao participante autenticado e sua presença foi confirmada.

As respostas do PDF usam `Cache-Control: no-store`. Alunos e Público Geral não possuem endpoint para confirmar a própria presença.

### Configuração

Configure valores reais somente por variáveis de ambiente, `.env` local não versionado ou user secrets:

| Variável | Finalidade | Padrão no Compose |
| --- | --- | --- |
| `EmailSettings__Host` | Host SMTP | `smtp.gmail.com` |
| `EmailSettings__Port` | Porta SMTP | `587` |
| `EmailSettings__Email` | Conta remetente | sem padrão |
| `EmailSettings__Password` | Senha/token SMTP | sem padrão |
| `EmailSettings__EnableSsl` | Habilita TLS | `true` |
| `Certificacao__MaxTentativas` | Limite total de tentativas | `5` |
| `Certificacao__RetryMinutos` | Espera-base do retry | `5` |
| `Certificacao__ReservaMinutos` | Validade da reserva de preparação/envio | `10` |
| `Automacoes__Ativas` | Ativa o worker de pendências | `true` no Compose |
| `Automacoes__IntervaloMinutos` | Intervalo do worker | `30` |

No desenvolvimento sem SMTP, o check-in continua válido e o estado de envio registra a falha. Para testar o anexo sem provedor externo, os testes automatizados iniciam um servidor SMTP local em memória e conferem os bytes recebidos.

## Execução

Com Docker:

```bash
docker compose up --build
```

Serviços principais:

- API: `http://localhost:5227`
- Web: `http://localhost:5173`
- Mobile/Expo: `http://localhost:8081`

Variáveis relevantes estão em `docker-compose.yml`, incluindo `PASSWORD_DB`, `Jwt__Key`, `Jwt__Issuer`, `Jwt__Audience` e configurações de e-mail.

## Testes

```bash
dotnet test Unievent.Tests/Unievent.Tests.csproj
npm run build --prefix Unievent-Project-Web-Frontend
npm run test:e2e --prefix Unievent-Project-Web-Frontend
(cd Unievent-Project-Mobile/biased-orange-donut && npx expo export --platform all --output-dir /tmp/unievent-mobile-export)
```

Os testes de integração usam SQLite por padrão. O teste real de migration SQL Server é executado quando `UNIEVENT_TEST_SQLSERVER` contém uma connection string para uma instância descartável; ele cria e remove um banco temporário, sem alterar o banco `Unievent`.

O projeto utiliza EF Core migrations versionadas em `Unievent.Infra/Migrations`. A migration `AddCertificadoPdfDelivery` adiciona o estado de entrega e os dados do PDF preservando participações e certificados existentes. Emissões legadas já marcadas como enviadas são migradas para `Enviado`; emissões antigas sem confirmação de envio ficam como `EnvioIncerto`, evitando reenvio automático indevido.

O desenho técnico, o modelo de dados e o diagrama de sequência estão em [Certificação automática](docs/CERTIFICACAO_AUTOMATICA.md).

O fluxo completo do ingresso Web está em [Fluxos Público Geral e Secretaria](docs/fluxos-publico-secretaria-capacidade.md), e a sequência entre Web, API, banco e Secretaria está em [Diagrama de sequência do ingresso Web](docs/DIAGRAMA_SEQUENCIA_INGRESSO_WEB.puml).
