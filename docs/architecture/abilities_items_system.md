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

## 4. Pipeline Hooks (Passive Stats) ✅ **IMPLEMENTED**

For things that don't create Actions but modify numbers (e.g., *Choice Band*), we use the **Damage Pipeline**.

### `IStatModifier` Interface ✅
```csharp
public interface IStatModifier {
    /// <summary>
    /// Gets the stat multiplier for a specific stat.
    /// Returns 1.0f if no modification.
    /// </summary>
    float GetStatMultiplier(BattleSlot holder, Stat stat, BattleField field);
    
    /// <summary>
    /// Gets the damage multiplier for an attack.
    /// Returns 1.0f if no modification.
    /// </summary>
    float GetDamageMultiplier(DamageContext context);
}
```

### Adapters ✅
We use adapter classes to convert `AbilityData` and `ItemData` to `IStatModifier`:

```csharp
// AbilityStatModifier - Converts AbilityData to IStatModifier
public class AbilityStatModifier : IStatModifier {
    private readonly AbilityData _abilityData;
    
    public float GetStatMultiplier(BattleSlot holder, Stat stat, BattleField field) {
        // Currently no abilities provide passive stat multipliers
        // Future: Huge Power, Pure Power
        return 1.0f;
    }
    
    public float GetDamageMultiplier(DamageContext context) {
        // Check for HP threshold abilities (Blaze, Torrent, Overgrow)
        if (_abilityData.HPThreshold > 0f && _abilityData.AffectedType.HasValue) {
            var pokemon = context.Attacker.Pokemon;
            float hpPercent = (float)pokemon.CurrentHP / pokemon.MaxHP;
            
            if (hpPercent <= _abilityData.HPThreshold && 
                context.Move.Type == _abilityData.AffectedType.Value) {
                return _abilityData.Multiplier; // e.g., 1.5f for Blaze
            }
        }
        return 1.0f;
    }
}

// ItemStatModifier - Converts ItemData to IStatModifier
public class ItemStatModifier : IStatModifier {
    private readonly ItemData _itemData;
    
    public float GetStatMultiplier(BattleSlot holder, Stat stat, BattleField field) {
        if (_itemData.TargetStat == stat && _itemData.StatMultiplier > 0f) {
            return _itemData.StatMultiplier; // e.g., 1.5f for Choice Band
        }
        return 1.0f;
    }
    
    public float GetDamageMultiplier(DamageContext context) {
        if (_itemData.DamageMultiplier > 0f) {
            return _itemData.DamageMultiplier; // e.g., 1.3f for Life Orb
        }
        return 1.0f;
    }
}
```

**Integration in Pipeline:** ✅

1. **BaseDamageStep**: Applies stat modifiers before calculating stat ratio
   ```csharp
   // Inside BaseDamageStep.Process()
   if (attacker.Pokemon.HeldItem != null) {
       var itemModifier = new ItemStatModifier(attacker.Pokemon.HeldItem);
       float multiplier = itemModifier.GetStatMultiplier(attacker, stat, field);
       effectiveStat *= multiplier;
   }
   ```

2. **AttackerAbilityStep**: Applies ability damage multipliers (after STAB)
   ```csharp
   // Step in DamagePipeline
   public class AttackerAbilityStep : IDamageStep {
       public void Process(DamageContext context) {
           if (context.Attacker.Pokemon.Ability != null) {
               var abilityModifier = new AbilityStatModifier(context.Attacker.Pokemon.Ability);
               float multiplier = abilityModifier.GetDamageMultiplier(context);
               context.Multiplier *= multiplier;
           }
       }
   }
   ```

3. **AttackerItemStep**: Applies item damage multipliers (after ability step)
   ```csharp
   // Step in DamagePipeline
   public class AttackerItemStep : IDamageStep {
       public void Process(DamageContext context) {
           if (context.Attacker.Pokemon.HeldItem != null) {
               var itemModifier = new ItemStatModifier(context.Attacker.Pokemon.HeldItem);
               float multiplier = itemModifier.GetDamageMultiplier(context);
               context.Multiplier *= multiplier;
           }
       }
   }
   ```

### Implemented Items & Abilities ✅

**Items:**
- **Choice Band**: +50% Attack stat (applied in BaseDamageStep)
- **Choice Specs**: +50% SpAttack stat (applied in BaseDamageStep)
- **Choice Scarf**: +50% Speed stat (applied in TurnOrderResolver.GetEffectiveSpeed)
- **Life Orb**: +30% damage multiplier (applied in AttackerItemStep)
- **Assault Vest**: +50% SpDefense stat (applied in BaseDamageStep)
- **Eviolite**: +50% Defense/SpDefense if Pokemon can evolve (applied in BaseDamageStep)

**Abilities:**
- **Blaze**: +50% Fire damage when HP ≤ 33% (applied in AttackerAbilityStep)
- **Torrent**: +50% Water damage when HP ≤ 33% (applied in AttackerAbilityStep)
- **Overgrow**: +50% Grass damage when HP ≤ 33% (applied in AttackerAbilityStep)
- **Swarm**: +50% Bug damage when HP ≤ 33% (applied in AttackerAbilityStep)

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
