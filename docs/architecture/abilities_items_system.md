# Abilities & Items System Specification

## 1. Overview
Abilities (e.g., *Intimidate*, *Blaze*) and Items (e.g., *Leftovers*, *Choice Band*) add complexity because they can trigger at **any** moment or modify **any** calculation.

To handle this without spaghetti code, we use two patterns:
1.  **Event Triggers** (for Actions): "At the end of the turn, do X."
2.  **Pipeline Hooks** (for Math): "When calculating Attack, add 50%."

## 1.1 Current Implementation Status ✅

The **data layer** for abilities and items is fully implemented:

| Component | Status | Location |
|-----------|--------|----------|
| `AbilityData` | ✅ Complete | `Core/Blueprints/AbilityData.cs` |
| `AbilityBuilder` | ✅ Complete | `Core/Builders/AbilityBuilder.cs` |
| `AbilityCatalog` | ✅ 35 abilities | `Content/Catalogs/Abilities/` |
| `ItemData` | ✅ Complete | `Core/Blueprints/ItemData.cs` |
| `ItemBuilder` | ✅ Complete | `Core/Builders/ItemBuilder.cs` |
| `ItemCatalog` | ✅ 23 items | `Content/Catalogs/Items/` |
| `PokemonInstance.Ability` | ✅ Linked | `Core/Instances/PokemonInstance.cs` |
| `PokemonInstance.HeldItem` | ✅ Linked | `Core/Instances/PokemonInstance.cs` |
| `PokemonSpeciesData.Abilities` | ✅ 3 slots | `Core/Blueprints/PokemonSpeciesData.cs` |

**Runtime listeners** (`IBattleListener`) are pending for Phase 2.5+.

## 2. Data Layer (Implemented ✅)

### AbilityData Blueprint
```csharp
public sealed class AbilityData {
    public string Id { get; }
    public string Name { get; }
    public string Description { get; }
    public int Generation { get; }
    
    // When this ability activates
    public AbilityTrigger Triggers { get; }  // Flags: OnSwitchIn, OnTurnEnd, etc.
    public AbilityEffect Effect { get; }     // LowerOpponentStat, ChanceToStatus, etc.
    
    // Effect parameters
    public Stat? TargetStat { get; }
    public int StatStages { get; }
    public float EffectChance { get; }
    public PersistentStatus? StatusEffect { get; }
    public PokemonType? AffectedType { get; }
    public float Multiplier { get; }
    public Weather? WeatherCondition { get; }
}
```

### AbilityBuilder (Fluent API)
```csharp
public static readonly AbilityData Intimidate = Ability.Define("Intimidate")
    .Description("Lowers opposing Pokémon's Attack stat.")
    .Gen(3)
    .LowersOpponentStat(Stat.Attack, -1)
    .Build();

public static readonly AbilityData Blaze = Ability.Define("Blaze")
    .Description("Powers up Fire moves when HP is low.")
    .Gen(3)
    .BoostsTypeWhenLowHP(PokemonType.Fire)
    .Build();
```

### ItemData Blueprint
```csharp
public sealed class ItemData {
    public string Id { get; }
    public string Name { get; }
    public string Description { get; }
    public ItemCategory Category { get; }  // HeldItem, Berry, EvolutionItem, etc.
    public int Price { get; }
    public bool IsHoldable { get; }
    public bool IsConsumable { get; }
    
    // Battle effects
    public ItemTrigger Triggers { get; }
    public Stat? TargetStat { get; }
    public float StatMultiplier { get; }
    public float DamageMultiplier { get; }
    public bool LocksMove { get; }  // Choice items
}
```

### ItemBuilder (Fluent API)
```csharp
public static readonly ItemData Leftovers = Item.Define("Leftovers")
    .Description("Restores HP every turn.")
    .Category(ItemCategory.HeldItem)
    .Price(200)
    .OnTrigger(ItemTrigger.OnTurnEnd)
    .HealsPercentEachTurn(0.0625f)
    .Build();

public static readonly ItemData ChoiceBand = Item.Define("Choice Band")
    .Description("Boosts Attack but locks into one move.")
    .Category(ItemCategory.HeldItem)
    .BoostsStat(Stat.Attack, 1.5f)
    .LocksIntoMove()
    .Build();
```

### Species ↔ Abilities (3 Slots)
```csharp
public class PokemonSpeciesData {
    public AbilityData Ability1 { get; set; }      // Primary (common)
    public AbilityData Ability2 { get; set; }      // Secondary (common, optional)
    public AbilityData HiddenAbility { get; set; } // Hidden (rare)
    
    public bool HasSecondaryAbility => Ability2 != null;
    public bool HasHiddenAbility => HiddenAbility != null;
    public IEnumerable<AbilityData> GetAllAbilities();
    public AbilityData GetRandomAbility(Random random);
}
```

### Instance ↔ Ability/Item (Runtime)
```csharp
public partial class PokemonInstance {
    public AbilityData Ability { get; private set; }
    public ItemData HeldItem { get; set; }
    
    public bool HasAbility => Ability != null;
    public bool HasHeldItem => HeldItem != null;
    public bool IsUsingHiddenAbility => Ability == Species.HiddenAbility;
    
    public bool HasAbilityNamed(string name);
    public void SetAbility(AbilityData ability);  // For Skill Swap
    public void GiveItem(ItemData item);
    public ItemData TakeItem();
}
```

### Creating Pokemon with Abilities/Items
```csharp
// Default ability (random from Ability1/Ability2)
var pokemon = Pokemon.Create(PokemonCatalog.Pikachu, 25).Build();
// pokemon.Ability → Static

// With hidden ability
var haBlaziken = Pokemon.Create(PokemonCatalog.Blaziken, 50)
    .WithHiddenAbility()  // → Speed Boost
    .Build();

// With held item
var competitive = Pokemon.Create(PokemonCatalog.Charizard, 50)
    .WithHiddenAbility()
    .Holding(ItemCatalog.ChoiceScarf)
    .Build();
```

## 3. The Event Trigger System (Pending)
*Namespace: `PokemonUltimate.Combat.Events`*

We define a central **Broadcaster** that notifies listeners of key game phases.

### `BattleTrigger` (Enum)
```csharp
public enum BattleTrigger {
    OnSwitchIn,         // Intimidate
    OnBeforeMove,       // Truant
    OnAfterMove,        // Life Orb recoil
    OnDamageTaken,      // Static, Rough Skin
    OnTurnEnd,          // Leftovers, Speed Boost
    OnWeatherChange     // Swift Swim
}
```

### `IBattleListener` (Interface)
Any object that wants to react to battle events (Abilities, Items, Statuses) implements this.

```csharp
public interface IBattleListener {
    // Returns a list of Actions to enqueue.
    // We pass the context so the item knows "Who am I holding?"
    IEnumerable<BattleAction> OnTrigger(BattleTrigger trigger, BattleSlot holder, BattleField field);
}
```

## 3. Implementation: Items & Abilities

### Example: `Leftovers` (Item)
*Logic: Heals 1/16th HP at end of turn.*

```csharp
public class LeftoversEffect : IBattleListener {
    public IEnumerable<BattleAction> OnTrigger(BattleTrigger trigger, BattleSlot holder, BattleField field) {
        if (trigger == BattleTrigger.OnTurnEnd) {
            if (holder.Pokemon.CurrentHP < holder.Pokemon.Stats[Stat.HP]) {
                int healAmount = holder.Pokemon.Stats[Stat.HP] / 16;
                // Return the Action to be queued
                yield return new MessageAction($"{holder.Pokemon.Name}'s Leftovers!");
                yield return new HealAction(holder, healAmount);
            }
        }
    }
}
```

### Example: `Intimidate` (Ability)
*Logic: Lowers opponent Attack on switch-in.*

```csharp
public class IntimidateAbility : IBattleListener {
    public IEnumerable<BattleAction> OnTrigger(BattleTrigger trigger, BattleSlot holder, BattleField field) {
        if (trigger == BattleTrigger.OnSwitchIn) {
            var enemies = field.GetOpposingSlots(holder.Side);
            foreach (var enemy in enemies) {
                yield return new MessageAction($"{holder.Pokemon.Name}'s Intimidate!");
                yield return new StatChangeAction(enemy, Stat.Attack, -1);
            }
        }
    }
}
```

## 4. Pipeline Hooks (Passive Stats)
For things that don't create Actions but modify numbers (e.g., *Choice Band*), we use the **Damage Pipeline**.

We add a new interface `IStatModifier`.

```csharp
public interface IStatModifier {
    float GetStatMultiplier(BattleSlot holder, Stat stat);
    float GetDamageMultiplier(DamageContext ctx);
}
```

### Example: `ChoiceBand`
```csharp
public class ChoiceBandItem : IStatModifier {
    public float GetStatMultiplier(BattleSlot holder, Stat stat) {
        if (stat == Stat.Attack) return 1.5f;
        return 1.0f;
    }
    // ...
}
```

**Integration in Pipeline:**
The `StatRatioStep` in the Damage Pipeline iterates over the Attacker's Item/Ability and asks for multipliers.

```csharp
// Inside StatRatioStep.Process()
float multiplier = 1.0f;
if (attacker.Item is IStatModifier mod) {
    multiplier *= mod.GetStatMultiplier(attacker, Stat.Attack);
}
atk *= multiplier;
```

## 5. The Event Manager
The `CombatEngine` is responsible for firing these triggers.

```csharp
public class CombatEngine {
    public async Task RunTurn() {
        // ... Phase 1, Phase 2 ...

        // Phase 3: End of Turn
        await ProcessTriggers(BattleTrigger.OnTurnEnd);
    }

    private async Task ProcessTriggers(BattleTrigger trigger) {
        foreach (var slot in _field.GetAllActiveSlots()) {
            // 1. Check Item
            if (slot.Pokemon.Item != null) {
                var actions = slot.Pokemon.Item.OnTrigger(trigger, slot, _field);
                foreach (var action in actions) _queue.Enqueue(action);
            }
            
            // 2. Check Ability
            if (slot.Pokemon.Ability != null) {
                 var actions = slot.Pokemon.Ability.OnTrigger(trigger, slot, _field);
                 foreach (var action in actions) _queue.Enqueue(action);
            }
        }
        
        // Run the queue immediately to show the effects
        await _queue.ProcessQueue(_field, _view);
    }
}
```

## 6. Integration with Guidelines
1.  **Everything is an Action**: `Leftovers` doesn't heal directly. It returns a `HealAction`. The Queue handles the visual/logic.
2.  **Modularity**: New items/abilities are just new classes implementing `IBattleListener`. No changes to `CombatEngine`.
3.  **Testability**: We can test `LeftoversEffect.OnTrigger` in isolation.
    *   *Test*: Call `OnTrigger(OnTurnEnd)`, assert it returns a `HealAction`.
