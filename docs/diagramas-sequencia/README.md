# Diagramas de sequência do UniEvent

Esta pasta contém os diagramas de sequência dos fluxos funcionais do UniEvent. Os arquivos `.puml` são as fontes editáveis em PlantUML e os arquivos `.png` são as versões renderizadas para consulta e inclusão em documentos.

Os diagramas foram derivados dos casos de uso documentados e conferidos contra as rotas atuais dos controllers, serviços do frontend web, aplicativo mobile e serviços de aplicação/infraestrutura.

## Índice e cobertura

| Diagrama | Casos de uso cobertos | Fluxo |
| --- | --- | --- |
| [01_acesso_publico_eventos.png](01_acesso_publico_eventos.png) | UC001 a UC004 | Landing page, descoberta, filtros e detalhes públicos |
| [02_cadastro_confirmacao_login_aluno.png](02_cadastro_confirmacao_login_aluno.png) | UC005 a UC008 | Cadastro institucional, instituição, confirmação e login mobile |
| [03_eventos_e_favoritos_mobile.png](03_eventos_e_favoritos_mobile.png) | UC009 a UC011 | Eventos disponíveis, detalhes e favoritos locais |
| [04_inscricao_e_ingresso_mobile.png](04_inscricao_e_ingresso_mobile.png) | UC012 a UC015 | Inscrição, meus eventos, ingresso e QR Code mobile |
| [05_perfil_e_preferencias.png](05_perfil_e_preferencias.png) | UC016 e UC017 | Perfil e preferências de notificação |
| [06_login_web_e_admin_inicial.png](06_login_web_e_admin_inicial.png) | UC018 e UC019 | Login administrativo e bootstrap do Admin |
| [07_gestao_instituicoes.png](07_gestao_instituicoes.png) | UC020 a UC023 | CRUD e ativação de instituições |
| [08_gestao_usuarios_secretaria.png](08_gestao_usuarios_secretaria.png) | UC024 a UC026 | Cadastro e moderação de secretarias |
| [09_dashboard_admin_e_automacoes.png](09_dashboard_admin_e_automacoes.png) | UC027 e UC028 | Métricas globais e execução de automações |
| [10_dashboards_instituicao_evento.png](10_dashboards_instituicao_evento.png) | UC029 e UC045 | Dashboards institucional e individual do evento |
| [11_gestao_eventos.png](11_gestao_eventos.png) | UC030 a UC034 | CRUD de eventos e público permitido |
| [12_responsaveis_e_templates_certificado.png](12_responsaveis_e_templates_certificado.png) | UC035 e UC036 | Responsáveis e templates de certificado |
| [13_validacao_checkin.png](13_validacao_checkin.png) | UC037 a UC039 | Leitura do código, autorização e janela de check-in |
| [14_certificacao_automatica_email.png](14_certificacao_automatica_email.png) | UC040 e UC041 | PDF, persistência, e-mail e tratamento de falhas |
| [15_cadastro_login_inscricao_publico.png](15_cadastro_login_inscricao_publico.png) | UC042 a UC044 | Conta, login e inscrição do Público Geral |
| [16_certificado_proprio.png](16_certificado_proprio.png) | UC046 | Consulta e download seguro do certificado próprio |
| [17_meus_ingressos_web.png](17_meus_ingressos_web.png) | UC047 | Ingressos web, QR Code e atualização após check-in |

## Convenções

- As chamadas HTTP exibem as rotas atuais da API.
- Blocos `alt` representam sucesso, falhas de validação, autorização ou indisponibilidade.
- O banco representa o `AppDbContext` e os repositórios do projeto.
- Operações restritas consideram JWT, role e escopo institucional.
- O tema visual compartilhado fica em `_tema.puml`.

## Renderização

Com PlantUML instalado, execute a partir desta pasta:

```bash
JAVA_TOOL_OPTIONS=-Djava.awt.headless=true plantuml -charset UTF-8 *.puml
```

Para validar sem gerar imagens:

```bash
JAVA_TOOL_OPTIONS=-Djava.awt.headless=true plantuml -checkonly -charset UTF-8 *.puml
```
