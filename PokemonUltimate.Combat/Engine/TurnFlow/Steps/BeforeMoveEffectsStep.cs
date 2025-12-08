using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Engine.TurnFlow.Definition;
using PokemonUltimate.Combat.Handlers.Registry;
using PokemonUltimate.Core.Data.Enums;

namespace PokemonUltimate.Combat.Engine.TurnFlow.Steps
{
    /// <summary>
    /// Step que procesa efectos antes del movimiento (abilities, items).
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `DECOUPLED_STEPS_PROPOSAL.md`
    /// </remarks>
    public class BeforeMoveEffectsStep : ITurnStep
    {
        private readonly CombatEffectHandlerRegistry _handlerRegistry;

        public string StepName => "Before Move Effects";
        public bool ExecuteEvenIfFainted => false;

        public BeforeMoveEffectsStep(CombatEffectHandlerRegistry handlerRegistry)
        {
            _handlerRegistry = handlerRegistry ?? throw new ArgumentNullException(nameof(handlerRegistry));
        }

        public async Task<bool> ExecuteAsync(TurnContext context)
        {
            var moveActions = context.SortedActions.OfType<UseMoveAction>()
                .Where(ma => context.MoveValidations?.GetValueOrDefault(ma, true) == true &&
                             !context.ProtectionChecks?.GetValueOrDefault(ma, false) == true &&
                             context.AccuracyChecks?.GetValueOrDefault(ma, true) == true);

            foreach (var moveAction in moveActions)
            {
                context.Logger?.LogDebug(
                    $"Processing before-move effects for {moveAction.Move.Name} " +
                    $"({moveAction.User.Pokemon?.DisplayName})");

                var pokemon = moveAction.User.Pokemon;
                if (pokemon == null)
                    continue;

                // Process ability using handler registry
                if (pokemon.Ability != null)
                {
                    var abilityActions = _handlerRegistry.ProcessAbility(
                        pokemon.Ability, moveAction.User, context.Field, AbilityTrigger.OnBeforeMove);
                    context.GeneratedActions.AddRange(abilityActions);
                }

                // Process item using handler registry
                if (pokemon.HeldItem != null)
                {
                    var itemActions = _handlerRegistry.ProcessItem(
                        pokemon.HeldItem, moveAction.User, context.Field, ItemTrigger.OnMoveUsed);
                    context.GeneratedActions.AddRange(itemActions);
                }
            }

            return await Task.FromResult(true);
        }
    }
}
