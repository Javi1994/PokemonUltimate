using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Localization.Providers;
using PokemonUltimate.Localization.Providers.Definition;

namespace PokemonUltimate.Localization.Extensions
{
    /// <summary>
    /// Extension methods for AbilityData localization.
    /// </summary>
    /// <remarks>
    /// **Feature**: 4: Unity Integration
    /// **Sub-Feature**: 4.9: Localization System
    /// **Documentation**: See `docs/features/4-unity-integration/4.9-localization-system/CONTENT_LOCALIZATION_DESIGN.md`
    /// **Note**: These extension methods are in the Localization project to avoid circular dependencies.
    /// </remarks>
    public static class AbilityDataExtensions
    {
        /// <summary>
        /// Gets the localized display name for this ability.
        /// Falls back to the original name if translation not found.
        /// </summary>
        /// <param name="ability">The ability data.</param>
        /// <param name="localizationProvider">The localization provider.</param>
        /// <returns>The localized name, or the original name if translation not available.</returns>
        public static string GetDisplayName(this AbilityData ability, ILocalizationProvider localizationProvider)
        {
            if (ability == null)
                return string.Empty;

            if (localizationProvider == null)
                return ability.Name;

            var key = GenerateAbilityNameKey(ability.Name);
            var translatedName = localizationProvider.GetString(key);

            // If translation not found, GetString returns the key, so fallback to original name
            if (translatedName == key)
                return ability.Name;

            return translatedName;
        }

        /// <summary>
        /// Gets the localized description for this ability.
        /// Falls back to the original description if translation not found.
        /// </summary>
        /// <param name="ability">The ability data.</param>
        /// <param name="localizationProvider">The localization provider.</param>
        /// <returns>The localized description, or the original description if translation not available.</returns>
        public static string GetDescription(this AbilityData ability, ILocalizationProvider localizationProvider)
        {
            if (ability == null)
                return string.Empty;

            if (localizationProvider == null)
                return ability.Description;

            var key = GenerateAbilityDescriptionKey(ability.Name);
            var translatedDescription = localizationProvider.GetString(key);

            // If translation not found, GetString returns the key, so fallback to original description
            if (translatedDescription == key)
                return ability.Description;

            return translatedDescription;
        }

        /// <summary>
        /// Generates a localization key for an ability name.
        /// Format: "ability_name_{abilityName}" (lowercase, spaces replaced with underscores).
        /// </summary>
        private static string GenerateAbilityNameKey(string abilityName)
        {
            if (string.IsNullOrEmpty(abilityName))
                return string.Empty;

            var normalized = abilityName.ToLowerInvariant()
                .Replace(" ", "_")
                .Replace("-", "_")
                .Replace("'", "")
                .Replace(".", "")
                .Replace("(", "")
                .Replace(")", "");

            return $"ability_name_{normalized}";
        }

        /// <summary>
        /// Generates a localization key for an ability description.
        /// Format: "ability_description_{abilityName}" (lowercase, spaces replaced with underscores).
        /// </summary>
        private static string GenerateAbilityDescriptionKey(string abilityName)
        {
            if (string.IsNullOrEmpty(abilityName))
                return string.Empty;

            var normalized = abilityName.ToLowerInvariant()
                .Replace(" ", "_")
                .Replace("-", "_")
                .Replace("'", "")
                .Replace(".", "")
                .Replace("(", "")
                .Replace(")", "");

            return $"ability_description_{normalized}";
        }
    }
}
