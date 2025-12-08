using System;
using PokemonUltimate.Combat.Systems.Damage.Definition;
using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Core.Data.Constants;
using PokemonUltimate.Core.Domain.Instances.Pokemon;

namespace PokemonUltimate.Combat.Systems.Damage.Steps
{
    /// <summary>
    /// Applies terrain-based damage multipliers to moves.
    /// Terrain can boost or reduce damage for certain move types, but only affects grounded Pokemon.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.13: Terrain System
    /// **Documentation**: See `docs/features/2-combat-system/2.13-terrain-system/README.md`
    /// </remarks>
    public class TerrainStep : IDamageStep
    {
        /// <summary>
        /// Processes the damage context and applies terrain-based damage multipliers.
        /// </summary>
        /// <param name="context">The damage context to modify.</param>
        public void Process(DamageContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context), ErrorMessages.ContextCannotBeNull);

            var field = context.Field;
            var moveType = context.Move.Type;

            // Check if there's active terrain
            if (field.TerrainData == null)
                return;

            var terrainData = field.TerrainData;
            float terrainMultiplier = 1.0f;

            // Check for type boost (Electric, Grassy, Psychic Terrain)
            // Boost applies if attacker is grounded
            if (terrainData.BoostedType.HasValue && terrainData.BoostedType.Value == moveType)
            {
                var attacker = context.Attacker.Pokemon;
                if (IsGrounded(attacker))
                {
                    terrainMultiplier = terrainData.BoostMultiplier;
                }
            }

            // Check for damage reduction (Misty Terrain)
            // Reduction applies if defender is grounded
            if (terrainData.ReducedDamageType.HasValue && terrainData.ReducedDamageType.Value == moveType)
            {
                var defender = context.Defender.Pokemon;
                if (IsGrounded(defender))
                {
                    terrainMultiplier = terrainData.DamageReductionMultiplier;
                }
            }

            // Apply multiplier
            context.Multiplier *= terrainMultiplier;
        }

        /// <summary>
        /// Checks if a Pokemon is grounded (affected by terrain).
        /// Pokemon is grounded if it doesn't have Flying type, Levitate ability, or Air Balloon item.
        /// </summary>
        private static bool IsGrounded(PokemonInstance pokemon)
        {
            if (pokemon == null)
                return false;

            return TerrainData.IsGrounded(
                pokemon.Species.PrimaryType,
                pokemon.Species.SecondaryType,
                pokemon.Ability?.Id,
                pokemon.HeldItem?.Id);
        }
    }
}

