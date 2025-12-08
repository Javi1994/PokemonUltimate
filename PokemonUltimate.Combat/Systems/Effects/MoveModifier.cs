using System;
using PokemonUltimate.Combat.Systems.Effects.Definition;
using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Core.Data.Enums;

namespace PokemonUltimate.Combat.Systems.Effects
{
    /// <summary>
    /// Encapsulates temporary modifications to move properties.
    /// Creates modified MoveData instances without mutating the original.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.5: Combat Actions
    /// **Documentation**: See `docs/features/2-combat-system/2.5-combat-actions/architecture.md`
    /// </remarks>
    public class MoveModifier : IMoveModifier
    {
        private readonly float? _powerMultiplier;
        private readonly int? _powerOverride;
        private readonly float? _accuracyMultiplier;
        private readonly int? _accuracyOverride;
        private readonly PokemonType? _typeOverride;
        private readonly MoveCategory? _categoryOverride;

        /// <summary>
        /// Creates a new move modifier with the specified modifications.
        /// </summary>
        /// <param name="powerMultiplier">Multiplier for move power (e.g., 2.0 for doubling).</param>
        /// <param name="powerOverride">Override for move power (takes precedence over multiplier).</param>
        /// <param name="accuracyMultiplier">Multiplier for move accuracy.</param>
        /// <param name="accuracyOverride">Override for move accuracy.</param>
        /// <param name="typeOverride">Override for move type.</param>
        /// <param name="categoryOverride">Override for move category.</param>
        private MoveModifier(
            float? powerMultiplier = null,
            int? powerOverride = null,
            float? accuracyMultiplier = null,
            int? accuracyOverride = null,
            PokemonType? typeOverride = null,
            MoveCategory? categoryOverride = null)
        {
            _powerMultiplier = powerMultiplier;
            _powerOverride = powerOverride;
            _accuracyMultiplier = accuracyMultiplier;
            _accuracyOverride = accuracyOverride;
            _typeOverride = typeOverride;
            _categoryOverride = categoryOverride;
        }

        /// <summary>
        /// Creates a move modifier that multiplies power (e.g., Pursuit doubles power).
        /// </summary>
        /// <param name="multiplier">The power multiplier (e.g., 2.0 for doubling).</param>
        /// <returns>A new MoveModifier instance.</returns>
        public static MoveModifier MultiplyPower(float multiplier)
        {
            return new MoveModifier(powerMultiplier: multiplier);
        }

        /// <summary>
        /// Creates a move modifier that overrides power to a specific value.
        /// </summary>
        /// <param name="power">The power value to set.</param>
        /// <returns>A new MoveModifier instance.</returns>
        public static MoveModifier OverridePower(int power)
        {
            return new MoveModifier(powerOverride: power);
        }

        /// <summary>
        /// Creates a move modifier that multiplies accuracy.
        /// </summary>
        /// <param name="multiplier">The accuracy multiplier.</param>
        /// <returns>A new MoveModifier instance.</returns>
        public static MoveModifier MultiplyAccuracy(float multiplier)
        {
            return new MoveModifier(accuracyMultiplier: multiplier);
        }

        /// <summary>
        /// Creates a move modifier that overrides accuracy to a specific value.
        /// </summary>
        /// <param name="accuracy">The accuracy value to set.</param>
        /// <returns>A new MoveModifier instance.</returns>
        public static MoveModifier OverrideAccuracy(int accuracy)
        {
            return new MoveModifier(accuracyOverride: accuracy);
        }

        /// <summary>
        /// Creates a move modifier that overrides the move type.
        /// </summary>
        /// <param name="type">The type to set.</param>
        /// <returns>A new MoveModifier instance.</returns>
        public static MoveModifier OverrideType(PokemonType type)
        {
            return new MoveModifier(typeOverride: type);
        }

        /// <summary>
        /// Creates a move modifier that overrides the move category.
        /// </summary>
        /// <param name="category">The category to set.</param>
        /// <returns>A new MoveModifier instance.</returns>
        public static MoveModifier OverrideCategory(MoveCategory category)
        {
            return new MoveModifier(categoryOverride: category);
        }

        /// <summary>
        /// Combines this modifier with another, applying both sets of modifications.
        /// </summary>
        /// <param name="other">The other modifier to combine with.</param>
        /// <returns>A new MoveModifier that applies both modifications.</returns>
        public MoveModifier Combine(MoveModifier other)
        {
            if (other == null)
                return this;

            return new MoveModifier(
                powerMultiplier: other._powerMultiplier ?? _powerMultiplier,
                powerOverride: other._powerOverride ?? _powerOverride,
                accuracyMultiplier: other._accuracyMultiplier ?? _accuracyMultiplier,
                accuracyOverride: other._accuracyOverride ?? _accuracyOverride,
                typeOverride: other._typeOverride ?? _typeOverride,
                categoryOverride: other._categoryOverride ?? _categoryOverride);
        }

        /// <summary>
        /// Applies modifications to a move's properties.
        /// </summary>
        public MoveData ApplyModifications(MoveData originalMove)
        {
            if (originalMove == null)
                throw new ArgumentNullException(nameof(originalMove));

            // Check if any modifications are needed
            bool needsModification = _powerMultiplier.HasValue ||
                                    _powerOverride.HasValue ||
                                    _accuracyMultiplier.HasValue ||
                                    _accuracyOverride.HasValue ||
                                    _typeOverride.HasValue ||
                                    _categoryOverride.HasValue;

            if (!needsModification)
                return originalMove;

            // Create modified MoveData
            int modifiedPower = _powerOverride ??
                (_powerMultiplier.HasValue ? (int)(originalMove.Power * _powerMultiplier.Value) : originalMove.Power);

            int modifiedAccuracy = _accuracyOverride ??
                (_accuracyMultiplier.HasValue ? (int)(originalMove.Accuracy * _accuracyMultiplier.Value) : originalMove.Accuracy);

            return new MoveData
            {
                Name = originalMove.Name,
                Power = modifiedPower,
                Accuracy = modifiedAccuracy,
                Type = _typeOverride ?? originalMove.Type,
                Category = _categoryOverride ?? originalMove.Category,
                MaxPP = originalMove.MaxPP,
                Priority = originalMove.Priority,
                TargetScope = originalMove.TargetScope,
                Effects = originalMove.Effects,
                BypassesProtect = originalMove.BypassesProtect
            };
        }
    }
}
