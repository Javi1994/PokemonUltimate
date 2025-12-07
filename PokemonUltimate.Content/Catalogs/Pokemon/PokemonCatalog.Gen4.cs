// Pokemon names and game data referenced here are trademarks of Nintendo/Game Freak/The Pokemon Company.
// This is a non-commercial fan project for educational purposes only. See LEGAL.md for details.

using PokemonUltimate.Content.Catalogs.Abilities;
using PokemonUltimate.Content.Extensions;
using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Core.Data.Enums;

namespace PokemonUltimate.Content.Catalogs.Pokemon
{
    /// <summary>
    /// Generation 4 Pokemon (Sinnoh region, #387-493).
    /// </summary>
    /// <remarks>
    /// **Feature**: 3: Content Expansion
    /// **Sub-Feature**: 3.1: Pokemon Expansion
    /// **Documentation**: See `docs/features/3-content-expansion/3.1-pokemon-expansion/README.md`
    /// </remarks>
    public static partial class PokemonCatalog
    {
        // ===== GRASS/ICE LINE (Abomasnow → Snover) =====

        /// <summary>
        /// The Frost Tree Pokemon. Grass/Ice type. Has Snow Warning ability.
        /// </summary>
        public static readonly PokemonSpeciesData Abomasnow = Core.Infrastructure.Builders.Pokemon.Define("Abomasnow", 460)
            .Types(PokemonType.Grass, PokemonType.Ice)
            .Stats(90, 92, 75, 92, 85, 60)
            .GenderRatio(50f)
            .Abilities(AbilityCatalog.SnowWarning, AbilityCatalog.Soundproof)
            .HiddenAbility(AbilityCatalog.SlushRush)
            .BaseExp(173)
            .CatchRate(60)
            .BaseFriendship(70)
            .GrowthRate(GrowthRate.Slow)
            .Build()
            .WithPokedexData()
            .WithLearnset();

        /// <summary>
        /// The Frost Tree Pokemon. Grass/Ice type. Has Snow Warning ability.
        /// </summary>
        public static readonly PokemonSpeciesData Snover = Core.Infrastructure.Builders.Pokemon.Define("Snover", 459)
            .Types(PokemonType.Grass, PokemonType.Ice)
            .Stats(60, 62, 50, 62, 60, 40)
            .GenderRatio(50f)
            .Abilities(AbilityCatalog.SnowWarning, AbilityCatalog.Soundproof)
            .HiddenAbility(AbilityCatalog.SlushRush)
            .EvolvesTo(Abomasnow, e => e.AtLevel(40))
            .BaseExp(67)
            .CatchRate(120)
            .BaseFriendship(70)
            .GrowthRate(GrowthRate.Slow)
            .Build()
            .WithPokedexData()
            .WithLearnset();

        // ===== REGISTRATION =====

        static partial void RegisterGen4()
        {
            // Grass/Ice line
            _all.Add(Snover);
            _all.Add(Abomasnow);
        }
    }
}

