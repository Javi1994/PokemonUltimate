using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Core.Infrastructure.Localization;

namespace PokemonUltimate.Core.Utilities.Extensions
{
    /// <summary>
    /// Extension methods for PokemonSpeciesData localization.
    /// </summary>
    /// <remarks>
    /// **Feature**: 4: Unity Integration
    /// **Sub-Feature**: 4.9: Localization System
    /// **Documentation**: See `docs/features/4-unity-integration/4.9-localization-system/CONTENT_LOCALIZATION_DESIGN.md`
    /// </remarks>
    public static class PokemonSpeciesDataExtensions
    {
        /// <summary>
        /// Gets the localized display name for this Pokemon species.
        /// Falls back to the original name if translation not found.
        /// </summary>
        /// <param name="species">The Pokemon species data.</param>
        /// <param name="localizationProvider">The localization provider.</param>
        /// <returns>The localized name, or the original name if translation not available.</returns>
        public static string GetDisplayName(this PokemonSpeciesData species, ILocalizationProvider localizationProvider)
        {
            if (species == null)
                return string.Empty;

            if (localizationProvider == null)
                return species.Name;

            var key = GeneratePokemonNameKey(species.Name);
            var translatedName = localizationProvider.GetString(key);

            // If translation not found, GetString returns the key, so fallback to original name
            if (translatedName == key)
                return species.Name;

            return translatedName;
        }

        /// <summary>
        /// Generates a localization key for a Pokemon name.
        /// Format: "pokemon_name_{pokemonName}" (lowercase, spaces replaced with underscores).
        /// </summary>
        private static string GeneratePokemonNameKey(string pokemonName)
        {
            if (string.IsNullOrEmpty(pokemonName))
                return string.Empty;

            var normalized = pokemonName.ToLowerInvariant()
                .Replace(" ", "_")
                .Replace("-", "_")
                .Replace("'", "")
                .Replace(".", "")
                .Replace("(", "")
                .Replace(")", "");

            return $"pokemon_name_{normalized}";
        }
    }
}
