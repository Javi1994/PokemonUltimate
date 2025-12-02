# ✅ Feature Completion Checklist

> **Use this checklist before considering any feature complete.**
> All items must be checked before moving to the next feature.
>
> **Note**: Before starting, ensure you completed `pre_implementation.md` checklist!

---

## 📚 Spec Compliance

-   [ ] **Spec reviewed** - Read relevant `docs/architecture/` document
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
-   [ ] **Test structure** - Follows structure in `docs/testing/test_structure_definition.md`
-   [ ] **Test location** - Tests in correct directory (Systems/, Blueprints/, or Data/)
-   [ ] **Test-Driven Discovery** - All missing functionality revealed by tests has been implemented

## 🏗️ Code Quality

-   [ ] **No compiler warnings** - `dotnet build` shows 0 warnings
-   [ ] **No TODOs/FIXMEs** - All temporary markers resolved
-   [ ] **Follows guidelines** - Checked against `docs/project_guidelines.md`
-   [ ] **No anti-patterns** - Checked against `docs/anti-patterns.md`
-   [ ] **Constants centralized** - No magic strings, uses `ErrorMessages`/`GameMessages`

## 📐 Architecture

-   [ ] **Correct layer** - Code in appropriate project (Core/Content/Tests)
-   [ ] **Pattern compliance** - Uses Blueprint/Instance, Registry, Builder patterns correctly
-   [ ] **Interface segregation** - No god interfaces, single responsibility
-   [ ] **Dependencies** - No circular dependencies, proper direction

## 📚 Documentation

-   [ ] **XML docs** - Public APIs have `<summary>` comments
-   [ ] **Architecture docs** - Updated if new patterns introduced or API changed
-   [ ] **Context updated** - `.ai/context.md` reflects current state (phase, test count, completed systems)
-   [ ] **Decisions documented** - Architectural decisions added to `.ai/context.md` if significant
-   [ ] **Use cases updated** - Combat use cases marked complete (if applicable)

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

## Additional Resources

| Need | Document |
|------|----------|
| Troubleshooting | `docs/workflow/troubleshooting.md` |
| Refactoring | `docs/workflow/refactoring_guide.md` |
| Integration Tests | `docs/testing/integration_testing_guide.md` |

---

## Sign-Off

```
Feature: [NAME]
Date: [DATE]
Tests: [X] passing
Checklist: All items verified ✅
```
