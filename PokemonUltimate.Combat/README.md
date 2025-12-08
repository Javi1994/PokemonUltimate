# PokemonUltimate.Combat

The core combat system for PokemonUltimate. This module implements a complete, modular battle engine that handles turn-based Pokemon battles with a step-based architecture.

## Architecture Overview

The combat system is built on a **step-based pipeline architecture** that separates concerns and makes the system highly modular and testable:

-   **Battle Flow**: High-level battle lifecycle (setup → execution → cleanup)
-   **Turn Flow**: Detailed turn execution with 23 distinct steps
-   **Damage Pipeline**: Modular damage calculation with 11 steps
-   **Action System**: Command pattern for battle actions
-   **Handler Registry**: Unified system for abilities, items, and move effects

## Key Components

### Engine (`Engine/`)

The main battle execution engine. Contains:

-   **CombatEngine**: Orchestrates the full battle lifecycle
-   **TurnEngine**: Executes individual turns using a step pipeline
-   **BattleFlow**: Battle-level flow (initialization, loop, cleanup)
-   **TurnFlow**: Turn-level flow (action collection, validation, execution, effects)

See `Engine/README.md` for detailed documentation.

### Actions (`Actions/`)

Battle actions represent commands that can be executed during battle. All actions follow a two-phase pattern:

-   **Logic Phase**: Instant game state updates
-   **Visual Phase**: Async presentation to the player

**13 Action Types**: UseMoveAction, DamageAction, HealAction, ApplyStatusAction, FaintAction, StatChangeAction, SetWeatherAction, SetTerrainAction, SetSideConditionAction, SwitchAction, MessageAction, ContactDamageAction, BattleAction (base)

See `Actions/README.md` for detailed documentation.

### Damage (`Damage/`)

Modular damage calculation pipeline. Each step modifies the damage context independently, making the system extensible and testable.

**11 Damage Steps**: BaseDamageStep, CriticalHitStep, RandomFactorStep, StabStep, AttackerAbilityStep, AttackerItemStep, WeatherStep, TerrainStep, ScreenStep, TypeEffectivenessStep, BurnStep

See `Damage/README.md` for detailed documentation.

### Handlers (`Handlers/`)

Unified registry system for processing abilities, items, and move effects. Handlers generate actions in response to triggers.

**34 Handler Classes**:

-   4 Ability Handlers (ContactAbilityHandler, IntimidateHandler, MoxieHandler, SpeedBoostHandler)
-   3 Item Handlers (LeftoversHandler, LifeOrbHandler, RockyHelmetHandler)
-   12 Move Effect Handlers (StatusEffectHandler, StatChangeEffectHandler, RecoilEffectHandler, etc.)
-   15 Checker Handlers (DamageApplicationHandler, StatusApplicationHandler, etc.)

See `Handlers/README.md` for detailed documentation.

### Field (`Field/`)

Battlefield representation including both sides, weather, terrain, and battle rules.

**4 Core Classes**: BattleField, BattleSide, BattleSlot, BattleRules

See `Field/README.md` for detailed documentation.

### Target (`Target/`)

Target resolution system that handles move targeting and redirection effects (e.g., Follow Me, Lightning Rod).

**3 Components**: TargetResolver, TargetRedirectionResolver, Specific resolvers (FollowMeResolver, LightningRodResolver)

See `Target/README.md` for detailed documentation.

### Utilities (`Utilities/`)

Helper classes for turn order resolution, accuracy checking, and battle slot extensions.

**3 Utilities**: TurnOrderResolver, AccuracyChecker, BattleSlotExtensions

See `Utilities/README.md` for detailed documentation.

### AI (`AI/`)

Action providers that make decisions for Pokemon during battle.

**6 AI Implementations**: RandomAI, AlwaysAttackAI, FixedMoveAI, NoActionAI, SmartAI, TeamBattleAI

See `AI/README.md` for detailed documentation.

### Infrastructure (`Infrastructure/`)

Supporting systems including:

-   **Factories**: BattleFieldFactory, BattleQueueFactory, CombatEngineFactory, DamageContextFactory
-   **Logging**: BattleLogger, NullBattleLogger (see `Infrastructure/Logging/README.md`)
-   **Messages**: BattleMessageFormatter (see `Infrastructure/Messages/README.md`)
-   **Events**: BattleEventManager, BattleEvents (see `Infrastructure/Events/README.md`)
-   **Statistics**: BattleStatistics, BattleStatisticsCollector (see `Infrastructure/Statistics/README.md`)
-   **Simulation**: BattleSimulator, MoveSimulator (see `Infrastructure/Simulation/README.md`)
-   **Value Objects**: StatStages, WeatherState, TerrainState, etc. (see `Infrastructure/ValueObjects/README.md`)
-   **Builders**: BattleBuilder
-   **Constants**: BattleOutcome, BattleResult, BattleConstants, etc.
-   **Providers**: RandomProvider, PlayerInputProvider

## Battle Execution Flow

1. **Initialization**: `CombatEngine.Initialize()` sets up parties, providers, and creates the battle flow context
2. **Battle Flow**: Executes 8 setup steps (create field, queue, dependencies)
3. **Battle Loop**: Executes turns until battle ends
4. **Turn Execution**: Each turn goes through 34 steps (23 unique steps + ProcessGeneratedActionsStep repetitions)
5. **Cleanup**: Battle end flow handles statistics and cleanup

## Key Design Patterns

-   **Pipeline Pattern**: Used in damage calculation and turn execution
-   **Command Pattern**: Battle actions encapsulate commands
-   **Registry Pattern**: Handler registry maps triggers to handlers
-   **Strategy Pattern**: Different handlers and AI for different effects
-   **Factory Pattern**: Factories create battle components
-   **Value Object Pattern**: Immutable state objects
-   **Null Object Pattern**: Null logger and null view implementations

## Complete System Statistics

| System             | Components         | Files                                                                       |
| ------------------ | ------------------ | --------------------------------------------------------------------------- |
| **Engine**         | 2 engines, 2 flows | CombatEngine, TurnEngine, BattleFlow (8 steps), TurnFlow (23 steps)         |
| **Actions**        | 13 action types    | BattleAction + 12 specific actions                                          |
| **Damage**         | 11 steps           | BaseDamageStep + 10 modifier steps                                          |
| **Handlers**       | 34 handlers        | 4 abilities + 3 items + 12 effects + 15 checkers                            |
| **Field**          | 4 classes          | BattleField, BattleSide, BattleSlot, BattleRules                            |
| **Target**         | 3 resolvers        | TargetResolver + 2 redirection resolvers                                    |
| **Utilities**      | 3 utilities        | TurnOrderResolver, AccuracyChecker, BattleSlotExtensions                    |
| **AI**             | 6 implementations  | RandomAI, AlwaysAttackAI, FixedMoveAI, NoActionAI, SmartAI, TeamBattleAI    |
| **Infrastructure** | Multiple systems   | Factories, Logging, Messages, Events, Statistics, Simulation, Value Objects |

## Documentation

For detailed documentation on specific subsystems, see:

-   `Engine/README.md` - Engine architecture and step lists
-   `Engine/TurnFlow/Steps/README.md` - Turn step documentation (34 steps)
-   `Engine/BattleFlow/Steps/README.md` - Battle flow step documentation (8 steps)
-   `Actions/README.md` - Action system documentation (13 actions)
-   `Damage/README.md` - Damage pipeline documentation (11 steps)
-   `Handlers/README.md` - Handler system documentation (34 handlers)
-   `Field/README.md` - Field structure documentation
-   `Target/README.md` - Target resolution documentation
-   `Utilities/README.md` - Utility classes documentation
-   `AI/README.md` - AI implementations documentation (6 AIs)
-   `Infrastructure/Logging/README.md` - Logging system documentation
-   `Infrastructure/Messages/README.md` - Message formatting documentation
-   `Infrastructure/Events/README.md` - Event system documentation
-   `Infrastructure/Statistics/README.md` - Statistics collection documentation
-   `Infrastructure/Simulation/README.md` - Battle simulation documentation
-   `Infrastructure/ValueObjects/README.md` - Value objects documentation (8 objects)

## Testing

The modular architecture makes testing straightforward:

-   Each step can be tested independently
-   Handlers can be tested in isolation
-   Actions can be tested without full battle context
-   Damage steps can be tested with mock contexts
-   AI can be tested with mock fields
-   Value objects can be tested for immutability

## Extension Points

To extend the system:

1. **New Actions**: Create action classes inheriting from `BattleAction`
2. **New Handlers**: Implement handler interfaces and register in `CombatEffectHandlerRegistry`
3. **New Damage Steps**: Implement `IDamageStep` and add to `DamagePipeline`
4. **New Turn Steps**: Implement `ITurnStep` and add to `TurnEngine` step list
5. **New Battle Flow Steps**: Implement `IBattleFlowStep` and add to `CombatEngine` initialization
6. **New AI**: Create class inheriting from `ActionProviderBase`
7. **New Value Objects**: Create immutable value object classes
8. **New Loggers**: Implement `IBattleLogger` interface
9. **New Message Formatters**: Implement `IBattleMessageFormatter` interface

## Usage Example

```csharp
// Create engine with dependencies
var engine = new CombatEngine(
    battleFieldFactory,
    battleQueueFactory,
    randomProvider,
    accuracyChecker,
    damagePipeline,
    handlerRegistry
);

// Initialize battle
engine.Initialize(
    rules,
    playerParty,
    enemyParty,
    playerProvider,
    enemyProvider,
    view
);

// Run battle
var result = await engine.RunBattle();
```

## Related Documentation

-   See individual subsystem READMEs for detailed documentation
-   `docs/features/2-combat-system/` - Feature documentation
-   Architecture documents in each subsystem
