using System;
using System.Collections.Generic;
using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Core.Data.Enums;

namespace PokemonUltimate.Core.Infrastructure.Builders
{
    /// <summary>
    /// Fluent builder for creating WeatherData.
    /// </summary>
    /// <remarks>
    /// **Feature**: 3: Content Expansion
    /// **Sub-Feature**: 3.9: Builders
    /// **Documentation**: See `docs/features/3-content-expansion/3.9-builders/README.md`
    /// </remarks>
    public class WeatherBuilder
    {
        private readonly string _id;
        private readonly string _name;
        private string _description = string.Empty;
        private Weather _weather = Weather.None;
        private int _defaultDuration = 5;
        private bool _canBeOverwritten = true;
        private float _endOfTurnDamage;
        private List<PokemonType> _damageImmuneTypes = new List<PokemonType>();
        private List<PokemonType> _boostedTypes = new List<PokemonType>();
        private List<PokemonType> _weakenedTypes = new List<PokemonType>();
        private List<PokemonType> _nullifiedTypes = new List<PokemonType>();
        private float _boostMultiplier = 1.5f;
        private float _weakenMultiplier = 0.5f;
        private Stat? _boostedStat;
        private float _statBoostMultiplier = 1.5f;
        private List<PokemonType> _statBoostTypes = new List<PokemonType>();
        private List<string> _perfectAccuracyMoves = new List<string>();
        private List<string> _instantChargeMoves = new List<string>();
        private List<string> _damageImmunityAbilities = new List<string>();
        private List<string> _speedBoostAbilities = new List<string>();
        private List<string> _healingAbilities = new List<string>();

        private WeatherBuilder(string name)
        {
            _name = name ?? throw new ArgumentNullException(nameof(name));
            _id = name.ToLowerInvariant().Replace(" ", "-");
        }

        /// <summary>
        /// Start defining a new weather condition.
        /// </summary>
        public static WeatherBuilder Define(string name) => new WeatherBuilder(name);

        #region Identity

        public WeatherBuilder Description(string description)
        {
            _description = description;
            return this;
        }

        /// <summary>
        /// Set the weather enum value.
        /// </summary>
        public WeatherBuilder Type(Weather weather)
        {
            _weather = weather;
            return this;
        }

        #endregion

        #region Duration

        /// <summary>
        /// Set default duration (5 turns is standard).
        /// </summary>
        public WeatherBuilder Duration(int turns)
        {
            _defaultDuration = turns;
            return this;
        }

        /// <summary>
        /// Weather lasts indefinitely (until replaced).
        /// </summary>
        public WeatherBuilder Indefinite()
        {
            _defaultDuration = 0;
            return this;
        }

        /// <summary>
        /// Mark as primal weather (cannot be overwritten).
        /// </summary>
        public WeatherBuilder Primal()
        {
            _canBeOverwritten = false;
            return this;
        }

        #endregion

        #region End of Turn Damage

        /// <summary>
        /// Deals damage at end of turn (1/16 = 0.0625).
        /// </summary>
        public WeatherBuilder DealsDamagePerTurn(float fraction, params PokemonType[] immuneTypes)
        {
            _endOfTurnDamage = fraction;
            _damageImmuneTypes.AddRange(immuneTypes);
            return this;
        }

        /// <summary>
        /// Add types that are immune to weather damage.
        /// </summary>
        public WeatherBuilder DamageImmune(params PokemonType[] types)
        {
            _damageImmuneTypes.AddRange(types);
            return this;
        }

        #endregion

        #region Type Power Modifiers

        /// <summary>
        /// Boost certain type's moves (default 1.5x).
        /// </summary>
        public WeatherBuilder Boosts(params PokemonType[] types)
        {
            _boostedTypes.AddRange(types);
            return this;
        }

        /// <summary>
        /// Weaken certain type's moves (default 0.5x).
        /// </summary>
        public WeatherBuilder Weakens(params PokemonType[] types)
        {
            _weakenedTypes.AddRange(types);
            return this;
        }

        /// <summary>
        /// Nullify certain type's moves (0x, primal weathers).
        /// </summary>
        public WeatherBuilder Nullifies(params PokemonType[] types)
        {
            _nullifiedTypes.AddRange(types);
            return this;
        }

        /// <summary>
        /// Set custom boost multiplier (default 1.5).
        /// </summary>
        public WeatherBuilder BoostMultiplier(float multiplier)
        {
            _boostMultiplier = multiplier;
            return this;
        }

        /// <summary>
        /// Set custom weaken multiplier (default 0.5).
        /// </summary>
        public WeatherBuilder WeakenMultiplier(float multiplier)
        {
            _weakenMultiplier = multiplier;
            return this;
        }

        #endregion

        #region Stat Modifiers

        /// <summary>
        /// Boost a stat for certain types (Rock SpDef in Sandstorm).
        /// </summary>
        public WeatherBuilder BoostsStat(Stat stat, float multiplier, params PokemonType[] types)
        {
            _boostedStat = stat;
            _statBoostMultiplier = multiplier;
            _statBoostTypes.AddRange(types);
            return this;
        }

        #endregion

        #region Move Effects

        /// <summary>
        /// Moves that have 100% accuracy in this weather.
        /// </summary>
        public WeatherBuilder PerfectAccuracy(params string[] moveNames)
        {
            _perfectAccuracyMoves.AddRange(moveNames);
            return this;
        }

        /// <summary>
        /// Moves that charge instantly (no turn skip).
        /// </summary>
        public WeatherBuilder InstantCharge(params string[] moveNames)
        {
            _instantChargeMoves.AddRange(moveNames);
            return this;
        }

        #endregion

        #region Ability Interactions

        /// <summary>
        /// Abilities that grant immunity to damage.
        /// </summary>
        public WeatherBuilder AbilitiesImmuneToDamage(params string[] abilityNames)
        {
            _damageImmunityAbilities.AddRange(abilityNames);
            return this;
        }

        /// <summary>
        /// Abilities that double Speed in this weather.
        /// </summary>
        public WeatherBuilder AbilitiesDoubleSpeed(params string[] abilityNames)
        {
            _speedBoostAbilities.AddRange(abilityNames);
            return this;
        }

        /// <summary>
        /// Abilities that heal HP in this weather.
        /// </summary>
        public WeatherBuilder AbilitiesHeal(params string[] abilityNames)
        {
            _healingAbilities.AddRange(abilityNames);
            return this;
        }

        #endregion

        #region Build

        public WeatherData Build()
        {
            return new WeatherData(
                _id,
                _name,
                _description,
                _weather,
                _defaultDuration,
                _canBeOverwritten,
                _endOfTurnDamage,
                _damageImmuneTypes.ToArray(),
                _boostedTypes.ToArray(),
                _weakenedTypes.ToArray(),
                _nullifiedTypes.ToArray(),
                _boostMultiplier,
                _weakenMultiplier,
                _boostedStat,
                _statBoostMultiplier,
                _statBoostTypes.ToArray(),
                _perfectAccuracyMoves.ToArray(),
                _instantChargeMoves.ToArray(),
                _damageImmunityAbilities.ToArray(),
                _speedBoostAbilities.ToArray(),
                _healingAbilities.ToArray());
        }

        #endregion
    }

    /// <summary>
    /// Alias for WeatherBuilder for cleaner syntax.
    /// </summary>
    public static class WeatherEffect
    {
        public static WeatherBuilder Define(string name) => WeatherBuilder.Define(name);
    }
}

