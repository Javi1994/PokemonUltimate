# 📋 Prompt Template: Code Review

> Use this template when asking the AI to review existing code.

---

## Template

```
## Code Review Request

### Target
[File path or feature name to review]

### Review Focus
- [ ] Architecture alignment with docs/architecture/
- [ ] Compliance with docs/project_guidelines.md
- [ ] Test coverage (functional + edge cases)
- [ ] Error handling (fail-fast, centralized messages)
- [ ] Code quality (no magic strings, proper naming)
- [ ] Performance considerations

### Specific Concerns
[Any specific areas you want examined]

### Expected Output
1. List of issues found (if any)
2. Suggested improvements
3. Implementation of fixes (if requested)
```

---

## Self-Review Prompt

Use this to make the AI review its own code:

```
Review the code you just wrote as a senior developer would.
Check against:
1. docs/project_guidelines.md - All 24 rules
2. docs/anti-patterns.md - Common mistakes
3. docs/checklists/feature_complete.md - Quality gates

What would you improve? Apply the improvements.
```

---

## Architecture Review

```
Review [COMPONENT] for architectural alignment:
1. Does it follow the Blueprint/Instance pattern correctly?
2. Are dependencies properly injected?
3. Is it properly separated from game data (Content)?
4. Does it use the Registry pattern for data access?
5. Are effects composable (IMoveEffect)?

Reference: docs/architecture/project_structure.md
```

---

## Quick Review

```
Quick review [FILE]: Check guidelines compliance, test coverage,
error handling. Fix any issues found.
```
