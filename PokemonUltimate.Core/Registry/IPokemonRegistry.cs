using PokemonUltimate.Core.Blueprints;

namespace PokemonUltimate.Core.Registry
{
    /// <summary>
    /// Contract for a registry that handles Pokemon-specific lookups.
    /// Supports lookup by Name (string) or PokedexNumber (int).
    /// </summary>
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

