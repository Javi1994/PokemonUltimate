using PokemonUltimate.Core.Strategies.Effect.Definition;

namespace PokemonUltimate.Core.Strategies.Effect
{
    public class TauntDescriptionStrategy : IMoveRestrictionDescriptionStrategy
    {
        public string GetDescription(int duration)
        {
            return $"Target can only use damaging moves for {duration} turns.";
        }
    }
}
