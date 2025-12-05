using PokemonUltimate.Core.Effects.Strategies;

namespace PokemonUltimate.Core.Effects.Strategies.Implementations
{
    public class ImprisonDescriptionStrategy : IMoveRestrictionDescriptionStrategy
    {
        public string GetDescription(int duration)
        {
            return "Opponents cannot use moves the user knows.";
        }
    }
}
