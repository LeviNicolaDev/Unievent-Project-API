# Diagramas entidade-relacionamento do UniEvent

Esta pasta documenta o modelo persistente atual do UniEvent. As entidades, chaves, restrições e cardinalidades foram conferidas no `AppDbContext` e no `AppDbContextModelSnapshot`.

## Diagramas

| Diagrama | Conteúdo |
| --- | --- |
| [01_modelo_relacional_geral.png](01_modelo_relacional_geral.png) | Visão geral das 10 entidades persistidas e seus relacionamentos |
| [02_identidade_instituicoes.png](02_identidade_instituicoes.png) | Usuários, instituições, responsáveis e vínculo institucional |
| [03_eventos_participacoes_certificacao.png](03_eventos_participacoes_certificacao.png) | Eventos, inscrições, check-in, certificados, preferências e notificações |

Os arquivos `.puml` são as fontes editáveis e os `.png` são as versões renderizadas.

## Convenções

- `PK`: chave primária.
- `FK`: chave estrangeira.
- `UQ`: índice ou composição única.
- `0..1`: relacionamento opcional, correspondente a uma FK anulável.
- `1` ou `||`: relacionamento obrigatório.
- `0..*` ou `o{`: zero ou muitos registros relacionados.
- `CASCADE` e `RESTRICT` indicam o comportamento de exclusão configurado pelo EF Core.

## Renderização

```bash
JAVA_TOOL_OPTIONS=-Djava.awt.headless=true plantuml -charset UTF-8 *.puml
```

## Validação

```bash
JAVA_TOOL_OPTIONS=-Djava.awt.headless=true plantuml -checkonly -charset UTF-8 *.puml
```
