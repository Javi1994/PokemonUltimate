using System;
using System.Linq;
using System.Threading.Tasks;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Engine.TurnFlow.Definition;
using PokemonUltimate.Combat.Handlers.Registry;
using PokemonUltimate.Combat.Utilities.Extensions;
using PokemonUltimate.Core.Data.Enums;

namespace PokemonUltimate.Combat.Engine.TurnFlow.Steps
{
    /// <summary>
    /// Step que procesa efectos reactivos al recibir daño (abilities, items).
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `DECOUPLED_STEPS_PROPOSAL.md`
    /// </remarks>
    public class DamageTakenEffectsStep : ITurnStep
    {
        private readonly CombatEffectHandlerRegistry _handlerRegistry;

        public string StepName => "Damage Taken Effects";
        public bool ExecuteEvenIfFainted => false;

        public DamageTakenEffectsStep(CombatEffectHandlerRegistry handlerRegistry)
        {
            _handlerRegistry = handlerRegistry ?? throw new ArgumentNullException(nameof(handlerRegistry));
        }

        public async Task<bool> ExecuteAsync(TurnContext context)
        {
            // Procesar efectos reactivos de daño recibido
            var damageActions = context.ProcessedActions.OfType<DamageAction>()
                .Where(da => da.Context?.FinalDamage > 0 &&
                             da.Target != null &&
                             da.Target.IsActive() &&
                             !da.Target.Pokemon.IsFainted);

            foreach (var damageAction in damageActions)
            {
                context.Logger?.LogDebug(
                    $"Processing damage-taken effects for {damageAction.Target.Pokemon?.DisplayName}");

                var pokemon = damageAction.Target.Pokemon;
                if (pokemon == null)
                    continue;

                // Process ability using handler registry
                if (pokemon.Ability != null)
                {
                    var abilityActions = _handlerRegistry.ProcessAbility(
                        pokemon.Ability, damageAction.Target, context.Field, AbilityTrigger.OnDamageTaken);
                    context.GeneratedActions.AddRange(abilityActions);
                }

                // Process item using handler registry
                if (pokemon.HeldItem != null)
                {
                    var itemActions = _handlerRegistry.ProcessItem(
                        pokemon.HeldItem, damageAction.Target, context.Field, ItemTrigger.OnLowHP);
                    context.GeneratedActions.AddRange(itemActions);
                }
            }

            return await Task.FromResult(true);
        }
    }
}
