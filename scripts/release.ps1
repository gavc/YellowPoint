Param(
    [string]$Configuration = "Release"
)

$ErrorActionPreference = "Stop"

$root = Split-Path -Parent $PSScriptRoot
$project = Join-Path $root "YellowPoint.csproj"
$outRoot = Join-Path $root "artifacts\local"
$fdOut = Join-Path $outRoot "framework-dependent"
$scOut = Join-Path $outRoot "self-contained"
$fdZip = Join-Path $outRoot "yellowpoint-win-x64-framework-dependent.zip"
$scZip = Join-Path $outRoot "yellowpoint-win-x64-self-contained.zip"

if (Test-Path $outRoot) { Remove-Item $outRoot -Recurse -Force }
New-Item -ItemType Directory -Path $fdOut | Out-Null
New-Item -ItemType Directory -Path $scOut | Out-Null

Write-Host "Publishing framework-dependent build (requires .NET runtime installed)..."
dotnet publish $project -c $Configuration -r win-x64 --self-contained false -p:PublishSingleFile=false -p:DebugType=embedded -o $fdOut

Write-Host "Publishing self-contained build (includes .NET runtime as separate files)..."
dotnet publish $project -c $Configuration -r win-x64 --self-contained true -p:PublishSingleFile=false -p:DebugType=embedded -o $scOut

# Remove PDB files if any remain
Get-ChildItem -Path $fdOut -Filter "*.pdb" -Recurse | Remove-Item -Force
Get-ChildItem -Path $scOut -Filter "*.pdb" -Recurse | Remove-Item -Force

if (Test-Path $fdZip) { Remove-Item $fdZip -Force }
if (Test-Path $scZip) { Remove-Item $scZip -Force }

Compress-Archive -Path (Join-Path $fdOut "*") -DestinationPath $fdZip
Compress-Archive -Path (Join-Path $scOut "*") -DestinationPath $scZip

Write-Host ""
Write-Host "Build completed successfully!" -ForegroundColor Green
Write-Host "Output directory: $outRoot"
Write-Host ""
Write-Host "Framework-dependent build:" -ForegroundColor Cyan
Write-Host "  - Requires .NET 10 runtime installed on target machine"
Write-Host "  - Smaller file size"
Write-Host "  - Location: $fdZip"
Write-Host ""
Write-Host "Self-contained build:" -ForegroundColor Cyan  
Write-Host "  - Includes .NET runtime files beside the app"
Write-Host "  - No dependencies required on target machine"
Write-Host "  - Location: $scZip"
