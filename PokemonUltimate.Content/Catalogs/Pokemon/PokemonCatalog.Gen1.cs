using PokemonUltimate.Content.Catalogs.Abilities;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Content.Catalogs.Pokemon
{
    /// <summary>
    /// Generation 1 Pokemon (Kanto region, #001-151).
    /// Defined in reverse evolution order so evolutions can reference targets.
    /// </summary>
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
            .Stats(45, 49, 49, 65, 65, 45)
            .GenderRatio(87.5f)
            .Ability(AbilityCatalog.Overgrow)
            .HiddenAbility(AbilityCatalog.Chlorophyll)
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
            .Ability(AbilityCatalog.Static)
            .HiddenAbility(AbilityCatalog.LightningRod)
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
            // Legendary
            _all.Add(Mewtwo);
            _all.Add(Mew);
        }
    }
}
