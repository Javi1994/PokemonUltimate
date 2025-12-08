using System;
using System.Linq;
using System.Threading.Tasks;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Engine.TurnFlow.Definition;
using PokemonUltimate.Combat.Handlers.Registry;
using PokemonUltimate.Core.Data.Enums;

namespace PokemonUltimate.Combat.Engine.TurnFlow.Steps
{
    /// <summary>
    /// Step que procesa efectos de contacto (Static, Rough Skin, Rocky Helmet).
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `DECOUPLED_STEPS_PROPOSAL.md`
    /// </remarks>
    public class ContactReceivedEffectsStep : ITurnStep
    {
        private readonly CombatEffectHandlerRegistry _handlerRegistry;

        public string StepName => "Contact Received Effects";
        public bool ExecuteEvenIfFainted => false;

        public ContactReceivedEffectsStep(CombatEffectHandlerRegistry handlerRegistry)
        {
            _handlerRegistry = handlerRegistry ?? throw new ArgumentNullException(nameof(handlerRegistry));
        }

        public async Task<bool> ExecuteAsync(TurnContext context)
        {
            // Procesar efectos de contacto
            var damageActions = context.ProcessedActions.OfType<DamageAction>()
                .Where(da => da.Context?.FinalDamage > 0 &&
                             da.Context.Move != null &&
                             da.Context.Move.MakesContact &&
                             da.Target != null &&
                             !da.Target.Pokemon.IsFainted);

            foreach (var damageAction in damageActions)
            {
                context.Logger?.LogDebug(
                    $"Processing contact-received effects for {damageAction.Target.Pokemon?.DisplayName} " +
                    $"(attacker: {damageAction.User.Pokemon?.DisplayName})");

                var pokemon = damageAction.Target.Pokemon;
                if (pokemon == null)
                    continue;

                // Process ability using handler registry (pass attacker for contact effects)
                if (pokemon.Ability != null)
                {
                    var abilityActions = _handlerRegistry.ProcessAbility(
                        pokemon.Ability, damageAction.Target, context.Field, AbilityTrigger.OnContactReceived, damageAction.User);
                    context.GeneratedActions.AddRange(abilityActions);
                }

                // Process item using handler registry (pass attacker for contact effects)
                if (pokemon.HeldItem != null)
                {
                    var itemActions = _handlerRegistry.ProcessItem(
                        pokemon.HeldItem, damageAction.Target, context.Field, ItemTrigger.OnContactReceived, damageAction.User);
                    context.GeneratedActions.AddRange(itemActions);
                }
            }

            return await Task.FromResult(true);
        }
    }
}
