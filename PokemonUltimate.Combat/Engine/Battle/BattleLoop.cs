using System;
using System.Threading.Tasks;
using PokemonUltimate.Combat.Constants;
using PokemonUltimate.Combat.Events;
using PokemonUltimate.Combat.Logging;

namespace PokemonUltimate.Combat.Processors.Phases
{
    /// <summary>
    /// Manages the main battle loop until a conclusion is reached.
    /// Handles turn execution, outcome checking, and infinite loop detection.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `docs/features/2-combat-system/2.6-combat-engine/architecture.md`
    /// </remarks>
    public class BattleLoop
    {
        private readonly IBattleEventBus _eventBus;
        private readonly IBattleLogger _logger;
        private readonly Func<int, Task> _turnExecutor;

        /// <summary>
        /// Creates a new BattleLoop.
        /// </summary>
        /// <param name="eventBus">The event bus for publishing events. Cannot be null.</param>
        /// <param name="logger">Logger for warnings. If null, creates a default one.</param>
        /// <param name="turnExecutor">Function to execute a turn. Cannot be null.</param>
        /// <exception cref="ArgumentNullException">If eventBus or turnExecutor is null.</exception>
        public BattleLoop(
            IBattleEventBus eventBus,
            IBattleLogger logger,
            Func<int, Task> turnExecutor)
        {
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _logger = logger ?? new Logging.NullBattleLogger();
            _turnExecutor = turnExecutor ?? throw new ArgumentNullException(nameof(turnExecutor));
        }

        /// <summary>
        /// Runs the complete battle until a conclusion is reached.
        /// </summary>
        /// <param name="field">The battlefield. Cannot be null.</param>
        /// <param name="outcome">The current battle outcome (will be updated).</param>
        /// <returns>The detailed battle result.</returns>
        /// <exception cref="ArgumentNullException">If field is null.</exception>
        public async Task<BattleResult> RunBattle(BattleField field, BattleOutcome outcome)
        {
            if (field == null)
                throw new InvalidOperationException("BattleField must be initialized before running battle.");

            int turnCount = 0;
            int turnsWithoutHPChange = 0;
            int previousPlayerTotalHP = BattleUtilities.GetTotalHP(field.PlayerSide);
            int previousEnemyTotalHP = BattleUtilities.GetTotalHP(field.EnemySide);

            while (outcome == BattleOutcome.Ongoing && turnCount < BattleConstants.MaxTurns)
            {
                int currentTurn = turnCount + 1;

                // Execute turn
                await _turnExecutor(currentTurn);
                turnCount++;

                // Check outcome after each turn
                outcome = BattleArbiter.CheckOutcome(field);

                // Check for infinite loop: detect if HP hasn't changed
                int currentPlayerTotalHP = BattleUtilities.GetTotalHP(field.PlayerSide);
                int currentEnemyTotalHP = BattleUtilities.GetTotalHP(field.EnemySide);

                bool hpChanged = (currentPlayerTotalHP != previousPlayerTotalHP) ||
                                 (currentEnemyTotalHP != previousEnemyTotalHP);

                if (hpChanged)
                {
                    turnsWithoutHPChange = 0;
                    previousPlayerTotalHP = currentPlayerTotalHP;
                    previousEnemyTotalHP = currentEnemyTotalHP;
                }
                else
                {
                    turnsWithoutHPChange++;

                    // If no HP changes for many turns, consider it an infinite loop
                    if (turnsWithoutHPChange >= BattleConstants.MaxTurnsWithoutHPChange)
                    {
                        _logger.LogWarning($"Battle detected as infinite loop: {turnsWithoutHPChange} turns without HP changes. Ending in draw.");
                        outcome = BattleOutcome.Draw;
                        break;
                    }
                }

                // Publish turn ended event
                _eventBus.PublishEvent(new BattleEvent(
                    BattleEventType.TurnEnded,
                    turnNumber: turnCount,
                    isPlayerSide: false,
                    data: new BattleEventData()));

                // Note: Observer notifications are handled by TurnExecutor
            }

            // If we reached max turns without a conclusion, end in draw
            if (outcome == BattleOutcome.Ongoing && turnCount >= BattleConstants.MaxTurns)
            {
                _logger.LogWarning($"Battle reached maximum turn limit ({BattleConstants.MaxTurns}). Ending in draw.");
                outcome = BattleOutcome.Draw;
            }

            // Generate result
            return new BattleResult
            {
                Outcome = outcome,
                TurnsTaken = turnCount
            };
        }
    }
}
