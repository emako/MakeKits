Set-Location -LiteralPath $PSScriptRoot

$pkg = Join-Path -Path $env:USERPROFILE -ChildPath ".nuget\packages\makekits.tools"

if (-not (Test-Path -LiteralPath $pkg)) {
    Write-Error "Package not found:`n$pkg"
}

$versionDir = Get-ChildItem -LiteralPath $pkg -Directory |
    Sort-Object Name -Descending |
    Select-Object -First 1

if (-not $versionDir) {
    Write-Error "No installed version found."
}

$makekitsExe = Join-Path -Path $versionDir.FullName -ChildPath "build\makekits.exe"

if (-not (Test-Path -LiteralPath $makekitsExe)) {
    Write-Error "$makekitsExe not found."
}

& $makekitsExe $args
Read-Host "Press Enter to continue"
