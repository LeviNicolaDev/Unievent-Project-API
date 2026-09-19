# Diagramas de atividades de negócio do UniEvent

Esta pasta contém um diagrama de atividade para cada um dos 47 casos de uso do UniEvent.

## Escopo

Os diagramas descrevem somente a perspectiva do negócio:

- atores e áreas responsáveis;
- atividades realizadas;
- regras e decisões de negócio;
- resultados esperados;
- caminhos alternativos e exceções aplicáveis.

Detalhes de implementação como rotas, autenticação técnica, banco de dados, classes, serviços e protocolos não fazem parte destes diagramas. Esses assuntos estão nos diagramas de sequência, de classes e entidade-relacionamento.

## Exceções

Todos os diagramas possuem ao menos um caminho marcado como `exceção`. As exceções incluem, conforme o processo, dados inválidos, duplicidade, ausência de autorização, indisponibilidade, falta de vagas, inscrição cancelada, período encerrado, ausência de presença e falha de entrega.

## Renderização

```bash
JAVA_TOOL_OPTIONS=-Djava.awt.headless=true plantuml -charset UTF-8 UC*.puml
```

## Validação

```bash
JAVA_TOOL_OPTIONS=-Djava.awt.headless=true plantuml -checkonly -charset UTF-8 UC*.puml
```
