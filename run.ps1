# Builds the Angular app (unless -SkipBuild) and runs the API serving it.
# Open http://localhost:5005 afterwards.
param([switch]$SkipBuild)
$ErrorActionPreference = 'Stop'
$root = $PSScriptRoot
$web = Join-Path $root 'frontend\secund-memory-web'
if (-not $SkipBuild) {
    Push-Location $web
    if (-not (Test-Path node_modules)) { npm install }
    npm run build
    if ($LASTEXITCODE -ne 0) { Pop-Location; throw 'frontend build failed' }
    Pop-Location
}
$env:Spa__Root = Join-Path $web 'dist\secund-memory-web\browser'
$env:ASPNETCORE_ENVIRONMENT = 'Development'
Set-Location (Join-Path $root 'backend\src\SecundMemory.Api')
dotnet run --launch-profile http
