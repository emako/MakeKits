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

function Get-NugetPackageFileNameFromUrl {
    param([string]$Url)

    $path = ([Uri]$Url).AbsolutePath.TrimEnd('/')

    if ($path -match '\.nupkg$') {
        return [System.IO.Path]::GetFileName($path)
    }

    if ($path -match '/package/([^/]+)/([^/]+)$') {
        return "$($Matches[1]).$($Matches[2]).nupkg"
    }

    throw "Failed to resolve NuGet package file name from URL: $Url"
}

$packageUrl = "https://www.nuget.org/api/v2/package/MakeKits.Tools/0.0.6"
$nupkgFileName = Get-NugetPackageFileNameFromUrl $packageUrl
$packageFileName = [System.IO.Path]::GetFileNameWithoutExtension($nupkgFileName)
$cacheDir = Join-Path $PSScriptRoot ".cache"
$nupkgPath = Join-Path $cacheDir $nupkgFileName
$extractDir = Join-Path $cacheDir $packageFileName

Write-Host "Downloading NuGet package: $packageUrl"
New-Item -ItemType Directory -Path $cacheDir -Force | Out-Null
@"
*
!.gitignore
"@ | Set-Content -LiteralPath (Join-Path $cacheDir ".gitignore") -Encoding UTF8
Invoke-WebRequest -Uri $packageUrl -OutFile $nupkgPath -UseBasicParsing

Write-Host "Extracting to: $extractDir"
if (Test-Path $extractDir) {
    Remove-Item -LiteralPath $extractDir -Recurse -Force
}
New-Item -ItemType Directory -Path $extractDir -Force | Out-Null

Add-Type -AssemblyName System.IO.Compression.FileSystem
[System.IO.Compression.ZipFile]::ExtractToDirectory($nupkgPath, $extractDir)

Write-Host "Done. Output directory: $extractDir"
