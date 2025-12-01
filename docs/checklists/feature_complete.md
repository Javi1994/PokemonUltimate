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
-   [ ] **Changes noted** - Any deviations from spec are documented
-   [ ] **Use cases validated** - Check `docs/combat_use_cases.md` (if combat)

---

## 🧪 Testing

-   [ ] **Functional tests pass** - Core behavior verified
-   [ ] **Edge case tests pass** - Boundaries and invalid inputs covered
-   [ ] **Real-world tests** - Verified against actual game data (when applicable)
-   [ ] **No skipped tests** - All tests enabled and passing
-   [ ] **Test naming** - Follows `MethodName_Scenario_ExpectedResult` pattern

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
-   [ ] **Architecture docs** - Updated if new patterns introduced
-   [ ] **Context updated** - `.ai/context.md` reflects current state

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

---

## Sign-Off

```
Feature: [NAME]
Date: [DATE]
Tests: [X] passing
Checklist: All items verified ✅
```
