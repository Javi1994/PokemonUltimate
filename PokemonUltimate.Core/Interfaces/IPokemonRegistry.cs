using PokemonUltimate.Core.Data;

namespace PokemonUltimate.Core.Interfaces
{
    // Contract for a registry that handles Pokemon-specific lookups.
    // Supports lookup by Name (string) or PokedexNumber (int).
    public interface IPokemonRegistry : IDataRegistry<PokemonSpeciesData>
    {
        // Retrieve Pokemon by its unique name (e.g., "Pikachu")
        PokemonSpeciesData GetByName(string name);

        // Retrieve Pokemon by its Pokedex number (e.g., 25)
        PokemonSpeciesData GetByPokedexNumber(int number);

        // Check if a Pokemon with the given Pokedex number exists
        bool ExistsByPokedexNumber(int number);
    }
}
