# Roteiro de entrevista — S0-T01

**Duração sugerida:** 45–60 minutos  
**Participante:** usar apenas `RI-NN` ou `TN-NN`  
**Material permitido:** cenários e documentos sintéticos

## 1. Abertura

Ler ou explicar:

> Esta conversa busca entender o trabalho atual e validar hipóteses de um sistema auxiliar. O Cartório Twin não substitui conferência humana, não aprova registro e não consulta automaticamente o acervo oficial. Não informe nomes, contatos, números de matrícula, conteúdo de documentos reais, credenciais ou detalhes internos identificáveis. As notas enviadas ao Git serão sanitizadas e agregadas.

Confirmar no registro:

- finalidade compreendida;
- autorização para notas sanitizadas;
- especialidade correta;
- gravação desativada, salvo processo externo formalmente autorizado.

## 2. Contexto não identificável

1. Especialidade principal: RI ou TN?
2. UF de atuação, se puder ser divulgada sem reidentificação?
3. Faixa de experiência: `<2`, `2–5`, `6–10` ou `>10` anos?
4. Papel geral no fluxo: recepção, qualificação, conferência, revisão, autorização, suporte ou gestão?
5. Volume relativo: baixo, médio ou alto? Não solicitar números confidenciais.

## 3. Documentos, atos e complexidade

1. Quais tipos de documento chegam com maior frequência?
2. Entre PDF textual, digitalizado e misto, quais predominam?
3. Quais degradações mais atrapalham: rotação, baixa resolução, carimbos, manuscritos, tabelas, múltiplas colunas ou páginas ausentes?
4. Quais atos e averbações exigem mais tempo ou segunda revisão?
5. Quais informações são críticas para identificar matrícula, pessoas, direitos, frações, ônus e cancelamentos?
6. Quais situações devem sempre ser tratadas como não suportadas ou encaminhadas a especialista?
7. O que muda por UF, especialidade, serventia ou tipo de operação?

Não converter resposta em regra jurídica. Registrar como hipótese ou prática relatada até curadoria formal.

## 4. Fluxo atual de conferência e autorização

1. Como o caso entra no fluxo e quem faz a primeira triagem?
2. Quais etapas existem até a decisão humana final?
3. Onde ocorrem correções, devoluções, segunda conferência e justificativas?
4. Quem pode confirmar campos, atos, estado reconstruído e resultado de análise?
5. Que evidência precisa permanecer visível para a conferência?
6. Como conflitos ou falta de documentos são registrados?
7. Como uma alteração posterior invalida ou reabre trabalho já feito?
8. Quais contingências são usadas quando sistema, OCR, rede ou fonte está indisponível?

## 5. Três jornadas críticas

Para cada jornada, pedir que o participante descreva: ordem esperada, papéis, decisões, evidências necessárias, bloqueios, recuperação de erro e critério de encerramento.

### J1 — RI: matrícula até estado documentado

`Importar PDF sintético → extrair/OCR → sugerir atos e campos → revisar atos → reconstruir estado → revisar estado`

Perguntas:

- Em que pontos a revisão humana é indispensável?
- Como abrir rapidamente página e trecho que sustentam um campo?
- Que lacunas impedem concluir o quadro de titularidades?
- O que caracteriza um encaminhamento seguro em vez de uma conclusão?

### J2 — TN: matrícula e proposta/minuta opcional

`Importar matrícula → revisar estado documentado → informar proposta ou anexar minuta sintética → comparar pessoas/frações → simular → revisar dossiê`

Perguntas:

- Quais dados da proposta são necessários antes da comparação?
- Em que casos a minuta existe e em quais a proposta nasce manualmente?
- Que diferenças de identidade, qualificação ou fração exigem bloqueio ou revisão?
- Quais ressalvas devem acompanhar a simulação auxiliar?

### J3 — Lacuna, conflito ou ato não suportado

`Detectar ausência/ambiguidade → preservar evidência → impedir projeção completa → encaminhar revisão/contingência → registrar motivo`

Perguntas:

- Como distinguir falta de documento, erro de leitura, conflito e ato não suportado?
- Quem decide pedir nova fonte ou seguir com análise inconclusiva?
- Que informação deve continuar visível no histórico?
- O sistema pode apoiar algo sem induzir aparência de aprovação?

## 6. Cenário sintético CT-001

Apresentar sem documento real:

> Uma matrícula sintética revisada mostra João com `1/2` e Maria com `1/2`. A proposta informa que João pretende transmitir `1/1` para Carlos. O sistema sinaliza divergência de disponibilidade (`RT-004`) e não produz estado projetado.

Perguntar:

1. A sinalização é compreensível e operacionalmente útil?
2. Quais dados e evidências precisam aparecer juntos?
3. Qual vocabulário evitaria interpretação de parecer ou aprovação jurídica?
4. Quem deveria revisar, justificar e encerrar o achado?
5. Há fluxo alternativo legítimo que deve ser registrado sem liberar projeção enganosa?

## 7. Dores e impacto

Para cada dor citada, registrar:

- etapa afetada;
- frequência: rara, ocasional, frequente ou diária;
- impacto: baixo, médio, alto ou crítico;
- consequência: tempo, retrabalho, erro, risco, espera ou perda de rastreabilidade;
- prática atual de mitigação;
- resultado desejado;
- diferença percebida entre RI e TN.

Finalizar pedindo as cinco maiores dores em ordem, sem forçar consenso durante a entrevista individual.

## 8. Infraestrutura, dados e IA

1. Quais sistemas operacionais, estações, servidores e restrições de rede são relevantes? Usar categorias, não endereços ou inventário identificável.
2. Existe IdP/OIDC ou autenticação corporativa disponível?
3. Quais papéis precisam de acesso e segregação?
4. Quais classes de dados possuem guarda, restrição, bloqueio de exclusão ou backup específico?
5. Processamento local de PDF/OCR/IA é necessário ou preferível? Por quê?
6. Existe alguma autorização atual para processamento externo? Ausência de resposta significa **não autorizado**.
7. Que falhas exigem operação manual, fila, bloqueio ou interrupção total?
8. Quais evidências seriam necessárias para aprovar um piloto técnico?

## 9. Bloco específico de RI

1. Como abertura, registros e averbações são localizados e relacionados?
2. Quais alterações de qualificação costumam provocar associação incorreta de pessoas?
3. Como direitos distintos e frações são conferidos sem misturar propriedade, usufruto e ônus?
4. Quais cancelamentos, retificações e referências cruzadas exigem análise especializada?
5. Que sinais sugerem cadeia incompleta sem provar ausência de ato?

## 10. Bloco específico de TN

1. Em que momento matrícula, proposta e minuta são comparadas?
2. Quais diferenças de nomes, documentos, estado civil, representação e frações geram retorno?
3. Quem corrige proposta/minuta e como a nova versão é identificada?
4. Que informações do RI são essenciais, mas não suficientes, para o trabalho notarial?
5. Como apresentar achados sem transformar o sistema em redator ou aprovador automático do ato?

## 11. Encerramento

1. Qual parte das três jornadas traria maior valor primeiro?
2. Qual risco impediria adoção mesmo com ganho de tempo?
3. Que hipótese apresentada está errada ou incompleta?
4. O participante aceita revisar posteriormente a síntese sanitizada da sua especialidade?

Registrar pendências e combinar revisão da síntese sem guardar contato no Git.
