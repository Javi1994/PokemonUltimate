using PokemonUltimate.Core.Data.Enums;
using PokemonUltimate.Localization.Constants;
using PokemonUltimate.Localization.Providers.Definition;

namespace PokemonUltimate.Localization.Extensions
{
    /// <summary>
    /// Extension methods for VolatileStatus enum to provide localized display names.
    /// </summary>
    /// <remarks>
    /// **Feature**: 4: Unity Integration
    /// **Sub-Feature**: 4.9: Localization System
    /// **Note**: These extension methods are in the Localization project to avoid circular dependencies.
    /// </remarks>
    public static class VolatileStatusExtensions
    {
        /// <summary>
        /// Gets the localized display name for a volatile status.
        /// </summary>
        public static string GetDisplayName(this VolatileStatus status, ILocalizationProvider localizationProvider)
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

        private static string GetLocalizationKey(VolatileStatus status)
        {
            switch (status)
            {
                case VolatileStatus.None:
                    return LocalizationKey.VolatileStatus_None;
                case VolatileStatus.Confusion:
                    return LocalizationKey.VolatileStatus_Confusion;
                case VolatileStatus.Flinch:
                    return LocalizationKey.VolatileStatus_Flinch;
                case VolatileStatus.LeechSeed:
                    return LocalizationKey.VolatileStatus_LeechSeed;
                case VolatileStatus.Attract:
                    return LocalizationKey.VolatileStatus_Attract;
                case VolatileStatus.Curse:
                    return LocalizationKey.VolatileStatus_Curse;
                case VolatileStatus.Encore:
                    return LocalizationKey.VolatileStatus_Encore;
                case VolatileStatus.Taunt:
                    return LocalizationKey.VolatileStatus_Taunt;
                case VolatileStatus.Torment:
                    return LocalizationKey.VolatileStatus_Torment;
                case VolatileStatus.Disable:
                    return LocalizationKey.VolatileStatus_Disable;
                case VolatileStatus.SemiInvulnerable:
                    return LocalizationKey.VolatileStatus_SemiInvulnerable;
                case VolatileStatus.Charging:
                    return LocalizationKey.VolatileStatus_Charging;
                case VolatileStatus.Protected:
                    return LocalizationKey.VolatileStatus_Protected;
                case VolatileStatus.SwitchingOut:
                    return LocalizationKey.VolatileStatus_SwitchingOut;
                case VolatileStatus.Focusing:
                    return LocalizationKey.VolatileStatus_Focusing;
                case VolatileStatus.FollowMe:
                    return LocalizationKey.VolatileStatus_FollowMe;
                case VolatileStatus.RagePowder:
                    return LocalizationKey.VolatileStatus_RagePowder;
                default:
                    return status.ToString();
            }
        }
    }
}
