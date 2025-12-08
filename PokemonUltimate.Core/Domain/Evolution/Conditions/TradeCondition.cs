using PokemonUltimate.Core.Domain.Evolution.Definition;
using PokemonInstance = PokemonUltimate.Core.Domain.Instances.Pokemon.PokemonInstance;

namespace PokemonUltimate.Core.Domain.Evolution.Conditions
{
    /// <summary>
    /// Evolution condition: Pokemon must be traded.
    /// This condition returns false by default - trade must happen explicitly.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.11: Evolution System
    /// **Documentation**: See `docs/features/1-game-data/1.11-evolution-system/README.md`
    /// </remarks>
    public class TradeCondition : IEvolutionCondition
    {
        public EvolutionConditionType ConditionType => EvolutionConditionType.Trade;
        public string Description => "Trade";

        /// <summary>
        /// Trade conditions are never automatically met - they require explicit trading.
        /// </summary>
        public bool IsMet(PokemonInstance pokemon)
        {
            // Trade conditions require explicit trading
            return false;
        }

        /// <summary>
        /// Checks if the condition is met after a trade occurs.
        /// </summary>
        public bool IsMet(PokemonInstance pokemon, bool wasTraded)
        {
            return pokemon != null && wasTraded;
        }
    }
}
