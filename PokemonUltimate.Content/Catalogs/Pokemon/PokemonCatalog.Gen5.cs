// Pokemon names and game data referenced here are trademarks of Nintendo/Game Freak/The Pokemon Company.
// This is a non-commercial fan project for educational purposes only. See LEGAL.md for details.

using PokemonUltimate.Content.Catalogs.Abilities;
using PokemonUltimate.Content.Extensions;
using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Core.Data.Enums;

namespace PokemonUltimate.Content.Catalogs.Pokemon
{
    /// <summary>
    /// Generation 5 Pokemon (Unova region, #494-649).
    /// </summary>
    /// <remarks>
    /// **Feature**: 3: Content Expansion
    /// **Sub-Feature**: 3.1: Pokemon Expansion
    /// **Documentation**: See `docs/features/3-content-expansion/3.1-pokemon-expansion/README.md`
    /// </remarks>
    public static partial class PokemonCatalog
    {
        // ===== GRASS/STEEL LINE (Ferrothorn → Ferroseed) =====

        /// <summary>
        /// The Thorn Pod Pokemon. Grass/Steel type. Has Iron Barbs ability.
        /// </summary>
        public static readonly PokemonSpeciesData Ferrothorn = Core.Infrastructure.Builders.Pokemon.Define("Ferrothorn", 598)
            .Types(PokemonType.Grass, PokemonType.Steel)
            .Stats(74, 94, 131, 54, 116, 20)
            .GenderRatio(50f)
            .Abilities(AbilityCatalog.IronBarbs, AbilityCatalog.Anticipation)
            .HiddenAbility(AbilityCatalog.IronBarbs)
            .BaseExp(171)
            .CatchRate(90)
            .BaseFriendship(70)
            .GrowthRate(GrowthRate.MediumFast)
            .Build()
            .WithPokedexData()
            .WithLearnset();

        /// <summary>
        /// The Thorn Seed Pokemon. Grass/Steel type. Has Iron Barbs ability.
        /// </summary>
        public static readonly PokemonSpeciesData Ferroseed = Core.Infrastructure.Builders.Pokemon.Define("Ferroseed", 597)
            .Types(PokemonType.Grass, PokemonType.Steel)
            .Stats(44, 50, 91, 24, 86, 10)
            .GenderRatio(50f)
            .Abilities(AbilityCatalog.IronBarbs, AbilityCatalog.Anticipation)
            .HiddenAbility(AbilityCatalog.IronBarbs)
            .EvolvesTo(Ferrothorn, e => e.AtLevel(40))
            .BaseExp(61)
            .CatchRate(255)
            .BaseFriendship(70)
            .GrowthRate(GrowthRate.MediumFast)
            .Build()
            .WithPokedexData()
            .WithLearnset();

        // ===== REGISTRATION =====

        static partial void RegisterGen5()
        {
            // Grass/Steel line
            _all.Add(Ferroseed);
            _all.Add(Ferrothorn);
        }
    }
}

