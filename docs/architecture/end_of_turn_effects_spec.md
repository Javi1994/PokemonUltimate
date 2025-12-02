# End-of-Turn Effects System Specification

## 1. Overview

The **End-of-Turn Effects** system processes all effects that occur at the end of each battle turn, after all actions have been executed. This includes:
- Status condition damage (Burn, Poison, Badly Poisoned)
- Item effects (Leftovers, Black Sludge)
- Volatile status effects (Leech Seed drain)
- Weather damage (future)
- Field effects (future)

## 2. Design Goals

1. **Centralized Processing**: All end-of-turn effects processed in one place
2. **Extensible**: Easy to add new effects without modifying core logic
3. **Testable**: Each effect type can be tested independently
4. **Performance**: Efficient iteration over active Pokemon
5. **Integration**: Works seamlessly with existing `CombatEngine` and `BattleAction` system

## 3. Architecture

### 3.1 Component: `EndOfTurnProcessor`

A static helper class that processes all end-of-turn effects for a battlefield.

```csharp
namespace PokemonUltimate.Combat.Engine
{
    /// <summary>
    /// Processes all end-of-turn effects after action queue is empty.
    /// Generates actions for status damage, item effects, and other end-of-turn mechanics.
    /// </summary>
    public static class EndOfTurnProcessor
    {
        /// <summary>
        /// Processes all end-of-turn effects for the battlefield.
        /// Returns actions to be executed (status damage, healing, etc.).
        /// </summary>
        /// <param name="field">The battlefield. Cannot be null.</param>
        /// <returns>List of actions to execute for end-of-turn effects.</returns>
        /// <exception cref="ArgumentNullException">If field is null.</exception>
        public static List<BattleAction> ProcessEffects(BattleField field);
    }
}
```

### 3.2 Integration Point

Integrated into `CombatEngine.RunTurn()` after queue processing:

```csharp
// In CombatEngine.RunTurn()
// 4. Process the queue
await Queue.ProcessQueue(Field, _view);

// 5. End-of-turn effects
var endOfTurnActions = EndOfTurnProcessor.ProcessEffects(Field);
if (endOfTurnActions.Count > 0)
{
    Queue.EnqueueRange(endOfTurnActions);
    await Queue.ProcessQueue(Field, _view);
}
```

## 4. Effect Specifications

### 4.1 Status Damage

#### Burn
- **Damage**: 1/16 of Max HP per turn
- **Formula**: `damage = MaxHP / 16`
- **Minimum**: 1 HP (if MaxHP < 16)
- **Condition**: Only if Pokemon has `PersistentStatus.Burn`
- **Message**: `"{pokemon.Name} is hurt by its burn!"`

#### Poison
- **Damage**: 1/8 of Max HP per turn
- **Formula**: `damage = MaxHP / 8`
- **Minimum**: 1 HP (if MaxHP < 8)
- **Condition**: Only if Pokemon has `PersistentStatus.Poison`
- **Message**: `"{pokemon.Name} is hurt by poison!"`

#### Badly Poisoned (Toxic)
- **Damage**: Escalating damage per turn
- **Formula**: `damage = (StatusTurnCounter * MaxHP) / 16`
- **Counter**: Starts at 1, increments each turn
- **Minimum**: 1 HP per turn
- **Condition**: Only if Pokemon has `PersistentStatus.BadlyPoisoned`
- **Message**: `"{pokemon.Name} is hurt by poison!"`
- **Counter Increment**: After damage, increment `StatusTurnCounter`

### 4.2 Item Effects (Future - Deferred)

#### Leftovers
- **Healing**: 1/16 of Max HP per turn
- **Condition**: Pokemon holds Leftovers item
- **Status**: ⏳ Deferred to future phase

#### Black Sludge
- **Healing**: 1/16 of Max HP per turn (Poison types)
- **Damage**: 1/8 of Max HP per turn (non-Poison types)
- **Status**: ⏳ Deferred to future phase

### 4.3 Volatile Status Effects (Future - Deferred)

#### Leech Seed
- **Drain**: 1/8 of Max HP per turn
- **Healing**: Drains to opponent
- **Status**: ⏳ Deferred to future phase

## 5. Implementation Details

### 5.1 Processing Order

Effects are processed in this order:
1. Status damage (Burn, Poison, Badly Poisoned)
2. Item effects (Leftovers, Black Sludge) - Future
3. Volatile status effects (Leech Seed) - Future
4. Weather damage (Sandstorm, Hail) - Future

### 5.2 Action Generation

Each effect generates one or more `BattleAction`s:
- `DamageAction` for status damage
- `HealAction` for Leftovers healing
- `MessageAction` for status messages

### 5.3 Edge Cases

1. **Fainted Pokemon**: Skip processing (already fainted)
2. **Empty Slots**: Skip empty slots
3. **Minimum Damage**: Always deal at least 1 HP damage
4. **Overkill Prevention**: `DamageAction` handles this automatically
5. **Multiple Status**: Only one persistent status can be active (by design)

### 5.4 Status Turn Counter

For `BadlyPoisoned`:
- Counter starts at 1 when status is applied
- Increments by 1 each turn after damage
- Stored in `PokemonInstance.StatusTurnCounter`
- Reset when status is removed or Pokemon switches out

## 6. Examples

### Example 1: Burn Damage

```csharp
// Pokemon with 100 Max HP, Burn status
var actions = EndOfTurnProcessor.ProcessEffects(field);
// Generates:
// 1. MessageAction: "Pikachu is hurt by its burn!"
// 2. DamageAction: 6 HP damage (100 / 16 = 6.25 → 6)
```

### Example 2: Poison Damage

```csharp
// Pokemon with 80 Max HP, Poison status
var actions = EndOfTurnProcessor.ProcessEffects(field);
// Generates:
// 1. MessageAction: "Bulbasaur is hurt by poison!"
// 2. DamageAction: 10 HP damage (80 / 8 = 10)
```

### Example 3: Badly Poisoned Escalating

```csharp
// Turn 1: Counter = 1, 100 Max HP
// Damage: (1 * 100) / 16 = 6 HP
// Counter increments to 2

// Turn 2: Counter = 2, 100 Max HP
// Damage: (2 * 100) / 16 = 12 HP
// Counter increments to 3

// Turn 3: Counter = 3, 100 Max HP
// Damage: (3 * 100) / 16 = 18 HP
// Counter increments to 4
```

## 7. Testing Requirements

### 7.1 Functional Tests

- `ProcessEffects_BurnStatus_DealsDamage`
- `ProcessEffects_PoisonStatus_DealsDamage`
- `ProcessEffects_BadlyPoisonedStatus_DealsEscalatingDamage`
- `ProcessEffects_MultiplePokemon_ProcessesAll`
- `ProcessEffects_NoStatus_ReturnsEmptyList`
- `ProcessEffects_BurnDamage_CalculatesCorrectly`
- `ProcessEffects_PoisonDamage_CalculatesCorrectly`
- `ProcessEffects_BadlyPoisoned_IncrementsCounter`

### 7.2 Edge Case Tests

- `ProcessEffects_NullField_ThrowsException`
- `ProcessEffects_FaintedPokemon_SkipsProcessing`
- `ProcessEffects_EmptySlot_SkipsProcessing`
- `ProcessEffects_LowMaxHP_MinimumDamage`
- `ProcessEffects_BadlyPoisonedCounter_StartsAtOne`
- `ProcessEffects_BadlyPoisonedCounter_IncrementsEachTurn`
- `ProcessEffects_NoActivePokemon_ReturnsEmptyList`

## 8. Integration with Existing Systems

### 8.1 CombatEngine

- Called after `Queue.ProcessQueue()` completes
- Generated actions are enqueued and processed immediately
- Does not affect turn count

### 8.2 BattleAction System

- Uses existing `DamageAction` for damage
- Uses existing `MessageAction` for status messages
- Uses existing `HealAction` for healing (future)

### 8.3 PokemonInstance

- Reads `Status` property for status condition
- Reads `StatusTurnCounter` for Toxic counter
- Reads `MaxHP` for damage calculation
- Modifies `StatusTurnCounter` for Toxic increment

## 9. Future Extensions

### Phase 2.8+ (Deferred)
- Leftovers/Black Sludge healing
- Leech Seed drain
- Weather damage (Sandstorm, Hail)
- Binding damage (Wrap, Fire Spin)
- Wish healing
- Perish Song countdown

## 10. Constants

All damage fractions should use named constants:

```csharp
public static class EndOfTurnConstants
{
    public const float BurnDamageFraction = 1f / 16f;      // 0.0625
    public const float PoisonDamageFraction = 1f / 8f;     // 0.125
    public const float BadlyPoisonedBaseFraction = 1f / 16f; // 0.0625
    public const int MinimumDamage = 1;
}
```

## 11. Status

- **Phase**: 2.8 (Post-Integration)
- **Priority**: Medium
- **Dependencies**: CombatEngine, BattleAction, PokemonInstance
- **Status**: ⏳ Ready for Implementation

