using System;
using PokemonUltimate.Combat.Damage;
using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Core.Data.Constants;
using PokemonUltimate.Core.Data.Enums;

namespace PokemonUltimate.Combat.Damage.Steps
{
    /// <summary>
    /// Applies screen-based damage reduction to moves.
    /// Screens reduce damage based on move category (Physical/Special) or all damage (Aurora Veil).
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.16: Field Conditions
    /// **Documentation**: See `docs/features/2-combat-system/2.16-field-conditions/README.md`
    /// </remarks>
    public class ScreenStep : IDamageStep
    {
        /// <summary>
        /// Processes the damage context and applies screen-based damage reduction.
        /// </summary>
        /// <param name="context">The damage context to modify.</param>
        public void Process(DamageContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context), ErrorMessages.ContextCannotBeNull);

            var defenderSide = context.Defender.Side;

            // If defender has no side assigned (e.g., in tests), skip screen checks
            if (defenderSide == null)
                return;

            var moveCategory = context.Move.Category;

            // Check for screens on defender's side
            // Priority: Aurora Veil > Reflect/Light Screen (Aurora Veil reduces all damage)
            if (defenderSide.HasSideCondition(SideCondition.AuroraVeil))
            {
                var auroraVeilData = defenderSide.GetSideConditionData(SideCondition.AuroraVeil);
                if (auroraVeilData != null && auroraVeilData.ReducesDamage)
                {
                    bool isDoubles = context.Field.Rules.PlayerSlots > 1 || context.Field.Rules.EnemySlots > 1;
                    float multiplier = auroraVeilData.GetDamageMultiplier(isDoubles);
                    context.Multiplier *= multiplier;
                    return; // Aurora Veil applies to all damage, no need to check other screens
                }
            }

            // Check Reflect (Physical damage reduction)
            if (moveCategory == MoveCategory.Physical && defenderSide.HasSideCondition(SideCondition.Reflect))
            {
                var reflectData = defenderSide.GetSideConditionData(SideCondition.Reflect);
                if (reflectData != null && reflectData.ReducesMoveCategory(MoveCategory.Physical))
                {
                    bool isDoubles = context.Field.Rules.PlayerSlots > 1 || context.Field.Rules.EnemySlots > 1;
                    float multiplier = reflectData.GetDamageMultiplier(isDoubles);
                    context.Multiplier *= multiplier;
                    return;
                }
            }

            // Check Light Screen (Special damage reduction)
            if (moveCategory == MoveCategory.Special && defenderSide.HasSideCondition(SideCondition.LightScreen))
            {
                var lightScreenData = defenderSide.GetSideConditionData(SideCondition.LightScreen);
                if (lightScreenData != null && lightScreenData.ReducesMoveCategory(MoveCategory.Special))
                {
                    bool isDoubles = context.Field.Rules.PlayerSlots > 1 || context.Field.Rules.EnemySlots > 1;
                    float multiplier = lightScreenData.GetDamageMultiplier(isDoubles);
                    context.Multiplier *= multiplier;
                    return;
                }
            }
        }
    }
}

