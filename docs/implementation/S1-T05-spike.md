# Spike S1-T05 — Revisão humana e promoção controlada

**Estado:** baseline técnica candidata implementada; integração e validação externa pendentes

**Data:** 16/10/2026

**Dependências:** baseline técnica candidata de S1-T04 integrada; legitimidade da autorização depende de S1-T08

## Objetivo

Representar fatos documentais revisáveis separando ciclo de vida, proveniência e referência opaca de autorização. `Confirmado` registra decisão humana; não representa verdade ou efeito jurídico.

## Rastreabilidade

- RF-014: revisão humana registra decisão, usuário, instante, justificativa e versão-alvo.
- RF-015: fatos preservam vínculo com ato, fonte, versão, página e trecho verificável.
- RNF-002: promoção terminal exige referência de autorização, sem antecipar RBAC ou sessão.
- RNF-011: domínio puro, determinístico e sem relógio global, I/O ou infraestrutura.
- ADR-006: saída extraída permanece candidata até decisão humana explícita.
- ADR-013: versões anteriores são imutáveis e correções posteriores geram nova identidade.
- ADR-014: proveniência documental permanece distinta da decisão e da autorização.

## API candidata

- `FatoDocumentalId`, `RevisaoFatoDocumentalId`, `UsuarioId` e `AutorizacaoRevisaoId` fornecem identidades fortes e rejeitam UUID vazio.
- `VersaoFatoDocumental` exige número positivo e incremento exato com overflow controlado.
- `FatoDocumentalRevisavel<TValor>` deriva contexto do `AtoRegistral`, valida caminho de campo e evidências e mantém valores sugerido e final separadamente.
- `RevisaoFatoDocumental<TValor>` registra decisão, revisor, autorização opaca, versão-alvo e instante UTC fornecido pelo chamador.
- `ValorOpcional<TValor>` distingue ausência explícita de valor do valor presente, inclusive para tipos por valor.
- Estados terminais não reabrem. Alteração posterior cria novo fato, incrementa a versão em uma unidade e vincula a identidade anterior.

Transições candidatas:

- `Extraido → EmRevisao`;
- confirmação do valor sugerido → `Confirmado + Localizada`;
- correção diferente e justificada → `Confirmado + Localizada`;
- contestação → `Contestado`;
- não localizado → `Contestado + NaoLocalizada`;
- declaração manual → `Confirmado + DeclaradaSemEvidencia`;
- não suportado → `NaoSuportado`.

## Compatibilidade

Os testes leem diretamente GD-001…GD-005 com `JsonDocument`, sem DTO público nem alteração do schema. Os estados externos `extracted`, `under_review`, `confirmed`, `contested`, `not_found`, `declared_without_evidence` e `unsupported` são mapeados para ciclo de vida e proveniência separados.

## Limites

- Sem RBAC, validação de sessão ou garantia de que `AutorizacaoRevisaoId` seja legítima; isso pertence à S1-T08.
- Sem persistência, API, WPF, DTO, schema novo, relógio global ou serviço de autorização.
- Sem revisão completa do ato, composição de campos, reconstrução ou efeito jurídico.
- `Confirmado` significa decisão humana registrada, não comprovação jurídica.
- `DeclaradaSemEvidencia` não equivale a evidência documental e, isoladamente, não autoriza projeção.

## Riscos

- A referência opaca pode ser criada por chamador não autorizado até existir validação na camada de aplicação.
- O tipo genérico exige política de igualdade adequada para cada valor usado pelo chamador.
- Caminhos de campo ainda são identificadores técnicos candidatos e podem mudar após estabilização contratual.
- A correção cria versão nova, mas persistência e concorrência otimista permanecem fora deste spike.
- Taxonomia e transições dependem de validação externa e podem mudar antes da estabilização.

## Evidências locais

- Validador de contratos: 5 fixtures sintéticas aprovadas.
- Casos negativos: 4 mutações inválidas rejeitadas.
- `dotnet restore CartorioTwin.sln --locked-mode`: aprovado; lock files preservados.
- `dotnet build CartorioTwin.sln -c Release --no-restore -m:1`: aprovado com 0 avisos e 0 erros.
- `dotnet test CartorioTwin.sln -c Release --no-build -m:1`: 112 testes aprovados em 4 projetos; 109 pertencem ao Domain.
- Testes novos: 29 casos cobrindo identidades, versões, transições, proveniência, autorização, UTC, imutabilidade, novas versões e compatibilidade com fixtures.
- Formatação: arquivos C# da S1-T05 formatados isoladamente; dívida legada fora do escopo não foi reescrita.
- `dotnet list ... reference` e `dotnet list ... package --include-transitive`: Domain sem referência de projeto ou pacote.
- `git diff --check`: aprovado.

S1-T05, Sprint 1 e G1 permanecem formalmente pendentes de S1-T08 e validação externa.
