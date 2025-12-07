using PokemonUltimate.Core.Data.Enums;
using PokemonUltimate.Core.Infrastructure.Localization;

namespace PokemonUltimate.Core.Utilities.Extensions
{
    /// <summary>
    /// Extension methods for MoveCategory enum to provide localized display names.
    /// </summary>
    /// <remarks>
    /// **Feature**: 4: Unity Integration
    /// **Sub-Feature**: 4.9: Localization System
    /// **Documentation**: See `docs/features/4-unity-integration/4.9-localization-system/architecture.md`
    /// </remarks>
    public static class MoveCategoryExtensions
    {
        /// <summary>
        /// Gets the localized display name for a MoveCategory.
        /// </summary>
        /// <param name="category">The MoveCategory.</param>
        /// <param name="localizationProvider">The localization provider to use.</param>
        /// <returns>The localized category name, or the English name if translation is not found.</returns>
        public static string GetDisplayName(this MoveCategory category, ILocalizationProvider localizationProvider)
        {
            if (localizationProvider == null)
                return category.ToString();

            var key = GetLocalizationKey(category);
            if (localizationProvider.HasString(key))
            {
                return localizationProvider.GetString(key);
            }

            // Fallback to English name
            return category.ToString();
        }

        /// <summary>
        /// Gets the localization key for a MoveCategory.
        /// </summary>
        private static string GetLocalizationKey(MoveCategory category)
        {
            switch (category)
            {
                case MoveCategory.Physical:
                    return LocalizationKey.MoveCategory_Physical;
                case MoveCategory.Special:
                    return LocalizationKey.MoveCategory_Special;
                case MoveCategory.Status:
                    return LocalizationKey.MoveCategory_Status;
                default:
                    return category.ToString();
            }
        }
    }
}
