namespace PokemonUltimate.Core.Constants
{
    /// <summary>
    /// In-game messages for battle feedback and UI.
    /// These are the messages shown to players during gameplay.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.10: Enums & Constants
    /// **Documentation**: See `docs/features/1-game-data/1.10-enums-constants/README.md`
    /// </remarks>
    public static class GameMessages
    {
        #region Type Effectiveness

        public const string NoEffect = "It has no effect...";
        public const string SuperEffective4x = "It's super effective! (4x)";
        public const string SuperEffective = "It's super effective!";
        public const string NotVeryEffective025x = "It's not very effective... (0.25x)";
        public const string NotVeryEffective = "It's not very effective...";
        public const string NormalEffectiveness = null;

        #endregion

        #region Multi-Hit

        public const string HitsExactly = "Hits {0} times.";
        public const string HitsRange = "Hits {0}-{1} times.";

        #endregion

        #region Move Execution

        public const string MoveNoPP = "{0} has no PP left!";
        public const string MoveFlinched = "{0} flinched and couldn't move!";
        public const string MoveAsleep = "{0} is fast asleep.";
        public const string MoveFrozen = "{0} is frozen solid!";
        public const string MoveParalyzed = "{0} is paralyzed! It can't move!";
        public const string MoveMissed = "The attack missed!";
        public const string MoveProtected = "{0} protected itself!";
        public const string MoveProtectFailed = "{0} avoided the attack!";
        public const string MoveCountered = "{0} countered the attack!";
        public const string MoveFocusing = "{0} is tightening its focus!";
        public const string MoveFocusLost = "{0} lost its focus!";
        public const string MoveSemiInvulnerable = "{0} {1}!";

        #endregion

        #region Status Effects

        public const string StatusBurnDamage = "{0} is hurt by its burn!";
        public const string StatusPoisonDamage = "{0} is hurt by poison!";

        #endregion

        #region Weather Effects

        public const string WeatherSandstormDamage = "{0} is buffeted by the sandstorm!";
        public const string WeatherHailDamage = "{0} is pelted by hail!";

        #endregion

        #region Terrain Effects

        public const string TerrainHealing = "{0} is healed by the {1}!";

        #endregion

        #region Entry Hazards

        public const string HazardSpikesDamage = "{0} is hurt by Spikes!";
        public const string HazardStealthRockDamage = "{0} is hurt by Stealth Rock!";
        public const string HazardToxicSpikesAbsorbed = "{0} absorbed the Toxic Spikes!";
        public const string HazardToxicSpikesStatus = "{0} was poisoned by Toxic Spikes!";
        public const string HazardStickyWebSpeed = "{0} was caught in a Sticky Web!";

        #endregion

        #region Abilities & Items

        public const string AbilityActivated = "{0}'s {1}!";
        public const string ItemActivated = "{0}'s {1}!";
        public const string MoveFailed = "{0} can't use {1}!";
        public const string TruantLoafing = "{0} is loafing around!";

        #endregion
    }
}

