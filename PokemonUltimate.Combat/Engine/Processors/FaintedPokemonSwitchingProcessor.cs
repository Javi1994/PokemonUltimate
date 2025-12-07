using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.AI;
using PokemonUltimate.Combat.Logging;
using PokemonUltimate.Combat.Providers;
using PokemonUltimate.Core.Instances;

namespace PokemonUltimate.Combat.Processors.Phases
{
    /// <summary>
    /// Handles automatic switching for fainted Pokemon in team battles.
    /// Checks all slots for fainted Pokemon and forces automatic switching if available.
    /// Coordinates switches per side to avoid duplicate Pokemon selections in doubles battles.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `docs/features/2-combat-system/2.6-combat-engine/architecture.md`
    /// </remarks>
    public class FaintedPokemonSwitchingProcessor : IActionGeneratingPhaseProcessor
    {
        private readonly IRandomProvider _randomProvider;
        private readonly IBattleLogger _logger;

        /// <summary>
        /// Creates a new FaintedPokemonSwitchingProcessor.
        /// </summary>
        /// <param name="randomProvider">Random provider for selecting Pokemon. Cannot be null.</param>
        /// <param name="logger">Logger for errors. If null, creates a default one.</param>
        /// <exception cref="ArgumentNullException">If randomProvider is null.</exception>
        public FaintedPokemonSwitchingProcessor(IRandomProvider randomProvider, IBattleLogger logger = null)
        {
            _randomProvider = randomProvider ?? throw new ArgumentNullException(nameof(randomProvider));
            _logger = logger ?? new Logging.NullBattleLogger();
        }

        /// <summary>
        /// Gets the phase this processor handles.
        /// </summary>
        public BattlePhase Phase => BattlePhase.FaintedPokemonSwitching;

        /// <summary>
        /// Processes automatic switching for fainted Pokemon.
        /// </summary>
        /// <param name="field">The battlefield. Cannot be null.</param>
        /// <returns>List of switch actions to execute.</returns>
        /// <exception cref="ArgumentNullException">If field is null.</exception>
        public async Task<List<BattleAction>> ProcessAsync(BattleField field)
        {
            if (field == null)
                throw new ArgumentNullException(nameof(field), Core.Constants.ErrorMessages.FieldCannotBeNull);

            var switchActions = new List<BattleAction>();

            // Process switches per side to coordinate and avoid duplicates
            await ProcessSideSwitches(field.PlayerSide, switchActions, field);
            await ProcessSideSwitches(field.EnemySide, switchActions, field);

            return switchActions;
        }

        /// <summary>
        /// Processes switches for a single side, coordinating to avoid duplicate Pokemon selections.
        /// </summary>
        private async Task ProcessSideSwitches(BattleSide side, List<BattleAction> switchActions, BattleField field)
        {
            if (side == null)
                return;

            // Find all slots that need switching
            var slotsNeedingSwitch = new List<BattleSlot>();
            foreach (var slot in side.Slots)
            {
                // Check if slot has a fainted Pokemon
                bool hasFaintedPokemon = slot.Pokemon != null && slot.Pokemon.IsFainted;

                // Check if slot is empty but party has active Pokemon available
                bool isEmptyButHasAvailablePokemon = slot.IsEmpty &&
                    side.Party != null &&
                    side.Party.Any(p => !p.IsFainted && !side.Slots.Any(s => s.Pokemon == p));

                if (hasFaintedPokemon || isEmptyButHasAvailablePokemon)
                {
                    slotsNeedingSwitch.Add(slot);
                }
            }

            // If no slots need switching, return early
            if (slotsNeedingSwitch.Count == 0)
                return;

            // Track Pokemon already selected for switching to avoid duplicates
            var reservedPokemon = new HashSet<Core.Instances.PokemonInstance>();

            // Process each slot that needs switching
            foreach (var slot in slotsNeedingSwitch)
            {
                if (slot.ActionProvider == null)
                    continue;

                try
                {
                    // Get available switches excluding already reserved Pokemon
                    var availableSwitches = side.GetAvailableSwitches(reservedPokemon).ToList();

                    // If no Pokemon available, skip this slot
                    if (availableSwitches.Count == 0)
                        continue;

                    // Select a Pokemon for this slot
                    Core.Instances.PokemonInstance selectedPokemon;

                    // For TeamBattleAI, we can simulate its selection logic
                    // Otherwise, get action from provider and validate
                    if (slot.ActionProvider is TeamBattleAI)
                    {
                        // Use random selection similar to TeamBattleAI
                        int index = _randomProvider.Next(0, availableSwitches.Count);
                        selectedPokemon = availableSwitches[index];
                    }
                    else
                    {
                        // For other AI types, get action normally but validate no duplicates
                        var action = await slot.ActionProvider.GetAction(field, slot);
                        if (action != null && action is SwitchAction providerSwitchAction)
                        {
                            // Check if this Pokemon is already reserved
                            if (reservedPokemon.Contains(providerSwitchAction.NewPokemon))
                            {
                                // Try to find an alternative from available switches
                                if (availableSwitches.Count > 0)
                                {
                                    int index = _randomProvider.Next(0, availableSwitches.Count);
                                    selectedPokemon = availableSwitches[index];
                                }
                                else
                                {
                                    // No alternative available, skip this slot
                                    continue;
                                }
                            }
                            else
                            {
                                selectedPokemon = providerSwitchAction.NewPokemon;
                            }
                        }
                        else
                        {
                            // No action returned, skip this slot
                            continue;
                        }
                    }

                    // Reserve this Pokemon and create switch action
                    reservedPokemon.Add(selectedPokemon);
                    var newSwitchAction = new SwitchAction(slot, selectedPokemon);
                    switchActions.Add(newSwitchAction);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error getting switch action for fainted Pokemon: {ex.Message}");
                }
            }
        }
    }
}
