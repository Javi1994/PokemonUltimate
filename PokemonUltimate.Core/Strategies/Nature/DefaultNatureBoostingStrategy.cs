namespace PokemonUltimate.Core.Strategies.Nature
{
    /// <summary>
    /// Default strategy that returns a neutral nature (Hardy).
    /// Used for HP or unknown stats.
    /// </summary>
    public class DefaultNatureBoostingStrategy : INatureBoostingStrategy
    {
        public Data.Enums.Nature GetBoostingNature()
        {
            return Data.Enums.Nature.Hardy;
        }
    }
}
