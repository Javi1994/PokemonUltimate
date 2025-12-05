namespace PokemonUltimate.Core.Instances.Strategies
{
    public class SpeedStatGetterStrategy : IPokemonStatGetterStrategy
    {
        public int GetStat(PokemonInstance pokemon)
        {
            return pokemon.Speed;
        }
    }
}
