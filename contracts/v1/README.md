# Contratos candidatos v1

> **SINTÉTICO — NÃO É EVIDÊNCIA PROFISSIONAL.** Este diretório pertence ao spike S0-T06 e não representa contrato estabilizado, golden dataset validado ou conclusão de gate.

`golden-case.schema.json` usa JSON Schema Draft 2020-12 e reúne definições candidatas de fonte, página, evidência, ato, revisão, cenário e resultado esperado. `fixtures/` contém exatamente cinco casos sintéticos usados para testar forma, referências e offsets UTF-16.

Regras do spike:

- hashes usam `sha256:` seguido por 64 dígitos hexadecimais minúsculos;
- GUIDs usam formato D minúsculo;
- página e versão iniciam em 1;
- o trecho deve corresponder exatamente ao intervalo UTF-16 do texto da página;
- `synthetic` deve ser `true`;
- casos inconclusivos ou divergentes não permitem projeção;
- nenhum arquivo representa prática jurídica ou estadual validada.

Validação local:

```powershell
pwsh -NoProfile -File scripts/Validate-ContractFixtures.ps1
pwsh -NoProfile -File scripts/Test-ContractFixtureValidator.ps1
```

S0-T06 permanece bloqueada por S0-T02 e S0-T03. Os contratos podem mudar após corpus e catálogo profissionalmente validados.
