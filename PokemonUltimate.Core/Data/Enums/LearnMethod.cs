namespace PokemonUltimate.Core.Data.Enums
{
    /// <summary>
    /// Defines how a Pokemon can learn a move.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.10: Enums & Constants
    /// **Documentation**: See `docs/features/1-game-data/1.10-enums-constants/README.md`
    /// </remarks>
    public enum LearnMethod
    {
        /// <summary>Move available from level 1.</summary>
        Start,

        /// <summary>Learns when reaching a specific level.</summary>
        LevelUp,

        /// <summary>Learns upon evolving.</summary>
        Evolution,

        /// <summary>Learns via TM/HM.</summary>
        TM,

        /// <summary>Inherited from parents (breeding).</summary>
        Egg,

        /// <summary>Taught by a move tutor.</summary>
        Tutor
    }
}

