namespace PokemonUltimate.Core.Instances.Strategies
{
    public class AttackStatGetterStrategy : IPokemonStatGetterStrategy
    {
        public int GetStat(PokemonInstance pokemon)
        {
            return pokemon.Attack;
        }
    }
}
