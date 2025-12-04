#!/bin/bash
# Setup Project Script - AI Workflow Optimization
# Creates initial project structure for new projects

PROJECT_ROOT="${1:-.}"

echo "🚀 Setting up AI Workflow Optimization for project..."
echo ""

# Create directory structure
DIRECTORIES=(
    "docs"
    "docs/features"
    "Tests"
    ".ai"
)

echo "Creating directory structure..."
for dir in "${DIRECTORIES[@]}"; do
    FULL_PATH="$PROJECT_ROOT/$dir"
    if [ ! -d "$FULL_PATH" ]; then
        mkdir -p "$FULL_PATH"
        echo "  ✓ Created: $dir"
    else
        echo "  ⊙ Already exists: $dir"
    fi
done

# Check if .cursorrules exists
CURSORRULES_PATH="$PROJECT_ROOT/.cursorrules"
if [ ! -f "$CURSORRULES_PATH" ]; then
    TEMPLATE_PATH="$PROJECT_ROOT/ai_workflow/cursorrules-template.md"
    if [ -f "$TEMPLATE_PATH" ]; then
        cp "$TEMPLATE_PATH" "$CURSORRULES_PATH"
        echo "  ✓ Created: .cursorrules from template"
    else
        echo "  ⚠ Template not found: ai_workflow/cursorrules-template.md"
        echo "  → Create .cursorrules manually using the template"
    fi
else
    echo "  ⊙ Already exists: .cursorrules"
fi

# Check if .gitignore exists
GITIGNORE_PATH="$PROJECT_ROOT/.gitignore"
if [ ! -f "$GITIGNORE_PATH" ]; then
    GITIGNORE_TEMPLATE_PATH="$PROJECT_ROOT/ai_workflow/templates/gitignore-template"
    if [ -f "$GITIGNORE_TEMPLATE_PATH" ]; then
        cp "$GITIGNORE_TEMPLATE_PATH" "$GITIGNORE_PATH"
        echo "  ✓ Created: .gitignore from template"
    else
        echo "  ⚠ Template not found: ai_workflow/templates/gitignore-template"
        echo "  → Create .gitignore manually using the template"
    fi
else
    echo "  ⊙ Already exists: .gitignore"
fi

echo ""
echo "✅ Directory structure created!"
echo ""
echo "📋 Next Steps:"
echo "  1. Review .cursorrules and customize if needed"
echo "  2. Run Game Definition workflow:"
echo "     Say: 'Define game' or 'What is this game?'"
echo "  3. Start developing features!"
echo ""
echo "📚 Documentation:"
echo "  - START_HERE.md - Complete setup guide"
echo "  - INDEX.md - Complete index"
echo ""

