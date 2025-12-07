using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Localization;

namespace PokemonUltimate.Core.Extensions
{
    /// <summary>
    /// Extension methods for PersistentStatus enum to provide localized display names.
    /// </summary>
    /// <remarks>
    /// **Feature**: 4: Unity Integration
    /// **Sub-Feature**: 4.9: Localization System
    /// </remarks>
    public static class PersistentStatusExtensions
    {
        /// <summary>
        /// Gets the localized display name for a persistent status.
        /// </summary>
        public static string GetDisplayName(this PersistentStatus status, ILocalizationProvider localizationProvider)
        {
            if (localizationProvider == null)
                return status.ToString();

            var key = GetLocalizationKey(status);
            if (localizationProvider.HasString(key))
            {
                return localizationProvider.GetString(key);
            }

            return status.ToString();
        }

        private static string GetLocalizationKey(PersistentStatus status)
        {
            switch (status)
            {
                case PersistentStatus.None:
                    return LocalizationKey.Status_None;
                case PersistentStatus.Burn:
                    return LocalizationKey.Status_Burn;
                case PersistentStatus.Paralysis:
                    return LocalizationKey.Status_Paralysis;
                case PersistentStatus.Sleep:
                    return LocalizationKey.Status_Sleep;
                case PersistentStatus.Poison:
                    return LocalizationKey.Status_Poison;
                case PersistentStatus.BadlyPoisoned:
                    return LocalizationKey.Status_BadlyPoisoned;
                case PersistentStatus.Freeze:
                    return LocalizationKey.Status_Freeze;
                default:
                    return status.ToString();
            }
        }
    }
}
