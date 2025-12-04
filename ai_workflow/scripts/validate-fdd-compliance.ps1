# Validate FDD Compliance Script (PowerShell)
# Validates that code follows Feature-Driven Development principles

param(
    [string]$CodeDir = ".",
    [string]$FeaturesDir = "docs/features",
    [string]$MasterList = "docs/features_master_list.md"
)

$ErrorActionPreference = "Stop"

Write-Host "[VALIDATE] Validating FDD compliance" -ForegroundColor Cyan
Write-Host ""

$Errors = 0
$Warnings = 0

# Function to check feature references in code
function Check-FeatureReferences {
    Write-Host "Checking feature references in code..."
    
    # Find code files (C# specific: exclude test files and generated files)
    $codeFiles = Get-ChildItem -Path $CodeDir -Recurse -File -Include *.cs | 
        Where-Object { 
            $_.FullName -notmatch '\\bin\\|\\obj\\|\\Tests\\|\\\.ai\\|AssemblyInfo\.cs|\.Designer\.cs|\.generated\.cs' -and
            $_.Name -notmatch 'Tests\.cs$|Test\.cs$'
        }
    
    foreach ($file in $codeFiles) {
        $content = Get-Content $file.FullName -Raw
        
        if ($content -notmatch 'Feature:') {
            Write-Host "[WARNING] Missing feature reference: $($file.FullName)" -ForegroundColor Yellow
            $script:Warnings++
        }
    }
}

# Function to check feature documentation completeness
function Check-FeatureDocs {
    Write-Host "Checking feature documentation completeness..."
    
    if (-not (Test-Path $FeaturesDir)) {
        Write-Host "[ERROR] Features directory not found: $FeaturesDir" -ForegroundColor Red
        $script:Errors++
        return
    }
    
    $RequiredDocs = @("README.md", "architecture.md", "roadmap.md", "use_cases.md", "testing.md", "code_location.md")
    
    # Check each feature folder (main features, not sub-features)
    $featureDirs = Get-ChildItem -Path $FeaturesDir -Directory | Where-Object {
        $_.Name -notmatch '^\d+\.\d+'  # Skip sub-features (pattern: N.M-sub-feature-name)
    }
    
    foreach ($featureDir in $featureDirs) {
        # Check if feature has sub-features
        $subFeatureDirs = Get-ChildItem -Path $featureDir.FullName -Directory | Where-Object {
            $_.Name -match '^\d+\.\d+'  # Sub-features pattern: N.M-sub-feature-name
        }
        
        # If feature has sub-features, only require README.md at main level
        # Otherwise, require all documents
        $docsToCheck = if ($subFeatureDirs.Count -gt 0) {
            @("README.md")  # Only README required if has sub-features
        } else {
            $RequiredDocs  # All documents required if no sub-features
        }
        
        foreach ($doc in $docsToCheck) {
            $docPath = Join-Path $featureDir.FullName $doc
            if (-not (Test-Path $docPath)) {
                Write-Host "[ERROR] Missing document: $docPath" -ForegroundColor Red
                $script:Errors++
            }
        }
    }
}

# Function to check master list consistency
function Check-MasterList {
    Write-Host "Checking master list consistency..."
    
    if (-not (Test-Path $MasterList)) {
        Write-Host "[ERROR] Master list not found: $MasterList" -ForegroundColor Red
        $script:Errors++
        return
    }
    
    $masterListContent = Get-Content $MasterList -Raw
    
    if (Test-Path $FeaturesDir) {
        $featureDirs = Get-ChildItem -Path $FeaturesDir -Directory | Where-Object {
            $_.Name -notmatch '\.'  # Skip sub-features
        }
        
        foreach ($featureDir in $featureDirs) {
            $featureName = $featureDir.Name
            if ($masterListContent -notmatch $featureName) {
                Write-Host "[WARNING] Feature not in master list: $featureName" -ForegroundColor Yellow
                $script:Warnings++
            }
        }
    }
}

# Function to check test organization
function Check-TestOrganization {
    Write-Host "Checking test organization..."
    
    $TestDir = "Tests"
    
    if (-not (Test-Path $TestDir)) {
        Write-Host "[WARNING] Test directory not found: $TestDir" -ForegroundColor Yellow
        $script:Warnings++
        return
    }
    
    Write-Host "  Test organization check (manual review recommended)"
}

# Run all checks
Check-FeatureReferences
Check-FeatureDocs
Check-MasterList
Check-TestOrganization

# Summary
Write-Host ""
Write-Host "=========================================="
if ($Errors -eq 0 -and $Warnings -eq 0) {
    Write-Host "[SUCCESS] FDD compliance validated successfully!" -ForegroundColor Green
    exit 0
} else {
    Write-Host "[FAILED] Validation completed with issues" -ForegroundColor Red
    Write-Host "  Errors: $Errors"
    Write-Host "  Warnings: $Warnings"
    exit 1
}

