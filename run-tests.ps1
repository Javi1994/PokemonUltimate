# ============================================
# Pokemon Ultimate - Test Runner Script
# ============================================
# Usage:
#   .\run-tests.ps1              # Run all tests
#   .\run-tests.ps1 -Filter "Builder"    # Run tests containing "Builder"
#   .\run-tests.ps1 -Verbose     # Extra verbose output
#   .\run-tests.ps1 -Failed      # Show only failed tests
# ============================================

param(
    [string]$Filter = "",
    [switch]$Verbose,
    [switch]$Failed,
    [switch]$List
)

$ErrorActionPreference = "Continue"
$projectPath = "PokemonUltimate.Tests"

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Pokemon Ultimate - Test Runner" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Build first
Write-Host "[BUILD] Compiling project..." -ForegroundColor Yellow
$buildOutput = dotnet build --verbosity quiet 2>&1
if ($LASTEXITCODE -ne 0) {
    Write-Host "[BUILD] FAILED!" -ForegroundColor Red
    Write-Host $buildOutput
    exit 1
}
Write-Host "[BUILD] OK" -ForegroundColor Green
Write-Host ""

# Prepare test command
$testArgs = @(
    "test",
    $projectPath,
    "--no-build",
    "--logger", "console;verbosity=detailed"
)

if ($Filter) {
    $testArgs += "--filter"
    $testArgs += "FullyQualifiedName~$Filter"
    Write-Host "[FILTER] Running tests matching: $Filter" -ForegroundColor Magenta
}

if ($List) {
    $testArgs += "--list-tests"
    Write-Host "[LIST] Listing tests only (not running)" -ForegroundColor Magenta
}

Write-Host "[TEST] Running tests..." -ForegroundColor Yellow
Write-Host ""

# Run tests and capture output
$startTime = Get-Date
$output = & dotnet $testArgs 2>&1 | Out-String
$endTime = Get-Date
$duration = ($endTime - $startTime).TotalSeconds

# Parse and display results
$lines = $output -split "`n"

$passed = 0
$failed = 0
$skipped = 0
$failedTests = @()

foreach ($line in $lines) {
    $trimmed = $line.Trim()
    
    if ($trimmed -match "^\s*Passed\s+(\S+)") {
        $passed++
        if (-not $Failed) {
            Write-Host "  [PASS] $($Matches[1])" -ForegroundColor Green
        }
    }
    elseif ($trimmed -match "^\s*Failed\s+(\S+)") {
        $failed++
        $testName = $Matches[1]
        $failedTests += $testName
        Write-Host "  [FAIL] $testName" -ForegroundColor Red
    }
    elseif ($trimmed -match "^\s*Skipped\s+(\S+)") {
        $skipped++
        Write-Host "  [SKIP] $($Matches[1])" -ForegroundColor Yellow
    }
    elseif ($trimmed -match "Error Message:" -or $trimmed -match "Stack Trace:" -or $trimmed -match "Assert\.") {
        Write-Host "         $trimmed" -ForegroundColor DarkRed
    }
    elseif ($Verbose -and $trimmed.Length -gt 0) {
        Write-Host "  $trimmed" -ForegroundColor Gray
    }
}

# Summary
Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  SUMMARY" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

$totalTests = $passed + $failed + $skipped
Write-Host "  Total:   $totalTests tests" -ForegroundColor White
Write-Host "  Passed:  $passed" -ForegroundColor Green
if ($failed -gt 0) {
    Write-Host "  Failed:  $failed" -ForegroundColor Red
} else {
    Write-Host "  Failed:  $failed" -ForegroundColor Green
}
if ($skipped -gt 0) {
    Write-Host "  Skipped: $skipped" -ForegroundColor Yellow
}
Write-Host "  Time:    $([math]::Round($duration, 2))s" -ForegroundColor White
Write-Host ""

if ($failed -gt 0) {
    Write-Host "  RESULT: FAILED" -ForegroundColor Red
    Write-Host ""
    Write-Host "  Failed tests:" -ForegroundColor Red
    foreach ($test in $failedTests) {
        Write-Host "    - $test" -ForegroundColor Red
    }
    exit 1
} else {
    Write-Host "  RESULT: PASSED" -ForegroundColor Green
    exit 0
}

