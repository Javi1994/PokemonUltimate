namespace PokemonUltimate.Core.Instances.Strategies
{
    public class SpDefenseStatGetterStrategy : IPokemonStatGetterStrategy
    {
        public int GetStat(PokemonInstance pokemon)
        {
            return pokemon.SpDefense;
        }
    }
}
