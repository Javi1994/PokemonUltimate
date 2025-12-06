using PokemonUltimate.Content.Builders;
using PokemonUltimate.Content.Catalogs.Abilities;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Enums;

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

        public static readonly PokemonSpeciesData Venusaur = Builders.Pokemon.Define("Venusaur", 3)
            .Types(PokemonType.Grass, PokemonType.Poison)
            .Stats(80, 82, 83, 100, 100, 80)
            .GenderRatio(87.5f)
            .Ability(AbilityCatalog.Overgrow)
            .HiddenAbility(AbilityCatalog.Chlorophyll)
            .Moves(m => m
                .StartsWith(Moves.MoveCatalog.VineWhip, Moves.MoveCatalog.Growl)
                .AtLevel(32, Moves.MoveCatalog.RazorLeaf)
                .AtLevel(65, Moves.MoveCatalog.SolarBeam))
            .Build();

        public static readonly PokemonSpeciesData Ivysaur = Builders.Pokemon.Define("Ivysaur", 2)
            .Types(PokemonType.Grass, PokemonType.Poison)
            .Stats(60, 62, 63, 80, 80, 60)
            .GenderRatio(87.5f)
            .Ability(AbilityCatalog.Overgrow)
            .HiddenAbility(AbilityCatalog.Chlorophyll)
            .Moves(m => m
                .StartsWith(Moves.MoveCatalog.VineWhip, Moves.MoveCatalog.Growl)
                .AtLevel(22, Moves.MoveCatalog.RazorLeaf))
            .EvolvesTo(Venusaur, e => e.AtLevel(32))
            .Build();

        public static readonly PokemonSpeciesData Bulbasaur = Builders.Pokemon.Define("Bulbasaur", 1)
            .Types(PokemonType.Grass, PokemonType.Poison)
            .Stats(45, 49, 49, 64, 64, 45)
            .GenderRatio(87.5f)
            .Ability(AbilityCatalog.Overgrow)
            .HiddenAbility(AbilityCatalog.Chlorophyll)
            .BaseExp(64)
            .CatchRate(45)
            .BaseFriendship(70)
            .GrowthRate(GrowthRate.MediumSlow)
            .Description("A strange seed was planted on its back at birth. The plant sprouts and grows with this Pokemon.")
            .Category("Seed Pokemon")
            .Height(0.7f)
            .Weight(6.9f)
            .Color(PokemonColor.Green)
            .Shape(PokemonShape.Quadruped)
            .Habitat(PokemonHabitat.Grassland)
            .Moves(m => m
                .StartsWith(Moves.MoveCatalog.Tackle, Moves.MoveCatalog.Growl)
                .AtLevel(9, Moves.MoveCatalog.VineWhip)
                .AtLevel(27, Moves.MoveCatalog.RazorLeaf))
            .EvolvesTo(Ivysaur, e => e.AtLevel(16))
            .Build();

        // ===== FIRE STARTER LINE (Charizard → Charmeleon → Charmander) =====
        // Starters have 87.5% male ratio

        public static readonly PokemonSpeciesData Charizard = Builders.Pokemon.Define("Charizard", 6)
            .Types(PokemonType.Fire, PokemonType.Flying)
            .Stats(78, 84, 78, 109, 85, 100)
            .GenderRatio(87.5f)
            .Ability(AbilityCatalog.Blaze)
            .HiddenAbility(AbilityCatalog.SolarPower)
            .Moves(m => m
                .StartsWith(Moves.MoveCatalog.Scratch, Moves.MoveCatalog.Ember)
                .AtLevel(46, Moves.MoveCatalog.Flamethrower)
                .ByTM(Moves.MoveCatalog.FireBlast, Moves.MoveCatalog.Earthquake))
            .Build();

        public static readonly PokemonSpeciesData Charmeleon = Builders.Pokemon.Define("Charmeleon", 5)
            .Type(PokemonType.Fire)
            .Stats(58, 64, 58, 80, 65, 80)
            .GenderRatio(87.5f)
            .Ability(AbilityCatalog.Blaze)
            .HiddenAbility(AbilityCatalog.SolarPower)
            .Moves(m => m
                .StartsWith(Moves.MoveCatalog.Scratch, Moves.MoveCatalog.Growl)
                .AtLevel(17, Moves.MoveCatalog.Ember)
                .AtLevel(39, Moves.MoveCatalog.Flamethrower))
            .EvolvesTo(Charizard, e => e.AtLevel(36))
            .Build();

        public static readonly PokemonSpeciesData Charmander = Builders.Pokemon.Define("Charmander", 4)
            .Type(PokemonType.Fire)
            .Stats(39, 52, 43, 60, 50, 65)
            .GenderRatio(87.5f)
            .Ability(AbilityCatalog.Blaze)
            .HiddenAbility(AbilityCatalog.SolarPower)
            .BaseExp(62)
            .CatchRate(45)
            .BaseFriendship(70)
            .GrowthRate(GrowthRate.MediumSlow)
            .Description("Obviously prefers hot places. When it rains, steam is said to spout from the tip of its tail.")
            .Category("Lizard Pokemon")
            .Height(0.6f)
            .Weight(8.5f)
            .Color(PokemonColor.Red)
            .Shape(PokemonShape.Quadruped)
            .Habitat(PokemonHabitat.Mountain)
            .Moves(m => m
                .StartsWith(Moves.MoveCatalog.Scratch, Moves.MoveCatalog.Growl)
                .AtLevel(9, Moves.MoveCatalog.Ember)
                .AtLevel(38, Moves.MoveCatalog.Flamethrower))
            .EvolvesTo(Charmeleon, e => e.AtLevel(16))
            .Build();

        // ===== WATER STARTER LINE (Blastoise → Wartortle → Squirtle) =====
        // Starters have 87.5% male ratio

        public static readonly PokemonSpeciesData Blastoise = Builders.Pokemon.Define("Blastoise", 9)
            .Type(PokemonType.Water)
            .Stats(79, 83, 100, 85, 105, 78)
            .GenderRatio(87.5f)
            .Ability(AbilityCatalog.Torrent)
            .HiddenAbility(AbilityCatalog.RainDish)
            .Moves(m => m
                .StartsWith(Moves.MoveCatalog.Tackle, Moves.MoveCatalog.WaterGun)
                .AtLevel(42, Moves.MoveCatalog.Surf)
                .AtLevel(52, Moves.MoveCatalog.HydroPump))
            .Build();

        public static readonly PokemonSpeciesData Wartortle = Builders.Pokemon.Define("Wartortle", 8)
            .Type(PokemonType.Water)
            .Stats(59, 63, 80, 65, 80, 58)
            .GenderRatio(87.5f)
            .Ability(AbilityCatalog.Torrent)
            .HiddenAbility(AbilityCatalog.RainDish)
            .Moves(m => m
                .StartsWith(Moves.MoveCatalog.Tackle, Moves.MoveCatalog.WaterGun)
                .AtLevel(28, Moves.MoveCatalog.Surf))
            .EvolvesTo(Blastoise, e => e.AtLevel(36))
            .Build();

        public static readonly PokemonSpeciesData Squirtle = Builders.Pokemon.Define("Squirtle", 7)
            .Type(PokemonType.Water)
            .Stats(44, 48, 65, 50, 64, 43)
            .GenderRatio(87.5f)
            .Ability(AbilityCatalog.Torrent)
            .HiddenAbility(AbilityCatalog.RainDish)
            .Moves(m => m
                .StartsWith(Moves.MoveCatalog.Tackle, Moves.MoveCatalog.Growl)
                .AtLevel(8, Moves.MoveCatalog.WaterGun)
                .AtLevel(25, Moves.MoveCatalog.Surf))
            .EvolvesTo(Wartortle, e => e.AtLevel(16))
            .Build();

        // ===== ELECTRIC LINE (Raichu → Pikachu) =====

        public static readonly PokemonSpeciesData Raichu = Builders.Pokemon.Define("Raichu", 26)
            .Type(PokemonType.Electric)
            .Stats(60, 90, 55, 90, 80, 110)
            .Ability(AbilityCatalog.Static)
            .HiddenAbility(AbilityCatalog.LightningRod)
            .Moves(m => m
                .StartsWith(Moves.MoveCatalog.ThunderShock, Moves.MoveCatalog.QuickAttack)
                .ByTM(Moves.MoveCatalog.Thunderbolt, Moves.MoveCatalog.Thunder))
            .Build();

        public static readonly PokemonSpeciesData Pikachu = Builders.Pokemon.Define("Pikachu", 25)
            .Type(PokemonType.Electric)
            .Stats(35, 55, 40, 50, 50, 90)
            .GenderRatio(50f)
            .Ability(AbilityCatalog.Static)
            .HiddenAbility(AbilityCatalog.LightningRod)
            .BaseExp(112)
            .CatchRate(190)
            .BaseFriendship(70)
            .GrowthRate(GrowthRate.MediumFast)
            .Description("When it releases pent-up energy in a burst, the power is equal to a lightning bolt.")
            .Category("Mouse Pokemon")
            .Height(0.4f)
            .Weight(6.0f)
            .Color(PokemonColor.Yellow)
            .Shape(PokemonShape.Quadruped)
            .Habitat(PokemonHabitat.Forest)
            .Moves(m => m
                .StartsWith(Moves.MoveCatalog.ThunderShock, Moves.MoveCatalog.Growl)
                .AtLevel(11, Moves.MoveCatalog.QuickAttack)
                .AtLevel(26, Moves.MoveCatalog.Thunderbolt)
                .ByTM(Moves.MoveCatalog.Thunder))
            .EvolvesTo(Raichu, e => e.WithItem("Thunder Stone"))
            .Build();

        // ===== NORMAL POKEMON =====

        public static readonly PokemonSpeciesData Eevee = Builders.Pokemon.Define("Eevee", 133)
            .Type(PokemonType.Normal)
            .Stats(55, 55, 50, 45, 65, 55)
            .GenderRatio(87.5f)
            .Abilities(AbilityCatalog.RunAway, AbilityCatalog.Adaptability)
            .HiddenAbility(AbilityCatalog.Anticipation)
            .Moves(m => m
                .StartsWith(Moves.MoveCatalog.Tackle, Moves.MoveCatalog.Growl)
                .AtLevel(15, Moves.MoveCatalog.QuickAttack))
            .Build();
        // Note: Eevee evolutions would reference Vaporeon, Jolteon, etc.
        // which aren't defined yet. We'll add them when we have those Pokemon.

        public static readonly PokemonSpeciesData Snorlax = Builders.Pokemon.Define("Snorlax", 143)
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

        public static readonly PokemonSpeciesData Gengar = Builders.Pokemon.Define("Gengar", 94)
            .Types(PokemonType.Ghost, PokemonType.Poison)
            .Stats(60, 65, 60, 130, 75, 110)
            .GenderRatio(50f)
            .Ability(AbilityCatalog.Levitate)
            .Moves(m => m
                .StartsWith(Moves.MoveCatalog.Lick, Moves.MoveCatalog.Hypnosis)
                .AtLevel(25, Moves.MoveCatalog.ShadowBall)
                .AtLevel(50, Moves.MoveCatalog.SludgeBomb))
            .Build();

        public static readonly PokemonSpeciesData Haunter = Builders.Pokemon.Define("Haunter", 93)
            .Types(PokemonType.Ghost, PokemonType.Poison)
            .Stats(45, 50, 45, 115, 55, 95)
            .GenderRatio(50f)
            .Ability(AbilityCatalog.Levitate)
            .Moves(m => m
                .StartsWith(Moves.MoveCatalog.Lick, Moves.MoveCatalog.Hypnosis)
                .AtLevel(25, Moves.MoveCatalog.ShadowBall))
            .EvolvesTo(Gengar, e => e.ByTrade())
            .Build();

        public static readonly PokemonSpeciesData Gastly = Builders.Pokemon.Define("Gastly", 92)
            .Types(PokemonType.Ghost, PokemonType.Poison)
            .Stats(30, 35, 30, 100, 35, 80)
            .GenderRatio(50f)
            .Ability(AbilityCatalog.Levitate)
            .Moves(m => m
                .StartsWith(Moves.MoveCatalog.Lick, Moves.MoveCatalog.Hypnosis)
                .AtLevel(21, Moves.MoveCatalog.ShadowBall))
            .EvolvesTo(Haunter, e => e.AtLevel(25))
            .Build();

        // ===== ROCK/GROUND LINE (Golem → Graveler → Geodude) =====

        public static readonly PokemonSpeciesData Golem = Builders.Pokemon.Define("Golem", 76)
            .Types(PokemonType.Rock, PokemonType.Ground)
            .Stats(80, 120, 130, 55, 65, 45)
            .GenderRatio(50f)
            .Abilities(AbilityCatalog.Sturdy, AbilityCatalog.ClearBody)
            .HiddenAbility(AbilityCatalog.SandStream)
            .Moves(m => m
                .StartsWith(Moves.MoveCatalog.Tackle, Moves.MoveCatalog.DefenseCurl)
                .AtLevel(11, Moves.MoveCatalog.RockThrow)
                .AtLevel(29, Moves.MoveCatalog.Earthquake)
                .AtLevel(50, Moves.MoveCatalog.RockSlide))
            .Build();

        public static readonly PokemonSpeciesData Graveler = Builders.Pokemon.Define("Graveler", 75)
            .Types(PokemonType.Rock, PokemonType.Ground)
            .Stats(55, 95, 115, 45, 45, 35)
            .GenderRatio(50f)
            .Abilities(AbilityCatalog.Sturdy, AbilityCatalog.ClearBody)
            .HiddenAbility(AbilityCatalog.SandStream)
            .Moves(m => m
                .StartsWith(Moves.MoveCatalog.Tackle, Moves.MoveCatalog.DefenseCurl)
                .AtLevel(11, Moves.MoveCatalog.RockThrow)
                .AtLevel(29, Moves.MoveCatalog.Earthquake))
            .EvolvesTo(Golem, e => e.ByTrade())
            .Build();

        public static readonly PokemonSpeciesData Geodude = Builders.Pokemon.Define("Geodude", 74)
            .Types(PokemonType.Rock, PokemonType.Ground)
            .Stats(40, 80, 100, 30, 30, 20)
            .GenderRatio(50f)
            .Abilities(AbilityCatalog.Sturdy, AbilityCatalog.ClearBody)
            .HiddenAbility(AbilityCatalog.SandStream)
            .Moves(m => m
                .StartsWith(Moves.MoveCatalog.Tackle, Moves.MoveCatalog.DefenseCurl)
                .AtLevel(11, Moves.MoveCatalog.RockThrow)
                .AtLevel(26, Moves.MoveCatalog.Earthquake))
            .EvolvesTo(Graveler, e => e.AtLevel(25))
            .Build();

        // ===== WATER/FLYING LINE (Gyarados → Magikarp) =====

        public static readonly PokemonSpeciesData Gyarados = Builders.Pokemon.Define("Gyarados", 130)
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

        public static readonly PokemonSpeciesData Magikarp = Builders.Pokemon.Define("Magikarp", 129)
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

        public static readonly PokemonSpeciesData Alakazam = Builders.Pokemon.Define("Alakazam", 65)
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

        public static readonly PokemonSpeciesData Kadabra = Builders.Pokemon.Define("Kadabra", 64)
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

        public static readonly PokemonSpeciesData Abra = Builders.Pokemon.Define("Abra", 63)
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

        public static readonly PokemonSpeciesData Mewtwo = Builders.Pokemon.Define("Mewtwo", 150)
            .Type(PokemonType.Psychic)
            .Stats(106, 110, 90, 154, 90, 130)
            .Genderless()
            .Ability(AbilityCatalog.Pressure)
            .HiddenAbility(AbilityCatalog.Unnerve)
            .Moves(m => m
                .StartsWith(Moves.MoveCatalog.Psychic)
                .ByTM(Moves.MoveCatalog.Thunderbolt, Moves.MoveCatalog.FireBlast, Moves.MoveCatalog.HydroPump))
            .Build();

        public static readonly PokemonSpeciesData Mew = Builders.Pokemon.Define("Mew", 151)
            .Type(PokemonType.Psychic)
            .Stats(100, 100, 100, 100, 100, 100)
            .Genderless()
            .Ability(AbilityCatalog.Synchronize)
            .Moves(m => m
                .StartsWith(Moves.MoveCatalog.Psychic)
                .ByTM(Moves.MoveCatalog.Thunderbolt, Moves.MoveCatalog.Flamethrower, Moves.MoveCatalog.Surf))
            .Build();

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
