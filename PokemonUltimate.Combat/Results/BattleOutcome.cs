namespace PokemonUltimate.Combat
{
    /// <summary>
    /// Represents the possible outcomes of a battle.
    /// </summary>
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

