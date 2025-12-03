using System;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Core.Blueprints
{
    /// <summary>
    /// Immutable data defining an entry hazard's behavior.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.6: Field Conditions Data
    /// **Documentation**: See `docs/features/1-game-data/1.6-field-conditions-data/README.md`
    /// </remarks>
    public sealed class HazardData
    {
        #region Identity

        /// <summary>
        /// Internal identifier for the hazard.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Display name of the hazard.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Description of what the hazard does.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// The hazard type this data represents.
        /// </summary>
        public HazardType Type { get; }

        #endregion

        #region Layers

        /// <summary>
        /// Maximum number of layers (1 for Stealth Rock, 3 for Spikes, 2 for Toxic Spikes).
        /// </summary>
        public int MaxLayers { get; }

        /// <summary>
        /// Whether this hazard supports multiple layers.
        /// </summary>
        public bool HasLayers => MaxLayers > 1;

        #endregion

        #region Targeting

        /// <summary>
        /// Whether this hazard affects Flying types (only Stealth Rock does).
        /// </summary>
        public bool AffectsFlying { get; }

        /// <summary>
        /// Whether this hazard affects Pokemon with Levitate.
        /// </summary>
        public bool AffectsLevitate { get; }

        /// <summary>
        /// Whether grounded check is required (all except Stealth Rock).
        /// </summary>
        public bool RequiresGrounded => !AffectsFlying;

        #endregion

        #region Damage

        /// <summary>
        /// Type used for effectiveness calculation (Rock for Stealth Rock).
        /// Null for hazards that don't use type effectiveness.
        /// </summary>
        public PokemonType? DamageType { get; }

        /// <summary>
        /// Base damage percentage for each layer (as fraction of max HP).
        /// For Stealth Rock: [0.125] (then multiplied by effectiveness).
        /// For Spikes: [0.125, 0.167, 0.25].
        /// </summary>
        public float[] DamageByLayer { get; }

        /// <summary>
        /// Whether damage is modified by type effectiveness.
        /// </summary>
        public bool UseTypeEffectiveness => DamageType.HasValue;

        #endregion

        #region Status

        /// <summary>
        /// Status applied by each layer (for Toxic Spikes).
        /// </summary>
        public PersistentStatus[] StatusByLayer { get; }

        /// <summary>
        /// Whether this hazard applies status instead of damage.
        /// </summary>
        public bool AppliesStatus => StatusByLayer.Length > 0;

        /// <summary>
        /// Whether Poison types absorb this hazard on entry.
        /// </summary>
        public bool AbsorbedByPoisonTypes { get; }

        #endregion

        #region Stat Changes

        /// <summary>
        /// Stat lowered by this hazard (for Sticky Web).
        /// </summary>
        public Stat? StatToLower { get; }

        /// <summary>
        /// Number of stages to lower.
        /// </summary>
        public int StatStages { get; }

        /// <summary>
        /// Whether this hazard lowers stats instead of dealing damage.
        /// </summary>
        public bool LowersStat => StatToLower.HasValue;

        #endregion

        #region Removal

        /// <summary>
        /// Moves that can remove this hazard.
        /// </summary>
        public string[] RemovedByMoves { get; }

        #endregion

        #region Constructor

        internal HazardData(
            string id,
            string name,
            string description,
            HazardType type,
            int maxLayers,
            bool affectsFlying,
            bool affectsLevitate,
            PokemonType? damageType,
            float[] damageByLayer,
            PersistentStatus[] statusByLayer,
            bool absorbedByPoisonTypes,
            Stat? statToLower,
            int statStages,
            string[] removedByMoves)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description ?? string.Empty;
            Type = type;
            MaxLayers = maxLayers;
            AffectsFlying = affectsFlying;
            AffectsLevitate = affectsLevitate;
            DamageType = damageType;
            DamageByLayer = damageByLayer ?? Array.Empty<float>();
            StatusByLayer = statusByLayer ?? Array.Empty<PersistentStatus>();
            AbsorbedByPoisonTypes = absorbedByPoisonTypes;
            StatToLower = statToLower;
            StatStages = statStages;
            RemovedByMoves = removedByMoves ?? new[] { "Rapid Spin", "Defog" };
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Gets the damage for a specific number of layers.
        /// </summary>
        public float GetDamage(int layers)
        {
            if (DamageByLayer.Length == 0) return 0f;
            int index = Math.Min(layers, DamageByLayer.Length) - 1;
            return index >= 0 ? DamageByLayer[index] : 0f;
        }

        /// <summary>
        /// Gets the status for a specific number of layers.
        /// </summary>
        public PersistentStatus GetStatus(int layers)
        {
            if (StatusByLayer.Length == 0) return PersistentStatus.None;
            int index = Math.Min(layers, StatusByLayer.Length) - 1;
            return index >= 0 ? StatusByLayer[index] : PersistentStatus.None;
        }

        /// <summary>
        /// Checks if a Pokemon is affected by this hazard.
        /// </summary>
        public bool AffectsPokemon(PokemonType primaryType, PokemonType? secondaryType, string abilityName)
        {
            // Check Flying immunity
            if (!AffectsFlying)
            {
                if (primaryType == PokemonType.Flying || secondaryType == PokemonType.Flying)
                    return false;
            }

            // Check Levitate immunity
            if (!AffectsLevitate)
            {
                if (!string.IsNullOrEmpty(abilityName) && 
                    abilityName.Equals("Levitate", StringComparison.OrdinalIgnoreCase))
                    return false;
            }

            // Check Magic Guard (immune to indirect damage)
            if (!string.IsNullOrEmpty(abilityName) && 
                abilityName.Equals("Magic Guard", StringComparison.OrdinalIgnoreCase) &&
                !LowersStat)
                return false;

            return true;
        }

        /// <summary>
        /// Checks if Poison type absorbs this hazard.
        /// </summary>
        public bool IsPoisonAbsorber(PokemonType primaryType, PokemonType? secondaryType)
        {
            if (!AbsorbedByPoisonTypes) return false;
            return primaryType == PokemonType.Poison || secondaryType == PokemonType.Poison;
        }

        public override string ToString() => Name;

        #endregion
    }
}

