using PokemonUltimate.Core.Domain.Instances.Pokemon;

namespace PokemonUltimate.Core.Strategies.Stats
{
    public class DefenseStatGetterStrategy : IPokemonStatGetterStrategy
    {
        public int GetStat(PokemonInstance pokemon)
        {
            return pokemon.Defense;
        }
    }
}
