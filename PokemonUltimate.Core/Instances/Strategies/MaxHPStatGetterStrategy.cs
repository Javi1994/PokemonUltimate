namespace PokemonUltimate.Core.Instances.Strategies
{
    public class MaxHPStatGetterStrategy : IPokemonStatGetterStrategy
    {
        public int GetStat(PokemonInstance pokemon)
        {
            return pokemon.MaxHP;
        }
    }
}
