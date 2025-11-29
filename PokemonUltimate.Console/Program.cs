using System;
using System.Linq;
using PokemonUltimate.Core.Builders;
using PokemonUltimate.Core.Catalogs;
using PokemonUltimate.Core.Effects;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Evolution.Conditions;
using PokemonUltimate.Core.Models;
using PokemonUltimate.Core.Registry;

namespace PokemonUltimate.Console;

/// <summary>
/// Runtime smoke test for all data systems.
/// This validates that everything works correctly outside of unit tests.
/// </summary>
class Program
{
    static int _passedTests = 0;
    static int _failedTests = 0;

    static void Main(string[] args)
    {
        PrintHeader();

        // ═══════════════════════════════════════════════════════
        // SECTION 1: CATALOGS
        // ═══════════════════════════════════════════════════════
        PrintSection("CATALOGS");

        Test("PokemonCatalog.Count > 0", () => PokemonCatalog.Count > 0);
        Test("MoveCatalog.Count > 0", () => MoveCatalog.Count > 0);
        Test("PokemonCatalog.All enumerable", () => PokemonCatalog.All.Any());
        Test("MoveCatalog.All enumerable", () => MoveCatalog.All.Any());
        Test("Direct access: PokemonCatalog.Pikachu", () => PokemonCatalog.Pikachu != null);
        Test("Direct access: MoveCatalog.Thunderbolt", () => MoveCatalog.Thunderbolt != null);

        PrintInfo($"Pokemon in catalog: {PokemonCatalog.Count}");
        PrintInfo($"Moves in catalog: {MoveCatalog.Count}");

        // ═══════════════════════════════════════════════════════
        // SECTION 2: REGISTRIES
        // ═══════════════════════════════════════════════════════
        PrintSection("REGISTRIES");

        var pokemonRegistry = new PokemonRegistry();
        var moveRegistry = new MoveRegistry();

        PokemonCatalog.RegisterAll(pokemonRegistry);
        MoveCatalog.RegisterAll(moveRegistry);

        Test("RegisterAll populates PokemonRegistry", () => pokemonRegistry.GetAll().Count() == PokemonCatalog.Count);
        Test("RegisterAll populates MoveRegistry", () => moveRegistry.GetAll().Count() == MoveCatalog.Count);
        Test("GetByName finds Pikachu", () => pokemonRegistry.GetByName("Pikachu") != null);
        Test("GetByPokedexNumber finds #25", () => pokemonRegistry.GetByPokedexNumber(25)?.Name == "Pikachu");
        Test("GetByType finds Fire moves", () => moveRegistry.GetByType(PokemonType.Fire).Any());
        Test("GetByCategory finds Status moves", () => moveRegistry.GetByCategory(MoveCategory.Status).Any());
        Test("Same instance from catalog and registry", () => 
            ReferenceEquals(PokemonCatalog.Pikachu, pokemonRegistry.GetByName("Pikachu")));

        // ═══════════════════════════════════════════════════════
        // SECTION 3: POKEMON DATA
        // ═══════════════════════════════════════════════════════
        PrintSection("POKEMON DATA");

        var charizard = PokemonCatalog.Charizard;
        Test("Charizard has name", () => charizard.Name == "Charizard");
        Test("Charizard has Pokedex #6", () => charizard.PokedexNumber == 6);
        Test("Charizard is Fire/Flying", () => 
            charizard.PrimaryType == PokemonType.Fire && charizard.SecondaryType == PokemonType.Flying);
        Test("Charizard.IsDualType", () => charizard.IsDualType);
        Test("Charizard.HasType(Fire)", () => charizard.HasType(PokemonType.Fire));
        Test("Charizard.HasType(Flying)", () => charizard.HasType(PokemonType.Flying));
        Test("Charizard has BaseStats", () => charizard.BaseStats != null);
        Test("Charizard BaseStats.Total = 534", () => charizard.BaseStats.Total == 534);

        PrintInfo($"Charizard stats: HP={charizard.BaseStats.HP}, Atk={charizard.BaseStats.Attack}, " +
                  $"Def={charizard.BaseStats.Defense}, SpA={charizard.BaseStats.SpAttack}, " +
                  $"SpD={charizard.BaseStats.SpDefense}, Spe={charizard.BaseStats.Speed}");

        // ═══════════════════════════════════════════════════════
        // SECTION 4: LEARNSETS
        // ═══════════════════════════════════════════════════════
        PrintSection("LEARNSETS");

        var charmander = PokemonCatalog.Charmander;
        Test("Charmander has Learnset", () => charmander.Learnset != null && charmander.Learnset.Count > 0);
        Test("Charmander.GetStartingMoves() returns moves", () => charmander.GetStartingMoves().Any());
        Test("Charmander can learn Ember", () => charmander.CanLearn(MoveCatalog.Ember));
        Test("Charmander learns Ember at level 9", () => 
            charmander.GetMovesAtLevel(9).Any(m => m.Move.Name == "Ember"));

        PrintInfo($"Charmander starting moves: {string.Join(", ", charmander.GetStartingMoves().Select(m => m.Move.Name))}");
        PrintInfo($"Charmander total learnable: {charmander.Learnset.Count} moves");

        // ═══════════════════════════════════════════════════════
        // SECTION 5: EVOLUTIONS
        // ═══════════════════════════════════════════════════════
        PrintSection("EVOLUTIONS");

        Test("Charmander.CanEvolve", () => charmander.CanEvolve);
        Test("Charmander evolves to Charmeleon", () => 
            charmander.Evolutions.Any(e => e.Target.Name == "Charmeleon"));
        Test("Charizard cannot evolve", () => !charizard.CanEvolve);

        var evolution = charmander.Evolutions.First();
        Test("Evolution has LevelCondition", () => evolution.HasCondition<LevelCondition>());
        Test("Evolution level is 16", () => evolution.GetCondition<LevelCondition>()?.MinLevel == 16);

        PrintInfo($"Charmander → {evolution.Target.Name} ({evolution.Description})");

        // Pikachu stone evolution
        var pikachu = PokemonCatalog.Pikachu;
        Test("Pikachu evolves with Thunder Stone", () =>
            pikachu.Evolutions.Any(e => e.HasCondition<ItemCondition>()));
        
        var pikachuEvo = pikachu.Evolutions.First();
        PrintInfo($"Pikachu → {pikachuEvo.Target.Name} ({pikachuEvo.Description})");

        // ═══════════════════════════════════════════════════════
        // SECTION 6: GENDER SYSTEM
        // ═══════════════════════════════════════════════════════
        PrintSection("GENDER SYSTEM");

        Test("Pikachu has gender ratio", () => pikachu.GenderRatio >= 0);
        Test("Pikachu.HasBothGenders", () => pikachu.HasBothGenders);
        Test("Mewtwo.IsGenderless", () => PokemonCatalog.Mewtwo.IsGenderless);

        PrintInfo($"Pikachu gender ratio: {pikachu.GenderRatio * 100}% female");
        PrintInfo($"Mewtwo is genderless: {PokemonCatalog.Mewtwo.IsGenderless}");

        // ═══════════════════════════════════════════════════════
        // SECTION 7: NATURE SYSTEM
        // ═══════════════════════════════════════════════════════
        PrintSection("NATURE SYSTEM");

        Test("NatureData.GetStatMultiplier works", () => 
            NatureData.GetStatMultiplier(Nature.Adamant, Stat.Attack) == 1.1f);
        Test("Adamant increases Attack", () => 
            NatureData.GetIncreasedStat(Nature.Adamant) == Stat.Attack);
        Test("Adamant decreases SpAttack", () => 
            NatureData.GetDecreasedStat(Nature.Adamant) == Stat.SpAttack);
        Test("Hardy is neutral", () => NatureData.IsNeutral(Nature.Hardy));
        Test("Adamant is not neutral", () => !NatureData.IsNeutral(Nature.Adamant));

        PrintInfo("Adamant: +Attack, -SpAttack");
        PrintInfo("Modest: +SpAttack, -Attack");
        PrintInfo("Jolly: +Speed, -SpAttack");

        // ═══════════════════════════════════════════════════════
        // SECTION 8: MOVE DATA & EFFECTS
        // ═══════════════════════════════════════════════════════
        PrintSection("MOVE DATA & EFFECTS");

        var thunderbolt = MoveCatalog.Thunderbolt;
        Test("Thunderbolt has name", () => thunderbolt.Name == "Thunderbolt");
        Test("Thunderbolt is Electric", () => thunderbolt.Type == PokemonType.Electric);
        Test("Thunderbolt is Special", () => thunderbolt.Category == MoveCategory.Special);
        Test("Thunderbolt power is 90", () => thunderbolt.Power == 90);
        Test("Thunderbolt has effects", () => thunderbolt.Effects.Count > 0);
        Test("Thunderbolt has DamageEffect", () => thunderbolt.HasEffect<DamageEffect>());
        Test("Thunderbolt has StatusEffect", () => thunderbolt.HasEffect<StatusEffect>());

        var statusEffect = thunderbolt.GetEffect<StatusEffect>();
        Test("Thunderbolt may paralyze", () => statusEffect?.Status == PersistentStatus.Paralysis);
        Test("Thunderbolt 10% paralyze chance", () => statusEffect?.ChancePercent == 10);

        PrintInfo($"Thunderbolt: {thunderbolt.Power} power, {thunderbolt.Accuracy}% acc, {thunderbolt.MaxPP} PP");
        PrintInfo($"Effects: {string.Join(", ", thunderbolt.Effects.Select(e => e.EffectType))}");

        // ═══════════════════════════════════════════════════════
        // SECTION 9: MOVE BUILDER
        // ═══════════════════════════════════════════════════════
        PrintSection("MOVE BUILDER");

        var customMove = Move.Define("Dragon Claw")
            .Description("Sharp claws slash the foe.")
            .Type(PokemonType.Dragon)
            .Physical(80, 100, 15)
            .WithEffects(e => e.Damage())
            .Build();

        Test("Builder creates move with name", () => customMove.Name == "Dragon Claw");
        Test("Builder sets type", () => customMove.Type == PokemonType.Dragon);
        Test("Builder sets category", () => customMove.Category == MoveCategory.Physical);
        Test("Builder sets power", () => customMove.Power == 80);
        Test("Builder adds effects", () => customMove.HasEffect<DamageEffect>());

        // Complex move with multiple effects
        var complexMove = Move.Define("Fire Fang")
            .Type(PokemonType.Fire)
            .Physical(65, 95, 15)
            .WithEffects(e => e
                .Damage()
                .MayBurn(10)
                .MayFlinch(10))
            .Build();

        Test("Complex move has 3 effects", () => complexMove.Effects.Count == 3);
        Test("Complex move has burn chance", () => complexMove.GetEffect<StatusEffect>()?.Status == PersistentStatus.Burn);
        Test("Complex move has flinch chance", () => complexMove.HasEffect<FlinchEffect>());

        PrintInfo($"Created: {customMove.Name} ({customMove.Type}, {customMove.Power} power)");
        PrintInfo($"Created: {complexMove.Name} with {complexMove.Effects.Count} effects");

        // ═══════════════════════════════════════════════════════
        // SECTION 10: POKEMON BUILDER
        // ═══════════════════════════════════════════════════════
        PrintSection("POKEMON BUILDER");

        var customPokemon = Pokemon.Define("TestMon", 999)
            .Types(PokemonType.Fire, PokemonType.Dragon)
            .Stats(80, 120, 70, 100, 80, 110)
            .GenderRatio(0.5f)
            .Moves(m => m
                .StartsWith(MoveCatalog.Scratch, MoveCatalog.Ember)
                .AtLevel(20, MoveCatalog.Flamethrower))
            .Build();

        Test("Builder creates Pokemon with name", () => customPokemon.Name == "TestMon");
        Test("Builder sets Pokedex number", () => customPokemon.PokedexNumber == 999);
        Test("Builder sets dual type", () => customPokemon.IsDualType);
        Test("Builder sets stats total", () => customPokemon.BaseStats.Total == 560);
        Test("Builder sets gender ratio", () => customPokemon.GenderRatio == 0.5f);
        Test("Builder creates learnset", () => customPokemon.Learnset.Count == 3);

        PrintInfo($"Created: {customPokemon.Name} (#{customPokemon.PokedexNumber})");
        PrintInfo($"Types: {customPokemon.PrimaryType}/{customPokemon.SecondaryType}");
        PrintInfo($"BST: {customPokemon.BaseStats.Total}");

        // ═══════════════════════════════════════════════════════
        // SECTION 11: EFFECT TYPES
        // ═══════════════════════════════════════════════════════
        PrintSection("EFFECT TYPES");

        Test("DamageEffect has correct type", () => new DamageEffect().EffectType == EffectType.Damage);
        Test("StatusEffect has correct type", () => new StatusEffect().EffectType == EffectType.Status);
        Test("StatChangeEffect has correct type", () => new StatChangeEffect().EffectType == EffectType.StatChange);
        Test("RecoilEffect has correct type", () => new RecoilEffect(25).EffectType == EffectType.Recoil);
        Test("DrainEffect has correct type", () => new DrainEffect(50).EffectType == EffectType.Drain);
        Test("HealEffect has correct type", () => new HealEffect(50).EffectType == EffectType.Heal);
        Test("FlinchEffect has correct type", () => new FlinchEffect(30).EffectType == EffectType.Flinch);
        Test("MultiHitEffect has correct type", () => new MultiHitEffect(2, 5).EffectType == EffectType.MultiHit);
        Test("FixedDamageEffect has correct type", () => new FixedDamageEffect(40).EffectType == EffectType.FixedDamage);

        // ═══════════════════════════════════════════════════════
        // SECTION 12: TARGET SCOPES
        // ═══════════════════════════════════════════════════════
        PrintSection("TARGET SCOPES");

        var earthquake = MoveCatalog.Earthquake;
        var growl = MoveCatalog.Growl;
        var surf = MoveCatalog.Surf;

        Test("Earthquake targets AllOthers", () => earthquake.TargetScope == TargetScope.AllOthers);
        Test("Growl targets AllEnemies", () => growl.TargetScope == TargetScope.AllEnemies);
        Test("Surf targets AllEnemies", () => surf.TargetScope == TargetScope.AllEnemies);
        Test("Thunderbolt targets SingleEnemy", () => thunderbolt.TargetScope == TargetScope.SingleEnemy);

        PrintInfo($"Available TargetScopes: {Enum.GetNames(typeof(TargetScope)).Length}");

        // ═══════════════════════════════════════════════════════
        // SECTION 13: COMPLETE POKEMON LISTING
        // ═══════════════════════════════════════════════════════
        PrintSection("ALL POKEMON IN CATALOG");

        foreach (var pokemon in PokemonCatalog.All.OrderBy(p => p.PokedexNumber))
        {
            var types = pokemon.IsDualType 
                ? $"{pokemon.PrimaryType}/{pokemon.SecondaryType}" 
                : pokemon.PrimaryType.ToString();
            var evoInfo = pokemon.CanEvolve 
                ? $" → {pokemon.Evolutions.First().Target.Name}" 
                : "";
            PrintInfo($"#{pokemon.PokedexNumber:D3} {pokemon.Name,-12} [{types,-15}] BST:{pokemon.BaseStats.Total}{evoInfo}");
        }

        // ═══════════════════════════════════════════════════════
        // SECTION 14: COMPLETE MOVE LISTING
        // ═══════════════════════════════════════════════════════
        PrintSection("ALL MOVES IN CATALOG");

        foreach (var move in MoveCatalog.All.OrderBy(m => m.Type).ThenBy(m => m.Name))
        {
            var effectList = string.Join("+", move.Effects.Select(e => e.EffectType.ToString().Substring(0, 3)));
            PrintInfo($"{move.Name,-15} [{move.Type,-8}] {move.Category,-8} P:{move.Power,3} A:{move.Accuracy,3} [{effectList}]");
        }

        // ═══════════════════════════════════════════════════════
        // FINAL RESULTS
        // ═══════════════════════════════════════════════════════
        PrintResults();
    }

    #region Test Helpers

    static void Test(string name, Func<bool> test)
    {
        try
        {
            if (test())
            {
                _passedTests++;
                System.Console.ForegroundColor = ConsoleColor.Green;
                System.Console.Write("  ✓ ");
                System.Console.ResetColor();
                System.Console.WriteLine(name);
            }
            else
            {
                _failedTests++;
                System.Console.ForegroundColor = ConsoleColor.Red;
                System.Console.Write("  ✗ ");
                System.Console.ResetColor();
                System.Console.WriteLine($"{name} [FAILED]");
            }
        }
        catch (Exception ex)
        {
            _failedTests++;
            System.Console.ForegroundColor = ConsoleColor.Red;
            System.Console.Write("  ✗ ");
            System.Console.ResetColor();
            System.Console.WriteLine($"{name} [EXCEPTION: {ex.Message}]");
        }
    }

    #endregion

    #region Console Helpers

    static void PrintHeader()
    {
        System.Console.Clear();
        System.Console.ForegroundColor = ConsoleColor.Cyan;
        System.Console.WriteLine();
        System.Console.WriteLine("╔═══════════════════════════════════════════════════════════════╗");
        System.Console.WriteLine("║     POKEMON ULTIMATE - Runtime System Verification            ║");
        System.Console.WriteLine("║                   Data Layer Smoke Test                       ║");
        System.Console.WriteLine("╚═══════════════════════════════════════════════════════════════╝");
        System.Console.ResetColor();
    }

    static void PrintSection(string title)
    {
        System.Console.WriteLine();
        System.Console.ForegroundColor = ConsoleColor.Yellow;
        System.Console.WriteLine($"═══ {title} ═══");
        System.Console.ResetColor();
    }

    static void PrintInfo(string message)
    {
        System.Console.ForegroundColor = ConsoleColor.DarkGray;
        System.Console.Write("    → ");
        System.Console.ResetColor();
        System.Console.WriteLine(message);
    }

    static void PrintResults()
    {
        System.Console.WriteLine();
        System.Console.WriteLine();
        
        if (_failedTests == 0)
        {
            System.Console.ForegroundColor = ConsoleColor.Green;
            System.Console.WriteLine("╔═══════════════════════════════════════════════════════════════╗");
            System.Console.WriteLine($"║  ✓ ALL {_passedTests} TESTS PASSED - Systems Ready for Combat!       ║");
            System.Console.WriteLine("╚═══════════════════════════════════════════════════════════════╝");
        }
        else
        {
            System.Console.ForegroundColor = ConsoleColor.Red;
            System.Console.WriteLine("╔═══════════════════════════════════════════════════════════════╗");
            System.Console.WriteLine($"║  ✗ {_failedTests} TESTS FAILED out of {_passedTests + _failedTests}                            ║");
            System.Console.WriteLine("╚═══════════════════════════════════════════════════════════════╝");
        }
        
        System.Console.ResetColor();
        System.Console.WriteLine();
        System.Console.WriteLine($"  Passed: {_passedTests}");
        System.Console.WriteLine($"  Failed: {_failedTests}");
        System.Console.WriteLine($"  Total:  {_passedTests + _failedTests}");
        System.Console.WriteLine();
    }

    #endregion
}
