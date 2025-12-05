namespace PokemonUltimate.Core.Instances.Strategies
{
    public class SpAttackStatGetterStrategy : IPokemonStatGetterStrategy
    {
        public int GetStat(PokemonInstance pokemon)
        {
            return pokemon.SpAttack;
        }
    }
}
