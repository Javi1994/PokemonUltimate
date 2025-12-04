# Feature-Driven Development Guide

> **MANDATORY: This process MUST be followed for ALL development work.**

## Overview

This project uses **Feature-Driven Development** - every piece of work must be assigned to a feature (or sub-feature) before implementation begins. This ensures:

- **Organization** - All work is categorized and documented
- **Consistency** - Same structure and patterns across features
- **Discoverability** - Easy to find where code lives and what it does
- **Maintainability** - Clear documentation for future updates

## The Feature-Driven Development Process

### Step 0: Feature Discovery & Assignment ⭐ **MUST DO FIRST**

**Before writing ANY code or starting ANY work:**

#### 0.1 Review Existing Features

1. **Read `docs/features_master_list.md`**
   - This is the **master reference** for all features
   - Review all feature descriptions and sub-features
   - Understand what each feature covers

2. **Determine Feature Assignment**
   - **Does the work fit an existing feature?**
     - YES → Go to Step 0.2 (Assign to Existing Feature)
     - NO → Go to Step 0.3 (Create New Feature/Sub-Feature)

#### 0.2 Assign to Existing Feature

If work fits an existing feature:

1. **Read Feature's Complete Documentation**
   - `README.md` - Feature overview, status, sub-features
   - `architecture.md` - Technical specification
   - `roadmap.md` - Implementation plan
   - `use_cases.md` - All scenarios
   - `code_location.md` - Where code lives
   - `testing.md` - Testing strategy

2. **Determine Sub-Feature**
   - **Fits existing sub-feature?**
     - YES → Use that sub-feature
     - NO → Create new sub-feature folder with `README.md`

3. **Proceed to Standard Development Workflow** (Step 1)

#### 0.3 Create New Feature/Sub-Feature

If work doesn't fit any existing feature:

1. **Determine Feature Number**
   - Check `docs/features_master_list.md` for next available number
   - Features are numbered sequentially (1, 2, 3...)
   - Sub-features use decimal notation (1.1, 1.2, 2.1, 2.2...)

2. **Create Feature Folder**
   - Path: `docs/features/[N]-[feature-name]/`
   - Use kebab-case for folder name
   - Example: `docs/features/6-new-feature/`

3. **Create Complete Documentation Structure**
   
   **Required Documents** (follow `docs/feature_documentation_standard.md`):
   
   - **`README.md`** - Feature overview, status, sub-features list, quick links
   - **`architecture.md`** - Complete technical specification, design, APIs
   - **`roadmap.md`** - Implementation plan with phases, status, dependencies
   - **`use_cases.md`** - All scenarios, user stories, edge cases
   - **`testing.md`** - Testing strategy, test organization, coverage
   - **`code_location.md`** - Where code lives, namespaces, key classes
   
   **For Sub-Features:**
   - Create `[N.M]-[sub-feature-name]/` folder
   - Minimum: `README.md` (overview)
   - Optional: `architecture.md` (if complex sub-feature)

4. **Update Master List**
   - Add entry to `docs/features_master_list.md`
   - Include: Feature number, name, folder, status, description, sub-features

5. **Follow Documentation Standard**
   - Use `docs/feature_documentation_standard.md` as template
   - Ensure all required sections are present
   - Use numbered feature paths in all references

6. **Proceed to Standard Development Workflow** (Step 1)

### Standard Development Workflow (After Feature Assignment)

Once feature is assigned and documented:

1. **Read Context & Specs**
   - Read `.ai/context.md` - Current project state
   - Read feature's `architecture.md` or sub-feature `README.md`
   - Read feature's `code_location.md` - Find where code lives
   - **MANDATORY: Read existing code** - Review all related classes

2. **Verify Spec Completeness**
   - Ensure all details documented
   - Complete spec if incomplete

3. **TDD: Write Functional Tests**
   - Write tests first (red phase)
   - Follow `docs/ai/testing_structure_definition.md`

4. **Implement Feature**
   - Follow spec and existing patterns
   - **MANDATORY: Add feature references** - All code must reference its feature
   - Make tests pass (green phase)

5. **Write Edge Case Tests**
   - Test boundaries and invalid inputs
   - Implement missing functionality if discovered

6. **Write Integration Tests**
   - Test system interactions (if applicable)
   - Place in `Systems/[Module]/Integration/`

7. **Validate Against Use Cases**
   - Check feature's `use_cases.md`
   - Mark items complete

8. **Verify Implementation**
   - `dotnet build` - 0 warnings
   - `dotnet test` - All pass
   - Check `docs/ai/checklists/feature_complete.md`

9. **Update Documentation** ⭐ **MANDATORY AFTER EVERY FEATURE**

   **Feature Documentation** (update ALL relevant files):
   - **`roadmap.md`** - Mark completed phases/sub-features as ✅
   - **`architecture.md`** - Update if implementation differs from spec
   - **`use_cases.md`** - Mark completed use cases
   - **`code_location.md`** - Add new files/classes
   - **`testing.md`** - Document new tests and test organization
   
   **Master Documents:**
   - **`docs/features_master_list.md`** - Update feature status if changed
   - **`.ai/context.md`** - Update current project state
   
   **CRITICAL**: Documentation must always reflect actual implementation state

## Rules

### ⚠️ MANDATORY Rules

1. **NEVER start coding without feature assignment**
   - If feature not assigned → Stop and assign first
   - If feature doesn't exist → Create it first

2. **NEVER skip documentation creation**
   - New features MUST have complete documentation structure
   - New sub-features MUST have at least `README.md`

3. **NEVER skip documentation updates**
   - After every feature work → Update feature documentation
   - Documentation must reflect actual state

4. **ALWAYS use numbered feature paths**
   - ✅ `docs/features/2-combat-system/`
   - ❌ `docs/features/combat-system/`

5. **ALWAYS update master list**
   - New features → Add to `features_master_list.md`
   - Status changes → Update `features_master_list.md`

## Examples

### Example 1: Work Fits Existing Feature

**User Request**: "Add Stealth Rock move"

**Process**:
1. Read `docs/features_master_list.md`
2. Determine: Fits Feature 2: Combat System, Sub-Feature 2.14: Hazards System
3. Read `docs/features/2-combat-system/2.14-hazards-system/README.md`
4. Read `docs/features/2-combat-system/architecture.md`
5. Read `docs/features/2-combat-system/code_location.md`
6. Read existing hazards code
7. Proceed with implementation
8. Update `docs/features/2-combat-system/roadmap.md` (mark Stealth Rock complete)
9. Update `docs/features/2-combat-system/use_cases.md`

### Example 2: Work Needs New Sub-Feature

**User Request**: "Add Mega Evolution system"

**Process**:
1. Read `docs/features_master_list.md`
2. Determine: Fits Feature 1: Game Data, but needs new sub-feature
3. Check Feature 1 sub-features: 1.18 Variants System exists but is planned
4. Create `docs/features/1-game-data/1.18-variants-system/README.md`
5. Update `docs/features/1-game-data/README.md` to include new sub-feature
6. Read Feature 1 documentation
7. Proceed with implementation
8. Update all Feature 1 documentation files

### Example 3: Work Needs New Feature

**User Request**: "Add online multiplayer system"

**Process**:
1. Read `docs/features_master_list.md`
2. Determine: Doesn't fit any existing feature
3. Next feature number: 6
4. Create `docs/features/6-online-multiplayer/` folder
5. Create all 6 required documents (README, architecture, roadmap, use_cases, testing, code_location)
6. Update `docs/features_master_list.md` with Feature 6 entry
7. Proceed with implementation
8. Update all Feature 6 documentation files

## Checklist

Before starting ANY work:

- [ ] Read `docs/features_master_list.md`
- [ ] Determined feature assignment (existing or new)
- [ ] If existing → Read feature's complete documentation
- [ ] If new → Created feature folder and complete documentation structure
- [ ] Updated `docs/features_master_list.md` if new feature
- [ ] Ready to proceed with standard development workflow

After completing ANY work:

- [ ] Updated feature's `roadmap.md` - Marked completed phases
- [ ] Updated feature's `architecture.md` - Reflected actual implementation
- [ ] Updated feature's `use_cases.md` - Marked completed cases
- [ ] Updated feature's `code_location.md` - Added new files
- [ ] Updated feature's `testing.md` - Documented tests
- [ ] Updated `docs/features_master_list.md` - Updated status if changed
- [ ] Updated `.ai/context.md` - Current state

## Related Documents

- **Feature Master List**: `docs/features_master_list.md` ⭐ **START HERE**
- **Documentation Standard**: `docs/feature_documentation_standard.md`
- **Pre-Implementation Checklist**: `docs/ai/checklists/pre_implementation.md`
- **Feature Complete Checklist**: `docs/ai/checklists/feature_complete.md`
- **Development Workflow**: `.cursorrules`

---

**Remember**: Feature assignment and documentation are not optional - they are mandatory steps that ensure project organization and maintainability.

