# Spike S1-T03 — Atos registrais estruturais

**Estado:** Draft técnico empilhado; não integrado; S1-T03 pendente

**Data:** 16/10/2026

**Dependências:** draft PR #7 e, transitivamente, draft PRs #5 e #6

## Objetivo

Experimentar tipos de domínio capazes de preservar a estrutura documental de atos registrais sem antecipar revisão humana, efeito jurídico, reconstrução de estado ou persistência.

## Rastreabilidade

- RF-015: extração estruturada com proveniência documental.
- RF-016: vínculos verificáveis entre atos e respectivas fontes.
- RNF-004: rastreabilidade e ausência de inferência silenciosa.
- RNF-011: representação determinística e testável.
- ADR-006: separação entre informação extraída e decisão humana.
- ADR-013: conteúdo e versão de fonte tratados como referências explícitas.

## API candidata

- `AtoRegistralId` e `OrdemAtoRegistral` distinguem identidade do ato, ordem documental e número do marcador.
- `IdentificadorAtoRegistral` preserva o texto original sanitizado e expõe `R-n` ou `AV-n` somente quando o tipo é conhecido.
- `DatasDocumentaisAto` mantém título, ato e apresentação independentes, inclusive quando ausentes ou fora de ordem cronológica.
- `ParteAtoRegistral` separa o rótulo documental de um `PessoaId` opcional; texto nunca cria identidade de pessoa.
- `RelacaoAtoRegistral` registra alvo e tipo sem inferir efeito.
- `AtoRegistral` copia páginas, partes e relações; exige página; rejeita página repetida, autorrelação e relação duplicada.

Atos com marcador não identificado são preservados. `R-5` não é identidade global: matrícula, fonte, versão e `AtoRegistralId` mantêm o contexto.

## Compatibilidade experimental

GD-003 é mapeada para dois atos candidatos, mantendo a ordem documental, as páginas e a relação `AV-4 → R-3`. A matrícula usada pelo teste é sintética porque o schema candidato ainda não traz esse contexto.

O schema atual também não possui datas documentais nem partes estruturadas. Essa lacuna permanece explícita; nenhum schema é alterado neste spike.

## Limites

- Sem estado de revisão, evidência por campo, efeito, direito ou reconstrução de matrícula.
- Sem persistência, DTO, API, WPF, OCR ou parser de documento.
- Sem regra jurídica ou conclusão sobre o significado de R, AV ou relações.
- Sem novas referências de projeto ou pacote no Domain.
- Os tipos são candidatos e podem mudar após estabilização de S0-T06 e S1-T02.

## Riscos

- Cardinalidades e nomes podem mudar quando o contrato externo for estabilizado.
- Ordem na fonte não representa prioridade, validade ou cronologia jurídica.
- Relação estrutural não comprova cancelamento, retificação ou qualquer efeito.
- Rótulos documentais podem ser ambíguos e não substituem identificação humana.

## Evidências locais

- Validador de contratos: 5 fixtures sintéticas aprovadas.
- Casos negativos: 4 mutações inválidas rejeitadas.
- `dotnet build CartorioTwin.sln -c Release --no-restore`: aprovado com 0 avisos e 0 erros.
- `dotnet test CartorioTwin.sln -c Release --no-build`: 63 testes aprovados em 4 projetos; 60 pertencem ao Domain.
- `dotnet restore CartorioTwin.sln --locked-mode`: aprovado; lock files preservados.
- `dotnet format ... whitespace --verify-no-changes`: aprovado para os arquivos C# do spike.
- `dotnet list ... reference` e `dotnet list ... package`: Domain sem referência de projeto ou pacote.
- `git diff --check`: aprovado.
- CI e URL serão registrados no draft PR.

S0-T06, S1-T02, S1-T03, Sprint 0, Sprint 1, G0 e G1 permanecem pendentes.
