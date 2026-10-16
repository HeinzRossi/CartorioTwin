# Spike de compatibilidade S0-T06 ↔ S1-T02

**Estado:** Draft técnico empilhado; não integrado; tasks pendentes

**Data:** 16/10/2026

**Dependências:** draft PRs #5 e #6; S0-T02 e S0-T03 continuam bloqueadas pela S0-T01 real

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
- Não autoriza integração dos PRs #5, #6 ou deste spike.

## Validação prevista

- Parser canônico aceita round-trip válido e rejeita caixa, prefixo, comprimento ou caractere inválido.
- Cinco fixtures mapeiam metadados de fonte e todos os trechos para os tipos candidatos.
- Validadores positivo e negativo dos contratos continuam aprovados.
- Restore bloqueado, build Release, suíte completa e CI devem passar.
- O Domain deve permanecer sem referências de projeto ou pacote.

## Evidências locais

- Validador de contratos: 5 fixtures sintéticas aprovadas.
- Casos negativos: 4 mutações inválidas rejeitadas.
- `dotnet restore CartorioTwin.sln --locked-mode`: aprovado.
- `dotnet build CartorioTwin.sln -c Release --no-restore`: aprovado com 0 avisos e 0 erros.
- `dotnet test CartorioTwin.sln -c Release --no-build`: 44 testes aprovados em 4 projetos; 41 pertencem ao Domain.
- `dotnet format ... whitespace --verify-no-changes`: aprovado para os arquivos C# do spike.
- `dotnet list ... reference` e `dotnet list ... package`: Domain sem referências de projeto ou pacote.
- Draft PR [#7](https://github.com/HeinzRossi/CartorioTwin/pull/7): primeira rodada de `build-and-test` aprovada em 1m5s e `secret-scan` aprovada em 9s, execução `38845023087`.

S0-T06, S1-T02, Sprint 0, G0 e G1 permanecem pendentes.
