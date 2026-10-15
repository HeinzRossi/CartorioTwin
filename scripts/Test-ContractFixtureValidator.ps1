[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$contractsRoot = Join-Path $repositoryRoot 'contracts/v1'
$schemaPath = Join-Path $contractsRoot 'golden-case.schema.json'
$sourceFixturesPath = Join-Path $contractsRoot 'fixtures'
$validatorPath = Join-Path $PSScriptRoot 'Validate-ContractFixtures.ps1'
$systemTempPath = [System.IO.Path]::GetFullPath([System.IO.Path]::GetTempPath())
$testRoot = Join-Path $systemTempPath ("cartorio-twin-contract-tests-" + [Guid]::NewGuid().ToString('N'))
$resolvedTestRoot = [System.IO.Path]::GetFullPath($testRoot)

if (-not $resolvedTestRoot.StartsWith($systemTempPath, [System.StringComparison]::OrdinalIgnoreCase)) {
    throw 'Diretório temporário saiu da raiz esperada.'
}

function Set-FixtureMutation {
    param(
        [Parameter(Mandatory)][string]$Path,
        [Parameter(Mandatory)][scriptblock]$Mutation
    )

    $document = Get-Content -LiteralPath $Path -Raw | ConvertFrom-Json -Depth 100
    & $Mutation $document
    $document | ConvertTo-Json -Depth 100 | Set-Content -LiteralPath $Path -Encoding utf8NoBOM
}

function Assert-ValidatorRejects {
    param(
        [Parameter(Mandatory)][string]$Name,
        [Parameter(Mandatory)][string]$FixtureName,
        [Parameter(Mandatory)][scriptblock]$Mutation
    )

    $casePath = Join-Path $testRoot $Name
    New-Item -ItemType Directory -Path $casePath | Out-Null
    Copy-Item -Path (Join-Path $sourceFixturesPath '*') -Destination $casePath
    Set-FixtureMutation -Path (Join-Path $casePath $FixtureName) -Mutation $Mutation

    $rejected = $false
    try {
        & $validatorPath -SchemaPath $schemaPath -FixturesPath $casePath *> $null
    }
    catch {
        $rejected = $true
    }

    if (-not $rejected) {
        throw "Validador aceitou caso negativo: $Name."
    }
}

try {
    New-Item -ItemType Directory -Path $testRoot | Out-Null

    Assert-ValidatorRejects -Name 'not-synthetic' -FixtureName 'GD-001-native-text.json' -Mutation {
        param($document)
        $document.synthetic = $false
    }

    Assert-ValidatorRejects -Name 'invalid-reference' -FixtureName 'GD-003-multipage-relation.json' -Mutation {
        param($document)
        $document.acts[1].relations[0].targetActId = 'aaaaaaaa-aaaa-4aaa-8aaa-aaaaaaaaaaaa'
    }

    Assert-ValidatorRejects -Name 'invalid-offset' -FixtureName 'GD-001-native-text.json' -Mutation {
        param($document)
        $document.acts[0].fields[0].evidences[0].endUtf16++
    }

    Assert-ValidatorRejects -Name 'invalid-projection' -FixtureName 'GD-005-ct001-divergence.json' -Mutation {
        param($document)
        $document.expected.projectionAllowed = $true
    }

    Write-Host 'Casos negativos rejeitados: 4.'
}
finally {
    if (Test-Path -LiteralPath $testRoot) {
        Remove-Item -LiteralPath $testRoot -Recurse -Force
    }
}
