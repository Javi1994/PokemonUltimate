namespace PokemonUltimate.Core.Blueprints.Strategies
{
    public class SpAttackStatGetterStrategy : IStatGetterStrategy
    {
        public int GetStat(BaseStats baseStats)
        {
            return baseStats.SpAttack;
        }
    }
}
