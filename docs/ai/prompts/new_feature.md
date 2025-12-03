# 📋 Prompt Template: New Feature

> Use this template when asking the AI to implement a new feature.

---

## Template

```
## Feature Request: [FEATURE NAME]

### Context
Read the following before implementing:
- `.ai/context.md` - Current project state
- `docs/ai/guidelines/project_guidelines.md` - Coding standards
- `docs/architecture/[relevant_spec].md` - Architecture reference
- `docs/roadmaps/[relevant_roadmap].md` - Roadmap for feature area (if applicable)

### Requirements
[Describe what the feature should do]

### Expected Behavior
[Describe how it should work]

### Constraints
- Follow TDD: Write tests first
- Use existing patterns from the codebase
- Centralize any new error messages in ErrorMessages.cs
- Check relevant roadmap for phase status and dependencies
- Update .ai/context.md when complete
- Update roadmap if phase completed

### Acceptance Criteria
- [ ] Functional tests pass
- [ ] Edge case tests pass
- [ ] Integration tests pass (if applicable)
- [ ] No warnings
- [ ] Documentation updated
- [ ] Use cases validated (if combat-related)
```

---

## Example Usage

```
## Feature Request: Damage Calculator

### Context
Read the following before implementing:
- `.ai/context.md` - Current project state
- `docs/ai/guidelines/project_guidelines.md` - Coding standards
- `docs/architecture/damage_and_effect_system.md` - Damage formula spec

### Requirements
Implement the Gen 5+ damage formula that calculates damage based on:
- Attacker's Attack/SpAtk
- Defender's Defense/SpDef
- Move power
- Type effectiveness
- STAB
- Critical hits
- Random factor

### Expected Behavior
- Physical moves use Attack vs Defense
- Special moves use SpAtk vs SpDef
- Returns integer damage value
- Minimum damage is 1 (unless immune)

### Constraints
- Follow TDD: Write tests first
- Use existing TypeEffectiveness class
- Add DamageCalculator to Factories/

### Acceptance Criteria
- [ ] Functional tests for basic damage calculation
- [ ] Edge cases: crits, immunities, STAB combinations
- [ ] Verified against real Pokemon damage examples
```

---

## Quick Version

For simple features, use this shorter format:

```
Implement [FEATURE] following project guidelines.
Reference: docs/architecture/[relevant_doc].md
Requirements: [brief description]
Use TDD - tests first, then implementation.
```
