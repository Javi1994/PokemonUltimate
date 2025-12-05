namespace PokemonUltimate.Core.Constants
{
    /// <summary>
    /// Constants for Core module.
    /// Centralizes all magic numbers and configuration values.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.10: Enums & Constants
    /// **Documentation**: See `docs/features/1-game-data/1.10-enums-constants/README.md`
    /// </remarks>
    public static class CoreConstants
    {
        #region Shiny

        /// <summary>
        /// Natural shiny odds (1/4096).
        /// </summary>
        public const int ShinyOdds = 4096;

        #endregion

        #region Friendship

        /// <summary>
        /// Default friendship for wild Pokemon.
        /// </summary>
        public const int DefaultWildFriendship = 70;

        /// <summary>
        /// Friendship for hatched Pokemon.
        /// </summary>
        public const int HatchedFriendship = 120;

        /// <summary>
        /// High friendship threshold (for evolutions).
        /// </summary>
        public const int HighFriendshipThreshold = 220;

        /// <summary>
        /// Maximum friendship value.
        /// </summary>
        public const int MaxFriendship = 255;

        #endregion

        #region IVs and EVs

        /// <summary>
        /// Maximum Individual Value (0-31).
        /// </summary>
        public const int MaxIV = 31;

        /// <summary>
        /// Maximum Effort Value per stat (0-252).
        /// </summary>
        public const int MaxEV = 252;

        /// <summary>
        /// Maximum total EVs across all stats (510).
        /// </summary>
        public const int MaxTotalEV = 510;

        /// <summary>
        /// Default IV used in this game (always max for roguelike).
        /// </summary>
        public const int DefaultIV = MaxIV;

        /// <summary>
        /// Default EV used in this game (always max for roguelike).
        /// </summary>
        public const int DefaultEV = MaxEV;

        #endregion

        #region Stat Stages

        /// <summary>
        /// Minimum stat stage (-6).
        /// </summary>
        public const int MinStatStage = -6;

        /// <summary>
        /// Maximum stat stage (+6).
        /// </summary>
        public const int MaxStatStage = 6;

        #endregion

        #region Stat Calculation Formulas

        /// <summary>
        /// Base multiplier for stat calculation (2x base stat).
        /// </summary>
        public const int StatFormulaBase = 2;

        /// <summary>
        /// Divisor for stat calculation (divide by 100).
        /// </summary>
        public const int StatFormulaDivisor = 100;

        /// <summary>
        /// Bonus added to non-HP stats (+5).
        /// </summary>
        public const int StatFormulaBonus = 5;

        /// <summary>
        /// Bonus added to HP calculation (+10).
        /// </summary>
        public const int HPFormulaBonus = 10;

        /// <summary>
        /// Divisor for EV bonus calculation (divide EV by 4).
        /// </summary>
        public const int EVBonusDivisor = 4;

        #endregion
    }
}
