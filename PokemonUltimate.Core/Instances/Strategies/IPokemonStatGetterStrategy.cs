namespace PokemonUltimate.Core.Instances.Strategies
{
    /// <summary>
    /// Strategy interface for getting stat values from PokemonInstance.
    /// </summary>
    public interface IPokemonStatGetterStrategy
    {
        /// <summary>
        /// Gets the stat value from the PokemonInstance.
        /// </summary>
        int GetStat(PokemonInstance pokemon);
    }
}
