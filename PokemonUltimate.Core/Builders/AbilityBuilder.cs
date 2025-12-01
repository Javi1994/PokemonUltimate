using System;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Core.Builders
{
    /// <summary>
    /// Fluent builder for creating AbilityData.
    /// </summary>
    public class AbilityBuilder
    {
        private readonly string _id;
        private readonly string _name;
        private string _description = string.Empty;
        private int _generation = 3; // Most abilities from Gen 3
        private AbilityTrigger _triggers = AbilityTrigger.None;
        private AbilityEffect _effect = AbilityEffect.None;
        private Stat? _targetStat;
        private int _statStages;
        private float _effectChance = 1.0f;
        private PersistentStatus? _statusEffect;
        private PokemonType? _affectedType;
        private PokemonType? _secondaryAffectedType;
        private float _multiplier = 1.0f;
        private float _hpThreshold;
        private Weather? _weatherCondition;
        private Terrain? _terrainCondition;

        private AbilityBuilder(string name)
        {
            _name = name ?? throw new ArgumentNullException(nameof(name));
            _id = name.ToLowerInvariant().Replace(" ", "-");
        }

        /// <summary>
        /// Start defining a new ability.
        /// </summary>
        public static AbilityBuilder Define(string name) => new AbilityBuilder(name);

        #region Core Properties

        public AbilityBuilder Description(string description)
        {
            _description = description;
            return this;
        }

        public AbilityBuilder Gen(int generation)
        {
            _generation = generation;
            return this;
        }

        #endregion

        #region Triggers

        public AbilityBuilder OnTrigger(AbilityTrigger trigger)
        {
            _triggers |= trigger;
            return this;
        }

        public AbilityBuilder OnSwitchIn()
        {
            _triggers |= AbilityTrigger.OnSwitchIn;
            return this;
        }

        public AbilityBuilder OnTurnEnd()
        {
            _triggers |= AbilityTrigger.OnTurnEnd;
            return this;
        }

        public AbilityBuilder OnContactReceived()
        {
            _triggers |= AbilityTrigger.OnContactReceived;
            return this;
        }

        public AbilityBuilder OnDamageTaken()
        {
            _triggers |= AbilityTrigger.OnDamageTaken;
            return this;
        }

        public AbilityBuilder OnWouldFaint()
        {
            _triggers |= AbilityTrigger.OnWouldFaint;
            return this;
        }

        public AbilityBuilder OnStatusAttempt()
        {
            _triggers |= AbilityTrigger.OnStatusAttempt;
            return this;
        }

        public AbilityBuilder OnStatChangeAttempt()
        {
            _triggers |= AbilityTrigger.OnStatChangeAttempt;
            return this;
        }

        public AbilityBuilder OnStatChanged()
        {
            _triggers |= AbilityTrigger.OnStatChanged;
            return this;
        }

        public AbilityBuilder Passive()
        {
            _triggers |= AbilityTrigger.Passive;
            return this;
        }

        #endregion

        #region Effects

        public AbilityBuilder Effect(AbilityEffect effect)
        {
            _effect = effect;
            return this;
        }

        /// <summary>
        /// Lowers opponent's stat on switch-in (Intimidate).
        /// </summary>
        public AbilityBuilder LowersOpponentStat(Stat stat, int stages = -1)
        {
            _effect = AbilityEffect.LowerOpponentStat;
            _targetStat = stat;
            _statStages = stages;
            _triggers |= AbilityTrigger.OnSwitchIn;
            return this;
        }

        /// <summary>
        /// Raises own stat at end of turn (Speed Boost).
        /// </summary>
        public AbilityBuilder RaisesStatEachTurn(Stat stat, int stages = 1)
        {
            _effect = AbilityEffect.RaiseOwnStat;
            _targetStat = stat;
            _statStages = stages;
            _triggers |= AbilityTrigger.OnTurnEnd;
            return this;
        }

        /// <summary>
        /// Chance to apply status on contact (Static, Poison Point).
        /// </summary>
        public AbilityBuilder ChanceToStatusOnContact(PersistentStatus status, float chance = 0.30f)
        {
            _effect = AbilityEffect.ChanceToStatusOnContact;
            _statusEffect = status;
            _effectChance = chance;
            _triggers |= AbilityTrigger.OnContactReceived;
            return this;
        }

        /// <summary>
        /// Prevents a specific status (Limber, Immunity).
        /// </summary>
        public AbilityBuilder PreventsStatus(PersistentStatus status)
        {
            _effect = AbilityEffect.PreventStatus;
            _statusEffect = status;
            _triggers |= AbilityTrigger.OnStatusAttempt;
            return this;
        }

        /// <summary>
        /// Immune to type, may gain boost (Flash Fire, Volt Absorb).
        /// </summary>
        public AbilityBuilder TypeImmunity(PokemonType type, AbilityEffect bonus = AbilityEffect.TypeImmunityWithBoost)
        {
            _effect = bonus;
            _affectedType = type;
            _triggers |= AbilityTrigger.OnDamageTaken;
            return this;
        }

        /// <summary>
        /// Heals when hit by type (Water Absorb, Volt Absorb).
        /// </summary>
        public AbilityBuilder HealsFromType(PokemonType type, float healPercent = 0.25f)
        {
            _effect = AbilityEffect.HealFromType;
            _affectedType = type;
            _multiplier = healPercent;
            _triggers |= AbilityTrigger.OnDamageTaken;
            return this;
        }

        /// <summary>
        /// Ground immunity (Levitate).
        /// </summary>
        public AbilityBuilder GroundImmunity()
        {
            _effect = AbilityEffect.GroundImmunity;
            _affectedType = PokemonType.Ground;
            _triggers |= AbilityTrigger.Passive;
            return this;
        }

        /// <summary>
        /// Survives fatal hit at full HP (Sturdy).
        /// </summary>
        public AbilityBuilder SurvivesFatalHit()
        {
            _effect = AbilityEffect.SurviveFatalHit;
            _hpThreshold = 1.0f; // Must be at full HP
            _triggers |= AbilityTrigger.OnWouldFaint;
            return this;
        }

        /// <summary>
        /// Reduces damage from type(s) (Thick Fat).
        /// </summary>
        public AbilityBuilder ReducesDamageFromType(PokemonType type, float multiplier = 0.5f, PokemonType? secondType = null)
        {
            _effect = AbilityEffect.ReduceTypeDamage;
            _affectedType = type;
            _secondaryAffectedType = secondType;
            _multiplier = multiplier;
            _triggers |= AbilityTrigger.OnDamageTaken;
            return this;
        }

        /// <summary>
        /// Boosts type power when HP is low (Blaze, Torrent, Overgrow).
        /// </summary>
        public AbilityBuilder BoostsTypeWhenLowHP(PokemonType type, float threshold = 0.33f, float multiplier = 1.5f)
        {
            _effect = AbilityEffect.TypePowerBoostWhenLowHP;
            _affectedType = type;
            _hpThreshold = threshold;
            _multiplier = multiplier;
            _triggers |= AbilityTrigger.Passive;
            return this;
        }

        /// <summary>
        /// Passive stat multiplier (Huge Power, Pure Power).
        /// </summary>
        public AbilityBuilder PassiveStatMultiplier(Stat stat, float multiplier)
        {
            _effect = AbilityEffect.PassiveStatMultiplier;
            _targetStat = stat;
            _multiplier = multiplier;
            _triggers |= AbilityTrigger.Passive;
            return this;
        }

        /// <summary>
        /// Prevents own stats from being lowered (Clear Body, White Smoke).
        /// </summary>
        public AbilityBuilder PreventsStatLoss()
        {
            _effect = AbilityEffect.PreventStatLoss;
            _triggers |= AbilityTrigger.OnStatChangeAttempt;
            return this;
        }

        /// <summary>
        /// Raises stat when own stat is lowered (Defiant, Competitive).
        /// </summary>
        public AbilityBuilder RaisesStatOnLoss(Stat boostStat, int stages = 2)
        {
            _effect = AbilityEffect.RaiseStatOnLoss;
            _targetStat = boostStat;
            _statStages = stages;
            _triggers |= AbilityTrigger.OnStatChanged;
            return this;
        }

        /// <summary>
        /// Summons weather on switch-in (Drizzle, Drought).
        /// </summary>
        public AbilityBuilder SummonsWeather(Weather weather)
        {
            _effect = AbilityEffect.SummonWeather;
            _weatherCondition = weather;
            _triggers |= AbilityTrigger.OnSwitchIn;
            return this;
        }

        /// <summary>
        /// Doubles speed in weather (Swift Swim, Chlorophyll).
        /// </summary>
        public AbilityBuilder SpeedBoostInWeather(Weather weather, float multiplier = 2.0f)
        {
            _effect = AbilityEffect.SpeedBoostInWeather;
            _weatherCondition = weather;
            _multiplier = multiplier;
            _triggers |= AbilityTrigger.Passive;
            return this;
        }

        /// <summary>
        /// Damages attacker on contact (Rough Skin, Iron Barbs).
        /// </summary>
        public AbilityBuilder DamagesOnContact(float damagePercent = 0.125f)
        {
            _effect = AbilityEffect.DamageOnContact;
            _multiplier = damagePercent;
            _triggers |= AbilityTrigger.OnContactReceived;
            return this;
        }

        /// <summary>
        /// Heals HP percentage each turn in weather (Rain Dish).
        /// </summary>
        public AbilityBuilder HealsPercentInWeather(Weather weather, float healPercent = 0.0625f)
        {
            _effect = AbilityEffect.HealInWeather;
            _weatherCondition = weather;
            _multiplier = healPercent;
            _triggers |= AbilityTrigger.OnTurnEnd;
            return this;
        }

        /// <summary>
        /// Increases STAB bonus (Adaptability - 2.0x instead of 1.5x).
        /// </summary>
        public AbilityBuilder IncreasesStab(float newMultiplier = 2.0f)
        {
            _effect = AbilityEffect.IncreaseStab;
            _multiplier = newMultiplier;
            _triggers |= AbilityTrigger.Passive;
            return this;
        }

        /// <summary>
        /// Activates when receiving a status condition (Synchronize).
        /// </summary>
        public AbilityBuilder OnStatusReceived()
        {
            _triggers |= AbilityTrigger.OnStatusApplied;
            return this;
        }

        /// <summary>
        /// Summons terrain on switch-in (Grassy Surge, Electric Surge).
        /// </summary>
        public AbilityBuilder SummonsTerrain(Terrain terrain)
        {
            _effect = AbilityEffect.SummonTerrain;
            _terrainCondition = terrain;
            _triggers |= AbilityTrigger.OnSwitchIn;
            return this;
        }

        /// <summary>
        /// Benefits from terrain (Grass Pelt, Surge Surfer).
        /// </summary>
        public AbilityBuilder TerrainBoost(Terrain terrain, Stat stat, float multiplier = 1.5f)
        {
            _effect = AbilityEffect.TerrainBoost;
            _terrainCondition = terrain;
            _targetStat = stat;
            _multiplier = multiplier;
            _triggers |= AbilityTrigger.Passive;
            return this;
        }

        #endregion

        #region Build

        public AbilityData Build()
        {
            return new AbilityData(
                _id,
                _name,
                _description,
                _generation,
                _triggers,
                _effect,
                _targetStat,
                _statStages,
                _effectChance,
                _statusEffect,
                _affectedType,
                _secondaryAffectedType,
                _multiplier,
                _hpThreshold,
                _weatherCondition,
                _terrainCondition);
        }

        #endregion
    }

    /// <summary>
    /// Alias for AbilityBuilder for cleaner syntax in catalogs.
    /// </summary>
    public static class Ability
    {
        public static AbilityBuilder Define(string name) => AbilityBuilder.Define(name);
    }
}

