param(
    [string]$ApiKey
)

Set-Location $PSScriptRoot

Write-Host @"
███╗   ██╗██╗   ██╗ ██████╗ ███████╗████████╗
████╗  ██║██║   ██║██╔════╝ ██╔════╝╚══██╔══╝
██╔██╗ ██║██║   ██║██║  ███╗█████╗     ██║   
██║╚██╗██║██║   ██║██║   ██║██╔══╝     ██║   
██║ ╚████║╚██████╔╝╚██████╔╝███████╗   ██║   
╚═╝  ╚═══╝ ╚═════╝  ╚═════╝ ╚══════╝   ╚═╝   
"@

$nupkg = Get-ChildItem -Path . -Name "MakeKits.Tools.*.nupkg" |
    ForEach-Object {
        if ($_ -match 'MakeKits\.Tools\.(\d+\.\d+\.\d+)\.nupkg') {
            [PSCustomObject]@{ File = $_; Version = [version]$matches[1] }
        }
    } |
    Sort-Object Version -Descending |
    Select-Object -First 1 -ExpandProperty File

if (-not $nupkg) {
    Write-Host "MakeKits.Tools.*.nupkg not found"
    exit 1
}

$version = if ($nupkg -match '(\d+\.\d+\.\d+)\.nupkg') { $matches[1] }

$source = [System.IO.Path]::GetFullPath("$env:USERPROFILE\.nuget\packages\")

Write-Host "Adding MakeKits.Tools v$version ($nupkg) to $source"
& .\bin\nuget.exe add $nupkg -Source $source

$pkgDir = "$source\makekits.tools\$version"
$nupkgPath = "$pkgDir\$nupkg"
Write-Host "Extracting $nupkgPath to $pkgDir"
& .\bin\7z.exe x $nupkgPath -o"$pkgDir" -y

Write-Host "`nPress any key to exit..."
[void][System.Console]::ReadKey($true)
