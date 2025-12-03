# Sub-Feature 2.1: Battle Foundation - Architecture

> Technical specification of BattleField, Slots, Sides, and Rules.

**Sub-Feature Number**: 2.1  
**Parent Feature**: Feature 2: Combat System  
**See**: [`../../features_master_list.md`](../../features_master_list.md) for feature numbering standards.

## 1. Overview

The **Battle Field System** manages the spatial and structural state of the combat. It replaces simple "Player vs Enemy" variables with a robust **Slot-based** architecture.

This system is responsible for:
-   Tracking who is on the field (`ActiveSlots`).
-   Tracking who is in reserve (`Bench`).
-   Managing Side Effects (Reflect, Spikes).
-   Resolving Targeting (Who is adjacent to whom?).

## 2. Core Structures
*Namespace: `PokemonGame.Core.Battle`*

### `BattleSlot` (The Unit Container)
Represents a specific position on the field (e.g., "Player Left", "Enemy Center").

```csharp
public class BattleSlot {
    public int Index { get; private set; } // 0, 1, 2...
    public BattleSide Side { get; private set; }
    
    // The Pokemon currently occupying this slot (Can be null)
    public PokemonInstance Pokemon { get; set; }
    
    // Slot-specific modifiers (independent of Pokemon)
    // Example: "Trapped by Fire Spin" (often attached to slot in some games, or pokemon)
    // Example: "Future Sight" target
    public List<SlotEffect> Effects { get; private set; }

    // Helper for AI/Logic
    public bool IsOccupied => Pokemon != null && Pokemon.CurrentHP > 0;
    public IActionProvider ActionProvider { get; set; } // The Brain driving this slot
}
```

### `BattleSide` (The Team)
Represents one side of the conflict (Player Team vs Enemy Team).

```csharp
public class BattleSide {
    public bool IsPlayerSide { get; private set; }
    
    // The active fighting positions
    public List<BattleSlot> ActiveSlots { get; private set; }
    
    // The reserve party
    public List<PokemonInstance> Bench { get; private set; }
    
    // Side-wide effects (Hazards, Screens)
    // These affect ANY pokemon in this side
    public SideStatus SideStatus { get; set; } // Flags: Reflect, LightScreen, Tailwind
    public int SpikesCount { get; set; }

    public BattleSide(int slotCount, bool isPlayer) {
        IsPlayerSide = isPlayer;
        ActiveSlots = new List<BattleSlot>();
        for(int i=0; i<slotCount; i++) {
            ActiveSlots.Add(new BattleSlot(i, this));
        }
    }
}
```

### `BattleField` (The Global Container)
The root object held by `CombatEngine`.

```csharp
public class BattleField {
    public BattleSide PlayerSide { get; private set; }
    public BattleSide EnemySide { get; private set; }
    
    // Global Field Effects (Weather, Terrain)
    public WeatherType Weather { get; set; }
    public TerrainType Terrain { get; set; }

    public void Setup(BattleRules rules) {
        PlayerSide = new BattleSide(rules.PlayerSideSlots, true);
        EnemySide = new BattleSide(rules.EnemySideSlots, false);
    }
}
```

## 3. Targeting Logic (Adjacency)
In 1v1, targeting is trivial. In 3v3 (Triple Battle), position matters.
The `BattleField` provides utility methods for this.

```csharp
public bool IsAdjacent(BattleSlot a, BattleSlot b) {
    if (a.Side == b.Side) {
        return Math.Abs(a.Index - b.Index) == 1;
    } else {
        // Simplified logic: In standard games, usually "Opposite" or "Any"
        // For a grid-based tactical game, this would be complex.
        // For standard RPG, usually any slot can hit any slot unless specified otherwise.
        return true; 
    }
}

public List<BattleSlot> GetValidTargets(BattleSlot user, TargetScope scope) {
    switch (scope) {
        case TargetScope.SingleEnemy:
            return GetOppositeSide(user).ActiveSlots.Where(s => s.IsOccupied).ToList();
        case TargetScope.AllEnemies:
             return GetOppositeSide(user).ActiveSlots.Where(s => s.IsOccupied).ToList();
        // ...
    }
}
```

## 4. Switching System (`SwitchAction`)
Switching is a fundamental mechanic that interacts directly with Slots.

```csharp
public class SwitchAction : BattleAction {
    private BattleSlot _slot;
    private PokemonInstance _newPokemon;

    public override IEnumerable<BattleAction> ExecuteLogic(BattleField field) {
        var actions = new List<BattleAction>();
        
        // 1. Recall Message
        if (_slot.IsOccupied) {
            actions.Add(new MessageAction($"Come back, {_slot.Pokemon.Name}!"));
            // Reset Volatile Status (Confusion, Stat changes)
            _slot.Pokemon.VolatileStatus = VolatileStatus.None;
            _slot.Pokemon.ResetStats();
        }

        // 2. Swap Logic
        _slot.Pokemon = _newPokemon;
        
        // 3. Send Out Message & Anim
        actions.Add(new MessageAction($"Go! {_newPokemon.Name}!"));
        actions.Add(new SendOutAnimationAction(_slot)); // Visual spawn

        // 4. Entry Hazards (Spikes)
        if (_slot.Side.SpikesCount > 0) {
            int dmg = CalculateSpikesDamage(_newPokemon);
            actions.Add(new DamageAction(_slot, dmg));
        }

        return actions;
    }
    
    public override Task ExecuteVisual(IBattleView view) {
        return Task.CompletedTask;
    }
}
```

## 5. Integration with Guidelines
1.  **Testability**: We can create a `BattleField` with 3 slots, put dummy pokemon in them, and test `GetValidTargets` without Unity.
2.  **Modularity**: `BattleSlot` holds the `IActionProvider`. This means Slot 1 can be Human, Slot 2 can be AI (Autoplay), and Slot 3 can be empty.
3.  **Action Queue**: Switching is just an Action. Hazards (Spikes) trigger Actions (`DamageAction`) automatically upon entry.

---

## Related Documents

- **[Sub-Feature README](README.md)** - Overview of Battle Foundation
- **[Parent Architecture](../../architecture.md)** - Overall combat system design
- **[Use Cases](../../use_cases.md#battle-formats)** - Battle format scenarios
- **[Roadmap](../../roadmap.md#phase-21-battle-foundation)** - Implementation details
- **[Testing](../../testing.md)** - Testing strategy
- **[Code Location](../../code_location.md)** - Where this is implemented
- **[Sub-Feature 2.2: Action Queue](../2.2-action-queue-system/)** - Uses BattleField for action processing
- **[Sub-Feature 2.6: Combat Engine](../2.6-combat-engine/)** - Uses BattleField for battle execution

---

**Last Updated**: 2025-01-XX