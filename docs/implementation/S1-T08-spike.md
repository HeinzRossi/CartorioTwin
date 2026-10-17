# Spike S1-T08 — Identidade, autorização e isolamento operacional

**Estado:** baseline técnica candidata implementada; integração e validações externas pendentes

**Data:** 16/10/2026

**Dependências:** S0-T05 para decisões de IdP, certificados, storage, ACL e retenção; S1-T09 para integridade do pacote; validação externa das baselines anteriores

## Objetivo

Estabelecer uma fronteira neutra de provedor para autenticação OIDC/JWT, autorização baseada em capacidades, isolamento por serventia, contas de serviço, autorização auditável de revisão e preparação do cliente desktop. Nenhum endpoint público de negócio, IdP produtivo, senha ou emissão de token foi criado.

## Rastreabilidade

- RF-001: o cliente desktop candidato usa Authorization Code + PKCE, navegador/callback injetáveis e nenhum client secret.
- RF-012: decisões humanas autorizadas produzem registro append-only com ator, perfil efetivo, capacidade, recurso, operação, serventia, instante UTC e correlação.
- RNF-001: JWT é validado quanto a assinatura, emissor, audiência, expiração e claims técnicas obrigatórias; authority de produção deve ser HTTPS.
- RNF-002: a serventia selecionada pelo cliente é apenas uma solicitação; o vínculo ativo resolvido por `iss + sub` no banco define o contexto autorizado.
- RNF-009: access token fica somente em memória; refresh token é protegido por DPAPI `CurrentUser`, escrito atomicamente e removido no logout.
- ADR-009: a API permanece a única fronteira de acesso a persistência e autorização; o desktop não consulta banco.
- ADR-014: confirmação humana exige capacidade efetiva e gera referência de autorização opaca, não reutilizável como credencial.

## Autenticação e resolução de identidade

- A API usa `Microsoft.AspNetCore.Authentication.JwtBearer` 10.0.12, `MapInboundClaims=false` e fallback policy autenticada.
- Produção falha no startup sem authority HTTPS, audience e allowlist não vazia de client IDs de worker.
- Tokens de usuário exigem `iss`, `sub`, `client_id`/`azp`, `jti` e `iat`; a validação JWT exige assinatura, emissor, audiência, validade e expiração.
- Usuários são resolvidos por `(EmissorIdentidade, IdentidadeExterna)` e pelo vínculo ativo com a serventia solicitada. Usuário, serventia ou vínculo inativo é negado.
- Roles do token são ignoradas. O perfil persistido é a autoridade final.
- Contas de serviço exigem `actor_type=service` e client ID na allowlist. Mesmo contendo roles indevidas, recebem apenas `jobs.processar`.

## Matriz candidata de capacidades

| Perfil | Capacidades |
|---|---|
| Operador | `fontes.ler`, `fontes.criar` |
| Conferente | Operador + `fatos.revisar`, `snapshots.revisar`, `simulacoes.executar`, `dossies.exportar` |
| Responsável Jurídico | Conferente + `revisoes-juridicas.registrar`, `auditoria.consultar` |
| Administrador | `usuarios.administrar`, `auditoria.consultar` |
| Auditor | `dossies.exportar`, `auditoria.consultar` |
| Worker autorizado | somente `jobs.processar` |

Administrador não recebe implicitamente `fatos.revisar`. Propriedade administrativa e confirmação documental são responsabilidades distintas.

## Persistência e autorização de revisão

- A segunda migration, `S1T08IdentidadeAutorizacao`, mantém intacta a migration inicial.
- `Usuario` recebe o emissor da identidade e índice único filtrado por `(EmissorIdentidade, IdentidadeExterna)`.
- `UsuarioServentia.Perfil` aceita somente os cinco perfis conhecidos.
- `AutorizacaoRevisao` registra ID, serventia, operação, tipo/ID/versão do recurso, usuário, perfil efetivo, capacidade, instante UTC e correlação.
- FKs compostas ligam a autorização à operação e ao vínculo usuário–serventia corretos. Exclusões são restritas.
- O repositório somente adiciona registros e o `DbContext` rejeita alteração ou exclusão rastreada: a autorização comprova a decisão da sessão, mas não é credencial reutilizável.

## Storage autorizado

- A Application expõe uma porta de storage e um decorador obrigatório para leitura, staging e promoção.
- O serviço gera chaves canônicas a partir de serventia, operação, etapa e ID; caminho arbitrário do cliente não é aceito.
- Capacidade e contexto são validados antes de qualquer chamada ao backend; promoção entre operações é negada.
- Backend físico, ACL do sistema operacional, quarentena e retenção pertencem a S0-T05/S3-T01.

## Cliente desktop

- `Duende.IdentityModel.OidcClient` 7.1.0 configura cliente público sem segredo, authority HTTPS, scopes explícitos, redirect URI e navegador do sistema injetável.
- O access token existe somente na instância de sessão em memória.
- O refresh token é cifrado por DPAPI `CurrentUser`, com entropia da aplicação, escrita temporária seguida de substituição atômica e remoção no logout.
- Nenhuma tela de login foi criada; integração visual pertence à S6-T01.

## Modelo de ameaças resumido

| Ameaça | Controle nesta baseline | Limite aberto |
|---|---|---|
| Token forjado, expirado ou para outro destinatário | assinatura RSA/IdP, issuer, audience, lifetime e claims obrigatórias | IdP e certificados produtivos em S0-T05 |
| Elevação por role inserida no token | roles ignoradas; perfil obtido no banco | governança de concessão externa |
| Acesso entre serventias | vínculo ativo no banco, contexto server-side, FKs compostas e ocultação HTTP 404 | testes de penetração e política produtiva |
| Worker executando decisão humana | allowlist e capacidade técnica exclusiva | identidade gerenciada/certificado produtivo |
| Reuso de autorização de revisão | registro opaco append-only, sem interpretação como credencial | trilha completa e retenção produtiva |
| Exfiltração de token no desktop | access token em memória e refresh token DPAPI | hardening do endpoint Windows |
| Path traversal/storage cruzado | chaves geradas pelo serviço e validação antes de I/O | backend e ACL reais |

## Testes implementados

- JWT RSA local: válido, assinatura, issuer, audience, expiração e claims obrigatórias inválidas.
- HTTP: 401 sem token/inválido, 403 sem capacidade, 404 para recurso de outra serventia e corpo incapaz de substituir o contexto validado.
- Matriz completa, componentes inativos, roles ignoradas, worker técnico e proibição de ações humanas/administrativas.
- Storage nega contexto cruzado, capacidade ausente e caminho arbitrário antes de I/O.
- Desktop verifica cliente público, configuração segura, DPAPI sem texto claro, round-trip e limpeza.
- SQL Server valida upgrade por duas migrations, unicidade `iss + sub`, perfil conhecido, FKs tenant-aware, append-only e persistência após reinício.

## Riscos e limitações

- O modelo não escolhe IdP, tenant OIDC, certificados, rotação, MFA ou topologia produtiva.
- A allowlist de worker é configuração, não provisionamento de identidade gerenciada.
- O teste HTTP usa endpoints exclusivos do host de testes; a API de produção continua sem endpoint funcional de negócio.
- DPAPI protege no contexto do usuário Windows, mas não substitui hardening, logout remoto ou política corporativa do dispositivo.
- O storage físico e suas ACLs não foram implementados.
- `Confirmado` continua significando decisão humana registrada, não verdade ou efeito jurídico.

## Evidências locais preliminares

- Restore completo com lock files atualizado: aprovado.
- Build Release serial: aprovado com 0 avisos e 0 erros.
- Suíte Windows: 168 testes aprovados; 11 testes SQL explicitamente delegados ao job Docker.
- Suíte SQL Server Docker: 13 testes aprovados, dos quais 11 exercitam SQL Server/Testcontainers.
- Validadores contratuais: 5 fixtures aprovadas e 4 mutações negativas rejeitadas.
- Script idempotente das duas migrations gerado com EF Core 10.0.12.
- Formatação restrita em modo `--verify-no-changes`: aprovada.
- Domain confirmado sem referência de projeto ou pacote.
- `git diff --check`: aprovado.

As URLs do PR, squash, CI e proteção serão registradas em PR documental pós-merge. S1-T08, Sprint 1 e G1 permanecem formalmente pendentes de S0-T05, validação externa e S1-T09.
