# 06 — Plano de testes, qualidade e aceite

**Versão:** 1.2.0 · **Data:** 15/10/2026 · **Estado:** especificação documental; implementação e homologação pendentes.

## 1. Estratégia

Priorizar testes do **domínio puro** antes de WPF/SQL Server. Suíte determinística, com dados sintéticos. Usar xUnit (ou NUnit mediante decisão da equipe), testes de API com `WebApplicationFactory`, testes de integração EF com SQL Server de teste e UI automatizada somente para fluxos críticos.

### Camadas de testes

| Camada | O que validar | Exigência de aceite |
|---|---|---|
| Unitários/Property-based | `Fracao`, normalização, soma, comparações, imutabilidade, limites | Sem erro matemático, overflow silencioso ou estado mutado |
| Regras | Cada RT-001..RT-010 e tabela de decisão de 03 com dados válidos, inválidos e incompletos | Resultado, código e evidência estáveis |
| Simulação | CT-001..CT-008 e invariantes | Sem projeção parcial após falha |
| Application | Revisão, hash, versionamento, permissões | Mudança invalida saída anterior |
| API/Integração | Rotas, serialização, `If-Match`, multi-tenancy | Acesso cruzado impossível nos testes |
| UI ViewModels | Commands, validação, loading/error/retry | Sem lógica de domínio na UI |
| UI E2E | PDF textual/escaneado → atos → Portões A/B → proposta → simulação → dossiê | Fluxo completo reproduzível |
| Segurança | OWASP para API, importação de arquivo, session/tokens | Sem falhas críticas conhecidas |
| Usabilidade | Teste com usuários de RI e TN | Encontrar evidência e corrigir cenário sem orientação |

## 2. Casos mínimos de teste

| Caso | Entrada | Esperado |
|---|---|---|
| CT-001 | João 1/2 tenta vender 1/1 | Divergente; RT-004; sem estado projetado |
| CT-002 | João vende 1/2 | Carlos 1/2 + Maria 1/2; estado original imutável |
| CT-003 | João 1/2 + Maria 1/2 vendem a Carlos | Carlos 1/1, no escopo |
| CT-004 | Três titulares 1/3; A e B vendem a D | D 2/3 + C 1/3 exatos |
| CT-005 | Outro titular não identificado | Inconclusivo; não inventar totalidade |
| CT-006 | Nomes semelhantes sem vínculo validado | Não realizar *merge* de identidade |
| CT-007 | Alterar snapshot após execução | Relatório anterior histórico/desatualizado; nova execução necessária |
| CT-008 | Lote com segunda operação impossível | Sem efeito parcial; divergente |
| CT-009 | Mesmo cenário executado duas vezes sem mudanças | Hash/status/projeção equivalentes; novos metadados de execução podem diferir |
| CT-010 | Fração de transmissão zero, negativa ou >1 | Movimento rejeitado; saldo zero continua válido no domínio |
| CT-011 | Frações cujo denominador gera possível overflow | Operação rejeitada/controlada, sem wrap-around |
| CT-012 | Outro usuário da mesma serventia sem papel de revisor | Sem direito a revisão técnica/jurídica |
| CT-013 | Usuário de outra serventia consulta ID conhecido | `404`/`403` sem vazamento de dados |
| CT-014 | Troca de documento por outro com mesmo nome | Hash e versões diferentes; resultado obsoleto |
| CT-015 | PDF malformado/executável renomeado | Upload rejeitado/quarentena; sem execução arbitrária |
| CT-016 | Enviar conteúdo com instrução 'ignore regras' dentro de PDF | Extração trata como dado; não muda políticas |
| CT-017 | API indisponível no meio da operação | UI exibe falha clara; rascunho persistido não é perdido |
| CT-018 | Região sem pacote jurídico, mas com pacote técnico aprovado | Modo técnico explícito; sem pacote técnico retorna 422; nenhum rótulo de cobertura jurídica |
| CT-019 | Dossiê emitido | Referências, hashes, ressalvas e regras visíveis |
| CT-020 | Reexecutar com pacotes de regras diferentes | Resultado contém versão correta; execuções anteriores preservadas |

## 3. Exemplo de teste unitário C# (ilustrativo)

A implementação de produção deverá fornecer `Fracao.Criar`, `MatriculaSnapshot`, `SimuladorCompraVenda` e suas fábricas. O trecho abaixo define comportamento, **não compõe ainda um projeto compilável**:

```csharp
[Fact]
public void Joao_com_metade_nao_pode_transferir_inteiro()
{
    var baseDocumental = CenarioFixture.ComTitulares(
        ("Joao", Fracao.Criar(1, 2)),
        ("Maria", Fracao.Criar(1, 2)));
    var proposta = CenarioFixture.Venda("Joao", "Carlos", Fracao.Criar(1, 1));

    var entrada = CenarioFixture.EntradaRevisada(baseDocumental, proposta);
    var resultado = new SimuladorCompraVenda().Simular(entrada);

    Assert.Equal(StatusAnalise.Divergente, resultado.Status);
    Assert.Contains(resultado.Achados, a => a.CodigoRegra == "RT-004");
    Assert.Null(resultado.EstadoProjetado);
    Assert.Equal(Fracao.Criar(1, 2), baseDocumental.FracaoDe("Joao"));
}
```

Testes de propriedade devem contemplar normalização `(2/4 == 1/2)`, soma `(1/3+1/3==2/3)`, conservação da participação (total anterior = total posterior para transmissão simples de mesma espécie), não negatividade e identidade do cenário base.

## 4. Dados de ensaio

- Matrículas e minutas **sintéticas** em PDF e JSON; nomes inventados, números fictícios.
- Base dourada (`golden dataset`) versionada com expectativa validada por profissionais de RI/TN, sem dados pessoais reais.
- Casos obrigatórios de OCR no MVP: digitalização em baixa qualidade, marca d'água, documento rotacionado, texto em duas colunas e histórico de atos extenso.
- Massa de performance: 1, 10, 100 movimentos; 1, 10 e 50 fontes; medições da máquina-alvo documentadas.

## 5. Segurança e qualidade de release

**Critérios de bloqueio da entrega:** teste cruzado de serventia falhou; resultado sugere ‘apto’ juridicamente; exposição de documentos em log; simulação altera estado documental base; dossiê sem versão/hash; varredura de segurança crítica não tratada; restore de backup não demonstrado antes do piloto.

Checklist mínimo por PR: build limpo, análise estática, testes unitários e de integração, migração revisada, teste de autorização, documentação atualizada e revisão por outro desenvolvedor. Release com versionamento semântico, changelog e nota de alteração normativa quando aplicável.

## 6. Aceite por Sprint (mesmos gates de 07)

| Sprint | Gate | Evidência mínima |
|---|---|---|
| 0 | G0 | Entrevistas, cobertura, corpus, benchmark local, IdP/topologia, limiares e responsáveis definidos |
| 1 | G1 | Tipos, atos/evidências, modelos, schema/migrações, autorização e canonicalização testados |
| 2 | G2 | CT-001…004/008, tabela de decisão, API e isolamento inicial |
| 3 | G3 | Texto/OCR/mistos, quarantine, jobs duráveis e restart/lease/fencing |
| 4 | G4 | Schema, evidência contextual, revisão de atos e métricas no conjunto de ajuste |
| 5 | G5 | Replay, deduplicação, Portão B via API, publicação condicionada e fatia técnica integrada |
| 6 | G6 | Bancada WPF, dois portões, retomada, acessibilidade e conflitos de revisão |
| 7 | G7 | Proposta/minuta quando fornecida, regras/cobertura e movimentos inequívocos |
| 8 | G8 | E2E CT-PDF-01…07, CT-021…040, dossiê, isolamento, restore e distribuição pré-piloto |
| 9 | G9 | Holdout independente, segurança/jurídico aplicável, métricas e decisão go/no-go documentada |

## 7. Casos adicionais obrigatórios

| Caso | Entrada/falha | Esperado |
|---|---|---|
| CT-021 | Job inicia revisão 4, revisão 5 publicada antes do fim | Saída histórica; ponteiro corrente intacto; motivo visível |
| CT-022 | Queda entre promoção do arquivo e commit SQL | Reconciliação recupera ou coloca em intervenção; nenhum job perdido silenciosamente |
| CT-023 | Reentrega, lease expirado e worker antigo termina | Fencing impede publicação antiga; um efeito visível por checkpoint |
| CT-024 | Duas revisões humanas concorrentes | If-Match falha com 412; ausência de precondição 428; sem sobrescrita |
| CT-025 | Quadro declarado completo soma 3/4 | Divergente RT-005, sem projeção; declarado incompleto gera inconclusivo |
| CT-026 | Metade da quota de 1/2 | UI confirma conversão para 1/4 do total; contrato ambíguo é rejeitado |
| CT-027 | A→B e B→C no mesmo lote, B sem saldo inicial | Divergente RT-004; aquisição no lote não financia saída |
| CT-028 | Evidência real de pessoa/ato errado | Candidato não é confirmado automaticamente; contexto exige correção/revisão |
| CT-029 | Nenhuma regra mínima executada, todas N/A | Inconclusivo, nunca consistente por conjunto vazio |
| CT-030 | PDF misto com texto incompleto/rotação | Rota por página e lacunas explícitas; sem fonte vazia tratada como completa |
| CT-031 | Reimportar certidão e fontes com atos sobrepostos | Sem dupla aplicação; correspondência revisada; conflito não resolvido inconclusivo |
| CT-032 | Cancelamento parcial/ambíguo ou salto de numeração | Não extingue direito nem prova folha ausente automaticamente; revisão/lacuna |
| CT-033 | Troca de serventia e tentativa de abrir job/derivado de outra operação | Limpeza da UI/cache e negação no backend, sem revelar conteúdo |
| CT-034 | Estouro de CPU/memória/tempo e antivírus indisponível | Processo contido; API preservada; quarentena não liberada |
| CT-035 | Retry mesma chave com payload igual/diferente | Igual reutiliza recurso/resposta; diferente retorna 409 |
| CT-036 | Mesma entrada serializada com diferenças irrelevantes; mudança relevante | Hash canônico estável no primeiro caso, diferente no segundo; datas técnicas excluídas |
| CT-037 | Promover aprovação por JSON confirmed=true ou worker | Campo ignorado/rejeitado e autorização nega; só revisão humana autenticada confirma |
| CT-038 | Snapshot revisado incompleto e divergência confirmada coexistentes | Divergente com completude incompleta; preserva lacunas, sem projeção |
| CT-039 | Dossiê corrente fica obsoleto; restauração integrada | Download histórico sinalizado; restore valida arquivos/chaves/manifesto e RTO/RPO pactuados |
| CT-040 | Retomada WPF, DPI alto, teclado e sessão expirada | Progresso do servidor preservado; painel alternado acessível; ações respeitam sessão |

Casos de falha usam pontos de injeção controlados e dados sintéticos. Testar ambos os lados da corrida de cancelamento/publicação. CT-039 tem subcenários independentes de dossiê e recuperação; executar ambos.

## 8. Critério de encerramento do MVP

Usuário importa PDF textual/escaneado, recebe candidatos com evidências, revisa atos, obtém/revisa estado e lacunas, informa proposta/minuta quando houver, simula e emite dossiê rastreável. Incompletude essencial não gera projeção completa; nenhum candidato é confirmado pela IA; restart/concorrência não promovem resultado antigo. Corpus e limiares são os de 11, aprovados antes da avaliação. Todos os gates G0…G9 devem ter evidências; documentação não equivale a execução.

O conjunto de avaliação fica separado do ajuste; registrar recall de atos, erros contextuais, omissões não sinalizadas e tempo humano por formato/tipo. Atualização de prompt/modelo exige regressão, não apenas build. Build/testes WPF são demonstrados em agente Windows; este pacote não contém uma solução implementada nem resultados de execução.


