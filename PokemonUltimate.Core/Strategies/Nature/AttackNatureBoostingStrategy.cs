using PokemonUltimate.Core.Strategies.Nature.Definition;

namespace PokemonUltimate.Core.Strategies.Nature
{
    /// <summary>
    /// Strategy for Attack-boosting nature (Adamant).
    /// </summary>
    public class AttackNatureBoostingStrategy : INatureBoostingStrategy
    {
        public Data.Enums.Nature GetBoostingNature()
        {
            return Data.Enums.Nature.Adamant;
        }
    }
}
