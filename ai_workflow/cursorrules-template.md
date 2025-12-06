# Cursor Rules Template - AI Workflow Optimized

> **Template de .cursorrules optimizado con TDD + FDD integrados**  
> **Específico para**: C# .NET SDKs integrados con Unity

## 🚀 AI Workflow Optimization

**AI Quick Reference**: Ver `ai_workflow/AI_QUICK_REFERENCE.md` ⭐ **Guía rápida para ejecutar workflows**  
**User Prompts Guide**: Ver `ai_workflow/PROMPTS_GUIDE.md` ⭐ **Cómo interpretar requests del usuario**  
**Game Definition**: Ver `ai_workflow/decision-trees/game-definition.yaml` ⭐ **START HERE**  
**TDD Workflow**: Ver `ai_workflow/decision-trees/tdd-workflow.yaml`  
**Feature Discovery**: Ver `ai_workflow/decision-trees/feature-discovery.yaml`  
**Test Templates**: Ver `ai_workflow/templates/tests/`  
**Validation**: Ejecutar `ai_workflow/scripts/validate-*.sh` o `.ps1`

---

## 📋 Development Workflow

### When User Says "Define game" or "What is this game?"

**⚠️ CRITICAL: Game Definition - MUST DO FIRST FOR NEW PROJECTS**

**MANDATORY WORKFLOW** (follow `ai_workflow/decision-trees/game-definition.yaml`):

### When User Says "Redefine game definition" or "Improve game definition"

**⚠️ CRITICAL: Review and Improve Existing Definition**

**MANDATORY WORKFLOW** (follow `ai_workflow/decision-trees/game-definition.yaml` Step -1.8):

1. **Read existing definition**
   - Read `docs/GAME_DEFINITION.yaml`
   - Read `docs/features_master_list.md`
   - Read `docs/features_master_list_detailed.md`

2. **Analyze completeness**
   - Check for missing entities, interactions, systems
   - Verify feature granularity
   - Check dependency mapping
   - Compare with best practices

3. **Identify improvements**
   - Missing concepts
   - Missing features
   - Granularity improvements
   - Dependency improvements

4. **Present findings**
   - Show what's missing
   - Suggest improvements
   - Ask for user confirmation

5. **Apply improvements**
   - Update game definition
   - Regenerate feature lists
   - Update master lists

6. **Validate updated definition**

---

### When User Says "Define game" or "What is this game?" (New Project)

**MANDATORY WORKFLOW** (follow `ai_workflow/decision-trees/game-definition.yaml`):

#### Step -1: Game Definition ⭐ **MUST DO FIRST FOR NEW PROJECTS**

1. **Check if game defined**
   - Read `docs/GAME_DEFINITION.yaml`
   - If exists → Validate completeness
   - If not exists → Start definition

2. **Define game basics**
   - Extract: name, type, description, platform, SDK purpose
   - Use template: `ai_workflow/templates/game-definition-template.yaml`

3. **Identify core concepts**
   - Entities (what exists in the game)
   - Environment (the world/space)
   - Interactions (how things interact)
   - Systems (what systems are needed)

4. **Generate feature categories**
   - Map concepts to feature categories
   - Generate initial feature list

5. **Granularize features**
   - Break down into sub-features
   - Identify dependencies
   - Assign to development phases

6. **Create master list**
   - Generate `docs/GAME_DEFINITION.yaml`
   - Generate `docs/features_master_list.md`
   - Generate `docs/features_master_list_detailed.md`

7. **Validate definition**
   - Check completeness
   - If complete → Proceed to Step 0

---

### When User Says "Implement X" or "Add X"

**⚠️ CRITICAL: Feature-Driven Development + TDD - ALWAYS start here**

**MANDATORY WORKFLOW:**

#### Step 0: Feature Discovery & Assignment ⭐ **MUST DO FIRST**

**Prerequisite**: `docs/GAME_DEFINITION.yaml` must exist. If not, run Game Definition workflow first.

**Automated Process** (follow `ai_workflow/decision-trees/feature-discovery.yaml`):

1. **Read Game Definition**
   - Read `docs/GAME_DEFINITION.yaml` for context
   - Read `docs/features_master_list_index.md` ⭐ **OPTIMIZED FOR AI** (or `docs/features_master_list.md` as fallback)
   - Extract all existing features

2. **Semantic Search**
   - Run `codebase_search("features related to [user_request]")`
   - Search in `docs/features_master_list_index.md` and feature READMEs

3. **Check Matches**
   - If matches found → Read feature docs → Assign to existing
   - If no matches → Create new feature (see below)

4. **Assign to Existing Feature** (if applicable)
   - Read feature's complete documentation:
     - `README.md` - Feature overview
     - `architecture.md` - Technical specification
     - `roadmap.md` - Implementation plan
     - `use_cases.md` - All scenarios
     - `code_location.md` - Where code lives
     - `testing.md` - Testing strategy
   - Check if work fits existing sub-feature or needs new one
   - If new sub-feature needed: Create sub-feature folder with `README.md`

5. **Create New Feature/Sub-Feature** (if needed)
   - **Determine Feature Number**: Check `docs/features_master_list.md` for next available
   - **Create Feature Folder**: `docs/features/[N]-[feature-name]/`
   - **Create Complete Documentation Structure**:
     - `README.md` - Feature overview
     - `architecture.md` - Technical specification
     - `roadmap.md` - Implementation plan
     - `use_cases.md` - All scenarios
     - `testing.md` - Testing strategy
     - `code_location.md` - Where code lives
   - **For Sub-Features**: Create `[N.M]-[sub-feature-name]/` folder with `README.md` (and `architecture.md` if complex)
   - **Update Master List**: Add entry to `docs/features_master_list.md`
   - **Follow Standard**: Use `docs/feature_documentation_standard.md` as template

6. **Validate Assignment**
   - Run `ai_workflow/scripts/validate-fdd-compliance.sh` (or `.ps1`)
   - Verify feature assignment is correct

7. **Once Feature Assigned**: Proceed to Step 1

---

#### Step 1: Read Context & Specs

- Read `.ai/context-quick.md` (if exists) or `.ai/context.md`
- Read assigned feature's documentation (`README.md`, `architecture.md`, `roadmap.md`)
- **MANDATORY: Read existing code** - See `code_location.md` FIRST, then read ONLY key files mentioned
- **Code Reading Strategy**: 
  - Read `code_location.md` to identify key classes/files
  - Read only files listed in "Key Classes" section
  - Use codebase_search for discovery, not bulk reading
  - Understand existing patterns before writing

---

#### Step 2: Verify Spec Completeness

- List what to implement vs defer
- Complete spec if incomplete

---

#### Step 3: TDD - Write Tests FIRST ⭐ **MANDATORY**

**Automated Process** (follow `ai_workflow/decision-trees/tdd-workflow.yaml`):

1. **Determine Test Type**
   - Use decision tree: `ai_workflow/decision-trees/tdd-workflow.yaml`
   - Options:
     - New functionality → Functional test
     - Boundary conditions → Edge cases test
     - System interactions → Integration test

2. **Use Appropriate Template**
   - Functional → `ai_workflow/templates/tests/functional-template.[ext]`
   - Edge Cases → `ai_workflow/templates/tests/edgecases-template.[ext]`
   - Integration → `ai_workflow/templates/tests/integration-template.[ext]`

3. **Write Test** (Red Phase)
   - Follow naming convention: `MethodName_Scenario_ExpectedResult`
   - Include Arrange, Act, Assert sections
   - Add feature reference in comments
   - Write test that should FAIL (implementation doesn't exist yet)

4. **Verify Test Fails** (Red Phase)
   - Run tests: `[test_command] Tests/[Feature]/[Component]Tests.[ext]`
   - Expected: FAIL
   - If PASSES → ERROR: Test should fail in red phase, review test or implementation
   - If FAILS → CORRECT: Proceed to green phase

---

#### Step 4: Implement Feature (Green Phase)

- **MANDATORY: Read existing code first** - Match style and patterns
- **MANDATORY: Add feature references** - ALL code must reference its feature
- Write minimal implementation to make test pass
- Follow spec and existing patterns
- Make tests pass (green phase)

5. **Verify Test Passes** (Green Phase)
   - Run tests: `[test_command] Tests/[Feature]/[Component]Tests.[ext]`
   - Expected: PASS
   - If FAILS → Fix implementation, retry
   - If PASSES → CORRECT: Proceed to edge cases

---

#### Step 5: Write Edge Case Tests

- Read feature's `use_cases.md` for edge cases
- Use template: `ai_workflow/templates/tests/edgecases-template.[ext]`
- Test boundaries and invalid inputs
- Implement missing functionality if discovered (Test-Driven Discovery)

---

#### Step 6: Write Integration Tests** (if feature interacts with multiple systems)

- Use template: `ai_workflow/templates/tests/integration-template.[ext]`
- Test system interactions
- Place in `Tests/[Feature]/Integration/[Category]/`

---

#### Step 7: Refactor

- Improve code without changing behavior
- Ensure all tests still pass
- Follow coding rules in `.cursorrules` and `docs/ai/guidelines/`
- Avoid `docs/ai/anti-patterns.md`

---

#### Step 8: Validate Implementation

- **Validate Test Structure**: Run `ai_workflow/scripts/validate-test-structure.sh` (or `.ps1`)
- **Validate FDD Compliance**: Run `ai_workflow/scripts/validate-fdd-compliance.sh` (or `.ps1`)
- `dotnet build` - 0 warnings (or equivalent)
- `dotnet test` - All pass (or equivalent)
- Check `docs/ai/checklists/feature_complete.md`

---

#### Step 9: Update Documentation ⭐ **MANDATORY AFTER EVERY FEATURE**

**Feature Documentation** (update ALL relevant files):
- **`roadmap.md`** - Mark completed phases/sub-features as ✅
- **`architecture.md`** - Update if implementation differs from spec
- **`use_cases.md`** - Mark completed use cases
- **`code_location.md`** - Add new files/classes
- **`testing.md`** - Document new tests and test organization

**Master Documents**:
- **`docs/features_master_list.md`** - Update feature status if changed
- **`docs/features_master_list_index.md`** - Update index with new status and next available features ⭐ **OPTIMIZED FOR AI**
- **`.ai/context.md`** - Update current project state

**CRITICAL**: Documentation must always reflect actual implementation state

---

## ✅ Mandatory Rules

### Code Quality
- **NO magic strings** → Use constants or configuration
- **NO magic numbers** → Use named constants
- **NO try-catch** → Unless absolutely necessary (I/O, external APIs)
- **Fail-fast** → Throw exceptions for invalid inputs
- **Guard clauses** → Validate at method start

### Architecture
- **Core/** → Logic only, NO domain data, NO Unity dependencies
- **Content/** → Domain data, catalogs, builders (if applicable)
- **Tests/** → Organized by feature/system (NUnit)
- **Blueprints** → Immutable (no setters)
- **Instances** → Mutable runtime state
- **Unity Integration** → SDK provides logic layer, Unity handles presentation
- **.NET Standard 2.1** → Target framework for Unity compatibility

### Testing
- **TDD** → Tests before implementation
- **Framework** → NUnit (C#)
- **Three-phase** → Functional → Edge Cases → Integration
- **Naming** → `MethodName_Scenario_ExpectedResult`
- **Test-Driven Discovery** → If test reveals missing functionality, implement it
- **Structure** → Follow test structure standard
- **No Unity in Tests** → Tests should be pure C#, no Unity dependencies

### Documentation
- **API Documentation** → All public APIs (XML documentation comments in C#)
- **MANDATORY: Feature references** → ALL classes must reference their feature
- **MANDATORY: Read before write** → Never write code without reading existing code first
- **MANDATORY: Match existing patterns** → Follow style, naming, structure of existing code
- **Unity Integration Notes** → Document how SDK integrates with Unity (interfaces, events, etc.)

---

## 🔍 Quick Reference

### Exception Messages
```csharp
throw new ArgumentException(ErrorMessages.ValueCannotBeNegative, nameof(value));
```

### Test Pattern
```csharp
[Test]
public void MethodName_Scenario_ExpectedResult() { /* Arrange, Act, Assert */ }
```

### Validation Pattern
```csharp
public void Method(int value) {
    if (value < 0)
        throw new ArgumentException(ErrorMessages.ValueCannotBeNegative, nameof(value));
    // Main logic
}
```

---

## 📁 Key Files Reference

| Need | Read |
|------|------|
| Project state | `.ai/context.md` |
| Feature numbers/names | `docs/features_master_list_index.md` ⭐ **MASTER REFERENCE** |
| Feature documentation standard | `docs/feature_documentation_standard.md` |
| Feature naming in code | `docs/ai/guidelines/feature_naming_in_code.md` ⭐ **MANDATORY** |
| Coding rules | See `.cursorrules` and `docs/ai/guidelines/` |
| What NOT to do | `docs/ai/anti-patterns.md` |
| Code examples | `docs/ai/examples/good_code.md` |
| Test examples | `docs/ai/examples/good_tests.md` |
| Pre-implementation | `docs/ai/checklists/pre_implementation.md` |
| Feature checklist | `docs/ai/checklists/feature_complete.md` |
| Test structure | Test structure standard |

---

## 🔄 After Completing Work

Always:
1. Run validation scripts (`ai_workflow/scripts/validate-*.sh` or `.ps1`)
2. Run `dotnet build` - Verify no warnings (or equivalent)
3. Run `dotnet test` - Verify all pass (or equivalent)
4. Update `.ai/context.md` if major changes
5. Mention test count in response
6. Verify against `docs/ai/checklists/feature_complete.md`

---

## ⚡ Quick Verification Checklist

Before marking feature complete:
- [ ] All tests pass (`dotnet test` or equivalent)
- [ ] No warnings (`dotnet build` or equivalent)
- [ ] `.ai/context.md` updated
- [ ] Use cases validated (if applicable)
- [ ] Integration tests written (if applicable)
- [ ] Documentation updated
- [ ] **Feature references added** - All code references its feature
- [ ] **Existing code reviewed** - All related code was read before implementation
- [ ] **Test structure validated** - Run `ai_workflow/scripts/validate-test-structure.*`
- [ ] **FDD compliance validated** - Run `ai_workflow/scripts/validate-fdd-compliance.*`

---

**Última Actualización**: 2025-01-XX

