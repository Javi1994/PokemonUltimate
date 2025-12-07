using System;
using PokemonUltimate.Combat.Damage;
using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Core.Data.Constants;
using PokemonUltimate.Core.Data.Enums;

namespace PokemonUltimate.Combat.Factories
{
    /// <summary>
    /// Factory for creating DamageContext instances.
    /// Centralizes creation logic and eliminates duplication of dummy MoveData creation.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.4: Damage Calculation Pipeline
    /// **Documentation**: See `docs/features/2-combat-system/2.4-damage-calculation-pipeline/architecture.md`
    /// </remarks>
    public class DamageContextFactory
    {
        /// <summary>
        /// Creates a DamageContext for a move attack.
        /// </summary>
        /// <param name="attacker">The attacking slot.</param>
        /// <param name="defender">The defending slot.</param>
        /// <param name="move">The move being used.</param>
        /// <param name="field">The battlefield.</param>
        /// <param name="forceCritical">Force a critical hit (for testing).</param>
        /// <param name="fixedRandomValue">Use a fixed random value (for testing).</param>
        /// <returns>A new DamageContext for the move attack.</returns>
        /// <exception cref="ArgumentNullException">If any required parameter is null.</exception>
        public DamageContext CreateForMove(
            BattleSlot attacker,
            BattleSlot defender,
            MoveData move,
            BattleField field,
            bool forceCritical = false,
            float? fixedRandomValue = null)
        {
            if (attacker == null)
                throw new ArgumentNullException(nameof(attacker), ErrorMessages.PokemonCannotBeNull);
            if (defender == null)
                throw new ArgumentNullException(nameof(defender), ErrorMessages.PokemonCannotBeNull);
            if (move == null)
                throw new ArgumentNullException(nameof(move), ErrorMessages.MoveCannotBeNull);
            if (field == null)
                throw new ArgumentNullException(nameof(field), ErrorMessages.FieldCannotBeNull);

            return new DamageContext(attacker, defender, move, field, forceCritical, fixedRandomValue);
        }

        /// <summary>
        /// Creates a DamageContext for status damage (burn, poison, etc.).
        /// Uses a dummy move for the context structure.
        /// </summary>
        /// <param name="slot">The slot taking damage.</param>
        /// <param name="damage">The damage amount.</param>
        /// <param name="field">The battlefield.</param>
        /// <returns>A new DamageContext configured for status damage.</returns>
        /// <exception cref="ArgumentNullException">If slot or field is null.</exception>
        public DamageContext CreateForStatusDamage(
            BattleSlot slot,
            int damage,
            BattleField field)
        {
            if (slot == null)
                throw new ArgumentNullException(nameof(slot), ErrorMessages.PokemonCannotBeNull);
            if (field == null)
                throw new ArgumentNullException(nameof(field), ErrorMessages.FieldCannotBeNull);

            var dummyMove = CreateStatusDamageMove();
            var context = new DamageContext(slot, slot, dummyMove, field);
            context.BaseDamage = damage;
            context.Multiplier = 1.0f;
            context.TypeEffectiveness = 1.0f;
            return context;
        }

        /// <summary>
        /// Creates a DamageContext for hazard damage (Stealth Rock, Spikes, etc.).
        /// Uses a dummy move for the context structure.
        /// </summary>
        /// <param name="slot">The slot taking damage.</param>
        /// <param name="damage">The damage amount.</param>
        /// <param name="field">The battlefield.</param>
        /// <returns>A new DamageContext configured for hazard damage.</returns>
        /// <exception cref="ArgumentNullException">If slot or field is null.</exception>
        public DamageContext CreateForHazardDamage(
            BattleSlot slot,
            int damage,
            BattleField field)
        {
            if (slot == null)
                throw new ArgumentNullException(nameof(slot), ErrorMessages.PokemonCannotBeNull);
            if (field == null)
                throw new ArgumentNullException(nameof(field), ErrorMessages.FieldCannotBeNull);

            var dummyMove = CreateHazardDamageMove();
            var context = new DamageContext(slot, slot, dummyMove, field);
            context.BaseDamage = damage;
            context.Multiplier = 1.0f;
            context.TypeEffectiveness = 1.0f;
            return context;
        }

        /// <summary>
        /// Creates a dummy MoveData for status damage contexts.
        /// </summary>
        /// <returns>A MoveData instance representing status damage.</returns>
        private MoveData CreateStatusDamageMove()
        {
            return new MoveData
            {
                Name = "Status Damage",
                Power = 0,
                Accuracy = 100,
                Type = PokemonType.Normal,
                Category = MoveCategory.Status,
                MaxPP = 0,
                Priority = 0,
                TargetScope = TargetScope.Self
            };
        }

        /// <summary>
        /// Creates a DamageContext for recoil damage (e.g., from moves like Take Down).
        /// </summary>
        /// <param name="slot">The slot taking recoil damage.</param>
        /// <param name="damage">The recoil damage amount.</param>
        /// <param name="move">The move that caused the recoil.</param>
        /// <param name="field">The battlefield.</param>
        /// <returns>A new DamageContext configured for recoil damage.</returns>
        /// <exception cref="ArgumentNullException">If slot, move, or field is null.</exception>
        public DamageContext CreateForRecoil(
            BattleSlot slot,
            int damage,
            MoveData move,
            BattleField field)
        {
            if (slot == null)
                throw new ArgumentNullException(nameof(slot), ErrorMessages.PokemonCannotBeNull);
            if (move == null)
                throw new ArgumentNullException(nameof(move), ErrorMessages.MoveCannotBeNull);
            if (field == null)
                throw new ArgumentNullException(nameof(field), ErrorMessages.FieldCannotBeNull);

            var context = new DamageContext(slot, slot, move, field);
            context.BaseDamage = damage;
            context.Multiplier = 1.0f;
            context.TypeEffectiveness = 1.0f;
            return context;
        }

        /// <summary>
        /// Creates a DamageContext for counter damage (e.g., from Counter or Mirror Coat).
        /// </summary>
        /// <param name="attacker">The slot that will take counter damage.</param>
        /// <param name="defender">The slot that is countering.</param>
        /// <param name="damage">The counter damage amount.</param>
        /// <param name="move">The move being countered.</param>
        /// <param name="field">The battlefield.</param>
        /// <returns>A new DamageContext configured for counter damage.</returns>
        /// <exception cref="ArgumentNullException">If any required parameter is null.</exception>
        public DamageContext CreateForCounter(
            BattleSlot attacker,
            BattleSlot defender,
            int damage,
            MoveData move,
            BattleField field)
        {
            if (attacker == null)
                throw new ArgumentNullException(nameof(attacker), ErrorMessages.PokemonCannotBeNull);
            if (defender == null)
                throw new ArgumentNullException(nameof(defender), ErrorMessages.PokemonCannotBeNull);
            if (move == null)
                throw new ArgumentNullException(nameof(move), ErrorMessages.MoveCannotBeNull);
            if (field == null)
                throw new ArgumentNullException(nameof(field), ErrorMessages.FieldCannotBeNull);

            var context = new DamageContext(defender, attacker, move, field);
            context.BaseDamage = damage;
            context.Multiplier = 1.0f;
            context.TypeEffectiveness = 1.0f;
            return context;
        }

        /// <summary>
        /// Creates a dummy MoveData for hazard damage contexts.
        /// </summary>
        /// <returns>A MoveData instance representing hazard damage.</returns>
        private MoveData CreateHazardDamageMove()
        {
            return new MoveData
            {
                Name = "Entry Hazard",
                Power = 1,
                Accuracy = 100,
                Type = PokemonType.Normal,
                Category = MoveCategory.Physical,
                MaxPP = 0,
                Priority = 0,
                TargetScope = TargetScope.Self
            };
        }
    }
}
