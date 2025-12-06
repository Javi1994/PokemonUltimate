namespace PokemonUltimate.Core.Enums
{
    /// <summary>
    /// Experience growth rate curves for Pokemon.
    /// Different Pokemon species use different formulas to calculate EXP needed per level.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.14: Enums & Constants
    /// **Documentation**: See `docs/features/1-game-data/1.14-enums-constants/README.md`
    /// </remarks>
    public enum GrowthRate
    {
        /// <summary>
        /// Fast growth rate: Level^3 * 0.8
        /// Used by: Pikachu, Clefairy, Jigglypuff
        /// </summary>
        Fast,

        /// <summary>
        /// Medium Fast growth rate: Level^3 (default, most Pokemon)
        /// Used by: Most Pokemon
        /// </summary>
        MediumFast,

        /// <summary>
        /// Medium Slow growth rate: Complex formula
        /// Used by: Starters (Bulbasaur, Charmander, Squirtle)
        /// </summary>
        MediumSlow,

        /// <summary>
        /// Slow growth rate: Level^3 * 1.25
        /// Used by: Legendaries (Mewtwo, Mew)
        /// </summary>
        Slow,

        /// <summary>
        /// Erratic growth rate: Complex formula
        /// Used by: Some Pokemon like Flareon, Vaporeon
        /// </summary>
        Erratic,

        /// <summary>
        /// Fluctuating growth rate: Complex formula
        /// Used by: Some Pokemon like Magikarp
        /// </summary>
        Fluctuating
    }
}
