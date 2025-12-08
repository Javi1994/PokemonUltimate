using PokemonUltimate.Core.Strategies.Nature.Definition;

namespace PokemonUltimate.Core.Strategies.Nature
{
    /// <summary>
    /// Strategy for Defense-boosting nature (Bold).
    /// </summary>
    public class DefenseNatureBoostingStrategy : INatureBoostingStrategy
    {
        public Data.Enums.Nature GetBoostingNature()
        {
            return Data.Enums.Nature.Bold;
        }
    }
}
