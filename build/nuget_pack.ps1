Set-Location $PSScriptRoot

Write-Host @"
███╗   ██╗██╗   ██╗ ██████╗ ███████╗████████╗
████╗  ██║██║   ██║██╔════╝ ██╔════╝╚══██╔══╝
██╔██╗ ██║██║   ██║██║  ███╗█████╗     ██║   
██║╚██╗██║██║   ██║██║   ██║██╔══╝     ██║   
██║ ╚████║╚██████╔╝╚██████╔╝███████╗   ██║   
╚═╝  ╚═══╝ ╚═════╝  ╚═════╝ ╚══════╝   ╚═╝   
"@

# Nuget pack

$projects = @(
    "..\src\MakeKits.Workshop.Abstractions",
    "..\src\MakeKits.Workshop.Executable",
    "..\src\MakeKits.Workshop.Webview",
    "..\src\MakeKits.Workshop.WPF"
)

foreach ($proj in $projects) {
    Push-Location $proj
    Write-Host "Processing $proj..."
    dotnet restore /p:Configuration=Release
    dotnet build -c Release
    dotnet pack -c Release -o ../../build/
    Pop-Location
}

# Nuget tools publish

## Prepare makekits

$projects = @(
    "..\src\MakeKits.Cli"
)

foreach ($proj in $projects) {
    Push-Location $proj
    Write-Host "Processing $proj..."
    dotnet restore /p:Configuration=Release
    dotnet publish -c Release
    Pop-Location
}

Remove-Item -Force ".\makekits.exe" -ErrorAction SilentlyContinue
Copy-Item -Force "..\src\MakeKits.Cli\bin\Release\publish\MakeKits.Cli.exe" ".\"
Rename-Item -Force ".\MakeKits.Cli.exe" "makekits.exe"

## Prepare templates

$templateGitIgnore = @"
*
!.gitignore
"@

Remove-Item -Recurse -Force "..\template\MakeKits.Workshop.Webview.Default\obj" -ErrorAction SilentlyContinue
Remove-Item -Recurse -Force "..\template\MakeKits.Workshop.Webview.Default\bin" -ErrorAction SilentlyContinue
Remove-Item -Recurse -Force "..\template\MakeKits.Workshop.Webview.Default\Resources" -ErrorAction SilentlyContinue
$templateGitIgnore | Set-Content -LiteralPath "..\template\MakeKits.Workshop.Webview.Default\.gitignore"
& ".\bin\7z.exe" a ".\template\webview.7z" "..\template\MakeKits.Workshop.Webview.Default\*" -t7z -mx=5 -mf=BCJ2 -r -y
Remove-Item -LiteralPath "..\template\MakeKits.Workshop.Webview.Default\.gitignore" -Force

Remove-Item -Recurse -Force "..\template\MakeKits.Workshop.Executable.Default\obj" -ErrorAction SilentlyContinue
Remove-Item -Recurse -Force "..\template\MakeKits.Workshop.Executable.Default\bin" -ErrorAction SilentlyContinue
Remove-Item -Recurse -Force "..\template\MakeKits.Workshop.Executable.Default\Resources" -ErrorAction SilentlyContinue
$templateGitIgnore | Set-Content -LiteralPath "..\template\MakeKits.Workshop.Executable.Default\.gitignore"
& ".\bin\7z.exe" a ".\template\executable.7z" "..\template\MakeKits.Workshop.Executable.Default\*" -t7z -mx=5 -mf=BCJ2 -r -y
Remove-Item -LiteralPath "..\template\MakeKits.Workshop.Executable.Default\.gitignore" -Force

## Pack nuget tools

& ".\bin\nuget.exe" pack nuget.nuspec

Write-Host "`nPress any key to exit..."
[void][System.Console]::ReadKey($true)
