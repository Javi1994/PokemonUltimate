namespace PokemonUltimate.Core.Data.Enums
{
    /// <summary>
    /// Types of entry hazards that can be placed on a side of the field.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.10: Enums & Constants
    /// **Documentation**: See `docs/features/1-game-data/1.10-enums-constants/README.md`
    /// </remarks>
    public enum HazardType
    {
        /// <summary>No hazard.</summary>
        None,

        /// <summary>
        /// Stealth Rock - Deals damage based on type effectiveness vs Rock.
        /// Single layer, affects all Pokemon including Flying.
        /// </summary>
        StealthRock,

        /// <summary>
        /// Spikes - Deals percentage HP damage (1-3 layers).
        /// Does not affect Flying types or Levitate.
        /// </summary>
        Spikes,

        /// <summary>
        /// Toxic Spikes - Applies Poison (1 layer) or Badly Poisoned (2 layers).
        /// Poison types absorb on entry. Does not affect Flying/Levitate.
        /// </summary>
        ToxicSpikes,

        /// <summary>
        /// Sticky Web - Lowers Speed by 1 stage on entry.
        /// Does not affect Flying types or Levitate.
        /// </summary>
        StickyWeb,
    }
}

