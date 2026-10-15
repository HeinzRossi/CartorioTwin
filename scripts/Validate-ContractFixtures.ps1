[CmdletBinding()]
param(
    [string]$SchemaPath,
    [string]$FixturesPath
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$contractsRoot = Join-Path $repositoryRoot 'contracts/v1'
if (-not $SchemaPath) {
    $SchemaPath = Join-Path $contractsRoot 'golden-case.schema.json'
}

if (-not $FixturesPath) {
    $FixturesPath = Join-Path $contractsRoot 'fixtures'
}
$utf8Strict = [System.Text.UTF8Encoding]::new($false, $true)
$caseIds = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)
$sourceIds = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)
$entityIds = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)

$schemaBytes = [System.IO.File]::ReadAllBytes($SchemaPath)
if ($schemaBytes.Length -ge 3 -and $schemaBytes[0] -eq 0xEF -and $schemaBytes[1] -eq 0xBB -and $schemaBytes[2] -eq 0xBF) {
    throw 'JSON Schema deve usar UTF-8 sem BOM.'
}

$schemaJson = $utf8Strict.GetString($schemaBytes)
$null = $schemaJson | ConvertFrom-Json -Depth 100

function Get-CanonicalSha256 {
    param([Parameter(Mandatory)][string]$Text)

    $bytes = [System.Text.Encoding]::UTF8.GetBytes($Text)
    $hash = [System.Security.Cryptography.SHA256]::HashData($bytes)
    return 'sha256:' + [Convert]::ToHexString($hash).ToLowerInvariant()
}

function Test-SurrogateBoundary {
    param(
        [Parameter(Mandatory)][string]$Text,
        [Parameter(Mandatory)][int]$Index
    )

    return $Index -gt 0 -and
        $Index -lt $Text.Length -and
        [char]::IsHighSurrogate($Text[$Index - 1]) -and
        [char]::IsLowSurrogate($Text[$Index])
}

$fixtures = @(Get-ChildItem -LiteralPath $FixturesPath -Filter '*.json' -File | Sort-Object Name)
if ($fixtures.Count -ne 5) {
    throw "O spike exige exatamente cinco fixtures; encontrados: $($fixtures.Count)."
}

foreach ($fixture in $fixtures) {
    $bytes = [System.IO.File]::ReadAllBytes($fixture.FullName)
    if ($bytes.Length -ge 3 -and $bytes[0] -eq 0xEF -and $bytes[1] -eq 0xBB -and $bytes[2] -eq 0xBF) {
        throw "$($fixture.Name): UTF-8 com BOM não é permitido."
    }

    $json = $utf8Strict.GetString($bytes)
    if (-not (Test-Json -Json $json -SchemaFile $SchemaPath)) {
        throw "$($fixture.Name): conteúdo não atende ao JSON Schema."
    }

    $document = $json | ConvertFrom-Json -Depth 100
    if ($document.synthetic -ne $true) {
        throw "$($fixture.Name): fixture deve ser explicitamente sintética."
    }

    if (-not $caseIds.Add([string]$document.caseId)) {
        throw "$($fixture.Name): caseId duplicado: $($document.caseId)."
    }

    if (-not $sourceIds.Add([string]$document.source.id)) {
        throw "$($fixture.Name): source.id duplicado: $($document.source.id)."
    }

    if (-not $entityIds.Add([string]$document.source.id)) {
        throw "$($fixture.Name): identificador de entidade duplicado: $($document.source.id)."
    }

    if ($document.source.nameSanitized.IndexOfAny([char[]]'\/:*?"<>|') -ge 0) {
        throw "$($fixture.Name): nameSanitized contém caractere de caminho."
    }

    $sensitivePattern = '(?i)(\b\d{3}\.\d{3}\.\d{3}-\d{2}\b|\b\d{2}\.\d{3}\.\d{3}/\d{4}-\d{2}\b|[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,}|\b(?:\d{1,3}\.){3}\d{1,3}\b)'
    if ($json -match $sensitivePattern) {
        throw "$($fixture.Name): possível PII ou endereço sensível encontrado."
    }

    $pagesByNumber = @{}
    foreach ($page in @($document.pages)) {
        $pageKey = [string]$page.number
        if ($pagesByNumber.ContainsKey($pageKey)) {
            throw "$($fixture.Name): página duplicada: $pageKey."
        }

        if ((Get-CanonicalSha256 -Text ([string]$page.text)) -ne $page.textSha256) {
            throw "$($fixture.Name): hash do texto da página $pageKey não confere."
        }

        $pagesByNumber[$pageKey] = $page
    }

    $actsById = @{}
    $fieldPathsByAct = @{}
    foreach ($act in @($document.acts)) {
        if ($actsById.ContainsKey([string]$act.id)) {
            throw "$($fixture.Name): act.id duplicado: $($act.id)."
        }

        if (-not $entityIds.Add([string]$act.id)) {
            throw "$($fixture.Name): identificador de entidade duplicado: $($act.id)."
        }

        $actsById[[string]$act.id] = $act
        $paths = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)
        foreach ($pageNumber in @($act.pages)) {
            if (-not $pagesByNumber.ContainsKey([string]$pageNumber)) {
                throw "$($fixture.Name): ato $($act.id) referencia página inexistente $pageNumber."
            }
        }

        foreach ($field in @($act.fields)) {
            if (-not $paths.Add([string]$field.path)) {
                throw "$($fixture.Name): campo duplicado no ato $($act.id): $($field.path)."
            }

            foreach ($evidence in @($field.evidences)) {
                if ($evidence.sourceId -ne $document.source.id -or
                    $evidence.sourceVersion -ne $document.source.version -or
                    $evidence.sourceSha256 -ne $document.source.sha256) {
                    throw "$($fixture.Name): evidência não pertence à fonte/version/hash do caso."
                }

                $pageKey = [string]$evidence.page
                if (-not $pagesByNumber.ContainsKey($pageKey)) {
                    throw "$($fixture.Name): evidência referencia página inexistente $pageKey."
                }

                $page = $pagesByNumber[$pageKey]
                if ($evidence.textSha256 -ne $page.textSha256) {
                    throw "$($fixture.Name): evidência usa hash de texto diferente da página $pageKey."
                }

                $start = [int]$evidence.startUtf16
                $end = [int]$evidence.endUtf16
                if ($start -lt 0 -or $end -gt $page.text.Length -or $start -ge $end) {
                    throw "$($fixture.Name): intervalo UTF-16 inválido na página $pageKey."
                }

                if ((Test-SurrogateBoundary -Text $page.text -Index $start) -or
                    (Test-SurrogateBoundary -Text $page.text -Index $end)) {
                    throw "$($fixture.Name): intervalo UTF-16 divide par substituto na página $pageKey."
                }

                $actualExcerpt = $page.text.Substring($start, $end - $start)
                if ($actualExcerpt -cne $evidence.excerpt) {
                    throw "$($fixture.Name): trecho não coincide com offsets na página $pageKey."
                }
            }
        }

        $fieldPathsByAct[[string]$act.id] = $paths
    }

    foreach ($act in @($document.acts)) {
        foreach ($relation in @($act.relations)) {
            if (-not $actsById.ContainsKey([string]$relation.targetActId)) {
                throw "$($fixture.Name): relação aponta para ato inexistente $($relation.targetActId)."
            }
        }
    }

    foreach ($review in @($document.reviews)) {
        if (-not $entityIds.Add([string]$review.id)) {
            throw "$($fixture.Name): identificador de entidade duplicado: $($review.id)."
        }

        $actId = [string]$review.actId
        if (-not $actsById.ContainsKey($actId)) {
            throw "$($fixture.Name): revisão aponta para ato inexistente $actId."
        }

        if (-not $fieldPathsByAct[$actId].Contains([string]$review.fieldPath)) {
            throw "$($fixture.Name): revisão aponta para campo inexistente $($review.fieldPath)."
        }
    }

    if ($null -ne $document.scenario -and -not $entityIds.Add([string]$document.scenario.id)) {
        throw "$($fixture.Name): identificador de entidade duplicado: $($document.scenario.id)."
    }

    if ($document.expected.status -in @('inconclusive', 'divergent') -and $document.expected.projectionAllowed) {
        throw "$($fixture.Name): caso inconclusivo/divergente não pode permitir projeção."
    }
}

$requiredCases = @('GD-001', 'GD-002', 'GD-003', 'GD-004', 'GD-005')
foreach ($requiredCase in $requiredCases) {
    if (-not $caseIds.Contains($requiredCase)) {
        throw "Fixture obrigatória ausente: $requiredCase."
    }
}

Write-Host "Contratos candidatos válidos: $($fixtures.Count) fixtures sintéticas."
