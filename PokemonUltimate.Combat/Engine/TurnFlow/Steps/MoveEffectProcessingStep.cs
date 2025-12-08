using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Engine.TurnFlow.Definition;
using PokemonUltimate.Combat.Handlers.Registry;
using PokemonUltimate.Core.Data.Effects;

namespace PokemonUltimate.Combat.Engine.TurnFlow.Steps
{
    /// <summary>
    /// Step que procesa efectos del movimiento (recoil, drain, status) usando handler registry.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `DECOUPLED_STEPS_PROPOSAL.md`
    /// </remarks>
    public class MoveEffectProcessingStep : ITurnStep
    {
        private readonly CombatEffectHandlerRegistry _handlerRegistry;

        public string StepName => "Move Effect Processing";
        public bool ExecuteEvenIfFainted => false;

        public MoveEffectProcessingStep(CombatEffectHandlerRegistry handlerRegistry)
        {
            _handlerRegistry = handlerRegistry ?? throw new ArgumentNullException(nameof(handlerRegistry));
        }

        public async Task<bool> ExecuteAsync(TurnContext context)
        {
            if (context.GeneratedActions == null)
                context.GeneratedActions = new System.Collections.Generic.List<BattleAction>();

            var moveActions = context.SortedActions.OfType<UseMoveAction>()
                .Where(ma => context.MoveValidations?.GetValueOrDefault(ma, true) == true &&
                             !context.ProtectionChecks?.GetValueOrDefault(ma, false) == true &&
                             context.AccuracyChecks?.GetValueOrDefault(ma, true) == true);

            foreach (var moveAction in moveActions)
            {
                // Obtener daño calculado previamente (si existe)
                var damageDealt = context.DamageCalculations?.GetValueOrDefault(moveAction)?.FinalDamage ?? 0;

                context.Logger?.LogDebug(
                    $"Processing effects for {moveAction.Move.Name} " +
                    $"(damage dealt: {damageDealt})");

                // Procesar efectos del movimiento (recoil, drain, status, etc.)
                // Nota: El daño ya fue procesado en MoveDamageApplicationStep,
                // así que aquí solo procesamos efectos no relacionados con daño
                var effectActions = new System.Collections.Generic.List<BattleAction>();

                // Procesar todos los efectos excepto DamageEffect (ya procesado)
                foreach (var effect in moveAction.Move.Effects)
                {
                    if (effect is DamageEffect)
                        continue; // Ya procesado en MoveDamageCalculationStep

                    // Procesar efecto usando el registry
                    var actions = _handlerRegistry.ProcessMoveEffect(
                        effect,
                        moveAction.User,
                        moveAction.Target,
                        moveAction.Move,
                        context.Field,
                        damageDealt);

                    effectActions.AddRange(actions);
                }

                context.GeneratedActions.AddRange(effectActions);

                context.Logger?.LogDebug(
                    $"Processed {effectActions.Count} effect actions for {moveAction.Move.Name}");
            }

            return await Task.FromResult(true);
        }
    }
}
