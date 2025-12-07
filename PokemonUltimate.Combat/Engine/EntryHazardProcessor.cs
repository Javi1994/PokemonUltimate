using System;
using System.Collections.Generic;
using System.Linq;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Damage;
using PokemonUltimate.Combat.Factories;
using PokemonUltimate.Content.Catalogs.Abilities;
using PokemonUltimate.Content.Extensions;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Constants;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;
using PokemonUltimate.Core.Localization;

namespace PokemonUltimate.Combat.Engine
{
    /// <summary>
    /// Processes entry hazards when a Pokemon switches in.
    /// Generates actions for damage, status application, and stat changes.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.14: Hazards System
    /// **Documentation**: See `docs/features/2-combat-system/2.14-hazards-system/README.md`
    /// </remarks>
    public class EntryHazardProcessor : IEntryHazardProcessor
    {
        private readonly DamageContextFactory _damageContextFactory;

        /// <summary>
        /// Creates a new EntryHazardProcessor with a damage context factory.
        /// </summary>
        /// <param name="damageContextFactory">The factory for creating damage contexts. Cannot be null.</param>
        /// <exception cref="ArgumentNullException">If damageContextFactory is null.</exception>
        public EntryHazardProcessor(DamageContextFactory damageContextFactory)
        {
            _damageContextFactory = damageContextFactory ?? throw new ArgumentNullException(nameof(damageContextFactory), ErrorMessages.FieldCannotBeNull);
        }

        /// <summary>
        /// Processes all entry hazards on the opposing side when a Pokemon switches in.
        /// </summary>
        /// <param name="slot">The slot the Pokemon is switching into.</param>
        /// <param name="pokemon">The Pokemon switching in.</param>
        /// <param name="field">The battlefield.</param>
        /// <param name="getHazardData">Function to get HazardData by type. Cannot be null.</param>
        /// <returns>List of actions to execute for entry hazards.</returns>
        public List<BattleAction> ProcessHazards(BattleSlot slot, PokemonInstance pokemon, BattleField field, Func<HazardType, HazardData> getHazardData)
        {
            if (slot == null)
                throw new ArgumentNullException(nameof(slot));
            if (pokemon == null)
                throw new ArgumentNullException(nameof(pokemon));
            if (field == null)
                throw new ArgumentNullException(nameof(field), ErrorMessages.FieldCannotBeNull);
            if (getHazardData == null)
                throw new ArgumentNullException(nameof(getHazardData));

            var actions = new List<BattleAction>();
            var opposingSide = field.GetOppositeSide(slot.Side);

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
                var provider = LocalizationManager.Instance;
                var hazardName = hazardData.GetLocalizedName(provider);
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
                var provider = LocalizationManager.Instance;
                var hazardName = hazardData.GetLocalizedName(provider);
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
                var provider = LocalizationManager.Instance;
                var hazardName = hazardData.GetLocalizedName(provider);
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
                var provider = LocalizationManager.Instance;
                var hazardName = hazardData.GetLocalizedName(provider);
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

                // Check for Contrary ability (reverses stat changes) - use catalog lookup instead of hardcoded string
                var contraryAbility = AbilityCatalog.GetByName("Contrary");
                if (pokemon.Ability != null && contraryAbility != null &&
                    pokemon.Ability.Name.Equals(contraryAbility.Name, StringComparison.OrdinalIgnoreCase))
                {
                    stages = -stages; // Reverse the stat change
                }

                var provider = LocalizationManager.Instance;
                var hazardName = hazardData.GetLocalizedName(provider);
                actions.Add(new MessageAction(provider.GetString(LocalizationKey.HazardStickyWebSpeed, pokemon.DisplayName, hazardName)));
                actions.Add(new StatChangeAction(null, slot, hazardData.StatToLower.Value, stages));
            }
        }

        /// <summary>
        /// Calculates type effectiveness for a defending Pokemon.
        /// </summary>
        private float CalculateTypeEffectiveness(PokemonType attackingType, PokemonType primaryType, PokemonType? secondaryType)
        {
            float effectiveness1 = TypeEffectiveness.GetEffectiveness(attackingType, primaryType);
            float effectiveness2 = secondaryType.HasValue
                ? TypeEffectiveness.GetEffectiveness(attackingType, secondaryType.Value)
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
    }
}

