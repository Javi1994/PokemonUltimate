using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.AI;
using PokemonUltimate.Combat.Providers;
using PokemonUltimate.Core.Data.Constants;
using PokemonUltimate.Core.Domain.Instances;
using PokemonUltimate.Core.Domain.Instances.Pokemon;

namespace PokemonUltimate.Combat.Processors.Phases
{
    /// <summary>
    /// Collects actions from all active slots with coordination for strategic switches.
    /// Prevents duplicate Pokemon selections when multiple slots want to switch.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `docs/features/2-combat-system/2.6-combat-engine/architecture.md`
    /// </remarks>
    public class ActionCollectionProcessor : IActionGeneratingPhaseProcessor
    {
        private readonly IRandomProvider _randomProvider;

        /// <summary>
        /// Creates a new ActionCollectionProcessor.
        /// </summary>
        /// <param name="randomProvider">Random provider for coordinating switches. Cannot be null.</param>
        /// <exception cref="ArgumentNullException">If randomProvider is null.</exception>
        public ActionCollectionProcessor(IRandomProvider randomProvider)
        {
            _randomProvider = randomProvider ?? throw new ArgumentNullException(nameof(randomProvider));
        }

        /// <summary>
        /// Gets the phase this processor handles.
        /// </summary>
        public BattlePhase Phase => BattlePhase.ActionCollection;

        /// <summary>
        /// Collects actions from all active slots with coordination for strategic switches.
        /// </summary>
        /// <param name="field">The battlefield. Cannot be null.</param>
        /// <returns>List of collected actions.</returns>
        /// <exception cref="ArgumentNullException">If field is null.</exception>
        public async Task<List<BattleAction>> ProcessAsync(BattleField field)
        {
            if (field == null)
                throw new ArgumentNullException(nameof(field), ErrorMessages.FieldCannotBeNull);

            var pendingActions = new List<BattleAction>();

            // Collect actions per side to coordinate switches
            var playerActions = new List<(BattleSlot slot, BattleAction action)>();
            var enemyActions = new List<(BattleSlot slot, BattleAction action)>();

            // First pass: collect all actions
            foreach (var slot in field.GetAllActiveSlots())
            {
                if (slot.ActionProvider == null)
                    continue;

                var action = await slot.ActionProvider.GetAction(field, slot);
                if (action != null)
                {
                    if (slot.Side.IsPlayer)
                        playerActions.Add((slot, action));
                    else
                        enemyActions.Add((slot, action));
                }
            }

            // Coordinate switches for player side
            var coordinatedPlayerActions = CoordinateSwitches(playerActions, field.PlayerSide);
            pendingActions.AddRange(coordinatedPlayerActions);

            // Coordinate switches for enemy side
            var coordinatedEnemyActions = CoordinateSwitches(enemyActions, field.EnemySide);
            pendingActions.AddRange(coordinatedEnemyActions);

            return pendingActions;
        }

        /// <summary>
        /// Coordinates switches for a side to prevent duplicate Pokemon selections.
        /// </summary>
        private List<BattleAction> CoordinateSwitches(
            List<(BattleSlot slot, BattleAction action)> actions,
            BattleSide side)
        {
            var coordinatedActions = new List<BattleAction>();
            var reservedPokemon = new HashSet<PokemonInstance>();

            // Separate switch actions from other actions
            var switchActions = new List<(BattleSlot slot, SwitchAction action)>();
            var otherActions = new List<BattleAction>();

            foreach (var (slot, action) in actions)
            {
                if (action is SwitchAction switchAction)
                {
                    switchActions.Add((slot, switchAction));
                }
                else
                {
                    otherActions.Add(action);
                }
            }

            // Process switch actions with coordination
            foreach (var (slot, switchAction) in switchActions)
            {
                // Check if the selected Pokemon is already reserved
                if (reservedPokemon.Contains(switchAction.NewPokemon))
                {
                    // Find an alternative Pokemon
                    var availableSwitches = side.GetAvailableSwitches(reservedPokemon).ToList();
                    if (availableSwitches.Count > 0)
                    {
                        // Select a different Pokemon
                        int index = _randomProvider.Next(0, availableSwitches.Count);
                        var alternativePokemon = availableSwitches[index];
                        reservedPokemon.Add(alternativePokemon);

                        // Create new switch action with alternative Pokemon
                        var coordinatedSwitch = new SwitchAction(slot, alternativePokemon);
                        coordinatedActions.Add(coordinatedSwitch);
                    }
                    // If no alternative available, skip this switch (shouldn't happen in normal gameplay)
                }
                else
                {
                    // Pokemon not reserved, use original switch action
                    reservedPokemon.Add(switchAction.NewPokemon);
                    coordinatedActions.Add(switchAction);
                }
            }

            // Add non-switch actions (they don't need coordination)
            coordinatedActions.AddRange(otherActions);

            return coordinatedActions;
        }
    }
}
