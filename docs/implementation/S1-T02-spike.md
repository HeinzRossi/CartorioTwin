# Spike S1-T02 — Cadeia de evidência documental

**Estado:** Draft técnico; não integrado; S1-T02 pendente

**Data:** 15/10/2026

**Dependência bloqueadora:** S0-T06, que depende de S0-T02 e S0-T03

## Objetivo

Testar uma API de domínio mínima para vincular uma evidência a fonte, versão, hash, página e intervalo exato de texto. O spike é reversível e não estabiliza contratos antes dos schemas e fixtures de S0-T06.

## Rastreabilidade

- RF-019: proveniência e manifesto.
- RNF-004: rastreabilidade.
- ADR-013: versões fixadas e hashes não tratados como assinatura.
- `03_DOMAIN_RULES.md`: fonte documental imutável.
- `09_DOCUMENT_AI_PIPELINE.md`: trecho pertencente à versão exata da extração.
- `10_CONTRACTS_PROCESSING.md` §3: offsets UTF-16 e localização verificável.

## API candidata

- `FonteDocumentalId`: identidade forte sobre UUID.
- `Pagina`: número iniciado em 1.
- `VersaoFonte`: número positivo.
- `HashSha256`: valor hexadecimal canônico e cálculo puro sobre bytes ou UTF-8.
- `FonteDocumental`: metadados imutáveis mínimos, sem storage ou operação.
- `TrechoEvidencia`: fonte, versão, hashes, página e intervalo UTF-16.

`TrechoEvidencia.Criar` recebe o texto integral da página e deriva o trecho. Não existe parâmetro para fornecer um trecho independente da fonte, reduzindo o risco de evidência textual inventada. A validação definitiva contra a extração persistida pertence a tarefas posteriores.

## Limites

- Sem persistência, DTO, schema JSON, API, WPF, OCR ou IA.
- Sem `OperacaoId`, `ExtracaoId`, dimensões, rotação ou `BoundingBox`.
- Sem afirmar que coincidência literal comprova contexto, identidade ou efeito jurídico.
- Sem nova referência de projeto ou pacote no Domain.
- Os tipos públicos são candidatos do draft PR e podem mudar após S0-T06.

## Riscos

- S0-T06 pode alterar nomes, cardinalidades e fronteiras entre fonte e extração.
- `VersaoFonte` ainda não substitui a versão específica da execução de extração.
- Sanitização de nome é uma defesa mínima; política de upload pertence à borda da aplicação.
- SHA-256 detecta diferença de conteúdo, mas não é assinatura nem controle de acesso.

## Critérios do spike

- Hash canônico e vetor conhecido testados.
- Página, versão, IDs, metadados e intervalos inválidos rejeitados.
- Trecho derivado exatamente por unidades UTF-16, inclusive com caracteres suplementares.
- Referências da fonte preservadas na evidência.
- Domain permanece puro.
- Restore bloqueado, build Release, suíte completa e CI do draft PR aprovados.

## Evidências

- `dotnet restore CartorioTwin.sln --locked-mode`: aprovado; lock files preservados.
- `dotnet build CartorioTwin.sln -c Release --no-restore`: aprovado em Windows; 0 avisos e 0 erros.
- `dotnet test CartorioTwin.sln -c Release --no-build`: quatro assemblies; 36 testes aprovados, 0 falhas e 0 ignorados.
- `dotnet list ... reference`: nenhuma referência de projeto no Domain.
- `dotnet list ... package`: nenhum pacote no Domain.
- `dotnet format whitespace ... --include <arquivos do spike> --verify-no-changes`: aprovado; baseline fora do spike não foi alterada.
- `git diff --check`: aprovado.
- CI e URL serão registradas no draft PR.

Estas evidências validam somente o spike. Elas não concluem S1-T02, Sprint 1 ou G1.
