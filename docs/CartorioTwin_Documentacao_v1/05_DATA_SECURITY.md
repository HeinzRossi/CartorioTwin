# 05 — Dados, evidências, segurança e integrações

**Versão:** 1.2.0 · **Data:** 15/10/2026 · **Estado:** especificação documental; implementação e homologação pendentes.

## 1. Estratégia de persistência

Banco operacional proposto: **SQL Server + EF Core 10** no serviço backend. Histórico de simulações e revisões é armazenado em versões imutáveis, com histórico de snapshots separado das entidades editáveis. Documentos originais ficam em repositório de arquivos protegido e não como logs. O uso de SQL Server deve ser confirmado na Sprint 0 considerando operação, licenciamento e infraestrutura de cada serventia.

### Tabelas propostas (modelagem lógica, não DDL final)

| Tabela | Campos essenciais | Observações |
|---|---|---|
| `Serventia` | `Id`, `UF`, `Nome`, `Ativa`, `CreatedAtUtc` | Tenant lógico |
| `Usuario` | `Id`, `IdentidadeExterna?`, `NomeExibicao`, `Ativo` | Não armazenar senha em claro |
| `UsuarioServentia` | `UsuarioId`, `ServentiaId`, `Perfil`, `Ativo` | Muitos-para-muitos autorizado |
| `Operacao` | `Id`, `ServentiaId`, `Tipo`, `Estado`, `RowVersion`, timestamps | Raiz transacional |
| `FonteDocumental` | `Id`, `ServentiaId`, `OperacaoId`, `Sha256`, `StorageKey`, metadados | Conteúdo imutável, referência ao arquivo |
| `EvidenciaFonte` | `Id`, `ExtracaoId`, `PaginaExtraidaId`, `InicioUtf16`, `FimUtf16`, `Trecho`, `BoundingBox?` | Versão textual exata; declaração manual sem fonte fica explicitamente distinta |
| `MatriculaSnapshot` | `Id`, `OperacaoId`, `ReconstrucaoId`, `NumeroVersao`, `Hash`, `Completude`, timestamps | Fontes N:M em SnapshotFonte; aprovação separada |
| `PessoaReferencia` | `Id`, `ServentiaId`, `AliasLocal`, `Origem` | Sem identificação automática por nome |
| `TitularidadeSnapshot` | `Id`, `SnapshotId`, `PessoaRefId`, `Numerador`, `Denominador`, `TipoDireito` | Valor racional exato; TitularidadeEvidencia N:M |
| `DireitoDocumentado` | `Id`, `SnapshotId`, `Tipo`, `PessoaRefId`, `Extensao`, `EvidenciaId` | Não confundir com frações da propriedade |
| `Cenario` / `CenarioRevisao` | `Id`, `OperacaoId`, `Nome`, `RevisaoAtualId`; revisão com `SnapshotId`, `PropostaHash` | Revisão publicada não muda; cabeçalho é editável |
| `TransmissaoProposta` | `Id`, `CenarioRevisaoId`, `MatriculaId`, `AlienanteId`, `AdquirenteId`, `TipoDireito`, `BaseFracao`, `Numerador`, `Denominador` | Entrada imutável da execução |
| `ExecucaoSimulacao` | `Id`, `CenarioId`, `SnapshotHash`, `RegrasHash`, `Status`, `ExecutadaEmUtc` | Uma nova linha por execução |
| `Achado` | `Id`, `ExecucaoId`, `RegraCodigo`, `RegraVersao`, `Gravidade`, `Mensagem`, `DadosComparadosJson` | Sem opinião jurídica automática |
| `AchadoEvidencia` | `AchadoId`, `EvidenciaId` | Liga achado às origens |
| `RevisaoAchado` | `Id`, `AchadoId`, `UsuarioId`, `Conclusao`, `Justificativa`, `DataUtc` | Append-only |
| `PacoteRegra` | `Id`, `Versao`, `UF`, `TipoServentia`, `Ativo`, `AprovadoPor`, `DataAprovacao` | Controle jurídico |
| `EventoAuditoria` | `Id`, `ServentiaId`, `AtorId`, `Tipo`, `Recurso`, `RecursoId`, `DataUtc`, `CorrelationId` | Append-only, sem corpo de documentos |

O modelo acima é um resumo. **O catálogo completo de atos/campos/revisões, eventos, extrações, snapshots, evidências N:M, jobs e artefatos está em 10, seção 2, e é obrigatório para a modelagem física.** `RevisaoCampo`, `RevisaoAto`, `AprovacaoSnapshot` e `RevisaoAchado` são relações distintas e verificáveis, não um único campo AchadoId para qualquer revisão.

Todos os recursos pertencentes a uma serventia devem ser consultados por `ServentiaId` **no servidor**, inclusive downloads e dossiês. Recomendação: índices compostos `(ServentiaId, Id)`, `(ServentiaId, OperacaoId)`; FKs tenant-aware quando aplicáveis e testes para escapar de filtros globais EF. `RowVersion` em operações alteráveis; simulações já publicadas são imutáveis. Considerar chave aleatória (GUID/UUID) em operações para integração futura.

### Retenção, exclusão e auditoria

Definir prazo, necessidade, base legal e tratamento de cada classe de dado com a serventia e assessoria jurídica. O produto não presume eliminação automática de documentação sujeita a obrigações de conservação. Implementar pedidos de eliminação/restrição **sujeitos às bases legais e obrigações aplicáveis**, inclusive requisitos de preservação de registros/auditoria. A política deve ser parametrizada, aprovada e registrada.

## 2. Controle de acesso (RBAC)

| Capacidade | Operador | Conferente | Responsável jurídico | Administrador | Auditor |
|---|---:|---:|---:|---:|---:|
| Criar rascunho / fontes | ✓ | ✓ | ✓ | Configurável | — |
| Revisar snapshot | — | ✓ | ✓ | Configurável | — |
| Executar simulação | Configurável | ✓ | ✓ | — | — |
| Registrar revisão jurídica | — | — | ✓ | — | — |
| Exportar dossiê | — | ✓ | ✓ | Configurável | Leitura se autorizado |
| Administrar usuários | — | — | — | ✓ | — |
| Consultar auditoria | — | — | ✓ (escopo limitado) | ✓ | ✓ |

Implementar permissões como políticas no backend (`RequireAuthorization`) e espelhar disponibilidade visual no WPF; **nunca confiar apenas no botão oculto**. Requisições para recursos de outra serventia devem responder não encontrado/negado e **não revelar dados de existência**. Sessões e tokens com timeout/revogação; logs de acessos a documentos.

## 3. Privacidade e segurança desde o MVP

### Ameaças e controles

| Ameaça | Controle requerido |
|---|---|
| Vazamento de matrículas/escrituras | Criptografia em trânsito, isolamento por serventia, controle de download, segredo fora do cliente |
| Envio indevido a IA externa | Provedores externos desativados por padrão; finalidade, fundamento aplicável, contrato, avaliação e configuração autorizada antes de ativar; consentimento não presumido como base universal |
| Exposição por logs | Mascarar CPF, RG, nome, matrículas, títulos, documentos e prompts; logar apenas IDs e status |
| Importação de arquivo malicioso | Verificação de tipo real/MIME, tamanho, extensão, varredura antimalware e sandbox do extrator |
| PDF com prompt injection | Texto extraído é **dado não confiável**; não executar instruções contidas em documentos |
| Repetição de requisição/duplicidade | Idempotency-Key na criação e hashes de execução, ETag para escrita concorrente |
| Alteração não detectada de documento | Hash SHA-256 comparado a referência confiável; ACL, restrições de atualização e histórico. Hash sozinho não impede substituição do conteúdo e do hash |
| Acesso indevido por suporte | Acesso temporário explícito, auditável, sem acesso por padrão |
| Perda de banco/arquivo | Backup consistente de metadados + arquivos e teste periódico de restauração |
| Uso de resultado desatualizado | Exibir etiqueta de obsolescência e impedir dossiê corrente a partir de execução antiga |

### Criptografia e segredos

- Segredos, certificados e conexões residem somente no host do backend; usar cofre do sistema/serviço e rotação documentada.
- Criptografar volumes/backups; se criptografia de documentos por arquivo for adotada, gerenciar chaves e recuperação (não só criptografar e esquecer).
- Empregar HTTPS com certificado confiável; não permitir opção de ignorar validação TLS no código.
- Desabilitar *telemetry* com dados de negócios; diagnósticos exportáveis só após revisão e minimização.
- Desabilitar upload de documentos reais para ambientes de testes e serviços externos não aprovados.

### Conformidade

O projeto deve ser avaliado frente à LGPD, ao Código Nacional de Normas e às obrigações de TI/segurança e normas estaduais aplicáveis, por matriz de dispositivo, vigência, escopo e responsável. As referências recebidas como Provimentos CNJ 213/2026 e 243/2026 estão **pendentes de confirmação oficial**; não se presume nesta baseline sua identificação ou relação de alteração. Ver 11, seção 7. A análise de adequação e critérios de retenção precisam de validação profissional por serventia; a documentação **não certifica conformidade**.

## 4. Processamento documental obrigatório no MVP

`Upload/quarentena → validação técnica/liberação → job durável → extração nativa/OCR por página → segmentação/interpretação → candidatos/evidências → Portão A → eventos/replay → estado/lacunas → Portão B → proposta → simulação → dossiê`.

Texto nativo, OCR e interpretação assistida são P0. Provedor local é selecionado por benchmark na Sprint 0; envio externo permanece bloqueado sem autorização específica. A fonte original é imutável; texto, layout e saída candidata são derivados versionados. Campo manual é correção/contingência, com origem explícita.

Referência de evidência inclui extração/versão, página, offsets UTF-16 e geometria quando disponível. Não basta guardar trecho sem indicar a versão textual. Relações campo/ato/pessoa/direito precisam ser revisadas; evidência literal existente não prova interpretação correta. Confiança de OCR não é validade jurídica.

A conta do extrator não recebe acesso ao banco nem tokens de usuário; limites de memória/tempo/páginas/pixels e rede controlam arquivos hostis. Aplicar autorização a derivados, temporários, filas, caches e dossiês, além de tabelas principais. Protocolos completos: 10 e 11.

## 5. Contratos de integração

### Integrações internas (MVP)

- WPF ↔ API REST autenticada: HTTPS JSON, DTOs versionados, cancelamento e tratamento consistente de problemas.
- API ↔ SQL Server: EF Core, migrações controladas em manutenção; nunca migrar schema automaticamente sem plano/backup em ambiente produtivo.
- API ↔ armazenamento: stream de arquivo, hash, antivírus e validação de permissões.

### Integrações futuras, não assumidas

- Sistemas existentes de TN e RI: adaptadores sob contrato e com aprovação da serventia; leitura de dados conforme API/exportações autorizadas.
- ONR/SREI/e-Notariado: somente se houver interface documentada, credenciamento e finalidade juridicamente permitida. **Não utilizar scraping de portais protegidos**.
- Provedores de OCR/LLM: seleção por local de processamento, tratamento de dados, confidencialidade, suboperadores, retenção e custo; opção local deve ser avaliada.

Cada conector deve declarar `Capacidades`, `Direção`, `Contrato`, `Permissões`, `DadosTransmitidos`, `Timeout`, `Retry`, `Observabilidade` e `PolíticaDeFalhas`. Falha de conector não pode induzir conclusão de ausência de ônus/direito.

## 6. Operabilidade

Métricas técnicas sugeridas: latência de API, tempo de simulação p95, tamanho de upload, taxas de extração/revisão, falhas por conector, jobs interrompidos, inconsistência de hash, tentativas de acesso negadas e uso de espaço. Logs com `CorrelationId` por requisição. Executar testes reais de backup/restauração antes da primeira instalação em cartório.

## 7. Modelo de relatório de auditoria operacional

```text
Execução: CT-SIM-0001
Ser­ventia: [identificador autorizado]
Operação: Compra e venda
Snapshot: v2 / sha256:...
Regras: pacote RI-BR-basico-v1
Status: DIVERGENTE
Achado: RT-004 — fração transmitida maior que fração documental
Fonte: Matrícula X, documento Y, página Z, revisão humana por usuário Q
Ação posterior: hipótese revisada; nova simulação CT-SIM-0002
Nota: dossiê de apoio, sem efeito jurídico ou registral
```

Nunca registrar o texto integral dos documentos no `EventoAuditoria` técnico; relacionar apenas IDs/hashes/evidências em repositório sujeito à autorização.

## 8. Segurança transacional e operacional verificável

Fonte em quarentena não pode ser servida ao visualizador; falha do antivírus mantém bloqueio. API e worker têm identidades técnicas separadas, e worker nunca confirma candidato. Sessão expirada impede novas ações; logout/troca de serventia limpa memória/cache e revoga credenciais conforme o IdP. Suporte só recebe escopo temporário auditado quando autorizado pela serventia.

Inserção da execução e auditoria de publicação pertencem à mesma transação SQL. Arquivos seguem staging/promoção/reconciliação de 10; não há transação automática disco+SQL. Revisão da operação é incrementada explicitamente para alterações relevantes em filhos; publicação compara novamente revisão e configuração/pacotes.

Privacidade e retenção cobrem originais, extrações, prompts/respostas preservados como artefatos restritos, snapshots, jobs, dossiês e backups. Não registrar esses conteúdos em logs técnicos. Suspensão de exclusão por obrigação de preservação deve ser representada em política aprovada; eliminação, quando autorizada, considera vínculos e cópias de backup conforme procedimento definido. Hash não é anonimização.

Restore deve incluir banco, arquivos, artefatos, configurações e chaves no mesmo ponto consistente; RTO/RPO são pactuados antes dos testes. Runbook, incidentes e critérios de distribuição estão em 11.
