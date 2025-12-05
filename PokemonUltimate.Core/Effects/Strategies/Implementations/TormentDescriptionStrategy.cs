using PokemonUltimate.Core.Effects.Strategies;

namespace PokemonUltimate.Core.Effects.Strategies.Implementations
{
    public class TormentDescriptionStrategy : IMoveRestrictionDescriptionStrategy
    {
        public string GetDescription(int duration)
        {
            return "Target cannot use same move consecutively.";
        }
    }
}
