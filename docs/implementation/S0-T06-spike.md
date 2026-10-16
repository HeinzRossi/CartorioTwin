# Spike S0-T06 — Contratos e golden fixtures sintéticas

**Estado:** Draft técnico; S0-T06 pendente

**Data:** 15/10/2026

**Dependências bloqueadoras:** S0-T02 e S0-T03, ambas dependentes da S0-T01 real

## Objetivo

Exercitar um contrato versionado para fonte, página, evidência, ato candidato, revisão e resultado esperado. O material usa somente dados sintéticos e não substitui corpus autorizado, catálogo de atos ou validação profissional.

## Rastreabilidade

- RF-015: extração e revisão de atos.
- RF-016: reconstrução rastreável de fatos e relações.
- RF-019: manifesto e proveniência.
- RNF-004: rastreabilidade.
- RNF-011: contratos testáveis e manutenção.
- ADR-013: versões fixadas e hashes canônicos, sem tratar hash como assinatura.

## Entregáveis do spike

- JSON Schema Draft 2020-12 autocontido, versão `1.0-draft`.
- Cinco fixtures sintéticas cobrindo texto nativo, OCR de baixa qualidade, relação multipágina, ato não suportado e CT-001.
- Validador PowerShell sem dependência externa.
- Etapa de CI dentro de `build-and-test`.

## Validações

- Exatamente cinco casos, com IDs e fontes únicos.
- JSON válido contra schema, UTF-8 sem BOM e marcador sintético obrigatório.
- SHA-256 canônico, GUID minúsculo, páginas e versões positivas.
- Hash do texto recalculado e trecho conferido pelos offsets UTF-16.
- Referências de página, ato, campo, relação e revisão existentes no mesmo caso.
- Padrões de CPF, CNPJ, e-mail e IPv4 ausentes.
- Caso inconclusivo ou divergente sem projeção permitida.
- Quatro mutações negativas temporárias provam rejeição de fixture não sintética, referência inexistente, offset incorreto e projeção divergente.

## Riscos e limites

- O schema é candidato e pode mudar após S0-T02/S0-T03.
- Não há PDF no Git; hashes de fonte representam entradas sintéticas do exercício.
- Frequência, prática por UF, efeito jurídico e cobertura de ato não foram validados.
- O golden dataset oficial ainda exige revisão por profissionais de RI/TN.
- O schema não estabiliza DTO HTTP, persistência ou serialização canônica de S1-T09.

## Aceite deste spike

- `pwsh -NoProfile -File scripts/Validate-ContractFixtures.ps1`: 5 fixtures sintéticas aprovadas.
- `pwsh -NoProfile -File scripts/Test-ContractFixtureValidator.ps1`: 4 mutações inválidas rejeitadas.
- `dotnet restore CartorioTwin.sln --locked-mode`: aprovado.
- `dotnet build CartorioTwin.sln -c Release --no-restore`: aprovado com 0 avisos e 0 erros.
- `dotnet test CartorioTwin.sln -c Release --no-build`: 17 testes aprovados em 4 projetos.
- `git diff --check`: aprovado.
- Draft PR [#6](https://github.com/HeinzRossi/CartorioTwin/pull/6): `build-and-test` aprovado em 1m17s e `secret-scan` aprovado em 5s.
- Nenhum merge; S0-T06, S1-T02, Sprint 0, G0 e G1 permanecem pendentes.
