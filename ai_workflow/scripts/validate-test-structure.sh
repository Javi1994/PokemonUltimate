#!/bin/bash

# Validate Test Structure Script
# Validates that test files follow the correct structure and naming conventions

set -e

# Colors
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Configuration
TEST_DIR="${1:-Tests}"
SCHEMA_FILE="ai_workflow/schemas/test-structure-schema.yaml"

echo "🔍 Validating test structure in: $TEST_DIR"
echo ""

ERRORS=0
WARNINGS=0

# Function to check file naming
check_file_naming() {
    local file="$1"
    local filename=$(basename "$file")
    
    # Check if filename follows pattern: [Component]Tests.[ext]
    if [[ ! "$filename" =~ ^[A-Z][a-zA-Z0-9]+Tests\.[a-z]+$ ]]; then
        echo -e "${RED}✗ Invalid file name: $filename${NC}"
        echo "  Expected pattern: [Component]Tests.[ext]"
        echo "  Example: AuthenticationServiceTests.cs"
        ((ERRORS++))
        return 1
    fi
    
    return 0
}

# Function to check test method naming
check_method_naming() {
    local file="$1"
    
    # Extract test method names (pattern: MethodName_Scenario_ExpectedResult)
    # This is a simplified check - adjust based on your language
    if grep -q "public void test" "$file" 2>/dev/null || \
       grep -q "def test_" "$file" 2>/dev/null || \
       grep -q "test(" "$file" 2>/dev/null; then
        echo -e "${YELLOW}⚠ Found test methods that may not follow naming convention${NC}"
        echo "  File: $file"
        echo "  Expected pattern: MethodName_Scenario_ExpectedResult"
        ((WARNINGS++))
    fi
}

# Function to check required sections
check_required_sections() {
    local file="$1"
    
    # Check for Arrange, Act, Assert sections
    # This is a simplified check - adjust based on your language
    if ! grep -q "Arrange\|Act\|Assert" "$file" 2>/dev/null; then
        echo -e "${YELLOW}⚠ Missing required sections (Arrange, Act, Assert)${NC}"
        echo "  File: $file"
        ((WARNINGS++))
    fi
}

# Function to check feature reference
check_feature_reference() {
    local file="$1"
    
    # Check for feature reference in comments
    if ! grep -qi "Feature:" "$file" 2>/dev/null; then
        echo -e "${YELLOW}⚠ Missing feature reference${NC}"
        echo "  File: $file"
        echo "  Expected: Feature: [N]: [Feature Name]"
        ((WARNINGS++))
    fi
}

# Main validation
if [ ! -d "$TEST_DIR" ]; then
    echo -e "${RED}✗ Test directory not found: $TEST_DIR${NC}"
    exit 1
fi

# Find all test files
find "$TEST_DIR" -type f \( -name "*Tests.*" -o -name "*Test.*" \) | while read -r file; do
    echo "Checking: $file"
    
    check_file_naming "$file"
    check_method_naming "$file"
    check_required_sections "$file"
    check_feature_reference "$file"
    
    echo ""
done

# Summary
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
if [ $ERRORS -eq 0 ] && [ $WARNINGS -eq 0 ]; then
    echo -e "${GREEN}✓ All tests validated successfully!${NC}"
    exit 0
else
    echo -e "${RED}✗ Validation completed with issues${NC}"
    echo "  Errors: $ERRORS"
    echo "  Warnings: $WARNINGS"
    exit 1
fi

