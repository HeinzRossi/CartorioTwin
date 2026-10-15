# 08 — ADRs (Architecture Decision Records)

**Versão:** 1.2.0 · **Data:** 15/10/2026 · **Estado:** especificação documental; implementação e homologação pendentes.

ADR = decisão + contexto + consequências + estado. `Confirmada` preserva requisitos explícitos originais. `Adotada na baseline` indica desenho incorporado à documentação por autorização de 15/10/2026; não representa homologação do ambiente, contratação ou aprovação jurídica. Propostas de fornecedor/licenciamento/topologia física dependem de ensaio e decisão registrada na Sprint 0.

## ADR-001 — Linguagem e UI de desktop

- **Status:** Confirmada em 14/10/2026.
- **Decisão:** C# em **WPF (XAML) + .NET 10**, Windows.
- **Justificativa:** requisito explícito; ecossistema maduro, MVVM e interoperabilidade Windows.
- **Consequências:** aplicativo é Windows-only; publicar/testar DPI, teclado, instalador e versões de Windows.

## ADR-002 — RI + TN como foco

- **Status:** Confirmada.
- **Decisão:** orientar produto a compra e venda imobiliária com aspectos de Registro de Imóveis e Tabelionato de Notas.
- **Consequências:** exige linguagem que distingue minuta, título, documento apresentado, registro e titularidade; validação com profissionais das duas especialidades.

## ADR-003 — WPF + API local/intranet

- **Status:** Adotada na baseline documental 1.2.0; ambiente/licenças a validar.
- **Decisão:** WPF é interface principal; ASP.NET Core 10 é host de serviços; SQL Server é repositório operacional. API evita distribuir segredo de banco pelas estações.
- **Alternativas:** (A) WPF direto ao banco (menos componentes, segurança/gestão mais frágeis); (B) WPF + API (escolhida); (C) SaaS exclusivo em nuvem (foge da premissa de isolamento local e depende de contratos de dados).
- **Consequências:** instalador de API/serviço e suporte TLS; melhor base multiusuário e integração futura.
- **Validar:** topologia e parque de servidores/Windows do cliente piloto.

## ADR-004 — Simulação pura e snapshots imutáveis

- **Status:** Adotada na baseline documental 1.2.0; implementação/validação pendentes.
- **Decisão:** `ISimuladorOperacao` recebe dados completos e devolve novo estado; sem I/O, sem mutação de snapshot. Execuções persistidas como versões.
- **Alternativas:** modificar entidades vivas em transação rollback (maior risco de projeção confusa).
- **Consequências:** testes determinísticos e rastreabilidade; overhead de snapshots e hashes.

## ADR-005 — Frações racionais exatas

- **Status:** Adotada na baseline documental 1.2.0; implementação/validação pendentes.
- **Decisão:** value object com numerador/denominador inteiros normalizados; operações aritméticas verificadas.
- **Alternativas:** decimal com escala fixa (perde exatidão em terços/algumas razões); double (inaceitável para percentuais de direitos).
- **Consequências:** validações e conversões específicas; maior fidelidade aos atos.

## ADR-006 — Motor determinístico antes de IA

- **Status:** Adotada na baseline documental 1.2.0; implementação/validação pendentes.
- **Decisão:** desenvolver regras de integridade/aritmética puras primeiro; **PDF/OCR, interpretação, revisão em dois portões e reconstrução fazem parte do mesmo MVP**, com candidatos revisados. A ordem de implementação não remove esses componentes do escopo.
- **Consequências:** núcleo testável antes da integração, fatia técnica na Sprint 5 e pipeline completo obrigatório até G8/G9.

## ADR-007 — Status `ConsistenteNoEscopo`

- **Status:** Adotada na baseline documental 1.2.0; implementação/validação pendentes.
- **Decisão:** não usar `Apto`, `Regular` ou `RegistroAprovado`; ausências importantes devolvem `Inconclusivo`.
- **Consequências:** comunicação honesta e menor risco de induzir erro, embora percepção comercial exija educação de usuários.

## ADR-008 — Style ResourceDictionaries no WPF

- **Status:** Adotada na baseline documental 1.2.0; ambiente/licenças a validar.
- **Decisão:** separar cores, tipografia, controles, feedback e layouts em XAML; toolkit MVVM nos ViewModels.
- **Consequências:** consistência, testes e manutenção mais simples.

## ADR-009 — Banco, documentos e isolamento

- **Status:** Adotada na baseline documental 1.2.0; ambiente/licenças a validar.
- **Decisão:** SQL Server/EF Core 10, arquivos imutáveis por hash, perfis e escopo de serventia; suporte sem acesso por padrão.
- **Consequências:** migrações/backup precisam incluir storage e banco; validar custo de licenças e ambiente.

## ADR-010 — Normas jurídicas por pacote revisado

- **Status:** Adotada na baseline documental 1.2.0; implementação/validação pendentes.
- **Decisão:** regras com versão, data efetiva, UF, tipo de serventia, fundamento e aprovação humana antes de ativação.
- **Consequências:** necessidade de curadoria continuada; não inferir que o mesmo fluxo vale para todos os estados.

## ADR-011 — Git e entregas por Sprint

- **Status:** Adotada na baseline documental 1.2.0; ambiente/licenças a validar.
- **Decisão:** repositório privado sugerido `CartorioTwin`, CI, PR revisada e marco/tag por sprint concluída.
- **Consequências:** rastreabilidade, adoção de automação; URL/repositório ainda não criados.

## Decisões a tomar antes da implantação

- Definir quem administra o host da API, os certificados e a política de backup.
- Selecionar/provisionar IdP compatível com OIDC/PKCE, suporte remoto e recuperação de conta conforme ADR-014.
- Validar base jurídica/operacional para armazenar documentação de atos e tempo de retenção.
- Definir qual UF/serventia fornecerá a primeira revisão de regras estaduais.
- Aprovar orçamento de licenças e instalação, e plano de resposta a incidentes.

## ADR-012 — Worker separado e fila transacional SQL

- **Status:** Adotada na baseline; capacidade/host físico a validar na Sprint 0.
- **Decisão:** API aceita intenção durável e Worker executa jobs; SQL é fonte dos jobs. Lease, heartbeat, fencing, tentativas, checkpoints e reconciliação de storage seguem 10. Processos PDF/OCR são restritos e separados da API.
- **Alternativas:** fila apenas em memória perde trabalho em queda; broker externo acrescenta operação. Se broker for necessário, outbox é pré-condição do despacho, inclusive no MVP.
- **Consequências:** reinício/reentrega esperados; efeitos idempotentes, sem promessa de execução física única.

## ADR-013 — Revisão lógica, manifesto e publicação condicional

- **Status:** Adotada na baseline.
- **Decisão:** entradas e revisões fixadas em manifesto canônico v1; mudança de filho atualiza revisão do agregado. Publicação compara revisão, aprovação e epoch/pacotes em transação curta, com auditoria atômica.
- **Alternativas:** verificar somente na aceitação deixa corrida; manter transação aberta durante IA/OCR é inadequado ao fluxo.
- **Consequências:** resultado antigo pode existir como histórico, mas não avança corrente; obsolescência é metadado separado do conteúdo. Hash não é assinatura.

## ADR-014 — Identidade e isolamento operacional

- **Status:** Fluxo adotado na baseline; IdP/contratação a selecionar em S0-T05.
- **Decisão:** OIDC Authorization Code + PKCE via navegador do sistema; token de acesso em memória e refresh apenas autorizado/protegido por usuário Windows. API valida vínculo por serventia; worker não revisa fatos.
- **Consequências:** sem IdP compatível, provisionar um ou registrar novo ADR antes de implementar alternativa; suporte temporário, troca de serventia e cache são testados. Dupla revisão humana é política do servidor.

## ADR-015 — Uma matrícula e fração do total; movimentos simultâneos

- **Status:** Adotada na baseline.
- **Decisão:** uma matrícula por operação; múltiplas fontes/minuta opcional. Movimento transmite fração do total da propriedade; disponibilidade usa saldo inicial; aquisição no lote não financia saída.
- **Consequências:** encadeamento e várias matrículas exigem evolução explícita. Fração zero só para saldo, não transmissão. RT-005 com quadro completo e soma != 1 é divergente.

## ADR-016 — Eixos de resultado e cobertura de regras

- **Status:** Adotada na baseline.
- **Decisão:** execução técnica, conclusão, completude, cobertura e validade temporal são distintos. Pacote técnico aprovado obrigatório; jurídico só homologado/aplicável, opcional ao modo técnico. Divergência confirmada tem precedência sem esconder lacunas; ausência de verificações mínimas nunca gera consistente.
- **Consequências:** Portão B pode concluir revisão de estado incompleto, preservando limitação; falha técnica não é divergência documental.

## ADR-017 — Avaliação independente e gate quantitativo

- **Status:** Adotada na baseline; valores de capacidade/qualidade dependem do benchmark.
- **Decisão:** corpus de ajuste separado do holdout por matrícula/base; medir omissões de atos, erros de contexto, evidência, estado e esforço humano por estrato. Limiares registrados antes da avaliação; RTO/RPO antes do restore.
- **Consequências:** sem metas/amostra/capacidade pactuadas, G0 não encerra. Zero observado no corpus não é promessa universal. Atualização de modelo exige regressão.

## ADR-018 — Distribuição, recuperação e fontes normativas

- **Status:** Obrigações adotadas; forma MSI/MSIX e certificados definidos antes da distribuição.
- **Decisão:** binários/instalador assinados, SBOM e compatibilidade cliente/API/schema; restore integrado de SQL/storage/artefatos/chaves. Normas só habilitam regras após curadoria oficial de texto, vigência e aplicabilidade.
- **Consequências:** números/anos/relações de alteração não verificados continuam pendentes, sem interpretação presumida. Dossiê é apoio, não certidão ou assinatura de ato oficial.
