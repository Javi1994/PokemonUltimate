# Unity Integration Guide

> How to use PokemonUltimate.Core, PokemonUltimate.Combat, and PokemonUltimate.Content in Unity.

## Prerequisites

- **Unity 6** (or Unity 2021.3+)
- **Project Template**: 2D (URP) recommended
- **.NET Compatibility**: All DLLs target `netstandard2.1` (Unity compatible)

---

## Quick Start

### Step 1: Build the DLLs

From the solution root, run:

```powershell
dotnet build -c Release
```

This produces:
```
PokemonUltimate.Core/bin/Release/netstandard2.1/PokemonUltimate.Core.dll
PokemonUltimate.Combat/bin/Release/netstandard2.1/PokemonUltimate.Combat.dll
PokemonUltimate.Content/bin/Release/netstandard2.1/PokemonUltimate.Content.dll
```

### Step 2: Copy to Unity

1. In your Unity project, create folder: `Assets/Plugins/`
2. Copy all three DLLs into `Assets/Plugins/`

```
Unity Project/
└── Assets/
    └── Plugins/
        ├── PokemonUltimate.Core.dll
        ├── PokemonUltimate.Combat.dll
        └── PokemonUltimate.Content.dll
```

### Step 3: Verify Import

1. Open Unity
2. Check `Assets/Plugins/` in Project window
3. Both DLLs should appear without error icons
4. If they show errors, right-click → Reimport

---

## Usage Examples

### Creating Pokemon

```csharp
using UnityEngine;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Content.Catalogs.Pokemon;

public class PokemonSpawner : MonoBehaviour
{
    void Start()
    {
        // Simple creation
        var pikachu = PokemonFactory.Create(PokemonCatalog.Pikachu, 25);
        
        Debug.Log($"{pikachu.DisplayName} Lv.{pikachu.Level}");
        Debug.Log($"HP: {pikachu.CurrentHP}/{pikachu.MaxHP}");
    }
}
```

### Using the Builder (Advanced)

```csharp
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Content.Catalogs.Pokemon;
using PokemonUltimate.Content.Catalogs.Moves;

var pokemon = new PokemonInstanceBuilder(PokemonCatalog.Charizard)
    .AtLevel(50)
    .WithNature(Nature.Adamant)
    .Male()
    .WithMoves(
        MoveCatalog.Flamethrower,
        MoveCatalog.Earthquake,
        MoveCatalog.ThunderPunch,
        MoveCatalog.DragonClaw)
    .Shiny()
    .Build();
```

### Type Effectiveness

```csharp
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Enums;

// Single type
float multiplier = TypeEffectiveness.GetEffectiveness(
    PokemonType.Water, PokemonType.Fire);  // 2.0x

// Dual type
float dualMultiplier = TypeEffectiveness.GetEffectiveness(
    PokemonType.Ice, PokemonType.Dragon, PokemonType.Flying);  // 4.0x
```

### Level Up & Evolution

```csharp
var charmander = PokemonFactory.Create(PokemonCatalog.Charmander, 15);

// Add experience
charmander.AddExperience(1000);
Debug.Log($"New level: {charmander.Level}");

// Check evolution
if (charmander.CanEvolve())
{
    charmander.Evolve(PokemonCatalog.Charmeleon);
    Debug.Log($"Evolved into {charmander.DisplayName}!");
}
```

### Accessing Move Data

```csharp
using PokemonUltimate.Content.Catalogs.Moves;
using System.Linq;

// Get Pokemon's moves
foreach (var moveInstance in pokemon.Moves)
{
    var move = moveInstance.Move;
    Debug.Log($"{move.Name}: {move.Type} - Power {move.Power}");
}
```

---

## Required Namespaces

| Namespace | Contains |
|-----------|----------|
| `PokemonUltimate.Core.Factories` | `PokemonFactory`, `PokemonInstanceBuilder`, `TypeEffectiveness`, `StatCalculator` |
| `PokemonUltimate.Core.Enums` | `PokemonType`, `Nature`, `Gender`, `Stat`, `MoveCategory` |
| `PokemonUltimate.Core.Instances` | `PokemonInstance`, `MoveInstance` |
| `PokemonUltimate.Core.Blueprints` | `PokemonSpeciesData`, `MoveData`, `BaseStats` |
| `PokemonUltimate.Content.Catalogs.Pokemon` | `PokemonCatalog` (all Pokemon definitions) |
| `PokemonUltimate.Content.Catalogs.Moves` | `MoveCatalog` (all Move definitions) |

---

## Updating After Changes

When you modify Core or Content code:

1. **Rebuild**:
   ```powershell
   dotnet build -c Release
   ```

2. **Re-copy DLLs** to `Assets/Plugins/`

3. **Unity will auto-reimport** (or right-click → Reimport)

> **Tip**: Create a batch script to automate this:
> ```batch
> @echo off
> dotnet build -c Release
> copy "PokemonUltimate.Core\bin\Release\netstandard2.1\PokemonUltimate.Core.dll" "C:\path\to\unity\Assets\Plugins\"
> copy "PokemonUltimate.Content\bin\Release\netstandard2.1\PokemonUltimate.Content.dll" "C:\path\to\unity\Assets\Plugins\"
> echo Done!
> ```

---

## Troubleshooting

| Problem | Solution |
|---------|----------|
| "Assembly not found" | Ensure DLLs are in `Assets/Plugins/` |
| "Type not found" | Add the correct `using` statement |
| DLLs have error icon | Right-click → Reimport |
| Changes not reflected | Rebuild DLLs and re-copy |
| Namespace conflicts | Use full namespace: `PokemonUltimate.Core.Enums.PokemonType` |

---

## Architecture Notes

### What runs in Unity

```
Unity (MonoBehaviour)
    │
    ├── UI/Presentation (Sprites, Animations, Sound)
    ├── Input Handling (Touch, Keyboard)
    └── Calls into Core/Content DLLs
            │
            ├── PokemonUltimate.Core (Logic)
            │   ├── Factories (creation)
            │   ├── Instances (runtime state)
            │   └── Calculations (stats, damage)
            │
            └── PokemonUltimate.Content (Data)
                ├── Pokemon definitions
                └── Move definitions
```

### Separation of Concerns

| Layer | Responsibility | Unity Dependency |
|-------|----------------|------------------|
| **Core** | Game logic, calculations | ❌ None |
| **Content** | Pokemon/Move data | ❌ None |
| **Unity** | Visuals, input, audio | ✅ Yes |

This separation allows:
- Unit testing without Unity
- Reusing Core/Content in other contexts (console, web)
- Clean architecture with testable logic

---

## Future: Combat System Integration

When the combat system is implemented, Unity will:

1. **Implement `IBattleView`** - Show animations, messages
2. **Implement `IActionProvider`** - Handle player input
3. **Call `CombatEngine`** - Run battle logic

```csharp
// Future pattern (not yet implemented)
public class UnityBattleView : MonoBehaviour, IBattleView
{
    public async Task ShowMessage(string message)
    {
        // Display in UI, wait for player to dismiss
    }
    
    public async Task PlayAnimation(string animationId)
    {
        // Play Unity animation
    }
}
```

---

## See Also

- [Project Structure](architecture/project_structure.md)
- [Project Guidelines](project_guidelines.md)
- [Combat System Spec](architecture/combat_system_spec.md)

