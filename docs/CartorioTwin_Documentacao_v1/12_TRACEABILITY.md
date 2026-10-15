# 12 — Rastreabilidade e incorporação das melhorias

**Versão:** 1.2.0 · **Data:** 15/10/2026 · **Estado:** rastreabilidade documental; implementação e testes da aplicação pendentes.

## 1. Requisitos funcionais

Os IDs originais foram preservados. RF-101/102/103/109 passaram a P0; RF-015…020 detalham obrigações do pipeline. RF-110 é evolução pós-MVP. Tasks de 07 são o plano de execução, não evidência de conclusão. T04A/T05A/T05B são detalhados em 04.

| Requisito | Resultado esperado | ADR | Tasks | Testes de referência | Tela/contrato |
|---|---|---|---|---|---|
| RF-001 | Login e sessão | ADR-014 | S1-T08/S6-T01/S6-T08 | CT-012/013/033/040 | T01; identidade/policies |
| RF-002 | Operações e histórico | ADR-003/013 | S1-T07/S2-T06/S6-T01 | CT-017/021/024 | T02/T03; operações |
| RF-003 | Identificação de matrícula | ADR-015 | S1-T01/S1-T07/S3-T01 | CT-006/013/031 | T04; matrícula/fontes |
| RF-004 | Fontes imutáveis | ADR-009/012 | S3-T01/S3-T07/S3-T08 | CT-014/015/022/034 | T04/T04A; fontes |
| RF-005 | Portão B | ADR-013/016 | S5-T05/S5-T08/S6-T06 | CT-005/007/021/038 | T05B; aprovações |
| RF-006 | Transmissões | ADR-005/015 | S2-T03/S7-T01 | CT-002/003/026/027 | T06; revisões de cenário |
| RF-007 | Motor exato | ADR-004/005 | S2-T01/S2-T02/S2-T03 | CT-001/004/008/011/025 | T06; simulações |
| RF-008 | Cenários/retry | ADR-013/015 | S2-T05/S2-T06/S7-T01 | CT-007/009/035/036 | T06; cenário/revisões |
| RF-009 | Achados e evidências | ADR-007/013 | S2-T02/S7-T03/S8-T02 | CT-001/019/028/038 | T06/T07; achados |
| RF-010 | Atual/Pretendido/Projetado | ADR-007/016 | S8-T01/S8-T02 | CT-001/002/005/038 | T06; resultado |
| RF-011 | Dossiê explícito | ADR-013/018 | S8-T03/S8-T07 | CT-019/039 | T07; dossiês da execução |
| RF-012 | Auditoria | ADR-009/013 | S1-T07/S5-T08/S8-T04 | CT-014/021/024/037 | T08; auditoria/publicação |
| RF-013 | Estados de tela | ADR-008/016 | S6-T02/S6-T07/S6-T08 | CT-017/033/040 | T04A/T05A/T05B/T06 |
| RF-014 | Revisões separadas | ADR-013/014 | S4-T06/S6-T04/S6-T06 | CT-012/024/037/038 | T05A/T05B/T07; revisões |
| RF-015 | Atos/campos | ADR-006/013 | S4-T01/S4-T03/S4-T08 | CT-PDF-01/06; CT-028 | T05A; atos/evidências |
| RF-016 | Replay e lacunas | ADR-004/006/015 | S5-T01/S5-T02/S5-T07/S5-T08 | CT-031/032/038; CT-PDF-04/06 | T05B; reconstruções |
| RF-017 | Jobs duráveis | ADR-012 | S3-T05/S3-T08 | CT-022/023/034/035 | T04A; jobs/cancelamentos |
| RF-018 | Publicação segura | ADR-013 | S1-T09/S5-T08 | CT-021/023/024 | T05B/T06; publicação |
| RF-019 | Manifesto/proveniência | ADR-013 | S1-T02/S1-T09/S5-T05/S8-T03 | CT-009/019/036 | T07; manifesto |
| RF-020 | Cobertura e agregação | ADR-010/016 | S0-T10/S2-T04/S7-T06 | CT-018/020/029/038 | T06/T07; regras |
| RF-101 | Texto nativo P0 | ADR-006/012 | S0-T08/S3-T02/S3-T06 | CT-PDF-01 | T04A/T05A; extração |
| RF-102 | OCR/mistos P0 | ADR-006/012 | S0-T08/S3-T03/S3-T04 | CT-PDF-02; CT-030/034 | T04A/T05A; OCR |
| RF-103 | Minuta quando fornecida P0 | ADR-006/015 | S7-T02/S7-T05 | CT-006/028; CT-PDF-03/07 | T05A/T06; comparação |
| RF-109 | Interpretação candidata P0 | ADR-006/017 | S4-T02/S4-T04/S4-T05/S4-T08 | CT-016/028/037; CT-PDF-05 | T05A; interpretação |
| RF-104 | Representação/procuração P2 | ADR-010 | Pós-MVP: task a detalhar antes de ativar | RFJ-102: suíte futura especializada | Cobertura futura; não ativa |
| RF-105 | Usufruto/nua-propriedade P2 | ADR-010/015 | Pós-MVP: task a detalhar antes de ativar | RFJ-104: suíte futura especializada | Fatos tipados no MVP; simulação futura |
| RF-106 | Garantias específicas P2 | ADR-010/015 | Pós-MVP: task a detalhar antes de ativar | RFJ-105: suíte futura especializada | Alertas documentais no MVP; efeitos futuros |
| RF-107 | Permuta/matrículas múltiplas P2 | ADR-015 | Pós-MVP: task a detalhar antes de ativar | RFJ-106: suíte futura especializada | Fora do MVP de uma matrícula |
| RF-108 | Conectores autorizados P2 | ADR-003/009 | Pós-MVP: contrato/provedor a definir | Contrato/isolamento do conector antes de ativar | Sem integração externa presumida |
| RF-110 | Produtividade P1/P2 | ADR-013/017 | E1-T01/E1-T02/E1-T03/E1-T04 | Regressão e medição pós-G9 | Comparação de versões/cenários e fila |

RF-104…108 permanecem fora do MVP. Sua ativação exige backlog específico, especialista, contrato e testes antes de implementação; as referências RFJ indicam domínio de teste futuro, não casos já executáveis. RF-103 tem fixture CT-PDF-07 nas tasks S7-T02/T05; cobre sujeitos, frações e descrições revisados. CT-PDF-03 permanece a conferência da proposta com o estado revisado.

## 2. Requisitos não funcionais

| Requisito | Obrigação | ADR | Tasks | Evidência de aceite planejada |
|---|---|---|---|---|
| RNF-001 | TLS e segredos | ADR-003/014 | S0-T05/S1-T08/S8-T07 | CT-013/033/034 |
| RNF-002 | Autorização | ADR-009/014 | S1-T07/S1-T08/S8-T04 | CT-012/013/033/037 |
| RNF-003 | Privacidade | ADR-009/014/018 | S0-T05/S4-T04/S8-T04 | CT-016/033/034 |
| RNF-004 | Rastreabilidade | ADR-013 | S1-T02/S1-T09/S5-T05/S8-T03 | CT-019/036 |
| RNF-005 | Performance | ADR-017 | S0-T08/S0-T09/S8-T06 | Massa 1/10/100 movimentos e benchmark de 11 |
| RNF-006 | Recursos/UI | ADR-008/012 | S0-T08/S3-T08/S6-T08 | CT-030/034/040 |
| RNF-007 | Reprodução/invalidação | ADR-004/013 | S2-T05/S5-T06/S5-T08 | CT-007/009/020/021/036 |
| RNF-008 | Acessibilidade | ADR-008 | S0-T07/S6-T07/S6-T08 | CT-040 e checklist visual de 04 |
| RNF-009 | Logs minimizados | ADR-009/014 | S1-T08/S8-T04 | CT-016/033/034/037; inspeção de logs |
| RNF-010 | Implantação Windows/intranet | ADR-001/003/018 | S0-T05/S8-T07/S9-T05 | Smoke Windows e runbook de 11 |
| RNF-011 | Manutenibilidade | ADR-004/006 | S0-T04/S1-T06/S2-T01 | Testes puros sem WPF/EF; CI Windows |
| RNF-012 | Resiliência | ADR-012/013 | S3-T05/S3-T08/S6-T08 | CT-017/022/023/035/040 |
| RNF-013 | Durabilidade/publicação | ADR-012/013 | S3-T08/S5-T08 | CT-021/022/023/035 |
| RNF-014 | Recuperação | ADR-018 | S0-T09/S8-T07/S9-T04 | CT-039 e RTO/RPO de 11 |
| RNF-015 | Avaliação | ADR-017 | S0-T09/S4-T07/S9-T07 | Holdout, contagens e gates de 11 |
| RNF-016 | Distribuição/compatibilidade | ADR-018 | S8-T07/S9-T05 | Assinatura/SBOM, matriz de versões e rollback |
| RNF-017 | Contenção/provedores | ADR-012/014/017 | S0-T08/S3-T07/S3-T08/S4-T04 | CT-015/016/034/037 |

## 3. As 16 melhorias da revisão

Todas foram incorporadas à especificação ou ao planejamento indicado abaixo. Isso não significa que funcionalidades foram implementadas. M16 permanece evolução pós-MVP por decisão de escopo; demais itens têm obrigações nos componentes/sprints correspondentes. Nenhuma linha representa teste da aplicação executado nesta atualização.

| Melhoria | Alteração incorporada | Local de implementação/validação prevista |
|---|---|---|
| M01 | MVP unificado, versões/índice e governança | README; 01; 06; 07; AGENTS |
| M02 | Modelo completo e vínculos verificáveis | 03; 05; 10 §2; S1-T07 |
| M03 | Fila durável, leases, fencing, staging e reconciliação | 02; 10 §4–5; S3-T08 |
| M04 | Comparação transacional na publicação | 03; 10 §6; S5-T08; CT-021/024 |
| M05 | Manifesto/canonicalização e validade separada | 10 §7–8; S1-T09; CT-036 |
| M06 | Fração do total, movimentos simultâneos e classificação | 03 §4–5/10; ADR-015/016; CT-025…029/038 |
| M07 | Eventos, revisões, deduplicação e efeitos limitados | 03 §11; 09; 10; CT-031/032 |
| M08 | Evidência textual/contextual e promoção fora da IA | 09 §11; 10 §3; S4-T08; CT-028/037 |
| M09 | Isolamento, contenção e identidade especificados | 05; 10; 11 §4; S1-T08/S3-T08 |
| M10 | Corpus independente, métricas/limiares e go/no-go | 06; 11 §3; S0-T09/S9-T07 |
| M11 | Bancada WPF com dois portões, retomada e DPI | 04; S6-T08; CT-040 |
| M12 | Viabilidade local antecipada e política em falha | 11 §2; S0-T08; G0 |
| M13 | Dependências P0 e fatia integrada antecipada | 07; S0-T07/S2-T06 promovidas; S5-T09 |
| M14 | Runbook, recuperação, assinatura e compatibilidade | 11 §5–6; ADR-018; S8-T07 |
| M15 | Curadoria de normas e pendências sem suposições | README; 05; 11 §7; S0-T10 |
| M16 | Produtividade agendada após medição do piloto | 01 RF-110; 11 §8; E1-T01…04 |

## 4. Rastreabilidade complementar das tasks

Tasks não citadas nominalmente nas linhas anteriores têm os vínculos abaixo; todas continuam sujeitas à DoD comum de 07.

| Task | RF/RNF relacionados | Evidência planejada |
|---|---|---|
| S0-T01 | RF-002/005/014; RNF-015 | Entrevistas RI/TN e fluxos validados |
| S0-T02 | RF-101/102/109; RNF-003/015 | Corpus autorizado, particionado e anotado |
| S0-T06 | RF-015/016/019; RNF-004/011 | Schemas/fatos/atos/evidências e fixtures |
| S1-T03 | RF-015/016; RNF-004 | Atos preservam ordem, identidade e datas segregadas |
| S1-T04 | RF-005/007/016; RNF-011 | Categorias de direitos não se somam indevidamente |
| S1-T05 | RF-014/015; RNF-002 | Promoção só por revisão autorizada |
| S5-T03 | RF-005/016; RNF-007 | Replay exato, estado fechado somente com base suficiente |
| S5-T04 | RF-009/016/020; RNF-004 | Ônus/cancelamentos não suportados mantêm lacuna |
| S6-T03 | RF-009/015/019; RNF-008 | Evidência localiza página/trecho e mantém acesso por teclado |
| S6-T05 | RF-005/016; RNF-008 | Quadro, linha do tempo e lacunas diferenciados |
| S7-T04 | RF-009/020; RNF-004 | Alerta documental rastreável sem decisão jurídica presumida |
| S8-T05 | RF-005/007/011/014/017/018; RNF-002/013 | E2E e adversariais de 06 e 09 |
| S9-T01 | RF-001…020; RNF-003/015 | Piloto autorizado com RI/TN |
| S9-T02 | RF-015/016/109; RNF-015 | Métricas com contagens e baseline |
| S9-T03 | RF-020; RNF-004/015 | Cobertura/regra homologada somente no escopo verificado |
| S9-T06 | RNF-014/015/016 | Go/no-go com riscos, métricas e responsabilidades |

## 5. Decisões externas e gates ainda pendentes

| Pendência | Responsabilidade | Fechamento exigido |
|---|---|---|
| Provedor/hardware local, licenças e limites | Líder técnico/operação/contratante | S0-T08 antes de G0 |
| IdP, topologia física, SQL/licenciamento e certificados | Infraestrutura/segurança | S0-T05; sem improvisar fluxo de senha |
| Valores quantitativos do corpus e RTO/RPO | Produto/revisores/operação | S0-T09 antes da avaliação/ensaio |
| Cobertura de atos e mapeamento de efeitos | Revisores RI/TN | S0-T03/T10; antes de aplicar evento |
| Dispositivos normativos e referências não verificadas | Curadoria especializada | S0-T10; antes de ativar regra jurídica |
| Política de retenção, corpus e compartilhamento | Serventia/responsáveis por dados | S0-T05/T10; antes de usar dados reais |
| MSI/MSIX, certificado de assinatura e rollback | Operação/release | S8-T07; antes de distribuir piloto |

As obrigações foram especificadas; valores e aprovações que dependem do cartório não foram inventados. A documentação pode ser usada para iniciar tarefas técnicas com fixtures, mas o uso operacional depende de G0…G9 e de suas evidências.

## 6. Controle de mudança

Ao alterar requisito, atualizar a linha da matriz, ADR/contrato impactado, tarefas e casos de teste. Mudança de semântica de fração, hash, autorização ou publicação exige revisão coordenada. Preservar changelog e histórico do pacote; não afirmar commit, PR, build, benchmark ou homologação sem execução verificável.
