# S0-T01 — Kit de descoberta com RI e TN

Este diretório contém os instrumentos para entrevistar pelo menos dois profissionais de Registro de Imóveis (RI) e dois profissionais de Tabelionato de Notas (TN). O material prepara a execução da S0-T01; não representa entrevistas realizadas, validação profissional ou conclusão da task.

## Objetivos

- Identificar documentos, atos e variações mais recorrentes.
- Mapear conferência, autorização, exceções e contingências atuais.
- Validar três jornadas críticas do Cartório Twin.
- Priorizar dores por frequência, impacto e risco percebido.
- Levantar restrições de infraestrutura, guarda, retenção e processamento por IA.
- Registrar diferenças por especialidade e UF sem inventar regra jurídica.

## Amostra mínima

| Identificador | Especialidade | Situação |
|---|---|---|
| `RI-01` | Registro de Imóveis | Pendente |
| `RI-02` | Registro de Imóveis | Pendente |
| `TN-01` | Tabelionato de Notas | Pendente |
| `TN-02` | Tabelionato de Notas | Pendente |

Participantes adicionais usam sequência crescente. Uma pessoa não conta simultaneamente como RI e TN, salvo justificativa explícita no relatório; a amostra mínima continua exigindo duas perspectivas independentes por especialidade.

## Arquivos e ordem de uso

1. [`01_ROTEIRO_ENTREVISTA.md`](01_ROTEIRO_ENTREVISTA.md): condução, perguntas comuns e blocos RI/TN.
2. [`02_REGISTRO_PARTICIPANTE_TEMPLATE.md`](02_REGISTRO_PARTICIPANTE_TEMPLATE.md): uma cópia sanitizada por participante.
3. [`03_MATRIZ_CONSOLIDACAO.md`](03_MATRIZ_CONSOLIDACAO.md): síntese comparável sem dados pessoais.
4. [`04_RELATORIO_DESCOBERTA_TEMPLATE.md`](04_RELATORIO_DESCOBERTA_TEMPLATE.md): relatório final com cinco dores e três fluxos validados.

## Ensaio sintético de desenvolvimento

A pasta [`examples/`](examples/) contém quatro personas e uma consolidação completa para exercitar o processo documental. Todos os arquivos são rotulados **SINTÉTICO — NÃO É EVIDÊNCIA**. Eles não representam entrevistas, autorizações, validações profissionais, práticas por UF ou regras jurídicas.

- [`MATRIZ_CONSOLIDACAO_SINTETICA.md`](examples/MATRIZ_CONSOLIDACAO_SINTETICA.md): agrupamento de cinco dores e três jornadas do ensaio.
- [`RELATORIO_DESCOBERTA_SINTETICO.md`](examples/RELATORIO_DESCOBERTA_SINTETICO.md): relatório final de teste, separado do aceite oficial.

As contagens da amostra mínima permanecem pendentes. Os templates oficiais continuam vazios até existirem registros reais sanitizados e validados.

## Procedimento

1. Explicar finalidade, duração, uso das respostas e ausência de decisão jurídica automatizada.
2. Confirmar autorização para registrar notas sanitizadas. Gravação não é necessária nem presumida.
3. Usar apenas cenários e documentos sintéticos durante a conversa.
4. Aplicar o bloco comum, apresentar CT-001 e depois aplicar o bloco da especialidade.
5. Ao terminar, remover nomes, contatos, serventia, números reais, matrículas, partes e qualquer trecho documental.
6. Enviar ao Git apenas o registro sanitizado; notas brutas ficam fora do repositório, no local autorizado pela organização.
7. Consolidar respostas somente após atingir a amostra mínima.
8. Pedir aos participantes validação da síntese referente à sua especialidade antes de marcar um fluxo como validado.

Fixtures sintéticas podem exercitar esses passos, mas ficam em `examples/` e nunca alteram contagens ou checklists oficiais.

## Regras de segurança e privacidade

- Proibido anexar PDF, imagem, matrícula, minuta ou certidão real.
- Proibido registrar nome, CPF, contato, nome da serventia, credenciais, endpoints internos ou detalhes que permitam reidentificação.
- UF, especialidade e faixa de experiência podem ser registradas quando não identificarem a pessoa ou a organização.
- Citações devem ser parafraseadas. Texto literal só entra após revisão de reidentificação e autorização explícita.
- Resposta sensível é substituída por `[REMOVIDO — DADO SENSÍVEL]` e descrita apenas em categoria agregada.
- O Git não é repositório de consentimentos, notas brutas ou dados do corpus.

## Definição de pronto do relatório

- Pelo menos `RI-01`, `RI-02`, `TN-01` e `TN-02` entrevistados.
- Cinco dores priorizadas com evidência agregada de frequência e impacto.
- Três jornadas avaliadas e validadas por múltiplos participantes.
- Convergências e divergências entre RI, TN e UFs explícitas.
- Restrições de dados, infraestrutura e IA registradas sem presumir autorização.
- Pendências com responsável por obter decisão, sem aprovação fictícia.

Até esses itens existirem e forem revisados, S0-T01 permanece pendente.
