namespace PokemonUltimate.Combat.Actions
{
    /// <summary>
    /// Types of actions a player can choose during battle.
    /// </summary>
    public enum BattleActionType
    {
        /// <summary>
        /// Use a move to attack or apply an effect.
        /// </summary>
        Fight,

        /// <summary>
        /// Switch to a different Pokemon.
        /// </summary>
        Switch,

        /// <summary>
        /// Use an item (Potion, Poke Ball, etc.).
        /// </summary>
        Item, // Future feature

        /// <summary>
        /// Attempt to flee from battle.
        /// </summary>
        Run // Future feature
    }
}

