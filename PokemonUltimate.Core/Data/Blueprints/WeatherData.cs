using System;
using PokemonUltimate.Core.Data.Enums;


namespace PokemonUltimate.Core.Data.Blueprints
{
    /// <summary>
    /// Immutable data defining a weather condition's effects.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.6: Weather Data
    /// **Documentation**: See `docs/features/1-game-data/1.6-weather-data/README.md`
    /// </remarks>
    public sealed class WeatherData
    {
        #region Identity

        /// <summary>
        /// Internal identifier for the weather.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Display name of the weather.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Description of the weather effect.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// The weather enum this data represents.
        /// </summary>
        public Weather Weather { get; }

        #endregion

        #region Duration

        /// <summary>
        /// Default duration in turns (0 = indefinite, 5 = standard).
        /// </summary>
        public int DefaultDuration { get; }

        /// <summary>
        /// Whether this weather can be overwritten by other weather.
        /// Primal weathers (Heavy Rain, etc.) cannot be overwritten.
        /// </summary>
        public bool CanBeOverwritten { get; }

        /// <summary>
        /// Whether this is a primal/primordial weather.
        /// </summary>
        public bool IsPrimal => !CanBeOverwritten;

        #endregion

        #region End of Turn Damage

        /// <summary>
        /// HP damage dealt at end of turn as fraction of max HP (1/16 = 0.0625).
        /// </summary>
        public float EndOfTurnDamage { get; }

        /// <summary>
        /// Types immune to the end-of-turn damage.
        /// </summary>
        public PokemonType[] DamageImmuneTypes { get; }

        /// <summary>
        /// Whether this weather deals damage at end of turn.
        /// </summary>
        public bool DealsDamage => EndOfTurnDamage > 0;

        #endregion

        #region Type Power Modifiers

        /// <summary>
        /// Types whose moves are boosted (1.5x power).
        /// </summary>
        public PokemonType[] BoostedTypes { get; }

        /// <summary>
        /// Types whose moves are weakened (0.5x power).
        /// </summary>
        public PokemonType[] WeakenedTypes { get; }

        /// <summary>
        /// Types whose moves are completely nullified (0x power).
        /// Only for primal weathers.
        /// </summary>
        public PokemonType[] NullifiedTypes { get; }

        /// <summary>
        /// The boost multiplier for boosted types.
        /// </summary>
        public float BoostMultiplier { get; }

        /// <summary>
        /// The weaken multiplier for weakened types.
        /// </summary>
        public float WeakenMultiplier { get; }

        #endregion

        #region Stat Modifiers

        /// <summary>
        /// Stat that gets boosted for certain types (SpDef for Rock in Sandstorm).
        /// </summary>
        public Stat? BoostedStat { get; }

        /// <summary>
        /// The multiplier for the stat boost.
        /// </summary>
        public float StatBoostMultiplier { get; }

        /// <summary>
        /// Types that get the stat boost.
        /// </summary>
        public PokemonType[] StatBoostTypes { get; }

        #endregion

        #region Move Effects

        /// <summary>
        /// Move names that get 100% accuracy in this weather.
        /// </summary>
        public string[] PerfectAccuracyMoves { get; }

        /// <summary>
        /// Move names that skip charge turn in this weather.
        /// </summary>
        public string[] InstantChargeMoves { get; }

        #endregion

        #region Ability Interactions

        /// <summary>
        /// Abilities that grant immunity to this weather's damage.
        /// </summary>
        public string[] DamageImmunityAbilities { get; }

        /// <summary>
        /// Abilities that double Speed in this weather.
        /// </summary>
        public string[] SpeedBoostAbilities { get; }

        /// <summary>
        /// Abilities that heal HP in this weather.
        /// </summary>
        public string[] HealingAbilities { get; }

        #endregion

        #region Constructor

        internal WeatherData(
            string id,
            string name,
            string description,
            Weather weather,
            int defaultDuration,
            bool canBeOverwritten,
            float endOfTurnDamage,
            PokemonType[] damageImmuneTypes,
            PokemonType[] boostedTypes,
            PokemonType[] weakenedTypes,
            PokemonType[] nullifiedTypes,
            float boostMultiplier,
            float weakenMultiplier,
            Stat? boostedStat,
            float statBoostMultiplier,
            PokemonType[] statBoostTypes,
            string[] perfectAccuracyMoves,
            string[] instantChargeMoves,
            string[] damageImmunityAbilities,
            string[] speedBoostAbilities,
            string[] healingAbilities)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description ?? string.Empty;
            Weather = weather;
            DefaultDuration = defaultDuration;
            CanBeOverwritten = canBeOverwritten;
            EndOfTurnDamage = endOfTurnDamage;
            DamageImmuneTypes = damageImmuneTypes ?? Array.Empty<PokemonType>();
            BoostedTypes = boostedTypes ?? Array.Empty<PokemonType>();
            WeakenedTypes = weakenedTypes ?? Array.Empty<PokemonType>();
            NullifiedTypes = nullifiedTypes ?? Array.Empty<PokemonType>();
            BoostMultiplier = boostMultiplier;
            WeakenMultiplier = weakenMultiplier;
            BoostedStat = boostedStat;
            StatBoostMultiplier = statBoostMultiplier;
            StatBoostTypes = statBoostTypes ?? Array.Empty<PokemonType>();
            PerfectAccuracyMoves = perfectAccuracyMoves ?? Array.Empty<string>();
            InstantChargeMoves = instantChargeMoves ?? Array.Empty<string>();
            DamageImmunityAbilities = damageImmunityAbilities ?? Array.Empty<string>();
            SpeedBoostAbilities = speedBoostAbilities ?? Array.Empty<string>();
            HealingAbilities = healingAbilities ?? Array.Empty<string>();
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Checks if a type is immune to weather damage.
        /// </summary>
        public bool IsTypeImmuneToDamage(PokemonType type)
        {
            foreach (var immuneType in DamageImmuneTypes)
            {
                if (immuneType == type) return true;
            }
            return false;
        }

        /// <summary>
        /// Checks if a type's moves are boosted.
        /// </summary>
        public bool IsTypeBoosted(PokemonType type)
        {
            foreach (var boosted in BoostedTypes)
            {
                if (boosted == type) return true;
            }
            return false;
        }

        /// <summary>
        /// Checks if a type's moves are weakened.
        /// </summary>
        public bool IsTypeWeakened(PokemonType type)
        {
            foreach (var weakened in WeakenedTypes)
            {
                if (weakened == type) return true;
            }
            return false;
        }

        /// <summary>
        /// Checks if a type's moves are completely nullified.
        /// </summary>
        public bool IsTypeNullified(PokemonType type)
        {
            foreach (var nullified in NullifiedTypes)
            {
                if (nullified == type) return true;
            }
            return false;
        }

        /// <summary>
        /// Gets the power multiplier for a move type.
        /// </summary>
        public float GetTypePowerMultiplier(PokemonType moveType)
        {
            if (IsTypeNullified(moveType)) return 0f;
            if (IsTypeBoosted(moveType)) return BoostMultiplier;
            if (IsTypeWeakened(moveType)) return WeakenMultiplier;
            return 1f;
        }

        /// <summary>
        /// Checks if a type gets a stat boost in this weather.
        /// </summary>
        public bool TypeGetsStatBoost(PokemonType type)
        {
            if (BoostedStat == null) return false;
            foreach (var boostType in StatBoostTypes)
            {
                if (boostType == type) return true;
            }
            return false;
        }

        /// <summary>
        /// Checks if a move has perfect accuracy in this weather.
        /// </summary>
        public bool HasPerfectAccuracy(string moveName)
        {
            foreach (var move in PerfectAccuracyMoves)
            {
                if (move.Equals(moveName, StringComparison.OrdinalIgnoreCase)) return true;
            }
            return false;
        }

        /// <summary>
        /// Checks if a move charges instantly in this weather.
        /// </summary>
        public bool ChargesInstantly(string moveName)
        {
            foreach (var move in InstantChargeMoves)
            {
                if (move.Equals(moveName, StringComparison.OrdinalIgnoreCase)) return true;
            }
            return false;
        }

        public override string ToString() => Name;

        #endregion
    }
}

