namespace PokemonUltimate.Core.Enums
{
    // Determines which stats are used for damage calculation.
    public enum MoveCategory
    {
        // Uses Attack vs Defense
        Physical,
        
        // Uses SpAttack vs SpDefense
        Special,
        
        // No damage, only applies effects (status, stat changes, etc.)
        Status
    }
}

