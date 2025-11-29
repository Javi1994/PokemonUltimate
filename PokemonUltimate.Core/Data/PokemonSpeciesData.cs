using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Interfaces;

namespace PokemonUltimate.Core.Data
{
    /// <summary>
    /// Blueprint for a Pokemon species (immutable data).
    /// Pokemon can be retrieved by Name (unique string) or PokedexNumber (unique int).
    /// This is the "Species" data - shared by all Pokemon of the same kind.
    /// </summary>
    public class PokemonSpeciesData : IIdentifiable
    {
        /// <summary>
        /// Unique identifier - the Pokemon's name (e.g., "Pikachu", "Charizard").
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// National Pokedex number (e.g., 25 for Pikachu, 6 for Charizard).
        /// </summary>
        public int PokedexNumber { get; set; }

        /// <summary>
        /// Primary type of the Pokemon (every Pokemon has one).
        /// </summary>
        public PokemonType PrimaryType { get; set; }

        /// <summary>
        /// Secondary type of the Pokemon (optional, null if mono-type).
        /// </summary>
        public PokemonType? SecondaryType { get; set; }

        /// <summary>
        /// Base stats used for calculating actual stats.
        /// </summary>
        public BaseStats BaseStats { get; set; } = new BaseStats();

        /// <summary>
        /// IIdentifiable implementation - Name serves as the unique ID.
        /// </summary>
        public string Id => Name;

        /// <summary>
        /// Returns true if this Pokemon has two types.
        /// </summary>
        public bool IsDualType => SecondaryType.HasValue;

        /// <summary>
        /// Checks if this Pokemon has a specific type (primary or secondary).
        /// </summary>
        public bool HasType(PokemonType type)
        {
            return PrimaryType == type || SecondaryType == type;
        }
    }
}
