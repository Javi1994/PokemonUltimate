

namespace PokemonUltimate.Core.Strategies.Effect
{
    public class EmbargoDescriptionStrategy : IMoveRestrictionDescriptionStrategy
    {
        public string GetDescription(int duration)
        {
            return $"Target cannot use items for {duration} turns.";
        }
    }
}
