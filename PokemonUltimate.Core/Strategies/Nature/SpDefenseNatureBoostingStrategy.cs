namespace PokemonUltimate.Core.Strategies.Nature
{
    /// <summary>
    /// Strategy for Special Defense-boosting nature (Calm).
    /// </summary>
    public class SpDefenseNatureBoostingStrategy : INatureBoostingStrategy
    {
        public Data.Enums.Nature GetBoostingNature()
        {
            return Data.Enums.Nature.Calm;
        }
    }
}
