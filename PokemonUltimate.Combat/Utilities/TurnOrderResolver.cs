using System;
using System.Collections.Generic;
using System.Linq;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Damage;
using PokemonUltimate.Combat.Field;
using PokemonUltimate.Combat.Infrastructure.Constants;
using PokemonUltimate.Combat.Infrastructure.Providers.Definition;
using PokemonUltimate.Core.Data.Constants;
using PokemonUltimate.Core.Data.Enums;

namespace PokemonUltimate.Combat.Utilities
{
    /// <summary>
    /// Sorts battle actions by priority and speed to determine turn order.
    /// Centralized turn order resolution system for combat engine.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.3: Turn Order Resolution
    /// **Documentation**: See `docs/features/2-combat-system/2.3-turn-order-resolution/architecture.md`
    /// </remarks>
    public class TurnOrderResolver
    {
        private readonly IRandomProvider _randomProvider;

        /// <summary>
        /// Creates a new TurnOrderResolver with a random provider.
        /// </summary>
        /// <param name="randomProvider">The random provider for tie-breaking. Cannot be null.</param>
        /// <exception cref="ArgumentNullException">If randomProvider is null.</exception>
        public TurnOrderResolver(IRandomProvider randomProvider)
        {
            _randomProvider = randomProvider ?? throw new ArgumentNullException(nameof(randomProvider), ErrorMessages.PokemonCannotBeNull);
        }

        /// <summary>
        /// Sorts actions by priority (descending), then by effective speed (descending).
        /// Speed ties are resolved randomly.
        /// </summary>
        /// <param name="actions">Actions to sort.</param>
        /// <param name="field">The battlefield for context (speed modifiers, etc.).</param>
        /// <returns>Actions sorted by turn order.</returns>
        /// <exception cref="ArgumentNullException">If actions or field is null.</exception>
        public List<BattleAction> SortActions(IEnumerable<BattleAction> actions, BattleField field)
        {
            if (actions == null)
                throw new ArgumentNullException(nameof(actions));
            if (field == null)
                throw new ArgumentNullException(nameof(field));

            return actions
                .OrderByDescending(a => GetPriority(a))
                .ThenByDescending(a => GetEffectiveSpeed(a?.User, field))
                .ThenBy(_ => _randomProvider.Next(int.MaxValue)) // Random tiebreaker
                .ToList();
        }

        /// <summary>
        /// Gets the priority of a battle action.
        /// Higher priority actions go first.
        /// </summary>
        /// <param name="action">The action to check.</param>
        /// <returns>The priority value (default 0).</returns>
        public int GetPriority(BattleAction action)
        {
            if (action == null)
                return 0;

            return action.Priority;
        }

        /// <summary>
        /// Calculates the effective speed of a slot, including all modifiers.
        /// </summary>
        /// <param name="slot">The slot to calculate speed for.</param>
        /// <param name="field">The battlefield for field effects.</param>
        /// <returns>The effective speed value.</returns>
        public float GetEffectiveSpeed(BattleSlot slot, BattleField field)
        {
            if (slot == null || slot.Pokemon == null)
                return 0;

            // Base speed (already includes IVs, EVs, Nature from stat calculation)
            float speed = slot.Pokemon.Speed;

            // Apply stat stages
            speed *= GetStatStageMultiplier(slot.GetStatStage(Stat.Speed));

            // Apply status conditions
            speed *= GetStatusMultiplier(slot.Pokemon.Status);

            // Apply item modifiers (Choice Scarf, Iron Ball)
            speed *= GetItemSpeedMultiplier(slot, field);

            // Apply side condition speed modifiers (Tailwind)
            speed *= GetSideConditionSpeedMultiplier(slot.Side);

            // Apply ability speed modifiers (Swift Swim, Chlorophyll, Sand Rush, Slush Rush, Speed Boost)
            speed *= GetAbilitySpeedMultiplier(slot, field);

            // Future: Apply field effects (Trick Room)

            return speed;
        }

        /// <summary>
        /// Gets the multiplier for a stat stage.
        /// Formula: +stages = (2 + stages) / 2, -stages = 2 / (2 - stages)
        /// </summary>
        /// <param name="stages">The number of stat stages (-6 to +6).</param>
        /// <returns>The multiplier (0.25x to 4.0x).</returns>
        private static float GetStatStageMultiplier(int stages)
        {
            // Clamp to valid range
            stages = Math.Max(-6, Math.Min(6, stages));

            if (stages >= 0)
            {
                return (2f + stages) / 2f;
            }
            else
            {
                return 2f / (2f - stages);
            }
        }

        /// <summary>
        /// Gets the speed multiplier for a status condition.
        /// </summary>
        /// <param name="status">The persistent status.</param>
        /// <returns>The speed multiplier.</returns>
        private static float GetStatusMultiplier(PersistentStatus status)
        {
            if (status == PersistentStatus.Paralysis)
            {
                return StatusConstants.ParalysisSpeedMultiplier;
            }

            return 1.0f;
        }

        /// <summary>
        /// Gets the speed multiplier from held items (Choice Scarf, etc.).
        /// </summary>
        /// <param name="slot">The slot to check.</param>
        /// <param name="field">The battlefield for context.</param>
        /// <returns>The speed multiplier (1.0f if no modification).</returns>
        private static float GetItemSpeedMultiplier(BattleSlot slot, BattleField field)
        {
            if (slot?.Pokemon?.HeldItem == null)
                return 1.0f;

            var itemModifier = new ItemStatModifier(slot.Pokemon.HeldItem);
            return itemModifier.GetStatMultiplier(slot, Stat.Speed, field);
        }

        /// <summary>
        /// Gets the speed multiplier from side conditions (Tailwind, etc.).
        /// </summary>
        /// <param name="side">The side to check.</param>
        /// <returns>The speed multiplier (1.0f if no modification).</returns>
        /// <remarks>
        /// **Feature**: 2: Combat System
        /// **Sub-Feature**: 2.16: Field Conditions
        /// **Documentation**: See `docs/features/2-combat-system/2.16-field-conditions/README.md`
        /// </remarks>
        private static float GetSideConditionSpeedMultiplier(BattleSide side)
        {
            if (side == null)
                return 1.0f;

            // Check for Tailwind
            if (side.HasSideCondition(SideCondition.Tailwind))
            {
                var tailwindData = side.GetSideConditionData(SideCondition.Tailwind);
                if (tailwindData != null && tailwindData.ModifiesSpeed)
                {
                    return tailwindData.SpeedMultiplier;
                }
            }

            return 1.0f;
        }

        /// <summary>
        /// Gets the speed multiplier from abilities (Swift Swim, Chlorophyll, Sand Rush, Slush Rush, Speed Boost).
        /// </summary>
        /// <param name="slot">The slot to check.</param>
        /// <param name="field">The battlefield for weather context.</param>
        /// <returns>The speed multiplier (1.0f if no modification).</returns>
        /// <remarks>
        /// **Feature**: 2: Combat System
        /// **Sub-Feature**: 2.17: Advanced Abilities
        /// **Documentation**: See `docs/features/2-combat-system/PLAN_COMPLETAR_FEATURE_2.md`
        /// </remarks>
        private static float GetAbilitySpeedMultiplier(BattleSlot slot, BattleField field)
        {
            if (slot?.Pokemon?.Ability == null || field == null)
                return 1.0f;

            var ability = slot.Pokemon.Ability;

            // Speed Boost: +1 Speed stage each turn (handled via stat stages, not multiplier)
            // This is handled by OnTurnEnd trigger, not here

            // SpeedBoostInWeather: Double speed in specific weather (Swift Swim, Chlorophyll, Sand Rush, Slush Rush)
            if (ability.Effect == AbilityEffect.SpeedBoostInWeather)
            {
                // Check if current weather matches ability's weather condition
                if (ability.WeatherCondition.HasValue && field.Weather == ability.WeatherCondition.Value)
                {
                    // Use multiplier from ability (default 2.0f for Swift Swim, Chlorophyll, etc.)
                    return ability.Multiplier > 0 ? ability.Multiplier : 2.0f;
                }
            }

            return 1.0f;
        }
    }
}

