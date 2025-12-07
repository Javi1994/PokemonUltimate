using System;
using PokemonUltimate.Core.Data.Enums;


namespace PokemonUltimate.Core.Data.Blueprints
{
    /// <summary>
    /// Immutable data defining a side condition's behavior.
    /// Side conditions affect all Pokemon on one side of the field.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.9: Side Condition Data
    /// **Documentation**: See `docs/features/1-game-data/1.9-side-condition-data/README.md`
    /// </remarks>
    public sealed class SideConditionData
    {
        #region Identity

        /// <summary>
        /// Internal identifier for the condition.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Display name of the condition.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Description of what the condition does.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// The condition type this data represents.
        /// </summary>
        public SideCondition Type { get; }

        #endregion

        #region Duration

        /// <summary>
        /// Default duration in turns.
        /// </summary>
        public int DefaultDuration { get; }

        /// <summary>
        /// Extended duration when holder has extending item (Light Clay).
        /// </summary>
        public int ExtendedDuration { get; }

        /// <summary>
        /// Item that extends duration.
        /// </summary>
        public string ExtendingItem { get; }

        /// <summary>
        /// Whether this is a single-turn protection (Wide Guard, Quick Guard).
        /// </summary>
        public bool IsSingleTurn => DefaultDuration == 1;

        #endregion

        #region Damage Reduction

        /// <summary>
        /// Move category this condition reduces damage from (Physical, Special, or null for both).
        /// </summary>
        public MoveCategory? ReducesDamageFrom { get; }

        /// <summary>
        /// Damage multiplier in Singles battles (0.5 = 50% reduction).
        /// </summary>
        public float DamageMultiplierSingles { get; }

        /// <summary>
        /// Damage multiplier in Doubles/Triples battles (0.66 = 33% reduction).
        /// </summary>
        public float DamageMultiplierDoubles { get; }

        /// <summary>
        /// Whether this condition reduces damage.
        /// </summary>
        public bool ReducesDamage => ReducesDamageFrom.HasValue ||
            (DamageMultiplierSingles < 1.0f && DamageMultiplierSingles > 0f);

        #endregion

        #region Speed Modification

        /// <summary>
        /// Speed multiplier for the side (2.0 for Tailwind).
        /// </summary>
        public float SpeedMultiplier { get; }

        /// <summary>
        /// Whether this condition modifies speed.
        /// </summary>
        public bool ModifiesSpeed => Math.Abs(SpeedMultiplier - 1.0f) > 0.001f;

        #endregion

        #region Status Prevention

        /// <summary>
        /// Whether this condition prevents status conditions (Safeguard).
        /// </summary>
        public bool PreventsStatus { get; }

        /// <summary>
        /// Whether this condition prevents stat reductions (Mist).
        /// </summary>
        public bool PreventsStatReduction { get; }

        /// <summary>
        /// Whether this condition prevents critical hits (Lucky Chant).
        /// </summary>
        public bool PreventsCriticalHits { get; }

        #endregion

        #region Move Protection

        /// <summary>
        /// Whether this condition blocks spread moves (Wide Guard).
        /// </summary>
        public bool BlocksSpreadMoves { get; }

        /// <summary>
        /// Whether this condition blocks priority moves (Quick Guard).
        /// </summary>
        public bool BlocksPriorityMoves { get; }

        /// <summary>
        /// Whether this condition blocks all damaging moves (Mat Block).
        /// </summary>
        public bool BlocksDamagingMoves { get; }

        #endregion

        #region Requirements

        /// <summary>
        /// Weather required to set this condition (Hail/Snow for Aurora Veil).
        /// </summary>
        public Weather? RequiredWeather { get; }

        /// <summary>
        /// Secondary weather that also allows this condition.
        /// </summary>
        public Weather? AlternateWeather { get; }

        /// <summary>
        /// Whether this can only be used on first turn out (Mat Block).
        /// </summary>
        public bool FirstTurnOnly { get; }

        #endregion

        #region Removal

        /// <summary>
        /// Moves that remove this condition from both sides.
        /// </summary>
        public string[] RemovedByMoves { get; }

        #endregion

        #region Constructor

        internal SideConditionData(
            string id,
            string name,
            string description,
            SideCondition type,
            int defaultDuration,
            int extendedDuration,
            string extendingItem,
            MoveCategory? reducesDamageFrom,
            float damageMultiplierSingles,
            float damageMultiplierDoubles,
            float speedMultiplier,
            bool preventsStatus,
            bool preventsStatReduction,
            bool preventsCriticalHits,
            bool blocksSpreadMoves,
            bool blocksPriorityMoves,
            bool blocksDamagingMoves,
            Weather? requiredWeather,
            Weather? alternateWeather,
            bool firstTurnOnly,
            string[] removedByMoves)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description ?? string.Empty;
            Type = type;
            DefaultDuration = defaultDuration;
            ExtendedDuration = extendedDuration;
            ExtendingItem = extendingItem ?? string.Empty;
            ReducesDamageFrom = reducesDamageFrom;
            DamageMultiplierSingles = damageMultiplierSingles;
            DamageMultiplierDoubles = damageMultiplierDoubles;
            SpeedMultiplier = speedMultiplier;
            PreventsStatus = preventsStatus;
            PreventsStatReduction = preventsStatReduction;
            PreventsCriticalHits = preventsCriticalHits;
            BlocksSpreadMoves = blocksSpreadMoves;
            BlocksPriorityMoves = blocksPriorityMoves;
            BlocksDamagingMoves = blocksDamagingMoves;
            RequiredWeather = requiredWeather;
            AlternateWeather = alternateWeather;
            FirstTurnOnly = firstTurnOnly;
            RemovedByMoves = removedByMoves ?? Array.Empty<string>();
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Gets the damage multiplier based on battle format.
        /// </summary>
        public float GetDamageMultiplier(bool isDoubles)
        {
            return isDoubles ? DamageMultiplierDoubles : DamageMultiplierSingles;
        }

        /// <summary>
        /// Checks if this condition can be set in the given weather.
        /// </summary>
        public bool CanBeSetInWeather(Weather currentWeather)
        {
            if (!RequiredWeather.HasValue) return true;
            if (currentWeather == RequiredWeather.Value) return true;
            if (AlternateWeather.HasValue && currentWeather == AlternateWeather.Value) return true;
            return false;
        }

        /// <summary>
        /// Gets the duration considering item extension.
        /// </summary>
        public int GetDuration(string heldItem)
        {
            if (!string.IsNullOrEmpty(ExtendingItem) &&
                !string.IsNullOrEmpty(heldItem) &&
                heldItem.Equals(ExtendingItem, StringComparison.OrdinalIgnoreCase))
            {
                return ExtendedDuration;
            }
            return DefaultDuration;
        }

        /// <summary>
        /// Checks if a move type is reduced by this condition.
        /// </summary>
        public bool ReducesMoveCategory(MoveCategory category)
        {
            if (!ReducesDamageFrom.HasValue) return DamageMultiplierSingles < 1.0f;
            return ReducesDamageFrom.Value == category;
        }

        public override string ToString() => Name;

        #endregion
    }
}

