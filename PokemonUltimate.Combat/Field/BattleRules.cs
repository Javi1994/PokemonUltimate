namespace PokemonUltimate.Combat
{
    /// <summary>
    /// Configuration for a battle, defining the number of slots and other rules.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.1: Battle Foundation
    /// **Documentation**: See `docs/features/2-combat-system/2.1-battle-foundation/architecture.md`
    /// </remarks>
    public class BattleRules
    {
        /// <summary>
        /// Number of active slots on the player's side.
        /// 1 = Singles, 2 = Doubles, 3 = Triples.
        /// </summary>
        public int PlayerSlots { get; set; } = 1;

        /// <summary>
        /// Number of active slots on the enemy's side.
        /// Usually matches PlayerSlots, but can differ for horde battles (1v3).
        /// </summary>
        public int EnemySlots { get; set; } = 1;

        /// <summary>
        /// Maximum number of turns before the battle ends in a draw.
        /// 0 means no limit.
        /// </summary>
        public int MaxTurns { get; set; } = 0;

        /// <summary>
        /// Whether items can be used during battle.
        /// </summary>
        public bool AllowItems { get; set; } = true;

        /// <summary>
        /// Whether switching Pokemon is allowed.
        /// </summary>
        public bool AllowSwitching { get; set; } = true;

        /// <summary>
        /// Creates default battle rules for a standard 1v1 singles battle.
        /// </summary>
        public static BattleRules Singles => new BattleRules { PlayerSlots = 1, EnemySlots = 1 };

        /// <summary>
        /// Creates battle rules for a 2v2 doubles battle.
        /// </summary>
        public static BattleRules Doubles => new BattleRules { PlayerSlots = 2, EnemySlots = 2 };

        /// <summary>
        /// Creates battle rules for a 1v3 horde battle.
        /// </summary>
        public static BattleRules Horde => new BattleRules { PlayerSlots = 1, EnemySlots = 3 };
    }
}

