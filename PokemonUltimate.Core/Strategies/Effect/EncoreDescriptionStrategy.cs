

namespace PokemonUltimate.Core.Strategies.Effect
{
    public class EncoreDescriptionStrategy : IMoveRestrictionDescriptionStrategy
    {
        public string GetDescription(int duration)
        {
            return $"Forces target to repeat last move for {duration} turns.";
        }
    }
}
