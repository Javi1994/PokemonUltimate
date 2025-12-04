#!/bin/bash

# Validate FDD Compliance Script
# Validates that code follows Feature-Driven Development principles

set -e

# Colors
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Configuration
CODE_DIR="${1:-.}"
FEATURES_DIR="docs/features"
MASTER_LIST="docs/features_master_list.md"

echo "🔍 Validating FDD compliance"
echo ""

ERRORS=0
WARNINGS=0

# Function to check feature references in code
check_feature_references() {
    echo "Checking feature references in code..."
    
    # Find code files (adjust extensions as needed)
    find "$CODE_DIR" -type f \( -name "*.cs" -o -name "*.js" -o -name "*.py" \) \
        -not -path "*/bin/*" -not -path "*/obj/*" -not -path "*/node_modules/*" | \
    while read -r file; do
        # Check for feature reference in comments
        if ! grep -qi "Feature:" "$file" 2>/dev/null; then
            echo -e "${YELLOW}⚠ Missing feature reference: $file${NC}"
            ((WARNINGS++))
        fi
    done
}

# Function to check feature documentation completeness
check_feature_docs() {
    echo "Checking feature documentation completeness..."
    
    if [ ! -d "$FEATURES_DIR" ]; then
        echo -e "${RED}✗ Features directory not found: $FEATURES_DIR${NC}"
        ((ERRORS++))
        return
    fi
    
    # Required documents
    REQUIRED_DOCS=("README.md" "architecture.md" "roadmap.md" "use_cases.md" "testing.md" "code_location.md")
    
    # Check each feature folder
    find "$FEATURES_DIR" -mindepth 1 -maxdepth 1 -type d | while read -r feature_dir; do
        feature_name=$(basename "$feature_dir")
        
        # Skip if it's a sub-feature (contains dot)
        if [[ "$feature_name" =~ \. ]]; then
            continue
        fi
        
        for doc in "${REQUIRED_DOCS[@]}"; do
            if [ ! -f "$feature_dir/$doc" ]; then
                echo -e "${RED}✗ Missing document: $feature_dir/$doc${NC}"
                ((ERRORS++))
            fi
        done
    done
}

# Function to check master list consistency
check_master_list() {
    echo "Checking master list consistency..."
    
    if [ ! -f "$MASTER_LIST" ]; then
        echo -e "${RED}✗ Master list not found: $MASTER_LIST${NC}"
        ((ERRORS++))
        return
    fi
    
    # Check that all features in folders are in master list
    if [ -d "$FEATURES_DIR" ]; then
        find "$FEATURES_DIR" -mindepth 1 -maxdepth 1 -type d | while read -r feature_dir; do
            feature_name=$(basename "$feature_dir")
            
            # Skip sub-features
            if [[ "$feature_name" =~ \. ]]; then
                continue
            fi
            
            if ! grep -q "$feature_name" "$MASTER_LIST" 2>/dev/null; then
                echo -e "${YELLOW}⚠ Feature not in master list: $feature_name${NC}"
                ((WARNINGS++))
            fi
        done
    fi
}

# Function to check test organization
check_test_organization() {
    echo "Checking test organization..."
    
    TEST_DIR="${TEST_DIR:-Tests}"
    
    if [ ! -d "$TEST_DIR" ]; then
        echo -e "${YELLOW}⚠ Test directory not found: $TEST_DIR${NC}"
        ((WARNINGS++))
        return
    fi
    
    # Check that tests are organized by feature
    # This is a simplified check - adjust based on your structure
    echo "  Test organization check (manual review recommended)"
}

# Run all checks
check_feature_references
check_feature_docs
check_master_list
check_test_organization

# Summary
echo ""
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
if [ $ERRORS -eq 0 ] && [ $WARNINGS -eq 0 ]; then
    echo -e "${GREEN}✓ FDD compliance validated successfully!${NC}"
    exit 0
else
    echo -e "${RED}✗ Validation completed with issues${NC}"
    echo "  Errors: $ERRORS"
    echo "  Warnings: $WARNINGS"
    exit 1
fi

