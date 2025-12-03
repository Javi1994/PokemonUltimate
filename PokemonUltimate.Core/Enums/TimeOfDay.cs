namespace PokemonUltimate.Core.Enums
{
    /// <summary>
    /// Time of day for evolution conditions and other game mechanics.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.10: Enums & Constants
    /// **Documentation**: See `docs/features/1-game-data/1.10-enums-constants/README.md`
    /// </remarks>
    public enum TimeOfDay
    {
        /// <summary>Morning (4:00 - 10:00).</summary>
        Morning,
        
        /// <summary>Day (10:00 - 18:00).</summary>
        Day,
        
        /// <summary>Evening (18:00 - 20:00).</summary>
        Evening,
        
        /// <summary>Night (20:00 - 4:00).</summary>
        Night
    }
}

