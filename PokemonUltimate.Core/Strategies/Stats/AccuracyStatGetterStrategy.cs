using PokemonUltimate.Core.Domain.Instances.Pokemon;

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
