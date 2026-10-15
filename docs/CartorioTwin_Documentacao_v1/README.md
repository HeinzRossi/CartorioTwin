# Cartório Twin — documentação funcional e técnica

**Versão:** 1.2.0 (baseline documental consolidada)  
**Data:** 15/10/2026  
**Plataforma:** Windows, **C# 14 + WPF em .NET 10 (LTS)**  
**Especialidades iniciais:** Registro de Imóveis (RI) e Tabelionato de Notas (TN)  
**Estado do projeto:** documentação proposta; nenhuma implementação, validação jurídica formal ou implantação concluída.

## Propósito

Cartório Twin é um **simulador de operações imobiliárias assistido por evidências documentais**. Distingue (a) situação documentada em fontes fornecidas, (b) operação pretendida e (c) estado projetado **condicional**. O produto identifica inconsistências estruturais e pontos de revisão, mas **não pratica atos notariais/registrais, não registra títulos, não certifica validade jurídica, nem substitui a qualificação de tabeliães e registradores**.

A inovação está em manter um *modelo de direitos, pessoas, imóveis e atos* que possa ser interrogado e simulado, com rastreabilidade até documentos-fonte; não em gerar pareceres jurídicos automáticos.

## Decisões e premissas

| Tema | Decisão nesta versão | Situação |
|---|---|---|
| Linguagem/plataforma | C# 14, .NET 10 e WPF/XAML, Windows | **Definida pelo solicitante** |
| Especialidades | RI e TN | **Definida pelo solicitante** |
| Diferencial | Gêmeo digital + simulação de direitos + conferência assistida | Visão de produto |
| Frontend | Desktop WPF MVVM | Proposta técnica |
| Backend | ASP.NET Core 10 hospedado na intranet/on-premises | Proposta técnica, pode ser revista |
| Banco | SQL Server (produção); testes com banco efêmero | Proposta técnica, validar licenças/implantação |
| MVP | PDF textual/escaneado → extração/OCR → atos candidatos → revisão de atos → reconstrução → revisão do estado → proposta → simulação → dossiê | Escopo consolidado; implantação e homologação pendentes |
| IA/OCR | Pipeline obrigatório; provedor local a selecionar por benchmark. **Envio externo desativado por padrão** | Viabilidade medida na Sprint 0; sem promoção autônoma de fatos |
| Fonte dos dados | Apenas documentos/integrações legitimamente disponibilizados pela serventia | Restrição permanente |
| Jurisdição | Regras nacionais base; regras locais por versão e serventia após curadoria | A validar juridicamente |

## Como ler este pacote

| Arquivo | Conteúdo |
|---|---|
| [01_PRODUCT.md](01_PRODUCT.md) | Visão, personas, casos de uso, escopo, critérios de sucesso e requisitos |
| [02_ARCHITECTURE.md](02_ARCHITECTURE.md) | Solução C#/.NET 10, camadas, projetos, contratos, APIs e implantação |
| [03_DOMAIN_RULES.md](03_DOMAIN_RULES.md) | Modelo jurídico-documental, simulação, regras, estados e exemplos |
| [04_DESIGN_WPF.md](04_DESIGN_WPF.md) | Navegação, layout WPF, componentes, cores, estados e acessibilidade |
| [05_DATA_SECURITY.md](05_DATA_SECURITY.md) | Modelo de dados, arquivos, evidências, autorização, privacidade, auditoria e integrações |
| [06_TEST_PLAN.md](06_TEST_PLAN.md) | Matriz de cenários, testes automatizados, segurança, aceitação e métricas |
| [07_SPRINTS_TASKS.md](07_SPRINTS_TASKS.md) | Roadmap com tarefas, entregas, dependências, critérios de aceite e riscos |
| [08_ADRS.md](08_ADRS.md) | Registro das decisões arquiteturais e questões em aberto |
| [09_DOCUMENT_AI_PIPELINE.md](09_DOCUMENT_AI_PIPELINE.md) | Pipeline obrigatório, atos, evidências e dois portões de revisão |
| [10_CONTRACTS_PROCESSING.md](10_CONTRACTS_PROCESSING.md) | Dados, contratos HTTP, jobs duráveis, hashes e publicação concorrente |
| [11_OPERATIONS_EVALUATION.md](11_OPERATIONS_EVALUATION.md) | Provedores, corpus, métricas, recuperação, distribuição e curadoria normativa |
| [12_TRACEABILITY.md](12_TRACEABILITY.md) | RF/RNF → ADR → tasks → testes e aplicação das 16 melhorias |
| [CHANGELOG.md](CHANGELOG.md) | Histórico da consolidação documental |
| [AGENTS.md](AGENTS.md) | Instruções para ferramentas de codificação (Copilot/Codex e equivalentes) |

## Entrega prevista por etapas

- **Fase 1 — MVP (Sprints 0–9):** jornada documental completa de PDF textual/escaneado até dossiê, revisão em dois portões, uma matrícula por operação, compra e venda de frações, minuta opcional quando fornecida, jobs recuperáveis e segurança. Entrada manual é correção/contingência.
- **Fase 2 — Produtividade:** comparação A/B de cenários, fontes sobrepostas e versões da matrícula, fila de revisão e cobertura documental ampliada, após medir o piloto.
- **Fase 3 — Direitos complexos:** usufruto, garantias e múltiplas matrículas com novas regras jurídicas versionadas e validadas.
- **Fase 4 — Conectividade:** conectores autorizados, análise automatizada supervisionada e implantação multi-serventia.

## Primeira demonstração de negócio (MVP)

**Cenário CT-001**: PDF sintético de matrícula processado e revisado com João (1/2) e Maria (1/2); operação pretende transferir 100% apenas de João a Carlos. O sistema retorna `DIVERGENTE`, mostra o cálculo **1/2 < 1/1**, aponta a origem dos dados de titularidade e **não persiste alteração na situação atual**. Quando a proposta é ajustada para 1/2, projeta Carlos (1/2) + Maria (1/2). Quando João e Maria transferem suas metades, projeta Carlos (1/1). Nada disso produz efeitos registrais.

## Regras gerais de entrega

- Documentação é *baseline de produto*, não laudo jurídico. Toda regra sobre ato registral deve ter **fonte normativa, versão, aplicabilidade por UF/serventia e aprovação de especialista** antes de ser ativada em produção.
- `CONSISTENTE` significa que **as regras executadas sobre as evidências disponíveis não encontraram divergências**; **não significa 'título apto'**. Falta de evidência produz `INCONCLUSIVO`.
- Nenhum dado pessoal real deve ser adicionado a exemplos, testes, logs, modelos de IA públicos ou repositório.
- Não há integração presumida com e-Notariado, SREI, ONR ou sistemas de terceiros; qualquer conexão depende de autorização, contrato e interfaces disponíveis.

## Fontes de referência (verificar atualizações antes de uso em produção)

- [Documentação WPF — Microsoft](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/overview/)
- [.NET 10 LTS — Microsoft](https://learn.microsoft.com/pt-br/dotnet/core/releases-and-support)
- [Lei nº 10.406/2002 — Código Civil](https://www.planalto.gov.br/ccivil_03/leis/2002/l10406compilada.htm)
- [Lei nº 6.015/1973 — Registros Públicos](https://www.planalto.gov.br/ccivil_03/leis/l6015consolidado.htm)
- [Provimento CNJ nº 149/2023 — Código Nacional de Normas](https://atos.cnj.jus.br/atos/detalhar/5243)
- Referências recebidas como [Provimento CNJ nº 213/2026](https://atos.cnj.jus.br/atos/detalhar/6734) e [nº 243/2026](https://atos.cnj.jus.br/atos/detalhar/6936): **pendentes de confirmação de número, ano, ementa, vigência e relação de alteração**; os links retornaram 403 na revisão. Não habilitam regras nem comprovam conformidade.
- [Lei nº 13.709/2018 — LGPD](https://www.planalto.gov.br/ccivil_03/_ato2015-2018/2018/lei/l13709.htm)

**Atenção:** referências são insumos para análise, não uma matriz exaustiva de requisitos legais estaduais/municipais; validar o texto consolidado na data do desenvolvimento.

## Governança desta baseline

Esta versão incorpora as melhorias autorizadas em 15/10/2026 **na documentação**; não representa implementação, testes da aplicação ou homologação. O escopo é governado por 01 e pelo contrato de conclusão de 07; 03 define semântica; 10 define protocolos; ADRs registram decisões. AGENTS orienta execução e não redefine produto. Divergências devem gerar correção coordenada e changelog, nunca interpretação silenciosa. A matriz 12 fornece rastreabilidade.

A numeração v1 no nome do pacote identifica a família documental. A versão vigente é 1.2.0 em todos os arquivos. Componentes locais de OCR/IA são obrigatórios no MVP aprovado, mas sua tecnologia e hardware dependem de benchmark; se inviáveis, o piloto fica bloqueado até revisão explícita do plano.
