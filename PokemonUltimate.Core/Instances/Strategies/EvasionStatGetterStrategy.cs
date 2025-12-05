namespace PokemonUltimate.Core.Instances.Strategies
{
    public class EvasionStatGetterStrategy : IPokemonStatGetterStrategy
    {
        public int GetStat(PokemonInstance pokemon)
        {
            return 100; // Base evasion is 100%
        }
    }
}
