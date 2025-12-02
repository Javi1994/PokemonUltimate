using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Engine;
using PokemonUltimate.Combat.Helpers;
using PokemonUltimate.Core.Constants;
using PokemonUltimate.Core.Instances;

namespace PokemonUltimate.Combat
{
    /// <summary>
    /// Main controller for battle execution.
    /// Orchestrates the full battle loop, turn execution, and outcome detection.
    /// </summary>
    public class CombatEngine
    {
        private IBattleView _view;
        private IActionProvider _playerProvider;
        private IActionProvider _enemyProvider;

        /// <summary>
        /// The battlefield for this battle.
        /// </summary>
        public BattleField Field { get; private set; }

        /// <summary>
        /// The action queue for processing battle actions.
        /// </summary>
        public BattleQueue Queue { get; private set; }

        /// <summary>
        /// The current outcome of the battle.
        /// </summary>
        public BattleOutcome Outcome { get; private set; }

        /// <summary>
        /// Initializes the combat engine with parties and action providers.
        /// </summary>
        /// <param name="rules">Battle configuration. Cannot be null.</param>
        /// <param name="playerParty">Player's Pokemon party. Cannot be null.</param>
        /// <param name="enemyParty">Enemy's Pokemon party. Cannot be null.</param>
        /// <param name="playerProvider">Provider for player actions. Cannot be null.</param>
        /// <param name="enemyProvider">Provider for enemy actions. Cannot be null.</param>
        /// <param name="view">Battle view for visual feedback. Cannot be null.</param>
        /// <exception cref="ArgumentNullException">If any parameter is null.</exception>
        public void Initialize(
            BattleRules rules,
            IReadOnlyList<PokemonInstance> playerParty,
            IReadOnlyList<PokemonInstance> enemyParty,
            IActionProvider playerProvider,
            IActionProvider enemyProvider,
            IBattleView view)
        {
            if (rules == null)
                throw new ArgumentNullException(nameof(rules));
            if (playerParty == null)
                throw new ArgumentNullException(nameof(playerParty), ErrorMessages.PartyCannotBeNull);
            if (enemyParty == null)
                throw new ArgumentNullException(nameof(enemyParty), ErrorMessages.PartyCannotBeNull);
            if (playerProvider == null)
                throw new ArgumentNullException(nameof(playerProvider));
            if (enemyProvider == null)
                throw new ArgumentNullException(nameof(enemyProvider));
            if (view == null)
                throw new ArgumentNullException(nameof(view));

            _playerProvider = playerProvider;
            _enemyProvider = enemyProvider;
            _view = view;

            Field = new BattleField();
            Field.Initialize(rules, playerParty, enemyParty);

            // Assign action providers to slots
            foreach (var slot in Field.PlayerSide.Slots)
            {
                slot.ActionProvider = _playerProvider;
            }

            foreach (var slot in Field.EnemySide.Slots)
            {
                slot.ActionProvider = _enemyProvider;
            }

            Queue = new BattleQueue();
            Outcome = BattleOutcome.Ongoing;
        }

        /// <summary>
        /// Runs the complete battle until a conclusion is reached.
        /// </summary>
        /// <returns>The detailed battle result.</returns>
        public async Task<BattleResult> RunBattle()
        {
            if (Field == null)
                throw new InvalidOperationException("CombatEngine must be initialized before running battle.");

            int turnCount = 0;
            const int maxTurns = 1000; // Safety limit

            while (Outcome == BattleOutcome.Ongoing && turnCount < maxTurns)
            {
                await RunTurn();
                turnCount++;

                // Check outcome after each turn
                Outcome = BattleArbiter.CheckOutcome(Field);
            }

            // Generate result
            var result = new BattleResult
            {
                Outcome = Outcome,
                TurnsTaken = turnCount
            };

            // TODO: Calculate MVP, defeated enemies, EXP, loot (Phase 2.7+)

            return result;
        }

        /// <summary>
        /// Executes a single turn of battle.
        /// </summary>
        public async Task RunTurn()
        {
            if (Field == null)
                throw new InvalidOperationException("CombatEngine must be initialized before running turn.");

            // 1. Collect actions from all active slots
            var pendingActions = new List<BattleAction>();
            foreach (var slot in Field.GetAllActiveSlots())
            {
                if (slot.ActionProvider != null)
                {
                    var action = await slot.ActionProvider.GetAction(Field, slot);
                    if (action != null)
                    {
                        pendingActions.Add(action);
                    }
                }
            }

            // 2. Sort by turn order (priority, then speed)
            var sortedActions = TurnOrderResolver.SortActions(pendingActions, Field);

            // 3. Enqueue all actions in sorted order
            Queue.EnqueueRange(sortedActions);

            // 4. Process the queue
            await Queue.ProcessQueue(Field, _view);

            // 5. End-of-turn effects
            var endOfTurnActions = EndOfTurnProcessor.ProcessEffects(Field);
            if (endOfTurnActions.Count > 0)
            {
                Queue.EnqueueRange(endOfTurnActions);
                await Queue.ProcessQueue(Field, _view);
            }
        }
    }
}

