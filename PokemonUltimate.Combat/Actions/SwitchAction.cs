using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PokemonUltimate.Combat.Engine;
using PokemonUltimate.Combat.Events;
using PokemonUltimate.Combat.Factories;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Constants;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Instances;

namespace PokemonUltimate.Combat.Actions
{
    /// <summary>
    /// Switches a Pokemon in battle.
    /// The current Pokemon is sent to the bench and a new Pokemon takes its place.
    /// Battle state (stat stages, volatile status) is reset for the new Pokemon.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.5: Combat Actions
    /// **Documentation**: See `docs/features/2-combat-system/2.5-combat-actions/architecture.md`
    /// </remarks>
    public class SwitchAction : BattleAction
    {
        private readonly IEntryHazardProcessor _entryHazardProcessor;
        private readonly IBattleTriggerProcessor _battleTriggerProcessor;

        /// <summary>
        /// The slot being switched.
        /// </summary>
        public BattleSlot Slot { get; }

        /// <summary>
        /// The Pokemon to switch in.
        /// </summary>
        public PokemonInstance NewPokemon { get; }

        /// <summary>
        /// Optional function to get HazardData by type for entry hazard processing.
        /// If null, entry hazards will not be processed.
        /// </summary>
        public Func<HazardType, HazardData> GetHazardData { get; }

        /// <summary>
        /// Switch actions have highest priority (+6).
        /// </summary>
        public override int Priority => 6;

        /// <summary>
        /// Switch actions cannot be blocked.
        /// </summary>
        public override bool CanBeBlocked => false;

        /// <summary>
        /// Creates a new switch action.
        /// </summary>
        /// <param name="slot">The slot to switch. Cannot be null.</param>
        /// <param name="newPokemon">The Pokemon to switch in. Cannot be null.</param>
        /// <param name="getHazardData">Optional function to get HazardData for entry hazard processing. If null, hazards won't be processed.</param>
        /// <param name="entryHazardProcessor">The entry hazard processor. If null, creates a temporary one.</param>
        /// <param name="battleTriggerProcessor">The battle trigger processor. If null, creates a temporary one.</param>
        /// <exception cref="ArgumentNullException">If slot or newPokemon is null.</exception>
        public SwitchAction(
            BattleSlot slot,
            PokemonInstance newPokemon,
            Func<HazardType, HazardData> getHazardData = null,
            IEntryHazardProcessor entryHazardProcessor = null,
            IBattleTriggerProcessor battleTriggerProcessor = null) : base(slot)
        {
            Slot = slot ?? throw new ArgumentNullException(nameof(slot), ErrorMessages.PokemonCannotBeNull);
            NewPokemon = newPokemon ?? throw new ArgumentNullException(nameof(newPokemon), ErrorMessages.PokemonCannotBeNull);
            GetHazardData = getHazardData;

            // Create EntryHazardProcessor if not provided (temporary until full DI refactoring)
            _entryHazardProcessor = entryHazardProcessor ?? new EntryHazardProcessor(new DamageContextFactory());

            // Create BattleTriggerProcessor if not provided (temporary until full DI refactoring)
            _battleTriggerProcessor = battleTriggerProcessor ?? new BattleTriggerProcessor();
        }

        /// <summary>
        /// Switches the Pokemon in the slot.
        /// Returns the old Pokemon to the bench and places the new one in the slot.
        /// Resets battle state for the new Pokemon.
        /// </summary>
        public override IEnumerable<BattleAction> ExecuteLogic(BattleField field)
        {
            if (field == null)
                throw new ArgumentNullException(nameof(field));

            if (Slot.IsEmpty)
                return Enumerable.Empty<BattleAction>();

            var side = Slot.Side;
            if (side == null)
                return Enumerable.Empty<BattleAction>();

            // Mark Pokemon as switching out (for Pursuit detection)
            Slot.AddVolatileStatus(VolatileStatus.SwitchingOut);

            // Get the current Pokemon
            var oldPokemon = Slot.Pokemon;

            // Switch Pokemon
            // Note: SetPokemon automatically resets battle state for the new Pokemon
            Slot.SetPokemon(NewPokemon);

            // Note: The party management is handled externally by BattleField initialization.
            // BattleSide.Party is a read-only reference to the party provided during initialization.
            // When switching, we simply replace the Pokemon in the slot. The old Pokemon
            // remains in the party list (if it was there) and can be switched back in later.
            // If the old Pokemon is not in the party, it means it was a temporary instance
            // or the party structure is managed elsewhere in the system.

            // Battle state is reset automatically by SetPokemon -> ResetBattleState

            var allActions = new List<BattleAction>();

            // Process entry hazards if hazard data provider is available
            if (GetHazardData != null)
            {
                var hazardActions = _entryHazardProcessor.ProcessHazards(Slot, NewPokemon, field, GetHazardData);
                allActions.AddRange(hazardActions);
            }

            // Trigger OnSwitchIn for abilities and items
            var switchInActions = _battleTriggerProcessor.ProcessTrigger(BattleTrigger.OnSwitchIn, field);
            allActions.AddRange(switchInActions);

            return allActions;
        }

        /// <summary>
        /// Plays switch-out and switch-in animations.
        /// </summary>
        public override Task ExecuteVisual(IBattleView view)
        {
            if (view == null)
                throw new ArgumentNullException(nameof(view));

            if (Slot.IsEmpty)
                return Task.CompletedTask;

            // Play switch-out animation first, then switch-in
            return Task.WhenAll(
                view.PlaySwitchOutAnimation(Slot),
                view.PlaySwitchInAnimation(Slot)
            );
        }
    }
}

