namespace PokemonUltimate.Core.Enums
{
    /// <summary>
    /// Major status conditions that persist outside of battle.
    /// Only one can be active at a time on a Pokemon.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.10: Enums & Constants
    /// **Documentation**: See `docs/features/1-game-data/1.10-enums-constants/README.md`
    /// </remarks>
    public enum PersistentStatus
    {
        /// <summary>No status condition.</summary>
        None = 0,

        /// <summary>Reduces Attack by 50%, deals 1/16 max HP damage per turn.</summary>
        Burn = 1,

        /// <summary>Reduces Speed by 50%, 25% chance to be fully paralyzed each turn.</summary>
        Paralysis = 2,

        /// <summary>Cannot act. Wakes up after 1-3 turns.</summary>
        Sleep = 3,

        /// <summary>Deals 1/8 max HP damage per turn.</summary>
        Poison = 4,

        /// <summary>Deals increasing damage (1/16, 2/16, 3/16...) per turn.</summary>
        BadlyPoisoned = 5,

        /// <summary>Cannot act. 20% chance to thaw each turn, or thawed by Fire moves.</summary>
        Freeze = 6
    }
}
