using PokemonUltimate.Core.Domain.Instances.Pokemon;

namespace PokemonUltimate.Core.Strategies.Stats
{
    public class MaxHPStatGetterStrategy : IPokemonStatGetterStrategy
    {
        public int GetStat(PokemonInstance pokemon)
        {
            return pokemon.MaxHP;
        }
    }
}
