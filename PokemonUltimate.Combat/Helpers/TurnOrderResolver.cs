using System;
using System.Collections.Generic;
using System.Linq;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Damage;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Combat.Helpers
{
    /// <summary>
    /// Sorts battle actions by priority and speed to determine turn order.
    /// Centralized turn order resolution system for combat engine.
    /// </summary>
    public static class TurnOrderResolver
    {
        private static readonly Random _random = new Random();

        /// <summary>
        /// Sorts actions by priority (descending), then by effective speed (descending).
        /// Speed ties are resolved randomly.
        /// </summary>
        /// <param name="actions">Actions to sort.</param>
        /// <param name="field">The battlefield for context (speed modifiers, etc.).</param>
        /// <returns>Actions sorted by turn order.</returns>
        /// <exception cref="ArgumentNullException">If actions or field is null.</exception>
        public static List<BattleAction> SortActions(IEnumerable<BattleAction> actions, BattleField field)
        {
            if (actions == null)
                throw new ArgumentNullException(nameof(actions));
            if (field == null)
                throw new ArgumentNullException(nameof(field));

            return actions
                .OrderByDescending(a => GetPriority(a))
                .ThenByDescending(a => GetEffectiveSpeed(a?.User, field))
                .ThenBy(_ => _random.Next()) // Random tiebreaker
                .ToList();
        }

        /// <summary>
        /// Gets the priority of a battle action.
        /// Higher priority actions go first.
        /// </summary>
        /// <param name="action">The action to check.</param>
        /// <returns>The priority value (default 0).</returns>
        public static int GetPriority(BattleAction action)
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
        public static float GetEffectiveSpeed(BattleSlot slot, BattleField field)
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

            // Future: Apply ability modifiers (Swift Swim, Sand Rush)
            // Future: Apply field effects (Tailwind, Trick Room)

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
                return 0.5f;
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
    }
}

