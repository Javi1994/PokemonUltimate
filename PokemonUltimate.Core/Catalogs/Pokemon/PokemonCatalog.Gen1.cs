using PokemonUltimate.Core.Builders;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Models;

namespace PokemonUltimate.Core.Catalogs
{
    /// <summary>
    /// Generation 1 Pokemon (Kanto region, #001-151).
    /// Defined in reverse evolution order so evolutions can reference targets.
    /// </summary>
    public static partial class PokemonCatalog
    {
        // ===== GRASS STARTER LINE (Venusaur → Ivysaur → Bulbasaur) =====
        // Starters have 87.5% male ratio

        public static readonly PokemonSpeciesData Venusaur = Pokemon.Define("Venusaur", 3)
            .Types(PokemonType.Grass, PokemonType.Poison)
            .Stats(80, 82, 83, 100, 100, 80)
            .GenderRatio(87.5f)
            .Moves(m => m
                .StartsWith(MoveCatalog.VineWhip, MoveCatalog.Growl)
                .AtLevel(32, MoveCatalog.RazorLeaf)
                .AtLevel(65, MoveCatalog.SolarBeam))
            .Build();

        public static readonly PokemonSpeciesData Ivysaur = Pokemon.Define("Ivysaur", 2)
            .Types(PokemonType.Grass, PokemonType.Poison)
            .Stats(60, 62, 63, 80, 80, 60)
            .GenderRatio(87.5f)
            .Moves(m => m
                .StartsWith(MoveCatalog.VineWhip, MoveCatalog.Growl)
                .AtLevel(22, MoveCatalog.RazorLeaf))
            .EvolvesTo(Venusaur, e => e.AtLevel(32))
            .Build();

        public static readonly PokemonSpeciesData Bulbasaur = Pokemon.Define("Bulbasaur", 1)
            .Types(PokemonType.Grass, PokemonType.Poison)
            .Stats(45, 49, 49, 65, 65, 45)
            .GenderRatio(87.5f)
            .Moves(m => m
                .StartsWith(MoveCatalog.Tackle, MoveCatalog.Growl)
                .AtLevel(9, MoveCatalog.VineWhip)
                .AtLevel(27, MoveCatalog.RazorLeaf))
            .EvolvesTo(Ivysaur, e => e.AtLevel(16))
            .Build();

        // ===== FIRE STARTER LINE (Charizard → Charmeleon → Charmander) =====
        // Starters have 87.5% male ratio

        public static readonly PokemonSpeciesData Charizard = Pokemon.Define("Charizard", 6)
            .Types(PokemonType.Fire, PokemonType.Flying)
            .Stats(78, 84, 78, 109, 85, 100)
            .GenderRatio(87.5f)
            .Moves(m => m
                .StartsWith(MoveCatalog.Scratch, MoveCatalog.Ember)
                .AtLevel(46, MoveCatalog.Flamethrower)
                .ByTM(MoveCatalog.FireBlast, MoveCatalog.Earthquake))
            .Build();

        public static readonly PokemonSpeciesData Charmeleon = Pokemon.Define("Charmeleon", 5)
            .Type(PokemonType.Fire)
            .Stats(58, 64, 58, 80, 65, 80)
            .GenderRatio(87.5f)
            .Moves(m => m
                .StartsWith(MoveCatalog.Scratch, MoveCatalog.Growl)
                .AtLevel(17, MoveCatalog.Ember)
                .AtLevel(39, MoveCatalog.Flamethrower))
            .EvolvesTo(Charizard, e => e.AtLevel(36))
            .Build();

        public static readonly PokemonSpeciesData Charmander = Pokemon.Define("Charmander", 4)
            .Type(PokemonType.Fire)
            .Stats(39, 52, 43, 60, 50, 65)
            .GenderRatio(87.5f)
            .Moves(m => m
                .StartsWith(MoveCatalog.Scratch, MoveCatalog.Growl)
                .AtLevel(9, MoveCatalog.Ember)
                .AtLevel(38, MoveCatalog.Flamethrower))
            .EvolvesTo(Charmeleon, e => e.AtLevel(16))
            .Build();

        // ===== WATER STARTER LINE (Blastoise → Wartortle → Squirtle) =====
        // Starters have 87.5% male ratio

        public static readonly PokemonSpeciesData Blastoise = Pokemon.Define("Blastoise", 9)
            .Type(PokemonType.Water)
            .Stats(79, 83, 100, 85, 105, 78)
            .GenderRatio(87.5f)
            .Moves(m => m
                .StartsWith(MoveCatalog.Tackle, MoveCatalog.WaterGun)
                .AtLevel(42, MoveCatalog.Surf)
                .AtLevel(52, MoveCatalog.HydroPump))
            .Build();

        public static readonly PokemonSpeciesData Wartortle = Pokemon.Define("Wartortle", 8)
            .Type(PokemonType.Water)
            .Stats(59, 63, 80, 65, 80, 58)
            .GenderRatio(87.5f)
            .Moves(m => m
                .StartsWith(MoveCatalog.Tackle, MoveCatalog.WaterGun)
                .AtLevel(28, MoveCatalog.Surf))
            .EvolvesTo(Blastoise, e => e.AtLevel(36))
            .Build();

        public static readonly PokemonSpeciesData Squirtle = Pokemon.Define("Squirtle", 7)
            .Type(PokemonType.Water)
            .Stats(44, 48, 65, 50, 64, 43)
            .GenderRatio(87.5f)
            .Moves(m => m
                .StartsWith(MoveCatalog.Tackle, MoveCatalog.Growl)
                .AtLevel(8, MoveCatalog.WaterGun)
                .AtLevel(25, MoveCatalog.Surf))
            .EvolvesTo(Wartortle, e => e.AtLevel(16))
            .Build();

        // ===== ELECTRIC LINE (Raichu → Pikachu) =====

        public static readonly PokemonSpeciesData Raichu = Pokemon.Define("Raichu", 26)
            .Type(PokemonType.Electric)
            .Stats(60, 90, 55, 90, 80, 110)
            .Moves(m => m
                .StartsWith(MoveCatalog.ThunderShock, MoveCatalog.QuickAttack)
                .ByTM(MoveCatalog.Thunderbolt, MoveCatalog.Thunder))
            .Build();

        public static readonly PokemonSpeciesData Pikachu = Pokemon.Define("Pikachu", 25)
            .Type(PokemonType.Electric)
            .Stats(35, 55, 40, 50, 50, 90)
            .Moves(m => m
                .StartsWith(MoveCatalog.ThunderShock, MoveCatalog.Growl)
                .AtLevel(11, MoveCatalog.QuickAttack)
                .AtLevel(26, MoveCatalog.Thunderbolt)
                .ByTM(MoveCatalog.Thunder))
            .EvolvesTo(Raichu, e => e.WithItem("Thunder Stone"))
            .Build();

        // ===== NORMAL POKEMON =====

        public static readonly PokemonSpeciesData Eevee = Pokemon.Define("Eevee", 133)
            .Type(PokemonType.Normal)
            .Stats(55, 55, 50, 45, 65, 55)
            .GenderRatio(87.5f)
            .Moves(m => m
                .StartsWith(MoveCatalog.Tackle, MoveCatalog.Growl)
                .AtLevel(15, MoveCatalog.QuickAttack))
            .Build();
            // Note: Eevee evolutions would reference Vaporeon, Jolteon, etc.
            // which aren't defined yet. We'll add them when we have those Pokemon.

        public static readonly PokemonSpeciesData Snorlax = Pokemon.Define("Snorlax", 143)
            .Type(PokemonType.Normal)
            .Stats(160, 110, 65, 65, 110, 30)
            .GenderRatio(87.5f)
            .Moves(m => m
                .StartsWith(MoveCatalog.Tackle, MoveCatalog.Growl)
                .AtLevel(35, MoveCatalog.HyperBeam)
                .ByTM(MoveCatalog.Earthquake, MoveCatalog.Psychic))
            .Build();

        // ===== LEGENDARY/MYTHICAL =====
        // Legendaries are typically genderless

        public static readonly PokemonSpeciesData Mewtwo = Pokemon.Define("Mewtwo", 150)
            .Type(PokemonType.Psychic)
            .Stats(106, 110, 90, 154, 90, 130)
            .Genderless()
            .Moves(m => m
                .StartsWith(MoveCatalog.Psychic)
                .ByTM(MoveCatalog.Thunderbolt, MoveCatalog.FireBlast, MoveCatalog.HydroPump))
            .Build();

        public static readonly PokemonSpeciesData Mew = Pokemon.Define("Mew", 151)
            .Type(PokemonType.Psychic)
            .Stats(100, 100, 100, 100, 100, 100)
            .Genderless()
            .Moves(m => m
                .StartsWith(MoveCatalog.Psychic)
                .ByTM(MoveCatalog.Thunderbolt, MoveCatalog.Flamethrower, MoveCatalog.Surf))
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
            // Legendary
            _all.Add(Mewtwo);
            _all.Add(Mew);
        }
    }
}
