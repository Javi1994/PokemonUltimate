namespace PokemonUltimate.Core.Evolution.Conditions
{
    /// <summary>
    /// Evolution condition: Use a specific evolution item (stone, etc.).
    /// </summary>
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
    }
}

