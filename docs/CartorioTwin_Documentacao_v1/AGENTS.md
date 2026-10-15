# AGENTS.md — instruções para agentes de implementação

**Versão:** 1.2.0 · **Data:** 15/10/2026 · **Estado:** especificação documental; implementação e homologação pendentes.

## Missão

Implementar **Cartório Twin**, uma plataforma **PDF de matrícula → extração/OCR → interpretação assistida de atos → revisão → reconstrução do estado documentado → negócio pretendido → simulação → dossiê**, em **C# + .NET 10 + WPF/XAML** para Windows, com serviço API ASP.NET Core 10 e SQL Server **conforme ADRs propostos**. Não transformar o desktop em React/Electron/Delphi. Não escrever motor jurídico que emita aprovação do registro.

## Ordem de leitura obrigatória

1. `README.md` — visão, restrições e fontes.
2. `01_PRODUCT.md` — RF e limites de produto.
3. `03_DOMAIN_RULES.md` — semântica de estados e regras.
4. `02_ARCHITECTURE.md` — solução, contratos e API.
5. `04_DESIGN_WPF.md` — UI detalhada.
6. `05_DATA_SECURITY.md` — segurança e LGPD.
7. `06_TEST_PLAN.md` — casos de aceitação.
8. `07_SPRINTS_TASKS.md` — execução por Sprint.
9. `08_ADRS.md` — decisões confirmadas e pendentes.
10. `09_DOCUMENT_AI_PIPELINE.md` — **pipeline obrigatório do MVP** e portões de revisão.
11. `10_CONTRACTS_PROCESSING.md` — contratos, dados, jobs, hashes e publicação concorrente.
12. `11_OPERATIONS_EVALUATION.md` — avaliação, licenças/capacidade, operação e normas.
13. `12_TRACEABILITY.md` e `CHANGELOG.md` — rastreabilidade e versão vigente.

## Regras de implementação obrigatórias

- **Domain é puro**: sem EF, WPF, `HttpClient`, relógio global, disco ou banco.
- **Não entregar MVP baseado apenas em entrada manual.** PDF com texto nativo, PDF escaneado com OCR, segmentação/interpretação de atos, revisão humana e estado reconstruído são requisitos **P0**.
- Manter 3 engines desacopladas: `Document Intelligence`, `Registry State` e `Legal Simulation`. IA nunca altera diretamente titularidade/ônus e não gera estados confirmados.
- Todo campo candidato precisa de fonte/página/trecho (ou estado `NaoLocalizado`); validador confere que evidência consta da fonte.
- Dois portões obrigatórios: revisão humana dos **atos/campos** e revisão humana do **estado reconstruído**. Cada etapa com versão, usuário e justificativa.
- Ato relevante não suportado/lacuna/efeito ambíguo impede consistência e quadro completo; sem divergência confirmada leva a `Inconclusivo`, com motivo de encaminhamento.
- Frações de direitos em **razões exatas**; `double` proibido em frações de propriedade.
- `MatriculaSnapshot` é imutável; simulações produzem projeções **condicionais** e nunca modificam dados de origem.
- Falta de fonte essencial impede consistência e projeção; sem divergência confirmada => `INCONCLUSIVO`. Divergência confirmada => `DIVERGENTE` mantendo lacunas. `CONSISTENTE_NO_ESCOPO` exige entradas suficientes e verificações mínimas executadas, não apenas ausência de alertas.
- Nenhum componente usa termos `AptoParaRegistro`, `RegistroAprovado` ou `ValidadeGarantida`.
- Todo alerta deve ter código da regra, versão, severidade e referência de evidência (ou explicitar que não há evidência suficiente).
- Não baixar automaticamente bases oficiais nem implementar scraping ou bypass de autenticação.
- Não enviar documento a LLM/cloud sem decisão formal de contratação, finalidade, privacy review e configuração opt-in autorizada.
- Nunca incluir PII real em testes, exemplos, prompts ou logs; criar dados sintéticos.
- Endpoints autorizam por usuário + serventia e cadeia operação/cenário/snapshot/fonte no backend; não confiar apenas na UI ou em `tenantId` do payload.
- WPF usa MVVM; código de negócio fica fora de ViewModels e code-behind. Estilos em XAML separados.
- Expor telas de loading, empty, error, success, disabled e stale; avisos importantes persistentes.
- Preferir `async/await` em operações com I/O e `CancellationToken` em chamadas; simulação pura síncrona.
- Falha parcial em lote não pode produzir estado projetado completo; usar resultado divergente/inconclusivo.

## Convenções C#

- Nullable reference types habilitados; `implicit usings` habilitados.
- `record`/`record struct` para valores imutáveis com invariantes; exceptions para invariantes técnicas, resultados tipados para falhas de negócio previstas.
- Nomeação PascalCase em tipos/propriedades/métodos, nomes portugueses coerentes e sem abreviações obscuras.
- Interfaces apenas em fronteiras que requerem substituição/testabilidade, sem abstração artificial.
- Validar entrada nas bordas (`Api`/Application) e invariantes de domínio internamente.
- Dependency Injection em `App.xaml.cs` (host) e API; `HttpClientFactory`; sem service locator no domínio.
- Scripts/migrations versionados, não aplicar alterações destrutivas em produção sem plano explícito.

## Plano de implementação

Executar `07_SPRINTS_TASKS.md` na ordem. Antes de cada task: registrar RF/RNF atendido, critérios de teste, arquivos impactados e riscos. Após cada task: build/teste, atualizar docs/ADR, abrir PR. O planejamento vigente é **Sprints 0–9**; não seguir o antigo 0–8. A conclusão da sprint exige todos os critérios de aceite e tag/marco de release; **não relatar tarefa concluída sem evidência de build/teste**.

## Comandos esperados (somente quando existir solução implementada)

```powershell
dotnet --info
dotnet restore CartorioTwin.sln
dotnet build CartorioTwin.sln -c Release --no-restore
dotnet test CartorioTwin.sln -c Release --no-build
```

Se o ambiente não tiver Windows/SDK suportado para WPF, documentar impedimento e validar em agente Windows. **Não afirmar que WPF compilou em Linux** sem pipeline Windows comprovado.

## Primeira task recomendada

`S0-T04`: criar solução, projetos e testes vazios. Após isso `S1-T01`: implementar `Fracao` como tipo racional exato com testes. Após testes do núcleo determinístico, executar Sprints 3–5 **antes da integração final WPF**: extração/OCR, IA com schema/evidências e reconstrução. **Não deslocar o pipeline para pós-MVP.** A Sprint 5 já exige a fatia técnica integrada S5-T09; Sprint 8 exige CT-PDF-01…07 e CT-021…040, dossiê e recuperação. G9 exige avaliação independente; o MVP não pode ser declarado concluído antes dos gates.

## Contratos da baseline 1.2.0

- Documentos de produto governam escopo; ADRs registram decisões; AGENTS não redefine produto. Corrigir divergências em todos os arquivos e atualizar 12/changelog. A atualização documental não é evidência de build/implantação.
- Seguir as decisões de 03/10: uma matrícula por operação, fração do total, propriedade plena, movimentos simultâneos contra snapshot inicial. Não implementar encadeamento por iteração acidental.
- Completar persistência de atos/campos/revisões, extrações, eventos, lacunas, aprovação, revisões do cenário, jobs e artefatos. Evidência e fontes são N:M quando necessário; não reduzir tudo a uma única RevisaoHumana ligada a achado.
- OIDC/PKCE é o fluxo da baseline; IdP/capacidade/licenças são gates da Sprint 0. Não improvisar captura de senha, ignorar TLS ou enviar documento externo para contornar indisponibilidade local.
- Fila SQL durável; lease/heartbeat/fencing; idempotência em cada efeito; reconciliar SQL/storage e testar queda. Worker/extrator nunca aprovam fatos humanos.
- Verificar versão na publicação, não só no início; alterações relevantes de filhos incrementam revisão do pai. Execução + auditoria em commit SQL; conteúdo publicado imutável. Resultado tardio histórico não vira corrente.
- Hash canônico v1 e manifesto seguem 10, com fixtures. Hash não é assinatura; relógio/ID técnico da execução não alteram hash lógico. Não chamar IA para reproduzir estado já fixado.
- Diferenciar estado técnico, conclusão, completude, cobertura e validade. N/A só na regra; conjunto vazio não é consistente. Falta de pacote jurídico mantém modo técnico explícito se houver pacote técnico; falta deste bloqueia (422).
- Revisão de snapshot incompleto pode ser concluída sem comprovar dados; análise retorna inconclusivo sem projeção, salvo divergência confirmada que prevalece mantendo lacunas. Sem Portão B, não publicar simulação.
- Não considerar detecção de palavras de injection ou schema válido como defesa suficiente. Permissões e promoção de revisão ficam fora do modelo; evidência literal não garante contexto correto.
- Prioridades de dependências obrigatórias são P0. Implementar tasks adicionais de 07; construir fatia técnica até Sprint 5. Sem benchmark e critérios quantitativos definidos, G0 não encerra; sem restore e assinatura/compatibilidade, não distribuir ao piloto.
- Normas 213/2026 e 243/2026 citadas anteriormente permanecem não verificadas; não inventar correção, fundamento ou vigência. Curar fonte oficial antes de ativar regra.
- Medir recall de atos e omissão crítica, não apenas campos recuperados. Não usar holdout para ajustar prompts e depois apresentá-lo como independente. RTO/RPO são definidos antes de medir.
