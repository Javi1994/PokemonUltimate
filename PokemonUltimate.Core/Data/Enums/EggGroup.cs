namespace PokemonUltimate.Core.Data.Enums
{
    /// <summary>
    /// Egg groups determine breeding compatibility between Pokemon species.
    /// Two Pokemon can breed if they share at least one egg group (unless one is Ditto).
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.1: Pokemon Data (extensions - Breeding System)
    /// **Documentation**: See `docs/features/1-game-data/roadmap.md#phase-4-optional-enhancements-low-priority`
    /// </remarks>
    public enum EggGroup
    {
        /// <summary>Monster egg group (e.g., Charizard, Snorlax).</summary>
        Monster,

        /// <summary>Water 1 egg group (e.g., Squirtle, Poliwag).</summary>
        Water1,

        /// <summary>Bug egg group (e.g., Caterpie, Scyther).</summary>
        Bug,

        /// <summary>Flying egg group (e.g., Pidgey, Spearow).</summary>
        Flying,

        /// <summary>Field egg group (e.g., Pikachu, Growlithe).</summary>
        Field,

        /// <summary>Fairy egg group (e.g., Clefairy, Togepi).</summary>
        Fairy,

        /// <summary>Grass egg group (e.g., Bulbasaur, Oddish).</summary>
        Grass,

        /// <summary>Human-Like egg group (e.g., Machop, Mr. Mime).</summary>
        HumanLike,

        /// <summary>Mineral egg group (e.g., Geodude, Magnemite).</summary>
        Mineral,

        /// <summary>Amorphous egg group (e.g., Gastly, Ditto).</summary>
        Amorphous,

        /// <summary>Dragon egg group (e.g., Dratini, Bagon).</summary>
        Dragon,

        /// <summary>Ditto egg group - can breed with any Pokemon (except Undiscovered).</summary>
        Ditto,

        /// <summary>Undiscovered egg group - cannot breed (e.g., legendaries, baby Pokemon).</summary>
        Undiscovered
    }
}

