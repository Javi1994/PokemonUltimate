namespace PokemonUltimate.Core.Blueprints.Strategies
{
    /// <summary>
    /// Strategy interface for getting stat values from BaseStats.
    /// </summary>
    public interface IStatGetterStrategy
    {
        /// <summary>
        /// Gets the stat value from the BaseStats instance.
        /// </summary>
        int GetStat(BaseStats baseStats);
    }
}
