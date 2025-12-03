using PokemonUltimate.Core.Instances;

namespace PokemonUltimate.Core.Evolution.Conditions
{
    /// <summary>
    /// Evolution condition: Use a specific evolution item (stone, etc.).
    /// This condition returns false by default - items must be used explicitly.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.7: Evolution System
    /// **Documentation**: See `docs/features/1-game-data/1.7-evolution-system/architecture.md`
    /// </remarks>
    public class ItemCondition : IEvolutionCondition
    {
        public EvolutionConditionType ConditionType => EvolutionConditionType.UseItem;
        public string Description => $"Use {ItemName}";

        /// <summary>
        /// Name of the required item.
        /// </summary>
        public string ItemName { get; set; } = string.Empty;

        public ItemCondition() { }

        public ItemCondition(string itemName)
        {
            ItemName = itemName;
        }

        /// <summary>
        /// Item conditions are never automatically met - they require explicit item use.
        /// Use IsMet(pokemon, itemUsed) overload for item evolution checks.
        /// </summary>
        public bool IsMet(PokemonInstance pokemon)
        {
            // Item conditions require explicit item use
            return false;
        }

        /// <summary>
        /// Checks if the condition is met when a specific item is used.
        /// </summary>
        public bool IsMet(PokemonInstance pokemon, string itemUsed)
        {
            return pokemon != null && 
                   !string.IsNullOrEmpty(itemUsed) && 
                   itemUsed.Equals(ItemName, System.StringComparison.OrdinalIgnoreCase);
        }
    }
}
