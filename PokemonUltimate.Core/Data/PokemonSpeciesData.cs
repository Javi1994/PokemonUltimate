using PokemonUltimate.Core.Interfaces;

namespace PokemonUltimate.Core.Data
{
    // Blueprint for a Pokemon species (immutable data).
    // Pokemon can be retrieved by Name (unique string) or PokedexNumber (unique int).
    public class PokemonSpeciesData : IIdentifiable
    {
        // Unique identifier - the Pokemon's name (e.g., "Pikachu", "Charizard")
        public string Name { get; set; } = string.Empty;

        // National Pokedex number (e.g., 25 for Pikachu, 6 for Charizard)
        public int PokedexNumber { get; set; }

        // IIdentifiable implementation - Name serves as the unique ID
        public string Id => Name;

        // We will add Stats, Types, etc. later as we expand.
    }
}
