using System;
using System.Collections.Generic;
using System.Linq;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Damage;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Constants;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Combat.Engine
{
    /// <summary>
    /// Constants for end-of-turn damage calculations.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.8: End-of-Turn Effects
    /// **Documentation**: See `docs/features/2-combat-system/2.8-end-of-turn-effects/architecture.md`
    /// </remarks>
    public static class EndOfTurnConstants
    {
        public const float BurnDamageFraction = 1f / 16f;      // 0.0625
        public const float PoisonDamageFraction = 1f / 8f;     // 0.125
        public const float BadlyPoisonedBaseFraction = 1f / 16f; // 0.0625
        public const int MinimumDamage = 1;
    }

    /// <summary>
    /// Processes all end-of-turn effects after action queue is empty.
    /// Generates actions for status damage, item effects, and other end-of-turn mechanics.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.8: End-of-Turn Effects
    /// **Documentation**: See `docs/features/2-combat-system/2.8-end-of-turn-effects/architecture.md`
    /// </remarks>
    public static class EndOfTurnProcessor
    {
        // Dummy move for status damage context (not used in calculation)
        private static MoveData CreateStatusDamageMove()
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
        /// Processes all end-of-turn effects for the battlefield.
        /// Returns actions to be executed (status damage, healing, etc.).
        /// </summary>
        /// <param name="field">The battlefield. Cannot be null.</param>
        /// <returns>List of actions to execute for end-of-turn effects.</returns>
        /// <exception cref="ArgumentNullException">If field is null.</exception>
        public static List<BattleAction> ProcessEffects(BattleField field)
        {
            if (field == null)
                throw new ArgumentNullException(nameof(field), ErrorMessages.FieldCannotBeNull);

            var actions = new List<BattleAction>();

            // Process all active slots
            foreach (var slot in field.GetAllActiveSlots())
            {
                if (slot.IsEmpty || slot.HasFainted)
                    continue;

                var pokemon = slot.Pokemon;
                var statusActions = ProcessStatusEffects(slot, field);
                actions.AddRange(statusActions);
            }

            return actions;
        }

        /// <summary>
        /// Processes status effects for a single Pokemon slot.
        /// </summary>
        private static List<BattleAction> ProcessStatusEffects(BattleSlot slot, BattleField field)
        {
            var actions = new List<BattleAction>();
            var pokemon = slot.Pokemon;

            switch (pokemon.Status)
            {
                case PersistentStatus.Burn:
                    actions.AddRange(ProcessBurn(slot, field));
                    break;

                case PersistentStatus.Poison:
                    actions.AddRange(ProcessPoison(slot, field));
                    break;

                case PersistentStatus.BadlyPoisoned:
                    actions.AddRange(ProcessBadlyPoisoned(slot, field));
                    break;

                case PersistentStatus.None:
                case PersistentStatus.Paralysis:
                case PersistentStatus.Sleep:
                case PersistentStatus.Freeze:
                    // No end-of-turn damage for these
                    break;
            }

            return actions;
        }

        /// <summary>
        /// Processes Burn status: deals 1/16 Max HP damage.
        /// </summary>
        private static List<BattleAction> ProcessBurn(BattleSlot slot, BattleField field)
        {
            var actions = new List<BattleAction>();
            var pokemon = slot.Pokemon;

            int damage = CalculateBurnDamage(pokemon.MaxHP);
            actions.Add(new MessageAction(string.Format(GameMessages.StatusBurnDamage, pokemon.DisplayName)));
            actions.Add(CreateStatusDamageAction(slot, field, damage));

            return actions;
        }

        /// <summary>
        /// Processes Poison status: deals 1/8 Max HP damage.
        /// </summary>
        private static List<BattleAction> ProcessPoison(BattleSlot slot, BattleField field)
        {
            var actions = new List<BattleAction>();
            var pokemon = slot.Pokemon;

            int damage = CalculatePoisonDamage(pokemon.MaxHP);
            actions.Add(new MessageAction(string.Format(GameMessages.StatusPoisonDamage, pokemon.DisplayName)));
            actions.Add(CreateStatusDamageAction(slot, field, damage));

            return actions;
        }

        /// <summary>
        /// Processes Badly Poisoned (Toxic) status: deals escalating damage and increments counter.
        /// </summary>
        private static List<BattleAction> ProcessBadlyPoisoned(BattleSlot slot, BattleField field)
        {
            var actions = new List<BattleAction>();
            var pokemon = slot.Pokemon;

            // Ensure counter starts at 1 if not set
            if (pokemon.StatusTurnCounter < 1)
                pokemon.StatusTurnCounter = 1;

            int damage = CalculateBadlyPoisonedDamage(pokemon.MaxHP, pokemon.StatusTurnCounter);
            actions.Add(new MessageAction(string.Format(GameMessages.StatusPoisonDamage, pokemon.DisplayName)));
            actions.Add(CreateStatusDamageAction(slot, field, damage));

            // Increment counter for next turn
            pokemon.StatusTurnCounter++;

            return actions;
        }

        /// <summary>
        /// Calculates Burn damage: 1/16 of Max HP (minimum 1).
        /// </summary>
        private static int CalculateBurnDamage(int maxHP)
        {
            int damage = maxHP / 16;
            return Math.Max(EndOfTurnConstants.MinimumDamage, damage);
        }

        /// <summary>
        /// Calculates Poison damage: 1/8 of Max HP (minimum 1).
        /// </summary>
        private static int CalculatePoisonDamage(int maxHP)
        {
            int damage = maxHP / 8;
            return Math.Max(EndOfTurnConstants.MinimumDamage, damage);
        }

        /// <summary>
        /// Calculates Badly Poisoned damage: (counter * Max HP) / 16 (minimum 1).
        /// </summary>
        private static int CalculateBadlyPoisonedDamage(int maxHP, int counter)
        {
            int damage = (counter * maxHP) / 16;
            return Math.Max(EndOfTurnConstants.MinimumDamage, damage);
        }

        /// <summary>
        /// Creates a DamageAction for status damage using a simple DamageContext.
        /// </summary>
        private static DamageAction CreateStatusDamageAction(BattleSlot slot, BattleField field, int damage)
        {
            // Create a dummy context with the desired damage
            var dummyMove = CreateStatusDamageMove();
            var context = new DamageContext(slot, slot, dummyMove, field);
            context.BaseDamage = damage;
            context.Multiplier = 1.0f;
            context.TypeEffectiveness = 1.0f; // Status damage is always effective

            return new DamageAction(slot, slot, context);
        }
    }
}

