# Instituição do aluno

`Aluno.InstituicaoId` identifica a FATEC de origem declarada pelo aluno no cadastro mobile.

Esse vínculo não filtra automaticamente o catálogo de eventos exibido no aplicativo. Alunos internos podem visualizar eventos públicos de todas as instituições cadastradas no UniEvent. O filtro por instituição no mobile é opcional e representa a instituição organizadora do evento.

Significados atuais:

- `Aluno.InstituicaoId`: instituição de origem autodeclarada pelo aluno.
- `Evento.InstituicaoId`: instituição organizadora do evento.
- `UsuarioSecretaria.InstituicaoId`: instituição que a Secretaria pode administrar.
- `ResponsavelEvento.InstituicaoId`: instituição à qual o responsável pertence.

A restrição por instituição deve ser aplicada na inscrição, não na visualização. Para eventos com `PublicoPermitido.AlunosDaInstituicao`, o backend permite inscrição somente quando `Aluno.InstituicaoId == Evento.InstituicaoId`. Para `TodosAlunosFatec` e `PublicoGeral`, alunos autenticados podem se inscrever conforme as demais regras do evento, como período, vagas e inscrição duplicada.

O Público Geral da web também utiliza `Aluno`, mas com `TipoParticipante.Externo` e sem `InstituicaoId`. Esse participante não precisa de e-mail institucional e só consegue se inscrever quando `Evento.PublicoPermitido == PublicoGeral`.

As vagas disponíveis não são armazenadas no cadastro do evento. Elas são calculadas por `Evento.Capacidade - quantidade de Participacao ativa`, somando alunos internos e público externo na mesma capacidade. Participações canceladas permanecem no histórico e não consomem vaga.

Essa mesma abstração atende a certificação. Depois que a Secretaria ou um operador autorizado confirma a presença, a `Participacao` armazena o PDF, o código, o destinatário, as datas e o estado da entrega. O e-mail vem de `Aluno.Email` tanto para `Interno` quanto para `Externo`; não existem serviços paralelos de certificado por tipo de participante.

O participante pode consultar apenas as próprias participações em `GET /api/Certificado/meus` e baixar o PDF próprio em `GET /api/Certificado/eventos/{eventoId}/pdf`. `Aluno.InstituicaoId` continua sendo usado na elegibilidade de eventos restritos, enquanto `Evento.InstituicaoId` define qual Secretaria pode confirmar a presença.

Para ingressos, `GET /api/Evento/meus-ingressos` extrai o `AlunoId` do JWT e retorna somente as participações do titular. Aluno FATEC e Público Geral compartilham `Participacao.CodigoIngresso`: o Mobile e a Web geram o QR Code a partir desse mesmo token aleatório, e a Secretaria envia o conteúdo lido ao endpoint de check-in. Nenhum dado pessoal ou JWT é codificado no QR.

Nesta versão, a instituição do aluno é autodeclarada. O sistema valida o e-mail institucional Fatec, mas não confirma automaticamente se o aluno pertence à unidade selecionada. Uma evolução futura pode integrar o UniEvent a um serviço oficial do Centro Paula Souza para validar e-mail, RA, instituição, curso e status acadêmico antes de marcar a instituição como verificada.
