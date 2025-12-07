using PokemonUltimate.Core.Domain.Instances.Pokemon;

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
