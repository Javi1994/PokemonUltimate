# Abilities & Items System Specification

## 1. Overview
Abilities (e.g., *Intimidate*, *Blaze*) and Items (e.g., *Leftovers*, *Choice Band*) add complexity because they can trigger at **any** moment or modify **any** calculation.

To handle this without spaghetti code, we use two patterns:
1.  **Event Triggers** (for Actions): "At the end of the turn, do X."
2.  **Pipeline Hooks** (for Math): "When calculating Attack, add 50%."

## 2. The Event Trigger System
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
