using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Core.Infrastructure.Localization;

namespace PokemonUltimate.Core.Utilities.Extensions
{
    /// <summary>
    /// Extension methods for ItemData localization.
    /// </summary>
    /// <remarks>
    /// **Feature**: 4: Unity Integration
    /// **Sub-Feature**: 4.9: Localization System
    /// **Documentation**: See `docs/features/4-unity-integration/4.9-localization-system/CONTENT_LOCALIZATION_DESIGN.md`
    /// </remarks>
    public static class ItemDataExtensions
    {
        /// <summary>
        /// Gets the localized display name for this item.
        /// Falls back to the original name if translation not found.
        /// </summary>
        /// <param name="item">The item data.</param>
        /// <param name="localizationProvider">The localization provider.</param>
        /// <returns>The localized name, or the original name if translation not available.</returns>
        public static string GetDisplayName(this ItemData item, ILocalizationProvider localizationProvider)
        {
            if (item == null)
                return string.Empty;

            if (localizationProvider == null)
                return item.Name;

            var key = GenerateItemNameKey(item.Name);
            var translatedName = localizationProvider.GetString(key);

            // If translation not found, GetString returns the key, so fallback to original name
            if (translatedName == key)
                return item.Name;

            return translatedName;
        }

        /// <summary>
        /// Gets the localized description for this item.
        /// Falls back to the original description if translation not found.
        /// </summary>
        /// <param name="item">The item data.</param>
        /// <param name="localizationProvider">The localization provider.</param>
        /// <returns>The localized description, or the original description if translation not available.</returns>
        public static string GetDescription(this ItemData item, ILocalizationProvider localizationProvider)
        {
            if (item == null)
                return string.Empty;

            if (localizationProvider == null)
                return item.Description;

            var key = GenerateItemDescriptionKey(item.Name);
            var translatedDescription = localizationProvider.GetString(key);

            // If translation not found, GetString returns the key, so fallback to original description
            if (translatedDescription == key)
                return item.Description;

            return translatedDescription;
        }

        /// <summary>
        /// Generates a localization key for an item name.
        /// Format: "item_name_{itemName}" (lowercase, spaces replaced with underscores).
        /// </summary>
        private static string GenerateItemNameKey(string itemName)
        {
            if (string.IsNullOrEmpty(itemName))
                return string.Empty;

            var normalized = itemName.ToLowerInvariant()
                .Replace(" ", "_")
                .Replace("-", "_")
                .Replace("'", "")
                .Replace(".", "")
                .Replace("(", "")
                .Replace(")", "");

            return $"item_name_{normalized}";
        }

        /// <summary>
        /// Generates a localization key for an item description.
        /// Format: "item_description_{itemName}" (lowercase, spaces replaced with underscores).
        /// </summary>
        private static string GenerateItemDescriptionKey(string itemName)
        {
            if (string.IsNullOrEmpty(itemName))
                return string.Empty;

            var normalized = itemName.ToLowerInvariant()
                .Replace(" ", "_")
                .Replace("-", "_")
                .Replace("'", "")
                .Replace(".", "")
                .Replace("(", "")
                .Replace(")", "");

            return $"item_description_{normalized}";
        }
    }
}
