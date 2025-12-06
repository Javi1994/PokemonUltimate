// Pokemon names and game data referenced here are trademarks of Nintendo/Game Freak/The Pokemon Company.
// This is a non-commercial fan project for educational purposes only. See LEGAL.md for details.

using PokemonUltimate.Content.Builders;
using PokemonUltimate.Content.Catalogs.Abilities;
using PokemonUltimate.Content.Extensions;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Content.Catalogs.Pokemon
{
    /// <summary>
    /// Generation 3 Pokemon (Hoenn region, #252-386).
    /// </summary>
    /// <remarks>
    /// **Feature**: 3: Content Expansion
    /// **Sub-Feature**: 3.1: Pokemon Expansion
    /// **Documentation**: See `docs/features/3-content-expansion/3.1-pokemon-expansion/README.md`
    /// </remarks>
    public static partial class PokemonCatalog
    {
        // ===== NORMAL LINE (Slaking → Vigoroth → Slakoth) =====
        // Defined in reverse evolution order

        /// <summary>
        /// The Apex Pokemon. Normal type. Has Truant ability.
        /// </summary>
        public static readonly PokemonSpeciesData Slaking = Builders.Pokemon.Define("Slaking", 289)
            .Type(PokemonType.Normal)
            .Stats(150, 160, 100, 95, 65, 100)
            .GenderRatio(50f)
            .Ability(AbilityCatalog.Truant)
            .BaseExp(252)
            .CatchRate(45)
            .BaseFriendship(70)
            .GrowthRate(GrowthRate.Slow)
            .Build()
            .WithPokedexData()
            .WithLearnset();

        /// <summary>
        /// The Wild Monkey Pokemon. Normal type. Has Vital Spirit ability.
        /// </summary>
        public static readonly PokemonSpeciesData Vigoroth = Builders.Pokemon.Define("Vigoroth", 288)
            .Type(PokemonType.Normal)
            .Stats(80, 80, 80, 55, 55, 90)
            .GenderRatio(50f)
            .Ability(AbilityCatalog.VitalSpirit)
            .EvolvesTo(Slaking, e => e.AtLevel(36))
            .BaseExp(154)
            .CatchRate(120)
            .BaseFriendship(70)
            .GrowthRate(GrowthRate.Slow)
            .Build()
            .WithPokedexData()
            .WithLearnset();

        /// <summary>
        /// The Slacker Pokemon. Normal type. Has Truant ability.
        /// </summary>
        public static readonly PokemonSpeciesData Slakoth = Builders.Pokemon.Define("Slakoth", 287)
            .Type(PokemonType.Normal)
            .Stats(60, 60, 60, 35, 35, 30)
            .GenderRatio(50f)
            .Ability(AbilityCatalog.Truant)
            .EvolvesTo(Vigoroth, e => e.AtLevel(18))
            .BaseExp(56)
            .CatchRate(255)
            .BaseFriendship(70)
            .GrowthRate(GrowthRate.Slow)
            .Build()
            .WithPokedexData()
            .WithLearnset();

        // ===== WATER/DARK LINE (Sharpedo → Carvanha) =====

        /// <summary>
        /// The Brutal Pokemon. Water/Dark type. Has Speed Boost ability.
        /// </summary>
        public static readonly PokemonSpeciesData Sharpedo = Builders.Pokemon.Define("Sharpedo", 319)
            .Types(PokemonType.Water, PokemonType.Dark)
            .Stats(70, 120, 40, 95, 40, 95)
            .GenderRatio(50f)
            .Abilities(AbilityCatalog.RoughSkin, AbilityCatalog.SpeedBoost)
            .HiddenAbility(AbilityCatalog.SpeedBoost)
            .BaseExp(161)
            .CatchRate(60)
            .BaseFriendship(35)
            .GrowthRate(GrowthRate.Slow)
            .Build()
            .WithPokedexData()
            .WithLearnset();

        /// <summary>
        /// The Savage Pokemon. Water/Dark type. Has Rough Skin ability.
        /// </summary>
        public static readonly PokemonSpeciesData Carvanha = Builders.Pokemon.Define("Carvanha", 318)
            .Types(PokemonType.Water, PokemonType.Dark)
            .Stats(45, 90, 20, 65, 20, 65)
            .GenderRatio(50f)
            .Abilities(AbilityCatalog.RoughSkin, AbilityCatalog.SpeedBoost)
            .HiddenAbility(AbilityCatalog.SpeedBoost)
            .EvolvesTo(Sharpedo, e => e.AtLevel(30))
            .BaseExp(61)
            .CatchRate(225)
            .BaseFriendship(35)
            .GrowthRate(GrowthRate.Slow)
            .Build()
            .WithPokedexData()
            .WithLearnset();

        // ===== REGISTRATION =====

        static partial void RegisterGen3()
        {
            // Normal line
            _all.Add(Slakoth);
            _all.Add(Vigoroth);
            _all.Add(Slaking);
            // Water/Dark line
            _all.Add(Carvanha);
            _all.Add(Sharpedo);
        }
    }
}

