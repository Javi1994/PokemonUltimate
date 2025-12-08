using PokemonUltimate.Combat.Field;
using PokemonUltimate.Combat.Handlers.Definition;
using PokemonUltimate.Core.Data.Enums;

namespace PokemonUltimate.Combat.Handlers.Checkers
{
    /// <summary>
    /// Handler para verificación de aplicación de cambios de estadísticas.
    /// Valida si un cambio de stats puede aplicarse considerando condiciones como Mist.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `docs/features/2-combat-system/2.6-combat-engine/PROCESSOR_REFACTOR_PROPOSAL.md`
    /// </remarks>
    public class StatChangeApplicationHandler : IApplicationHandler
    {
        /// <summary>
        /// Verifica si un cambio de estadística puede aplicarse.
        /// Considera condiciones como Mist que previenen reducciones de stats de oponentes.
        /// </summary>
        /// <param name="target">El slot objetivo. No puede ser null.</param>
        /// <param name="user">El slot que inició el cambio. Puede ser null para acciones del sistema.</param>
        /// <param name="change">El cambio de estadística (positivo o negativo).</param>
        /// <param name="field">El campo de batalla. No puede ser null.</param>
        /// <returns>True si el cambio puede aplicarse, false en caso contrario.</returns>
        public bool CanApplyStatChange(BattleSlot target, BattleSlot user, int change, BattleField field)
        {
            if (target == null || field == null)
                return false;

            // No change means no effect
            if (change == 0)
                return false;

            // Mist prevents stat reductions from opponents (but allows stat increases)
            if (change < 0 && target.Side.HasSideCondition(SideCondition.Mist))
            {
                var mistData = target.Side.GetSideConditionData(SideCondition.Mist);
                if (mistData != null && mistData.PreventsStatReduction)
                {
                    // Check if the stat reduction is from an opponent
                    // If User is null or User.Side != Target.Side, it's from an opponent
                    if (user == null || user.Side != target.Side)
                    {
                        // Stat reduction is blocked by Mist
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
