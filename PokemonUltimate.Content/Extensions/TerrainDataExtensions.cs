using PokemonUltimate.Content.Providers;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Localization;

namespace PokemonUltimate.Content.Extensions
{
    /// <summary>
    /// Extension methods for TerrainData.
    /// </summary>
    /// <remarks>
    /// **Feature**: 4: Unity Integration
    /// **Sub-Feature**: 4.9: Localization System
    /// **Documentation**: See `docs/features/4-unity-integration/4.9-localization-system/architecture.md`
    /// </remarks>
    public static class TerrainDataExtensions
    {
        /// <summary>
        /// Gets the localized name for this terrain.
        /// </summary>
        /// <param name="terrain">The terrain data.</param>
        /// <param name="localizationProvider">Optional localization provider. If null, uses default LocalizationProvider.</param>
        /// <returns>The localized name, or the original name if translation not found.</returns>
        public static string GetLocalizedName(this TerrainData terrain, ILocalizationProvider localizationProvider = null)
        {
            if (terrain == null)
                return string.Empty;

            localizationProvider = localizationProvider ?? new LocalizationProvider();
            var key = GenerateTerrainNameKey(terrain.Name);

            if (localizationProvider.HasString(key))
                return localizationProvider.GetString(key);

            return terrain.Name;
        }

        /// <summary>
        /// Gets the localized description for this terrain.
        /// </summary>
        /// <param name="terrain">The terrain data.</param>
        /// <param name="localizationProvider">Optional localization provider. If null, uses default LocalizationProvider.</param>
        /// <returns>The localized description, or the original description if translation not found.</returns>
        public static string GetLocalizedDescription(this TerrainData terrain, ILocalizationProvider localizationProvider = null)
        {
            if (terrain == null)
                return string.Empty;

            localizationProvider = localizationProvider ?? new LocalizationProvider();
            var key = GenerateTerrainDescriptionKey(terrain.Name);

            if (localizationProvider.HasString(key))
                return localizationProvider.GetString(key);

            return terrain.Description;
        }

        /// <summary>
        /// Generates a localization key for a terrain name.
        /// </summary>
        private static string GenerateTerrainNameKey(string terrainName)
        {
            if (string.IsNullOrEmpty(terrainName))
                return string.Empty;

            var normalized = terrainName.ToLowerInvariant()
                .Replace(" ", "_")
                .Replace("-", "_")
                .Replace("'", "")
                .Replace(".", "")
                .Replace("(", "")
                .Replace(")", "");

            return $"terrain_name_{normalized}";
        }

        /// <summary>
        /// Generates a localization key for a terrain description.
        /// </summary>
        private static string GenerateTerrainDescriptionKey(string terrainName)
        {
            if (string.IsNullOrEmpty(terrainName))
                return string.Empty;

            var normalized = terrainName.ToLowerInvariant()
                .Replace(" ", "_")
                .Replace("-", "_")
                .Replace("'", "")
                .Replace(".", "")
                .Replace("(", "")
                .Replace(")", "");

            return $"terrain_description_{normalized}";
        }
    }
}
