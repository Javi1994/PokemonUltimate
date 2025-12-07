using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Core.Infrastructure.Builders;
using PokemonUltimate.Core.Data.Enums;

namespace PokemonUltimate.Content.Catalogs.Abilities
{
    /// <summary>
    /// Generation 4 abilities (Diamond/Pearl/Platinum).
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
        /// Doubles Speed in sandstorm.
        /// </summary>
        public static readonly AbilityData SandRush = Ability.Define("Sand Rush")
            .Description("Doubles Speed in sandstorm.")
            .Gen(4)
            .SpeedBoostInWeather(Core.Data.Enums.Weather.Sandstorm, 2.0f)
            .OnWeatherChange()
            .Build();

        // ===== WEATHER ABILITIES =====

        /// <summary>
        /// Summons hail on switch-in.
        /// </summary>
        public static readonly AbilityData SnowWarning = Ability.Define("Snow Warning")
            .Description("Summons a hailstorm on entry.")
            .Gen(4)
            .SummonsWeather(Core.Data.Enums.Weather.Hail)
            .Build();

        // ===== REGISTRATION =====

        static partial void RegisterGen4Abilities()
        {
            _all.Add(SandRush);
            _all.Add(SnowWarning);
        }
    }
}

