using PokemonUltimate.Core.Domain.Instances.Pokemon;

namespace PokemonUltimate.Core.Strategies.Stats.Definition
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
