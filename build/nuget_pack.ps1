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
    "..\src\MakeKits.Workshop.Abstractions"
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

### Pack template\webview.7z

Remove-Item -Recurse -Force "..\template\MakeKits.Workshop.Webview.Default\obj" -ErrorAction SilentlyContinue
Remove-Item -Recurse -Force "..\template\MakeKits.Workshop.Webview.Default\bin" -ErrorAction SilentlyContinue
Remove-Item -Recurse -Force "..\template\MakeKits.Workshop.Webview.Default\Resources" -ErrorAction SilentlyContinue
New-Item "..\template\MakeKits.Workshop.Webview.Default\Shared" -ItemType Directory -Force | Out-Null
Copy-Item "..\src\MakeKits.Workshop.Webview\*.cs" "..\template\MakeKits.Workshop.Webview.Default\Shared" -Force
(Get-Content "..\template\MakeKits.Workshop.Webview.Default\MakeKits.Workshop.Webview.Default.csproj") | Where-Object { $_.TrimStart() -notmatch '^<Import' } | Set-Content "..\template\MakeKits.Workshop.Webview.Default\MakeKits.Workshop.Webview.Default.csproj" -Encoding UTF8
$templateGitIgnore | Set-Content -LiteralPath "..\template\MakeKits.Workshop.Webview.Default\.gitignore"
& ".\bin\7z.exe" a ".\template\webview.7z" "..\template\MakeKits.Workshop.Webview.Default\*" -t7z -mx=5 -mf=BCJ2 -r -y
Remove-Item -LiteralPath "..\template\MakeKits.Workshop.Webview.Default\.gitignore" -Force
Remove-Item -LiteralPath "..\template\MakeKits.Workshop.Webview.Default\Shared" -Recurse -Force
git checkout -- "..\template\MakeKits.Workshop.Webview.Default\MakeKits.Workshop.Webview.Default.csproj"

### Pack template\webdbg.7z

Remove-Item -Recurse -Force "..\template\MakeKits.Workshop.Webview.Debugger\obj" -ErrorAction SilentlyContinue
Remove-Item -Recurse -Force "..\template\MakeKits.Workshop.Webview.Debugger\bin" -ErrorAction SilentlyContinue
Remove-Item -Recurse -Force "..\template\MakeKits.Workshop.Webview.Debugger\Resources" -ErrorAction SilentlyContinue
New-Item "..\template\MakeKits.Workshop.Webview.Debugger\Shared" -ItemType Directory -Force | Out-Null
Copy-Item "..\src\MakeKits.Workshop.Webview\*.cs" "..\template\MakeKits.Workshop.Webview.Debugger\Shared" -Force
(Get-Content "..\template\MakeKits.Workshop.Webview.Debugger\MakeKits.Workshop.Webview.Debugger.csproj") | Where-Object { $_.TrimStart() -notmatch '^<Import' } | Set-Content "..\template\MakeKits.Workshop.Webview.Debugger\MakeKits.Workshop.Webview.Debugger.csproj" -Encoding UTF8
$templateGitIgnore | Set-Content -LiteralPath "..\template\MakeKits.Workshop.Webview.Debugger\.gitignore"
& ".\bin\7z.exe" a ".\template\webdbg.7z" "..\template\MakeKits.Workshop.Webview.Debugger\*" -t7z -mx=5 -mf=BCJ2 -r -y
Remove-Item -LiteralPath "..\template\MakeKits.Workshop.Webview.Debugger\.gitignore" -Force
Remove-Item -LiteralPath "..\template\MakeKits.Workshop.Webview.Debugger\Shared" -Recurse -Force
git checkout -- "..\template\MakeKits.Workshop.Webview.Debugger\MakeKits.Workshop.Webview.Debugger.csproj"

### Pack template\executable.7z

Remove-Item -Recurse -Force "..\template\MakeKits.Workshop.Executable.Default\obj" -ErrorAction SilentlyContinue
Remove-Item -Recurse -Force "..\template\MakeKits.Workshop.Executable.Default\bin" -ErrorAction SilentlyContinue
Remove-Item -Recurse -Force "..\template\MakeKits.Workshop.Executable.Default\Resources" -ErrorAction SilentlyContinue
New-Item "..\template\MakeKits.Workshop.Executable.Default\Shared" -ItemType Directory -Force | Out-Null
Copy-Item "..\src\MakeKits.Workshop.Executable\*.cs" "..\template\MakeKits.Workshop.Executable.Default\Shared" -Force
(Get-Content "..\template\MakeKits.Workshop.Executable.Default\MakeKits.Workshop.Executable.Default.csproj") | Where-Object { $_.TrimStart() -notmatch '^<Import' } | Set-Content "..\template\MakeKits.Workshop.Executable.Default\MakeKits.Workshop.Executable.Default.csproj" -Encoding UTF8
$templateGitIgnore | Set-Content -LiteralPath "..\template\MakeKits.Workshop.Executable.Default\.gitignore"
& ".\bin\7z.exe" a ".\template\executable.7z" "..\template\MakeKits.Workshop.Executable.Default\*" -t7z -mx=5 -mf=BCJ2 -r -y
Remove-Item -LiteralPath "..\template\MakeKits.Workshop.Executable.Default\.gitignore" -Force
Remove-Item -LiteralPath "..\template\MakeKits.Workshop.Executable.Default\Shared" -Recurse -Force
git checkout -- "..\template\MakeKits.Workshop.Executable.Default\MakeKits.Workshop.Executable.Default.csproj"

### Pack template\console.7z

Remove-Item -Recurse -Force "..\template\MakeKits.Workshop.Console.Default\obj" -ErrorAction SilentlyContinue
Remove-Item -Recurse -Force "..\template\MakeKits.Workshop.Console.Default\bin" -ErrorAction SilentlyContinue
Remove-Item -Recurse -Force "..\template\MakeKits.Workshop.Console.Default\Resources" -ErrorAction SilentlyContinue
New-Item "..\template\MakeKits.Workshop.Console.Default\Shared" -ItemType Directory -Force | Out-Null
Copy-Item "..\src\MakeKits.Workshop.Executable\*.cs" "..\template\MakeKits.Workshop.Console.Default\Shared" -Force
(Get-Content "..\template\MakeKits.Workshop.Console.Default\MakeKits.Workshop.Console.Default.csproj") | Where-Object { $_.TrimStart() -notmatch '^<Import' } | Set-Content "..\template\MakeKits.Workshop.Console.Default\MakeKits.Workshop.Console.Default.csproj" -Encoding UTF8
$templateGitIgnore | Set-Content -LiteralPath "..\template\MakeKits.Workshop.Console.Default\.gitignore"
& ".\bin\7z.exe" a ".\template\console.7z" "..\template\MakeKits.Workshop.Console.Default\*" -t7z -mx=5 -mf=BCJ2 -r -y
Remove-Item -LiteralPath "..\template\MakeKits.Workshop.Console.Default\.gitignore" -Force
Remove-Item -LiteralPath "..\template\MakeKits.Workshop.Console.Default\Shared" -Recurse -Force
git checkout -- "..\template\MakeKits.Workshop.Console.Default\MakeKits.Workshop.Console.Default.csproj"

## Pack nuget tools

& ".\bin\nuget.exe" pack nuget.nuspec

Write-Host "`nPress any key to exit..."
[void][System.Console]::ReadKey($true)
