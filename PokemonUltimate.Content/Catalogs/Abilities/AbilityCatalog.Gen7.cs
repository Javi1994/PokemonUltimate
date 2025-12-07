using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Core.Infrastructure.Builders;
using PokemonUltimate.Core.Data.Enums;

namespace PokemonUltimate.Content.Catalogs.Abilities
{
    /// <summary>
    /// Generation 7 abilities (Sun/Moon).
    /// </summary>
    /// <remarks>
    /// **Feature**: 3: Content Expansion
    /// **Sub-Feature**: 3.4: Ability Expansion
    /// **Documentation**: See `docs/features/3-content-expansion/3.4-ability-expansion/README.md`
    /// </remarks>
    public static partial class AbilityCatalog
    {
        // ===== WEATHER ABILITIES =====

        /// <summary>
        /// Doubles Speed in hail.
        /// </summary>
        public static readonly AbilityData SlushRush = Ability.Define("Slush Rush")
            .Description("Doubles Speed in hail.")
            .Gen(7)
            .SpeedBoostInWeather(Core.Data.Enums.Weather.Hail, 2.0f)
            .OnWeatherChange()
            .Build();

        // ===== REGISTRATION =====

        static partial void RegisterGen7Abilities()
        {
            _all.Add(SlushRush);
        }
    }
}

