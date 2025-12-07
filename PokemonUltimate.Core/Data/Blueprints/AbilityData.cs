using System;
using PokemonUltimate.Core.Data.Enums;


namespace PokemonUltimate.Core.Data.Blueprints
{
    /// <summary>
    /// Immutable data defining a Pokemon ability.
    /// Contains all the information needed to determine when and how an ability activates.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.3: Ability Data
    /// **Documentation**: See `docs/features/1-game-data/1.3-ability-data/README.md`
    /// </remarks>
    public sealed class AbilityData
    {
        #region Core Properties

        /// <summary>
        /// Internal identifier for the ability.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Display name of the ability.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Description of what the ability does.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// The generation this ability was introduced.
        /// </summary>
        public int Generation { get; }

        #endregion

        #region Trigger Configuration

        /// <summary>
        /// When this ability activates (flags can be combined).
        /// </summary>
        public AbilityTrigger Triggers { get; }

        /// <summary>
        /// The type of effect this ability has.
        /// </summary>
        public AbilityEffect Effect { get; }

        #endregion

        #region Effect Parameters

        /// <summary>
        /// Target stat for stat-modifying abilities.
        /// </summary>
        public Stat? TargetStat { get; }

        /// <summary>
        /// Number of stat stages to modify (-6 to +6).
        /// </summary>
        public int StatStages { get; }

        /// <summary>
        /// Probability of effect activating (0.0 to 1.0).
        /// </summary>
        public float EffectChance { get; }

        /// <summary>
        /// Status to apply/prevent for status-related abilities.
        /// </summary>
        public PersistentStatus? StatusEffect { get; }

        /// <summary>
        /// Pokemon type this ability interacts with.
        /// </summary>
        public PokemonType? AffectedType { get; }

        /// <summary>
        /// Secondary affected type (for abilities like Thick Fat).
        /// </summary>
        public PokemonType? SecondaryAffectedType { get; }

        /// <summary>
        /// Multiplier for damage/stat modifications.
        /// </summary>
        public float Multiplier { get; }

        /// <summary>
        /// HP threshold as percentage (0.0 to 1.0) for HP-based triggers.
        /// </summary>
        public float HPThreshold { get; }

        /// <summary>
        /// Weather type for weather-related abilities.
        /// </summary>
        public Weather? WeatherCondition { get; }

        /// <summary>
        /// Terrain type for terrain-related abilities (Grassy Surge, etc.).
        /// </summary>
        public Terrain? TerrainCondition { get; }

        #endregion

        #region Constructor

        internal AbilityData(
            string id,
            string name,
            string description,
            int generation,
            AbilityTrigger triggers,
            AbilityEffect effect,
            Stat? targetStat,
            int statStages,
            float effectChance,
            PersistentStatus? statusEffect,
            PokemonType? affectedType,
            PokemonType? secondaryAffectedType,
            float multiplier,
            float hpThreshold,
            Weather? weatherCondition,
            Terrain? terrainCondition)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description ?? string.Empty;
            Generation = generation;
            Triggers = triggers;
            Effect = effect;
            TargetStat = targetStat;
            StatStages = statStages;
            EffectChance = effectChance;
            StatusEffect = statusEffect;
            AffectedType = affectedType;
            SecondaryAffectedType = secondaryAffectedType;
            Multiplier = multiplier;
            HPThreshold = hpThreshold;
            WeatherCondition = weatherCondition;
            TerrainCondition = terrainCondition;
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Checks if this ability listens to a specific trigger.
        /// </summary>
        public bool ListensTo(AbilityTrigger trigger)
        {
            return (Triggers & trigger) != 0;
        }

        /// <summary>
        /// Checks if this ability provides passive stat modifications.
        /// </summary>
        public bool IsPassive => (Triggers & AbilityTrigger.Passive) != 0;

        public override string ToString() => Name;

        #endregion
    }
}

