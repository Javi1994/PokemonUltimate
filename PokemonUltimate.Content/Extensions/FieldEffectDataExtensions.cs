using PokemonUltimate.Content.Providers;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Localization;

namespace PokemonUltimate.Content.Extensions
{
    /// <summary>
    /// Extension methods for FieldEffectData.
    /// </summary>
    /// <remarks>
    /// **Feature**: 4: Unity Integration
    /// **Sub-Feature**: 4.9: Localization System
    /// **Documentation**: See `docs/features/4-unity-integration/4.9-localization-system/architecture.md`
    /// </remarks>
    public static class FieldEffectDataExtensions
    {
        /// <summary>
        /// Gets the localized name for this field effect.
        /// </summary>
        /// <param name="fieldEffect">The field effect data.</param>
        /// <param name="localizationProvider">Optional localization provider. If null, uses default LocalizationProvider.</param>
        /// <returns>The localized name, or the original name if translation not found.</returns>
        public static string GetLocalizedName(this FieldEffectData fieldEffect, ILocalizationProvider localizationProvider = null)
        {
            if (fieldEffect == null)
                return string.Empty;

            localizationProvider = localizationProvider ?? new LocalizationProvider();
            var key = GenerateFieldEffectNameKey(fieldEffect.Name);

            if (localizationProvider.HasString(key))
                return localizationProvider.GetString(key);

            return fieldEffect.Name;
        }

        /// <summary>
        /// Gets the localized description for this field effect.
        /// </summary>
        /// <param name="fieldEffect">The field effect data.</param>
        /// <param name="localizationProvider">Optional localization provider. If null, uses default LocalizationProvider.</param>
        /// <returns>The localized description, or the original description if translation not found.</returns>
        public static string GetLocalizedDescription(this FieldEffectData fieldEffect, ILocalizationProvider localizationProvider = null)
        {
            if (fieldEffect == null)
                return string.Empty;

            localizationProvider = localizationProvider ?? new LocalizationProvider();
            var key = GenerateFieldEffectDescriptionKey(fieldEffect.Name);

            if (localizationProvider.HasString(key))
                return localizationProvider.GetString(key);

            return fieldEffect.Description;
        }

        /// <summary>
        /// Generates a localization key for a field effect name.
        /// </summary>
        private static string GenerateFieldEffectNameKey(string fieldEffectName)
        {
            if (string.IsNullOrEmpty(fieldEffectName))
                return string.Empty;

            var normalized = fieldEffectName.ToLowerInvariant()
                .Replace(" ", "_")
                .Replace("-", "_")
                .Replace("'", "")
                .Replace(".", "")
                .Replace("(", "")
                .Replace(")", "");

            return $"field_effect_name_{normalized}";
        }

        /// <summary>
        /// Generates a localization key for a field effect description.
        /// </summary>
        private static string GenerateFieldEffectDescriptionKey(string fieldEffectName)
        {
            if (string.IsNullOrEmpty(fieldEffectName))
                return string.Empty;

            var normalized = fieldEffectName.ToLowerInvariant()
                .Replace(" ", "_")
                .Replace("-", "_")
                .Replace("'", "")
                .Replace(".", "")
                .Replace("(", "")
                .Replace(")", "");

            return $"field_effect_description_{normalized}";
        }
    }
}
