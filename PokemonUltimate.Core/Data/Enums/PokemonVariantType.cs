namespace PokemonUltimate.Core.Data.Enums
{
    /// <summary>
    /// Types of Pokemon variants (Mega Evolution, Dinamax, Terracristalización, Regional Forms).
    /// Variants are implemented as separate Pokemon species with potentially different stats, types, or visual appearance.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.18: Variants System
    /// **Documentation**: See `docs/features/1-game-data/1.18-variants-system/architecture.md`
    /// </remarks>
    public enum PokemonVariantType
    {
        /// <summary>
        /// Mega Evolution variant - permanent form with stat changes, type changes, and ability changes.
        /// Example: Mega Charizard X (Fire/Dragon, higher BST)
        /// </summary>
        Mega,

        /// <summary>
        /// Dinamax variant - permanent form with significantly increased HP (typically 2x base HP).
        /// Example: Charizard Dinamax (156 HP vs 78 base)
        /// </summary>
        Dinamax,

        /// <summary>
        /// Terracristalización variant - permanent form that changes to mono-type (Tera type).
        /// Example: Charizard Tera Fire (mono-Fire type)
        /// </summary>
        Tera,

        /// <summary>
        /// Regional form variant - permanent form with potentially different types, stats, abilities, but primarily visual differences.
        /// Examples: Alolan Vulpix (Ice type), Galarian Meowth (Steel type), Hisuian Zorua (Normal/Ghost)
        /// Note: Some regional forms have identical stats to base form (purely visual), others have different stats/types.
        /// </summary>
        Regional,

        /// <summary>
        /// Cosmetic variant - purely visual changes, no gameplay impact (same stats, types, abilities).
        /// Examples: Pikachu Libre, Spiky-eared Pichu, Partner Pikachu
        /// </summary>
        Cosmetic
    }
}

