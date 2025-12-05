namespace PokemonUltimate.Core.Blueprints.Strategies
{
    public class SpeedStatGetterStrategy : IStatGetterStrategy
    {
        public int GetStat(BaseStats baseStats)
        {
            return baseStats.Speed;
        }
    }
}
