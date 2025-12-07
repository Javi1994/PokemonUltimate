using System.Collections.Generic;
using PokemonUltimate.Content.Providers;
using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Core.Data.Enums;

namespace PokemonUltimate.Content.Extensions
{
    /// <summary>
    /// Extension methods for PokemonSpeciesData.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data, 3: Content Expansion
    /// **Sub-Feature**: 1.19: Pokedex Fields, 3.1: Pokemon Expansion
    /// **Documentation**: See `docs/features/1-game-data/1.19-pokedex-fields/README.md` and `docs/features/3-content-expansion/3.1-pokemon-expansion/README.md`
    /// </remarks>
    public static class PokemonSpeciesDataExtensions
    {
        /// <summary>
        /// Applies Pokedex data to this Pokemon species if available.
        /// Only updates fields that are empty/default (preserves existing values if set).
        /// </summary>
        /// <param name="pokemon">The Pokemon species to apply Pokedex data to.</param>
        /// <returns>The same Pokemon instance for method chaining.</returns>
        public static PokemonSpeciesData WithPokedexData(this PokemonSpeciesData pokemon)
        {
            if (pokemon == null)
                return pokemon;

            var pokedexData = PokedexDataProvider.GetData(pokemon.Name);
            if (pokedexData == null)
                return pokemon;

            // Apply data, preserving existing non-empty values
            if (string.IsNullOrEmpty(pokemon.Description))
                pokemon.Description = pokedexData.Description;

            if (string.IsNullOrEmpty(pokemon.Category))
                pokemon.Category = pokedexData.Category;

            if (pokemon.Height == 0f)
                pokemon.Height = pokedexData.Height;

            if (pokemon.Weight == 0f)
                pokemon.Weight = pokedexData.Weight;

            if (pokemon.Color == PokemonColor.Unknown)
                pokemon.Color = pokedexData.Color;

            if (pokemon.Shape == PokemonShape.Unknown)
                pokemon.Shape = pokedexData.Shape;

            if (pokemon.Habitat == PokemonHabitat.Unknown)
                pokemon.Habitat = pokedexData.Habitat;

            return pokemon;
        }

        /// <summary>
        /// Applies learnset data to this Pokemon species if available.
        /// Only updates if learnset is empty (preserves existing learnset if set).
        /// </summary>
        /// <param name="pokemon">The Pokemon species to apply learnset data to.</param>
        /// <returns>The same Pokemon instance for method chaining.</returns>
        public static PokemonSpeciesData WithLearnset(this PokemonSpeciesData pokemon)
        {
            if (pokemon == null)
                return pokemon;

            // Only apply if learnset is empty
            if (pokemon.Learnset != null && pokemon.Learnset.Count > 0)
                return pokemon;

            var learnsetData = LearnsetProvider.GetLearnset(pokemon.Name);
            if (learnsetData == null)
                return pokemon;

            // Apply learnset
            pokemon.Learnset = learnsetData.Moves;

            return pokemon;
        }

        /// <summary>
        /// Gets all variant forms for this Pokemon from the VariantProvider.
        /// </summary>
        /// <param name="pokemon">The base Pokemon species.</param>
        /// <returns>Enumerable collection of variant forms.</returns>
        public static IEnumerable<PokemonSpeciesData> GetVariants(this PokemonSpeciesData pokemon)
        {
            return VariantProvider.GetVariants(pokemon);
        }

        /// <summary>
        /// Checks if this Pokemon has any variant forms available.
        /// </summary>
        /// <param name="pokemon">The base Pokemon species.</param>
        /// <returns>True if variants exist, false otherwise.</returns>
        public static bool HasVariantsAvailable(this PokemonSpeciesData pokemon)
        {
            return VariantProvider.HasVariants(pokemon);
        }
    }
}
