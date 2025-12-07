using PokemonUltimate.Content.Providers;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Localization;

namespace PokemonUltimate.Content.Extensions
{
    /// <summary>
    /// Extension methods for ItemData.
    /// </summary>
    /// <remarks>
    /// **Feature**: 4: Unity Integration
    /// **Sub-Feature**: 4.9: Localization System
    /// **Documentation**: See `docs/features/4-unity-integration/4.9-localization-system/architecture.md`
    /// </remarks>
    public static class ItemDataExtensions
    {
        /// <summary>
        /// Gets the localized name for this item.
        /// </summary>
        /// <param name="item">The item data.</param>
        /// <param name="localizationProvider">Optional localization provider. If null, uses default LocalizationProvider.</param>
        /// <returns>The localized name, or the original name if translation not found.</returns>
        public static string GetLocalizedName(this ItemData item, ILocalizationProvider localizationProvider = null)
        {
            if (item == null)
                return string.Empty;

            localizationProvider = localizationProvider ?? new LocalizationProvider();
            var key = GenerateItemNameKey(item.Name);

            if (localizationProvider.HasString(key))
                return localizationProvider.GetString(key);

            return item.Name;
        }

        /// <summary>
        /// Gets the localized description for this item.
        /// </summary>
        /// <param name="item">The item data.</param>
        /// <param name="localizationProvider">Optional localization provider. If null, uses default LocalizationProvider.</param>
        /// <returns>The localized description, or the original description if translation not found.</returns>
        public static string GetLocalizedDescription(this ItemData item, ILocalizationProvider localizationProvider = null)
        {
            if (item == null)
                return string.Empty;

            localizationProvider = localizationProvider ?? new LocalizationProvider();
            var key = GenerateItemDescriptionKey(item.Name);

            if (localizationProvider.HasString(key))
                return localizationProvider.GetString(key);

            return item.Description;
        }

        /// <summary>
        /// Generates a localization key for an item name.
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
