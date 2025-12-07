using System;
using PokemonUltimate.Core.Data.Enums;


namespace PokemonUltimate.Core.Data.Blueprints
{
    /// <summary>
    /// Immutable data defining a terrain condition's effects.
    /// Terrains only affect "grounded" Pokemon (not Flying, Levitate, or Air Balloon).
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.7: Terrain Data
    /// **Documentation**: See `docs/features/1-game-data/1.7-terrain-data/README.md`
    /// </remarks>
    public sealed class TerrainData
    {
        #region Identity

        /// <summary>
        /// Internal identifier for the terrain.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Display name of the terrain.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Description of the terrain effect.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// The terrain enum this data represents.
        /// </summary>
        public Terrain Terrain { get; }

        #endregion

        #region Duration

        /// <summary>
        /// Default duration in turns (5 standard, 8 with Terrain Extender).
        /// </summary>
        public int DefaultDuration { get; }

        #endregion

        #region Type Power Modifiers

        /// <summary>
        /// Type whose moves are boosted (Grass for Grassy, etc.).
        /// </summary>
        public PokemonType? BoostedType { get; }

        /// <summary>
        /// The boost multiplier for the boosted type (Gen 7+ = 1.3x).
        /// </summary>
        public float BoostMultiplier { get; }

        /// <summary>
        /// Type whose damage is reduced (Dragon for Misty).
        /// </summary>
        public PokemonType? ReducedDamageType { get; }

        /// <summary>
        /// The damage reduction multiplier (0.5 for Misty vs Dragon).
        /// </summary>
        public float DamageReductionMultiplier { get; }

        #endregion

        #region End of Turn Effects

        /// <summary>
        /// HP healed at end of turn as fraction of max HP (1/16 = 0.0625).
        /// </summary>
        public float EndOfTurnHealing { get; }

        /// <summary>
        /// Whether this terrain heals at end of turn.
        /// </summary>
        public bool HealsEachTurn => EndOfTurnHealing > 0;

        #endregion

        #region Status Prevention

        /// <summary>
        /// Status conditions prevented by this terrain.
        /// </summary>
        public PersistentStatus[] PreventedStatuses { get; }

        /// <summary>
        /// Whether this terrain prevents any status conditions.
        /// </summary>
        public bool PreventsStatuses => PreventedStatuses.Length > 0;

        #endregion

        #region Move Modifications

        /// <summary>
        /// Whether this terrain blocks priority moves against grounded targets.
        /// </summary>
        public bool BlocksPriorityMoves { get; }

        /// <summary>
        /// Move names whose damage is halved in this terrain.
        /// </summary>
        public string[] HalvedDamageMoves { get; }

        /// <summary>
        /// Move that this terrain changes Nature Power into.
        /// </summary>
        public string NaturePowerMove { get; }

        /// <summary>
        /// Move that this terrain changes Camouflage type into.
        /// </summary>
        public PokemonType? CamouflageType { get; }

        /// <summary>
        /// Move that this terrain changes Secret Power effect into.
        /// </summary>
        public string SecretPowerEffect { get; }

        #endregion

        #region Ability Interactions

        /// <summary>
        /// Abilities that set this terrain on switch-in.
        /// </summary>
        public string[] SetterAbilities { get; }

        /// <summary>
        /// Abilities that benefit from this terrain (double duration, etc.).
        /// </summary>
        public string[] BenefitingAbilities { get; }

        #endregion

        #region Constructor

        internal TerrainData(
            string id,
            string name,
            string description,
            Terrain terrain,
            int defaultDuration,
            PokemonType? boostedType,
            float boostMultiplier,
            PokemonType? reducedDamageType,
            float damageReductionMultiplier,
            float endOfTurnHealing,
            PersistentStatus[] preventedStatuses,
            bool blocksPriorityMoves,
            string[] halvedDamageMoves,
            string naturePowerMove,
            PokemonType? camouflageType,
            string secretPowerEffect,
            string[] setterAbilities,
            string[] benefitingAbilities)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description ?? string.Empty;
            Terrain = terrain;
            DefaultDuration = defaultDuration;
            BoostedType = boostedType;
            BoostMultiplier = boostMultiplier;
            ReducedDamageType = reducedDamageType;
            DamageReductionMultiplier = damageReductionMultiplier;
            EndOfTurnHealing = endOfTurnHealing;
            PreventedStatuses = preventedStatuses ?? Array.Empty<PersistentStatus>();
            BlocksPriorityMoves = blocksPriorityMoves;
            HalvedDamageMoves = halvedDamageMoves ?? Array.Empty<string>();
            NaturePowerMove = naturePowerMove ?? string.Empty;
            CamouflageType = camouflageType;
            SecretPowerEffect = secretPowerEffect ?? string.Empty;
            SetterAbilities = setterAbilities ?? Array.Empty<string>();
            BenefitingAbilities = benefitingAbilities ?? Array.Empty<string>();
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Gets the power multiplier for a move type.
        /// </summary>
        public float GetTypePowerMultiplier(PokemonType moveType)
        {
            if (BoostedType.HasValue && BoostedType.Value == moveType)
                return BoostMultiplier;
            return 1f;
        }

        /// <summary>
        /// Gets the damage multiplier when a grounded Pokemon is hit by a type.
        /// </summary>
        public float GetDamageReceivedMultiplier(PokemonType attackingType)
        {
            if (ReducedDamageType.HasValue && ReducedDamageType.Value == attackingType)
                return DamageReductionMultiplier;
            return 1f;
        }

        /// <summary>
        /// Checks if a status is prevented by this terrain for grounded Pokemon.
        /// </summary>
        public bool PreventsStatus(PersistentStatus status)
        {
            foreach (var prevented in PreventedStatuses)
            {
                if (prevented == status) return true;
            }
            return false;
        }

        /// <summary>
        /// Checks if a move has halved damage in this terrain.
        /// </summary>
        public bool HalvesMoveDamage(string moveName)
        {
            foreach (var move in HalvedDamageMoves)
            {
                if (move.Equals(moveName, StringComparison.OrdinalIgnoreCase)) return true;
            }
            return false;
        }

        /// <summary>
        /// Checks if a Pokemon is affected by terrain (grounded check).
        /// Pokemon is grounded if it doesn't have Flying type, Levitate ability,
        /// or is holding Air Balloon, and isn't under Magnet Rise/Telekinesis.
        /// </summary>
        public static bool IsGrounded(PokemonType primaryType, PokemonType? secondaryType, string abilityName, string heldItemName)
        {
            // Flying types are not grounded
            if (primaryType == PokemonType.Flying || secondaryType == PokemonType.Flying)
                return false;

            // Levitate ability makes Pokemon not grounded
            if (!string.IsNullOrEmpty(abilityName) &&
                abilityName.Equals("Levitate", StringComparison.OrdinalIgnoreCase))
                return false;

            // Air Balloon makes Pokemon not grounded
            if (!string.IsNullOrEmpty(heldItemName) &&
                heldItemName.Equals("Air Balloon", StringComparison.OrdinalIgnoreCase))
                return false;

            return true;
        }

        public override string ToString() => Name;

        #endregion
    }
}

