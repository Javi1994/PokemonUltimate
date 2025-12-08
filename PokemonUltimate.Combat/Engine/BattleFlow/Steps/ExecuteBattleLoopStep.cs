using System;
using System.Threading.Tasks;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Engine.BattleFlow;
using PokemonUltimate.Combat.Engine.BattleFlow.Definition;
using PokemonUltimate.Combat.Engine.Service;
using PokemonUltimate.Combat.Field;
using PokemonUltimate.Combat.Infrastructure.Constants;
using PokemonUltimate.Combat.Infrastructure.Events;
using PokemonUltimate.Combat.Utilities;

namespace PokemonUltimate.Combat.Engine.BattleFlow.Steps
{
    /// <summary>
    /// Step que ejecuta el loop principal de batalla hasta que termine.
    /// Maneja la ejecución de turnos, verificación de resultado y detección de loops infinitos.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `BATTLE_FLOW_STEPS_PROPOSAL.md`
    /// </remarks>
    public class ExecuteBattleLoopStep : IBattleFlowStep
    {
        public string StepName => "Execute Battle Loop";

        // Track if real damage was dealt during the current turn
        private bool _damageDealtThisTurn = false;

        public async Task<bool> ExecuteAsync(BattleFlowContext context)
        {
            if (context.Field == null)
                throw new InvalidOperationException("Field must be created before executing battle loop.");
            if (context.TurnEngine == null)
                throw new InvalidOperationException("TurnExecutor must be created before executing battle loop.");

            context.Logger?.LogDebug("Starting battle loop execution");

            int turnCount = 0;
            int turnsWithoutDamage = 0;

            // Subscribe to ActionExecuted to track real damage
            EventHandler<ActionExecutedEventArgs> actionHandler = (sender, e) =>
            {
                // Only track DamageAction with actual damage (> 0)
                if (e.Action is DamageAction damageAction && damageAction.Context.FinalDamage > 0)
                {
                    _damageDealtThisTurn = true;
                }
            };

            BattleEventManager.ActionExecuted += actionHandler;

            try
            {
                while (context.Outcome == BattleOutcome.Ongoing && turnCount < BattleConstants.MaxTurns)
                {
                    int currentTurn = turnCount + 1;

                    // Reset damage tracking for this turn
                    _damageDealtThisTurn = false;

                    // Raise turn start event
                    BattleEventManager.RaiseTurnStart(currentTurn, context.Field);

                    // Execute turn (use ConfigureAwait(false) to avoid capturing context)
                    await context.TurnEngine.ExecuteTurn(context.Field, currentTurn).ConfigureAwait(false);
                    turnCount++;

                    // Raise turn end event
                    BattleEventManager.RaiseTurnEnd(currentTurn, context.Field);

                    // Check outcome after each turn
                    context.Outcome = BattleArbiterService.CheckOutcome(context.Field);

                    // Check for infinite loop: detect if no real damage was dealt
                    if (_damageDealtThisTurn)
                    {
                        turnsWithoutDamage = 0;
                    }
                    else
                    {
                        turnsWithoutDamage++;

                        // If no damage for many turns, consider it an infinite loop
                        if (turnsWithoutDamage >= BattleConstants.MaxTurnsWithoutHPChange)
                        {
                            context.Logger?.LogWarning($"Battle detected as infinite loop: {turnsWithoutDamage} turns without damage. Ending in draw.");
                            context.Outcome = BattleOutcome.Draw;
                            break;
                        }
                    }
                }
            }
            finally
            {
                // Always unsubscribe to prevent memory leaks
                BattleEventManager.ActionExecuted -= actionHandler;
            }

            // If we reached max turns without a conclusion, end in draw
            if (context.Outcome == BattleOutcome.Ongoing && turnCount >= BattleConstants.MaxTurns)
            {
                context.Logger?.LogWarning($"Battle reached maximum turn limit ({BattleConstants.MaxTurns}). Ending in draw.");
                context.Outcome = BattleOutcome.Draw;
            }

            // Generate result
            context.Result = new BattleResult
            {
                Outcome = context.Outcome,
                TurnsTaken = turnCount
            };

            return await Task.FromResult(true);
        }
    }
}
