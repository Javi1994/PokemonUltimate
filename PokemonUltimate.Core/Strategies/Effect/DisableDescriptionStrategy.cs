

namespace PokemonUltimate.Core.Strategies.Effect
{
    public class DisableDescriptionStrategy : IMoveRestrictionDescriptionStrategy
    {
        public string GetDescription(int duration)
        {
            return $"Disables target's last move for {duration} turns.";
        }
    }
}
