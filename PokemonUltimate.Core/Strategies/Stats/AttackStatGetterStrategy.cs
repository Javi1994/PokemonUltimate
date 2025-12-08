using PokemonUltimate.Core.Domain.Instances.Pokemon;
using PokemonUltimate.Core.Strategies.Stats.Definition;

namespace PokemonUltimate.Core.Strategies.Stats
{
    public class AttackStatGetterStrategy : IPokemonStatGetterStrategy
    {
        public int GetStat(PokemonInstance pokemon)
        {
            return pokemon.Attack;
        }
    }
}
