using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Localization;

namespace PokemonUltimate.Core.Extensions
{
    /// <summary>
    /// Extension methods for Nature enum to provide localized display names.
    /// </summary>
    /// <remarks>
    /// **Feature**: 4: Unity Integration
    /// **Sub-Feature**: 4.9: Localization System
    /// **Documentation**: See `docs/features/4-unity-integration/4.9-localization-system/architecture.md`
    /// </remarks>
    public static class NatureExtensions
    {
        /// <summary>
        /// Gets the localized display name for a Nature.
        /// </summary>
        /// <param name="nature">The Nature.</param>
        /// <param name="localizationProvider">The localization provider to use.</param>
        /// <returns>The localized nature name, or the English name if translation is not found.</returns>
        public static string GetDisplayName(this Nature nature, ILocalizationProvider localizationProvider)
        {
            if (localizationProvider == null)
                return nature.ToString();

            var key = GetLocalizationKey(nature);
            if (localizationProvider.HasString(key))
            {
                return localizationProvider.GetString(key);
            }

            // Fallback to English name
            return nature.ToString();
        }

        /// <summary>
        /// Gets the localization key for a Nature.
        /// </summary>
        private static string GetLocalizationKey(Nature nature)
        {
            switch (nature)
            {
                case Nature.Hardy:
                    return LocalizationKey.Nature_Hardy;
                case Nature.Docile:
                    return LocalizationKey.Nature_Docile;
                case Nature.Serious:
                    return LocalizationKey.Nature_Serious;
                case Nature.Bashful:
                    return LocalizationKey.Nature_Bashful;
                case Nature.Quirky:
                    return LocalizationKey.Nature_Quirky;
                case Nature.Lonely:
                    return LocalizationKey.Nature_Lonely;
                case Nature.Brave:
                    return LocalizationKey.Nature_Brave;
                case Nature.Adamant:
                    return LocalizationKey.Nature_Adamant;
                case Nature.Naughty:
                    return LocalizationKey.Nature_Naughty;
                case Nature.Bold:
                    return LocalizationKey.Nature_Bold;
                case Nature.Relaxed:
                    return LocalizationKey.Nature_Relaxed;
                case Nature.Impish:
                    return LocalizationKey.Nature_Impish;
                case Nature.Lax:
                    return LocalizationKey.Nature_Lax;
                case Nature.Timid:
                    return LocalizationKey.Nature_Timid;
                case Nature.Hasty:
                    return LocalizationKey.Nature_Hasty;
                case Nature.Jolly:
                    return LocalizationKey.Nature_Jolly;
                case Nature.Naive:
                    return LocalizationKey.Nature_Naive;
                case Nature.Modest:
                    return LocalizationKey.Nature_Modest;
                case Nature.Mild:
                    return LocalizationKey.Nature_Mild;
                case Nature.Quiet:
                    return LocalizationKey.Nature_Quiet;
                case Nature.Rash:
                    return LocalizationKey.Nature_Rash;
                case Nature.Calm:
                    return LocalizationKey.Nature_Calm;
                case Nature.Gentle:
                    return LocalizationKey.Nature_Gentle;
                case Nature.Sassy:
                    return LocalizationKey.Nature_Sassy;
                case Nature.Careful:
                    return LocalizationKey.Nature_Careful;
                default:
                    return nature.ToString();
            }
        }
    }
}
