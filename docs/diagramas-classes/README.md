# Diagramas de classes do UniEvent

Esta pasta contém as visões de classes do backend UniEvent, desde o modelo de domínio até as dependências entre API, aplicação e infraestrutura.

## Diagramas

| Diagrama | Conteúdo |
| --- | --- |
| [01_modelo_dominio.png](01_modelo_dominio.png) | Entidades, enums, herança e associações do domínio |
| [02_arquitetura_camadas.png](02_arquitetura_camadas.png) | Controllers, interfaces, serviços, repositórios, `AppDbContext` e dependências entre camadas |
| [03_eventos_participacoes_certificacao.png](03_eventos_participacoes_certificacao.png) | Classes centrais de eventos, inscrição, ingresso, check-in e certificação |
| [04_identidade_gestao_institucional.png](04_identidade_gestao_institucional.png) | Autenticação, usuários, instituições, responsáveis e persistência |

Os arquivos `.puml` são as fontes editáveis e os `.png` são as versões renderizadas.

## Relações UML utilizadas

- `<|--`: herança.
- `..|>`: implementação de interface.
- `-->`: dependência ou chamada direta.
- `o--`: agregação.
- Multiplicidades como `0..1` e `0..*` representam as associações do domínio.

## Renderização

```bash
JAVA_TOOL_OPTIONS=-Djava.awt.headless=true plantuml -charset UTF-8 *.puml
```

## Validação

```bash
JAVA_TOOL_OPTIONS=-Djava.awt.headless=true plantuml -checkonly -charset UTF-8 *.puml
```
