using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Core.Factories.Strategies.NatureBoosting
{
    /// <summary>
    /// Default strategy that returns a neutral nature (Hardy).
    /// Used for HP or unknown stats.
    /// </summary>
    public class DefaultNatureBoostingStrategy : INatureBoostingStrategy
    {
        public Nature GetBoostingNature()
        {
            return Nature.Hardy;
        }
    }
}
