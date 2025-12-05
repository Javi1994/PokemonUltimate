using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Core.Factories.Strategies.NatureBoosting
{
    /// <summary>
    /// Strategy for Attack-boosting nature (Adamant).
    /// </summary>
    public class AttackNatureBoostingStrategy : INatureBoostingStrategy
    {
        public Nature GetBoostingNature()
        {
            return Nature.Adamant;
        }
    }
}
