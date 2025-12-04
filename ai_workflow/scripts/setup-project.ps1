# Setup Project Script - AI Workflow Optimization
# Creates initial project structure for new projects

param(
    [string]$ProjectRoot = "."
)

Write-Host "Setting up AI Workflow Optimization for project..." -ForegroundColor Cyan
Write-Host ""

# Create directory structure
$directories = @(
    "docs",
    "docs/features",
    "Tests",
    ".ai"
)

Write-Host "Creating directory structure..." -ForegroundColor Yellow
foreach ($dir in $directories) {
    $fullPath = Join-Path $ProjectRoot $dir
    if (-not (Test-Path $fullPath)) {
        New-Item -ItemType Directory -Force -Path $fullPath | Out-Null
        Write-Host "  Created: $dir" -ForegroundColor Green
    } else {
        Write-Host "  Already exists: $dir" -ForegroundColor Gray
    }
}

# Check if .cursorrules exists
$cursorrulesPath = Join-Path $ProjectRoot ".cursorrules"
if (-not (Test-Path $cursorrulesPath)) {
    $templatePath = Join-Path $ProjectRoot "ai_workflow/cursorrules-template.md"
    if (Test-Path $templatePath) {
        Copy-Item $templatePath $cursorrulesPath
        Write-Host "  Created: .cursorrules from template" -ForegroundColor Green
    } else {
        Write-Host "  Warning: Template not found: ai_workflow/cursorrules-template.md" -ForegroundColor Yellow
        Write-Host "  -> Create .cursorrules manually using the template" -ForegroundColor Yellow
    }
} else {
    Write-Host "  Already exists: .cursorrules" -ForegroundColor Gray
}

# Check if .gitignore exists
$gitignorePath = Join-Path $ProjectRoot ".gitignore"
if (-not (Test-Path $gitignorePath)) {
    $gitignoreTemplatePath = Join-Path $ProjectRoot "ai_workflow/templates/gitignore-template"
    if (Test-Path $gitignoreTemplatePath) {
        Copy-Item $gitignoreTemplatePath $gitignorePath
        Write-Host "  Created: .gitignore from template" -ForegroundColor Green
    } else {
        Write-Host "  Warning: Template not found: ai_workflow/templates/gitignore-template" -ForegroundColor Yellow
        Write-Host "  -> Create .gitignore manually using the template" -ForegroundColor Yellow
    }
} else {
    Write-Host "  Already exists: .gitignore" -ForegroundColor Gray
}

Write-Host ""
Write-Host "Directory structure created!" -ForegroundColor Green
Write-Host ""
Write-Host "Next Steps:" -ForegroundColor Cyan
Write-Host "  1. Review .cursorrules and customize if needed" -ForegroundColor White
Write-Host "  2. Run Game Definition workflow:" -ForegroundColor White
Write-Host "     Say: Define game or What is this game?" -ForegroundColor White
Write-Host "  3. Start developing features!" -ForegroundColor White
Write-Host ""
Write-Host "Documentation:" -ForegroundColor Cyan
Write-Host "  - START_HERE.md - Complete setup guide" -ForegroundColor White
Write-Host "  - INDEX.md - Complete index" -ForegroundColor White
Write-Host ""
