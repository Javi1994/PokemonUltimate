namespace PokemonUltimate.Core.Evolution.Conditions
{
    /// <summary>
    /// Evolution condition: Pokemon must be traded.
    /// </summary>
    public class TradeCondition : IEvolutionCondition
    {
        public EvolutionConditionType ConditionType => EvolutionConditionType.Trade;
        public string Description => "Trade";
    }
}

