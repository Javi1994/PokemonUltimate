using PokemonUltimate.Core.Effects.Strategies;

namespace PokemonUltimate.Core.Effects.Strategies.Implementations
{
    public class ThroatChopDescriptionStrategy : IMoveRestrictionDescriptionStrategy
    {
        public string GetDescription(int duration)
        {
            return $"Target cannot use sound moves for {duration} turns.";
        }
    }
}
