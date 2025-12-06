# Build DLLs for Unity Integration
# This script builds all PokemonUltimate DLLs and copies them to a Unity project

param(
    [Parameter(Mandatory=$false)]
    [string]$UnityProjectPath = "",
    
    [Parameter(Mandatory=$false)]
    [string]$Configuration = "Release"
)

$ErrorActionPreference = "Stop"

Write-Host "=== Building PokemonUltimate DLLs for Unity ===" -ForegroundColor Cyan

# Get script directory (project root)
$ScriptDir = Split-Path -Parent $PSScriptRoot
$ProjectRoot = Split-Path -Parent $ScriptDir

# Build DLLs
Write-Host "`n[1/3] Building DLLs in $Configuration configuration..." -ForegroundColor Yellow
Push-Location $ProjectRoot

try {
    dotnet build -c $Configuration --no-incremental
    
    if ($LASTEXITCODE -ne 0) {
        throw "Build failed with exit code $LASTEXITCODE"
    }
    
    Write-Host "[OK] Build successful!" -ForegroundColor Green
}
catch {
    Write-Host "[ERROR] Build failed: $_" -ForegroundColor Red
    Pop-Location
    exit 1
}
finally {
    Pop-Location
}

# Find DLL output directories
$CoreDll = Get-ChildItem -Path "$ProjectRoot\PokemonUltimate.Core\bin\$Configuration\netstandard2.1" -Filter "PokemonUltimate.Core.dll" -ErrorAction SilentlyContinue
$CombatDll = Get-ChildItem -Path "$ProjectRoot\PokemonUltimate.Combat\bin\$Configuration\netstandard2.1" -Filter "PokemonUltimate.Combat.dll" -ErrorAction SilentlyContinue
$ContentDll = Get-ChildItem -Path "$ProjectRoot\PokemonUltimate.Content\bin\$Configuration\netstandard2.1" -Filter "PokemonUltimate.Content.dll" -ErrorAction SilentlyContinue

if (-not $CoreDll -or -not $CombatDll -or -not $ContentDll) {
    Write-Host "[ERROR] Could not find all DLLs. Please check build output." -ForegroundColor Red
    Write-Host "  Core: $($CoreDll.FullName)" -ForegroundColor Yellow
    Write-Host "  Combat: $($CombatDll.FullName)" -ForegroundColor Yellow
    Write-Host "  Content: $($ContentDll.FullName)" -ForegroundColor Yellow
    exit 1
}

Write-Host "`n[2/3] Found DLLs:" -ForegroundColor Yellow
Write-Host "  [OK] $($CoreDll.Name)" -ForegroundColor Green
Write-Host "  [OK] $($CombatDll.Name)" -ForegroundColor Green
Write-Host "  [OK] $($ContentDll.Name)" -ForegroundColor Green

# Copy to Unity if path provided
if ($UnityProjectPath -ne "") {
    if (-not (Test-Path $UnityProjectPath)) {
        Write-Host "[ERROR] Unity project path does not exist: $UnityProjectPath" -ForegroundColor Red
        exit 1
    }
    
    $PluginsPath = Join-Path $UnityProjectPath "Assets\Plugins"
    
    if (-not (Test-Path $PluginsPath)) {
        Write-Host "`n[3/3] Creating Plugins directory..." -ForegroundColor Yellow
        New-Item -ItemType Directory -Path $PluginsPath -Force | Out-Null
    }
    
    Write-Host "`n[3/3] Copying DLLs to Unity project..." -ForegroundColor Yellow
    Copy-Item -Path $CoreDll.FullName -Destination $PluginsPath -Force
    Copy-Item -Path $CombatDll.FullName -Destination $PluginsPath -Force
    Copy-Item -Path $ContentDll.FullName -Destination $PluginsPath -Force
    
    Write-Host "[OK] DLLs copied to: $PluginsPath" -ForegroundColor Green
}
else {
    Write-Host "`n[3/3] DLLs ready. Copy manually to Unity project:" -ForegroundColor Yellow
    Write-Host "  Source: $($CoreDll.DirectoryName)" -ForegroundColor Cyan
    Write-Host "  Destination: UnityProject/Assets/Plugins/" -ForegroundColor Cyan
    Write-Host "`nTo auto-copy, run:" -ForegroundColor Yellow
    Write-Host "  .\build-dlls-for-unity.ps1 -UnityProjectPath 'C:\Path\To\UnityProject'" -ForegroundColor White
}

Write-Host "`n=== Done ===" -ForegroundColor Cyan
