namespace PokemonUltimate.Core.Instances.Strategies
{
    public class AccuracyStatGetterStrategy : IPokemonStatGetterStrategy
    {
        public int GetStat(PokemonInstance pokemon)
        {
            return 100; // Base accuracy is 100%
        }
    }
}
