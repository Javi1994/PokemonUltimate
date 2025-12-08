using System;
using System.Collections.Generic;
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
    /// Step que procesa triggers de fin de turno (habilidades e items).
    /// </summary>
    public class TurnEndStep : ITurnStep
    {
        private readonly CombatEffectHandlerRegistry _handlerRegistry;

        public string StepName => "Turn End";
        public bool ExecuteEvenIfFainted => false;

        public TurnEndStep(CombatEffectHandlerRegistry handlerRegistry)
        {
            _handlerRegistry = handlerRegistry ?? throw new ArgumentNullException(nameof(handlerRegistry));
        }

        public async Task<bool> ExecuteAsync(TurnContext context)
        {
            var actions = new List<BattleAction>();

            // Get all active slots ordered by speed (fastest first)
            var activeSlots = context.Field.GetAllActiveSlots()
                .Where(slot => slot.IsActive())
                .OrderByDescending(slot => slot.Pokemon.GetEffectiveStat(Stat.Speed))
                .ToList();

            foreach (var slot in activeSlots)
            {
                var pokemon = slot.Pokemon;
                if (pokemon == null)
                    continue;

                // Process ability using handler registry
                if (pokemon.Ability != null)
                {
                    var abilityActions = _handlerRegistry.ProcessAbility(
                        pokemon.Ability, slot, context.Field, AbilityTrigger.OnTurnEnd);
                    actions.AddRange(abilityActions);
                }

                // Process held item using handler registry
                if (pokemon.HeldItem != null)
                {
                    var itemActions = _handlerRegistry.ProcessItem(
                        pokemon.HeldItem, slot, context.Field, ItemTrigger.OnTurnEnd);
                    actions.AddRange(itemActions);
                }
            }

            if (actions.Count > 0)
            {
                context.GeneratedActions.AddRange(actions);
            }

            return await Task.FromResult(true);
        }
    }
}
