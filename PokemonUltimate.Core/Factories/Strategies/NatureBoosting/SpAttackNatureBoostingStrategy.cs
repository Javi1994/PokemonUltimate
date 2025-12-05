using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Core.Factories.Strategies.NatureBoosting
{
    /// <summary>
    /// Strategy for Special Attack-boosting nature (Modest).
    /// </summary>
    public class SpAttackNatureBoostingStrategy : INatureBoostingStrategy
    {
        public Nature GetBoostingNature()
        {
            return Nature.Modest;
        }
    }
}
