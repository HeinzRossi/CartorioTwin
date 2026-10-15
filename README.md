# Cartório Twin

Fundação técnica do Cartório Twin, plataforma Windows para análise documental assistida e simulação condicional de operações imobiliárias.

> Estado atual: implementação inicial da task S0-T04. Nenhum gate G0–G9, sprint, MVP, homologação jurídica ou implantação foi concluído.

## Requisitos

- Windows x64.
- .NET SDK 10.0.401.
- Runtime Windows Desktop 10.
- Acesso ao NuGet para o primeiro restore.

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

Leia [`docs/CartorioTwin_Documentacao_v1/AGENTS.md`](docs/CartorioTwin_Documentacao_v1/AGENTS.md) antes de implementar qualquer task.
