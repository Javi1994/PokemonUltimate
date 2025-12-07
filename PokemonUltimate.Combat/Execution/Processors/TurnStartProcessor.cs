using System;
using System.Collections.Generic;
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
    /// Processes the start of a turn.
    /// Publishes turn started events and notifies observers.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `docs/features/2-combat-system/2.6-combat-engine/architecture.md`
    /// </remarks>
    public class TurnStartProcessor : IActionGeneratingPhaseProcessor
    {
        private readonly IBattleEventBus _eventBus;

        /// <summary>
        /// Creates a new TurnStartProcessor.
        /// </summary>
        /// <param name="eventBus">The event bus for publishing events. Cannot be null.</param>
        /// <exception cref="ArgumentNullException">If eventBus is null.</exception>
        public TurnStartProcessor(IBattleEventBus eventBus)
        {
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        }

        /// <summary>
        /// Gets the phase this processor handles.
        /// </summary>
        public BattlePhase Phase => BattlePhase.TurnStart;

        /// <summary>
        /// Processes the start of a turn.
        /// </summary>
        /// <param name="field">The battlefield. Cannot be null.</param>
        /// <param name="turnNumber">The current turn number.</param>
        /// <param name="queue">The battle queue for notifying observers. Can be null.</param>
        /// <returns>Empty list (no actions generated, only events published).</returns>
        /// <exception cref="ArgumentNullException">If field is null.</exception>
        public async Task<List<BattleAction>> ProcessAsync(BattleField field, int turnNumber, BattleQueue queue = null)
        {
            if (field == null)
                throw new ArgumentNullException(nameof(field), ErrorMessages.FieldCannotBeNull);

            // Publish turn started event
            _eventBus.PublishEvent(new BattleEvent(
                BattleEventType.TurnStarted,
                turnNumber: turnNumber,
                isPlayerSide: false,
                data: new BattleEventData()));

            // Notify observers about turn start
            if (queue != null)
            {
                foreach (var observer in queue.GetObservers())
                {
                    observer.OnTurnStart(turnNumber, field);
                }
            }

            return await Task.FromResult(new List<BattleAction>());
        }

        /// <summary>
        /// Processes the start of a turn (overload without turn number).
        /// </summary>
        /// <param name="field">The battlefield. Cannot be null.</param>
        /// <returns>Empty list.</returns>
        public async Task<List<BattleAction>> ProcessAsync(BattleField field)
        {
            return await ProcessAsync(field, 0);
        }
    }
}
