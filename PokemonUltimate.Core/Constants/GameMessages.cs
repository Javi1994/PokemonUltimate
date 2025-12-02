namespace PokemonUltimate.Core.Constants
{
    /// <summary>
    /// In-game messages for battle feedback and UI.
    /// These are the messages shown to players during gameplay.
    /// </summary>
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

        #endregion
    }
}

