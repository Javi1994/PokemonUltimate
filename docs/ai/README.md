# AI Documentation

> Documentation specifically designed for AI assistants working on this project.

## Overview

This directory contains documentation, standards, and prompts specifically designed to help AI assistants understand and work with this project effectively.

## Contents

### 📋 Project Standards

- **`../features_master_list.md`** - Master reference for all project features with standardized numbering and naming
- **`../feature_documentation_standard.md`** - Standard structure for all feature documentation
- **`guidelines/project_guidelines.md`** - 24+ coding rules and standards that AI must follow
- **`guidelines/feature_naming_in_code.md`** - How to reflect feature nomenclature in source code (XML docs, regions, attributes)
- **`anti-patterns.md`** - What NOT to do (with examples) - helps AI avoid common mistakes

### 🚀 Getting Started

- **`GETTING_STARTED.md`** - Quick start guide for new developers and AI assistants

### 🗺️ Technical Roadmap

- **`../implementation_plan.md`** - Overall technical roadmap and implementation status

### 🔄 Workflow

Process guides:
- **`workflow/troubleshooting.md`** - Problem-solving guide
- **`workflow/refactoring_guide.md`** - Safe refactoring process

### ✅ Checklists

Reusable checklists:
- **`checklists/pre_implementation.md`** - Before coding checklist
- **`checklists/feature_complete.md`** - Feature completion checklist
- **`checklists/pre_combat.md`** - Pre-combat checklist

### 📝 Examples

Code examples:
- **`examples/good_code.md`** - Correct patterns to follow
- **`examples/good_tests.md`** - Test patterns (functional, edge, integration)

### 💬 AI Prompts

Templates for AI interactions:
- **`prompts/new_feature.md`** - Template for implementing new features
- **`prompts/code_review.md`** - Template for code reviews
- **`prompts/edge_cases.md`** - Template for finding edge cases

## Usage

AI assistants should read these documents in this order:

1. **`../features_master_list.md`** - Understand feature numbering and structure
2. **`GETTING_STARTED.md`** - Understand project basics
3. **`guidelines/project_guidelines.md`** - Learn coding rules
4. **`anti-patterns.md`** - Learn what to avoid
5. **`../feature_documentation_standard.md`** - Understand documentation structure
6. **`../implementation_plan.md`** - Understand current project state

## Related Documentation

- **Feature-specific docs**: `docs/features/[N]-[feature-name]/` - Each feature has its own documentation (always use numbered format)
- **Testing strategy**: `docs/ai/testing_structure_definition.md` - Test structure standard

**⚠️ Feature Documentation**: Always use numbered paths:
- ✅ `docs/features/[N]-[feature-name]/` (e.g., `docs/features/2-combat-system/`)
- ❌ `docs/features/feature-name/` (wrong - missing number)

---

**Last Updated**: 2025-01-XX

