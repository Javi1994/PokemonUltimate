namespace PokemonUltimate.Core.Strategies.Nature
{
    /// <summary>
    /// Strategy for Special Attack-boosting nature (Modest).
    /// </summary>
    public class SpAttackNatureBoostingStrategy : INatureBoostingStrategy
    {
        public Data.Enums.Nature GetBoostingNature()
        {
            return Data.Enums.Nature.Modest;
        }
    }
}
