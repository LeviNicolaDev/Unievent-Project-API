# Instituição do aluno

`Aluno.InstituicaoId` identifica a FATEC de origem declarada pelo aluno no cadastro mobile.

Esse vínculo não filtra automaticamente o catálogo de eventos exibido no aplicativo. Alunos internos podem visualizar eventos públicos de todas as instituições cadastradas no UniEvent. O filtro por instituição no mobile é opcional e representa a instituição organizadora do evento.

Significados atuais:

- `Aluno.InstituicaoId`: instituição de origem autodeclarada pelo aluno.
- `Evento.InstituicaoId`: instituição organizadora do evento.
- `UsuarioSecretaria.InstituicaoId`: instituição que a Secretaria pode administrar.
- `ResponsavelEvento.InstituicaoId`: instituição à qual o responsável pertence.

A restrição por instituição deve ser aplicada na inscrição, não na visualização. Para eventos com `PublicoPermitido.AlunosDaInstituicao`, o backend permite inscrição somente quando `Aluno.InstituicaoId == Evento.InstituicaoId`. Para `TodosAlunosFatec` e `PublicoGeral`, alunos autenticados podem se inscrever conforme as demais regras do evento, como período, vagas e inscrição duplicada.

Nesta versão, a instituição do aluno é autodeclarada. O sistema valida o e-mail institucional Fatec, mas não confirma automaticamente se o aluno pertence à unidade selecionada. Uma evolução futura pode integrar o UniEvent a um serviço oficial do Centro Paula Souza para validar e-mail, RA, instituição, curso e status acadêmico antes de marcar a instituição como verificada.
