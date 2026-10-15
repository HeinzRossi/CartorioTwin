# 04 — UI/UX detalhada — WPF .NET 10

**Versão:** 1.2.0 · **Data:** 15/10/2026 · **Estado:** especificação documental; implementação e homologação pendentes.

## 1. Princípios de design

- Interface profissional e sóbria para uso intensivo em cartórios, orientada a **tarefas e evidências**.
- Desktop Windows com MVVM, navegação por teclado e escalabilidade em monitores de balcão/retaguarda.
- Separação visual explícita entre **fato comprovado**, **dado revisado**, **hipótese** e **projeção**.
- Não esconder criticidades em ícones isolados; sempre combinar cor + texto + evidência.
- Mensagens transientes: *toast* para sucesso/informação técnica. Alertas jurídicos, divergências e bloqueios ficam **fixos na tela**, nunca só em toast.
- Não desenhar aparência que confunda o produto com serviço oficial de e-Notariado/ONR/SREI.

## 2. Design tokens iniciais (revisáveis)

| Token | Cor | Uso |
|---|---|---|
| `Brand.Navy900` | `#0F172A` | Navegação lateral, títulos |
| `Brand.Teal700` | `#0F766E` | Botões primários/links de ação |
| `Brand.Teal100` | `#CCFBF1` | Seleção discreta |
| `Surface.Base` | `#F8FAFC` | Plano de fundo |
| `Surface.Card` | `#FFFFFF` | Cards/tabelas/formulários |
| `Border.Default` | `#CBD5E1` | Bordas |
| `Text.Primary` | `#0F172A` | Texto principal |
| `Text.Secondary` | `#475569` | Auxiliar |
| `Feedback.Critical` | `#B91C1C` | Divergências críticas |
| `Feedback.Warning` | `#92400E` | Pontos de atenção |
| `Feedback.Success` | `#166534` | Êxito técnico |
| `Feedback.Info` | `#1D4ED8` | Informações |

Tipografia: **Segoe UI** (Windows), corpo 14–15 DIPs, título de seção 22–26 DIPs, título de tela 26–30 DIPs. Grid de espaçamento 4/8/12/16/24/32; raio 8 DIPs; alvos clicáveis preferencialmente >= 40×40 DIPs. Revisar contraste real (WCAG 2.2 AA) na execução e fornecer tema de alto contraste respeitando configurações Windows. Padrão inicial: tema claro com navegação escura; tema escuro opcional futuro.

## 3. Estrutura da aplicação visual

**Resolução de projeto:** 1366×768, com fallback em 1024×768 por abas/painéis alternados; DPI 100%/125%/150%; monitores 2K/4K. Medidas XAML são DIPs, não pixels físicos. Layout com `Grid` e `*` sizing, não coordenadas absolutas. Em DPI alto, não forçar PDF e três colunas simultaneamente; informação e ações continuam acessíveis em navegação alternada.

| Região do Shell | Medida inicial | Conteúdo/comportamento |
|---|---|---|
| Cabeçalho | Altura 64 DIPs | Marca, serventia ativa, ajuda e conta |
| Navegação lateral | Largura 224 DIPs, recolhível | Visão geral, operações, nova análise, relatórios e administração conforme perfil |
| Área principal | Largura restante com Grid | Breadcrumb, conteúdo da tarefa, scroll e painéis adaptáveis |
| Rodapé lateral | Automático | Versão e conexão, sem dados pessoais |

Documentos são acessados dentro da operação no MVP. Administração depende de permissão.

### Estrutura XAML (estilos separados)

```text
Desktop.Wpf/
  App.xaml
  Themes/
    Colors.xaml
    Typography.xaml
    Spacing.xaml
    Brushes.xaml
    Buttons.xaml
    TextInputs.xaml
    DataGrid.xaml
    Feedback.xaml
  Controls/
    StatusPill.xaml
    EvidenceBadge.xaml
    FractionInput.xaml
    InlineAlert.xaml
    ToastHost.xaml
    DocumentViewer.xaml
  Views/
    ShellView.xaml
    DashboardView.xaml
    OperacoesView.xaml
    NovaAnaliseView.xaml
    ProcessamentoView.xaml
    RevisarAtosView.xaml
    RevisarEstadoView.xaml
    ConferenciaDocumentalView.xaml
    SimulacaoView.xaml
    ResultadoView.xaml
    RelatorioView.xaml
    ConfiguracoesView.xaml
  ViewModels/
    ShellViewModel.cs
    OperacoesViewModel.cs
    NovaAnaliseViewModel.cs
    ProcessamentoViewModel.cs
    RevisarAtosViewModel.cs
    RevisarEstadoViewModel.cs
    ConferenciaDocumentalViewModel.cs
    SimulacaoViewModel.cs
    ResultadoViewModel.cs
    RelatorioViewModel.cs
```

`ResourceDictionary` em arquivos separados por propósito. Componentes devem ter estados `Normal`, `Hover`, `Focus`, `Disabled`, `ValidationError`; nenhum estilo global descontextualizado que altere elementos do Windows sem justificativa.

## 4. Inventário e especificação de telas

### T01 — Acesso e seleção de serventia

**Objetivo:** autenticar com método configurado e associar explicitamente a serventia autorizada.  **Campos:** usuário/identidade federada, controle de sessão, seletor se múltiplas serventias, opção sair. **Ações:** entrar, trocar serventia, ajuda técnica. **Estados:** autenticando, credencial negada, serviço indisponível, sessão expirada. Não exibir documentos ou operações antes de autenticar. Login com IdP (se houver) deve abrir fluxo confiável e não criar telas inseguras de captura de senha do provedor.

### T02 — Visão geral

**Conteúdo:** cards ‘Operações em conferência’, ‘Simulações desatualizadas’, ‘Achados pendentes de revisão’, ‘Operações concluídas no período’; lista das últimas operações com filtro, botão `Nova análise`. Não exibir porcentagem de “aprovação” jurídica. **Estados:** vazio com CTA; erro/retry; skeleton durante carga.

### T03 — Operações

**Layout:** título + `Nova análise` no canto superior direito; barra de filtro (tipo, situação, data, matrícula, responsável); grade com colunas `Operação`, `Tipo`, `Matrícula(s)`, `Estado`, `Atualizada em`, `Responsável`, `Ações`. Duplo clique/Enter abre operação; paginação virtualizada; exportação apenas por perfil. Estado vazio diferencia ‘sem dados’ de ‘nenhum resultado no filtro’.

### T04 — Nova análise (fluxo documental obrigatório)

**Etapa 1 — Identificação e importação:** compra e venda de uma matrícula, serventia de origem, número/CNM quando disponível e data/emissor ou ausência explícita. Adicionar matrícula PDF; minuta é opcional. Fontes técnicas em quarentena não abrem no visualizador. `Salvar rascunho` preserva contexto no servidor.

**Etapa 2 — Processamento (T04A):** etapas upload, varredura, texto/OCR, segmentação e interpretação; JobId, progresso conhecido ou indeterminado, motivo de espera/erro, `Solicitar cancelamento` e `Reprocessar`. Retentar consulta não cria job novo. Fechar a tela não cancela processamento aceito; reabrir consulta job persistido.

**Etapa 3 — Revisar atos (T05A / Portão A):** PDF à esquerda e linha do tempo R/AV à direita, com campos do ato selecionado. Ações confirmar/corrigir/contestar/não localizado; exibir valor sugerido e valor final, fonte, página, qualidade técnica e revisão. Ordenação de revisão e filtros não alteram ordem registral confirmada. Campo crítico sem evidência ou ato relevante não suportado conserva pendência/lacuna.

**Etapa 4 — Revisar estado (T05B / Portão B):** quadro reconstruído de titulares/frações, direitos/ônus tipados separados, linha do tempo, lacunas e fontes. `Concluir revisão da base documental` exige perfil, versão/hash e premissas. Base incompleta pode ter revisão concluída como incompleta; aviso explicita que simulação resultará inconclusiva e não terá projeção completa.

**Etapa 5 — Proposta:** selecionar alienantes/adquirentes revisados e lançar `Fração do total do imóvel`. Mostrar participação inicial ao lado. Assistente de cálculo de metade da quota de 1/2 mostra 1/4 e exige confirmação. Minuta fornecida passa pelo mesmo pipeline/revisão antes de comparação. Movimentos são simultâneos contra a base; compras no lote não financiam novas vendas.

**Etapa 6 — Simular e emitir:** resumo da revisão da proposta e snapshot, hashes e cobertura; execução aponta revisão exata. Resultado tardio histórico não ocupa painel corrente. Botão de dossiê recebe execução explícita; histórico tem faixa e ação próprias.

### T05 — Bancada documental e estados de revisão

Painéis redimensionáveis mantêm evidência selecionada sincronizada com o campo/ato. A referência usa versão da extração e página; highlight pode ser indisponível quando não houver geometria, com motivo visível e acesso à página. Campo OCR exige acesso à imagem original. Origem manual sem fonte é `Declarado sem evidência`, nunca fato documental comprovado só pela revisão.

A tela mostra contagens de campos pendentes, atos não suportados e conflitos, permitindo `Próxima pendência`. Não há aprovação cega em massa. A e B são decisões distintas; mesma pessoa pode cumpri-las apenas conforme política do servidor. Corrigir fonte/campo já confirmado invalida base e resultados dependentes.

Retomada recarrega versões do servidor. Se outro usuário revisou durante edição, apresentar conflito, versão corrente e valores locais, com opção de reaplicar correção após leitura; não sobrescrever silenciosamente. Limpar conteúdo visual, cache documental e seleção ao sair/trocar serventia.

Visualizador precisa de teclado e nomes acessíveis; transcrição/referências acompanham imagem quando o controle não expuser texto acessível. Alto contraste e DPI alto não podem esconder pendências ou o motivo de ação desabilitada.

### T06 — Simulação: Atual × Pretendido × Projetado

Exemplo CT-001, com base revisada e cenário selecionado:

| Atual documentado | Pretendido | Projetado condicional |
|---|---|---|
| João 1/2; Maria 1/2, com evidências | João → Carlos: 1/1 do total | Sem projeção; RT-004 — fração insuficiente |

Barra de ações: selecionar/duplicar cenário, editar proposta, executar e abrir dossiê da execução explícita. Achados e evidências ficam em painel persistente abaixo; fonte/revisão e validade permanecem visíveis. Em área estreita, as três visões passam a abas equivalentes.

**Ponto-chave:** quando falta evidência, a terceira coluna diz `INCONCLUSIVO` (não inventar proprietário). Se consistente no escopo, apresenta composição projetada e legenda **‘hipótese sem efeito registral’**. Botão para comparação A/B em versão futura; no MVP selecionar/duplicar cenários.

### T07 — Resultado e dossiê

Cabeçalho fixo com `CONSISTENTE NO ESCOPO / DIVERGENTE / INCONCLUSIVO`, tempo, usuário e versão de regra. Seções: `Resumo`, `Quadro atual`, `Movimentos`, `Projeção (quando calculável)`, `Achados`, `Fontes`, `Revisões`, `Limitações`, `Auditoria`. Ações: `Visualizar evidência`, `Revisar achado`, `Exportar dossiê`, `Nova simulação`. Rodapé obrigatório: ‘Documento de apoio, não é certidão, qualificação registral ou aprovação jurídica’.

### T08 — Administração e parâmetros

Perfis/usuários e vínculo com serventia, limites de upload, tipos de documentos, regras habilitadas e versões, política de retenção, integração (se autorizada), estado do serviço, auditoria técnica. **Não permitir criar regra jurídica em texto livre executável sem validação de especialista.**

## 5. Mapa de navegação

```text
Login -> Shell
           ├─ Visão geral -> Operação -> [Conferência] -> [Simulação] -> [Resultado]
           ├─ Operações   -> Operação
           ├─ Nova análise -> Importar -> Processar -> Revisar Atos -> Revisar Estado -> Propor -> Simular
           ├─ Relatórios -> Dossiê existente
           └─ Administração -> Usuários | Regras | Auditoria
```

WPF navega por `INavigationService`/ViewModel locator (DI), sem `new Window()` diretamente nos casos de uso. Evitar múltiplas janelas modais aninhadas; preferir painéis contextuais. `Esc` fecha diálogos/menus sem perder rascunho; `Ctrl+S` salva rascunho quando permitido, `Ctrl+Enter` só simula com confirmação de pré-condições, `F1` abre ajuda contextual.

## 6. Regras para estados e feedback

| Estado | Apresentação | Ação |
|---|---|---|
| `Loading` | Skeleton/progresso identificável | Cancelar onde possível |
| `Empty` | Explicação e CTA específico | Criar operação ou adicionar fonte |
| `Error` | Mensagem humana + código rastreável | Tentar novamente sem duplicar operação |
| `ValidationError` | Erro junto ao campo e resumo acessível | Corrigir; manter dados digitados |
| `Success` | Toast curto | Seguir para etapa relevante |
| `Inconclusivo` | Banner persistente com dados faltantes | Incluir/validar evidência |
| `Divergente` | Banner persistente + regra e origem | Editar hipótese ou encaminhar revisão |
| `Stale` | Faixa ‘Simulação desatualizada’ | Reexecutar com snapshot vigente |
| `Disabled` | Ação desabilitada acompanhada do motivo | Exibir requisitos |

## 7. Critérios de aceite visuais

- O usuário distingue os 3 estados sem depender exclusivamente de cor.
- Informações de origem e hash da simulação podem ser alcançadas via teclado.
- Texto/campos não sobrepõem em 125% e 150% de escala de DPI.
- Contraste checado com ferramenta automatizada e revisão manual.
- Listas grandes virtualizadas e cancelamento de tarefas longas sem congelar a UI.
- `Fonte`, `Pretendido` e `Projetado` não compartilham o mesmo rótulo visual.
- Nenhuma ação de cadastro ou revisão usa mensagens de erro apenas em toast.

## 8. Sinais de qualidade na primeira prototipação

Construir wireframe em XAML (não imagem estática), primeiro com dados sintéticos CT-001 a CT-005. Validar fluxos de navegação com dois conferentes, um tabelião e um registrador antes de acoplar OCR/IA. Testar telas T04, T06 e T07 como prioridade.

## 9. Contratos de feedback adicionais

Mostrar separadamente: falha técnica do job, conclusão documental, completude, cobertura técnica/jurídica limitada e validade corrente/histórica. `Concluído` no progresso não equivale a `Consistente no escopo`. Sem pacote jurídico: faixa `Verificações técnicas — sem cobertura jurídica homologada`. Sem regras mínimas ou com lacunas: inconclusivo persistente.

Testar T04A/T05A/T05B e simulação em teclado, alto contraste, 125%/150%, layout reduzido, leitor de tela, sessão expirada, fechamento/reabertura e concorrência. Mensagens de erro não apagam campos locais; rascunho não salvo é indicado explicitamente. Próxima ação sempre informa sua pré-condição, sem mensagens de implementação desnecessárias ao usuário.
