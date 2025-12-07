namespace PokemonUltimate.Combat.Foundation.Field
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
        /// Whether this is a boss battle (raid format).
        /// Boss battles have special mechanics (increased HP, stats, phases).
        /// </summary>
        public bool IsBossBattle { get; set; } = false;

        /// <summary>
        /// HP multiplier for boss Pokemon (e.g., 5.0 = 5x HP).
        /// Only applies if <see cref="IsBossBattle"/> is true.
        /// </summary>
        public float BossMultiplier { get; set; } = 1.0f;

        /// <summary>
        /// Stat multiplier for boss Pokemon (e.g., 1.5 = 1.5x stats).
        /// Only applies if <see cref="IsBossBattle"/> is true.
        /// Defaults to 1.2 (20% increase) if not specified.
        /// </summary>
        public float BossStatMultiplier { get; set; } = 1.2f;

        /// <summary>
        /// Creates default battle rules for a standard 1v1 singles battle.
        /// </summary>
        public static BattleRules Singles => new BattleRules { PlayerSlots = 1, EnemySlots = 1 };

        /// <summary>
        /// Creates battle rules for a 2v2 doubles battle.
        /// </summary>
        public static BattleRules Doubles => new BattleRules { PlayerSlots = 2, EnemySlots = 2 };

        /// <summary>
        /// Creates battle rules for a 3v3 triples battle.
        /// </summary>
        public static BattleRules Triples => new BattleRules { PlayerSlots = 3, EnemySlots = 3 };

        /// <summary>
        /// Creates battle rules for a 1v3 horde battle.
        /// </summary>
        public static BattleRules Horde => new BattleRules { PlayerSlots = 1, EnemySlots = 3 };

        /// <summary>
        /// Creates battle rules for a 1v2 horde battle.
        /// </summary>
        public static BattleRules Horde1v2 => new BattleRules { PlayerSlots = 1, EnemySlots = 2 };

        /// <summary>
        /// Creates battle rules for a 1v3 horde battle.
        /// Alias for <see cref="Horde"/>.
        /// </summary>
        public static BattleRules Horde1v3 => new BattleRules { PlayerSlots = 1, EnemySlots = 3 };

        /// <summary>
        /// Creates battle rules for a 1v5 horde battle.
        /// </summary>
        public static BattleRules Horde1v5 => new BattleRules { PlayerSlots = 1, EnemySlots = 5 };

        /// <summary>
        /// Creates battle rules for a 1vBoss raid battle.
        /// Boss has 5x HP and 1.2x stats by default.
        /// </summary>
        public static BattleRules Raid1vBoss => new BattleRules
        {
            PlayerSlots = 1,
            EnemySlots = 1,
            IsBossBattle = true,
            BossMultiplier = 5.0f,
            BossStatMultiplier = 1.2f
        };

        /// <summary>
        /// Creates battle rules for a 2vBoss raid battle.
        /// Boss has 5x HP and 1.2x stats by default.
        /// </summary>
        public static BattleRules Raid2vBoss => new BattleRules
        {
            PlayerSlots = 2,
            EnemySlots = 1,
            IsBossBattle = true,
            BossMultiplier = 5.0f,
            BossStatMultiplier = 1.2f
        };
    }
}

