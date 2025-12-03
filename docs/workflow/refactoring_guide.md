# 🔄 Refactoring Guide

> How to safely improve existing code without breaking functionality.

## When to Refactor

**Good Reasons:**
- Code violates `docs/anti-patterns.md`
- Code doesn't follow `docs/project_guidelines.md`
- Performance issues identified
- Code is hard to understand/maintain
- Tests are difficult to write

**Bad Reasons:**
- "I don't like the style"
- "It could be better" (without clear benefit)
- Premature optimization

## Refactoring Workflow

### 1. Identify Scope

- [ ] Read current implementation
- [ ] Identify what needs improvement
- [ ] Check `docs/anti-patterns.md` for violations
- [ ] Verify tests exist (if not, write them first using TDD)
- [ ] Document current behavior

### 2. Ensure Test Coverage

**CRITICAL:** Never refactor without tests!

- [ ] All existing tests pass
- [ ] Feature has functional tests
- [ ] Edge cases are covered
- [ ] Integration tests exist (if applicable)

**If tests don't exist:**
1. Write tests first (TDD)
2. Ensure tests pass
3. Then refactor

### 3. Refactor Safely

**Principles:**
- **Small, incremental changes** - One improvement at a time
- **Run tests after each change** - Catch regressions immediately
- **Maintain API compatibility** - Or document breaking changes
- **Follow existing patterns** - Don't introduce new patterns unnecessarily

**Steps:**
1. Make one small change
2. Run tests
3. If tests pass → Commit/continue
4. If tests fail → Fix immediately
5. Repeat

### 4. Common Refactoring Patterns

#### Extract Method
```csharp
// Before
public void ProcessDamage()
{
    // 50 lines of damage calculation
    // Mixed with validation, logging, etc.
}

// After
public void ProcessDamage()
{
    ValidateInputs();
    var damage = CalculateDamage();
    ApplyDamage(damage);
    LogDamage(damage);
}
```

#### Extract Class
```csharp
// Before: Everything in one class
public class BattleEngine
{
    // Damage calculation
    // Turn order
    // Status effects
    // 500+ lines
}

// After: Separated concerns
public class BattleEngine
{
    private DamageCalculator _damage;
    private TurnOrderResolver _turnOrder;
    private StatusProcessor _status;
}
```

#### Replace Magic Numbers
```csharp
// Before
if (level > 100) throw new Exception("Invalid level");

// After
if (level > Constants.MaxLevel)
    throw new ArgumentException(ErrorMessages.LevelExceedsMaximum, nameof(level));
```

#### Replace Magic Strings
```csharp
// Before
throw new Exception("Pokemon cannot be null");

// After
throw new ArgumentNullException(nameof(pokemon), ErrorMessages.PokemonCannotBeNull);
```

### 5. Update Documentation

- [ ] Update architecture docs if pattern changed
- [ ] Update `.ai/context.md` if significant
- [ ] Document rationale for changes
- [ ] Update code comments if behavior changed

## Refactoring Checklist

Before starting:
- [ ] Tests exist and pass
- [ ] Scope is clear
- [ ] Impact is understood

During refactoring:
- [ ] Small, incremental changes
- [ ] Tests pass after each change
- [ ] No breaking changes (or documented)

After refactoring:
- [ ] All tests pass
- [ ] No warnings
- [ ] Documentation updated
- [ ] Code follows guidelines

## Common Refactoring Scenarios

### Scenario: Extract Magic String to Constant

**Before:**
```csharp
if (pokemon.Name == "Pikachu")
    // ...
```

**After:**
```csharp
if (pokemon.Name == PokemonNames.Pikachu)
    // ...
```

**Steps:**
1. Create constant class/file
2. Replace all occurrences
3. Run tests
4. Verify behavior unchanged

### Scenario: Split Large Method

**Before:**
```csharp
public void ProcessTurn()
{
    // 100+ lines
    // Multiple responsibilities
}
```

**After:**
```csharp
public void ProcessTurn()
{
    CollectActions();
    SortActions();
    ProcessActions();
    ProcessEndOfTurn();
}
```

**Steps:**
1. Identify logical sections
2. Extract each section to method
3. Run tests after each extraction
4. Verify behavior unchanged

### Scenario: Replace Conditional with Polymorphism

**Before:**
```csharp
public void ProcessEffect(IMoveEffect effect)
{
    if (effect is DamageEffect)
        // ...
    else if (effect is StatusEffect)
        // ...
}
```

**After:**
```csharp
public void ProcessEffect(IMoveEffect effect)
{
    effect.Apply(this);
}
```

**Steps:**
1. Ensure interface supports polymorphism
2. Move logic to effect classes
3. Update callers
4. Run tests
5. Verify behavior unchanged

## Anti-Patterns to Avoid

### ❌ Big Bang Refactoring
- Don't refactor everything at once
- Small, incremental changes are safer

### ❌ Refactoring Without Tests
- Always have tests before refactoring
- Tests are your safety net

### ❌ Changing Behavior During Refactor
- Refactoring = same behavior, better code
- If behavior changes, it's a feature change

### ❌ Over-Abstraction
- Don't create abstractions "just in case"
- Prefer duplication over wrong abstraction

## When to Stop

**Stop refactoring when:**
- Code follows guidelines
- Tests pass
- Code is maintainable
- No clear benefit from further changes

**Don't refactor:**
- Code that works and is rarely touched
- Code that will be replaced soon
- Code where refactoring risk > benefit

## Roadmaps Reference

Before refactoring, check the relevant roadmap to understand:
- Planned changes that might affect the code
- Dependencies on other systems
- Future phases that might require different approach

| Feature Area | Roadmap Document |
|--------------|------------------|
| **Combat System** | `docs/roadmaps/combat_roadmap.md` |
| **Content Expansion** | `docs/roadmaps/content_expansion_roadmap.md` |
| **Unity Integration** | `docs/roadmaps/unity_integration_roadmap.md` |
| **Game Features** | `docs/roadmaps/game_features_roadmap.md` |
| **Testing Improvements** | `docs/roadmaps/testing_roadmap.md` |

See `docs/roadmaps/README.md` for overview of all roadmaps.

