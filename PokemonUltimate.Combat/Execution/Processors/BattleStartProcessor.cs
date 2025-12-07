using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Execution.Battle;
using PokemonUltimate.Combat.Execution.Processors.Interfaces;
using PokemonUltimate.Combat.Foundation.Field;
using PokemonUltimate.Combat.Infrastructure.Events;
using PokemonUltimate.Core.Data.Constants;

namespace PokemonUltimate.Combat.Execution.Processors
{
    /// <summary>
    /// Processes the start of a battle.
    /// Publishes battle started events and initial Pokemon switch-in events.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `docs/features/2-combat-system/2.6-combat-engine/architecture.md`
    /// </remarks>
    public class BattleStartProcessor : IActionGeneratingPhaseProcessor
    {
        private readonly IBattleEventBus _eventBus;

        /// <summary>
        /// Creates a new BattleStartProcessor.
        /// </summary>
        /// <param name="eventBus">The event bus for publishing events. Cannot be null.</param>
        /// <exception cref="ArgumentNullException">If eventBus is null.</exception>
        public BattleStartProcessor(IBattleEventBus eventBus)
        {
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        }

        /// <summary>
        /// Gets the phase this processor handles.
        /// </summary>
        public BattlePhase Phase => BattlePhase.BattleStart;

        /// <summary>
        /// Processes the start of a battle.
        /// </summary>
        /// <param name="field">The battlefield. Cannot be null.</param>
        /// <returns>Empty list (no actions generated, only events published).</returns>
        /// <exception cref="ArgumentNullException">If field is null.</exception>
        public async Task<List<BattleAction>> ProcessAsync(BattleField field)
        {
            if (field == null)
                throw new ArgumentNullException(nameof(field), ErrorMessages.FieldCannotBeNull);

            // Determine battle mode name
            string battleMode = BattleUtilities.DetermineBattleMode(field.Rules);

            // Collect initial Pokemon information
            var initialPlayerPokemon = field.PlayerSide.Slots
                .Where(s => !s.IsEmpty && s.Pokemon != null)
                .Select(s => s.Pokemon)
                .ToList();
            var initialEnemyPokemon = field.EnemySide.Slots
                .Where(s => !s.IsEmpty && s.Pokemon != null)
                .Select(s => s.Pokemon)
                .ToList();

            // Publish battle started event with setup information
            _eventBus.PublishEvent(new BattleEvent(
                BattleEventType.BattleStarted,
                turnNumber: 0,
                isPlayerSide: false,
                pokemon: initialPlayerPokemon.FirstOrDefault(),
                data: new BattleEventData
                {
                    PlayerSlots = field.Rules.PlayerSlots,
                    EnemySlots = field.Rules.EnemySlots,
                    PlayerPartySize = field.PlayerSide?.Party?.Count ?? 0,
                    EnemyPartySize = field.EnemySide?.Party?.Count ?? 0,
                    BattleMode = battleMode,
                    RemainingPokemon = initialPlayerPokemon.Count,
                    TotalPokemon = initialEnemyPokemon.Count
                }));

            // Publish additional events for initial Pokemon in each slot
            foreach (var slot in field.PlayerSide.Slots)
            {
                if (!slot.IsEmpty && slot.Pokemon != null)
                {
                    _eventBus.PublishEvent(new BattleEvent(
                        BattleEventType.PokemonSwitched,
                        turnNumber: 0,
                        isPlayerSide: true,
                        pokemon: slot.Pokemon,
                        data: new BattleEventData
                        {
                            SwitchedIn = slot.Pokemon,
                            SlotIndex = slot.SlotIndex
                        }));
                }
            }

            foreach (var slot in field.EnemySide.Slots)
            {
                if (!slot.IsEmpty && slot.Pokemon != null)
                {
                    _eventBus.PublishEvent(new BattleEvent(
                        BattleEventType.PokemonSwitched,
                        turnNumber: 0,
                        isPlayerSide: false,
                        pokemon: slot.Pokemon,
                        data: new BattleEventData
                        {
                            SwitchedIn = slot.Pokemon,
                            SlotIndex = slot.SlotIndex
                        }));
                }
            }

            return await Task.FromResult(new List<BattleAction>());
        }
    }
}
