using PokemonUltimate.Content.Catalogs.Abilities;
using PokemonUltimate.Content.Extensions;
using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Core.Data.Enums;

namespace PokemonUltimate.Content.Catalogs.Pokemon
{
    /// <summary>
    /// Generation 1 Pokemon (Kanto region, #001-151).
    /// Defined in reverse evolution order so evolutions can reference targets.
    /// </summary>
    /// <remarks>
    /// **Feature**: 3: Content Expansion
    /// **Sub-Feature**: 3.1: Pokemon Expansion
    /// **Documentation**: See `docs/features/3-content-expansion/3.1-pokemon-expansion/README.md`
    /// </remarks>
    public static partial class PokemonCatalog
    {
        // ===== GRASS STARTER LINE (Venusaur → Ivysaur → Bulbasaur) =====
        // Starters have 87.5% male ratio

        public static readonly PokemonSpeciesData Venusaur = Core.Infrastructure.Builders.Pokemon.Define("Venusaur", 3)
            .Types(PokemonType.Grass, PokemonType.Poison)
            .Stats(80, 82, 83, 100, 100, 80)
            .GenderRatio(87.5f)
            .Ability(AbilityCatalog.Overgrow)
            .HiddenAbility(AbilityCatalog.Chlorophyll)
            .Build()
            .WithPokedexData()
            .WithLearnset();

        public static readonly PokemonSpeciesData Ivysaur = Core.Infrastructure.Builders.Pokemon.Define("Ivysaur", 2)
            .Types(PokemonType.Grass, PokemonType.Poison)
            .Stats(60, 62, 63, 80, 80, 60)
            .GenderRatio(87.5f)
            .Ability(AbilityCatalog.Overgrow)
            .HiddenAbility(AbilityCatalog.Chlorophyll)
            .EvolvesTo(Venusaur, e => e.AtLevel(32))
            .Build()
            .WithPokedexData()
            .WithLearnset();

        public static readonly PokemonSpeciesData Bulbasaur = Core.Infrastructure.Builders.Pokemon.Define("Bulbasaur", 1)
            .Types(PokemonType.Grass, PokemonType.Poison)
            .Stats(45, 49, 49, 65, 65, 45)
            .GenderRatio(87.5f)
            .Ability(AbilityCatalog.Overgrow)
            .HiddenAbility(AbilityCatalog.Chlorophyll)
            .BaseExp(64)
            .CatchRate(45)
            .BaseFriendship(70)
            .GrowthRate(GrowthRate.MediumSlow)
            .EvolvesTo(Ivysaur, e => e.AtLevel(16))
            .Build()
            .WithPokedexData()
            .WithLearnset();

        // ===== FIRE STARTER LINE (Charizard → Charmeleon → Charmander) =====
        // Starters have 87.5% male ratio

        public static readonly PokemonSpeciesData Charizard = Core.Infrastructure.Builders.Pokemon.Define("Charizard", 6)
            .Types(PokemonType.Fire, PokemonType.Flying)
            .Stats(78, 84, 78, 109, 85, 100)
            .GenderRatio(87.5f)
            .Ability(AbilityCatalog.Blaze)
            .HiddenAbility(AbilityCatalog.SolarPower)
            .Build()
            .WithPokedexData()
            .WithLearnset();

        public static readonly PokemonSpeciesData Charmeleon = Core.Infrastructure.Builders.Pokemon.Define("Charmeleon", 5)
            .Type(PokemonType.Fire)
            .Stats(58, 64, 58, 80, 65, 80)
            .GenderRatio(87.5f)
            .Ability(AbilityCatalog.Blaze)
            .HiddenAbility(AbilityCatalog.SolarPower)
            .EvolvesTo(Charizard, e => e.AtLevel(36))
            .Build()
            .WithPokedexData()
            .WithLearnset();

        public static readonly PokemonSpeciesData Charmander = Core.Infrastructure.Builders.Pokemon.Define("Charmander", 4)
            .Type(PokemonType.Fire)
            .Stats(39, 52, 43, 60, 50, 65)
            .GenderRatio(87.5f)
            .Ability(AbilityCatalog.Blaze)
            .HiddenAbility(AbilityCatalog.SolarPower)
            .BaseExp(62)
            .CatchRate(45)
            .BaseFriendship(70)
            .GrowthRate(GrowthRate.MediumSlow)
            .EvolvesTo(Charmeleon, e => e.AtLevel(16))
            .Build()
            .WithPokedexData()
            .WithLearnset();

        // ===== WATER STARTER LINE (Blastoise → Wartortle → Squirtle) =====
        // Starters have 87.5% male ratio

        public static readonly PokemonSpeciesData Blastoise = Core.Infrastructure.Builders.Pokemon.Define("Blastoise", 9)
            .Type(PokemonType.Water)
            .Stats(79, 83, 100, 85, 105, 78)
            .GenderRatio(87.5f)
            .Ability(AbilityCatalog.Torrent)
            .HiddenAbility(AbilityCatalog.RainDish)
            .Build()
            .WithPokedexData()
            .WithLearnset();

        public static readonly PokemonSpeciesData Wartortle = Core.Infrastructure.Builders.Pokemon.Define("Wartortle", 8)
            .Type(PokemonType.Water)
            .Stats(59, 63, 80, 65, 80, 58)
            .GenderRatio(87.5f)
            .Ability(AbilityCatalog.Torrent)
            .HiddenAbility(AbilityCatalog.RainDish)
            .EvolvesTo(Blastoise, e => e.AtLevel(36))
            .Build()
            .WithPokedexData()
            .WithLearnset();

        public static readonly PokemonSpeciesData Squirtle = Core.Infrastructure.Builders.Pokemon.Define("Squirtle", 7)
            .Type(PokemonType.Water)
            .Stats(44, 48, 65, 50, 64, 43)
            .GenderRatio(87.5f)
            .Ability(AbilityCatalog.Torrent)
            .HiddenAbility(AbilityCatalog.RainDish)
            .EvolvesTo(Wartortle, e => e.AtLevel(16))
            .Build()
            .WithPokedexData()
            .WithLearnset();

        // ===== ELECTRIC LINE (Raichu → Pikachu) =====

        public static readonly PokemonSpeciesData Raichu = Core.Infrastructure.Builders.Pokemon.Define("Raichu", 26)
            .Type(PokemonType.Electric)
            .Stats(60, 90, 55, 90, 80, 110)
            .Ability(AbilityCatalog.Static)
            .HiddenAbility(AbilityCatalog.LightningRod)
            .Build()
            .WithPokedexData()
            .WithLearnset();

        public static readonly PokemonSpeciesData Pikachu = Core.Infrastructure.Builders.Pokemon.Define("Pikachu", 25)
            .Type(PokemonType.Electric)
            .Stats(35, 55, 40, 50, 50, 90)
            .GenderRatio(50f)
            .Ability(AbilityCatalog.Static)
            .HiddenAbility(AbilityCatalog.LightningRod)
            .BaseExp(112)
            .CatchRate(190)
            .BaseFriendship(70)
            .GrowthRate(GrowthRate.MediumFast)
            .EvolvesTo(Raichu, e => e.WithItem("Thunder Stone"))
            .Build()
            .WithPokedexData() // Apply Pokedex data from PokedexDataProvider
            .WithLearnset(); // Apply learnset data from LearnsetProvider

        // ===== NORMAL POKEMON =====

        public static readonly PokemonSpeciesData Eevee = Core.Infrastructure.Builders.Pokemon.Define("Eevee", 133)
            .Type(PokemonType.Normal)
            .Stats(55, 55, 50, 45, 65, 55)
            .GenderRatio(87.5f)
            .Abilities(AbilityCatalog.RunAway, AbilityCatalog.Adaptability)
            .HiddenAbility(AbilityCatalog.Anticipation)
            .Build()
            .WithLearnset(); // Apply learnset data from LearnsetProvider
        // Note: Eevee evolutions would reference Vaporeon, Jolteon, etc.
        // which aren't defined yet. We'll add them when we have those Pokemon.

        public static readonly PokemonSpeciesData Snorlax = Core.Infrastructure.Builders.Pokemon.Define("Snorlax", 143)
            .Type(PokemonType.Normal)
            .Stats(160, 110, 65, 65, 110, 30)
            .GenderRatio(87.5f)
            .Abilities(AbilityCatalog.Immunity, AbilityCatalog.ThickFat)
            .HiddenAbility(AbilityCatalog.Gluttony)
            .Moves(m => m
                .StartsWith(Moves.MoveCatalog.Tackle, Moves.MoveCatalog.Growl)
                .AtLevel(35, Moves.MoveCatalog.HyperBeam)
                .ByTM(Moves.MoveCatalog.Earthquake, Moves.MoveCatalog.Psychic))
            .Build();

        // ===== GHOST/POISON LINE (Gengar → Haunter → Gastly) =====

        public static readonly PokemonSpeciesData Gengar = Core.Infrastructure.Builders.Pokemon.Define("Gengar", 94)
            .Types(PokemonType.Ghost, PokemonType.Poison)
            .Stats(60, 65, 60, 130, 75, 110)
            .GenderRatio(50f)
            .Ability(AbilityCatalog.Levitate)
            .Build()
            .WithPokedexData()
            .WithLearnset();

        public static readonly PokemonSpeciesData Haunter = Core.Infrastructure.Builders.Pokemon.Define("Haunter", 93)
            .Types(PokemonType.Ghost, PokemonType.Poison)
            .Stats(45, 50, 45, 115, 55, 95)
            .GenderRatio(50f)
            .Ability(AbilityCatalog.Levitate)
            .EvolvesTo(Gengar, e => e.ByTrade())
            .Build()
            .WithPokedexData()
            .WithLearnset();

        public static readonly PokemonSpeciesData Gastly = Core.Infrastructure.Builders.Pokemon.Define("Gastly", 92)
            .Types(PokemonType.Ghost, PokemonType.Poison)
            .Stats(30, 35, 30, 100, 35, 80)
            .GenderRatio(50f)
            .Ability(AbilityCatalog.Levitate)
            .EvolvesTo(Haunter, e => e.AtLevel(25))
            .Build()
            .WithPokedexData()
            .WithLearnset();

        // ===== ROCK/GROUND LINE (Golem → Graveler → Geodude) =====

        public static readonly PokemonSpeciesData Golem = Core.Infrastructure.Builders.Pokemon.Define("Golem", 76)
            .Types(PokemonType.Rock, PokemonType.Ground)
            .Stats(80, 120, 130, 55, 65, 45)
            .GenderRatio(50f)
            .Abilities(AbilityCatalog.Sturdy, AbilityCatalog.ClearBody)
            .HiddenAbility(AbilityCatalog.SandStream)
            .Build()
            .WithPokedexData()
            .WithLearnset();

        public static readonly PokemonSpeciesData Graveler = Core.Infrastructure.Builders.Pokemon.Define("Graveler", 75)
            .Types(PokemonType.Rock, PokemonType.Ground)
            .Stats(55, 95, 115, 45, 45, 35)
            .GenderRatio(50f)
            .Abilities(AbilityCatalog.Sturdy, AbilityCatalog.ClearBody)
            .HiddenAbility(AbilityCatalog.SandStream)
            .EvolvesTo(Golem, e => e.ByTrade())
            .Build()
            .WithPokedexData()
            .WithLearnset();

        public static readonly PokemonSpeciesData Geodude = Core.Infrastructure.Builders.Pokemon.Define("Geodude", 74)
            .Types(PokemonType.Rock, PokemonType.Ground)
            .Stats(40, 80, 100, 30, 30, 20)
            .GenderRatio(50f)
            .Abilities(AbilityCatalog.Sturdy, AbilityCatalog.ClearBody)
            .HiddenAbility(AbilityCatalog.SandStream)
            .EvolvesTo(Graveler, e => e.AtLevel(25))
            .Build()
            .WithPokedexData()
            .WithLearnset();

        // ===== WATER/FLYING LINE (Gyarados → Magikarp) =====

        public static readonly PokemonSpeciesData Gyarados = Core.Infrastructure.Builders.Pokemon.Define("Gyarados", 130)
            .Types(PokemonType.Water, PokemonType.Flying)
            .Stats(95, 125, 79, 60, 100, 81)
            .GenderRatio(50f)
            .Ability(AbilityCatalog.Intimidate)
            .HiddenAbility(AbilityCatalog.SpeedBoost)
            .Moves(m => m
                .StartsWith(Moves.MoveCatalog.Tackle, Moves.MoveCatalog.Splash)
                .AtLevel(21, Moves.MoveCatalog.Waterfall)
                .AtLevel(30, Moves.MoveCatalog.DragonRage)
                .AtLevel(50, Moves.MoveCatalog.HydroPump))
            .Build();

        public static readonly PokemonSpeciesData Magikarp = Core.Infrastructure.Builders.Pokemon.Define("Magikarp", 129)
            .Type(PokemonType.Water)
            .Stats(20, 10, 55, 15, 20, 80)
            .GenderRatio(50f)
            .Ability(AbilityCatalog.SwiftSwim)
            .HiddenAbility(AbilityCatalog.RoughSkin)
            .Moves(m => m
                .StartsWith(Moves.MoveCatalog.Splash, Moves.MoveCatalog.Tackle))
            .EvolvesTo(Gyarados, e => e.AtLevel(20))
            .Build();

        // ===== PSYCHIC LINE (Alakazam → Kadabra → Abra) =====

        public static readonly PokemonSpeciesData Alakazam = Core.Infrastructure.Builders.Pokemon.Define("Alakazam", 65)
            .Type(PokemonType.Psychic)
            .Stats(55, 50, 45, 135, 95, 120)
            .GenderRatio(75f)
            .Abilities(AbilityCatalog.Synchronize, AbilityCatalog.ClearBody)
            .HiddenAbility(AbilityCatalog.Synchronize)
            .Moves(m => m
                .StartsWith(Moves.MoveCatalog.Teleport, Moves.MoveCatalog.Confusion)
                .AtLevel(16, Moves.MoveCatalog.Psybeam)
                .AtLevel(36, Moves.MoveCatalog.Psychic)
                .ByTM(Moves.MoveCatalog.Thunderbolt, Moves.MoveCatalog.FireBlast))
            .Build();

        public static readonly PokemonSpeciesData Kadabra = Core.Infrastructure.Builders.Pokemon.Define("Kadabra", 64)
            .Type(PokemonType.Psychic)
            .Stats(40, 35, 30, 120, 70, 105)
            .GenderRatio(75f)
            .Abilities(AbilityCatalog.Synchronize, AbilityCatalog.ClearBody)
            .HiddenAbility(AbilityCatalog.Synchronize)
            .Moves(m => m
                .StartsWith(Moves.MoveCatalog.Teleport, Moves.MoveCatalog.Confusion)
                .AtLevel(16, Moves.MoveCatalog.Psybeam)
                .AtLevel(36, Moves.MoveCatalog.Psychic))
            .EvolvesTo(Alakazam, e => e.ByTrade())
            .Build();

        public static readonly PokemonSpeciesData Abra = Core.Infrastructure.Builders.Pokemon.Define("Abra", 63)
            .Type(PokemonType.Psychic)
            .Stats(25, 20, 15, 105, 55, 90)
            .GenderRatio(75f)
            .Abilities(AbilityCatalog.Synchronize, AbilityCatalog.ClearBody)
            .HiddenAbility(AbilityCatalog.Synchronize)
            .Moves(m => m
                .StartsWith(Moves.MoveCatalog.Teleport)
                .AtLevel(16, Moves.MoveCatalog.Confusion))
            .EvolvesTo(Kadabra, e => e.AtLevel(16))
            .Build();

        // ===== LEGENDARY/MYTHICAL =====
        // Legendaries are typically genderless

        public static readonly PokemonSpeciesData Mewtwo = Core.Infrastructure.Builders.Pokemon.Define("Mewtwo", 150)
            .Type(PokemonType.Psychic)
            .Stats(106, 110, 90, 154, 90, 130)
            .Genderless()
            .Ability(AbilityCatalog.Pressure)
            .HiddenAbility(AbilityCatalog.Unnerve)
            .Build()
            .WithPokedexData()
            .WithLearnset();

        public static readonly PokemonSpeciesData Mew = Core.Infrastructure.Builders.Pokemon.Define("Mew", 151)
            .Type(PokemonType.Psychic)
            .Stats(100, 100, 100, 100, 100, 100)
            .Genderless()
            .Ability(AbilityCatalog.Synchronize)
            .Build()
            .WithPokedexData()
            .WithLearnset();

        // ===== REGISTRATION =====

        static partial void RegisterGen1()
        {
            // Grass line
            _all.Add(Bulbasaur);
            _all.Add(Ivysaur);
            _all.Add(Venusaur);
            // Fire line
            _all.Add(Charmander);
            _all.Add(Charmeleon);
            _all.Add(Charizard);
            // Water line
            _all.Add(Squirtle);
            _all.Add(Wartortle);
            _all.Add(Blastoise);
            // Electric
            _all.Add(Pikachu);
            _all.Add(Raichu);
            // Normal
            _all.Add(Eevee);
            _all.Add(Snorlax);
            // Ghost/Poison
            _all.Add(Gastly);
            _all.Add(Haunter);
            _all.Add(Gengar);
            // Rock/Ground
            _all.Add(Geodude);
            _all.Add(Graveler);
            _all.Add(Golem);
            // Water/Flying
            _all.Add(Magikarp);
            _all.Add(Gyarados);
            // Psychic
            _all.Add(Abra);
            _all.Add(Kadabra);
            _all.Add(Alakazam);
            // Legendary
            _all.Add(Mewtwo);
            _all.Add(Mew);
        }
    }
}
