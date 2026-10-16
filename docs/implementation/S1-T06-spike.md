# Spike S1-T06 — Integridade e serialização versionada

**Estado:** baseline técnica candidata integrada; validação externa pendente

**Data:** 16/10/2026

**Dependências:** baselines técnicas candidatas de S1-T01…S1-T05 integradas; estabilização formal depende de S0-T06 e validações externas

## Objetivo

Serializar e reidratar um pacote documental completo sem expor entidades de persistência, sem enfraquecer invariantes do Domain e sem antecipar manifesto ou JSON canônico de S1-T09.

## Rastreabilidade

- RNF-004: fontes, páginas, evidências, atos, direitos, fatos e revisões mantêm referências verificáveis durante o round-trip.
- RNF-011: Domain continua sem pacote, referência de projeto, `System.Text.Json`, I/O ou infraestrutura.
- ADR-004: objetos de domínio são reidratados exclusivamente pelas fábricas imutáveis existentes.
- ADR-006: serialização não promove fatos nem substitui revisão humana.
- ADR-013: versões, predecessores e revisões append-only são validados antes de aceitar o pacote.

## Wire format candidato

- DTOs públicos ficam em `CartorioTwin.Contracts.Documentos.V1`; serialização e mapeamento ficam em `CartorioTwin.Application.Documentos.V1`.
- O envelope exige `schemaVersion: "1.0-draft"` e contém coleções de fontes, textos de páginas, atos, direitos, fatos e revisões.
- GUIDs usam formato D minúsculo; hashes usam `sha256:` com 64 hexadecimais minúsculos; datas documentais usam `yyyy-MM-dd`; instantes técnicos usam formato round-trip UTC.
- Valores de fatos formam união fechada: texto, inteiro, booleano, fração exata e identidade UUID.
- JSON rejeita propriedades desconhecidas, nomes com caixa divergente, enums numéricos e discriminadores desconhecidos.
- A leitura retorna resultado tipado com `JsonInvalido`, `VersaoNaoSuportada`, `ReferenciaInvalida` ou `InvarianteDominio`.

## Integridade

- Fontes podem possuir múltiplas versões e o pacote pode conter múltiplas fontes.
- Página pertence a uma fonte/versão e seu SHA-256 deve corresponder ao texto integral.
- Evidência é reconstruída pelo texto integral e offsets UTF-16; hashes e trecho informados devem coincidir com o resultado derivado.
- Relações entre atos, direitos, titulares, fatos, revisões e predecessores devem apontar para itens existentes.
- IDs, fonte/versão/página e revisões não podem ser duplicados.
- Cadeias de fatos começam na versão 1, incrementam exatamente uma unidade, preservam ato/campo e não admitem predecessor ausente ou ciclo.
- Estado e proveniência serializados devem ser iguais aos derivados pela aplicação da revisão de domínio.

## Fixture sintética

`PKG-001-full-roundtrip.json` contém uma fonte, duas páginas, dois atos relacionados, um direito documentado e fatos que exercitam as cinco variantes de valor, três situações de evidência e uma cadeia com duas versões.

O arquivo é exclusivamente massa sintética de desenvolvimento. Não representa matrícula, pessoa, serventia, regra jurídica ou evidência profissional.

## Limites

- Sem JSON Schema novo; `golden-case.schema.json` e GD-001…GD-005 permanecem intocados.
- Sem JSON canônico, manifesto, hash lógico, assinatura ou comparação byte a byte; isso pertence à S1-T09.
- Sem persistência, EF Core, migration, API, WPF, RBAC ou autorização de sessão.
- Whitespace e ordem de propriedades não têm significado lógico.
- O contrato `1.0-draft` pode mudar após S0-T06, corpus autorizado e revisão profissional.

## Riscos

- A união fechada pode exigir nova versão ao surgir categoria de valor adicional.
- O pacote transporta texto integral de páginas; autorização, retenção e proteção de dados permanecem obrigatórias antes de conteúdo real.
- Resultado tipado não substitui validação de tamanho, autenticação ou autorização na futura borda HTTP.
- Serialização válida preserva forma e referências, mas não comprova efeito ou verdade jurídica.

## Evidências locais

- Validador de contratos existente: 5 fixtures sintéticas aprovadas.
- Casos negativos existentes: 4 mutações inválidas rejeitadas.
- `dotnet restore CartorioTwin.sln --locked-mode`: aprovado; lock files preservados.
- `dotnet build CartorioTwin.sln -c Release --no-restore -m:1`: aprovado com 0 avisos e 0 erros.
- `dotnet test CartorioTwin.sln -c Release --no-build -m:1`: 132 testes aprovados em 4 projetos; 109 no Domain e 21 no Application.
- Testes novos: 20 casos cobrindo round-trip completo, união fechada, múltiplas fontes, whitespace e 17 mutações inválidas.
- Formatação restrita: todos os arquivos C# de S1-T06 aprovados em modo `--verify-no-changes`.
- `dotnet list ... reference` e `dotnet list ... package --include-transitive`: Domain sem referência de projeto ou pacote; Contracts sem referência de projeto.
- Fixture `PKG-001` confirmada como UTF-8 sem BOM.
- `git diff --check`: aprovado.

- PR [#14](https://github.com/HeinzRossi/CartorioTwin/pull/14): aprovado por `build-and-test` em 1m32s e `secret-scan` em 6s, execução `38947820366`.
- Squash `1e9b2b4` integrado exclusivamente pelo PR #14.
- CI subsequente da `main`: `build-and-test` aprovado em 1m28s e `secret-scan` em 7s, execução `38948087650`.

S1-T06, Sprint 1 e G1 permanecem formalmente pendentes das validações externas e das demais dependências do gate.
