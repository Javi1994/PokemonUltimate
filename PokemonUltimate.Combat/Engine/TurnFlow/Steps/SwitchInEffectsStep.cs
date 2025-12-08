using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Engine.TurnFlow.Definition;
using PokemonUltimate.Combat.Field;
using PokemonUltimate.Combat.Handlers.Effects;
using PokemonUltimate.Combat.Handlers.Registry;
using PokemonUltimate.Core.Data.Enums;
using PokemonUltimate.Core.Domain.Instances.Pokemon;

namespace PokemonUltimate.Combat.Engine.TurnFlow.Steps
{
    /// <summary>
    /// Step que procesa efectos de entrada cuando un Pokemon cambia (entry hazards, abilities, items).
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `RESPONSIBILITY_REVIEW.md`
    /// </remarks>
    public class SwitchInEffectsStep : ITurnStep
    {
        private readonly EntryHazardHandler _entryHazardHandler;
        private readonly CombatEffectHandlerRegistry _handlerRegistry;

        public string StepName => "Switch In Effects";
        public bool ExecuteEvenIfFainted => false;

        public SwitchInEffectsStep(EntryHazardHandler entryHazardHandler, CombatEffectHandlerRegistry handlerRegistry)
        {
            _entryHazardHandler = entryHazardHandler ?? throw new ArgumentNullException(nameof(entryHazardHandler));
            _handlerRegistry = handlerRegistry ?? throw new ArgumentNullException(nameof(handlerRegistry));
        }

        public async Task<bool> ExecuteAsync(TurnContext context)
        {
            // Procesar efectos de entrada para switches que se ejecutaron
            var switchActions = context.ProcessedActions.OfType<SwitchAction>()
                .Where(sa => sa.Slot != null && sa.NewPokemon != null);

            foreach (var switchAction in switchActions)
            {
                context.Logger?.LogDebug(
                    $"Processing switch-in effects for {switchAction.NewPokemon.DisplayName}");

                var actions = ProcessSwitchIn(switchAction.Slot, switchAction.NewPokemon, context.Field);
                context.GeneratedActions.AddRange(actions);
            }

            return await Task.FromResult(true);
        }

        /// <summary>
        /// Processes everything that happens when a Pokemon switches in.
        /// </summary>
        private List<BattleAction> ProcessSwitchIn(BattleSlot slot, PokemonInstance pokemon, BattleField field)
        {
            var actions = new List<BattleAction>();

            // 1. Process entry hazards first
            var hazardActions = _entryHazardHandler.ProcessHazards(slot, pokemon, field);
            actions.AddRange(hazardActions);

            // 2. Process ability (if exists) using handler registry
            if (pokemon.Ability != null)
            {
                var abilityActions = _handlerRegistry.ProcessAbility(
                    pokemon.Ability, slot, field, AbilityTrigger.OnSwitchIn);
                actions.AddRange(abilityActions);
            }

            return actions;
        }

    }
}
