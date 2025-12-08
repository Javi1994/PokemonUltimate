using PokemonUltimate.Core.Strategies.Effect.Definition;

namespace PokemonUltimate.Core.Strategies.Effect
{
    public class TormentDescriptionStrategy : IMoveRestrictionDescriptionStrategy
    {
        public string GetDescription(int duration)
        {
            return "Target cannot use same move consecutively.";
        }
    }
}
