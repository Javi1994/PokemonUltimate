using PokemonUltimate.Core.Domain.Instances.Pokemon;
using PokemonUltimate.Core.Strategies.Stats.Definition;

namespace PokemonUltimate.Core.Strategies.Stats
{
    public class SpeedStatGetterStrategy : IPokemonStatGetterStrategy
    {
        public int GetStat(PokemonInstance pokemon)
        {
            return pokemon.Speed;
        }
    }
}
