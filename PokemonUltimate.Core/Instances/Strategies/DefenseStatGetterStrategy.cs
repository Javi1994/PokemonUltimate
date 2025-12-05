namespace PokemonUltimate.Core.Instances.Strategies
{
    public class DefenseStatGetterStrategy : IPokemonStatGetterStrategy
    {
        public int GetStat(PokemonInstance pokemon)
        {
            return pokemon.Defense;
        }
    }
}
