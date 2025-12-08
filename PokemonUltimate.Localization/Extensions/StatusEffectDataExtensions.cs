using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Localization.Providers;
using PokemonUltimate.Localization.Providers.Definition;

namespace PokemonUltimate.Localization.Extensions
{
    /// <summary>
    /// Extension methods for StatusEffectData.
    /// </summary>
    /// <remarks>
    /// **Feature**: 4: Unity Integration
    /// **Sub-Feature**: 4.9: Localization System
    /// **Documentation**: See `docs/features/4-unity-integration/4.9-localization-system/architecture.md`
    /// </remarks>
    public static class StatusEffectDataExtensions
    {
        /// <summary>
        /// Gets the localized name for this status effect.
        /// </summary>
        /// <param name="statusEffect">The status effect data.</param>
        /// <param name="localizationProvider">Optional localization provider. If null, uses default LocalizationProvider.</param>
        /// <returns>The localized name, or the original name if translation not found.</returns>
        public static string GetDisplayName(this StatusEffectData statusEffect, ILocalizationProvider localizationProvider = null)
        {
            if (statusEffect == null)
                return string.Empty;

            localizationProvider = localizationProvider ?? new LocalizationProvider();
            var key = GenerateStatusEffectNameKey(statusEffect.Name);

            if (localizationProvider.HasString(key))
                return localizationProvider.GetString(key);

            return statusEffect.Name;
        }

        /// <summary>
        /// Gets the localized description for this status effect.
        /// </summary>
        /// <param name="statusEffect">The status effect data.</param>
        /// <param name="localizationProvider">Optional localization provider. If null, uses default LocalizationProvider.</param>
        /// <returns>The localized description, or the original description if translation not found.</returns>
        public static string GetDescription(this StatusEffectData statusEffect, ILocalizationProvider localizationProvider = null)
        {
            if (statusEffect == null)
                return string.Empty;

            localizationProvider = localizationProvider ?? new LocalizationProvider();
            var key = GenerateStatusEffectDescriptionKey(statusEffect.Name);

            if (localizationProvider.HasString(key))
                return localizationProvider.GetString(key);

            return statusEffect.Description;
        }

        /// <summary>
        /// Generates a localization key for a status effect name.
        /// </summary>
        private static string GenerateStatusEffectNameKey(string statusEffectName)
        {
            if (string.IsNullOrEmpty(statusEffectName))
                return string.Empty;

            var normalized = statusEffectName.ToLowerInvariant()
                .Replace(" ", "_")
                .Replace("-", "_")
                .Replace("'", "")
                .Replace(".", "")
                .Replace("(", "")
                .Replace(")", "");

            return $"status_effect_name_{normalized}";
        }

        /// <summary>
        /// Generates a localization key for a status effect description.
        /// </summary>
        private static string GenerateStatusEffectDescriptionKey(string statusEffectName)
        {
            if (string.IsNullOrEmpty(statusEffectName))
                return string.Empty;

            var normalized = statusEffectName.ToLowerInvariant()
                .Replace(" ", "_")
                .Replace("-", "_")
                .Replace("'", "")
                .Replace(".", "")
                .Replace("(", "")
                .Replace(")", "");

            return $"status_effect_description_{normalized}";
        }
    }
}
