using PokemonUltimate.Core.Strategies.Effect.Definition;

namespace PokemonUltimate.Core.Strategies.Effect
{
    public class ImprisonDescriptionStrategy : IMoveRestrictionDescriptionStrategy
    {
        public string GetDescription(int duration)
        {
            return "Opponents cannot use moves the user knows.";
        }
    }
}
