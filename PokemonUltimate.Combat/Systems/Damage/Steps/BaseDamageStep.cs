using System;
using PokemonUltimate.Combat.Foundation.Field;
using PokemonUltimate.Combat.Systems.Damage.Definition;
using PokemonUltimate.Core.Data.Effects;
using PokemonUltimate.Core.Data.Enums;

namespace PokemonUltimate.Combat.Systems.Damage.Steps
{
    /// <summary>
    /// Calculates the base damage using the Gen 3+ formula:
    /// BaseDamage = ((2 * Level / 5 + 2) * Power * Attack / Defense) / 50 + 2
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.4: Damage Calculation Pipeline
    /// **Documentation**: See `docs/features/2-combat-system/2.4-damage-calculation-pipeline/architecture.md`
    /// </remarks>
    public class BaseDamageStep : IDamageStep
    {
        public void Process(DamageContext context)
        {
            var attacker = context.Attacker.Pokemon;
            var defender = context.Defender.Pokemon;
            var move = context.Move;

            // Fixed damage moves (like Dragon Rage) bypass the damage formula
            if (move.HasEffect<FixedDamageEffect>())
            {
                var fixedDamage = move.GetEffect<FixedDamageEffect>();
                context.BaseDamage = fixedDamage.Amount;
                return;
            }

            // Get relevant stats based on move category
            int attackStat;
            int defenseStat;

            if (move.Category == MoveCategory.Physical)
            {
                attackStat = attacker.Attack;
                defenseStat = defender.Defense;
            }
            else if (move.Category == MoveCategory.Special)
            {
                attackStat = attacker.SpAttack;
                defenseStat = defender.SpDefense;
            }
            else
            {
                // Status moves don't deal damage
                context.BaseDamage = 0;
                return;
            }

            // Apply stat stages
            attackStat = ApplyStatStage(attackStat, context.Attacker.GetStatStage(
                move.Category == MoveCategory.Physical ? Stat.Attack : Stat.SpAttack));

            defenseStat = ApplyStatStage(defenseStat, context.Defender.GetStatStage(
                move.Category == MoveCategory.Physical ? Stat.Defense : Stat.SpDefense));

            // Apply stat modifiers from abilities and items
            attackStat = ApplyStatModifiers(attackStat, context.Attacker,
                move.Category == MoveCategory.Physical ? Stat.Attack : Stat.SpAttack, context.Field);

            defenseStat = ApplyStatModifiers(defenseStat, context.Defender,
                move.Category == MoveCategory.Physical ? Stat.Defense : Stat.SpDefense, context.Field);

            // Ensure minimum values
            attackStat = Math.Max(1, attackStat);
            defenseStat = Math.Max(1, defenseStat);

            // Gen 3+ formula: ((2 * Level / 5 + 2) * Power * Attack / Defense) / 50 + 2
            float levelFactor = (2f * attacker.Level / 5f) + 2f;
            float statRatio = (float)attackStat / defenseStat;
            float baseDamage = (levelFactor * move.Power * statRatio) / 50f + 2f;

            context.BaseDamage = baseDamage;
        }

        private int ApplyStatStage(int baseStat, int stage)
        {
            // Clamp stage to valid range
            stage = Math.Max(-6, Math.Min(6, stage));

            float multiplier;
            if (stage >= 0)
            {
                multiplier = (2f + stage) / 2f;
            }
            else
            {
                multiplier = 2f / (2f - stage);
            }

            return (int)(baseStat * multiplier);
        }

        private int ApplyStatModifiers(int statValue, BattleSlot slot, Stat stat, BattleField field)
        {
            float multiplier = 1.0f;

            // Check ability modifier
            if (slot.Pokemon.Ability != null)
            {
                var abilityModifier = new AbilityStatModifier(slot.Pokemon.Ability);
                multiplier *= abilityModifier.GetStatMultiplier(slot, stat, field);
            }

            // Check item modifier
            if (slot.Pokemon.HeldItem != null)
            {
                var itemModifier = new ItemStatModifier(slot.Pokemon.HeldItem);
                multiplier *= itemModifier.GetStatMultiplier(slot, stat, field);
            }

            return (int)(statValue * multiplier);
        }
    }
}

