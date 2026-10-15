# 10 — Contratos de dados, processamento e publicação

**Versão:** 1.2.0 · **Data:** 15/10/2026 · **Estado:** especificação para implementação; sem código executado.

## 1. Limites e convenções

Uma operação do MVP referencia uma matrícula, múltiplas fontes dessa matrícula e minuta opcional. Uma análise com várias matrículas deve ser recusada como não suportada; não misturar matrículas só porque nomes/partes coincidem. Serventia operadora (`ServentiaId`) é o contexto de autorização; serventia de origem da matrícula é atributo documental distinto. Pessoas e fatos ficam no escopo autorizado, sem cadastro global que una cidadãos automaticamente.

Todos os IDs abaixo são UUIDs; instantes técnicos usam `DateTimeOffset` UTC; datas documentais sem horário usam `DateOnly`/valor original quando apropriado. Estado desconhecido é explícito, não representado por data zero. Campos editáveis usam `RowVersion` e revisão lógica do agregado; artefatos publicados são imutáveis. Exemplos de tipos são contratos orientadores, não uma biblioteca C# compilável entregue.

Serventia e usuário derivam da sessão validada no backend. A seleção de serventia deve corresponder a vínculo autorizado; não confiar no valor do payload. Rotas verificam também o encadeamento operação → cenário → revisão → fonte/snapshot.

## 2. Modelo lógico complementar ao documento 05

O domínio separa candidato, revisão e efeito. As estruturas a seguir devem ser persistidas por tabelas ou documentos JSON versionados com relações verificáveis; a escolha física final será registrada na task S1-T07 antes da migração. Colunas estruturais usadas em autorização, unicidade e FKs não ficam escondidas apenas em JSON.

| Estrutura | Campos essenciais além de Id/ServentiaId | Restrição/invariante |
|---|---|---|
| MatriculaReferencia | OperacaoId, ServentiaOrigem, Numero, CNM?, IdentificacaoRevisada | Uma por operação; identificação de origem é revisada |
| Operacao | MatriculaId, RevisaoAtual, EstadoWorkflow, RowVersion | Ponteiro corrente e revisão alterados em transação |
| FonteDocumental | OperacaoId, Sha256, StorageKey, MimeValidado, Bytes, DataFonte?, Emissor?, EstadoTecnico | Fonte original imutável; disponibilidade só após liberação |
| PaginaFonte | FonteId, Numero, Dimensoes, Rotacao | Único `(FonteId, Numero)`; numeração inicia em 1 |
| ExecucaoExtracao | FonteId, RevisaoConfiguracao, VersoesMotores, Estado, Manifesto | Cada reprocessamento cria nova execução |
| PaginaExtraida | ExtracaoId, PaginaFonteId, Estrategia, TextoStorageKey, LayoutStorageKey, TextoHash, Qualidade | Texto/layout imutáveis; versão resolve offsets |
| EvidenciaFonte | ExtracaoId, PaginaExtraidaId, InicioUtf16, FimUtf16, Trecho, BoundingBox?, RevisaoVisual? | Intervalo válido na extração específica; imagem original localizável |
| SegmentoAto | ExtracaoId, OrdemNaFonte, RotuloLocalizado?, Evidencias | Pode atravessar páginas, sem concatenação perder origem |
| ExecucaoInterpretacao | SegmentoId, ProvedorVersao, PromptHash, SchemaVersao, ConfiguracaoHash, SaidaStorageKey, Issues | Saída candidata preservada; sem permissão de confirmar |
| AtoCandidato | InterpretacaoId, IdentificadorSugerido?, TipoSugerido, DatasSegregadas, Issues | Não aplica efeito por existir candidato |
| CampoCandidato | AtoCandidatoId, CaminhoCampo, ValorSugeridoJson, Tipo, Estado | Valor nulo não significa ausência comprovada |
| CampoCandidatoEvidencia | CampoId, EvidenciaId | N:M; pertença ao mesmo contexto conferida |
| RevisaoCampo | CampoId, NumeroVersao, ValorFinalJson, Decisao, Justificativa?, UsuarioId, EmUtc | Append-only; correção exige justificativa; autorização humana |
| RevisaoAto | AtoId, NumeroVersao, CamposRevisoesFixadas, Decisao, UsuarioId, EmUtc, Hash | Fixar quais revisões de campos compõem o ato |
| VinculoAtosFontes | MatriculaId, AtoOrigemId, AtoRelacionadoId, TipoRelacao, RevisaoId | Correspondência, referência ou duplicidade revisada; sem unir por rótulo isolado |
| EventoRevisado | RevisaoAtoId, TipoEvento, Alvos, Fracao?, OrdemRevisada, MapeamentoVersao, Premissas, Hash | Efeito suportado; origem e categoria explícitas |
| ExecucaoReconstrucao | OperacaoId, RevisaoCapturada, ManifestoHash, EventosFixados, MotorVersao, SaidaStorageKey | Replay determinístico sem IA/rede |
| LacunaReconstrucao | ReconstrucaoId, Tipo, Relevancia, Referencias, Motivo | Ausência/conflito não desaparece na revisão |
| MatriculaSnapshot | OperacaoId, ReconstrucaoId, NumeroVersao, Hash, Completude, ManifestoHash | Publicado imutável; sem único FonteId obrigatório |
| SnapshotFonte | SnapshotId, FonteId, Finalidade | N:M; manifesto fixa versões utilizadas |
| TitularidadeSnapshot | SnapshotId, PessoaId, TipoDireito, Numerador, Denominador | Agregação só após identidade/categoria revisadas |
| TitularidadeEvidencia | TitularidadeId, EvidenciaId, EventoId | N:M; cadeia até ato e fonte |
| AprovacaoSnapshot | SnapshotId, SnapshotHash, RevisaoOperacao, UsuarioId, EmUtc, Premissas, Justificativa | Portão B, append-only; completude continua explícita |
| Cenario | OperacaoId, Nome, RevisaoAtualId, RowVersion | Cabeçalho editável; revisões publicadas imutáveis |
| CenarioRevisao | CenarioId, NumeroVersao, SnapshotId, PropostaHash | Proposta não muda em execução histórica |
| TransmissaoProposta | CenarioRevisaoId, MatriculaId, AlienanteId, AdquirenteId, TipoDireito, BaseFracao, Numerador, Denominador | MVP: propriedade plena e fração do total; valor positivo |
| ExecucaoSimulacao | CenarioRevisaoId, SnapshotId, ManifestoHash, EntradasStorageKey, ResultadoStorageKey, MotorVersao, Conclusao, Completude, Cobertura | Entradas completas e resultado preservados |
| ValidadeExecucao | ExecucaoId, Situacao, Motivo, RevisaoSubstituta?, AtualizadaEmUtc | Metadado separado; não editar conclusão original |
| Achado / AchadoEvidencia | ExecucaoId, RegraCodigo/Versao, Severidade, FatosComparados, Evidencias | Sem sobrescrever resultado pelo revisor |
| RevisaoAchado | AchadoId, UsuarioId, Decisao, Justificativa, EmUtc | Distinta de revisão de campo/ato e aprovação do snapshot |
| DossieEmitido | ExecucaoId, ManifestoHash, Formato, StorageKey, ConteudoHash, EmitidoEmUtc | Corrente/histórico verificado na emissão e no download |
| PacoteRegra | TipoTecnicoOuJuridico, Versao, ConteudoHash, Jurisdicao?, Vigencia, Aprovacoes, Situacao | Versões aprovadas imutáveis; ativo é ponteiro gerenciado |
| JobProcessamento | OperacaoId, FonteId?, Tipo, ChaveIdempotencia, ManifestoHash, Estado, LeaseAte?, WorkerId?, FencingToken | Estado durável e posse exclusiva por tentativa |
| TentativaJob | JobId, Numero, Inicio, Fim?, Estado, ErroCodigo?, Artefatos, ConfiguracaoFixada | Não perde tentativa anterior |
| EventoAuditoria | Ator, Acao, RecursoId, OperacaoId, CorrelationId, EmUtc | Sem conteúdo integral de documentos/prompts |

### Constraints e índices a implementar

- FKs e chaves únicas compostas preservam serventia e operação nas relações sensíveis. Aplicar validação de pertença também ao abrir stream, processar job e servir dossiê. Filtro global EF não é a única barreira.
- `NumeroVersao` único por agregado: `(ServentiaId, AtoId, NumeroVersao)`, `(ServentiaId, CenarioId, NumeroVersao)` e equivalentes. Evidências não podem apontar para extração de outra fonte/operação.
- Frações normalizadas, denominador positivo; saldo `0..1`, movimento `>0..1`; unicidade semântica e duplicidade documental são verificadas no domínio, não por nome. Limites BIGINT e overflow tratados antes da gravação.
- Índices por `(ServentiaId, OperacaoId, CriadaEmUtc)`, jobs por estado/próxima execução/lease e auditoria por serventia/data. Paginação com limite inicial de 100 itens.
- Deduplicação física por hash só no escopo autorizado; não revelar se outro tenant já possui documento. O mesmo PDF pode ter diferentes referências de uso, mantendo autorização separada.
- Usuário de aplicação comum não recebe UPDATE/DELETE sobre versões publicadas; correções geram versões. Retenção autorizada usa procedimento administrativo auditado, respeitando preservação e vínculos, não DELETE genérico.

## 3. Protocolo de evidência

`InicioUtf16` inclusivo e `FimUtf16` exclusivo são índices de unidades de código UTF-16 no texto imutável da página, antes da normalização semântica. Trecho deve coincidir com o intervalo exato; texto normalizado mantém mapa para a versão original. `BoundingBox` usa pontos PDF, origem superior esquerda da página após rotação normalizada, com largura/altura e transformação para a imagem original. Registrar transformação, não adivinhar coordenadas faltantes. Se indisponíveis, página + trecho continuam localizáveis; UI informa limite do destaque.

OCR produz transcrição candidata. Evidência deve permitir abrir imagem correspondente e registrar revisão visual, pois coincidência textual não garante leitura correta. Correção humana preserva transcrição original e valor corrigido. Campo manual sem fonte é `DeclaradoSemEvidencia`; aprovação humana não o converte em `ComprovadoDocumentalmente`.

Validação exige tipos, enum, contexto do ato, sujeito, direito e extensão; trecho verdadeiro associado a pessoa/ato errado continua candidato. O servidor rejeita campos de revisão enviados pelo modelo. LLM não recebe ferramentas para confirmar, gravar banco, abrir rede arbitrária ou chamar endpoints administrativos.

## 4. Contratos HTTP adicionais

Prefixo: `/api/v1/operacoes/{operacaoId}`. Toda rota exige autorização e pertença. UUIDs inexistentes ou fora do contexto retornam 404; insuficiência de permissão para ação conhecida retorna 403 sem revelar terceiros.

| Método / sufixo | Entrada e condição | Resposta |
|---|---|---|
| POST `/fontes` | Multipart PDF; chave idempotente; metadados de origem; streaming limitado | 201 com fonte em quarentena; não implica leitura liberada |
| POST `/fontes/{fonteId}/processamentos` | Fonte liberada, configuração permitida, chave idempotente e revisão capturada | 202 com JobId e Location, somente após commit durável |
| GET `/jobs/{jobId}` | Usuário autorizado à operação | Etapa, progresso conhecido, tentativas, cancelamento, erro sanitizado |
| POST `/jobs/{jobId}/cancelamentos` | Comando idempotente | 202 enquanto pendente; 200 se terminal, sem alterar saída já publicada |
| POST `/fontes/{fonteId}/reprocessamentos` | Nova intenção e configuração explícitas | 202 novo job/revisão; não sobrescreve artefatos anteriores |
| GET `/atos` | Paginação e filtro pendentes/não suportados | Candidatos, versões e referências de evidência |
| POST `/atos/{atoId}/revisoes` | If-Match; decisões de campos, justificativas e IDs de revisões | 201 revisão append-only, ETag novo; usuário derivado da sessão |
| POST `/reconstrucoes` | Revisões de atos fixadas, If-Match da operação, chave idempotente | 202; job de replay, nenhuma revisão automática |
| GET `/reconstrucoes/{id}` | Contexto autorizado | Estado, lacunas, manifesto e validade |
| POST `/snapshots/{snapshotId}/aprovacoes` | SnapshotHash, revisão capturada, premissas; If-Match | 201 Portão B; pode concluir revisão de estado incompleto sem apagar lacunas |
| POST `/cenarios/{cenarioId}/revisoes` | SnapshotId/hash, movimentos, If-Match e chave idempotente | 201 revisão imutável do cenário |
| POST `/cenarios/{cenarioId}/simulacoes` | CenarioRevisaoId, SnapshotId/hash, If-Match da operação e chave idempotente | 201 execução funcional, inclusive divergente/inconclusiva; 409 se dependências obsoletas |
| POST `/simulacoes/{execucaoId}/dossies` | Formato HTML; modo Corrente ou Historico explícito; chave idempotente | 201 artefato; 409 se modo corrente pedir execução obsoleta |
| GET `/dossies/{dossieId}` | Autorização reavaliada | Stream com validade atual e histórico sinalizado quando necessário |

Consulta periódica de job basta no MVP; não exigir SignalR. Intervalo inicial de polling de 2 s, com backoff para 10 s em falha/ociosidade. Progresso indeterminado é permitido; não inventar percentuais.

### Exemplo de movimento canônico

```json
{
  "matriculaId": "33333333-3333-4333-8333-333333333333",
  "alienanteId": "44444444-4444-4444-8444-444444444444",
  "adquirenteId": "55555555-5555-4555-8555-555555555555",
  "tipoDireito": "PropriedadePlena",
  "baseFracao": "TotalDoDireitoNoImovel",
  "fracaoDoTotal": {"numerador": 1, "denominador": 4}
}
```

Este movimento significa 1/4 do total, mesmo que o alienante detenha 1/2. A UI pode calcular metade de sua quota, mas salva a fração final confirmada. Movimentos recebem IDs estáveis na revisão e não são alterados no POST de simulação.

### Precondições, erros e idempotência

- Escrita de revisão exige ETag forte opaco em `If-Match`: ausente → 428, diferente → 412. Versão foi válida na aceitação mas dependência mudou no trabalho/publicação → 409 ou job concluído histórico, conforme seção 6.
- 422: sintaxe correta mas pré-condição inexistente, por exemplo sem Portão B ou sem pacote técnico. Snapshot revisado como incompleto gera resultado funcional inconclusivo, sem projeção, e não 422 por mera incompletude.
- 413 tamanho, 415 formato, 429 cota, 503 provedor/varredura indisponível; erro interno usa ProblemDetails com `code`, `traceId` e mensagem sanitizada. Falha técnica não recebe conclusão documental.
- `Idempotency-Key` obrigatória em criações/comandos com efeito: escopo `(ServentiaId, UsuarioId, rota lógica, chave)`, hash do payload canônico, status e resposta persistidos. Concorrentes com mesma chave recebem o mesmo recurso/resultado após conclusão; payload diferente → 409. Resposta em curso indica recurso/status, não executa novamente.
- Janela inicial de chaves: 7 dias, configurável e informada ao cliente; depois de expirar, o retry deve consultar o recurso antes de reenviar. Unicidades de versão/job continuam impedindo duplicação de efeito. Nova execução intencional usa nova chave e pode preservar mesmo hash lógico.
- Fechamento de tela/cancelamento HTTP do cliente não cancela job aceito. O comando de cancelamento é explícito no servidor.

## 5. Jobs duráveis e arquivos

### Estados e responsabilidades

`Pendente → EmExecucao → Concluido | Falhou | Cancelado`. Falha transitória pode passar por `AguardandoRetry → Pendente`. Quarentena é estado da fonte, não sucesso de processamento. `CancelamentoSolicitado` é intenção persistida, atendida em checkpoints; não implica que código nativo já parou.

| Etapa | Saída de checkpoint | Falha e retomada |
|---|---|---|
| Receber/staging | Arquivo temporário fechado, bytes/hash conferidos | Upload interrompido é removido conforme TTL; não cria fonte liberada |
| Registrar fonte | Metadado PendenteConfirmacao e chave de storage | Reconciliador decide completar promoção ou colocar em intervenção |
| Varredura/liberação | Fonte Liberada ou Rejeitada | Varredor indisponível mantém quarentena; não liberar por timeout |
| Criar job | Intenção e job em transação SQL | Após commit, 202; sem broker externo no MVP |
| Extrair/OCR | Artefatos por página imutáveis e manifesto | Retomar páginas confirmadas da mesma configuração |
| Interpretar | Saída candidata versionada por segmento | Reentrega não confirma fatos nem substitui saída aceita |
| Reconstruir/publicar | Resultado vinculado à revisão capturada | Publicação corrente exige comparação transacional |

Job usa chave de etapa `(JobId, Etapa, PaginaOuSegmentoId, ConfiguracaoHash)` e constraint de unicidade. Resultado de tentativa substituída não pode escrever sobre a vencedora. Entrega é pelo menos uma vez; o efeito visível é idempotente, sem promessa de execução física única.

### Posse do job e recuperação

Aquisição é UPDATE condicional/transação SQL que muda estado, define worker/lease e incrementa `FencingToken`. Padrões iniciais configuráveis: lease 60 s, heartbeat 15 s, até 4 tentativas totais para falhas transitórias, com espera 10/60/300 s e jitter. Erro de formato/permissão/schema não tem retry cego; vai para falha/intervenção. Reinício recupera jobs com lease expirado, incrementa token e retoma checkpoints válidos.

Toda gravação de checkpoint/publicação verifica posse e token da tentativa; worker antigo com lease vencido não pode publicar mesmo se terminar depois. Auditoria registra mudança de posse. Cancelamento impede novos estágios, encerra subprocesso quando suportado e preserva originais/artefatos já publicados; se publicação terminou antes, resposta indica conclusão e não apaga histórico.

### Consistência SQL + storage

Não presumir transação distribuída sobre disco. Protocolo: staging privado → hash/varredura → registro pendente → promoção para chave imutável → confirmação de disponibilidade. No mesmo commit que libera intenção de processamento, criar job durável. Se promoção falhar, manter pendente com motivo; se commit falhar após mover arquivo, reconciliador detecta órfão. TTL inicial de staging órfão: 24 h, sujeito à política; nunca eliminar fonte referenciada pelo mesmo procedimento.

Reconciliador periódico verifica referências pendentes, jobs sem artefato esperado, hashes divergentes e órfãos. Artefato corrompido vai para quarentena e invalida dependentes. O armazenamento não recebe caminho arbitrário do cliente; nomes de exibição são sanitizados e chaves são geradas pelo serviço.

## 6. Publicação concorrente e auditoria

Cada mudança relevante incrementa `Operacao.RevisaoAtual` na mesma transação que salva fonte/revisão/cenário. Alterar tabela filha não depende de RowVersion do pai mudar automaticamente: a aplicação deve atualizá-lo explicitamente. Mudança de regras/configuração ativa tem versão/epoch própria incluída na comparação.

1. Capturar revisão da operação, snapshot/aprovação, revisão do cenário, pacotes e epoch aplicáveis; persistir manifesto antes de trabalho externo.
2. Executar OCR/IA fora de transação longa; executar motores puros sobre entradas fixadas.
3. Abrir transação curta para publicar. Verificar serventia, posse/fencing do job, revisão atual, hashes/aprovação e epoch/pacotes vigentes no contexto.
4. Inserir conteúdo imutável e auditoria de publicação no mesmo commit SQL; ponteiro corrente só avança se todos os predicados conferirem. Arquivos da saída seguem promoção/checkpoint do storage.
5. Se entrada mudou, preservar saída como histórica com motivo e revisão substituta; não avançar corrente. Simulação síncrona pode retornar 409 com ID da execução histórica; job assíncrono conclui com `publicacao=Historica` e motivo. Reexecutar requer nova intenção.

Uma revisão humana não altera achado/candidato original. Privilégio de administrador não implica revisar dados de negócio. No piloto, o mesmo conferente pode concluir A e B se permitido em política versionada; dupla revisão por pessoa distinta é configurável e aplicada no backend, nunca simulada por checkbox.

## 7. Manifesto e hash canônico

Separar três hashes: bytes do PDF original; texto/layout por versão de extração; entradas lógicas da reconstrução/simulação. `ManifestoVersao=1` identifica a convenção abaixo.

O manifesto lógico inclui: IDs/hash do contexto documental e matrícula, fontes utilizadas, revisões de atos/eventos ordenadas, snapshot e aprovação fixados, revisão da proposta, pacotes técnicos/jurídicos com conteúdo/hash, versões dos motores e parâmetros, instante lógico da análise quando influencia regras, versões/configuração de extração/interpretação usadas e política relevante. Persistir os bytes canônicos, não somente o hash.

Não entram no hash lógico: ID da tentativa/execução, duração, horário técnico de publicação, CorrelationId e logs. A aprovação e revisões de conteúdo entram por seus IDs/hash estáveis; duas execuções sobre a mesma aprovação mantêm entrada equivalente. Gerar novo fato/revisão diferente não é a mesma entrada mesmo se o texto aparentar igualdade.

### Canonicalização v1

- JSON UTF-8 sem BOM e sem espaços extras; nomes de propriedades ordenados ordinalmente; sem propriedades duplicadas; lista de campos permitidos do schema fixa.
- Inteiros exatos em base 10, sem expoente; frações normalizadas; não usar double. Datas lógicas UTC com precisão fixa de 7 casas, ou data civil `YYYY-MM-DD` quando não há instante; não transformar data civil em timestamp presumido.
- Nulos opcionais são emitidos explicitamente; enum é string estável definida em Contracts; GUID em minúsculas no formato D. Strings preservam conteúdo e escapam caracteres de JSON; não normalizar texto-fonte silenciosamente.
- Arrays com ordem semântica, como eventos, preservam sequência revisada. Conjuntos sem ordem são ordenados por chave estável definida no schema. Movimentos simultâneos são ordenados por ID estável da revisão para hashing; sua ordem não altera cálculo.
- SHA-256 sobre bytes canônicos; representação `sha256:` mais 64 hexadecimais minúsculos. Golden fixtures cobrem variantes de serialização e regressão de versão.

Hash não é assinatura nem proteção autônoma contra administrador que pode trocar conteúdo e referência. ACL, restrições de escrita, backup e trilha são controles distintos; assinatura/ancoragem de evidência deve ter decisão de risco registrada, sem promessa de certificação jurídica pelo hash.

## 8. Obsolescência e dossiê

No MVP, qualquer fonte nova/substituída, revisão de campo/ato/identidade já confirmado, proposta ou pacote ativo relevante incrementa revisão e invalida conservadoramente os dependentes da operação. Nova tentativa que não publicou conteúdo não deve invalidar sozinha. Versão histórica de regras permanece disponível para reprodução; ativar nova versão não reescreve execuções.

`ValidadeExecucao` registra motivo, dependência alterada, instante e substituta quando existente. Seu conteúdo não faz parte do resultado original. Dossiê corrente exige snapshot/aprovação/proposta e pacotes ainda vigentes. Histórico pode ser emitido por ação explícita com faixa “Histórico — análise sobre versões anteriores”. Se dossiê emitido corrente depois ficar obsoleto, download deve acompanhar aviso persistente e produzir envelope/cópia de apresentação histórica com hash próprio, preservando bytes da emissão original para auditoria. Nunca servir silenciosamente um dossiê obsoleto como corrente.

Incluir no dossiê: fontes/datas, escopo da matrícula/operação, eixos do resultado, verificações executadas/não executadas, cobertura jurídica limitada ou ausente, cálculos, linhagem, revisores, manifesto, versões, premissas e ressalva de apoio sem efeito registral. Conteúdo de documento/modelo é escapado em HTML; scripts, URLs ativas não autorizadas e markup arbitrário são proibidos.

## 9. Aceite deste contrato

S1-T07/T09 e S3-T08 produzem schema/migrações e fila; S4-T08 valida contexto; S5-T08 implementa publicação. CT-021…CT-040 em 06 são o contrato de regressão. Alterar convenção de hash, schema ou estados exige nova versão e atualização simultânea de 02/03/06/07/12/AGENTS.
