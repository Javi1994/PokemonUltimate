using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Core.Factories.Strategies.NatureBoosting
{
    /// <summary>
    /// Strategy for Defense-boosting nature (Bold).
    /// </summary>
    public class DefenseNatureBoostingStrategy : INatureBoostingStrategy
    {
        public Nature GetBoostingNature()
        {
            return Nature.Bold;
        }
    }
}
