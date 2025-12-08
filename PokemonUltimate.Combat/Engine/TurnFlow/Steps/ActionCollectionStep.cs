using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Engine.TurnFlow.Definition;
using PokemonUltimate.Combat.Field;
using PokemonUltimate.Combat.Infrastructure.Providers.Definition;
using PokemonUltimate.Core.Domain.Instances.Pokemon;

namespace PokemonUltimate.Combat.Engine.TurnFlow.Steps
{
    /// <summary>
    /// Step que recolecta acciones de todos los slots activos.
    /// </summary>
    public class ActionCollectionStep : ITurnStep
    {
        private readonly IRandomProvider _randomProvider;

        public string StepName => "Action Collection";
        public bool ExecuteEvenIfFainted => false;

        public ActionCollectionStep(IRandomProvider randomProvider)
        {
            _randomProvider = randomProvider ?? throw new ArgumentNullException(nameof(randomProvider));
        }

        public async Task<bool> ExecuteAsync(TurnContext context)
        {
            var pendingActions = new List<BattleAction>();

            // Collect actions per side to coordinate switches
            var playerActions = new List<(BattleSlot slot, BattleAction action)>();
            var enemyActions = new List<(BattleSlot slot, BattleAction action)>();

            // First pass: collect all actions
            foreach (var slot in context.Field.GetAllActiveSlots())
            {
                if (slot.ActionProvider == null)
                    continue;

                var action = await slot.ActionProvider.GetAction(context.Field, slot);
                if (action != null)
                {
                    if (slot.Side.IsPlayer)
                        playerActions.Add((slot, action));
                    else
                        enemyActions.Add((slot, action));
                }
            }

            // Coordinate switches for player side
            var coordinatedPlayerActions = CoordinateSwitches(playerActions, context.Field.PlayerSide);
            pendingActions.AddRange(coordinatedPlayerActions);

            // Coordinate switches for enemy side
            var coordinatedEnemyActions = CoordinateSwitches(enemyActions, context.Field.EnemySide);
            pendingActions.AddRange(coordinatedEnemyActions);

            context.CollectedActions = pendingActions;
            return true;
        }

        /// <summary>
        /// Coordinates switches for a side to prevent duplicate Pokemon selections.
        /// </summary>
        private List<BattleAction> CoordinateSwitches(
            List<(BattleSlot slot, BattleAction action)> actions,
            Field.BattleSide side)
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
