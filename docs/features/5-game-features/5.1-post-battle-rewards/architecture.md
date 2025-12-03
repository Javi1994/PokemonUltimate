# Sub-Feature 5.1: Post-Battle Rewards - Victory & Defeat System Architecture

> Technical specification for victory/defeat detection and post-battle processing.

**Sub-Feature Number**: 5.1  
**Parent Feature**: Feature 5: Game Features  
**See**: [`../../../features_master_list.md`](../../../features_master_list.md) for feature numbering standards.

## 1. Overview
The **Victory System** determines when a battle ends and what happens afterwards.
It follows the **Single Responsibility Principle**: The `CombatEngine` runs the turns, but the `BattleArbiter` decides if the battle is over.

**Key Responsibilities:**
1.  Check Win/Loss conditions every turn.
2.  Generate a `BattleResult` report.
3.  Handle Post-Battle phases (EXP, Loot, Evolution).

## 2. Core Components

### The Battle Result
~~*Namespace: `PokemonGame.Core.Battle.Results`*~~

A pure data class describing *how* the battle ended.

```csharp
public enum BattleOutcome {
    Ongoing,
    Victory,    // Player won
    Defeat,     // Player lost (whited out)
    Draw,       // Explosion/Self-Destruct double KO
    Fled,       // Player ran away
    Caught      // Wild Pokemon was caught (ends battle)
}

public class BattleResult {
    public BattleOutcome Outcome { get; set; }
    public BattleSlot MvpPokemon { get; set; } // Who dealt most damage?
    public int TurnsTaken { get; set; }
    public List<PokemonInstance> DefeatedEnemies { get; set; } = new();
    public Dictionary<PokemonInstance, int> ExpEarned { get; set; } = new();
    public List<ItemData> LootDropped { get; set; } = new();
}
```

### The Arbiter (Judge)
*Namespace: `PokemonGame.Core.Battle`*

A stateless service that inspects the field.

```csharp
public static class BattleArbiter {
    public static BattleOutcome CheckOutcome(BattleField field) {
        bool playerAlive = field.PlayerSide.HasAblePokemon();
        bool enemyAlive = field.EnemySide.HasAblePokemon();

        if (!playerAlive && !enemyAlive) return BattleOutcome.Draw;
        if (!playerAlive) return BattleOutcome.Defeat;
        if (!enemyAlive) return BattleOutcome.Victory;

        return BattleOutcome.Ongoing;
    }
}
```

## 3. Integration with Combat Loop

The `CombatEngine` checks the Arbiter after every Action that could change the state (Damage, Faint, Catch).

```csharp
// In CombatEngine.cs
public async Task RunBattle() {
    while (true) {
        await RunTurn();
        
        // Check Outcome
        var outcome = BattleArbiter.CheckOutcome(_field);
        if (outcome != BattleOutcome.Ongoing) {
            await EndBattle(outcome);
            return;
        }
    }
}

private async Task EndBattle(BattleOutcome outcome) {
    // 1. Generate Result
    var result = new BattleResult { Outcome = outcome };
    
    // 2. Visual Feedback
    if (outcome == BattleOutcome.Victory) {
        await _view.ShowMessage("battle_won");
        await _view.PlayMusic("music_victory");
    } else if (outcome == BattleOutcome.Defeat) {
        await _view.ShowMessage("battle_lost");
    }
    
    // 3. Post-Battle Processing (EXP, Loot)
    if (outcome == BattleOutcome.Victory) {
        await new PostBattleService().ProcessRewards(result, _field, _view);
    }
}
```

## 4. Post-Battle Service (Rewards)
*Namespace: `PokemonGame.Core.Battle.Rewards`*

Handles the "Aftermath". This is where we calculate EXP and Loot.

```csharp
public class PostBattleService {
    public async Task ProcessRewards(BattleResult result, BattleField field, IBattleView view) {
        // 1. Calculate EXP
        foreach (var enemy in result.DefeatedEnemies) {
            int exp = ExpCalculator.Calculate(enemy, isWild: true);
            // Distribute to party...
        }
        
        // 2. Show EXP Bar Animations
        await view.ShowExpGain(result.ExpEarned);
        
        // 3. Handle Level Ups
        foreach (var pokemon in field.PlayerSide.Party) {
            while (pokemon.CanLevelUp()) {
                pokemon.LevelUp();
                await view.ShowMessage("battle_levelup", pokemon.Name, pokemon.Level);
                // Check for Moves/Evolution...
            }
        }
        
        // 4. Loot
        if (result.LootDropped.Any()) {
            await view.ShowLootScreen(result.LootDropped);
        }
    }
}
```

## 5. Testability

Because `BattleArbiter` is a pure function of `BattleField`, it's trivial to test.

```csharp
[Test]
public void Test_Arbiter_Detects_Victory() {
    var field = new BattleField();
    // Setup: Player has 1 pokemon, Enemy has 0
    field.PlayerSide.Add(new PokemonInstance { CurrentHP = 10 });
    field.EnemySide.Add(new PokemonInstance { CurrentHP = 0 }); // Fainted
    
    var outcome = BattleArbiter.CheckOutcome(field);
    
    Assert.AreEqual(BattleOutcome.Victory, outcome);
}
```

## 6. Future Expansion (Roguelike Elements)
This system is designed to be extended:
-   **Roguelike Runs**: The `BattleResult` is passed to the `RunManager`. If `Defeat`, the RunManager deletes the save file.
-   **Meta-Currency**: Add `int ShardsEarned` to `BattleResult`.
-   **Score**: Calculate score based on `TurnsTaken` and `MvpPokemon`.

---

## Related Documents

- **[Sub-Feature README](README.md)** - Overview of Post-Battle Rewards
- **[Parent Feature README](../README.md)** - Overview of Game Features
- **[Parent Use Cases](../use_cases.md)** - All scenarios for post-battle rewards
- **[Parent Roadmap](../roadmap.md#phase-51-post-battle-rewards-system)** - Implementation plan
- **[Parent Testing](../testing.md)** - Testing strategy
- **[Parent Code Location](../code_location.md)** - Where post-battle code will be located
- **[Feature 2: Combat System](../../2-combat-system/architecture.md)** - Battle engine providing battle results
- **[Feature 2.7: Integration](../../2-combat-system/2.7-integration/architecture.md)** - IBattleView interface for post-battle UI
- **[Feature 1: Pokemon Data](../../1-pokemon-data/architecture.md)** - Pokemon data structure for EXP and level ups
- **[Feature 5.6: Progression System](../5.6-progression-system/README.md)** - Roguelike progression system

---

**Last Updated**: 2025-01-XX
