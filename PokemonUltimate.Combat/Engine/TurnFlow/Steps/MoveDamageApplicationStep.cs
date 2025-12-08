using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Engine.TurnFlow.Definition;
using PokemonUltimate.Combat.Handlers.Registry;

namespace PokemonUltimate.Combat.Engine.TurnFlow.Steps
{
    /// <summary>
    /// Step que aplica el daño calculado a los targets.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `DECOUPLED_STEPS_PROPOSAL.md`
    /// </remarks>
    public class MoveDamageApplicationStep : ITurnStep
    {
        private readonly CombatEffectHandlerRegistry _handlerRegistry;

        public string StepName => "Move Damage Application";
        public bool ExecuteEvenIfFainted => false;

        public MoveDamageApplicationStep(CombatEffectHandlerRegistry handlerRegistry)
        {
            _handlerRegistry = handlerRegistry ?? throw new ArgumentNullException(nameof(handlerRegistry));
        }

        public async Task<bool> ExecuteAsync(TurnContext context)
        {
            var moveActions = context.SortedActions.OfType<UseMoveAction>()
                .Where(ma => context.MoveValidations?.GetValueOrDefault(ma, true) == true &&
                             !context.ProtectionChecks?.GetValueOrDefault(ma, false) == true &&
                             context.AccuracyChecks?.GetValueOrDefault(ma, true) == true &&
                             context.DamageCalculations?.ContainsKey(ma) == true);

            foreach (var moveAction in moveActions)
            {
                var damageContext = context.DamageCalculations[moveAction];

                if (damageContext.FinalDamage <= 0)
                    continue;

                context.Logger?.LogDebug(
                    $"Applying {damageContext.FinalDamage} damage from {moveAction.Move.Name} " +
                    $"to {moveAction.Target.Pokemon?.DisplayName}");

                // Crear DamageAction con el daño calculado
                var damageAction = new DamageAction(
                    moveAction.User,
                    moveAction.Target,
                    damageContext,
                    _handlerRegistry);

                context.GeneratedActions.Add(damageAction);
            }

            return await Task.FromResult(true);
        }
    }
}
