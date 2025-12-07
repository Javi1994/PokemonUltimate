using System;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Extensions;

namespace PokemonUltimate.Core.Localization
{
    /// <summary>
    /// Helper class for localization utilities that don't depend on UI frameworks.
    /// For Windows Forms specific helpers, see WinFormsLocalizationHelper in the respective projects.
    /// </summary>
    /// <remarks>
    /// **Feature**: 4: Unity Integration
    /// **Sub-Feature**: 4.9: Localization System
    /// </remarks>
    public static class LocalizationHelper
    {
        /// <summary>
        /// Gets the current localization provider instance.
        /// </summary>
        public static ILocalizationProvider Provider => LocalizationManager.Instance;

        /// <summary>
        /// Gets the localized display name for an enum value.
        /// </summary>
        public static string GetEnumDisplayName<TEnum>(TEnum value) where TEnum : struct, Enum
        {
            return GetEnumDisplayName(value, Provider);
        }

        /// <summary>
        /// Gets the localized display name for an enum value using a specific provider.
        /// </summary>
        public static string GetEnumDisplayName<TEnum>(TEnum value, ILocalizationProvider provider) where TEnum : struct, Enum
        {
            if (UseExtensionMethod(value, provider, out string result))
                return result;

            return value.ToString();
        }

        /// <summary>
        /// Gets the application title from localization, or returns a default.
        /// </summary>
        public static string GetApplicationTitle()
        {
            return GetApplicationTitle(Provider);
        }

        /// <summary>
        /// Gets the application title from localization using a specific provider, or returns a default.
        /// </summary>
        public static string GetApplicationTitle(ILocalizationProvider provider)
        {
            return "Pokemon Ultimate";
        }

        #region Private Helpers

        private static bool UseExtensionMethod<TEnum>(TEnum value, ILocalizationProvider provider, out string result) where TEnum : struct, Enum
        {
            result = null;

            // Try PokemonType
            if (value is PokemonType pokemonType)
            {
                result = pokemonType.GetDisplayName(provider);
                return true;
            }

            // Try PersistentStatus
            if (value is PersistentStatus persistentStatus)
            {
                result = persistentStatus.GetDisplayName(provider);
                return true;
            }

            // Try VolatileStatus (flags enum - need to handle multiple flags)
            if (value is VolatileStatus volatileStatus)
            {
                // For flags enums, we might need to handle multiple values
                // For now, just handle single values
                if (volatileStatus != VolatileStatus.None)
                {
                    result = volatileStatus.GetDisplayName(provider);
                    return true;
                }
            }

            return false;
        }

        #endregion
    }
}
