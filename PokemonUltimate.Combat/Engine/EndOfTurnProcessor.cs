using System;
using System.Collections.Generic;
using System.Linq;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Damage;
using PokemonUltimate.Combat.Extensions;
using PokemonUltimate.Combat.Factories;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Constants;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Instances;

namespace PokemonUltimate.Combat.Engine
{
    /// <summary>
    /// Constants for end-of-turn damage calculations.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.8: End-of-Turn Effects
    /// **Documentation**: See `docs/features/2-combat-system/2.8-end-of-turn-effects/architecture.md`
    /// </remarks>
    public static class EndOfTurnConstants
    {
        public const float BurnDamageFraction = 1f / 16f;      // 0.0625
        public const float PoisonDamageFraction = 1f / 8f;     // 0.125
        public const float BadlyPoisonedBaseFraction = 1f / 16f; // 0.0625
        public const int MinimumDamage = 1;
    }

    /// <summary>
    /// Processes all end-of-turn effects after action queue is empty.
    /// Generates actions for status damage, item effects, and other end-of-turn mechanics.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.8: End-of-Turn Effects
    /// **Documentation**: See `docs/features/2-combat-system/2.8-end-of-turn-effects/architecture.md`
    /// </remarks>
    public class EndOfTurnProcessor : IEndOfTurnProcessor
    {
        private readonly DamageContextFactory _damageContextFactory;
        private readonly Effects.AccumulativeEffectTracker _accumulativeEffectTracker;

        /// <summary>
        /// Creates a new EndOfTurnProcessor with a damage context factory.
        /// </summary>
        /// <param name="damageContextFactory">The factory for creating damage contexts. Cannot be null.</param>
        /// <param name="accumulativeEffectTracker">Tracker for accumulative effects. If null, creates a default one.</param>
        /// <exception cref="ArgumentNullException">If damageContextFactory is null.</exception>
        public EndOfTurnProcessor(DamageContextFactory damageContextFactory, Effects.AccumulativeEffectTracker accumulativeEffectTracker = null)
        {
            _damageContextFactory = damageContextFactory ?? throw new ArgumentNullException(nameof(damageContextFactory), ErrorMessages.FieldCannotBeNull);
            _accumulativeEffectTracker = accumulativeEffectTracker ?? new Effects.AccumulativeEffectTracker();
        }

        /// <summary>
        /// Processes all end-of-turn effects for the battlefield.
        /// Returns actions to be executed (status damage, healing, etc.).
        /// </summary>
        /// <param name="field">The battlefield. Cannot be null.</param>
        /// <returns>List of actions to execute for end-of-turn effects.</returns>
        /// <exception cref="ArgumentNullException">If field is null.</exception>
        public List<BattleAction> ProcessEffects(BattleField field)
        {
            if (field == null)
                throw new ArgumentNullException(nameof(field), ErrorMessages.FieldCannotBeNull);

            var actions = new List<BattleAction>();

            // Process all active slots
            foreach (var slot in field.GetAllActiveSlots())
            {
                if (!slot.IsActive())
                    continue;

                var pokemon = slot.Pokemon;
                var statusActions = ProcessStatusEffects(slot, field);
                actions.AddRange(statusActions);
            }

            // Process weather damage for all active slots
            var weatherActions = ProcessWeatherDamage(field);
            actions.AddRange(weatherActions);

            // Process terrain healing for all active slots
            var terrainHealingActions = ProcessTerrainHealing(field);
            actions.AddRange(terrainHealingActions);

            // Remove Protect status at end of turn (but keep consecutive uses counter)
            // Reset damage tracking for Counter/Mirror Coat
            // Prepare semi-invulnerable moves for attack turn (if charging)
            foreach (var slot in field.GetAllActiveSlots())
            {
                if (slot.HasVolatileStatus(VolatileStatus.Protected))
                {
                    slot.RemoveVolatileStatus(VolatileStatus.Protected);
                }

                // If semi-invulnerable and charging, mark as ready for attack turn
                if (slot.HasVolatileStatus(VolatileStatus.SemiInvulnerable) && slot.IsSemiInvulnerableCharging)
                {
                    slot.SetSemiInvulnerableReady();
                }

                // Reset damage tracking for next turn (but keep stat stages and other volatile status)
                slot.ResetDamageTracking();
            }

            return actions;
        }

        /// <summary>
        /// Processes status effects for a single Pokemon slot.
        /// </summary>
        private List<BattleAction> ProcessStatusEffects(BattleSlot slot, BattleField field)
        {
            var actions = new List<BattleAction>();
            var pokemon = slot.Pokemon;

            switch (pokemon.Status)
            {
                case PersistentStatus.Burn:
                    actions.AddRange(ProcessBurn(slot, field));
                    break;

                case PersistentStatus.Poison:
                    actions.AddRange(ProcessPoison(slot, field));
                    break;

                case PersistentStatus.BadlyPoisoned:
                    actions.AddRange(ProcessBadlyPoisoned(slot, field));
                    break;

                case PersistentStatus.None:
                case PersistentStatus.Paralysis:
                case PersistentStatus.Sleep:
                case PersistentStatus.Freeze:
                    // No end-of-turn damage for these
                    break;
            }

            return actions;
        }

        /// <summary>
        /// Processes Burn status: deals 1/16 Max HP damage.
        /// </summary>
        private List<BattleAction> ProcessBurn(BattleSlot slot, BattleField field)
        {
            var actions = new List<BattleAction>();
            var pokemon = slot.Pokemon;

            int damage = CalculateBurnDamage(pokemon.MaxHP);
            actions.Add(new MessageAction(string.Format(GameMessages.StatusBurnDamage, pokemon.DisplayName)));
            actions.Add(CreateStatusDamageAction(slot, field, damage));

            return actions;
        }

        /// <summary>
        /// Processes Poison status: deals 1/8 Max HP damage.
        /// </summary>
        private List<BattleAction> ProcessPoison(BattleSlot slot, BattleField field)
        {
            var actions = new List<BattleAction>();
            var pokemon = slot.Pokemon;

            int damage = CalculatePoisonDamage(pokemon.MaxHP);
            actions.Add(new MessageAction(string.Format(GameMessages.StatusPoisonDamage, pokemon.DisplayName)));
            actions.Add(CreateStatusDamageAction(slot, field, damage));

            return actions;
        }

        /// <summary>
        /// Processes Badly Poisoned (Toxic) status: deals escalating damage and increments counter.
        /// Uses AccumulativeEffectTracker for centralized logic.
        /// </summary>
        private List<BattleAction> ProcessBadlyPoisoned(BattleSlot slot, BattleField field)
        {
            var actions = new List<BattleAction>();
            var pokemon = slot.Pokemon;

            // Process using AccumulativeEffectTracker
            int damage = _accumulativeEffectTracker.ProcessEffect(
                PersistentStatus.BadlyPoisoned,
                pokemon.MaxHP,
                pokemon.StatusTurnCounter,
                out int nextCounter);

            pokemon.StatusTurnCounter = nextCounter;

            actions.Add(new MessageAction(string.Format(GameMessages.StatusPoisonDamage, pokemon.DisplayName)));
            actions.Add(CreateStatusDamageAction(slot, field, damage));

            return actions;
        }

        /// <summary>
        /// Calculates Burn damage: 1/16 of Max HP (minimum 1).
        /// </summary>
        private int CalculateBurnDamage(int maxHP)
        {
            int damage = maxHP / 16;
            return damage.EnsureMinimumDamage();
        }

        /// <summary>
        /// Calculates Poison damage: 1/8 of Max HP (minimum 1).
        /// </summary>
        private int CalculatePoisonDamage(int maxHP)
        {
            int damage = maxHP / 8;
            return damage.EnsureMinimumDamage();
        }


        /// <summary>
        /// Creates a DamageAction for status damage using DamageContextFactory.
        /// </summary>
        private DamageAction CreateStatusDamageAction(BattleSlot slot, BattleField field, int damage)
        {
            var context = _damageContextFactory.CreateForStatusDamage(slot, damage, field);
            return new DamageAction(slot, slot, context);
        }

        /// <summary>
        /// Processes weather damage for all active Pokemon on the field.
        /// Weather like Sandstorm and Hail deal damage to non-immune types.
        /// </summary>
        /// <remarks>
        /// **Feature**: 2: Combat System
        /// **Sub-Feature**: 2.12: Weather System
        /// **Documentation**: See `docs/features/2-combat-system/2.12-weather-system/README.md`
        /// </remarks>
        private List<BattleAction> ProcessWeatherDamage(BattleField field)
        {
            var actions = new List<BattleAction>();

            // Check if there's active weather that deals damage
            if (field.WeatherData == null || !field.WeatherData.DealsDamage)
                return actions;

            var weatherData = field.WeatherData;

            // Process all active slots
            foreach (var slot in field.GetAllActiveSlots())
            {
                if (!slot.IsActive())
                    continue;

                var pokemon = slot.Pokemon;

                // Check if Pokemon is immune to weather damage by type
                bool isTypeImmune = IsTypeImmuneToWeatherDamage(pokemon, weatherData);

                // Check if Pokemon has ability that grants immunity
                bool hasImmunityAbility = HasWeatherImmunityAbility(pokemon, weatherData);

                // Skip if immune
                if (isTypeImmune || hasImmunityAbility)
                    continue;

                // Calculate and apply weather damage
                int damage = CalculateWeatherDamage(pokemon.MaxHP, weatherData.EndOfTurnDamage);
                string message = GetWeatherDamageMessage(field.Weather);

                actions.Add(new MessageAction(string.Format(message, pokemon.DisplayName)));
                actions.Add(CreateStatusDamageAction(slot, field, damage));
            }

            return actions;
        }

        /// <summary>
        /// Checks if a Pokemon's type makes it immune to weather damage.
        /// </summary>
        private bool IsTypeImmuneToWeatherDamage(PokemonInstance pokemon, WeatherData weatherData)
        {
            if (weatherData.DamageImmuneTypes == null || weatherData.DamageImmuneTypes.Length == 0)
                return false;

            // Check primary type
            if (weatherData.IsTypeImmuneToDamage(pokemon.Species.PrimaryType))
                return true;

            // Check secondary type (if exists)
            if (pokemon.Species.SecondaryType.HasValue)
            {
                if (weatherData.IsTypeImmuneToDamage(pokemon.Species.SecondaryType.Value))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Checks if a Pokemon has an ability that grants immunity to weather damage.
        /// </summary>
        private bool HasWeatherImmunityAbility(PokemonInstance pokemon, WeatherData weatherData)
        {
            if (pokemon.Ability == null)
                return false;

            if (weatherData.DamageImmunityAbilities == null || weatherData.DamageImmunityAbilities.Length == 0)
                return false;

            string abilityName = pokemon.Ability.Name;
            foreach (var immuneAbility in weatherData.DamageImmunityAbilities)
            {
                if (abilityName.Equals(immuneAbility, System.StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Calculates weather damage based on Max HP and damage fraction.
        /// </summary>
        private int CalculateWeatherDamage(int maxHP, float damageFraction)
        {
            int damage = (int)(maxHP * damageFraction);
            return damage.EnsureMinimumDamage();
        }

        /// <summary>
        /// Gets the appropriate message for weather damage.
        /// </summary>
        private string GetWeatherDamageMessage(Weather weather)
        {
            switch (weather)
            {
                case Weather.Sandstorm:
                    return GameMessages.WeatherSandstormDamage;
                case Weather.Hail:
                    return GameMessages.WeatherHailDamage;
                default:
                    return "{0} is hurt by the weather!";
            }
        }

        /// <summary>
        /// Processes terrain healing for all active Pokemon on the field.
        /// Only Grassy Terrain heals Pokemon (1/16 Max HP per turn for grounded Pokemon).
        /// </summary>
        /// <remarks>
        /// **Feature**: 2: Combat System
        /// **Sub-Feature**: 2.13: Terrain System
        /// **Documentation**: See `docs/features/2-combat-system/2.13-terrain-system/README.md`
        /// </remarks>
        private List<BattleAction> ProcessTerrainHealing(BattleField field)
        {
            var actions = new List<BattleAction>();

            // Check if there's active terrain that heals
            if (field.TerrainData == null || !field.TerrainData.HealsEachTurn)
                return actions;

            var terrainData = field.TerrainData;

            // Process all active slots
            foreach (var slot in field.GetAllActiveSlots())
            {
                if (!slot.IsActive())
                    continue;

                var pokemon = slot.Pokemon;

                // Check if Pokemon is grounded (terrain only affects grounded Pokemon)
                if (!IsGrounded(pokemon))
                    continue;

                // Calculate healing amount
                int healing = CalculateTerrainHealing(pokemon.MaxHP, terrainData.EndOfTurnHealing);

                // Only heal if Pokemon is not at full HP and healing amount > 0
                if (pokemon.CurrentHP < pokemon.MaxHP && healing > 0)
                {
                    actions.Add(new MessageAction(string.Format(GameMessages.TerrainHealing, pokemon.DisplayName, terrainData.Name)));
                    actions.Add(new HealAction(null, slot, healing)); // null user = system action
                }
            }

            return actions;
        }

        /// <summary>
        /// Checks if a Pokemon is grounded (affected by terrain).
        /// </summary>
        private bool IsGrounded(PokemonInstance pokemon)
        {
            if (pokemon == null)
                return false;

            return TerrainData.IsGrounded(
                pokemon.Species.PrimaryType,
                pokemon.Species.SecondaryType,
                pokemon.Ability?.Id,
                pokemon.HeldItem?.Id);
        }

        /// <summary>
        /// Calculates terrain healing based on Max HP and healing fraction.
        /// </summary>
        private int CalculateTerrainHealing(int maxHP, float healingFraction)
        {
            int healing = (int)(maxHP * healingFraction);
            return System.Math.Max(0, healing); // Minimum 0 (no negative healing)
        }
    }
}

