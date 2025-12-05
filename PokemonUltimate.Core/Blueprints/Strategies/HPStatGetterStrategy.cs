namespace PokemonUltimate.Core.Blueprints.Strategies
{
    public class HPStatGetterStrategy : IStatGetterStrategy
    {
        public int GetStat(BaseStats baseStats)
        {
            return baseStats.HP;
        }
    }
}
