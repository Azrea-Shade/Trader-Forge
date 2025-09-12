<# 
Usage (on a Windows machine with .NET SDK):
  pwsh -File .\scripts\publish_win.ps1
Artifacts will land in: src\App\bin\Publish\win-x64\
#>
$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path | Split-Path -Parent
Push-Location $root
Write-Host "Publishing Windows self-contained build..." -ForegroundColor Cyan
dotnet restore src/App/App.csproj
dotnet publish src/App/App.csproj `
  -c Release `
  -p:PublishProfile=Properties\PublishProfiles\WinSelfContained.pubxml
Write-Host "Done. See: src\App\bin\Publish\win-x64\" -ForegroundColor Green
Pop-Location
