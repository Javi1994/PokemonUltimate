using PokemonUltimate.Combat.Foundation.Field;

namespace PokemonUltimate.Combat.Execution.Battle
{
    /// <summary>
    /// Utility methods for battle processing.
    /// Static helper methods used by various processors.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `docs/features/2-combat-system/2.6-combat-engine/architecture.md`
    /// </remarks>
    public static class BattleUtilities
    {
        /// <summary>
        /// Determines the battle mode name based on slot configuration.
        /// </summary>
        /// <param name="rules">The battle rules. Cannot be null.</param>
        /// <returns>The battle mode name.</returns>
        public static string DetermineBattleMode(BattleRules rules)
        {
            if (rules == null)
                return "Custom";

            // Standard modes
            if (rules.PlayerSlots == 1 && rules.EnemySlots == 1)
                return rules.IsBossBattle ? "Raid (1vBoss)" : "Singles";
            if (rules.PlayerSlots == 2 && rules.EnemySlots == 2)
                return "Doubles";
            if (rules.PlayerSlots == 3 && rules.EnemySlots == 3)
                return "Triples";

            // Horde modes
            if (rules.PlayerSlots == 1 && rules.EnemySlots == 2)
                return "Horde (1v2)";
            if (rules.PlayerSlots == 1 && rules.EnemySlots == 3)
                return "Horde (1v3)";
            if (rules.PlayerSlots == 1 && rules.EnemySlots == 5)
                return "Horde (1v5)";

            // Raid modes
            if (rules.IsBossBattle)
            {
                if (rules.PlayerSlots == 1 && rules.EnemySlots == 1)
                    return "Raid (1vBoss)";
                if (rules.PlayerSlots == 2 && rules.EnemySlots == 1)
                    return "Raid (2vBoss)";
            }

            // Custom mode
            return $"Custom ({rules.PlayerSlots}v{rules.EnemySlots})";
        }

        /// <summary>
        /// Gets the total HP of all active Pokemon on a side.
        /// </summary>
        /// <param name="side">The battle side. Cannot be null.</param>
        /// <returns>The total HP of all active Pokemon.</returns>
        public static int GetTotalHP(BattleSide side)
        {
            if (side == null || side.Slots == null)
                return 0;

            int totalHP = 0;
            foreach (var slot in side.Slots)
            {
                if (slot.Pokemon != null && !slot.Pokemon.IsFainted)
                {
                    totalHP += slot.Pokemon.CurrentHP;
                }
            }
            return totalHP;
        }
    }
}
