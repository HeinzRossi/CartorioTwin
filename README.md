# Cartório Twin

Fundação técnica do Cartório Twin, plataforma Windows para análise documental assistida e simulação condicional de operações imobiliárias.

> Estado atual: fundação S0-T04 e núcleo de domínio S1-T01 concluídos; baselines técnicas candidatas de contratos, fixtures, evidências, atos, direitos documentados, revisão humana, serialização versionada, catálogo físico SQL Server e identidade/autorização estão implementadas. S0-T01 continua sem entrevistas reais e S0-T06/S1-T02…S1-T08 dependem de decisões e validações externas. Nenhum gate G0–G9, sprint, MVP, homologação jurídica ou implantação foi concluído.

## Requisitos

- Windows x64.
- .NET SDK 10.0.401.
- Runtime Windows Desktop 10.
- Acesso ao NuGet para o primeiro restore.
- Docker Desktop com engine Linux para os testes de persistência SQL Server.

## Estrutura

- `src/CartorioTwin.Desktop.Wpf`: cliente desktop WPF.
- `src/CartorioTwin.Desktop.Client`: cliente HTTP tipado futuro.
- `src/CartorioTwin.Api`: serviço ASP.NET Core.
- `src/CartorioTwin.Worker`: host de processamento futuro.
- `src/CartorioTwin.Application`: casos de uso.
- `src/CartorioTwin.Domain`: domínio puro.
- `src/CartorioTwin.Infrastructure`: adaptadores de infraestrutura.
- `src/CartorioTwin.Contracts`: contratos públicos versionados.
- `tests/`: testes xUnit por camada.
- `docs/CartorioTwin_Documentacao_v1`: baseline documental 1.2.0.

## Build e testes

```powershell
dotnet restore CartorioTwin.sln --locked-mode
dotnet build CartorioTwin.sln -c Release --no-restore
dotnet test CartorioTwin.sln -c Release --no-build
```

Os testes de `CartorioTwin.Infrastructure.Tests` iniciam um SQL Server 2022 descartável por Testcontainers. Defina `CARTORIOTWIN_SKIP_SQLSERVER_TESTS=true` somente quando a mesma suíte for executada por um job Docker separado, como ocorre no CI Windows/Linux.

A API opera fechada e exige configuração OIDC/JWT válida; perfis efetivos e vínculo com a serventia são resolvidos no banco. O cliente desktop está preparado para Authorization Code + PKCE, sem segredo, mantendo access token somente em memória e protegendo refresh token com DPAPI do usuário Windows. IdP, certificados, storage físico, ACL e retenção produtivos continuam deliberadamente não definidos.

Leia [`docs/CartorioTwin_Documentacao_v1/AGENTS.md`](docs/CartorioTwin_Documentacao_v1/AGENTS.md) antes de implementar qualquer task.
