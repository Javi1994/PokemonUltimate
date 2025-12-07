using PokemonUltimate.Content.Providers;
using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Core.Infrastructure.Localization;

namespace PokemonUltimate.Content.Extensions
{
    /// <summary>
    /// Extension methods for SideConditionData.
    /// </summary>
    /// <remarks>
    /// **Feature**: 4: Unity Integration
    /// **Sub-Feature**: 4.9: Localization System
    /// **Documentation**: See `docs/features/4-unity-integration/4.9-localization-system/architecture.md`
    /// </remarks>
    public static class SideConditionDataExtensions
    {
        /// <summary>
        /// Gets the localized name for this side condition.
        /// </summary>
        /// <param name="sideCondition">The side condition data.</param>
        /// <param name="localizationProvider">Optional localization provider. If null, uses default LocalizationProvider.</param>
        /// <returns>The localized name, or the original name if translation not found.</returns>
        public static string GetLocalizedName(this SideConditionData sideCondition, ILocalizationProvider localizationProvider = null)
        {
            if (sideCondition == null)
                return string.Empty;

            localizationProvider = localizationProvider ?? new LocalizationProvider();
            var key = GenerateSideConditionNameKey(sideCondition.Name);

            if (localizationProvider.HasString(key))
                return localizationProvider.GetString(key);

            return sideCondition.Name;
        }

        /// <summary>
        /// Gets the localized description for this side condition.
        /// </summary>
        /// <param name="sideCondition">The side condition data.</param>
        /// <param name="localizationProvider">Optional localization provider. If null, uses default LocalizationProvider.</param>
        /// <returns>The localized description, or the original description if translation not found.</returns>
        public static string GetLocalizedDescription(this SideConditionData sideCondition, ILocalizationProvider localizationProvider = null)
        {
            if (sideCondition == null)
                return string.Empty;

            localizationProvider = localizationProvider ?? new LocalizationProvider();
            var key = GenerateSideConditionDescriptionKey(sideCondition.Name);

            if (localizationProvider.HasString(key))
                return localizationProvider.GetString(key);

            return sideCondition.Description;
        }

        /// <summary>
        /// Generates a localization key for a side condition name.
        /// </summary>
        private static string GenerateSideConditionNameKey(string sideConditionName)
        {
            if (string.IsNullOrEmpty(sideConditionName))
                return string.Empty;

            var normalized = sideConditionName.ToLowerInvariant()
                .Replace(" ", "_")
                .Replace("-", "_")
                .Replace("'", "")
                .Replace(".", "")
                .Replace("(", "")
                .Replace(")", "");

            return $"side_condition_name_{normalized}";
        }

        /// <summary>
        /// Generates a localization key for a side condition description.
        /// </summary>
        private static string GenerateSideConditionDescriptionKey(string sideConditionName)
        {
            if (string.IsNullOrEmpty(sideConditionName))
                return string.Empty;

            var normalized = sideConditionName.ToLowerInvariant()
                .Replace(" ", "_")
                .Replace("-", "_")
                .Replace("'", "")
                .Replace(".", "")
                .Replace("(", "")
                .Replace(")", "");

            return $"side_condition_description_{normalized}";
        }
    }
}
