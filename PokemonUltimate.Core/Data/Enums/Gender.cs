namespace PokemonUltimate.Core.Data.Enums
{
    /// <summary>
    /// Gender of a Pokemon. Affects some evolutions, moves, and breeding.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.10: Enums & Constants
    /// **Documentation**: See `docs/features/1-game-data/1.10-enums-constants/README.md`
    /// </remarks>
    public enum Gender
    {
        /// <summary>Male Pokemon.</summary>
        Male,

        /// <summary>Female Pokemon.</summary>
        Female,

        /// <summary>Pokemon with no gender (Magnemite, Ditto, etc.).</summary>
        Genderless
    }
}

