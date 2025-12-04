# 📋 Pre-Implementation Checklist

> **MANDATORY: Complete this checklist BEFORE writing any code for a new feature.**

---

## 0. 🎯 Feature Discovery & Assignment ⭐ **MUST DO FIRST**

- [ ] **Read `docs/features_master_list.md`** - Review all existing features and sub-features
- [ ] **Determine if work fits existing feature:**
  - [ ] If YES → Assign to existing feature (go to step 0.1)
  - [ ] If NO → Create new feature/sub-feature (go to step 0.2)

### 0.1 Assign to Existing Feature

- [ ] **Read feature's complete documentation:**
  - [ ] `README.md` - Feature overview and status
  - [ ] `architecture.md` - Technical specification
  - [ ] `roadmap.md` - Implementation plan
  - [ ] `use_cases.md` - All scenarios
  - [ ] `code_location.md` - Where code lives
- [ ] **Determine sub-feature:**
  - [ ] Fits existing sub-feature → Use that sub-feature
  - [ ] Needs new sub-feature → Create sub-feature folder and `README.md`
- [ ] **Proceed to Step 1** (Read Technical Documentation)

### 0.2 Create New Feature/Sub-Feature

- [ ] **Determine feature number** - Check `features_master_list.md` for next available
- [ ] **Create feature folder** - `docs/features/[N]-[feature-name]/`
- [ ] **Create complete documentation structure:**
  - [ ] `README.md` - Feature overview, status, sub-features list
  - [ ] `architecture.md` - Complete technical specification
  - [ ] `roadmap.md` - Implementation plan with phases
  - [ ] `use_cases.md` - All scenarios and behaviors
  - [ ] `testing.md` - Testing strategy
  - [ ] `code_location.md` - Where code lives and organization
- [ ] **For sub-features:** Create `[N.M]-[sub-feature-name]/` folder with `README.md`
- [ ] **Update `docs/features_master_list.md`** - Add new feature/sub-feature entry
- [ ] **Follow `docs/feature_documentation_standard.md`** - Use as template
- [ ] **Proceed to Step 1** (Read Technical Documentation)

## 1. 📚 Read Technical Documentation

- [ ] **Read assigned feature's documentation** - `docs/features/[N]-[feature-name]/` (always use numbered format)
- [ ] **Read the ENTIRE spec** (`architecture.md` or sub-feature `README.md`) - Don't skim, understand fully
- [ ] **Identify ALL components** mentioned in the spec
- [ ] **Note the expected API** - Method names, parameters, return types
- [ ] **Understand integration points** - How does this connect to other systems?
- [ ] **MANDATORY: Read `code_location.md`** - Find where existing code lives
- [ ] **MANDATORY: Read existing code files** - Review all related classes/interfaces before writing
- [ ] **MANDATORY: Understand existing patterns** - Match style, naming, structure of existing code

## 2. 📊 Analyze Requirements

For each component in the spec, categorize:

| Component | Status | Notes |
|-----------|--------|-------|
| [Name] | ✅ Implement Now / ⏳ Defer | [Why] |

### Questions to Answer:

- [ ] What is the **minimum viable implementation** for this phase?
- [ ] What elements are **required** for the system to function?
- [ ] What elements can be **safely deferred** to later phases?
- [ ] Are there **dependencies** on other unimplemented systems?

## 3. ✏️ Document Decisions

Before coding, write down:

```markdown
### Implementation Plan for [Feature Name]

**Spec Document**: `docs/features/[N]-[feature-name]/architecture.md` or sub-feature `architecture.md` (always use numbered format)

**Implementing Now:**
- Component A (required for basic functionality)
- Component B (required for basic functionality)

**Deferring to Later:**
- Component C (requires [dependency] not yet implemented)
- Component D (enhancement, not core functionality)

**API Changes from Spec:**
- Changed `PlayerSideSlots` to `PlayerSlots` (simpler naming)
- Added `X` not in spec (discovered during TDD)
```

## 4. 🔍 Verify Understanding

- [ ] Can I explain what this feature does in one sentence?
- [ ] Can I list all the classes/interfaces I need to create?
- [ ] Do I know what tests I'll write first?
- [ ] Have I identified potential edge cases?
- [ ] Do I know which systems this feature will interact with?
- [ ] Have I identified integration test scenarios?
- [ ] **MANDATORY: Have I read all existing code?** - No code writing until existing code is reviewed
- [ ] **MANDATORY: Do I understand existing patterns?** - Can I match the style and structure?
- [ ] **MANDATORY: Will I add feature references to XML docs?** - See `docs/ai/guidelines/feature_naming_in_code.md`
- [ ] **MANDATORY: Will ALL code have feature references?** - No exceptions

## 5. ✅ Get Approval (if working with team)

- [ ] Shared implementation plan with reviewer
- [ ] Addressed any concerns about deferred items
- [ ] Confirmed API naming conventions

---

## Quick Reference: Architecture Docs

| Feature | Spec Document |
|---------|---------------|
| Combat System | `combat_system_spec.md` |
| Battle Field | `battle_field_system.md` |
| Damage Calculation | `damage_and_effect_system.md` |
| Turn Order | `turn_order_system.md` |
| Status Effects | `status_and_stat_system.md` |
| Victory/Defeat | `victory_defeat_system.md` |
| Player/AI Input | `player_ai_spec.md` |
| UI Presentation | `ui_presentation_system.md` |

## Roadmaps Reference

| Feature Area | Roadmap Document |
|--------------|------------------|
| **Combat System** | `docs/features/2-combat-system/roadmap.md` |
| **Content Expansion** | `docs/features/3-content-expansion/roadmap.md` |
| **Pokemon Data Structure** | `docs/features/1-pokemon-data/roadmap.md` |
| **Variants System** | `docs/features/1-pokemon-data/1.3-variants-system/architecture.md` |
| **Unity Integration** | `docs/features/4-unity-integration/roadmap.md` |
| **Game Features** | `docs/features/5-game-features/roadmap.md` |
| **Test Structure** | `docs/ai/testing_structure_definition.md` |

**Before implementing a feature, check the relevant roadmap to understand:**
- Current phase status
- Dependencies
- Implementation specifications
- Test requirements
- Completion criteria

## Additional Resources

| Need | Document |
|------|----------|
| Troubleshooting | `docs/ai/workflow/troubleshooting.md` |
| Refactoring | `docs/ai/workflow/refactoring_guide.md` |
| Integration Tests | `docs/features/2-combat-system/testing/integration_guide.md` |
| All Features | `docs/features/README.md` |

---

## Example: Pre-Implementation for BattleSlot

```markdown
### Implementation Plan for BattleSlot

**Spec Document**: `docs/features/2-combat-system/2.1-battle-foundation/architecture.md`

**Implementing Now:**
- BattleSlot.Index (SlotIndex) ✅
- BattleSlot.Pokemon ✅
- BattleSlot.IsOccupied (as IsEmpty - inverted) ✅
- Stat stages management ✅
- Volatile status management ✅

**Deferring to Later:**
- BattleSlot.Effects (SlotEffects) - Requires Effect system
- BattleSlot.ActionProvider - Part of Phase 2.3 (Turn Order)
- BattleSlot.Side reference - Not needed for current phase

**API Changes from Spec:**
- `Index` → `SlotIndex` (clearer naming)
- `IsOccupied` → `IsEmpty` (inverted for clarity)
- Added `HasFainted` property (useful helper)
```

---

## 5. 🔗 Identify Integration Points

- [ ] Does this feature interact with other systems?
- [ ] Will it create actions that go through BattleQueue?
- [ ] Will it modify state that affects other systems?
- [ ] Is it a system boundary (e.g., Status → End-of-Turn)?
- [ ] If yes to any → Plan integration tests

## When to Skip This Checklist

**Never.** Even for small features, at minimum:
1. Identify the relevant spec (if any)
2. Note what you're implementing vs deferring
3. Identify integration points
4. Proceed with TDD

---

## After Implementation

1. Update the spec if you made improvements
2. Document any deferred items in the implementation plan
3. Add to `.ai/context.md` if significant changes
4. Mark use cases as complete (if combat-related)
5. Write integration tests if integration points identified

