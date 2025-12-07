using PokemonUltimate.Content.Providers;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Localization;

namespace PokemonUltimate.Content.Extensions
{
    /// <summary>
    /// Extension methods for HazardData.
    /// </summary>
    /// <remarks>
    /// **Feature**: 4: Unity Integration
    /// **Sub-Feature**: 4.9: Localization System
    /// **Documentation**: See `docs/features/4-unity-integration/4.9-localization-system/architecture.md`
    /// </remarks>
    public static class HazardDataExtensions
    {
        /// <summary>
        /// Gets the localized name for this hazard.
        /// </summary>
        /// <param name="hazard">The hazard data.</param>
        /// <param name="localizationProvider">Optional localization provider. If null, uses default LocalizationProvider.</param>
        /// <returns>The localized name, or the original name if translation not found.</returns>
        public static string GetLocalizedName(this HazardData hazard, ILocalizationProvider localizationProvider = null)
        {
            if (hazard == null)
                return string.Empty;

            localizationProvider = localizationProvider ?? new LocalizationProvider();
            var key = GenerateHazardNameKey(hazard.Name);

            if (localizationProvider.HasString(key))
                return localizationProvider.GetString(key);

            return hazard.Name;
        }

        /// <summary>
        /// Gets the localized description for this hazard.
        /// </summary>
        /// <param name="hazard">The hazard data.</param>
        /// <param name="localizationProvider">Optional localization provider. If null, uses default LocalizationProvider.</param>
        /// <returns>The localized description, or the original description if translation not found.</returns>
        public static string GetLocalizedDescription(this HazardData hazard, ILocalizationProvider localizationProvider = null)
        {
            if (hazard == null)
                return string.Empty;

            localizationProvider = localizationProvider ?? new LocalizationProvider();
            var key = GenerateHazardDescriptionKey(hazard.Name);

            if (localizationProvider.HasString(key))
                return localizationProvider.GetString(key);

            return hazard.Description;
        }

        /// <summary>
        /// Generates a localization key for a hazard name.
        /// </summary>
        private static string GenerateHazardNameKey(string hazardName)
        {
            if (string.IsNullOrEmpty(hazardName))
                return string.Empty;

            var normalized = hazardName.ToLowerInvariant()
                .Replace(" ", "_")
                .Replace("-", "_")
                .Replace("'", "")
                .Replace(".", "")
                .Replace("(", "")
                .Replace(")", "");

            return $"hazard_name_{normalized}";
        }

        /// <summary>
        /// Generates a localization key for a hazard description.
        /// </summary>
        private static string GenerateHazardDescriptionKey(string hazardName)
        {
            if (string.IsNullOrEmpty(hazardName))
                return string.Empty;

            var normalized = hazardName.ToLowerInvariant()
                .Replace(" ", "_")
                .Replace("-", "_")
                .Replace("'", "")
                .Replace(".", "")
                .Replace("(", "")
                .Replace(")", "");

            return $"hazard_description_{normalized}";
        }
    }
}
