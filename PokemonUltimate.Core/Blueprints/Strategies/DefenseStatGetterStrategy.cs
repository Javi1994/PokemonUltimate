namespace PokemonUltimate.Core.Blueprints.Strategies
{
    public class DefenseStatGetterStrategy : IStatGetterStrategy
    {
        public int GetStat(BaseStats baseStats)
        {
            return baseStats.Defense;
        }
    }
}
