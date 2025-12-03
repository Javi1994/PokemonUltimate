# Feature Documentation Standard

> **Standard structure for all feature documentation.**

## Overview

Every feature in `docs/features/[N]-[feature-name]/` MUST follow this consistent structure to ensure:

**📋 Feature Reference**: See [`features_master_list.md`](features_master_list.md) for feature numbering and naming standards.

**⚠️ IMPORTANT**: Always use the **numbered feature format** (`[N]-[feature-name]`) when referencing features. Never use unnumbered paths.

-   **Consistency** - Same structure across all features
-   **Discoverability** - Easy to find specific information
-   **Maintainability** - Clear organization for future updates
-   **Onboarding** - New developers can quickly understand any feature

**⚠️ MANDATORY: Feature Naming in Code** - All public classes MUST include feature references in XML documentation comments. See [`docs/ai/guidelines/feature_naming_in_code.md`](../ai/guidelines/feature_naming_in_code.md) for complete guidelines.

---

## Required Documents

Each feature MUST have these 6 documents:

| Document             | Purpose                               | Required Sections                               |
| -------------------- | ------------------------------------- | ----------------------------------------------- |
| **README.md**        | Feature overview and quick navigation | Overview, Status, Quick Links, Related Features |
| **architecture.md**  | Technical specification               | Design, Components, APIs, Data Structures       |
| **use_cases.md**     | All scenarios and behaviors           | User stories, Edge cases, Examples              |
| **roadmap.md**       | Implementation plan                   | Phases, Status, Dependencies, Checklists        |
| **testing.md**       | Testing strategy                      | Test structure, Coverage, Patterns              |
| **code_location.md** | Code organization                     | Namespaces, Files, Key Classes                  |

---

## Document Templates

### 1. README.md Template

```markdown
# Feature [Number]: [Feature Name]

> [One-line description]

**Feature Number**: [Number]  
**See**: [`features_master_list.md`](features_master_list.md) for feature numbering standards.

## Overview

[What this feature does, why it exists]

## Current Status

-   ✅ **Implemented**: [What's done]
-   ⏳ **Planned**: [What's coming]

## Documentation

| Document                              | Purpose                     |
| ------------------------------------- | --------------------------- |
| **[Architecture](architecture.md)**   | Technical specification     |
| **[Use Cases](use_cases.md)**         | All scenarios and behaviors |
| **[Roadmap](roadmap.md)**             | Implementation plan         |
| **[Testing](testing.md)**             | Testing strategy            |
| **[Code Location](code_location.md)** | Where the code lives        |

## Related Features

-   **[Feature 1: Game Data](../1-game-data/)** - [How they relate]
-   **[Feature 2: Combat System](../2-combat-system/)** - [How they relate]

**⚠️ Always use numbered feature paths**: `../[N]-[feature-name]/` instead of `../feature-name/`

## Quick Links

-   **Current State**: [Link to status]
-   **Key Classes**: [Link to code_location.md]
-   **Tests**: [Link to testing.md]

---

**Last Updated**: YYYY-MM-DD
```

### 2. architecture.md Template

````markdown
# [Feature Name] Architecture

> Technical specification and design.

## Overview

[High-level design, philosophy, key decisions]

## Design Principles

-   [Principle 1]
-   [Principle 2]

## Components

### Component 1

[Description, responsibilities, interfaces]

### Component 2

[Description, responsibilities, interfaces]

## Data Structures

### [Structure Name]

```csharp
public class StructureName {
    // Fields and methods
}
```
````

## APIs

### [API Name]

```csharp
public interface IApiName {
    // Methods
}
```

## Integration Points

-   **[Feature 1: Game Data](../1-game-data/)** - [How they integrate]
-   **[Feature 2: Combat System](../2-combat-system/)** - [How they integrate]

**⚠️ Always use numbered feature paths**: `../[N]-[feature-name]/` instead of `../feature-name/`

## Related Documents

-   **[Use Cases](use_cases.md)** - Scenarios this architecture supports
-   **[Code Location](code_location.md)** - Where this is implemented
-   **[Testing](testing.md)** - How to test this architecture

````

### 3. use_cases.md Template

```markdown
# Feature [Number]: [Feature Name] Use Cases

> All scenarios, behaviors, and edge cases.

**Feature Number**: [Number]
**See**: [`features_master_list.md`](features_master_list.md) for feature numbering standards.

## Overview

[What scenarios this feature covers]

## Core Use Cases

### UC-001: [Use Case Name]
**Description**: [What happens]
**Actor**: [Who/what triggers it]
**Preconditions**: [What must be true]
**Steps**:
1. [Step 1]
2. [Step 2]
**Expected Result**: [What should happen]
**Status**: ✅ Implemented / ⏳ Planned

### UC-002: [Use Case Name]
[Same structure]

## Edge Cases

### EC-001: [Edge Case Name]
**Description**: [What edge case]
**Behavior**: [How it's handled]
**Status**: ✅ Implemented / ⏳ Planned

## Integration Scenarios

### INT-001: [Integration Scenario]
**Description**: [How this feature works with others]
**Status**: ✅ Implemented / ⏳ Planned

## Related Documents

- **[Architecture](architecture.md)** - Technical design
- **[Roadmap](roadmap.md)** - Implementation status
````

### 4. roadmap.md Template

```markdown
# Feature [Number]: [Feature Name] Roadmap

> Implementation plan with phases and status.

**Feature Number**: [Number]  
**See**: [`features_master_list.md`](features_master_list.md) for feature numbering standards.

## Overview

[What this roadmap covers, current phase]

## Current Status

-   ✅ **Phase X**: [Completed phase]
-   🎯 **Phase Y**: [Current phase]
-   ⏳ **Phase Z**: [Planned phase]

## Phases

### Phase X: [Phase Name]

**Goal**: [What this phase achieves]
**Status**: ✅ Complete / 🎯 In Progress / ⏳ Planned

**Components**:

-   [Component 1]
-   [Component 2]

**Dependencies**:

-   [Dependency 1]
-   [Dependency 2]

**Tests**:

-   [Test requirement 1]
-   [Test requirement 2]

**Checklist**:

-   [ ] [Task 1]
-   [ ] [Task 2]

---

### Phase Y: [Phase Name]

[Same structure]

## Related Documents

-   **[Architecture](architecture.md)** - Technical design
-   **[Use Cases](use_cases.md)** - Scenarios to implement
-   **[Testing](testing.md)** - Testing requirements
```

### 5. testing.md Template

````markdown
# Feature [Number]: [Feature Name] Testing Strategy

> How to test this feature.

**Feature Number**: [Number]  
**See**: [`features_master_list.md`](features_master_list.md) for feature numbering standards.

## Overview

[Testing philosophy, coverage goals]

## Test Structure

Following `docs/ai/testing_structure_definition.md`:

-   **Systems/** - [How systems work]
-   **Blueprints/** - [Data structure tests]
-   **Data/** - [Content-specific tests]

## Test Types

### Functional Tests

**Location**: `Tests/Systems/[Feature]/[Component]Tests.cs`
**Purpose**: [Normal behavior]
**Naming**: `MethodName_Scenario_ExpectedResult`

### Edge Case Tests

**Location**: `Tests/Systems/[Feature]/[Component]EdgeCasesTests.cs`
**Purpose**: [Boundary conditions, invalid inputs]
**Naming**: `MethodName_EdgeCase_ExpectedResult`

### Integration Tests

**Location**: `Tests/Systems/[Feature]/Integration/[Category]/`
**Purpose**: [System interactions]
**Naming**: `[System1]_[System2]_[ExpectedBehavior]`

## Coverage Requirements

-   [ ] All public APIs tested
-   [ ] All edge cases covered
-   [ ] Integration scenarios tested
-   [ ] Performance benchmarks (if applicable)

## Test Examples

```csharp
[Test]
public void MethodName_Scenario_ExpectedResult()
{
    // Arrange
    // Act
    // Assert
}
```
````

## Related Documents

-   **[Architecture](architecture.md)** - What to test
-   **[Use Cases](use_cases.md)** - Scenarios to verify
-   **[Code Location](code_location.md)** - Where tests live

````

### 6. code_location.md Template

```markdown
# Feature [Number]: [Feature Name] Code Location

> Where the code lives and how it's organized.

**Feature Number**: [Number]
**See**: [`features_master_list.md`](features_master_list.md) for feature numbering standards.

## Overview

[High-level organization]

## Namespaces

### [Namespace Path]
**Purpose**: [What this namespace contains]
**Key Classes**:
- `ClassName` - [Purpose]
- `ClassName` - [Purpose]

## Project Structure

````

[Project Name]/
├── [Folder]/ # [Purpose]
│ ├── File1.cs # [Purpose]
│ └── File2.cs # [Purpose]
└── [Folder]/ # [Purpose]
└── File3.cs # [Purpose]

````

## Key Classes

### [Class Name]
**Namespace**: `[Full.Namespace.Path]`
**File**: `[Project]/[Path]/[FileName].cs`
**Purpose**: [What it does]
**Key Methods**:
- `MethodName()` - [Purpose]

### [Class Name]
[Same structure]

## Factories & Builders

### [Factory/Builder Name]
**Namespace**: `[Full.Namespace.Path]`
**File**: `[Project]/[Path]/[FileName].cs`
**Usage**:
```csharp
// Example usage
````

## Catalogs & Registries

### [Catalog/Registry Name]

**Namespace**: `[Full.Namespace.Path]`
**File**: `[Project]/[Path]/[FileName].cs`
**Purpose**: [What it provides]

## Related Documents

-   **[Architecture](architecture.md)** - Design these classes implement
-   **[Testing](testing.md)** - Where tests are located

````

---

## Feature Status Checklist

When creating or updating a feature, verify:

- [ ] **README.md** exists and has Overview, Status, Documentation table, Related Features, Quick Links
- [ ] **architecture.md** exists and has Design, Components, Data Structures, APIs
- [ ] **use_cases.md** exists and has Core Use Cases, Edge Cases, Integration Scenarios
- [ ] **roadmap.md** exists and has Current Status, Phases with Goals/Status/Dependencies/Tests/Checklists
- [ ] **testing.md** exists and has Test Structure, Test Types, Coverage Requirements, Examples
- [ ] **code_location.md** exists and has Namespaces, Project Structure, Key Classes, Factories/Builders, Catalogs/Registries
- [ ] **Code includes feature references** - All public classes have `<remarks>` sections with feature numbers (see [`docs/ai/guidelines/feature_naming_in_code.md`](../ai/guidelines/feature_naming_in_code.md))
- [ ] All documents link to each other appropriately
- [ ] All documents follow the templates above
- [ ] Status indicators (✅ ⏳ 🎯) are used consistently

---

## Migration Guide

For existing features, migrate documents to match this standard:

1. **Review existing documents** - Identify what exists
2. **Create missing documents** - Use templates above
3. **Rename/consolidate** - Merge similar documents (e.g., `testing_strategy.md` → `testing.md`)
4. **Update README.md** - Add documentation table with all 6 documents
5. **Cross-reference** - Add links between documents
6. **Verify checklist** - Ensure all requirements met

---

## Related Documents

- **[Features Overview](../features/README.md)** - All features
- **[Test Structure](../ai/testing_structure_definition.md)** - Testing standards
- **[Project Guidelines](../ai/guidelines/project_guidelines.md)** - Coding rules

---

## 📍 Locating Feature Documentation

### For Features (Parent)

All feature documentation is located in: `docs/features/[N]-[feature-name]/`

**Example**: Feature 2: Combat System → `docs/features/2-combat-system/`

**Required Documents**:
- `README.md` - Feature overview
- `architecture.md` - Technical specification
- `use_cases.md` - All scenarios
- `roadmap.md` - Implementation plan
- `testing.md` - Testing strategy
- `code_location.md` - Code organization

**Quick Access**:
1. Check `docs/features_master_list.md` for feature number and folder name
2. Navigate to `docs/features/[N]-[feature-name]/`
3. Start with `README.md` for overview
4. Use the Documentation table in `README.md` to find specific documents

### For Sub-Features

All sub-feature documentation is located in: `docs/features/[N]-[feature-name]/[N.M]-[sub-feature-name]/`

**Example**: Sub-feature 2.5: Combat Actions → `docs/features/2-combat-system/2.5-combat-actions/`

**Required Documents** (some optional for smaller sub-features):
- `README.md` - Sub-feature overview (required)
- `architecture.md` - Technical specification (if applicable)
- `use_cases.md` - Scenarios (if applicable)
- `roadmap.md` - Implementation plan (if applicable)
- `testing.md` - Testing strategy (if applicable)

**Quick Access**:
1. Check `docs/features_master_list.md` for sub-feature number
2. Navigate to `docs/features/[N]-[feature-name]/[N.M]-[sub-feature-name]/`
3. Start with `README.md` for overview
4. Check parent feature's `README.md` for sub-feature links

### Finding Documentation by Feature Number

**Format**: `[N]-[kebab-case-name]` for features, `[N.M]-[kebab-case-name]` for sub-features

**Examples**:
- Feature 1 → `docs/features/1-game-data/`
- Feature 2.5 → `docs/features/2-combat-system/2.5-combat-actions/`
- Feature 3.1 → `docs/features/3-content-expansion/3.1-pokemon-expansion/`

**Always use numbered paths** - Never use unnumbered paths like `pokemon-data/` or `combat-system/`

---

## ➕ Creating New Features

### Step 1: Determine Feature Number

1. Check `docs/features_master_list.md` for the next available number
2. Features are numbered sequentially: 1, 2, 3, 4, 5...
3. **Never skip numbers** - Use the next sequential number

### Step 2: Choose Feature Name

1. Use **kebab-case** for folder names: `feature-name`
2. Use **Title Case** for display: `Feature Name`
3. Keep names concise and descriptive
4. Check `docs/features_master_list.md` to avoid duplicates

### Step 3: Create Feature Folder

**Format**: `docs/features/[N]-[feature-name]/`

**Example**: Creating Feature 6 → `docs/features/6-new-feature/`

```bash
mkdir -p docs/features/6-new-feature
````

### Step 4: Create Required Documents

Use the templates above to create all 6 required documents:

1. `README.md` - Use template from Section "1. README.md Template"
2. `architecture.md` - Use template from Section "2. architecture.md Template"
3. `use_cases.md` - Use template from Section "3. use_cases.md Template"
4. `roadmap.md` - Use template from Section "4. roadmap.md Template"
5. `testing.md` - Use template from Section "5. testing.md Template"
6. `code_location.md` - Use template from Section "6. code_location.md Template"

### Step 5: Update Master List

Add the new feature to `docs/features_master_list.md`:

```markdown
### Feature [N]: [Feature Name]

**Folder**: `docs/features/[N]-[feature-name]/`  
**Status**: ⏳ Planned  
**Description**: [One-line description]

**Sub-Features**:

-   **[N.1]**: [Sub-feature name] - [Description]
```

### Step 6: Update Features Overview

Add the feature to `docs/features/README.md` in the appropriate section.

### Step 7: Use Feature Numbering Consistently

**Always reference features using their number and name**:

-   ✅ Correct: "Feature 6: New Feature" or "6: New Feature"
-   ❌ Wrong: "New Feature" (without number)
-   ✅ Correct: `docs/features/6-new-feature/`
-   ❌ Wrong: `docs/features/new-feature/`

---

## ➕ Creating New Sub-Features

### Step 1: Determine Sub-Feature Number

1. Check parent feature's section in `docs/features_master_list.md`
2. Sub-features use decimal notation: `[N].[M]` where N is feature number, M is sub-feature number
3. **Never skip numbers** - Use the next sequential number within the feature

**Example**: Feature 2 has sub-features 2.1-2.19, so next would be 2.20

### Step 2: Choose Sub-Feature Name

1. Use **kebab-case** for folder names: `sub-feature-name`
2. Use **Title Case** for display: `Sub-Feature Name`
3. Keep names concise and descriptive

### Step 3: Create Sub-Feature Folder

**Format**: `docs/features/[N]-[feature-name]/[N.M]-[sub-feature-name]/`

**Example**: Creating Sub-feature 2.20 → `docs/features/2-combat-system/2.20-new-mechanic/`

```bash
mkdir -p docs/features/2-combat-system/2.20-new-mechanic
```

### Step 4: Create Required Documents

**Minimum Required**: `README.md`

**Optional** (create if applicable):

-   `architecture.md` - If sub-feature has technical specifications
-   `use_cases.md` - If sub-feature has specific scenarios
-   `roadmap.md` - If sub-feature has implementation phases
-   `testing.md` - If sub-feature has specific testing needs

**Use templates** from the parent feature's documents as reference.

### Step 5: Update Master List

Add the new sub-feature to `docs/features_master_list.md` under the parent feature:

```markdown
**Sub-Features**:

-   **[N.M]**: [Sub-feature name] - [Description]
```

### Step 6: Update Parent Feature README

Add the sub-feature to the parent feature's `README.md` in the "Sub-Features" section.

### Step 7: Use Sub-Feature Numbering Consistently

**Always reference sub-features using their number and name**:

-   ✅ Correct: "Sub-feature 2.20: New Mechanic" or "2.20: New Mechanic"
-   ❌ Wrong: "New Mechanic" (without number)
-   ✅ Correct: `docs/features/2-combat-system/2.20-new-mechanic/`
-   ❌ Wrong: `docs/features/2-combat-system/new-mechanic/`

---

## 📝 Naming Convention Rules

### ⚠️ CRITICAL: Always Use Numbered Format

**Features**:

-   ✅ **Correct**: `docs/features/1-game-data/`
-   ❌ **Wrong**: `docs/features/game-data/` (missing number prefix)

**Sub-Features**:

-   ✅ **Correct**: `docs/features/2-combat-system/2.5-combat-actions/`
-   ❌ **Wrong**: `docs/features/2-combat-system/combat-actions/`

**In Documentation**:

-   ✅ **Correct**: "Feature 2: Combat System" or "2: Combat System"
-   ❌ **Wrong**: "Combat System" (without number)

**In Code Comments**:

-   ✅ **Correct**: `// See Feature 2: Combat System architecture`
-   ❌ **Wrong**: `// See Combat System architecture`

### Folder Naming

-   **Format**: `[N]-[kebab-case-name]` for features
-   **Format**: `[N.M]-[kebab-case-name]` for sub-features
-   **Use kebab-case**: lowercase with hyphens
-   **Examples**: `1-game-data`, `2.5-combat-actions`, `3.1-pokemon-expansion`

### Display Naming

-   **Format**: `Feature [N]: [Feature Name]` or `[N]: [Feature Name]`
-   **Format**: `Sub-Feature [N.M]: [Sub-Feature Name]` or `[N.M]: [Sub-Feature Name]`
-   **Use Title Case**: Capitalize first letter of each word
-   **Examples**: "Feature 1: Game Data", "2.5: Combat Actions"

---

**Last Updated**: 2025-01-XX
