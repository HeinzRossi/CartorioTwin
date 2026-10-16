# Spike S1-T04 — Direitos documentados candidatos

**Estado:** baseline técnica candidata integrada; S1-T04 formalmente pendente

**Data:** 16/10/2026

**Dependências:** S1-T01 e baseline técnica candidata de S1-T03 integradas; validação externa de cobertura e efeitos continua pendente

## Objetivo

Representar direitos documentados, titulares, extensão e conhecimento do efeito sem aplicar consequência jurídica, reconstruir matrícula ou promover informação extraída a fato confirmado.

## Rastreabilidade

- RF-005: estrutura futura para revisão do quadro de titulares, frações e lacunas.
- RF-007: categorias e frações exatas preservam a base do motor determinístico.
- RF-016: direitos mantêm vínculo verificável com ato, fonte, versão, página e trecho.
- RNF-011: domínio puro e testes determinísticos, sem banco, WPF ou infraestrutura.
- ADR-004: valores imutáveis e ausência de mutação de estado reconstruído.
- ADR-006: estrutura determinística anterior à interpretação assistida e sem promoção por IA.
- ADR-015: extensão fracionária usa fração do total e mantém a categoria do direito.

## API candidata

- `DireitoDocumentadoId` fornece identidade forte sem geração aleatória no Domain.
- `TipoDireitoDocumentado` distingue propriedade plena, nua-propriedade, usufruto, ônus, garantia e tipo não identificado.
- `ExtensaoDireitoDocumentado` representa total, fração do total ou ausência explícita de identificação. Frações exigem `0 < f <= 1`; `1/1` é canonicalizado como total.
- `ConhecimentoEfeitoDireito` registra somente se o efeito não foi avaliado, é conhecido, é desconhecido ou não é suportado.
- `DireitoDocumentado` deriva matrícula, ato, fonte e versão do ato de origem; preserva datas documentadas sem impor cronologia; copia titulares e evidências defensivamente.
- Cada evidência deve pertencer à fonte, versão e páginas do ato. Ao menos uma evidência literal é obrigatória neste contrato candidato.
- `ChaveTitularidadeDocumentada` inclui matrícula, pessoa e categoria. Propriedade, nua-propriedade e usufruto da mesma pessoa produzem chaves distintas.

Titulares podem permanecer vazios quando não identificados. O agregado não cria pessoa a partir de rótulo textual e não descarta direito de tipo desconhecido.

## Limites

- Sem persistência, DTO, JSON Schema, API, WPF, OCR ou parser.
- Sem entrada manual, estados de revisão ou promoção para fato confirmado; esses pontos pertencem à S1-T05.
- Sem agregação de frações, snapshot ou simulação; essas operações pertencem às Sprints 2 e 5.
- Sem inferência de vigência, cancelamento, ausência de ônus, disponibilidade jurídica ou efeito registral.
- `Conhecido` descreve somente conhecimento declarado do efeito; não contém nem executa mapeamento jurídico.
- Schemas e fixtures de S0-T06 permanecem inalterados e ainda não possuem direitos estruturados.

## Riscos

- Taxonomia, cardinalidades e nomes podem mudar após validação de RI/TN e estabilização dos contratos externos.
- Um direito pode ter titulares ainda não identificados; coleção vazia não significa inexistência de titular.
- Extensão não identificada não equivale a zero nem a total.
- A presença de uma evidência literal não comprova contexto, identidade, vigência ou efeito.
- Ônus ou garantia não localizados pelo processamento não podem ser considerados inexistentes.

## Evidências locais

- Validador de contratos: 5 fixtures sintéticas aprovadas.
- Casos negativos: 4 mutações inválidas rejeitadas.
- `dotnet build CartorioTwin.sln -c Release --no-restore`: aprovado com 0 avisos e 0 erros.
- `dotnet test CartorioTwin.sln -c Release --no-build`: 83 testes aprovados em 4 projetos; 80 pertencem ao Domain.
- Testes novos: 20 casos cobrindo identidade, categorias, extensão, datas, titulares, evidências, conhecimento do efeito e separação das chaves.
- `dotnet restore CartorioTwin.sln --locked-mode`: aprovado; lock files preservados.
- `dotnet format ... whitespace --verify-no-changes --include ...`: aprovado para todos os arquivos C# da S1-T04.
- `dotnet list ... reference` e `dotnet list ... package --include-transitive`: Domain sem referência de projeto ou pacote.
- `git diff --check`: aprovado.

A verificação de formatação da solução inteira continua apontando arquivos legados WPF/smoke fora desta alteração. Nenhum deles foi reescrito neste spike.

- PR [#10](https://github.com/HeinzRossi/CartorioTwin/pull/10): aprovado por `build-and-test` em 1m16s e `secret-scan` em 6s, execução `38899593607`.
- Squash `a7de636` integrado exclusivamente pelo PR #10.
- CI subsequente da `main`: `build-and-test` aprovado em 1m17s e `secret-scan` em 9s, execução `38899849269`.

S1-T04, Sprint 1 e G1 permanecem pendentes de validação externa.
