namespace PokemonUltimate.Core.Blueprints.Strategies
{
    public class AttackStatGetterStrategy : IStatGetterStrategy
    {
        public int GetStat(BaseStats baseStats)
        {
            return baseStats.Attack;
        }
    }
}
