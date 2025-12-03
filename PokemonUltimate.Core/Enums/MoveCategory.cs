namespace PokemonUltimate.Core.Enums
{
    /// <summary>
    /// Determines which stats are used for damage calculation.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.10: Enums & Constants
    /// **Documentation**: See `docs/features/1-game-data/1.10-enums-constants/README.md`
    /// </remarks>
    public enum MoveCategory
    {
        /// <summary>Uses Attack vs Defense.</summary>
        Physical,
        
        /// <summary>Uses SpAttack vs SpDefense.</summary>
        Special,
        
        /// <summary>No damage, only applies effects (status, stat changes, etc.).</summary>
        Status
    }
}
