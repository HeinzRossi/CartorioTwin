# Registro sanitizado de participante — S0-T01

> Copiar este arquivo para ambiente autorizado durante a entrevista. Antes de versionar, remover toda PII, conteúdo documental, identificação da serventia, infraestrutura sensível e contato. Nome sugerido no Git: `RI-01.md` ou `TN-01.md`.

## Controle

| Campo | Valor |
|---|---|
| Participante | `RI-NN` ou `TN-NN` |
| Especialidade | `RI` ou `TN` |
| UF divulgável | `[UF]` ou `Não registrada` |
| Experiência | `<2`, `2–5`, `6–10` ou `>10` anos |
| Papel geral | `[categoria não identificável]` |
| Data | `AAAA-MM-DD` |
| Entrevistador | `[papel, sem nome pessoal no Git]` |
| Notas sanitizadas autorizadas | `Sim/Não` |
| Revisão de sanitização concluída | `Sim/Não` |

Se qualquer autorização estiver como `Não`, não versionar o registro.

## Documentos e atos relatados

| Item | Frequência relativa | Complexidade | Motivo | Variação por UF/especialidade |
|---|---|---|---|---|
| `[categoria]` | `[rara/ocasional/frequente/diária]` | `[baixa/média/alta]` | `[síntese]` | `[síntese/pendente]` |

## Fluxo atual

| Ordem | Etapa | Papel responsável | Entrada | Decisão/saída | Evidência necessária | Exceção/contingência |
|---:|---|---|---|---|---|---|
| 1 | `[etapa]` | `[papel]` | `[categoria]` | `[síntese]` | `[categoria]` | `[síntese]` |

## Avaliação das jornadas

### J1 — RI: matrícula até estado documentado

- Aplicável ao participante: `[Sim/Parcial/Não]`
- Ordem e papéis corretos: `[Sim/Parcial/Não]`
- Evidências indispensáveis: `[síntese]`
- Bloqueios e recuperação: `[síntese]`
- Alterações solicitadas: `[síntese]`

### J2 — TN: matrícula e proposta/minuta opcional

- Aplicável ao participante: `[Sim/Parcial/Não]`
- Ordem e papéis corretos: `[Sim/Parcial/Não]`
- Evidências indispensáveis: `[síntese]`
- Bloqueios e recuperação: `[síntese]`
- Alterações solicitadas: `[síntese]`

### J3 — Lacuna, conflito ou ato não suportado

- Aplicável ao participante: `[Sim/Parcial/Não]`
- Ordem e papéis corretos: `[Sim/Parcial/Não]`
- Evidências indispensáveis: `[síntese]`
- Bloqueios e recuperação: `[síntese]`
- Alterações solicitadas: `[síntese]`

## Reação ao CT-001

| Aspecto | Registro sanitizado |
|---|---|
| Divergência compreensível | `[Sim/Parcial/Não + motivo]` |
| Evidências necessárias | `[síntese]` |
| Vocabulário adequado/inadequado | `[síntese]` |
| Papel que revisa | `[categoria]` |
| Contingência sem projeção enganosa | `[síntese]` |

## Dores priorizadas pelo participante

| Ordem | Dor | Etapa | Frequência | Impacto | Mitigação atual | Resultado desejado |
|---:|---|---|---|---|---|---|
| 1 | `[síntese]` | `[etapa]` | `[nível]` | `[nível]` | `[síntese]` | `[síntese]` |

## Infraestrutura, guarda e IA

| Tema | Situação relatada | Restrição | Decisão existente ou pendência |
|---|---|---|---|
| Ambiente Windows/rede | `[categoria]` | `[síntese]` | `[decisão/pendente]` |
| Identidade e papéis | `[categoria]` | `[síntese]` | `[decisão/pendente]` |
| Guarda/retenção | `[categoria]` | `[síntese]` | `[decisão/pendente]` |
| Backup/continuidade | `[categoria]` | `[síntese]` | `[decisão/pendente]` |
| PDF/OCR/IA local | `[categoria]` | `[síntese]` | `[decisão/pendente]` |
| Processamento externo | `[Não autorizado/Autorização formal existente/Pendente]` | `[síntese]` | `[responsável por confirmar]` |

Nunca inferir autorização externa a partir de preferência técnica.

## Conclusões individuais

- Maior valor percebido: `[síntese]`
- Maior risco percebido: `[síntese]`
- Hipótese incorreta ou incompleta: `[síntese]`
- Aceita revisar síntese sanitizada: `[Sim/Não]`
- Pendências: `[lista sem responsáveis nominados]`

## Checklist antes do commit

- [ ] Não contém nome, CPF, contato ou identificação pessoal.
- [ ] Não contém nome ou localização identificável de serventia.
- [ ] Não contém número, trecho, imagem ou metadado de documento real.
- [ ] Não contém credencial, host, IP, endpoint ou inventário sensível.
- [ ] Citações foram parafraseadas ou autorizadas e revisadas.
- [ ] Campos não respondidos permanecem `Pendente`; nada foi presumido.
- [ ] Outra pessoa revisou a sanitização.
