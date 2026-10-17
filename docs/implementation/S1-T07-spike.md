# Spike S1-T07 — Persistência física SQL Server

**Estado:** baseline técnica candidata em integração; validação externa pendente

**Data:** 16/10/2026

**Dependências:** S0-T06 e S1-T06 possuem baselines técnicas integradas, mas continuam pendentes de validação externa

## Objetivo

Materializar o catálogo lógico da seção 10 em SQL Server/EF Core, preservando contexto de serventia e operação por constraints físicas. Esta entrega não implementa autenticação, autorização de sessão, storage de documentos nem execução de jobs.

## Rastreabilidade

- RF-002: operação, matrícula, histórico e artefatos possuem raízes físicas versionadas.
- RF-003: matrícula e suas fontes permanecem vinculadas à mesma operação.
- RF-012: eventos de auditoria possuem ator, recurso, operação, correlação e instante, sem corpo documental.
- RNF-002: FKs compostas impedem relações entre serventias e operações distintas; políticas de sessão pertencem à S1-T08.
- RNF-011: Domain permanece sem EF Core, pacote, referência de projeto, I/O ou banco.
- ADR-003/009: persistência usa SQL Server/EF Core atrás da API; o desktop não acessa banco.
- ADR-013: revisões e artefatos publicados possuem linhas separadas, versões únicas e relações verificáveis.

## Modelo físico candidato

- Migration inicial com 46 tabelas para identidade técnica, operações, fontes, extrações, evidências, atos, revisões, eventos, reconstruções, snapshots, direitos, cenários, simulações, achados, regras, jobs e auditoria.
- Registros EF são internos à Infrastructure e não substituem entidades do Domain.
- Toda entidade tenant-aware possui chave alternativa `(ServentiaId, Id)`; entidades operacionais também possuem `(ServentiaId, OperacaoId, Id)`.
- Relações sensíveis usam FKs compostas e `DeleteBehavior.Restrict`.
- Evidências são N:M por tabelas associativas próprias para segmentos, campos, direitos, titularidades e achados.
- Payloads evolutivos auxiliares usam JSON com `ISJSON(..., VALUE)`; IDs, versões, contexto e relações nunca ficam apenas em JSON.
- Hashes estruturados exigem `sha256:` e 64 hexadecimais minúsculos; páginas, offsets, versões, frações e instantes de revisão possuem checks próprios.
- `Operacao` e `Cenario` usam `rowversion`; versões publicadas usam linhas append-only.

## Banco e migrations

- Provider e ferramentas EF Core: 10.0.12.
- Testcontainers.MsSql: 4.15.0, somente nos testes.
- Imagem fixada: SQL Server 2022 `16.0.4295.3`, Ubuntu 22.04, digest `sha256:4402d880dd4c34bfa7d8705e56a86cd6c88da80a1f6bbbe741f999e76264a090`.
- Migration não é aplicada automaticamente pela API ou Worker. Implantação futura exige manutenção planejada, backup e revisão do script.
- A fábrica de design-time exige `CARTORIOTWIN_DESIGNTIME_CONNECTION`; não há fallback para LocalDB nem credencial versionada.
- `System.Security.Cryptography.Pkcs` foi elevado transitivamente para 9.0.21 devido aos avisos GHSA-2fmv-rv2c-59x7 e GHSA-846f-f764-76mx; nenhum alerta foi suprimido.

## Testes

- Modelo contém 46 entidades e associações tenant-aware esperadas.
- Migration sobe em banco vazio e produz script idempotente.
- Grafo fonte → extração → evidência → ato/campo/revisão e job/tentativa sobrevive ao descarte do contexto e reinício do container.
- SQL Server rejeita referência cruzada entre serventias, página duplicada, hash não canônico e intervalo vazio.
- Concorrência de operação é detectada por `rowversion`.
- Testes usam somente massa sintética e bancos descartáveis.

## Riscos e limites

- O catálogo é amplo e candidato; evolução exigirá migrations aditivas e compatibilidade explícita.
- Constraints relacionais não substituem autorização por usuário/serventia, que pertence à S1-T08.
- O modelo registra jobs, leases e fencing, mas o protocolo durável será implementado em S3-T08.
- JSON válido não comprova semântica, evidência ou efeito jurídico.
- Nenhum usuário SQL de produção, política de retenção ou permissão de UPDATE/DELETE foi definido nesta task.

## Evidências locais

- Catálogo gerado: 46 tabelas.
- `dotnet build tests/CartorioTwin.Infrastructure.Tests/... -c Release --no-restore -m:1`: aprovado com 0 avisos e 0 erros.
- Teste inicial Docker revelou e permitiu corrigir a aceitação de valores JSON escalares.
- Validadores contratuais: 5 fixtures aprovadas e 4 mutações negativas rejeitadas.
- `dotnet restore CartorioTwin.sln --locked-mode`: aprovado com lock files atualizados.
- `dotnet build CartorioTwin.sln -c Release --no-restore -m:1`: aprovado com 0 avisos e 0 erros.
- Suíte Windows: 134 testes aprovados e 7 testes SQL explicitamente delegados ao job Docker.
- Suíte SQL Server Docker: 9/9 testes aprovados, incluindo reinício real do container, hash, offsets, FKs, unicidade e concorrência.
- `dotnet ef migrations script --idempotent ...`: aprovado com ferramenta local 10.0.12.
- Formatação dos projetos alterados em modo `--verify-no-changes`: aprovada.
- Domain confirmado sem referência de projeto ou pacote.
- `git diff --check`: aprovado.
- Evidências de PR, squash e CI serão registradas após integração.

S1-T07, Sprint 1 e G1 permanecem formalmente pendentes das validações externas e de S1-T08/S1-T09.
