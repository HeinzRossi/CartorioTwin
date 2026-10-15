# 11 — Operação, avaliação documental e implantação

**Versão:** 1.2.0 · **Data:** 15/10/2026 · **Estado:** plano de validação, sem resultados de benchmark ou homologação.

## 1. Responsáveis e decisões da Sprint 0

| Decisão | Responsável pelo aceite | Evidência exigida | Task |
|---|---|---|---|
| Cobertura de atos e primeira jurisdição | Especialistas RI/TN da serventia piloto | Catálogo suportado/não suportado, efeitos e exceções | S0-T03/T10 |
| Hardware e licenças PDF/OCR/IA | Líder técnico + operação + contratante | Benchmark no hardware-alvo e inventário de licenças | S0-T08 |
| Identidade e topologia API/worker | Segurança/infraestrutura | ADR, IdP OIDC, contas, certificados, portas | S0-T05 |
| Corpus autorizado e proteção | Responsável pelos dados + revisores | Termo/finalidade, acesso, retenção e inspeção de anonimização | S0-T02/T09 |
| Limiares de go/no-go e capacidade | Produto + revisores + operação | Registro quantitativo antes da avaliação | S0-T09 |
| Guarda, restrição e eliminação | Serventia e responsáveis jurídicos/privacidade | Política por classe de dado e obrigações aplicáveis | S0-T05/T10 |

Papéis são responsabilidades a preencher com nomes internos, não pessoas presumidamente designadas. Campo sem responsável/valor não pode ser dado como aprovado. Aprovar documentação não equivale a contratar provedor externo, autorizar transferência ou homologar regra jurídica.

## 2. Prova de viabilidade de PDF/OCR/IA local

O MVP deve processar PDF textual, digitalizado e misto. Processamento externo permanece desativado por padrão. A Sprint 0 avalia opções locais em vez de assumir que o servidor atual suporta qualquer modelo.

Benchmark deve registrar: CPU/RAM/GPU/VRAM, Windows/SDK, versões exatas, licença comercial/distribuição, tamanho/páginas, tipo e degradação, tempo por etapa p50/p95, pico de memória, throughput sob concorrência, erro/omissão por tipo de ato e minutos de revisão. Comparar qualidade e esforço humano, não só velocidade de OCR.

| Capacidade | Critério de seleção | Evidência |
|---|---|---|
| Biblioteca PDF | Texto/layout por página, arquivos malformados, renderização e licença | PDFs textuais/mistos, páginas ausentes, criptografadas, assinadas e falhas controladas |
| OCR | Rotação, ruído, tabelas/layout, português, geometria e contenção | Mesmos casos visuais comparados com anotação |
| Interpretador | Schema tipado, contexto, limites, versão/configuração e processamento local | Candidatos com evidências e abstinência em ambiguidade |
| Visualizador | Teclado, DPI, destaque e renderização restrita | Ensaio WPF com usuários, sem depender de controle inacessível |

Provedor indisponível deixa job aguardando ou falho com motivo; não produz “texto completo” vazio. Cadastro manual é contingência rotulada e não substitui a demonstração documental exigida no aceite do MVP. Se a prova local falhar, registrar bloqueio, alternativa e novo ADR; não habilitar cloud automaticamente.

Cada revisão de modelo/prompt/OCR tem regressão antes de ativação. Persistir saídas candidatas para repetir replay e simulação sem novo provedor. Não prometer determinismo da IA, inclusive com temperatura baixa; determinismo é exigido para motores puros com entradas fixadas.

## 3. Corpus e métricas

### Composição e separação

Corpus de ajuste, corpus de avaliação e casos adversariais devem ter identidades separadas. Documentos derivados da mesma matrícula/base sintética ficam na mesma partição, evitando que uma versão quase idêntica contamine a avaliação. Revisor anota atos relevantes, campos críticos, relações, lacunas e estado esperado; divergência de anotação passa por segundo revisor e decisão registrada.

Materiais reais autorizados ficam em repositório restrito, nunca no Git ou logs. Anonimização precisa de inspeção do texto, imagem e metadados; esconder nome visualmente pode deixar camada textual intacta. Dados sintéticos representativos podem ser versionados. Corpus externo não autorizado não é coletado automaticamente.

Registrar contagens por PDF textual/digitalizado/misto, qualidade, tipo de ato, fonte e versão dos motores. Uma avaliação com poucos documentos não sustenta alegação universal de acurácia.

### Indicadores e cálculo

| Indicador | Definição | Interpretação |
|---|---|---|
| Recall de atos relevantes | Atos anotados corretamente recuperados / atos relevantes anotados | Conta também o ato inteiro que o pipeline omitiu |
| Precisão de atos | Atos recuperados corretos / atos recuperados | Um mesmo ato duplicado não é novo acerto |
| Exatidão de campos críticos | Valores corretos com sujeito/contexto certos / campos anotados aplicáveis | Nome certo ligado à pessoa errada é erro |
| Evidência verificável | Referências válidas de página/versão/trecho / referências geradas | Não substitui avaliação semântica do campo |
| Estado reconstruído correto | Casos com quadro/efeitos suportados iguais ao esperado / casos avaliáveis | Inconclusão correta é avaliada separadamente |
| Omissão crítica sem sinalização | Casos com ato/efeito crítico ausente e sem lacuna/alerta / casos aplicáveis | Bloqueador de piloto quando observado no conjunto de aceite |
| Inconclusão | Casos inconclusivos / casos processados, por motivo | Não ocultar nem remover da população total |
| Esforço de revisão | Mediana/p95 de minutos ativos por documento | Separar espera técnica e atividade humana |
| Custo de processamento | Infra/licenças/consumo mensurado por documento/página | Comparar com teto aprovado, sem supor preço de fornecedor |

Guardar numerador, denominador, amostra, intervalos de incerteza quando apropriados e versão. Não calcular média global que esconda falha em um tipo de ato crítico. Ground truth alterado exige nova versão, justificativa e recomputação.

### Registro obrigatório de aceite

Antes de executar o corpus de avaliação, preencher e aprovar: tamanho mínimo por estrato, recall mínimo por ato crítico, exatidão mínima dos campos essenciais, limite de inconclusão aceitável por formato, tempo máximo de revisão, throughput, custo teto, memória máxima e metas de recuperação. Valores dependem do benchmark S0-T08; **sem registro quantitativo, G0 não encerra e piloto não inicia**. Valores não podem ser escolhidos depois para acomodar o resultado.

Gates invariantes no conjunto delimitado: nenhuma confirmação por IA/worker; nenhuma exposição entre serventias; nenhuma alteração do acervo oficial; nenhuma projeção completa com lacuna essencial; nenhuma omissão crítica não sinalizada observada no conjunto de aceite; regressões matemáticas obrigatórias passam. “Zero observado” descreve o conjunto ensaiado, não garantia sobre todo documento futuro.

S9-T07 executa avaliação independente de S4-T07, com holdout não usado no ajuste. Se falhar, corrigir, registrar versão e usar nova avaliação controlada; não alterar anotação para esconder falha. Autoridade de go/no-go documenta riscos e limites remanescentes.

## 4. Contenção de processamento e privacidade

API/worker rodam com contas de serviço distintas e mínimos privilégios. Subprocesso de PDF/OCR recebe apenas fonte e pasta privada temporária da tentativa; não recebe token do usuário, chave de IA não necessária, conexão SQL ou permissões administrativas. Rede de saída bloqueada por padrão; liberar somente destinos contratados/autorizados. IA interpretadora tem acesso de leitura ao contexto necessário e nenhuma ferramenta de escrita/execução.

| Limite | Baseline para implantação | Ação em violação |
|---|---|---|
| Upload | 25 MiB/arquivo como valor inicial | 413; streaming interrompido sem promoção |
| Páginas, pixels, CPU, memória e tempo por etapa | Valores quantitativos de S0-T08 registrados antes de processar dados do piloto | Interromper subprocesso; falha/cota explícita; preservar fonte |
| Concorrência de jobs | Capacidade aprovada por máquina/serventia | Fila/backpressure, sem criar processos ilimitados |
| Temporários | Diretório privado por tentativa, ACL e TTL | Limpeza auditada; nunca remove original referenciado |
| Varredura antimalware | Necessária para liberar fonte | Indisponibilidade mantém quarentena |

Extensão, MIME e assinatura do formato são camadas; não provam arquivo seguro. PDFs criptografados exigem política/forma autorizada de acesso ou rejeição clara; não registrar senhas. Documentos assinados preservam bytes originais; renderização/normalização cria derivado identificado e não é validação oficial de assinatura.

Logs/telemetria não capturam corpos de documentos, imagens, prompts/respostas, cabeçalhos de autorização ou dados pessoais. Desabilitar logging automático de conteúdo em SDK/HTTP. Saídas necessárias à auditoria documental ficam em storage restrito, com retenção própria e download auditado. Suporte não tem acesso permanente ao acervo; sessão temporária aprovada, escopo e revogação definidos pela serventia.

Política de IA externa exige finalidade, fundamento, contrato, localização, suboperadores, retenção, proibição/condições de treinamento, minimização, segurança e registro de opt-in administrativo autorizado. Consentimento não é base universal presumida. Se não houver fundamento/autorização aplicável, transmissão permanece bloqueada.

## 5. Runbook de instalação e atualização

1. Validar inventário do servidor/estações, Windows suportado, espaço, SQL/licenciamento, IdP e capacidades medidas. Registrar responsáveis por serviço, dados e certificados.
2. Provisionar banco, storage, staging e temporários com ACL; criar contas API/worker/extrator e permissões separadas. Testar negações.
3. Instalar API e Worker como serviços Windows, com reinício controlado. Publicar HTTPS com certificado confiável, cadeia validada e expiração monitorada; não desativar validação TLS no WPF.
4. Configurar IdP OIDC, emissor/audiência, callback nativo/PKCE, perfis e serventias. Token de acesso em memória; refresh autorizado protegido pelo usuário Windows. Logout limpa memória/cache e revoga quando suportado. Troca de serventia recarrega contexto e descarta estado visual anterior.
5. Aplicar migração revisada com backup e janela definida; não migrar automaticamente no startup produtivo. Ensaiar rollback de aplicação/schema antes de release.
6. Instalar WPF por MSI/MSIX escolhido após teste do parque. Binários e instalador distribuídos ao piloto/produto devem ser assinados; CI gera SBOM, checksums, notas e procedência do build. Certificado/assinatura não representa validade jurídica dos dossiês.
7. Executar smoke autorizado com fonte sintética: login → processamento → revisões → simulação → evidência → dossiê; verificar contas/logs/backup.

Atualização declara versões de cliente/API/schema suportadas e mecanismo de detectar incompatibilidade. Manter janela compatível para atualização gradual ou exigir atualização coordenada explícita. Não fazer downgrade de banco destrutivo para acomodar binário antigo; usar restauração validada ou correção progressiva conforme plano. Patches de runtime, SO e bibliotecas entram em ciclo controlado, mesmo com SDK fixado para build.

## 6. Backup, restauração e continuidade

S0-T05/S0-T09 definem RTO (tempo de recuperação) e RPO (perda máxima de dados) quantitativos por componente antes do ensaio. S8-T07 verifica atendimento; apenas medir depois sem meta não constitui aceite.

Backup inclui SQL, originais, extrações/textos/layouts, artefatos de candidatos e resultados, manifests, regras/configuração e chaves necessárias. Definir estratégia de consistência e watermark entre banco e storage; restaurar SQL sem arquivos/chaves não é recuperação completa. Proteger backup com acesso, criptografia, retenção e cópia isolada conforme política aprovada.

Procedimento de restore: isolar destino → recuperar chaves/configuração → restaurar SQL/storage no mesmo ponto consistente → verificar hashes/referências e executar reconciliação → manter workers pausados enquanto valida filas/leases → recuperar jobs expirados → smoke sintético e validação amostral autorizada → liberar usuários com registro de responsáveis, tempos e perda medida. Backup não é considerado validado só porque o comando terminou.

| Incidente | Ação imediata | Evidência/critério de retomada |
|---|---|---|
| Disco/quotas esgotados | Recusar novos uploads e pausar etapas que escrevem | Espaço recuperado, órfãos reconciliados, jobs retomados sem duplicidade |
| Worker/IdP/provedor indisponível | Mostrar status técnico e preservar dados; sem simular sucesso | Saúde e backlog medidos; fluxo autorizado retestado |
| Certificado expirado | Falhar fechado, renovar e distribuir confiança | TLS válido; nenhum bypass permanente |
| Hash divergente | Quarentena e invalidação dos dependentes | Investigação, fonte confiável recuperada e nova execução |
| Suspeita de vazamento | Revogar acessos afetados, preservar evidências minimizadas e acionar responsáveis | Plano de incidente da serventia executado e retomada autorizada |
| Atualização interrompida | Suspender escrita incompatível e aplicar plano aprovado | Integridade e versões compatíveis comprovadas |

Registrar canal de suporte, dono do incidente, escalonamento e obrigações aplicáveis sem presumir prazos legais nesta especificação. Testar restauração antes do primeiro piloto e após mudanças relevantes da estratégia.

## 7. Curadoria normativa

Cada regra jurídica exige fonte oficial e dispositivo específico, texto/versão utilizados, datas de publicação/vigência/revogação, UF/especialidade/serventia aplicável, hipóteses/exceções, efeito permitido, testes e aprovação especializada. Regra técnica tem aprovação de engenharia e rótulo técnico; não inventar fundamento jurídico para soma de frações.

| Referência recebida | Situação na baseline | Ação |
|---|---|---|
| Código Civil, Lei de Registros Públicos e LGPD | Referências gerais do pacote, sem matriz exaustiva homologada | Vincular dispositivo/versionamento ao requisito aplicável |
| Provimento CNJ 149/2023 | Referência geral, texto consolidado a curar | Verificar alterações e aplicabilidade antes de regra |
| Referências denominadas 213/2026 e 243/2026 | Não verificadas nesta revisão; links retornaram 403 | Confirmar número, ano, ementa, texto e relação de alteração em fonte oficial; não ativar com base só na citação |
| Normas estaduais/locais | Primeiro pacote ainda a selecionar | Curadoria por UF e revisão da serventia |

Formato do registro: `NormaId`, `Dispositivo`, `FonteOficial`, `TextoHash`, `ConsultadoEm`, `VigenciaInicio/Fim`, `Jurisdição`, `RegraCodigo/Versao`, `Hipoteses/Excecoes`, `AprovadoPor/Em`, `ProximaRevisao` e `Situacao`. Norma não confirmada fica como pendência e não é “corrigida” por suposição. Mudança de pacote ativo atualiza epoch/validade; relatórios antigos mantêm contexto original.

## 8. Evolução de produtividade após o MVP

Estas tarefas não são bloqueadoras do MVP; implementá-las só após medir esforço humano e erros no piloto.

| Task | Prioridade | Entregável | Dependência | Aceite |
|---|---|---|---|---|
| E1-T01 | P1 | Comparação de versões da matrícula e atos sobrepostos | G9; deduplicação revisada | Diferenças localizadas sem reaplicar aquisição |
| E1-T02 | P1 | Comparação A/B de cenários | G9; revisões imutáveis | Mesma base/cobertura ou diferença explicitamente indicada |
| E1-T03 | P1 | Fila de revisão por motivo/criticidade | Métricas de S9 | Redução de tempo medida sem elevar omissão crítica |
| E1-T04 | P2 | Reaproveitar candidatos/fatos revisados | E1-T01; política de atualidade | Nova fonte exige revalidação; sem confirmação automática cruzada |

## 9. Referências técnicas primárias

- [Microsoft — concorrência no EF Core](https://learn.microsoft.com/en-us/ef/core/saving/concurrency)
- [Microsoft — hosted/background services](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-10.0)
- [OWASP — File Upload](https://cheatsheetseries.owasp.org/cheatsheets/File_Upload_Cheat_Sheet.html)
- [OWASP — LLM Prompt Injection Prevention](https://cheatsheetseries.owasp.org/cheatsheets/LLM_Prompt_Injection_Prevention_Cheat_Sheet.html)
- [Microsoft — versões e suporte do .NET](https://learn.microsoft.com/pt-br/dotnet/core/releases-and-support)

Essas referências apoiam práticas técnicas; valores de capacidade e metas operacionais são decisões do ambiente piloto, não garantias dos fornecedores.
