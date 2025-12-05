using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Core.Factories.Strategies.NatureBoosting
{
    /// <summary>
    /// Strategy for Special Defense-boosting nature (Calm).
    /// </summary>
    public class SpDefenseNatureBoostingStrategy : INatureBoostingStrategy
    {
        public Nature GetBoostingNature()
        {
            return Nature.Calm;
        }
    }
}
