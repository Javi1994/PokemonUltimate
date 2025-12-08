using PokemonUltimate.Core.Domain.Instances.Pokemon;
using PokemonUltimate.Core.Strategies.Stats.Definition;

namespace PokemonUltimate.Core.Strategies.Stats
{
    public class AccuracyStatGetterStrategy : IPokemonStatGetterStrategy
    {
        public int GetStat(PokemonInstance pokemon)
        {
            return 100; // Base accuracy is 100%
        }
    }
}
