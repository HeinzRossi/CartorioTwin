# 01 — Produto, requisitos e escopo

**Versão:** 1.2.0 · **Data:** 15/10/2026 · **Estado:** especificação documental; implementação e homologação pendentes.

## 1. Problema e posicionamento

Profissionais de RI e TN confrontam matrículas, títulos, procurações e documentos de qualificação. Divergências entre esses elementos podem ser percebidas tarde, gerando revisão, retrabalho e riscos. O Cartório Twin fornece **conferência preventiva rastreável** e **simulação matemática/documental de estados**, sem substituir o sistema de gestão nem tomar decisão jurídica.

### Promessa verificável

> Dada uma situação registral documentada e uma operação pretendida, o Cartório Twin mostra o que se sabe, o que falta, o que diverge e qual seria o estado projetado **se** a hipótese de transmissão for cumprida e adequadamente formalizada/registrada.

### Não prometer

- Aprovação de títulos, validade jurídica certificada, 'probabilidade de registro garantido' ou eliminação de exigências.
- Consulta automática de todos os ônus, todos os titulares ou a situação atual oficial sem fontes legítimas.
- Substituição de assinaturas, certidões, sistemas oficiais ou atribuições legais.

## 2. Público-alvo e personas

| Persona | Objetivo | Necessidade | Limite de permissão |
|---|---|---|---|
| Atendente/triagem (TN/RI) | Criar operação e anexar documentos | Entrada fácil e pendências claras | Não aprova revisão jurídica |
| Escrevente/conferente | Conferir dados e simular | Comparar fontes e justificar alertas | Pode validar dados sob alçada |
| Tabelião/registrador/substituto | Revisar achados e encerramentos | Evidências, versão de regra e auditabilidade | Decisões jurídicas permanecem humanas |
| Administrador da serventia | Gerenciar usuários, segurança, retenção | RBAC, relatórios e políticas | Não altera trilha imutável |
| Suporte de software autorizado | Diagnosticar sem acessar acervo | Telemetria técnica anonimizada | Sem acesso automático a documentos |

## 3. Jornada ponta a ponta (MVP)

1. Autenticar, selecionar serventia e criar análise de compra e venda de uma matrícula. Fontes e minuta podem ser múltiplas, respeitando o mesmo contexto.
2. Importar PDF textual, digitalizado ou misto; registrar origem, emissor/data conhecidos ou ausência explícita. Fonte fica em quarentena até liberação técnica.
3. Acompanhar job durável de extração/OCR por página, segmentação e interpretação de atos.
4. **Portão A:** conferir atos/campos contra evidências; confirmar, corrigir ou contestar. Entrada manual é correção/contingência, sempre rotulada.
5. Reconstruir estado documentado com eventos revisados, frações, direitos, linhagem e lacunas.
6. **Portão B:** concluir revisão do estado para versão/hash específicos. Estado incompleto pode ser revisado como incompleto; aprovação da revisão não comprova fatos ausentes.
7. Informar transmissões e, quando fornecida, revisar campos da minuta antes de comparar. Todas as frações de proposta referem-se ao total do direito de propriedade no imóvel.
8. Executar simulação atômica contra o snapshot inicial; incompletude essencial produz inconclusivo sem projeção completa.
9. Comparar Atual documentado / Pretendido / Projetado condicional; revisar achados preservando a saída original.
10. Publicar resultado somente se versões ainda vigentes; emitir dossiê da execução explícita, com cobertura, limites e evidências. Alterações geram nova revisão e obsolescência rastreável.

## 4. Escopo funcional

### No MVP — prioridade P0

| ID | Requisito | Critério observável |
|---|---|---|
| RF-001 | Login, sessão e perfil por serventia | Acesso negado fora do vínculo com serventia |
| RF-002 | Cadastro/listagem de operações | Criar, salvar rascunho, pesquisar, reabrir com histórico |
| RF-003 | Identificação de matrícula/imóvel | Uma matrícula por operação; serventia de origem e data/emissor ou ausência explícita; ausência essencial impede consistência |
| RF-004 | Inclusão de fontes documentais | Upload com tipo, tamanho e hash, sem sobrescrever original |
| RF-005 | Revisão do estado reconstruído (Portão B) | Titulares, frações e lacunas conferidos; aprovação vinculada à versão/hash; manual somente como correção/contingência |
| RF-006 | Definição de transmissões | Um ou vários alienantes/adquirentes na mesma matrícula; fração do total explícita |
| RF-007 | Motor determinístico | Não permite fracionamento maior que disponibilidade |
| RF-008 | Cenários independentes e versionados | Revisão de cenário imutável; retry idempotente reutiliza resposta, nova execução intencional preserva histórico |
| RF-009 | Alertas com evidência | Motivo, dados comparados, regra, gravidade, proveniência |
| RF-010 | Três estados comparáveis | Atual documentado, pretensão, projeção condicional |
| RF-011 | Dossiê da execução explícita | HTML imprimível obrigatório no MVP; PDF nativo opcional; manifesto, cobertura, revisão e ressalvas; dossiê histórico rotulado |
| RF-012 | Auditoria e rastreio | Ator, ação, tempo UTC, operação, mudança e correlação |
| RF-013 | Estados de tela e tratamento de erro | Loading/empty/error/success; validação inline + alertas persistentes |
| RF-014 | Governança de decisões | Revisões de campo/ato (Portão A), estado (Portão B) e achado separadas; mesma pessoa pode cumprir A/B no piloto se política permitir, dupla revisão configurável no servidor |
| RF-015 | Segmentação e revisão de atos | R/AV, referências, campos e candidatos preservados; desconhecidos não desaparecem |
| RF-016 | Reconstrução documental | Replay de revisões fixadas, eventos suportados, lacunas e deduplicação supervisionada |
| RF-017 | Jobs recuperáveis | Status, tentativas, cancelamento, retomada e reprocessamento idempotentes |
| RF-018 | Publicação concorrente | Resultado tardio permanece histórico; não substitui revisão corrente |
| RF-019 | Manifesto e proveniência | Hash canônico, versões e entradas completas permitem replay sem chamar IA |
| RF-020 | Cobertura explícita | Pacote técnico obrigatório; jurídico opcional homologado por jurisdição; nenhuma regra executada não gera consistente |

### Componentes documentais obrigatórios — P0 (IDs preservados)

| ID | Requisito | Marco |
|---|---|---|
| RF-101 | Extração de PDF com texto nativo | P0 — evidência por página e versão de extração |
| RF-102 | OCR de PDFs digitalizados/mistos | P0 — seleção por página, limites, revisão e fallback explícito |
| RF-103 | Comparação matrícula × minuta quando fornecida | P0 — campos revisados, identidade e extensão documentais; minuta não é obrigatória para criar proposta |
| RF-109 | IA/parser para sugerir interpretação | P0 — saída candidata, schema/evidência/contexto validados, revisão obrigatória |

### Incrementos P1/P2

| ID | Requisito | Marco |
|---|---|---|
| RF-104 | Regras de representação/procuração | P2 — depende de matriz normativa aprovada |
| RF-105 | Usufruto e nua-propriedade | P2 — tipos de direitos separados, sem somar percentuais incompatíveis |
| RF-106 | Alienação fiduciária/hipoteca | P2 — primeiro como alertas documentais, depois simulações específicas |
| RF-107 | Permuta/múltiplas matrículas | P2 — transação projetada em vários imóveis |
| RF-108 | Integrações com sistemas cartorários | P2 — somente fornecedores que disponibilizarem interface autorizada |
| RF-110 | Comparação de versões/cenários e fila de revisão | P1 — produtividade medida; sem reaproveitar confirmação automaticamente em fonte diferente |

### Fora do escopo inicial

- Protocolo, prenotação, registro, averbação ou lavratura automática em acervo oficial.
- Emissão de certidão, validação de assinatura eletrônica oficial ou substituição de centrais eletrônicas.
- Consulta irrestrita a bases externas, vigilância de cidadãos, pontuação de pessoas ou decisões automatizadas sobre suspeita/fraude.
- Motor fiscal/tributário definitivo, cálculo oficial de emolumentos, interpretação de toda a legislação estadual.
- Aplicativo mobile, site público de cliente ou componente em nuvem mandatória.

## 5. Requisitos não funcionais

| ID | Requisito | Meta inicial para piloto (validar em testes) |
|---|---|---|
| RNF-001 | Segurança | TLS 1.2+ em rede; TLS 1.3 quando suportado; segredos apenas no serviço |
| RNF-002 | Autorização | Escopo TenantId/serventia em toda consulta; testes negativos cross-tenant |
| RNF-003 | Privacidade | Sem envio a provedores externos por padrão; retenção configurável e auditada |
| RNF-004 | Rastreabilidade | Toda conclusão vincula snapshot + versão de regra + fontes + revisor |
| RNF-005 | Performance | Simulação estrutural de até 100 movimentos em <1 s p95 em ambiente de referência |
| RNF-006 | Uso de recursos | UI permanece responsiva ao importar PDF; tarefas canceláveis |
| RNF-007 | Confiabilidade | Alterar entradas invalida resultados; execução repetida produz saída equivalente |
| RNF-008 | Acessibilidade | Navegação por teclado, foco visível, zoom 125/150%, texto legível |
| RNF-009 | Observabilidade | Logs estruturados sem conteúdo de documento nem CPF em claro |
| RNF-010 | Portabilidade | Desktop Windows; backend instalável na infraestrutura da serventia |
| RNF-011 | Manutenibilidade | Testes de domínio sem banco nem WPF; interfaces e DI |
| RNF-012 | Resiliência | Erros de serviço não apagam rascunho persistido; retry seguro em requisições idempotentes |

Métricas de implantação piloto propostas: (a) 100% das divergências do conjunto de regressão detectadas; (b) 0 alterações no acervo oficial; (c) nenhuma promoção automática de candidato a fato; erros/omissões medidos no corpus delimitado; (d) tempo médio de revisão/triagem comparado à linha de base, sem promessa de ganho fixo.

## 6. Regras para confiança e transparência

- **Fonte**: fato vinculado a documento específico, versão/hash, data e trecho/página quando aplicável.
- **Inferência**: hipótese rotulada, jamais apresentada como fato oficial.
- **Ausência**: sem documento suficiente não há consistência/projeção completa; sem divergência confirmada => `INCONCLUSIVO`. Divergência confirmada prevalece com lacunas explícitas.
- **Escopo**: resultado vale para documentos e regras efetivamente analisados, na data da análise.
- **Revisão**: usuário habilitado pode aceitar/contestar um alerta com justificativa, sem reescrever a saída original do motor.

## 7. Riscos do produto

| Risco | Consequência | Mitigação/validação |
|---|---|---|
| Titularidade extraída incorretamente | Alerta falso ou omissão | Revisão obrigatória; evidência por campo; testes com documentos variados |
| Matrícula desatualizada | Cenário baseado em estado obsoleto | Data da certidão visível e regra de 'fonte potencialmente desatualizada' |
| Direito real reduzido a percentual | Simulação juridicamente enganosa | Modelo tipado; MVP limitado à propriedade/fração; fase posterior específica |
| Regra estadual ausente | Recomendações inadequadas | Pacotes jurídicos versionados; aviso de cobertura e aprovação local |
| Uso indevido do dossiê como parecer | Risco operacional/jurídico | Marca d'água e disclaimers obrigatórios; nomenclatura 'Análise auxiliar' |
| Falta de acesso a APIs de fornecedores | Bloqueia automação | Importação manual e conectores opcionais desde a arquitetura |

## 8. Protocolo de validação com usuários

Antes de concluir a Sprint 0, entrevistar profissionais de pelo menos 2 RI e 2 TN. Apresentar CT-001, verificar documentos mais recorrentes, regras praticadas por UF, fluxo de autorização, infraestrutura local, política de guarda e impacto percebido. Registrar as conclusões no backlog, sem presumir interoperabilidade autorizada.

## 9. Indicadores de qualidade comercial e funcional

- **Precisão operacional**: alertas relevantes confirmados / alertas avaliados; reportar por regra.
- **Cobertura**: tipos de operações e verificações efetivamente suportadas.
- **Inconclusivos**: quantidade por falta de fonte ou ambiguidade (não ocultar).
- **Omissão documental**: recall de atos críticos, com denominador anotado, formato e tipo de ato.
- **Avaliação independente**: corpus de ajuste separado do corpus de avaliação; limiares pactuados antes do piloto (11).
- **Tempo de conferência**: comparar com e sem ferramenta, por complexidade de caso.
- **Rastreabilidade**: % de afirmações apoiadas por evidência e versão de regra.
- **Adoção**: cenários concluídos, descartados, revisados e razões registradas.

## 10. Requisitos operacionais complementares

| ID | Obrigação | Aceite |
|---|---|---|
| RNF-013 | Durabilidade e publicação segura | Queda/reentrega não perdem job nem duplicam fatos; lease e versão de publicação verificados |
| RNF-014 | Recuperação integrada | RTO/RPO definidos antes do ensaio; banco, arquivos, artefatos e chaves restaurados com manifesto |
| RNF-015 | Avaliação verificável | Corpus independente, métricas estratificadas, contagens e critérios de go/no-go registrados |
| RNF-016 | Distribuição e compatibilidade | Binários/instalador assinados, SBOM, versões cliente/API/schema e rollback documentados |
| RNF-017 | Contenção e privacidade dos provedores | Sandbox, quotas, mínimos privilégios e tráfego externo bloqueado sem opt-in autorizado |

O detalhamento desses contratos está em 10 e 11. O piloto exige todas as obrigações P0, sem converter autorização de processamento em autorização de envio externo.
