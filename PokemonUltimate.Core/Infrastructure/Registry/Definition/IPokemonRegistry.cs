using PokemonUltimate.Core.Data.Blueprints;

namespace PokemonUltimate.Core.Infrastructure.Registry.Definition
{
    /// <summary>
    /// Contract for a registry that handles Pokemon-specific lookups.
    /// Supports lookup by Name (string) or PokedexNumber (int).
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.17: Registry System
    /// **Documentation**: See `docs/features/1-game-data/1.17-registry-system/README.md`
    /// </remarks>
    public interface IPokemonRegistry : IDataRegistry<PokemonSpeciesData>
    {
        /// <summary>
        /// Retrieve Pokemon by its unique name (e.g., "Pikachu")
        /// </summary>
        PokemonSpeciesData GetByName(string name);

        /// <summary>
        /// Retrieve Pokemon by its Pokedex number (e.g., 25)
        /// </summary>
        PokemonSpeciesData GetByPokedexNumber(int number);

        /// <summary>
        /// Check if a Pokemon with the given Pokedex number exists
        /// </summary>
        bool ExistsByPokedexNumber(int number);
    }
}

