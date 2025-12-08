using PokemonUltimate.Core.Strategies.Effect.Definition;

namespace PokemonUltimate.Core.Strategies.Effect
{
    public class ThroatChopDescriptionStrategy : IMoveRestrictionDescriptionStrategy
    {
        public string GetDescription(int duration)
        {
            return $"Target cannot use sound moves for {duration} turns.";
        }
    }
}
