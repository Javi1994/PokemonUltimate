namespace PokemonUltimate.Core.Enums
{
    /// <summary>
    /// Determines which stats are used for damage calculation.
    /// </summary>
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
