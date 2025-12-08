using System;
using PokemonUltimate.Combat.Foundation.Constants;
using PokemonUltimate.Combat.Foundation.Field;
using PokemonUltimate.Core.Data.Constants;

namespace PokemonUltimate.Combat.Execution.Battle
{
    /// <summary>
    /// Determines the outcome of a battle by inspecting the battlefield state.
    /// Stateless service that checks win/loss conditions.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `docs/features/2-combat-system/2.6-combat-engine/architecture.md`
    /// </remarks>
    public static class BattleArbiter
    {
        /// <summary>
        /// Checks the current outcome of the battle.
        /// </summary>
        /// <param name="field">The battlefield to inspect. Cannot be null.</param>
        /// <returns>The current battle outcome.</returns>
        /// <exception cref="ArgumentNullException">If field is null.</exception>
        public static BattleOutcome CheckOutcome(BattleField field)
        {
            if (field == null)
                throw new ArgumentNullException(nameof(field), ErrorMessages.FieldCannotBeNull);

            bool playerDefeated = field.PlayerSide.IsDefeated();
            bool enemyDefeated = field.EnemySide.IsDefeated();

            // Both sides defeated simultaneously (e.g., Explosion double KO)
            if (playerDefeated && enemyDefeated)
                return BattleOutcome.Draw;

            // Player defeated
            if (playerDefeated)
                return BattleOutcome.Defeat;

            // Enemy defeated
            if (enemyDefeated)
                return BattleOutcome.Victory;

            // Battle continues
            return BattleOutcome.Ongoing;
        }
    }
}

