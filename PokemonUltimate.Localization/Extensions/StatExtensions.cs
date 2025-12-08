using PokemonUltimate.Core.Data.Enums;
using PokemonUltimate.Localization.Constants;
using PokemonUltimate.Localization.Providers;
using PokemonUltimate.Localization.Providers.Definition;

namespace PokemonUltimate.Localization.Extensions
{
    /// <summary>
    /// Extension methods for Stat enum to provide localized display names.
    /// </summary>
    /// <remarks>
    /// **Feature**: 4: Unity Integration
    /// **Sub-Feature**: 4.9: Localization System
    /// **Documentation**: See `docs/features/4-unity-integration/4.9-localization-system/architecture.md`
    /// **Note**: These extension methods are in the Localization project to avoid circular dependencies.
    /// </remarks>
    public static class StatExtensions
    {
        /// <summary>
        /// Gets the localized display name for a Stat.
        /// </summary>
        /// <param name="stat">The Stat.</param>
        /// <param name="localizationProvider">The localization provider to use.</param>
        /// <returns>The localized stat name, or the English name if translation is not found.</returns>
        public static string GetDisplayName(this Stat stat, ILocalizationProvider localizationProvider)
        {
            if (localizationProvider == null)
                return stat.ToString();

            var key = GetLocalizationKey(stat);
            if (localizationProvider.HasString(key))
            {
                return localizationProvider.GetString(key);
            }

            // Fallback to English name
            return stat.ToString();
        }

        /// <summary>
        /// Gets the localization key for a Stat.
        /// </summary>
        private static string GetLocalizationKey(Stat stat)
        {
            switch (stat)
            {
                case Stat.HP:
                    return LocalizationKey.Stat_HP;
                case Stat.Attack:
                    return LocalizationKey.Stat_Attack;
                case Stat.Defense:
                    return LocalizationKey.Stat_Defense;
                case Stat.SpAttack:
                    return LocalizationKey.Stat_SpAttack;
                case Stat.SpDefense:
                    return LocalizationKey.Stat_SpDefense;
                case Stat.Speed:
                    return LocalizationKey.Stat_Speed;
                case Stat.Accuracy:
                    return LocalizationKey.Stat_Accuracy;
                case Stat.Evasion:
                    return LocalizationKey.Stat_Evasion;
                default:
                    return stat.ToString();
            }
        }
    }
}
