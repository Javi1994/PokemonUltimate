# Feature 6: Development Tools - Architecture

> Technical specification of the unified debugger system.

**Feature Number**: 6  
**Feature Name**: Development Tools  
**See**: [`../../features_master_list.md`](../../features_master_list.md) for feature numbering standards.

## Overview

The Development Tools feature provides a unified Windows Forms application (`PokemonUltimate.DeveloperTools`) that integrates multiple debugger tabs for testing and analyzing different aspects of the Pokemon battle system.

## Architecture

### Unified Application Structure

```
PokemonUltimate.DeveloperTools/
├── MainForm.cs                    # Main form with TabControl
├── Program.cs                     # Application entry point
├── Runners/
│   ├── BattleRunner.cs           # Reusable battle execution logic
│   ├── MoveRunner.cs             # Reusable move testing logic
│   ├── StatCalculatorRunner.cs   # Stat calculation logic (6.1)
│   ├── DamageCalculatorRunner.cs # Damage pipeline visualization (6.2)
│   ├── StatusEffectRunner.cs     # Status effect testing logic (6.3)
│   └── TurnOrderRunner.cs        # Turn order calculation logic (6.4)
└── Tabs/
    ├── BattleDebuggerTab.cs      # Battle debugger UserControl ✅
    ├── MoveDebuggerTab.cs        # Move debugger UserControl ✅
    ├── TypeMatchupDebuggerTab.cs # Type matchup UserControl ✅
    ├── StatCalculatorDebuggerTab.cs # Stat calculator UserControl (6.1)
    ├── DamageCalculatorDebuggerTab.cs # Damage calculator UserControl (6.2)
    ├── StatusEffectDebuggerTab.cs # Status effect UserControl (6.3)
    └── TurnOrderDebuggerTab.cs   # Turn order UserControl (6.4)
```

### MainForm

**Purpose**: Main application window with tabbed interface

**Key Components**:
- `TabControl` - Contains all debugger tabs
- Tab pages for each debugger
- UserControl instances for each debugger tab

### Runner Pattern

**Purpose**: Reusable logic for executing tests and collecting statistics

**Pattern**:
- Each debugger has a corresponding Runner class
- Runners handle the actual testing/calculation logic
- Tabs handle UI and display results
- Separation of concerns: logic vs presentation

**Existing Runners**:
- `BattleRunner` - Executes battles and collects statistics
- `MoveRunner` - Tests moves and collects statistics

**New Runners** (6.1-6.4):
- `StatCalculatorRunner` - Calculates stats with different configurations
- `DamageCalculatorRunner` - Visualizes damage pipeline step-by-step
- `StatusEffectRunner` - Tests status effects and interactions
- `TurnOrderRunner` - Calculates turn order with different configurations

### Tab Pattern

**Purpose**: UserControl-based tabs for each debugger

**Common Structure**:
1. **Configuration Panel** (left side)
   - Dropdowns for selecting Pokemon/moves/etc.
   - Numeric inputs for levels/tests
   - Checkboxes for options
   - Run button

2. **Results Panel** (right side)
   - TabControl with multiple result views
   - Summary tab (formatted text)
   - Data tabs (DataGridView tables)
   - Progress bar and status label

3. **Execution**
   - Async execution to keep UI responsive
   - Progress tracking during execution
   - Results displayed in multiple formats

## Sub-Feature Specifications

### 6.1: Stat Calculator Debugger

**Purpose**: Calculate and visualize Pokemon stats with different configurations

**Features**:
- Select Pokemon species
- Set level (1-100)
- Configure Nature
- Configure IVs (0-31 per stat)
- Configure EVs (0-252 per stat, 510 total)
- Display all calculated stats (HP, Attack, Defense, SpAttack, SpDefense, Speed)
- Show breakdown of calculation (base, IV, EV, Nature multiplier)
- Compare different configurations side-by-side

**Runner**: `StatCalculatorRunner`
**Tab**: `StatCalculatorDebuggerTab`

### 6.2: Damage Calculator Debugger

**Purpose**: Step-by-step damage calculation pipeline visualization

**Features**:
- Select attacker Pokemon
- Select defender Pokemon
- Select move
- Set level
- Display each step of damage pipeline:
  1. Base Damage
  2. Critical Hit (yes/no, multiplier)
  3. Random Factor (0.85-1.0)
  4. STAB multiplier
  5. Ability modifiers
  6. Item modifiers
  7. Weather modifiers
  8. Terrain modifiers
  9. Screen modifiers
  10. Type effectiveness
  11. Burn penalty
- Show final damage and HP percentage
- Show damage range (min/max with random factor)

**Runner**: `DamageCalculatorRunner`
**Tab**: `DamageCalculatorDebuggerTab`

### 6.3: Status Effect Debugger

**Purpose**: Test status effects and their interactions

**Features**:
- Select Pokemon
- Apply persistent status (Burn, Paralysis, Poison, Sleep, Freeze)
- Apply volatile status (Confusion, Flinch, etc.)
- View stat modifications (Burn reduces Attack, Paralysis reduces Speed)
- Test interactions (cannot apply multiple persistent statuses)
- View damage per turn (Poison/Burn)
- Test status removal conditions

**Runner**: `StatusEffectRunner`
**Tab**: `StatusEffectDebuggerTab`

### 6.4: Turn Order Debugger

**Purpose**: Visualize turn order determination with speed and priority

**Features**:
- Configure multiple Pokemon with different speeds
- Set move priorities
- Apply modifiers (Paralysis, Tailwind, stat stages)
- Display effective speed calculation
- Show final turn order
- Visualize priority vs speed sorting

**Runner**: `TurnOrderRunner`
**Tab**: `TurnOrderDebuggerTab`

## Integration Points

### With Game Data (Feature 1)

- Uses `PokemonSpeciesData` from `PokemonUltimate.Core.Blueprints`
- Uses `PokemonInstance` from `PokemonUltimate.Core.Instances`
- Uses `StatCalculator` from `PokemonUltimate.Core.Factories`
- Uses `TypeEffectiveness` from `PokemonUltimate.Core.Factories`

### With Combat System (Feature 2)

- Uses `DamagePipeline` from `PokemonUltimate.Combat.Damage`
- Uses `TurnOrderResolver` from `PokemonUltimate.Combat.Helpers`
- Uses `BattleField` from `PokemonUltimate.Combat.Field`
- Uses `CombatEngine` from `PokemonUltimate.Combat.Engine`

### With Content

- Uses `PokemonCatalog`, `MoveCatalog` from `PokemonUltimate.Content.Catalogs`

## Common UI Patterns

### Configuration Panel

- Left side panel (fixed width ~350px)
- Border style for visual separation
- Vertical layout with labels and controls
- Consistent spacing and padding

### Results Panel

- Right side panel (flexible width)
- TabControl with multiple result views
- Summary tab: RichTextBox with formatted text
- Data tabs: DataGridView with structured data
- Progress bar and status label at bottom

### Execution Flow

1. User configures options
2. Clicks "Run" button
3. UI disables controls, shows progress bar
4. Async execution starts
5. Progress updates during execution
6. Results displayed in tabs
7. Controls re-enabled

## Error Handling

- Clear error messages for invalid configurations
- Graceful failure handling
- UI remains responsive during errors
- Validation before execution

## Future Enhancements

- Export results to CSV/JSON
- Save/Load configurations
- Compare results side-by-side
- Batch testing capabilities
- Custom scenarios

---

**Last Updated**: January 2025

