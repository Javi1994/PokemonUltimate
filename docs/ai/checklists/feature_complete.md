# ✅ Feature Completion Checklist

> **Use this checklist before considering any feature complete.**
> All items must be checked before moving to the next feature.
>
> **Note**: Before starting, ensure you completed `pre_implementation.md` checklist!

---

## 📚 Spec Compliance

-   [ ] **Spec reviewed** - Read relevant `docs/features/[N]-[feature-name]/architecture.md` or sub-feature `architecture.md` document (always use numbered format)
-   [ ] **Components match spec** - API follows documented design
-   [ ] **Deferred items documented** - Listed what's postponed and why
-   [ ] **Changes noted** - Any deviations from spec are documented with rationale
-   [ ] **Use cases validated** - Check `docs/combat_use_cases.md` (if combat)
-   [ ] **Use cases marked complete** - Relevant items marked in use cases document

---

## 🧪 Testing

-   [ ] **Functional tests pass** - Core behavior verified
-   [ ] **Edge case tests pass** - Boundaries and invalid inputs covered
-   [ ] **Integration tests pass** - System interactions verified (if applicable)
-   [ ] **Real-world tests** - Verified against actual game data (when applicable)
-   [ ] **No skipped tests** - All tests enabled and passing
-   [ ] **Test naming** - Follows `MethodName_Scenario_ExpectedResult` pattern
-   [ ] **Test structure** - Follows structure in `docs/ai/testing_structure_definition.md`
-   [ ] **Test location** - Tests in correct directory (Systems/, Blueprints/, or Data/)
-   [ ] **Test-Driven Discovery** - All missing functionality revealed by tests has been implemented

## 🏗️ Code Quality

-   [ ] **No compiler warnings** - `dotnet build` shows 0 warnings
-   [ ] **No TODOs/FIXMEs** - All temporary markers resolved
-   [ ] **Follows guidelines** - Checked against `docs/ai/guidelines/project_guidelines.md`
-   [ ] **No anti-patterns** - Checked against `docs/ai/anti-patterns.md`
-   [ ] **Constants centralized** - No magic strings, uses `ErrorMessages`/`GameMessages`

## 📐 Architecture

-   [ ] **Correct layer** - Code in appropriate project (Core/Content/Tests)
-   [ ] **Pattern compliance** - Uses Blueprint/Instance, Registry, Builder patterns correctly
-   [ ] **Interface segregation** - No god interfaces, single responsibility
-   [ ] **Dependencies** - No circular dependencies, proper direction

## 📚 Documentation ⭐ **MANDATORY: Feature Documentation Updates**

-   [ ] **XML docs** - Public APIs have `<summary>` comments
-   [ ] **MANDATORY: Feature references in code** - ALL classes have `<remarks>` sections with feature numbers (see `docs/ai/guidelines/feature_naming_in_code.md`)
-   [ ] **MANDATORY: No code without feature reference** - Every class/interface references its feature
-   [ ] **MANDATORY: Existing code reviewed** - All related code files were read before implementation
-   [ ] **Feature Documentation Updated** ⭐ **MANDATORY AFTER EVERY FEATURE:**
  -   [ ] **`roadmap.md`** - Mark completed phases/sub-features as ✅
  -   [ ] **`architecture.md`** - Update if implementation differs from spec
  -   [ ] **`use_cases.md`** - Mark completed use cases
  -   [ ] **`code_location.md`** - Add new files/classes
  -   [ ] **`testing.md`** - Document new tests and test organization
-   [ ] **Master Documents Updated:**
  -   [ ] **`docs/features_master_list.md`** - Update feature status if changed
  -   [ ] **`.ai/context.md`** - Update current project state (phase, test count, completed systems)
-   [ ] **Decisions documented** - Architectural decisions added to `.ai/context.md` if significant
-   [ ] **CRITICAL**: Documentation must reflect actual implementation state

## 🔒 Error Handling

-   [ ] **Fail-fast** - Invalid inputs throw immediately
-   [ ] **No try-catch** - Unless absolutely necessary (I/O, external APIs)
-   [ ] **Clear messages** - Exceptions use `ErrorMessages` constants
-   [ ] **Validation** - Public methods validate parameters

---

## Quick Verification Commands

```bash
# Build with no warnings
dotnet build -v q

# Run all tests
dotnet test -v q

# Check for TODOs
grep -r "TODO\|FIXME" --include="*.cs" PokemonUltimate.Core PokemonUltimate.Content
```

## Roadmaps Reference

| Feature Area | Roadmap Document |
|--------------|------------------|
| **Combat System** | `docs/features/2-combat-system/roadmap.md` |
| **Content Expansion** | `docs/features/3-content-expansion/roadmap.md` |
| **Game Data Structure** | `docs/features/1-game-data/roadmap.md` |
| **Variants System** | `docs/features/1-game-data/1.18-variants-system/README.md` |
| **Unity Integration** | `docs/features/4-unity-integration/roadmap.md` |
| **Game Features** | `docs/features/5-game-features/roadmap.md` |
| **Test Structure** | `docs/ai/testing_structure_definition.md` |

**After completing a feature, update the relevant roadmap:**
- Mark phase as complete
- Update test counts
- Note any deviations from plan
- Document lessons learned

## Additional Resources

| Need | Document |
|------|----------|
| Troubleshooting | `docs/ai/workflow/troubleshooting.md` |
| Refactoring | `docs/ai/workflow/refactoring_guide.md` |
| Integration Tests | `docs/features/2-combat-system/testing/integration_guide.md` |
| All Features | `docs/features/README.md` |

---

## Sign-Off

```
Feature: [NAME]
Date: [DATE]
Tests: [X] passing
Checklist: All items verified ✅
```
