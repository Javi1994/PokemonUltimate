using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Events;

namespace PokemonUltimate.Combat.Processors.Phases
{
    /// <summary>
    /// Processes the end of a battle.
    /// Publishes battle ended events with statistics.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `docs/features/2-combat-system/2.6-combat-engine/architecture.md`
    /// </remarks>
    public class BattleEndProcessor : IActionGeneratingPhaseProcessor
    {
        private readonly IBattleEventBus _eventBus;

        /// <summary>
        /// Creates a new BattleEndProcessor.
        /// </summary>
        /// <param name="eventBus">The event bus for publishing events. Cannot be null.</param>
        /// <exception cref="ArgumentNullException">If eventBus is null.</exception>
        public BattleEndProcessor(IBattleEventBus eventBus)
        {
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        }

        /// <summary>
        /// Gets the phase this processor handles.
        /// </summary>
        public BattlePhase Phase => BattlePhase.BattleEnd;

        /// <summary>
        /// Processes the end of a battle.
        /// </summary>
        /// <param name="field">The battlefield. Cannot be null.</param>
        /// <param name="result">The battle result. Cannot be null.</param>
        /// <returns>Empty list (no actions generated, only events published).</returns>
        /// <exception cref="ArgumentNullException">If field or result is null.</exception>
        public List<BattleAction> ProcessBattleEnd(BattleField field, BattleResult result)
        {
            if (field == null)
                throw new ArgumentNullException(nameof(field), Core.Constants.ErrorMessages.FieldCannotBeNull);
            if (result == null)
                throw new ArgumentNullException(nameof(result));

            // Calculate team statistics for battle ended event
            int playerFainted = field?.PlayerSide?.Party?.Count(p => p.IsFainted) ?? 0;
            int playerTotal = field?.PlayerSide?.Party?.Count ?? 0;
            int enemyFainted = field?.EnemySide?.Party?.Count(p => p.IsFainted) ?? 0;
            int enemyTotal = field?.EnemySide?.Party?.Count ?? 0;

            // Publish battle ended event
            _eventBus.PublishEvent(new BattleEvent(
                BattleEventType.BattleEnded,
                turnNumber: result.TurnsTaken,
                isPlayerSide: false,
                data: new BattleEventData
                {
                    Outcome = result.Outcome,
                    TotalTurns = result.TurnsTaken,
                    PlayerFainted = playerFainted,
                    PlayerTotal = playerTotal,
                    EnemyFainted = enemyFainted,
                    EnemyTotal = enemyTotal
                }));

            return new List<BattleAction>();
        }

        /// <summary>
        /// Processes the battle end phase (required by interface).
        /// </summary>
        /// <param name="field">The battlefield.</param>
        /// <returns>Empty list (use ProcessBattleEnd instead).</returns>
        public async Task<List<BattleAction>> ProcessAsync(BattleField field)
        {
            return await Task.FromResult(new List<BattleAction>());
        }
    }
}
