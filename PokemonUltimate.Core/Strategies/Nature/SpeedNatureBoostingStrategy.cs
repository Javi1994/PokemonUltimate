namespace PokemonUltimate.Core.Strategies.Nature
{
    /// <summary>
    /// Strategy for Speed-boosting nature (Jolly).
    /// </summary>
    public class SpeedNatureBoostingStrategy : INatureBoostingStrategy
    {
        public Data.Enums.Nature GetBoostingNature()
        {
            return Data.Enums.Nature.Jolly;
        }
    }
}
