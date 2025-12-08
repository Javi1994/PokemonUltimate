using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Engine.TurnFlow.Definition;
using PokemonUltimate.Combat.Field;
using PokemonUltimate.Combat.Infrastructure.Logging.Definition;
using PokemonUltimate.Combat.Infrastructure.Providers.Definition;
using PokemonUltimate.Core.Domain.Instances.Pokemon;

namespace PokemonUltimate.Combat.Engine.TurnFlow.Steps
{
    /// <summary>
    /// Step reutilizable que verifica y maneja Pokemon debilitados.
    /// </summary>
    public class FaintedPokemonCheckStep : ITurnStep
    {
        private readonly IRandomProvider _randomProvider;
        private readonly IBattleLogger _logger;

        public string StepName => "Fainted Pokemon Check";
        public bool ExecuteEvenIfFainted => true; // Always execute

        public FaintedPokemonCheckStep(IRandomProvider randomProvider, IBattleLogger logger)
        {
            _randomProvider = randomProvider ?? throw new ArgumentNullException(nameof(randomProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> ExecuteAsync(TurnContext context)
        {
            var switchActions = new List<BattleAction>();
            bool hasFaintedPokemon = false;

            // Check if there are any fainted Pokemon on either side
            bool playerHasFainted = context.Field.PlayerSide.Slots.Any(s => s.Pokemon != null && s.Pokemon.IsFainted);
            bool enemyHasFainted = context.Field.EnemySide.Slots.Any(s => s.Pokemon != null && s.Pokemon.IsFainted);
            hasFaintedPokemon = playerHasFainted || enemyHasFainted;

            // Process switches per side to coordinate and avoid duplicates
            await ProcessSideSwitches(context.Field.PlayerSide, switchActions, context.Field);
            await ProcessSideSwitches(context.Field.EnemySide, switchActions, context.Field);

            if (switchActions.Count > 0)
            {
                context.GeneratedActions.AddRange(switchActions);
            }

            // Set HasFaintedPokemon flag even if no switches were created
            // This ensures battle end checks are performed
            context.HasFaintedPokemon = hasFaintedPokemon;

            return true;
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
            var reservedPokemon = new HashSet<PokemonInstance>();

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

                    // Select a Pokemon for this slot using the provider's auto-switch logic
                    // This allows IAs that inherit from ActionProviderBase to provide custom selection logic
                    // If provider doesn't support SelectAutoSwitch or returns null, fall back to random selection
                    PokemonInstance selectedPokemon = null;

                    if (slot.ActionProvider is ActionProviderBase baseProvider)
                    {
                        selectedPokemon = await baseProvider.SelectAutoSwitch(field, slot, availableSwitches);
                    }

                    if (selectedPokemon == null)
                    {
                        // Provider doesn't support SelectAutoSwitch or didn't provide selection, use random selection as fallback
                        if (availableSwitches.Count > 0)
                        {
                            int index = _randomProvider.Next(0, availableSwitches.Count);
                            selectedPokemon = availableSwitches[index];
                        }
                        else
                        {
                            // No Pokemon available, skip this slot
                            continue;
                        }
                    }
                    else
                    {
                        // Validate that selected Pokemon is in available list (safety check)
                        if (!availableSwitches.Contains(selectedPokemon))
                        {
                            _logger.LogWarning($"Provider selected Pokemon {selectedPokemon.DisplayName} not in available list, using random selection");
                            if (availableSwitches.Count > 0)
                            {
                                int index = _randomProvider.Next(0, availableSwitches.Count);
                                selectedPokemon = availableSwitches[index];
                            }
                            else
                            {
                                continue;
                            }
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
