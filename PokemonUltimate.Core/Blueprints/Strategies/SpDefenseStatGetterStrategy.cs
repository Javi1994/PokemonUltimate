namespace PokemonUltimate.Core.Blueprints.Strategies
{
    public class SpDefenseStatGetterStrategy : IStatGetterStrategy
    {
        public int GetStat(BaseStats baseStats)
        {
            return baseStats.SpDefense;
        }
    }
}
