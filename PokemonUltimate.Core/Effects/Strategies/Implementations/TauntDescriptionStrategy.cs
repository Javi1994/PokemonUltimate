using PokemonUltimate.Core.Effects.Strategies;

namespace PokemonUltimate.Core.Effects.Strategies.Implementations
{
    public class TauntDescriptionStrategy : IMoveRestrictionDescriptionStrategy
    {
        public string GetDescription(int duration)
        {
            return $"Target can only use damaging moves for {duration} turns.";
        }
    }
}
