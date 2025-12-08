using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Actions.Registry;
using PokemonUltimate.Combat.Engine.Processors.Definition;
using PokemonUltimate.Combat.Field;
using PokemonUltimate.Combat.Infrastructure.Factories;
using PokemonUltimate.Combat.Utilities.Extensions;
using PokemonUltimate.Content.Catalogs.Field;
using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Core.Data.Constants;
using PokemonUltimate.Core.Data.Enums;
using PokemonUltimate.Core.Domain.Instances.Pokemon;
using PokemonUltimate.Core.Services;
using PokemonUltimate.Localization.Constants;
using PokemonUltimate.Localization.Extensions;
using PokemonUltimate.Localization.Services;

namespace PokemonUltimate.Combat.Engine.Processors
{
    /// <summary>
    /// Processes everything that happens when a Pokemon switches in:
    /// - Entry hazards (Spikes, Stealth Rock, etc.)
    /// - Abilities (Intimidate, etc.)
    /// - Items (Focus Sash preparation, etc.)
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `docs/features/2-combat-system/2.6-combat-engine/architecture.md`
    /// </remarks>
    public class SwitchInProcessor : IActionGeneratingPhaseProcessor
    {
        private readonly DamageContextFactory _damageContextFactory;
        private readonly BehaviorCheckerRegistry _behaviorRegistry;

        /// <summary>
        /// Creates a new SwitchInProcessor.
        /// </summary>
        /// <param name="damageContextFactory">The factory for creating damage contexts. Cannot be null.</param>
        /// <param name="behaviorRegistry">The behavior checker registry. If null, creates a default one.</param>
        /// <exception cref="ArgumentNullException">If damageContextFactory is null.</exception>
        public SwitchInProcessor(DamageContextFactory damageContextFactory, BehaviorCheckerRegistry behaviorRegistry = null)
        {
            _damageContextFactory = damageContextFactory ?? throw new ArgumentNullException(nameof(damageContextFactory), ErrorMessages.FieldCannotBeNull);
            _behaviorRegistry = behaviorRegistry ?? new BehaviorCheckerRegistry();
        }

        /// <summary>
        /// Gets the phase this processor handles.
        /// </summary>
        public BattlePhase Phase => BattlePhase.SwitchIn;

        /// <summary>
        /// Processes everything that happens when a Pokemon switches in.
        /// </summary>
        /// <param name="slot">The slot the Pokemon is switching into.</param>
        /// <param name="pokemon">The Pokemon switching in.</param>
        /// <param name="field">The battlefield.</param>
        /// <returns>List of actions to execute.</returns>
        public List<BattleAction> ProcessSwitchIn(BattleSlot slot, PokemonInstance pokemon, BattleField field)
        {
            if (slot == null)
                throw new ArgumentNullException(nameof(slot));
            if (pokemon == null)
                throw new ArgumentNullException(nameof(pokemon));
            if (field == null)
                throw new ArgumentNullException(nameof(field), ErrorMessages.FieldCannotBeNull);

            var actions = new List<BattleAction>();

            // 1. Process entry hazards first
            var hazardActions = ProcessEntryHazards(slot, pokemon, field);
            actions.AddRange(hazardActions);

            // 2. Process ability (if exists)
            if (pokemon.Ability != null)
            {
                var abilityActions = ProcessAbility(pokemon.Ability, slot, field);
                actions.AddRange(abilityActions);
            }

            // 3. Process item (if exists)
            // Note: Items don't typically activate on switch-in, only abilities do
            // If needed in the future, add item processing here

            return actions;
        }

        /// <summary>
        /// Processes the switch-in phase (required by interface, but not used directly).
        /// </summary>
        /// <param name="field">The battlefield.</param>
        /// <returns>Empty list (use ProcessSwitchIn instead).</returns>
        public async Task<List<BattleAction>> ProcessAsync(BattleField field)
        {
            // This processor is called via ProcessSwitchIn method
            return await Task.FromResult(new List<BattleAction>());
        }

        /// <summary>
        /// Processes all entry hazards on the opposing side when a Pokemon switches in.
        /// </summary>
        private List<BattleAction> ProcessEntryHazards(BattleSlot slot, PokemonInstance pokemon, BattleField field)
        {
            var actions = new List<BattleAction>();
            var opposingSide = field.GetOppositeSide(slot.Side);
            Func<HazardType, HazardData> getHazardData = HazardCatalog.GetByType;

            // Process each hazard type
            ProcessSpikes(opposingSide, slot, pokemon, field, getHazardData, actions);
            ProcessStealthRock(opposingSide, slot, pokemon, field, getHazardData, actions);
            ProcessToxicSpikes(opposingSide, slot, pokemon, field, getHazardData, actions);
            ProcessStickyWeb(opposingSide, slot, pokemon, field, getHazardData, actions);

            return actions;
        }

        /// <summary>
        /// Processes Spikes hazard.
        /// </summary>
        private void ProcessSpikes(BattleSide side, BattleSlot slot, PokemonInstance pokemon, BattleField field, Func<HazardType, HazardData> getHazardData, List<BattleAction> actions)
        {
            if (!side.HasHazard(HazardType.Spikes))
                return;

            var hazardData = getHazardData(HazardType.Spikes);
            if (hazardData == null)
                return;

            // Check if Pokemon is affected (not Flying, not Levitate)
            if (!hazardData.AffectsPokemon(pokemon.Species.PrimaryType, pokemon.Species.SecondaryType, pokemon.Ability?.Name))
                return;

            int layers = side.GetHazardLayers(HazardType.Spikes);
            float damageFraction = hazardData.GetDamage(layers);
            int damage = CalculateHazardDamage(pokemon.MaxHP, damageFraction);

            if (damage > 0)
            {
                var provider = LocalizationService.Instance;
                var hazardName = hazardData.GetDisplayName(provider);
                actions.Add(new MessageAction(provider.GetString(LocalizationKey.HazardSpikesDamage, pokemon.DisplayName, hazardName)));
                actions.Add(CreateHazardDamageAction(slot, field, damage));
            }
        }

        /// <summary>
        /// Processes Stealth Rock hazard.
        /// </summary>
        private void ProcessStealthRock(BattleSide side, BattleSlot slot, PokemonInstance pokemon, BattleField field, Func<HazardType, HazardData> getHazardData, List<BattleAction> actions)
        {
            if (!side.HasHazard(HazardType.StealthRock))
                return;

            var hazardData = getHazardData(HazardType.StealthRock);
            if (hazardData == null)
                return;

            // Stealth Rock affects all Pokemon (including Flying)
            // Calculate type effectiveness vs Rock
            float effectiveness = CalculateTypeEffectiveness(
                hazardData.DamageType.Value,
                pokemon.Species.PrimaryType,
                pokemon.Species.SecondaryType);

            float baseDamageFraction = hazardData.GetDamage(1); // Always 1 layer
            float damageFraction = baseDamageFraction * effectiveness;
            int damage = CalculateHazardDamage(pokemon.MaxHP, damageFraction);

            if (damage > 0)
            {
                var provider = LocalizationService.Instance;
                var hazardName = hazardData.GetDisplayName(provider);
                actions.Add(new MessageAction(provider.GetString(LocalizationKey.HazardStealthRockDamage, pokemon.DisplayName, hazardName)));
                actions.Add(CreateHazardDamageAction(slot, field, damage));
            }
        }

        /// <summary>
        /// Processes Toxic Spikes hazard.
        /// </summary>
        private void ProcessToxicSpikes(BattleSide side, BattleSlot slot, PokemonInstance pokemon, BattleField field, Func<HazardType, HazardData> getHazardData, List<BattleAction> actions)
        {
            if (!side.HasHazard(HazardType.ToxicSpikes))
                return;

            var hazardData = getHazardData(HazardType.ToxicSpikes);
            if (hazardData == null)
                return;

            // Check if Poison type absorbs (removes spikes)
            if (hazardData.IsPoisonAbsorber(pokemon.Species.PrimaryType, pokemon.Species.SecondaryType))
            {
                side.RemoveHazard(HazardType.ToxicSpikes);
                var provider = LocalizationService.Instance;
                var hazardName = hazardData.GetDisplayName(provider);
                actions.Add(new MessageAction(provider.GetString(LocalizationKey.HazardToxicSpikesAbsorbed, pokemon.DisplayName, hazardName)));
                return;
            }

            // Check if Pokemon is affected (not Flying, not Levitate)
            if (!hazardData.AffectsPokemon(pokemon.Species.PrimaryType, pokemon.Species.SecondaryType, pokemon.Ability?.Name))
                return;

            int layers = side.GetHazardLayers(HazardType.ToxicSpikes);
            PersistentStatus status = hazardData.GetStatus(layers);

            if (status != PersistentStatus.None)
            {
                var provider = LocalizationService.Instance;
                var hazardName = hazardData.GetDisplayName(provider);
                actions.Add(new MessageAction(provider.GetString(LocalizationKey.HazardToxicSpikesStatus, pokemon.DisplayName, hazardName)));
                actions.Add(new ApplyStatusAction(null, slot, status));
            }
        }

        /// <summary>
        /// Processes Sticky Web hazard.
        /// </summary>
        private void ProcessStickyWeb(BattleSide side, BattleSlot slot, PokemonInstance pokemon, BattleField field, Func<HazardType, HazardData> getHazardData, List<BattleAction> actions)
        {
            if (!side.HasHazard(HazardType.StickyWeb))
                return;

            var hazardData = getHazardData(HazardType.StickyWeb);
            if (hazardData == null)
                return;

            // Check if Pokemon is affected (not Flying, not Levitate)
            if (!hazardData.AffectsPokemon(pokemon.Species.PrimaryType, pokemon.Species.SecondaryType, pokemon.Ability?.Name))
                return;

            if (hazardData.LowersStat)
            {
                int stages = hazardData.StatStages;

                // Use Behavior Checker to reverse stat changes if Pokemon has Contrary (eliminates hardcoding)
                var reversalChecker = _behaviorRegistry.GetStatChangeReversalChecker();
                stages = reversalChecker.ReverseStatChange(pokemon, stages);

                var provider = LocalizationService.Instance;
                var hazardName = hazardData.GetDisplayName(provider);
                actions.Add(new MessageAction(provider.GetString(LocalizationKey.HazardStickyWebSpeed, pokemon.DisplayName, hazardName)));
                actions.Add(new StatChangeAction(null, slot, hazardData.StatToLower.Value, stages));
            }
        }

        /// <summary>
        /// Calculates type effectiveness for a defending Pokemon.
        /// </summary>
        private float CalculateTypeEffectiveness(PokemonType attackingType, PokemonType primaryType, PokemonType? secondaryType)
        {
            float effectiveness1 = TypeEffectivenessService.GetEffectiveness(attackingType, primaryType);
            float effectiveness2 = secondaryType.HasValue
                ? TypeEffectivenessService.GetEffectiveness(attackingType, secondaryType.Value)
                : 1.0f;

            return effectiveness1 * effectiveness2;
        }

        /// <summary>
        /// Calculates hazard damage based on max HP and damage fraction.
        /// </summary>
        private int CalculateHazardDamage(int maxHP, float damageFraction)
        {
            int damage = (int)(maxHP * damageFraction);
            return Math.Max(1, damage); // Minimum 1 damage
        }

        /// <summary>
        /// Creates a DamageAction for hazard damage.
        /// </summary>
        private DamageAction CreateHazardDamageAction(BattleSlot slot, BattleField field, int damage)
        {
            var context = _damageContextFactory.CreateForHazardDamage(slot, damage, field);
            return new DamageAction(null, slot, context); // null user = system action
        }

        /// <summary>
        /// Processes an ability for switch-in effects.
        /// </summary>
        private List<BattleAction> ProcessAbility(AbilityData ability, BattleSlot slot, BattleField field)
        {
            var actions = new List<BattleAction>();

            // Check if this ability listens to OnSwitchIn trigger
            if (!ability.ListensTo(AbilityTrigger.OnSwitchIn))
                return actions;

            // Process based on ability effect
            switch (ability.Effect)
            {
                case AbilityEffect.LowerOpponentStat:
                    // Example: Intimidate
                    actions.AddRange(ProcessLowerOpponentStat(ability, slot, field));
                    break;

                    // Add other ability effects as needed
            }

            return actions;
        }

        /// <summary>
        /// Processes LowerOpponentStat ability effect (e.g., Intimidate).
        /// </summary>
        private List<BattleAction> ProcessLowerOpponentStat(AbilityData ability, BattleSlot slot, BattleField field)
        {
            var actions = new List<BattleAction>();

            if (ability.TargetStat == null)
                return actions;

            var opposingSide = field.GetOppositeSide(slot.Side);

            // Message for ability activation
            var provider = LocalizationService.Instance;
            var abilityName = ability.GetDisplayName(provider);
            actions.Add(new MessageAction(
                provider.GetString(LocalizationKey.AbilityActivated, slot.Pokemon.DisplayName, abilityName)));

            // Lower stat for all opposing active Pokemon
            foreach (var enemySlot in opposingSide.GetActiveSlots())
            {
                if (enemySlot.IsActive())
                {
                    actions.Add(new StatChangeAction(
                        slot, enemySlot, ability.TargetStat.Value, ability.StatStages));
                }
            }

            return actions;
        }
    }
}
