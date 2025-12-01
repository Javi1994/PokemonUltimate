# Contributing Guide

> Git conventions, workflow, and development practices for PokemonUltimate.

## Git Workflow

We use **GitHub Flow** (simplified):

```
main                         ← Always stable, all tests pass
  │
  ├── feature/combat-queue   ← New functionality
  ├── fix/type-chart-fairy   ← Bug fixes
  ├── test/registry-queries  ← Test additions
  └── docs/unity-setup       ← Documentation
```

### Branch Naming

```
<type>/<short-description>
```

| Prefix | Use | Example |
|--------|-----|---------|
| `feature/` | New functionality | `feature/damage-calculator` |
| `fix/` | Bug fixes | `fix/evolution-null-check` |
| `test/` | Test additions | `test/builder-edge-cases` |
| `docs/` | Documentation only | `docs/combat-system-spec` |
| `refactor/` | Code restructuring | `refactor/split-pokemon-instance` |

**Rules:**
- Use `kebab-case` (lowercase with hyphens)
- Keep it short but descriptive
- Branch from `main`, merge back to `main`

---

## Commit Messages

We follow **Conventional Commits**:

```
<type>(<scope>): <description>

[optional body]
[optional footer]
```

### Types

| Type | When to Use | Example |
|------|-------------|---------|
| `feat` | New feature | `feat(combat): add BattleQueue` |
| `fix` | Bug fix | `fix(types): correct Dragon immunity` |
| `test` | Adding/updating tests | `test(registry): add query edge cases` |
| `docs` | Documentation | `docs: add unity integration guide` |
| `refactor` | Code change (no new feature/fix) | `refactor(instance): extract battle methods` |
| `chore` | Maintenance tasks | `chore: update dependencies` |
| `style` | Formatting only | `style: fix indentation` |
| `perf` | Performance improvement | `perf(calc): optimize stat calculation` |

### Scopes (Optional)

| Scope | Area |
|-------|------|
| `core` | PokemonUltimate.Core project |
| `content` | PokemonUltimate.Content project |
| `tests` | PokemonUltimate.Tests project |
| `combat` | Battle/combat system |
| `pokemon` | Pokemon data/instances |
| `moves` | Move data/effects |
| `evolution` | Evolution system |
| `registry` | Data registries |
| `factory` | Factories/builders |
| `types` | Type effectiveness |
| `stats` | Stat calculations |

### Examples

```bash
# Features
feat(combat): implement action queue processing
feat(pokemon): add friendship evolution condition
feat(moves): add multi-hit effect

# Fixes
fix(evolution): handle missing target species
fix(types): correct Fairy vs Dragon effectiveness
fix(stats): clamp IVs to valid range

# Tests
test(registry): add empty result edge cases
test(evolution): verify 3-stage chains
test: add 50 type effectiveness edge cases

# Documentation
docs: create unity integration guide
docs(combat): update damage formula spec

# Refactoring
refactor(instance): split into partial classes
refactor: extract error messages to constants

# Chores
chore: configure .gitignore for Unity
chore: add MIT license
```

### Commit Message Rules

1. **Imperative mood**: "add feature" not "added feature"
2. **Lowercase**: Start with lowercase after type
3. **No period**: Don't end with a period
4. **Max 72 chars**: Keep first line short
5. **Body for details**: Use body for complex changes

```bash
# Good
feat(combat): add damage calculator pipeline

# Bad
feat(Combat): Added the damage calculator.
```

---

## Development Workflow

### Starting New Work

```bash
# 1. Ensure main is up to date
git checkout main
git pull origin main

# 2. Create feature branch
git checkout -b feature/my-feature

# 3. Develop with frequent commits
git commit -m "feat(scope): add initial implementation"
git commit -m "test(scope): add unit tests"
git commit -m "feat(scope): handle edge cases"
```

### Before Merging

```bash
# 1. Run all tests
dotnet test

# 2. Check for warnings
dotnet build

# 3. Verify test count hasn't decreased
# Current: 1,165 tests

# 4. Update documentation if needed
```

### Merging to Main

```bash
# 1. Switch to main
git checkout main

# 2. Merge feature branch
git merge feature/my-feature

# 3. Push
git push origin main

# 4. Delete feature branch (optional)
git branch -d feature/my-feature
```

---

## Version Tags

We use **Semantic Versioning**:

```
v<MAJOR>.<MINOR>.<PATCH>
```

| Change | Version | Example |
|--------|---------|---------|
| Breaking change | MAJOR | v1.0.0 → v2.0.0 |
| New feature | MINOR | v1.0.0 → v1.1.0 |
| Bug fix | PATCH | v1.0.0 → v1.0.1 |

### Current Milestones

| Version | Milestone | Status |
|---------|-----------|--------|
| v0.1.0 | Data Foundation | ✅ Complete |
| v0.2.0 | Combat MVP | 🎯 Next |
| v0.3.0 | Unity Integration | ⏳ Pending |
| v1.0.0 | First Playable | ⏳ Pending |

### Creating a Tag

```bash
git tag -a v0.1.0 -m "Data Foundation complete"
git push origin v0.1.0
```

---

## Code Review Checklist

Before merging, verify:

- [ ] All tests pass (`dotnet test`)
- [ ] No build warnings (`dotnet build`)
- [ ] New code has tests
- [ ] Edge cases covered
- [ ] No magic strings/numbers (use constants)
- [ ] XML documentation on public APIs
- [ ] Follows project guidelines (`docs/project_guidelines.md`)

---

## TDD Workflow

This project uses **Test-Driven Development**:

```
1. Write failing test
2. Implement minimum code to pass
3. Refactor if needed
4. Add edge case tests
5. Implement any missing functionality discovered
6. Commit
```

### Test Naming Convention

```
MethodName_Scenario_ExpectedResult
```

```csharp
[Test]
public void GetEffectiveness_FireVsWater_ReturnsHalf()
{
    var result = TypeEffectiveness.GetEffectiveness(
        PokemonType.Fire, PokemonType.Water);
    Assert.That(result, Is.EqualTo(0.5f));
}
```

---

## Quick Reference

### Common Commands

```bash
# Build
dotnet build
dotnet build -c Release

# Test
dotnet test
dotnet test -v q 2>&1 | Select-Object -Last 2  # Quick summary

# New branch
git checkout -b feature/name

# Commit
git commit -m "type(scope): description"

# Merge
git checkout main && git merge feature/name

# Tag
git tag -a v0.1.0 -m "Description"
```

### File Locations

| What | Where |
|------|-------|
| Coding rules | `docs/project_guidelines.md` |
| Anti-patterns | `docs/anti-patterns.md` |
| Unity setup | `docs/unity_integration.md` |
| Current state | `.ai/context.md` |
| Roadmap | `docs/implementation_plan.md` |

---

## Questions?

Check the documentation in `docs/` or review existing code patterns.

