namespace PokemonUltimate.Core.Data.Enums
{
    /// <summary>
    /// Terrain conditions that affect grounded Pokemon.
    /// Terrains last 5 turns (8 with Terrain Extender).
    /// Only affects Pokemon that are "grounded" (not Flying, Levitate, or holding Air Balloon).
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.10: Enums & Constants
    /// **Documentation**: See `docs/features/1-game-data/1.10-enums-constants/README.md`
    /// </remarks>
    public enum Terrain
    {
        /// <summary>No terrain effect.</summary>
        None,

        /// <summary>
        /// Grassy Terrain - Boosts Grass moves 1.3x, heals grounded Pokemon 1/16 HP,
        /// halves Earthquake/Bulldoze/Magnitude damage.
        /// </summary>
        Grassy,

        /// <summary>
        /// Electric Terrain - Boosts Electric moves 1.3x, prevents Sleep for grounded Pokemon.
        /// </summary>
        Electric,

        /// <summary>
        /// Psychic Terrain - Boosts Psychic moves 1.3x, blocks priority moves against grounded Pokemon.
        /// </summary>
        Psychic,

        /// <summary>
        /// Misty Terrain - Halves Dragon-type damage to grounded Pokemon, prevents status conditions.
        /// </summary>
        Misty,
    }
}

