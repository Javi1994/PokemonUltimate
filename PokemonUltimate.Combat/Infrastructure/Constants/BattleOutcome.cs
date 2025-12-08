namespace PokemonUltimate.Combat.Infrastructure.Constants
{
    /// <summary>
    /// Represents the possible outcomes of a battle.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `docs/features/2-combat-system/2.6-combat-engine/architecture.md`
    /// </remarks>
    public enum BattleOutcome
    {
        /// <summary>
        /// Battle is still ongoing.
        /// </summary>
        Ongoing,

        /// <summary>
        /// Player won the battle.
        /// </summary>
        Victory,

        /// <summary>
        /// Player lost the battle (all Pokemon fainted).
        /// </summary>
        Defeat,

        /// <summary>
        /// Both sides were defeated simultaneously (e.g., Explosion double KO).
        /// </summary>
        Draw,

        /// <summary>
        /// Player fled from the battle.
        /// </summary>
        Fled,

        /// <summary>
        /// Wild Pokemon was caught (ends battle).
        /// </summary>
        Caught
    }
}

