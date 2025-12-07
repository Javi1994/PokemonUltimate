

namespace PokemonUltimate.Core.Strategies.Effect
{
    public class HealBlockDescriptionStrategy : IMoveRestrictionDescriptionStrategy
    {
        public string GetDescription(int duration)
        {
            return $"Target cannot heal for {duration} turns.";
        }
    }
}
