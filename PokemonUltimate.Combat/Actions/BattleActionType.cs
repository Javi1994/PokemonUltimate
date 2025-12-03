namespace PokemonUltimate.Combat.Actions
{
    /// <summary>
    /// Types of actions a player can choose during battle.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.5: Combat Actions
    /// **Documentation**: See `docs/features/2-combat-system/2.5-combat-actions/architecture.md`
    /// </remarks>
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

