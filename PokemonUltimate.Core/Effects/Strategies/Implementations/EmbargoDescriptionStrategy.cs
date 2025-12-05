using PokemonUltimate.Core.Effects.Strategies;

namespace PokemonUltimate.Core.Effects.Strategies.Implementations
{
    public class EmbargoDescriptionStrategy : IMoveRestrictionDescriptionStrategy
    {
        public string GetDescription(int duration)
        {
            return $"Target cannot use items for {duration} turns.";
        }
    }
}
