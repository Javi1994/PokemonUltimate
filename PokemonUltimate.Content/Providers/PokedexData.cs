using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Content.Providers
{
    /// <summary>
    /// Immutable data structure containing Pokedex information for a Pokemon species.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.19: Pokedex Fields
    /// **Documentation**: See `docs/features/1-game-data/1.19-pokedex-fields/README.md`
    /// </remarks>
    public class PokedexData
    {
        /// <summary>
        /// Pokedex entry description text.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Pokemon classification (e.g., "Flame Pokemon", "Mouse Pokemon").
        /// </summary>
        public string Category { get; }

        /// <summary>
        /// Height in meters.
        /// </summary>
        public float Height { get; }

        /// <summary>
        /// Weight in kilograms.
        /// </summary>
        public float Weight { get; }

        /// <summary>
        /// Pokemon color classification.
        /// </summary>
        public PokemonColor Color { get; }

        /// <summary>
        /// Pokemon shape classification.
        /// </summary>
        public PokemonShape Shape { get; }

        /// <summary>
        /// Pokemon habitat classification.
        /// </summary>
        public PokemonHabitat Habitat { get; }

        /// <summary>
        /// Initializes a new instance of PokedexData.
        /// </summary>
        public PokedexData(
            string description,
            string category,
            float height,
            float weight,
            PokemonColor color,
            PokemonShape shape,
            PokemonHabitat habitat)
        {
            Description = description ?? string.Empty;
            Category = category ?? string.Empty;
            Height = height;
            Weight = weight;
            Color = color;
            Shape = shape;
            Habitat = habitat;
        }
    }
}
