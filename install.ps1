Set-Location $PSScriptRoot
$ErrorActionPreference = "Stop"

Write-Host @"
███╗   ███╗ █████╗ ██╗  ██╗███████╗██╗  ██╗██╗████████╗███████╗
████╗ ████║██╔══██╗██║ ██╔╝██╔════╝██║ ██╔╝██║╚══██╔══╝██╔════╝
██╔████╔██║███████║█████╔╝ █████╗  █████╔╝ ██║   ██║   ███████╗
██║╚██╔╝██║██╔══██║██╔═██╗ ██╔══╝  ██╔═██╗ ██║   ██║   ╚════██║
██║ ╚═╝ ██║██║  ██║██║  ██╗███████╗██║  ██╗██║   ██║   ███████║
╚═╝     ╚═╝╚═╝  ╚═╝╚═╝  ╚═╝╚══════╝╚═╝  ╚═╝╚═╝   ╚═╝   ╚══════╝
"@

$nugetDir = Join-Path $env:TEMP '.nuget'

try {
    New-Item -ItemType Directory -Path $nugetDir -Force | Out-Null
    Push-Location $nugetDir

    $nugetExe = Join-Path $nugetDir 'nuget.exe'
    Invoke-WebRequest -Uri 'https://dist.nuget.org/win-x86-commandline/latest/nuget.exe' -OutFile $nugetExe

    & $nugetExe install MakeKits.Tools -Source 'https://api.nuget.org/v3/index.json'
    if ($LASTEXITCODE -ne 0) {
        exit $LASTEXITCODE
    }
}
finally {
    Pop-Location
    Remove-Item -LiteralPath $nugetDir -Recurse -Force -ErrorAction SilentlyContinue
}
