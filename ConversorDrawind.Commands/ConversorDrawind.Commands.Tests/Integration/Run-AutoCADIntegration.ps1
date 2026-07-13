param(
    [Parameter(Mandatory = $true)]
    [string]$DwgPath,

    [Parameter(Mandatory = $true)]
    [string]$TxmlPath,

    [string]$DllPath = "",

    [string]$CoreConsolePath = "C:\Program Files\Autodesk\AutoCAD 2026\accoreconsole.exe",

    [string[]]$Commands = @("DRAWINDCAD_Convert"),

    [string]$OutputDirectory = ""
)

$ErrorActionPreference = "Stop"

function Resolve-RequiredPath {
    param(
        [string]$PathValue,
        [string]$Description
    )

    if ([string]::IsNullOrWhiteSpace($PathValue)) {
        throw "$Description path is required."
    }

    $resolved = Resolve-Path -LiteralPath $PathValue -ErrorAction Stop
    return $resolved.ProviderPath
}

function Quote-ScrValue {
    param([string]$Value)
    return '"' + $Value.Replace('"', '\"') + '"'
}

function New-AutoCADScript {
    param(
        [string]$NetloadDll,
        [string[]]$CommandList
    )

    $lines = New-Object System.Collections.Generic.List[string]
    $lines.Add("FILEDIA")
    $lines.Add("0")
    $lines.Add("CMDECHO")
    $lines.Add("1")
    $lines.Add("SECURELOAD")
    $lines.Add("0")
    $lines.Add("NETLOAD")
    $lines.Add((Quote-ScrValue $NetloadDll))

    foreach ($command in $CommandList) {
        if (-not [string]::IsNullOrWhiteSpace($command)) {
            $lines.Add($command)
        }
    }

    $lines.Add("QSAVE")
    $lines.Add("QUIT")
    $lines.Add("N")
    return ($lines -join [Environment]::NewLine) + [Environment]::NewLine
}

function Get-FileSha256 {
    param([string]$PathValue)
    return (Get-FileHash -LiteralPath $PathValue -Algorithm SHA256).Hash
}

function Write-IntegrationManifest {
    param(
        [string]$ManifestPath,
        [string]$InputDwg,
        [string]$WorkingDwgPath,
        [string]$InputTxml,
        [string]$NetloadDll,
        [string]$CoreConsole,
        [string[]]$CommandList,
        [string]$TempConfigurationPath,
        [string]$GeneratedScriptPath,
        [string]$StdoutLogPath,
        [string]$StderrLogPath,
        [int]$ExitCode,
        [string]$Status,
        [string[]]$FailureReasons = @()
    )

    $finishedAt = [DateTimeOffset]::Now
    $startedAt = [DateTimeOffset]::Parse($script:RunStartedAt)
    $elapsedMilliseconds = [int64]($finishedAt - $startedAt).TotalMilliseconds

    $manifest = [ordered]@{
        status = $Status
        startedAt = $script:RunStartedAt
        finishedAt = $finishedAt.ToString("o")
        elapsedMilliseconds = $elapsedMilliseconds
        exitCode = $ExitCode
        commands = $CommandList
        failureReasons = $FailureReasons
        paths = [ordered]@{
            inputDwg = $InputDwg
            workingDwg = $WorkingDwgPath
            txml = $InputTxml
            dll = $NetloadDll
            coreConsole = $CoreConsole
            tempConfiguration = $TempConfigurationPath
            script = $GeneratedScriptPath
            stdoutLog = $StdoutLogPath
            stderrLog = $StderrLogPath
        }
        bytes = [ordered]@{
            inputDwg = (Get-Item -LiteralPath $InputDwg).Length
            workingDwg = if (Test-Path -LiteralPath $WorkingDwgPath) { (Get-Item -LiteralPath $WorkingDwgPath).Length } else { $null }
            txml = (Get-Item -LiteralPath $InputTxml).Length
            dll = (Get-Item -LiteralPath $NetloadDll).Length
            stdoutLog = if (Test-Path -LiteralPath $StdoutLogPath) { (Get-Item -LiteralPath $StdoutLogPath).Length } else { $null }
            stderrLog = if (Test-Path -LiteralPath $StderrLogPath) { (Get-Item -LiteralPath $StderrLogPath).Length } else { $null }
        }
        sha256 = [ordered]@{
            inputDwg = Get-FileSha256 $InputDwg
            workingDwg = if (Test-Path -LiteralPath $WorkingDwgPath) { Get-FileSha256 $WorkingDwgPath } else { $null }
            txml = Get-FileSha256 $InputTxml
            dll = Get-FileSha256 $NetloadDll
            script = if (Test-Path -LiteralPath $GeneratedScriptPath) { Get-FileSha256 $GeneratedScriptPath } else { $null }
        }
    }

    $manifest | ConvertTo-Json -Depth 5 | Set-Content -LiteralPath $ManifestPath -Encoding UTF8
}

function Read-ConsoleLogText {
    param([string]$LogPath)

    if (-not (Test-Path -LiteralPath $LogPath)) {
        return ""
    }

    $bytes = [System.IO.File]::ReadAllBytes($LogPath)
    $text = [System.Text.Encoding]::UTF8.GetString($bytes)
    return $text.Replace([string][char]0, "").TrimStart([char]0xFEFF)
}

function Get-IntegrationFailureReasons {
    param(
        [string]$LogPath,
        [string[]]$CommandList
    )

    $logText = Read-ConsoleLogText $LogPath
    $reasons = New-Object System.Collections.Generic.List[string]

    if ($logText -match "Unable to load .* assembly") {
        $reasons.Add("NETLOAD did not load the DLL.")
    }

    foreach ($command in $CommandList) {
        if ($logText -match ("Unknown command\s+`"" + [regex]::Escape($command.ToUpperInvariant()) + "`"")) {
            $reasons.Add("AutoCAD did not recognize command '$command'.")
        }
    }

    if ($logText -match "Unhandled Exception|System\.Exception|Fatal Error") {
        $reasons.Add("AutoCAD log contains an exception or fatal error.")
    }

    return $reasons.ToArray()
}

$script:RunStartedAt = (Get-Date).ToString("o")
$dwg = Resolve-RequiredPath $DwgPath "DWG"
$txml = Resolve-RequiredPath $TxmlPath "TXML"
$coreConsole = Resolve-RequiredPath $CoreConsolePath "AutoCAD Core Console"

if ([string]::IsNullOrWhiteSpace($DllPath)) {
    $scriptRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
    $defaultDll = Join-Path $scriptRoot "..\..\ConversorDrawindDLL\bin\Debug\ConversorDrawind.Commands.dll"
    $DllPath = $defaultDll
}

$dll = Resolve-RequiredPath $DllPath "ConversorDrawind DLL"

if ($Commands.Count -eq 0) {
    throw "At least one AutoCAD command is required."
}

if ([string]::IsNullOrWhiteSpace($OutputDirectory)) {
    $OutputDirectory = Join-Path (Split-Path -Parent $dwg) ("integration-output-" + (Get-Date -Format "yyyyMMdd-HHmmss"))
}

if (-not (Test-Path -LiteralPath $OutputDirectory)) {
    New-Item -ItemType Directory -Path $OutputDirectory | Out-Null
}

$workingDwg = Join-Path $OutputDirectory (Split-Path -Leaf $dwg)
Copy-Item -LiteralPath $dwg -Destination $workingDwg -Force

$tempConfig = Join-Path ([System.IO.Path]::GetTempPath()) "ConversorDrawind.Temp"
Set-Content -LiteralPath $tempConfig -Value $txml -Encoding UTF8

$scriptPath = Join-Path $OutputDirectory "run-conversor.scr"
$scriptContent = New-AutoCADScript -NetloadDll $dll -CommandList $Commands
Set-Content -LiteralPath $scriptPath -Value $scriptContent -Encoding ASCII

$stdoutPath = Join-Path $OutputDirectory "accoreconsole.out.log"
$stderrPath = Join-Path $OutputDirectory "accoreconsole.err.log"
$manifestPath = Join-Path $OutputDirectory "integration-manifest.json"

Write-Host "DWG copy: $workingDwg"
Write-Host "TXML temp pointer: $tempConfig"
Write-Host "Script: $scriptPath"
Write-Host "Commands: $($Commands -join ', ')"

try {
    & $coreConsole /i $workingDwg /s $scriptPath *> $stdoutPath
    $exitCode = $LASTEXITCODE
    $failureReasons = Get-IntegrationFailureReasons -LogPath $stdoutPath -CommandList $Commands

    if ($exitCode -ne 0 -or $failureReasons.Count -gt 0) {
        $failureMessage = "AutoCAD Core Console failed with exit code $exitCode."
        if ($failureReasons.Count -gt 0) {
            $failureMessage += " " + ($failureReasons -join " ")
        }
        $failureMessage | Set-Content -LiteralPath $stderrPath -Encoding UTF8
        Write-IntegrationManifest `
            -ManifestPath $manifestPath `
            -InputDwg $dwg `
            -WorkingDwgPath $workingDwg `
            -InputTxml $txml `
            -NetloadDll $dll `
            -CoreConsole $coreConsole `
            -CommandList $Commands `
            -TempConfigurationPath $tempConfig `
            -GeneratedScriptPath $scriptPath `
            -StdoutLogPath $stdoutPath `
            -StderrLogPath $stderrPath `
            -ExitCode $exitCode `
            -Status "failed" `
            -FailureReasons $failureReasons
        throw "AutoCAD integration failed. See $stdoutPath, $stderrPath and $manifestPath."
    }

    Write-IntegrationManifest `
        -ManifestPath $manifestPath `
        -InputDwg $dwg `
        -WorkingDwgPath $workingDwg `
        -InputTxml $txml `
        -NetloadDll $dll `
        -CoreConsole $coreConsole `
        -CommandList $Commands `
        -TempConfigurationPath $tempConfig `
        -GeneratedScriptPath $scriptPath `
        -StdoutLogPath $stdoutPath `
        -StderrLogPath $stderrPath `
        -ExitCode $exitCode `
        -Status "passed" `
        -FailureReasons @()
}
catch {
    if (-not (Test-Path -LiteralPath $manifestPath)) {
        Write-IntegrationManifest `
            -ManifestPath $manifestPath `
            -InputDwg $dwg `
            -WorkingDwgPath $workingDwg `
            -InputTxml $txml `
            -NetloadDll $dll `
            -CoreConsole $coreConsole `
            -CommandList $Commands `
            -TempConfigurationPath $tempConfig `
            -GeneratedScriptPath $scriptPath `
            -StdoutLogPath $stdoutPath `
            -StderrLogPath $stderrPath `
            -ExitCode 1 `
            -Status "failed"
    }
    throw
}

Write-Host "Integration run completed. Logs: $stdoutPath"
Write-Host "Manifest: $manifestPath"
