# Targeting System Specification

## 1. Overview
The **Targeting System** determines **who** a move can hit.
It bridges the gap between a Move's definition (`TargetScope`) and the dynamic Battle Field.
It must handle:
-   **Scope**: Single, All, Self, Random, etc.
-   **Redirection**: "Follow Me", "Lightning Rod".
-   **Availability**: Ignoring fainted Pokemon (unless move targets fainted, e.g., Revive).

## 2. Core Definitions
*Namespace: `PokemonGame.Core.Combat.Targeting`*

### `TargetScope` (The Rule)
Defined in `MoveData`.

```csharp
public enum TargetScope {
    Self,
    SingleEnemy,
    AllEnemies,
    SingleAlly,
    AllAllies,
    Any,            // Any active pokemon (except self)
    RandomEnemy,    // For moves like "Outrage" or "Thrash"
    Field           // Affects the whole board (Weather, Trick Room)
}
```

## 3. The Resolver Service
*Namespace: `PokemonGame.Core.Combat.Targeting`*

This service answers the question: *"I want to use Move X. Who can I click on?"*

```csharp
public static class TargetResolver {
    
    public static List<BattleSlot> GetValidTargets(BattleSlot user, MoveData move, BattleField field) {
        var potentialTargets = new List<BattleSlot>();

        // 1. Initial Filtering based on Scope
        switch (move.TargetScope) {
            case TargetScope.Self:
                return new List<BattleSlot> { user };
                
            case TargetScope.SingleEnemy:
                potentialTargets.AddRange(field.GetOpposingSlots(user.Side));
                break;
                
            case TargetScope.SingleAlly:
                potentialTargets.AddRange(field.GetAllySlots(user.Side));
                // Usually exclude self, unless specified
                potentialTargets.Remove(user);
                break;
                
            case TargetScope.Any:
                potentialTargets.AddRange(field.GetAllActiveSlots());
                potentialTargets.Remove(user);
                break;
                
            // ... handle others
        }

        // 2. Filter Fainted (Standard Rule)
        // Some moves like "Revive" would skip this check
        potentialTargets = potentialTargets.Where(s => s.IsOccupied).ToList();

        // 3. Handle Redirection (The Complex Part)
        // "Follow Me" / "Rage Powder"
        var redirector = GetActiveRedirector(user, field);
        if (redirector != null && potentialTargets.Contains(redirector)) {
            // If a redirector exists and is a valid target for this move,
            // YOU MUST TARGET HIM.
            return new List<BattleSlot> { redirector };
        }

        return potentialTargets;
    }

    private static BattleSlot GetActiveRedirector(BattleSlot user, BattleField field) {
        // Check opposing side for "Follow Me" status
        var enemies = field.GetOpposingSlots(user.Side);
        return enemies.FirstOrDefault(s => s.Pokemon.VolatileStatus.HasFlag(VolatileStatus.FollowMe));
    }
}
```

## 4. Integration with Input (Symmetry)

Both Player and AI use `TargetResolver` to know their options.

### Player Flow (`PlayerInputProvider`)
```csharp
public async Task<BattleAction> GetAction(...) {
    // 1. Select Move
    var move = await _view.SelectMove(mySlot.Pokemon.Moves);

    // 2. Get Valid Targets
    var validTargets = TargetResolver.GetValidTargets(mySlot, move, field);

    // 3. Select Target
    BattleSlot target = null;
    if (validTargets.Count == 1) {
        target = validTargets[0]; // Auto-select if only 1 option
    } else {
        // UI shows selection cursor only on valid slots
        target = await _view.SelectTarget(validTargets); 
    }

    return new UseMoveAction(mySlot, move, target);
}
```

### AI Flow (`AIActionProvider`)
```csharp
public async Task<BattleAction> GetAction(...) {
    // ... inside Strategy ...
    
    foreach (var move in moves) {
        var validTargets = TargetResolver.GetValidTargets(mySlot, move, field);
        
        foreach (var target in validTargets) {
            // Evaluate (Move + Target) combination
            float score = Evaluate(move, target);
        }
    }
}
```

## 5. Handling "Multi-Target" Execution
Some moves target *multiple* slots (e.g., "Surf" hits all).
The `UseMoveAction` needs to handle this.

**Design Decision**: `UseMoveAction` should hold a **List** of targets, or the `TargetResolver` returns a list, but the Action logic re-resolves for Area of Effect?

**Better Approach**: The `UseMoveAction` holds the *Primary Target* (for animation focus). The Logic expands it if the scope is AOE.

```csharp
public class UseMoveAction : BattleAction {
    // ...
    public override IEnumerable<BattleAction> ExecuteLogic(BattleField field) {
        List<BattleSlot> finalTargets = new List<BattleSlot>();

        if (_move.TargetScope == TargetScope.AllEnemies) {
            finalTargets = TargetResolver.GetValidTargets(_user, _move, field);
        } else {
            finalTargets.Add(_target); // The manually selected one
        }

        // Generate DamageAction for EACH target
        foreach (var t in finalTargets) {
            actions.Add(new DamageAction(t, ...));
        }
        
        return actions;
    }
}
```

## 6. Testability
We can test Redirection logic without running the game.

```csharp
[Test]
public void Test_FollowMe_Forces_Targeting() {
    // Setup 2v2
    var field = BattleTestHelper.Create2v2();
    var player = field.PlayerSide.ActiveSlots[0];
    var enemy1 = field.EnemySide.ActiveSlots[0];
    var enemy2 = field.EnemySide.ActiveSlots[1];

    // Enemy 2 uses Follow Me
    enemy2.Pokemon.VolatileStatus |= VolatileStatus.FollowMe;

    // Player tries to use Single Target move
    var move = new MoveData { TargetScope = TargetScope.SingleEnemy };
    var targets = TargetResolver.GetValidTargets(player, move, field);

    // Assert
    Assert.AreEqual(1, targets.Count);
    Assert.AreEqual(enemy2, targets[0]); // Must be Enemy 2
}
```
