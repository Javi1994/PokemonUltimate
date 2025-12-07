using PokemonUltimate.Core.Data.Enums;
using PokemonUltimate.Core.Infrastructure.Localization;

namespace PokemonUltimate.Core.Utilities.Extensions
{
    /// <summary>
    /// Extension methods for PokemonType enum to provide localized display names.
    /// </summary>
    /// <remarks>
    /// **Feature**: 4: Unity Integration
    /// **Sub-Feature**: 4.9: Localization System
    /// **Documentation**: See `docs/features/4-unity-integration/4.9-localization-system/architecture.md`
    /// </remarks>
    public static class PokemonTypeExtensions
    {
        /// <summary>
        /// Gets the localized display name for a Pokemon type.
        /// </summary>
        /// <param name="type">The Pokemon type.</param>
        /// <param name="localizationProvider">The localization provider to use.</param>
        /// <returns>The localized type name, or the English name if translation is not found.</returns>
        public static string GetDisplayName(this PokemonType type, ILocalizationProvider localizationProvider)
        {
            if (localizationProvider == null)
                return type.ToString();

            var key = GetLocalizationKey(type);
            if (localizationProvider.HasString(key))
            {
                return localizationProvider.GetString(key);
            }

            // Fallback to English name
            return type.ToString();
        }

        /// <summary>
        /// Gets the localization key for a Pokemon type.
        /// </summary>
        private static string GetLocalizationKey(PokemonType type)
        {
            switch (type)
            {
                case PokemonType.Normal:
                    return LocalizationKey.Type_Normal;
                case PokemonType.Fire:
                    return LocalizationKey.Type_Fire;
                case PokemonType.Water:
                    return LocalizationKey.Type_Water;
                case PokemonType.Grass:
                    return LocalizationKey.Type_Grass;
                case PokemonType.Electric:
                    return LocalizationKey.Type_Electric;
                case PokemonType.Ice:
                    return LocalizationKey.Type_Ice;
                case PokemonType.Fighting:
                    return LocalizationKey.Type_Fighting;
                case PokemonType.Poison:
                    return LocalizationKey.Type_Poison;
                case PokemonType.Ground:
                    return LocalizationKey.Type_Ground;
                case PokemonType.Flying:
                    return LocalizationKey.Type_Flying;
                case PokemonType.Psychic:
                    return LocalizationKey.Type_Psychic;
                case PokemonType.Bug:
                    return LocalizationKey.Type_Bug;
                case PokemonType.Rock:
                    return LocalizationKey.Type_Rock;
                case PokemonType.Ghost:
                    return LocalizationKey.Type_Ghost;
                case PokemonType.Dragon:
                    return LocalizationKey.Type_Dragon;
                case PokemonType.Dark:
                    return LocalizationKey.Type_Dark;
                case PokemonType.Steel:
                    return LocalizationKey.Type_Steel;
                case PokemonType.Fairy:
                    return LocalizationKey.Type_Fairy;
                default:
                    return type.ToString();
            }
        }
    }
}
