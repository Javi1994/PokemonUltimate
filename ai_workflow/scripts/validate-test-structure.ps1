# Validate Test Structure Script (PowerShell)
# Validates that test files follow the correct structure and naming conventions

param(
    [string]$TestDir = "Tests"
)

$ErrorActionPreference = "Stop"

Write-Host "[VALIDATE] Validating test structure in: $TestDir" -ForegroundColor Cyan
Write-Host ""

$Errors = 0
$Warnings = 0

# Function to check file naming
function Check-FileNaming {
    param([string]$File)
    
    $filename = Split-Path -Leaf $File
    
    # Check if filename follows pattern: [Component]Tests.[ext]
    if ($filename -notmatch "^[A-Z][a-zA-Z0-9]+Tests\.[a-z]+$") {
        Write-Host "[ERROR] Invalid file name: $filename" -ForegroundColor Red
        Write-Host "  Expected pattern: [Component]Tests.[ext]"
        Write-Host "  Example: AuthenticationServiceTests.cs"
        $script:Errors++
        return $false
    }
    
    return $true
}

# Function to check test method naming
function Check-MethodNaming {
    param([string]$File)
    
    $content = Get-Content $File -Raw
    
    # Check for C# test methods (NUnit)
    if ($content -match "\[Test\]|\[TestCase\]") {
        # Check if methods follow naming convention: MethodName_Scenario_ExpectedResult
        $testMethods = [regex]::Matches($content, '(?:public\s+void\s+)([A-Za-z0-9_]+)\s*\(')
        foreach ($match in $testMethods) {
            $methodName = $match.Groups[1].Value
            # Pattern: MethodName_Scenario_ExpectedResult (at least 2 underscores)
            if ($methodName -notmatch "^[A-Z][a-zA-Z0-9]+_[A-Z][a-zA-Z0-9]+_[A-Z][a-zA-Z0-9]+$") {
                Write-Host "WARNING: Test method doesn't follow naming convention: $methodName" -ForegroundColor Yellow
                Write-Host "  File: $File"
                Write-Host "  Expected pattern: MethodName_Scenario_ExpectedResult"
                Write-Host "  Example: Authenticate_ValidCredentials_ReturnsTrue"
                $script:Warnings++
            }
        }
    }
    
    # Check for old-style test methods
    if ($content -match "public void test|def test_|test\(") {
        Write-Host "[WARNING] Found test methods that may not follow naming convention" -ForegroundColor Yellow
        Write-Host "  File: $File"
        Write-Host "  Expected pattern: MethodName_Scenario_ExpectedResult"
        $script:Warnings++
    }
}

# Function to check required sections
function Check-RequiredSections {
    param([string]$File)
    
    $content = Get-Content $File -Raw
    
    # Check for Arrange, Act, Assert sections
    $hasArrange = $content -match "Arrange"
    $hasAct = $content -match "Act"
    $hasAssert = $content -match "Assert"
    if (-not ($hasArrange -and $hasAct -and $hasAssert)) {
        Write-Host "[WARNING] Missing required sections (Arrange, Act, Assert)" -ForegroundColor Yellow
        Write-Host "  File: $File"
        $script:Warnings++
    }
}

# Function to check feature reference
function Check-FeatureReference {
    param([string]$File)
    
    $content = Get-Content $File -Raw
    
    # Check for feature reference in comments (flexible: Feature: or **Feature** or Feature)
    if ($content -notmatch "(Feature:|Feature\s*:|Feature\s*=\s*|\*\*Feature\*\*)") {
        Write-Host "[WARNING] Missing feature reference" -ForegroundColor Yellow
        Write-Host "  File: $File"
        Write-Host "  Expected: Feature: [N]: [Feature Name] or **Feature**: [N]: [Feature Name]"
        $script:Warnings++
    }
}

# Main validation
if (-not (Test-Path $TestDir)) {
    Write-Host "[ERROR] Test directory not found: $TestDir" -ForegroundColor Red
    exit 1
}

# Find all test files (C# specific: *.Tests.cs or *Tests.cs)
$testFiles = Get-ChildItem -Path $TestDir -Recurse -File | Where-Object {
    ($_.Name -match "Tests\.cs$") -or ($_.Name -match "Test\.cs$")
}

foreach ($file in $testFiles) {
    Write-Host "Checking: $($file.FullName)"
    
    Check-FileNaming $file.FullName
    Check-MethodNaming $file.FullName
    Check-RequiredSections $file.FullName
    Check-FeatureReference $file.FullName
    
    Write-Host ""
}

# Summary
Write-Host "=========================================="
if ($Errors -eq 0 -and $Warnings -eq 0) {
    Write-Host "[SUCCESS] All tests validated successfully!" -ForegroundColor Green
    exit 0
} else {
    Write-Host "[FAILED] Validation completed with issues" -ForegroundColor Red
    Write-Host "  Errors: $Errors"
    Write-Host "  Warnings: $Warnings"
    exit 1
}

