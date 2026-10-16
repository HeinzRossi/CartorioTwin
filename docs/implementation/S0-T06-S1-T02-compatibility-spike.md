# Spike de compatibilidade S0-T06 ↔ S1-T02

**Estado:** Baseline técnica candidata integrada; tasks formais pendentes

**Data:** 16/10/2026

**Dependências:** baselines dos PRs #5 e #6 integradas; S0-T02 e S0-T03 continuam bloqueadas pela S0-T01 real

## Objetivo

Provar que a parte sobreposta dos contratos candidatos — fonte documental, versão, hash, página e trecho — pode ser consumida pelo domínio sem texto inventado ou normalização silenciosa do formato externo.

## Decisões experimentais

- O JSON usa `sha256:<64 hex minúsculos>` e o domínio o recebe por parser canônico estrito.
- `HashSha256.Valor` preserva somente os 64 dígitos internos; `ValorCanonico` e `ToString()` preservam o formato externo.
- As fixtures são copiadas somente para o projeto de testes; `CartorioTwin.Domain` não referencia schema, JSON, pacote ou outro projeto.
- O teste percorre exatamente GD-001…GD-005 e reconstrói toda evidência presente a partir do texto integral da página.
- `application/pdf` é exercitado, mas o MIME do domínio não é definitivamente restringido neste spike.

## Limites

- Não cria DTO público nem modela ato, revisão, cenário, OCR, persistência ou API.
- Não estabiliza os contratos nem substitui S0-T02/S0-T03 ou validação profissional.
- A integração técnica não estabiliza os contratos nem autoriza uso operacional.

## Validação

- Parser canônico aceita round-trip válido e rejeita caixa, prefixo, comprimento ou caractere inválido.
- Cinco fixtures mapeiam metadados de fonte e todos os trechos para os tipos candidatos.
- Validadores positivo e negativo dos contratos continuam aprovados.
- Restore bloqueado, build Release, suíte completa e CI passam na baseline integrada.
- O Domain deve permanecer sem referências de projeto ou pacote.

## Evidências locais

- Validador de contratos: 5 fixtures sintéticas aprovadas.
- Casos negativos: 4 mutações inválidas rejeitadas.
- `dotnet restore CartorioTwin.sln --locked-mode`: aprovado.
- `dotnet build CartorioTwin.sln -c Release --no-restore`: aprovado com 0 avisos e 0 erros.
- `dotnet test CartorioTwin.sln -c Release --no-build`: 44 testes aprovados em 4 projetos; 41 pertencem ao Domain.
- `dotnet format ... whitespace --verify-no-changes`: aprovado para os arquivos C# do spike.
- `dotnet list ... reference` e `dotnet list ... package`: Domain sem referências de projeto ou pacote.
- PR [#7](https://github.com/HeinzRossi/CartorioTwin/pull/7): versão reconstruída sobre `main` aprovada por `build-and-test` em 59s e `secret-scan` em 9s, execução `38895241570`.
- Squash `42d4f7a`; CI subsequente da `main` aprovado em 1m15s/5s, execução `38895401796`.

S0-T06, S1-T02, Sprint 0, G0 e G1 permanecem pendentes.
