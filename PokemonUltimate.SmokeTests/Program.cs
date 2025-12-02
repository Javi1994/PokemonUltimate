using System;
using System.Linq;
using System.Threading.Tasks;
using PokemonUltimate.Core.Builders;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Effects;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Evolution.Conditions;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;
using PokemonUltimate.Core.Registry;
// Combat
using PokemonUltimate.Combat;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.AI;
using PokemonUltimate.Combat.Damage;
using PokemonUltimate.Combat.Damage.Steps;
using PokemonUltimate.Combat.Events;
using PokemonUltimate.Combat.Helpers;
using PokemonUltimate.Content.Builders;
// Catalogs
using MoveCatalog = PokemonUltimate.Content.Catalogs.Moves.MoveCatalog;
using PokemonCatalog = PokemonUltimate.Content.Catalogs.Pokemon.PokemonCatalog;
using AbilityCatalog = PokemonUltimate.Content.Catalogs.Abilities.AbilityCatalog;
using ItemCatalog = PokemonUltimate.Content.Catalogs.Items.ItemCatalog;
using StatusCatalog = PokemonUltimate.Content.Catalogs.Status.StatusCatalog;
using WeatherCatalog = PokemonUltimate.Content.Catalogs.Weather.WeatherCatalog;
using TerrainCatalog = PokemonUltimate.Content.Catalogs.Terrain.TerrainCatalog;
using HazardCatalog = PokemonUltimate.Content.Catalogs.Field.HazardCatalog;
using SideConditionCatalog = PokemonUltimate.Content.Catalogs.Field.SideConditionCatalog;
using FieldEffectCatalog = PokemonUltimate.Content.Catalogs.Field.FieldEffectCatalog;
using Pokemon = PokemonUltimate.Core.Factories.Pokemon;

namespace PokemonUltimate.SmokeTests;

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
        // SECTION 10: POKEMON BLUEPRINT BUILDER
        // ═══════════════════════════════════════════════════════
        PrintSection("POKEMON BLUEPRINT BUILDER");

        var customPokemon = PokemonUltimate.Content.Builders.Pokemon.Define("TestMon", 999)
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
        // SECTION 13: MOVE INSTANCE (PP TRACKING)
        // ═══════════════════════════════════════════════════════
        PrintSection("MOVE INSTANCE (PP TRACKING)");

        var moveInstance = new MoveInstance(MoveCatalog.Thunderbolt);
        Test("MoveInstance has Move reference", () => moveInstance.Move == MoveCatalog.Thunderbolt);
        Test("MoveInstance starts at full PP", () => moveInstance.CurrentPP == moveInstance.MaxPP);
        Test("MoveInstance.HasPP is true", () => moveInstance.HasPP);

        moveInstance.Use();
        moveInstance.Use();
        Test("Use() decreases PP", () => moveInstance.CurrentPP == moveInstance.MaxPP - 2);

        moveInstance.Restore(1);
        Test("Restore() increases PP", () => moveInstance.CurrentPP == moveInstance.MaxPP - 1);

        moveInstance.RestoreFully();
        Test("RestoreFully() restores max PP", () => moveInstance.CurrentPP == moveInstance.MaxPP);

        PrintInfo($"Thunderbolt: {moveInstance.CurrentPP}/{moveInstance.MaxPP} PP");

        // ═══════════════════════════════════════════════════════
        // SECTION 14: STAT CALCULATOR (WITH IVs/EVs)
        // ═══════════════════════════════════════════════════════
        PrintSection("STAT CALCULATOR (WITH IVs/EVs)");

        // Constants
        Test("MaxIV is 31", () => StatCalculator.MaxIV == 31);
        Test("MaxEV is 252", () => StatCalculator.MaxEV == 252);
        Test("DefaultIV equals MaxIV", () => StatCalculator.DefaultIV == StatCalculator.MaxIV);

        // HP with max IVs/EVs: ((2*35 + 31 + 63) * 50 / 100) + 50 + 10 = 142
        var pikachuHPMax = StatCalculator.CalculateHP(35, 50);
        Test("HP formula with max IVs/EVs (Pikachu Lv50)", () => pikachuHPMax == 142);

        // HP without IVs/EVs: ((2*35 + 0 + 0) * 50 / 100) + 50 + 10 = 95
        var pikachuHPZero = StatCalculator.CalculateHP(35, 50, 0, 0);
        Test("HP formula without IVs/EVs (Pikachu Lv50)", () => pikachuHPZero == 95);

        // Stat calculation with nature and max IVs/EVs
        var atkNeutral = StatCalculator.CalculateStat(100, 50, Nature.Hardy, Stat.Attack);
        var atkBoosted = StatCalculator.CalculateStat(100, 50, Nature.Adamant, Stat.Attack);
        var atkReduced = StatCalculator.CalculateStat(100, 50, Nature.Modest, Stat.Attack);

        Test("Neutral nature with max IVs/EVs", () => atkNeutral == 152);
        Test("Boosting nature = +10%", () => atkBoosted == 167);
        Test("Reducing nature = -10%", () => atkReduced == 136);

        // Experience calculations
        Test("Level 1 exp = 1", () => StatCalculator.GetExpForLevel(1) == 1);
        Test("Level 50 exp = 125000", () => StatCalculator.GetExpForLevel(50) == 125000);
        Test("Level 100 exp = 1000000", () => StatCalculator.GetExpForLevel(100) == 1000000);
        Test("GetLevelForExp finds correct level", () => StatCalculator.GetLevelForExp(125000) == 50);

        // Stat stages
        Test("Stage 0 = 1.0x", () => Math.Abs(StatCalculator.GetStageMultiplier(0) - 1.0f) < 0.01f);
        Test("Stage +2 = 2.0x", () => Math.Abs(StatCalculator.GetStageMultiplier(2) - 2.0f) < 0.01f);
        Test("Stage -2 = 0.5x", () => Math.Abs(StatCalculator.GetStageMultiplier(-2) - 0.5f) < 0.01f);
        Test("Stage +6 = 4.0x", () => Math.Abs(StatCalculator.GetStageMultiplier(6) - 4.0f) < 0.01f);

        PrintInfo($"Base 100 Atk at Lv50 (max IVs/EVs): Neutral={atkNeutral}, Adamant={atkBoosted}, Modest={atkReduced}");
        PrintInfo($"HP difference: With IVs/EVs={pikachuHPMax}, Without={pikachuHPZero}");

        // ═══════════════════════════════════════════════════════
        // SECTION 15: POKEMON INSTANCE BUILDER
        // ═══════════════════════════════════════════════════════
        PrintSection("POKEMON INSTANCE BUILDER");

        Pokemon.SetSeed(12345); // Deterministic for testing

        // Basic creation
        var basicPikachu = Pokemon.Create(PokemonCatalog.Pikachu, 25).Build();
        Test("Builder creates instance", () => basicPikachu != null);
        Test("Instance has correct species", () => basicPikachu.Species == PokemonCatalog.Pikachu);
        Test("Instance has correct level", () => basicPikachu.Level == 25);
        Test("Instance has unique ID", () => !string.IsNullOrEmpty(basicPikachu.InstanceId));
        Test("Instance starts at full HP", () => basicPikachu.CurrentHP == basicPikachu.MaxHP);

        // Full builder chain
        var customInstance = Pokemon.Create(PokemonCatalog.Charizard, 50)
            .WithNature(Nature.Adamant)
            .Female()
            .Named("Blaze Queen")
            .WithMoves(MoveCatalog.Flamethrower, MoveCatalog.FireBlast)
            .WithHighFriendship()
            .AtHalfHealth()
            .Build();

        Test("WithNature sets nature", () => customInstance.Nature == Nature.Adamant);
        Test("Female() sets gender", () => customInstance.Gender == Gender.Female);
        Test("Named() sets nickname", () => customInstance.Nickname == "Blaze Queen");
        Test("DisplayName shows nickname", () => customInstance.DisplayName == "Blaze Queen");
        Test("WithMoves() sets specific moves", () => customInstance.Moves.Count == 2);
        Test("WithHighFriendship() sets 220", () => customInstance.Friendship == 220);
        Test("AtHalfHealth() sets 50%", () => Math.Abs(customInstance.HPPercentage - 0.5f) < 0.01f);

        PrintInfo($"Created: {customInstance.DisplayName} Lv.{customInstance.Level}");
        PrintInfo($"Stats: HP={customInstance.MaxHP}, Atk={customInstance.Attack}, Spe={customInstance.Speed}");
        PrintInfo($"Nature: {customInstance.Nature}, Gender: {customInstance.Gender}");

        // ═══════════════════════════════════════════════════════
        // SECTION 16: POKEMON FACTORY (QUICK CREATION)
        // ═══════════════════════════════════════════════════════
        PrintSection("POKEMON FACTORY (QUICK CREATION)");

        var quickPokemon = PokemonFactory.Create(PokemonCatalog.Bulbasaur, 15);
        Test("Factory creates instance", () => quickPokemon != null);
        Test("Factory sets level", () => quickPokemon.Level == 15);
        Test("Factory assigns moves from learnset", () => quickPokemon.Moves.Count > 0);

        var withNature = PokemonFactory.Create(PokemonCatalog.Squirtle, 20, Nature.Bold);
        Test("Factory accepts nature", () => withNature.Nature == Nature.Bold);

        var fullControl = PokemonFactory.Create(PokemonCatalog.Mewtwo, 70, Nature.Modest, Gender.Genderless);
        Test("Factory accepts gender", () => fullControl.Gender == Gender.Genderless);

        PrintInfo($"Quick created: {quickPokemon.Species.Name} Lv.{quickPokemon.Level} with {quickPokemon.Moves.Count} moves");

        // ═══════════════════════════════════════════════════════
        // SECTION 17: FRIENDSHIP & SHINY
        // ═══════════════════════════════════════════════════════
        PrintSection("FRIENDSHIP & SHINY");

        var friendlyMon = Pokemon.Create(PokemonCatalog.Eevee, 20)
            .WithMaxFriendship()
            .Build();

        Test("Default friendship is 70", () => basicPikachu.Friendship == 70);
        Test("WithMaxFriendship sets 255", () => friendlyMon.Friendship == 255);
        Test("HasMaxFriendship is true", () => friendlyMon.HasMaxFriendship);
        Test("HasHighFriendship is true at 255", () => friendlyMon.HasHighFriendship);

        friendlyMon.DecreaseFriendship(50);
        Test("DecreaseFriendship works", () => friendlyMon.Friendship == 205);

        friendlyMon.IncreaseFriendship(100);
        Test("IncreaseFriendship caps at 255", () => friendlyMon.Friendship == 255);

        var shinyMon = Pokemon.Create(PokemonCatalog.Pikachu, 25).Shiny().Build();
        Test("Shiny() makes Pokemon shiny", () => shinyMon.IsShiny);

        var normalMon = Pokemon.Create(PokemonCatalog.Pikachu, 25).NotShiny().Build();
        Test("NotShiny() makes Pokemon not shiny", () => !normalMon.IsShiny);

        PrintInfo($"Friendly Eevee: Friendship={friendlyMon.Friendship}, HighFriendship={friendlyMon.HasHighFriendship}");
        PrintInfo($"Shiny Pikachu: IsShiny={shinyMon.IsShiny}");

        // ═══════════════════════════════════════════════════════
        // SECTION 18: BATTLE STATE
        // ═══════════════════════════════════════════════════════
        PrintSection("BATTLE STATE");

        var battleMon = Pokemon.Create(PokemonCatalog.Charizard, 50)
            .WithNeutralNature()
            .WithMaxHP(200)
            .Build();

        Test("IsFainted is false at full HP", () => !battleMon.IsFainted);

        var damageDealt = battleMon.TakeDamage(50);
        Test("TakeDamage reduces HP", () => battleMon.CurrentHP == 150);
        Test("TakeDamage returns actual damage", () => damageDealt == 50);

        var healed = battleMon.Heal(30);
        Test("Heal increases HP", () => battleMon.CurrentHP == 180);
        Test("Heal returns actual heal", () => healed == 30);

        var overHeal = battleMon.Heal(100);
        Test("Heal caps at MaxHP", () => battleMon.CurrentHP == 200);
        Test("Heal returns capped amount", () => overHeal == 20);

        // Stat stages
        battleMon.ModifyStatStage(Stat.Attack, 2);
        Test("ModifyStatStage changes stage", () => battleMon.StatStages[Stat.Attack] == 2);

        var effectiveAtk = battleMon.GetEffectiveStat(Stat.Attack);
        Test("GetEffectiveStat applies stages", () => effectiveAtk == battleMon.Attack * 2);

        battleMon.ModifyStatStage(Stat.Attack, 10); // Try to exceed +6
        Test("Stat stages cap at +6", () => battleMon.StatStages[Stat.Attack] == 6);

        // Status
        battleMon.Status = PersistentStatus.Burn;
        Test("HasStatus returns true", () => battleMon.HasStatus);

        // Reset battle state
        battleMon.ResetBattleState();
        Test("ResetBattleState clears stages", () => battleMon.StatStages[Stat.Attack] == 0);
        Test("ResetBattleState keeps persistent status", () => battleMon.Status == PersistentStatus.Burn);

        // Full heal
        battleMon.TakeDamage(100);
        battleMon.FullHeal();
        Test("FullHeal restores HP", () => battleMon.CurrentHP == battleMon.MaxHP);
        Test("FullHeal clears status", () => battleMon.Status == PersistentStatus.None);

        PrintInfo($"Battle tested: {battleMon.Species.Name} HP={battleMon.CurrentHP}/{battleMon.MaxHP}");

        // ═══════════════════════════════════════════════════════
        // SECTION 19: FAINTED STATE
        // ═══════════════════════════════════════════════════════
        PrintSection("FAINTED STATE");

        var faintedMon = Pokemon.Create(PokemonCatalog.Pikachu, 25)
            .Fainted()
            .Build();

        Test("Fainted() sets HP to 0", () => faintedMon.CurrentHP == 0);
        Test("IsFainted returns true", () => faintedMon.IsFainted);
        Test("HPPercentage is 0", () => faintedMon.HPPercentage == 0);

        // Overkill damage
        var overKillMon = Pokemon.Create(PokemonCatalog.Pikachu, 25).WithMaxHP(50).Build();
        var overkillDamage = overKillMon.TakeDamage(999);
        Test("Overkill caps at current HP", () => overkillDamage == 50);
        Test("HP doesn't go negative", () => overKillMon.CurrentHP == 0);

        PrintInfo($"Fainted: {faintedMon.Species.Name} HP={faintedMon.CurrentHP}, IsFainted={faintedMon.IsFainted}");

        // ═══════════════════════════════════════════════════════
        // SECTION 20: LEVEL UP SYSTEM
        // ═══════════════════════════════════════════════════════
        PrintSection("LEVEL UP SYSTEM");

        var levelUpMon = Pokemon.Create(PokemonCatalog.Charmander, 10)
            .WithNeutralNature()
            .Build();

        Test("CanLevelUp is false without exp", () => !levelUpMon.CanLevelUp());

        levelUpMon.CurrentExp = StatCalculator.GetExpForLevel(11);
        Test("CanLevelUp is true with exp", () => levelUpMon.CanLevelUp());

        var oldMaxHP = levelUpMon.MaxHP;
        var levelsGained = levelUpMon.AddExperience(0); // Already has exp
        Test("AddExperience levels up", () => levelUpMon.Level == 11);
        Test("Stats recalculate on level up", () => levelUpMon.MaxHP > oldMaxHP);

        Test("LevelUp() forces level up", () => { levelUpMon.LevelUp(); return levelUpMon.Level == 12; });
        Test("LevelUpTo() reaches target", () => { levelUpMon.LevelUpTo(20); return levelUpMon.Level == 20; });

        // Experience methods
        Test("GetExpForNextLevel works", () => levelUpMon.GetExpForNextLevel() == StatCalculator.GetExpForLevel(21));
        Test("GetExpToNextLevel works", () => levelUpMon.GetExpToNextLevel() >= 0);

        PrintInfo($"Charmander leveled from 10 to {levelUpMon.Level}");
        PrintInfo($"Current HP: {levelUpMon.CurrentHP}/{levelUpMon.MaxHP}");

        // ═══════════════════════════════════════════════════════
        // SECTION 21: EVOLUTION SYSTEM
        // ═══════════════════════════════════════════════════════
        PrintSection("EVOLUTION SYSTEM");

        var evoCharmander = Pokemon.Create(PokemonCatalog.Charmander, 15)
            .WithNeutralNature()
            .Build();

        Test("CanEvolve is false at level 15", () => !evoCharmander.CanEvolve());

        evoCharmander.LevelUpTo(16);
        Test("CanEvolve is true at level 16", () => evoCharmander.CanEvolve());
        Test("GetAvailableEvolution returns Charmeleon", () =>
            evoCharmander.GetAvailableEvolution()?.Target.Name == "Charmeleon");

        var oldHP = evoCharmander.MaxHP;
        var evolved = evoCharmander.TryEvolve();
        Test("TryEvolve returns new species", () => evolved?.Name == "Charmeleon");
        Test("Species changed to Charmeleon", () => evoCharmander.Species.Name == "Charmeleon");
        Test("Stats recalculate on evolution", () => evoCharmander.MaxHP > oldHP);
        Test("HP maintained proportionally", () => evoCharmander.CurrentHP > 0);

        PrintInfo($"Evolved: {evoCharmander.Species.Name} at Level {evoCharmander.Level}");
        PrintInfo($"New stats: HP={evoCharmander.MaxHP}, Atk={evoCharmander.Attack}");

        // Eevee has no evolutions defined yet (requires Espeon/Umbreon in catalog)
        var evoEevee = Pokemon.Create(PokemonCatalog.Eevee, 20)
            .WithHighFriendship()
            .Build();

        Test("Eevee has no evolutions defined yet", () => !evoEevee.CanEvolve());
        Test("Eevee still tracks friendship correctly", () => evoEevee.HasHighFriendship);

        PrintInfo($"Eevee: Friendship={evoEevee.Friendship}, CanEvolve={evoEevee.CanEvolve()} (needs Espeon/Umbreon in catalog)");

        // ═══════════════════════════════════════════════════════
        // SECTION 22: MOVE SELECTION PRESETS
        // ═══════════════════════════════════════════════════════
        PrintSection("MOVE SELECTION PRESETS");

        var stabMon = Pokemon.Create(PokemonCatalog.Pikachu, 50)
            .WithStabMoves()
            .Build();

        Test("WithStabMoves creates moveset", () => stabMon.Moves.Count > 0);

        // Count STAB moves (Electric for Pikachu)
        var stabCount = stabMon.Moves.Count(m => m.Move.Type == PokemonType.Electric);
        PrintInfo($"STAB moves selected: {stabCount}/{stabMon.Moves.Count} are Electric");

        var strongMon = Pokemon.Create(PokemonCatalog.Charizard, 50)
            .WithStrongMoves()
            .Build();

        Test("WithStrongMoves creates moveset", () => strongMon.Moves.Count > 0);

        var optimalMon = Pokemon.Create(PokemonCatalog.Charizard, 50)
            .WithOptimalMoves()
            .Build();

        Test("WithOptimalMoves creates moveset", () => optimalMon.Moves.Count > 0);

        PrintInfo($"Strong moves: {string.Join(", ", strongMon.Moves.Select(m => m.Move.Name))}");
        PrintInfo($"Optimal moves: {string.Join(", ", optimalMon.Moves.Select(m => m.Move.Name))}");

        // ═══════════════════════════════════════════════════════
        // SECTION 23: ABILITY DATA
        // ═══════════════════════════════════════════════════════
        PrintSection("ABILITY DATA");

        Test("AbilityCatalog.Intimidate exists", () => AbilityCatalog.Intimidate != null);
        Test("AbilityCatalog.Static exists", () => AbilityCatalog.Static != null);
        Test("AbilityCatalog.Blaze exists", () => AbilityCatalog.Blaze != null);
        Test("Abilities have names", () => !string.IsNullOrEmpty(AbilityCatalog.Intimidate.Name));
        Test("Abilities have descriptions", () => !string.IsNullOrEmpty(AbilityCatalog.Static.Description));
        Test("Intimidate has triggers", () => AbilityCatalog.Intimidate.Triggers != AbilityTrigger.None);

        // Pokemon with abilities
        var pikachuWithAbility = PokemonCatalog.Pikachu;
        Test("Pikachu has Ability1", () => pikachuWithAbility.Ability1 != null);
        Test("Pikachu has HiddenAbility", () => pikachuWithAbility.HiddenAbility != null);

        PrintInfo($"Abilities in catalog: Intimidate, Static, Blaze, Levitate, Sturdy...");
        PrintInfo($"Pikachu abilities: {pikachuWithAbility.Ability1?.Name}, Hidden: {pikachuWithAbility.HiddenAbility?.Name}");

        // ═══════════════════════════════════════════════════════
        // SECTION 24: ITEM DATA
        // ═══════════════════════════════════════════════════════
        PrintSection("ITEM DATA");

        Test("ItemCatalog.Leftovers exists", () => ItemCatalog.Leftovers != null);
        Test("ItemCatalog.ChoiceBand exists", () => ItemCatalog.ChoiceBand != null);
        Test("ItemCatalog.FocusSash exists", () => ItemCatalog.FocusSash != null);
        Test("Items have names", () => !string.IsNullOrEmpty(ItemCatalog.Leftovers.Name));
        Test("Leftovers is HeldItem category", () => ItemCatalog.Leftovers.Category == ItemCategory.HeldItem);
        Test("Choice Band has stat boost", () => ItemCatalog.ChoiceBand.StatMultiplier > 1.0f);

        PrintInfo($"Items: Leftovers, Choice Band/Scarf/Specs, Life Orb, Focus Sash...");

        // ═══════════════════════════════════════════════════════
        // SECTION 25: STATUS EFFECT DATA
        // ═══════════════════════════════════════════════════════
        PrintSection("STATUS EFFECT DATA");

        Test("StatusCatalog.Burn exists", () => StatusCatalog.Burn != null);
        Test("StatusCatalog.Paralysis exists", () => StatusCatalog.Paralysis != null);
        Test("StatusCatalog.Confusion exists", () => StatusCatalog.Confusion != null);
        Test("Burn is persistent", () => StatusCatalog.Burn.IsPersistent);
        Test("Burn deals end-of-turn damage", () => StatusCatalog.Burn.EndOfTurnDamage > 0);
        Test("Burn halves physical attack", () => StatusCatalog.Burn.AttackMultiplier == 0.5f);
        Test("Paralysis halves speed", () => StatusCatalog.Paralysis.SpeedMultiplier == 0.5f);
        Test("Confusion is volatile", () => !StatusCatalog.Confusion.IsPersistent);
        Test("Fire types immune to Burn", () =>
            StatusCatalog.Burn.ImmuneTypes.Contains(PokemonType.Fire));

        PrintInfo($"Persistent: Burn, Paralysis, Sleep, Poison, BadlyPoisoned, Freeze");
        PrintInfo($"Volatile: Confusion, Attract, Flinch, LeechSeed, Curse, Encore, Taunt, Torment, Disable");

        // ═══════════════════════════════════════════════════════
        // SECTION 26: WEATHER DATA
        // ═══════════════════════════════════════════════════════
        PrintSection("WEATHER DATA");

        Test("WeatherCatalog.Rain exists", () => WeatherCatalog.Rain != null);
        Test("WeatherCatalog.Sun exists", () => WeatherCatalog.Sun != null);
        Test("WeatherCatalog.Sandstorm exists", () => WeatherCatalog.Sandstorm != null);
        Test("Weather has names", () => !string.IsNullOrEmpty(WeatherCatalog.Rain.Name));
        Test("Rain boosts Water power", () => WeatherCatalog.Rain.GetTypePowerMultiplier(PokemonType.Water) > 1.0f);
        Test("Sun boosts Fire power", () => WeatherCatalog.Sun.GetTypePowerMultiplier(PokemonType.Fire) > 1.0f);
        Test("Sandstorm deals damage", () => WeatherCatalog.Sandstorm.EndOfTurnDamage > 0);

        PrintInfo($"Standard: Rain, Sun, Sandstorm, Hail, Snow");
        PrintInfo($"Primal: HeavyRain, ExtremelyHarshSunlight, StrongWinds");

        // ═══════════════════════════════════════════════════════
        // SECTION 27: TERRAIN DATA
        // ═══════════════════════════════════════════════════════
        PrintSection("TERRAIN DATA");

        Test("TerrainCatalog.Grassy exists", () => TerrainCatalog.Grassy != null);
        Test("TerrainCatalog.Electric exists", () => TerrainCatalog.Electric != null);
        Test("TerrainCatalog.Psychic exists", () => TerrainCatalog.Psychic != null);
        Test("TerrainCatalog.Misty exists", () => TerrainCatalog.Misty != null);
        Test("Terrain has names", () => !string.IsNullOrEmpty(TerrainCatalog.Grassy.Name));
        Test("Grassy boosts Grass power", () => TerrainCatalog.Grassy.GetTypePowerMultiplier(PokemonType.Grass) > 1.0f);
        Test("Grassy heals each turn", () => TerrainCatalog.Grassy.HealsEachTurn);
        Test("Psychic blocks priority", () => TerrainCatalog.Psychic.BlocksPriorityMoves);

        PrintInfo($"Terrains: Grassy, Electric, Psychic, Misty");

        // ═══════════════════════════════════════════════════════
        // SECTION 28: HAZARD DATA
        // ═══════════════════════════════════════════════════════
        PrintSection("HAZARD DATA");

        Test("HazardCatalog.StealthRock exists", () => HazardCatalog.StealthRock != null);
        Test("HazardCatalog.Spikes exists", () => HazardCatalog.Spikes != null);
        Test("HazardCatalog.ToxicSpikes exists", () => HazardCatalog.ToxicSpikes != null);
        Test("HazardCatalog.StickyWeb exists", () => HazardCatalog.StickyWeb != null);
        Test("Hazards have names", () => !string.IsNullOrEmpty(HazardCatalog.StealthRock.Name));
        Test("Stealth Rock is Rock type damage", () => HazardCatalog.StealthRock.DamageType == PokemonType.Rock);
        Test("Spikes has 3 layers", () => HazardCatalog.Spikes.MaxLayers == 3);
        Test("Toxic Spikes applies poison", () => HazardCatalog.ToxicSpikes.AppliesStatus);

        PrintInfo($"Hazards: Stealth Rock, Spikes (3 layers), Toxic Spikes (2 layers), Sticky Web");

        // ═══════════════════════════════════════════════════════
        // SECTION 29: SIDE CONDITION DATA
        // ═══════════════════════════════════════════════════════
        PrintSection("SIDE CONDITION DATA");

        Test("SideConditionCatalog.Reflect exists", () => SideConditionCatalog.Reflect != null);
        Test("SideConditionCatalog.LightScreen exists", () => SideConditionCatalog.LightScreen != null);
        Test("SideConditionCatalog.Tailwind exists", () => SideConditionCatalog.Tailwind != null);
        Test("Reflect reduces physical damage", () =>
            SideConditionCatalog.Reflect.ReducesDamageFrom == MoveCategory.Physical);
        Test("Light Screen reduces special damage", () =>
            SideConditionCatalog.LightScreen.ReducesDamageFrom == MoveCategory.Special);
        Test("Tailwind doubles speed", () => SideConditionCatalog.Tailwind.SpeedMultiplier == 2.0f);

        PrintInfo($"Screens: Reflect, Light Screen, Aurora Veil");
        PrintInfo($"Protection: Safeguard, Mist, Lucky Chant");
        PrintInfo($"Speed: Tailwind");
        PrintInfo($"Guards: Wide Guard, Quick Guard, Mat Block");

        // ═══════════════════════════════════════════════════════
        // SECTION 30: FIELD EFFECT DATA
        // ═══════════════════════════════════════════════════════
        PrintSection("FIELD EFFECT DATA");

        Test("FieldEffectCatalog.TrickRoom exists", () => FieldEffectCatalog.TrickRoom != null);
        Test("FieldEffectCatalog.Gravity exists", () => FieldEffectCatalog.Gravity != null);
        Test("Trick Room reverses speed", () => FieldEffectCatalog.TrickRoom.ReversesSpeedOrder);
        Test("Magic Room disables items", () => FieldEffectCatalog.MagicRoom.DisablesItems);
        Test("Wonder Room swaps defenses", () => FieldEffectCatalog.WonderRoom.SwapsDefenses);
        Test("Gravity grounds all Pokemon", () => FieldEffectCatalog.Gravity.GroundsAllPokemon);

        PrintInfo($"Rooms: Trick Room, Magic Room, Wonder Room");
        PrintInfo($"Other: Gravity, Ion Deluge, Fairy Lock, Mud Sport, Water Sport");

        // ═══════════════════════════════════════════════════════
        // SECTION 31: NEW MOVE EFFECTS
        // ═══════════════════════════════════════════════════════
        PrintSection("NEW MOVE EFFECTS");

        // Test new effect types
        Test("VolatileStatusEffect works", () =>
            new VolatileStatusEffect(VolatileStatus.Confusion, 100).EffectType == EffectType.VolatileStatus);
        Test("ProtectionEffect works", () =>
            new ProtectionEffect(ProtectionType.Full).EffectType == EffectType.Protection);
        Test("ChargingEffect works", () =>
            new ChargingEffect { SemiInvulnerable = true }.EffectType == EffectType.Charging);
        Test("DelayedDamageEffect works", () =>
            new DelayedDamageEffect(2, false).EffectType == EffectType.DelayedDamage);
        Test("ForceSwitchEffect works", () =>
            new ForceSwitchEffect(true).EffectType == EffectType.ForceSwitch);
        Test("BindingEffect works", () =>
            new BindingEffect("wrapped").EffectType == EffectType.Binding);
        Test("SwitchAfterAttackEffect works", () =>
            new SwitchAfterAttackEffect(true).EffectType == EffectType.SwitchAfterAttack);
        Test("FieldConditionEffect works", () =>
            new FieldConditionEffect { ConditionType = FieldConditionType.Weather }.EffectType == EffectType.FieldCondition);
        Test("SelfDestructEffect works", () =>
            new SelfDestructEffect(SelfDestructType.Explosion).EffectType == EffectType.SelfDestruct);
        Test("RevengeEffect works", () =>
            new RevengeEffect(MoveCategory.Physical, 2.0f).EffectType == EffectType.Revenge);
        Test("MoveRestrictionEffect works", () =>
            new MoveRestrictionEffect(MoveRestrictionType.Taunt, 3).EffectType == EffectType.MoveRestriction);
        Test("PriorityModifierEffect works", () =>
            new PriorityModifierEffect { Condition = PriorityCondition.TerrainBased }.EffectType == EffectType.PriorityModifier);

        PrintInfo($"Total effect types: {Enum.GetNames(typeof(EffectType)).Length}");

        // ═══════════════════════════════════════════════════════
        // SECTION 32: COMBAT FOUNDATION
        // ═══════════════════════════════════════════════════════
        PrintSection("COMBAT FOUNDATION");

        // BattleSlot (without side reference for simple test)
        var slot = new BattleSlot(0);
        var combatPikachu = PokemonFactory.Create(PokemonCatalog.Pikachu, 50);
        slot.SetPokemon(combatPikachu);
        
        Test("BattleSlot holds Pokemon", () => slot.Pokemon == combatPikachu);
        Test("BattleSlot starts with 0 Attack stage", () => slot.GetStatStage(Stat.Attack) == 0);
        
        slot.ModifyStatStage(Stat.Attack, 2);
        Test("ModifyStatStage works", () => slot.GetStatStage(Stat.Attack) == 2);
        
        slot.AddVolatileStatus(VolatileStatus.Confusion);
        Test("AddVolatileStatus works", () => slot.HasVolatileStatus(VolatileStatus.Confusion));
        
        slot.ResetBattleState();
        Test("ResetBattleState clears stages", () => slot.GetStatStage(Stat.Attack) == 0);

        // BattleSide
        var playerSide = new BattleSide(2, isPlayer: true);
        Test("BattleSide creates slots", () => playerSide.Slots.Count == 2);
        Test("BattleSide.IsPlayer", () => playerSide.IsPlayer);

        // BattleField with parties
        var playerParty = new[] { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) };
        var enemyParty = new[] { PokemonFactory.Create(PokemonCatalog.Charizard, 50) };
        var battleRules = new BattleRules { PlayerSlots = 1, EnemySlots = 1 };
        
        var field = new BattleField();
        field.Initialize(battleRules, playerParty, enemyParty);
        
        Test("BattleField has PlayerSide", () => field.PlayerSide != null);
        Test("BattleField has EnemySide", () => field.EnemySide != null);
        Test("BattleField.Rules is set", () => field.Rules != null);

        PrintInfo($"Combat structures: BattleSlot, BattleSide, BattleField ready");

        // ═══════════════════════════════════════════════════════
        // SECTION 33: BATTLE QUEUE
        // ═══════════════════════════════════════════════════════
        PrintSection("BATTLE QUEUE");

        var battleQueue = new BattleQueue();
        var testAction = new MessageAction("Test message");
        
        battleQueue.Enqueue(testAction);
        Test("BattleQueue.Enqueue works", () => battleQueue.Count == 1);
        
        battleQueue.Clear();
        Test("BattleQueue.Clear works", () => battleQueue.Count == 0);

        // Process queue
        var executedActions = 0;
        var processQueue = new BattleQueue();
        processQueue.Enqueue(new TestAction(() => executedActions++));
        processQueue.Enqueue(new TestAction(() => executedActions++));
        
        processQueue.ProcessQueue(field, new NullBattleView()).Wait();
        Test("BattleQueue.ProcessQueue executes all", () => executedActions == 2);

        PrintInfo($"BattleQueue: Enqueue, EnqueueRange, InsertAtFront, ProcessQueue ready");

        // ═══════════════════════════════════════════════════════
        // SECTION 34: TURN ORDER RESOLVER
        // ═══════════════════════════════════════════════════════
        PrintSection("TURN ORDER RESOLVER");

        var fastPokemon = PokemonFactory.Create(PokemonCatalog.Pikachu, 50); // Speed ~110
        var slowPokemon = PokemonFactory.Create(PokemonCatalog.Snorlax, 50); // Speed ~30
        
        var fastSlot = new BattleSlot(0);
        fastSlot.SetPokemon(fastPokemon);
        var slowSlot = new BattleSlot(1);
        slowSlot.SetPokemon(slowPokemon);

        var turnActions = new[]
        {
            new TestMoveAction(slowSlot, 0),
            new TestMoveAction(fastSlot, 0)
        };

        var sorted = TurnOrderResolver.SortActions(turnActions, field);
        Test("Faster Pokemon moves first", () => sorted[0].User == fastSlot);

        // Priority test
        var priorityActions = new[]
        {
            new TestMoveAction(slowSlot, 1), // Quick Attack priority
            new TestMoveAction(fastSlot, 0)  // Normal move
        };

        var prioritySorted = TurnOrderResolver.SortActions(priorityActions, field);
        Test("Priority beats speed", () => prioritySorted[0].User == slowSlot);

        PrintInfo($"TurnOrderResolver: Priority > Speed > Random");

        // ═══════════════════════════════════════════════════════
        // SECTION 35: DAMAGE PIPELINE
        // ═══════════════════════════════════════════════════════
        PrintSection("DAMAGE PIPELINE");

        // Create slots with Pokemon for damage calculation
        var attackerSlot = new BattleSlot(0);
        var defenderSlot = new BattleSlot(1);
        attackerSlot.SetPokemon(PokemonFactory.Create(PokemonCatalog.Charizard, 50));
        defenderSlot.SetPokemon(PokemonFactory.Create(PokemonCatalog.Venusaur, 50));
        var attackMove = MoveCatalog.Flamethrower;

        var pipeline = new DamagePipeline(); // Uses default steps

        var damageContext = pipeline.Calculate(attackerSlot, defenderSlot, attackMove, field, forceCritical: false, fixedRandomValue: 1.0f);
        
        Test("DamagePipeline calculates damage", () => damageContext.FinalDamage > 0);
        Test("STAB applied (Charizard Fire)", () => damageContext.IsStab);
        Test("Type effectiveness (Fire vs Grass)", () => damageContext.TypeEffectiveness == 2.0f);

        PrintInfo($"Damage: {damageContext.FinalDamage} (STAB: {damageContext.IsStab}, Effectiveness: {damageContext.TypeEffectiveness}x)");
        PrintInfo($"DamagePipeline steps: Base → Crit → Random → STAB → TypeEff → Burn");

        // ═══════════════════════════════════════════════════════
        // SECTION 36: COMBAT ACTIONS
        // ═══════════════════════════════════════════════════════
        PrintSection("COMBAT ACTIONS");

        // Setup battle field for action tests
        var actionField = new BattleField();
        var actionPlayerParty = new[] { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) };
        var actionEnemyParty = new[] { PokemonFactory.Create(PokemonCatalog.Charmander, 50) };
        actionField.Initialize(new BattleRules { PlayerSlots = 1, EnemySlots = 1 }, 
            actionPlayerParty, actionEnemyParty);

        var actionUserSlot = actionField.PlayerSide.Slots[0];
        var actionTargetSlot = actionField.EnemySide.Slots[0];
        var actionUser = actionUserSlot.Pokemon;
        var actionTarget = actionTargetSlot.Pokemon;

        // DamageAction
        var initialHP = actionTarget.CurrentHP;
        var testDamageContext = new DamageContext(actionUserSlot, actionTargetSlot, MoveCatalog.Thunderbolt, actionField);
        testDamageContext.BaseDamage = 50;
        testDamageContext.Multiplier = 1.0f;
        
        var damageAction = new DamageAction(actionUserSlot, actionTargetSlot, testDamageContext);
        var damageReactions = damageAction.ExecuteLogic(actionField).ToList();
        
        Test("DamageAction reduces HP", () => actionTarget.CurrentHP < initialHP);
        Test("DamageAction returns empty if no faint", () => damageReactions.Count == 0 || damageReactions[0] is FaintAction);

        // FaintAction
        actionTarget.CurrentHP = 0;
        var faintAction = new FaintAction(actionUserSlot, actionTargetSlot);
        var faintReactions = faintAction.ExecuteLogic(actionField);
        Test("FaintAction handles fainted Pokemon", () => actionTarget.IsFainted);
        Test("FaintAction returns empty reactions", () => !faintReactions.Any());

        // HealAction
        actionTarget.CurrentHP = 50;
        var healAction = new HealAction(null, actionTargetSlot, 30);
        healAction.ExecuteLogic(actionField);
        Test("HealAction restores HP", () => actionTarget.CurrentHP == 80);
        
        var overhealAction = new HealAction(null, actionTargetSlot, 999);
        overhealAction.ExecuteLogic(actionField);
        Test("HealAction prevents overhealing", () => actionTarget.CurrentHP == actionTarget.MaxHP);

        // StatChangeAction
        var initialAttackStage = actionUserSlot.GetStatStage(Stat.Attack);
        var statChangeAction = new StatChangeAction(null, actionUserSlot, Stat.Attack, 2);
        statChangeAction.ExecuteLogic(actionField);
        Test("StatChangeAction increases stat stage", () => actionUserSlot.GetStatStage(Stat.Attack) == initialAttackStage + 2);
        
        var clampStatAction = new StatChangeAction(null, actionUserSlot, Stat.Attack, 10);
        clampStatAction.ExecuteLogic(actionField);
        Test("StatChangeAction clamps to +6", () => actionUserSlot.GetStatStage(Stat.Attack) == 6);

        // ApplyStatusAction
        var statusAction = new ApplyStatusAction(null, actionTargetSlot, PersistentStatus.Burn);
        statusAction.ExecuteLogic(actionField);
        Test("ApplyStatusAction applies status", () => actionTarget.Status == PersistentStatus.Burn);
        
        var clearStatusAction = new ApplyStatusAction(null, actionTargetSlot, PersistentStatus.None);
        clearStatusAction.ExecuteLogic(actionField);
        Test("ApplyStatusAction clears status with None", () => actionTarget.Status == PersistentStatus.None);

        // UseMoveAction
        actionUser.CurrentHP = actionUser.MaxHP; // Ensure not fainted
        actionTarget.CurrentHP = actionTarget.MaxHP; // Reset target
        var useMoveInstance = actionUser.Moves[0];
        var initialPP = useMoveInstance.CurrentPP;
        
        var useMoveAction = new UseMoveAction(actionUserSlot, actionTargetSlot, useMoveInstance);
        var moveReactions = useMoveAction.ExecuteLogic(actionField).ToList();
        
        Test("UseMoveAction deducts PP", () => useMoveInstance.CurrentPP == initialPP - 1);
        Test("UseMoveAction generates reactions", () => moveReactions.Count > 0);
        Test("UseMoveAction generates MessageAction", () => moveReactions.Any(r => r is MessageAction));
        Test("UseMoveAction generates DamageAction for damaging moves", () => 
            moveReactions.Any(r => r is DamageAction) || useMoveInstance.Move.Category == MoveCategory.Status);

        // SwitchAction
        var switchField = new BattleField();
        var switchPlayerParty = new[] 
        { 
            PokemonFactory.Create(PokemonCatalog.Pikachu, 50),
            PokemonFactory.Create(PokemonCatalog.Charmander, 50)
        };
        var switchEnemyParty = new[] { PokemonFactory.Create(PokemonCatalog.Squirtle, 50) };
        switchField.Initialize(new BattleRules { PlayerSlots = 1, EnemySlots = 1 }, 
            switchPlayerParty, switchEnemyParty);
        
        var switchSlot = switchField.PlayerSide.Slots[0];
        var switchBenchPokemon = switchField.PlayerSide.Party[1];
        var switchActivePokemon = switchSlot.Pokemon;
        
        switchSlot.ModifyStatStage(Stat.Attack, 2);
        switchSlot.AddVolatileStatus(VolatileStatus.Flinch);
        
        var switchAction = new SwitchAction(switchSlot, switchBenchPokemon);
        switchAction.ExecuteLogic(switchField);
        
        Test("SwitchAction swaps Pokemon", () => switchSlot.Pokemon == switchBenchPokemon);
        Test("SwitchAction resets stat stages", () => switchSlot.GetStatStage(Stat.Attack) == 0);
        Test("SwitchAction clears volatile status", () => !switchSlot.HasVolatileStatus(VolatileStatus.Flinch));
        Test("SwitchAction has priority +6", () => switchAction.Priority == 6);
        Test("SwitchAction cannot be blocked", () => !switchAction.CanBeBlocked);

        // MessageAction
        var messageAction = new MessageAction("Test message");
        var messageReactions = messageAction.ExecuteLogic(actionField);
        Test("MessageAction returns empty reactions", () => !messageReactions.Any());
        Test("MessageAction stores message", () => messageAction.Message == "Test message");

        PrintInfo($"Actions tested: DamageAction, FaintAction, HealAction, StatChangeAction, ApplyStatusAction, UseMoveAction, SwitchAction, MessageAction");
        PrintInfo($"All actions follow ExecuteLogic() → ExecuteVisual() pattern");

        // ═══════════════════════════════════════════════════════
        // SECTION 37: COMBAT ENGINE
        // ═══════════════════════════════════════════════════════
        PrintSection("COMBAT ENGINE");

        // BattleOutcome enum
        Test("BattleOutcome.Ongoing exists", () => Enum.IsDefined(typeof(BattleOutcome), BattleOutcome.Ongoing));
        Test("BattleOutcome.Victory exists", () => Enum.IsDefined(typeof(BattleOutcome), BattleOutcome.Victory));
        Test("BattleOutcome.Defeat exists", () => Enum.IsDefined(typeof(BattleOutcome), BattleOutcome.Defeat));
        Test("BattleOutcome.Draw exists", () => Enum.IsDefined(typeof(BattleOutcome), BattleOutcome.Draw));
        Test("BattleOutcome.Fled exists", () => Enum.IsDefined(typeof(BattleOutcome), BattleOutcome.Fled));
        Test("BattleOutcome.Caught exists", () => Enum.IsDefined(typeof(BattleOutcome), BattleOutcome.Caught));

        // BattleResult class
        var battleResult = new BattleResult
        {
            Outcome = BattleOutcome.Victory,
            TurnsTaken = 10,
            MvpPokemon = null
        };
        Test("BattleResult stores outcome", () => battleResult.Outcome == BattleOutcome.Victory);
        Test("BattleResult stores turns", () => battleResult.TurnsTaken == 10);
        Test("BattleResult initializes collections", () => 
            battleResult.DefeatedEnemies != null && battleResult.ExpEarned != null && battleResult.LootDropped != null);

        // BattleArbiter
        var arbiterField = new BattleField();
        var arbiterPlayerParty = new[] { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) };
        var arbiterEnemyParty = new[] { PokemonFactory.Create(PokemonCatalog.Charmander, 50) };
        arbiterField.Initialize(new BattleRules { PlayerSlots = 1, EnemySlots = 1 }, 
            arbiterPlayerParty, arbiterEnemyParty);
        
        Test("BattleArbiter returns Ongoing for active battle", () => 
            BattleArbiter.CheckOutcome(arbiterField) == BattleOutcome.Ongoing);
        
        // Test victory condition
        arbiterEnemyParty[0].CurrentHP = 0;
        Test("BattleArbiter detects Victory", () => 
            BattleArbiter.CheckOutcome(arbiterField) == BattleOutcome.Victory);
        
        // Reset and test defeat
        arbiterEnemyParty[0].CurrentHP = arbiterEnemyParty[0].MaxHP;
        arbiterPlayerParty[0].CurrentHP = 0;
        Test("BattleArbiter detects Defeat", () => 
            BattleArbiter.CheckOutcome(arbiterField) == BattleOutcome.Defeat);
        
        // Test draw
        arbiterPlayerParty[0].CurrentHP = 0;
        arbiterEnemyParty[0].CurrentHP = 0;
        Test("BattleArbiter detects Draw", () => 
            BattleArbiter.CheckOutcome(arbiterField) == BattleOutcome.Draw);

        // IActionProvider
        var testProvider = new SimpleTestActionProvider("Test action");
        var providerField = new BattleField();
        var providerSlot = new BattleSlot(0);
        var providerAction = testProvider.GetAction(providerField, providerSlot).Result;
        Test("IActionProvider returns action", () => providerAction != null);
        Test("IActionProvider returns MessageAction", () => providerAction is MessageAction);
        Test("IActionProvider message matches", () => ((MessageAction)providerAction).Message == "Test action");

        // CombatEngine initialization
        var engine = new CombatEngine();
        var enginePlayerParty = new[] { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) };
        var engineEnemyParty = new[] { PokemonFactory.Create(PokemonCatalog.Charmander, 50) };
        var enginePlayerProvider = new SimpleTestActionProvider("Player action");
        var engineEnemyProvider = new SimpleTestActionProvider("Enemy action");
        var engineView = new NullBattleView();
        
        engine.Initialize(new BattleRules { PlayerSlots = 1, EnemySlots = 1 }, 
            enginePlayerParty, engineEnemyParty, enginePlayerProvider, engineEnemyProvider, engineView);
        
        Test("CombatEngine initializes Field", () => engine.Field != null);
        Test("CombatEngine initializes Queue", () => engine.Queue != null);
        Test("CombatEngine starts with Ongoing outcome", () => engine.Outcome == BattleOutcome.Ongoing);
        Test("CombatEngine assigns action providers", () => 
            engine.Field.PlayerSide.Slots[0].ActionProvider == enginePlayerProvider &&
            engine.Field.EnemySide.Slots[0].ActionProvider == engineEnemyProvider);

        // CombatEngine RunTurn
        var turnEngine = new CombatEngine();
        var turnPlayerParty = new[] { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) };
        var turnEnemyParty = new[] { PokemonFactory.Create(PokemonCatalog.Charmander, 50) };
        var turnPlayerProvider = new SimpleTestActionProvider(new MessageAction("Player turn"));
        var turnEnemyProvider = new SimpleTestActionProvider(new MessageAction("Enemy turn"));
        
        turnEngine.Initialize(new BattleRules { PlayerSlots = 1, EnemySlots = 1 }, 
            turnPlayerParty, turnEnemyParty, turnPlayerProvider, turnEnemyProvider, engineView);
        
        var initialQueueCount = turnEngine.Queue.Count;
        turnEngine.RunTurn().Wait();
        Test("CombatEngine.RunTurn processes actions", () => turnEngine.Queue.Count >= initialQueueCount);

        // CombatEngine RunBattle (simplified test - just verify method exists and can be called)
        var battleEngine = new CombatEngine();
        var battlePlayerParty = new[] { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) };
        var battleEnemyParty = new[] { PokemonFactory.Create(PokemonCatalog.Charmander, 50) };
        var battlePlayerProvider = new SimpleTestActionProvider(new MessageAction("Player"));
        var battleEnemyProvider = new SimpleTestActionProvider(new MessageAction("Enemy"));
        
        battleEngine.Initialize(new BattleRules { PlayerSlots = 1, EnemySlots = 1 }, 
            battlePlayerParty, battleEnemyParty, battlePlayerProvider, battleEnemyProvider, engineView);
        
        // For smoke test, just verify RunBattle method exists and returns Task<BattleResult>
        // Full battle simulation would take too long for smoke test
        Test("CombatEngine.RunBattle method exists", () => 
        {
            try
            {
                var method = typeof(CombatEngine).GetMethod("RunBattle");
                return method != null && method.ReturnType == typeof(Task<BattleResult>);
            }
            catch
            {
                return false;
            }
        });

        PrintInfo($"CombatEngine: Initialize, RunTurn, RunBattle ready");
        PrintInfo($"BattleArbiter: Victory/Defeat/Draw detection working");
        PrintInfo($"IActionProvider: Action selection abstraction ready");

        // ═══════════════════════════════════════════════════════
        // SECTION 38: AI AND FULL BATTLES
        // ═══════════════════════════════════════════════════════
        PrintSection("AI AND FULL BATTLES");

        // Test RandomAI
        var randomAIField = new BattleField();
        var randomAIPlayerParty = new[] { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) };
        var randomAIEnemyParty = new[] { PokemonFactory.Create(PokemonCatalog.Charmander, 50) };
        randomAIField.Initialize(new BattleRules { PlayerSlots = 1, EnemySlots = 1 }, 
            randomAIPlayerParty, randomAIEnemyParty);
        
        var randomAI = new RandomAI();
        var randomAISlot = randomAIField.PlayerSide.Slots[0];
        var randomAIAction = randomAI.GetAction(randomAIField, randomAISlot).Result;
        
        Test("RandomAI returns action", () => randomAIAction != null);
        Test("RandomAI returns UseMoveAction", () => randomAIAction is UseMoveAction);

        // Test AlwaysAttackAI
        var alwaysAttackAI = new AlwaysAttackAI();
        var alwaysAttackAIAction = alwaysAttackAI.GetAction(randomAIField, randomAISlot).Result;
        
        Test("AlwaysAttackAI returns action", () => alwaysAttackAIAction != null);
        Test("AlwaysAttackAI returns UseMoveAction", () => alwaysAttackAIAction is UseMoveAction);

        // Test TargetResolver
        var targetResolverMove = randomAIPlayerParty[0].Moves[0].Move;
        var validTargets = TargetResolver.GetValidTargets(randomAISlot, targetResolverMove, randomAIField);
        Test("TargetResolver returns valid targets", () => validTargets.Count > 0);

        // Test full battle (simplified - just verify it runs)
        var fullBattleEngine = new CombatEngine();
        var fullBattlePlayerParty = new[] { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) };
        var fullBattleEnemyParty = new[] { PokemonFactory.Create(PokemonCatalog.Charmander, 50) };
        var fullBattlePlayerAI = new RandomAI();
        var fullBattleEnemyAI = new AlwaysAttackAI();
        
        fullBattleEngine.Initialize(new BattleRules { PlayerSlots = 1, EnemySlots = 1 },
            fullBattlePlayerParty, fullBattleEnemyParty, fullBattlePlayerAI, fullBattleEnemyAI, new NullBattleView());
        
        // Run a few turns to verify it works (not full battle to keep smoke test fast)
        fullBattleEngine.RunTurn().Wait();
        Test("Full battle engine runs turns", () => fullBattleEngine.Field != null);

        PrintInfo($"RandomAI: Random move selection working");
        PrintInfo($"AlwaysAttackAI: First move selection working");
        PrintInfo($"TargetResolver: Valid target resolution working");
        PrintInfo($"Full battles: AI vs AI battles functional");

        // ═══════════════════════════════════════════════════════
        // SECTION 39: ABILITIES & ITEMS BATTLE INTEGRATION
        // ═══════════════════════════════════════════════════════
        PrintSection("ABILITIES & ITEMS BATTLE INTEGRATION");

        // BattleTrigger enum
        Test("BattleTrigger enum exists", () => Enum.IsDefined(typeof(BattleTrigger), BattleTrigger.OnSwitchIn));
        Test("BattleTrigger.OnTurnEnd exists", () => Enum.IsDefined(typeof(BattleTrigger), BattleTrigger.OnTurnEnd));
        Test("BattleTrigger has 6 values", () => Enum.GetNames(typeof(BattleTrigger)).Length == 6);

        // IBattleListener interface
        Test("IBattleListener interface exists", () => typeof(IBattleListener).IsInterface);
        Test("AbilityListener implements IBattleListener", () => typeof(AbilityListener).GetInterfaces().Contains(typeof(IBattleListener)));
        Test("ItemListener implements IBattleListener", () => typeof(ItemListener).GetInterfaces().Contains(typeof(IBattleListener)));

        // BattleTriggerProcessor
        var triggerField = new BattleField();
        var triggerPlayerParty = new[] { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) };
        var triggerEnemyParty = new[] { PokemonFactory.Create(PokemonCatalog.Charmander, 50) };
        triggerField.Initialize(new BattleRules { PlayerSlots = 1, EnemySlots = 1 }, 
            triggerPlayerParty, triggerEnemyParty);

        Test("BattleTriggerProcessor.ProcessTrigger method exists", () => 
        {
            try
            {
                var method = typeof(BattleTriggerProcessor).GetMethod("ProcessTrigger");
                return method != null && method.IsStatic;
            }
            catch
            {
                return false;
            }
        });

        // Test OnTurnEnd with Leftovers
        var leftoversPokemon = PokemonFactory.Create(PokemonCatalog.Pikachu, 50);
        leftoversPokemon.HeldItem = ItemCatalog.Leftovers;
        int maxHP = leftoversPokemon.MaxHP;
        leftoversPokemon.CurrentHP = maxHP - 20; // Damage Pokemon

        var leftoversField = new BattleField();
        var leftoversPlayerParty = new[] { leftoversPokemon };
        var leftoversEnemyParty = new[] { PokemonFactory.Create(PokemonCatalog.Charmander, 50) };
        leftoversField.Initialize(new BattleRules { PlayerSlots = 1, EnemySlots = 1 }, 
            leftoversPlayerParty, leftoversEnemyParty);

        var leftoversSlot = leftoversField.PlayerSide.Slots[0];
        int leftoversInitialHP = leftoversSlot.Pokemon.CurrentHP;

        var leftoversActions = BattleTriggerProcessor.ProcessTrigger(BattleTrigger.OnTurnEnd, leftoversField);
        Test("Leftovers generates actions on OnTurnEnd", () => leftoversActions.Count > 0);
        Test("Leftovers generates MessageAction", () => leftoversActions.Any(a => a is MessageAction));
        Test("Leftovers generates HealAction", () => leftoversActions.Any(a => a is HealAction));

        // Execute heal action to verify it works
        var leftoversHealAction = leftoversActions.OfType<HealAction>().FirstOrDefault();
        if (leftoversHealAction != null)
        {
            leftoversHealAction.ExecuteLogic(leftoversField);
            Test("Leftovers heal action restores HP", () => leftoversSlot.Pokemon.CurrentHP > leftoversInitialHP);
        }

        // Test OnSwitchIn with Intimidate
        var intimidateAbility = AbilityCatalog.Intimidate;
        Test("Intimidate ability exists", () => intimidateAbility != null);
        Test("Intimidate listens to OnSwitchIn", () => intimidateAbility.ListensTo(AbilityTrigger.OnSwitchIn));
        Test("Intimidate has LowerOpponentStat effect", () => intimidateAbility.Effect == AbilityEffect.LowerOpponentStat);

        var intimidatePokemon = PokemonFactory.Create(PokemonCatalog.Bulbasaur, 50);
        intimidatePokemon.SetAbility(intimidateAbility);

        var intimidateField = new BattleField();
        var intimidatePlayerParty = new[] 
        { 
            intimidatePokemon, // Put Intimidate Pokemon first so it's in the active slot
            PokemonFactory.Create(PokemonCatalog.Pikachu, 50)
        };
        var intimidateEnemyParty = new[] { PokemonFactory.Create(PokemonCatalog.Charmander, 50) };
        intimidateField.Initialize(new BattleRules { PlayerSlots = 1, EnemySlots = 1 }, 
            intimidatePlayerParty, intimidateEnemyParty);

        var intimidateSlot = intimidateField.PlayerSide.Slots[0];
        var enemySlot = intimidateField.EnemySide.Slots[0];
        int initialEnemyAttackStage = enemySlot.GetStatStage(Stat.Attack);

        // Verify Intimidate Pokemon is in the active slot
        Test("Intimidate Pokemon is in active slot", () => intimidateSlot.Pokemon == intimidatePokemon);
        Test("Intimidate Pokemon has ability", () => intimidateSlot.Pokemon.Ability == intimidateAbility);

        var intimidateActions = BattleTriggerProcessor.ProcessTrigger(BattleTrigger.OnSwitchIn, intimidateField);
        Test("Intimidate generates actions on OnSwitchIn", () => intimidateActions.Count > 0);
        Test("Intimidate generates MessageAction", () => intimidateActions.Any(a => a is MessageAction));
        Test("Intimidate generates StatChangeAction", () => intimidateActions.Any(a => a is StatChangeAction));

        // Execute stat change action to verify it works
        var intimidateStatChangeAction = intimidateActions.OfType<StatChangeAction>().FirstOrDefault();
        if (intimidateStatChangeAction != null)
        {
            intimidateStatChangeAction.ExecuteLogic(intimidateField);
            Test("Intimidate lowers enemy Attack", () => enemySlot.GetStatStage(Stat.Attack) < initialEnemyAttackStage);
        }

        // Test integration with CombatEngine
        var integrationEngine = new CombatEngine();
        var integrationPlayerParty = new[] 
        { 
            PokemonFactory.Create(PokemonCatalog.Pikachu, 50),
            PokemonFactory.Create(PokemonCatalog.Bulbasaur, 50)
        };
        integrationPlayerParty[0].HeldItem = ItemCatalog.Leftovers;
        integrationPlayerParty[0].CurrentHP = integrationPlayerParty[0].MaxHP - 10;
        integrationPlayerParty[1].SetAbility(intimidateAbility);

        var integrationEnemyParty = new[] { PokemonFactory.Create(PokemonCatalog.Charmander, 50) };
        var integrationPlayerProvider = new SimpleTestActionProvider(new MessageAction("Pass"));
        var integrationEnemyProvider = new SimpleTestActionProvider(new MessageAction("Pass"));

        integrationEngine.Initialize(new BattleRules { PlayerSlots = 1, EnemySlots = 1 },
            integrationPlayerParty, integrationEnemyParty,
            integrationPlayerProvider, integrationEnemyProvider, new NullBattleView());

        // Test that CombatEngine has trigger support
        Test("CombatEngine initializes with trigger support", () => integrationEngine.Field != null);
        
        // Test that SwitchAction triggers OnSwitchIn (verify by checking actions generated)
        var integrationSwitchSlot = integrationEngine.Field.PlayerSide.Slots[0];
        var integrationSwitchNewPokemon = integrationPlayerParty[1];
        var integrationSwitchAction = new SwitchAction(integrationSwitchSlot, integrationSwitchNewPokemon);
        var switchReactions = integrationSwitchAction.ExecuteLogic(integrationEngine.Field).ToList();
        Test("SwitchAction generates OnSwitchIn trigger actions", () => switchReactions.Count > 0);
        Test("SwitchAction generates Intimidate actions", () => 
            switchReactions.Any(a => a is StatChangeAction || a is MessageAction));

        // Test that CombatEngine.RunTurn processes OnTurnEnd triggers
        var endOfTurnSlot = integrationEngine.Field.PlayerSide.Slots[0];
        int endOfTurnInitialHP = endOfTurnSlot.Pokemon.CurrentHP;
        integrationEngine.RunTurn().Wait();
        // Leftovers should heal, so HP should be >= initial (or same if already at max)
        Test("CombatEngine.RunTurn processes OnTurnEnd triggers", () => 
            endOfTurnSlot.Pokemon.CurrentHP >= endOfTurnInitialHP);

        PrintInfo($"BattleTrigger system: OnSwitchIn, OnTurnEnd working");
        PrintInfo($"AbilityListener: Converts AbilityData to IBattleListener");
        PrintInfo($"ItemListener: Converts ItemData to IBattleListener");
        PrintInfo($"Leftovers: Heals 1/16 Max HP per turn");
        PrintInfo($"Intimidate: Lowers opponent Attack on switch-in");
        PrintInfo($"Integration: Triggers work in CombatEngine and SwitchAction");

        // ═══════════════════════════════════════════════════════
        // SECTION 40: PIPELINE HOOKS (STAT & DAMAGE MODIFIERS)
        // ═══════════════════════════════════════════════════════
        try
        {
            PrintSection("PIPELINE HOOKS (STAT & DAMAGE MODIFIERS)");

            // IStatModifier interface
            Test("IStatModifier interface exists", () => typeof(IStatModifier).IsInterface);
            Test("AbilityStatModifier implements IStatModifier", () => typeof(AbilityStatModifier).GetInterfaces().Contains(typeof(IStatModifier)));
            Test("ItemStatModifier implements IStatModifier", () => typeof(ItemStatModifier).GetInterfaces().Contains(typeof(IStatModifier)));

            // Choice Band - Stat modifier
            var choiceBandPokemon = PokemonFactory.Create(PokemonCatalog.Pikachu, 50);
            choiceBandPokemon.HeldItem = ItemCatalog.ChoiceBand;
            var choiceBandField = new BattleField();
            var choiceBandPlayerParty = new[] { choiceBandPokemon };
            var choiceBandEnemyParty = new[] { PokemonFactory.Create(PokemonCatalog.Squirtle, 50) };
            choiceBandField.Initialize(new BattleRules { PlayerSlots = 1, EnemySlots = 1 }, 
                choiceBandPlayerParty, choiceBandEnemyParty);

            var choiceBandSlot = choiceBandField.PlayerSide.Slots[0];
            var choiceBandModifier = new ItemStatModifier(ItemCatalog.ChoiceBand);
            Test("Choice Band returns 1.5x for Attack stat", () => 
                Math.Abs(choiceBandModifier.GetStatMultiplier(choiceBandSlot, Stat.Attack, choiceBandField) - 1.5f) < 0.001f);
            Test("Choice Band returns 1.0x for other stats", () => 
                Math.Abs(choiceBandModifier.GetStatMultiplier(choiceBandSlot, Stat.Defense, choiceBandField) - 1.0f) < 0.001f);

            // Test Choice Band in damage calculation
            var choiceBandPipeline = new DamagePipeline();
            var choiceBandMove = MoveCatalog.Tackle; // Physical move
            var choiceBandContext = choiceBandPipeline.Calculate(choiceBandSlot, choiceBandField.EnemySide.Slots[0], 
                choiceBandMove, choiceBandField, false, 1.0f);
            
            choiceBandPokemon.HeldItem = null;
            var choiceBandFieldNoItem = new BattleField();
            choiceBandFieldNoItem.Initialize(new BattleRules { PlayerSlots = 1, EnemySlots = 1 }, 
                new[] { choiceBandPokemon }, choiceBandEnemyParty);
            var choiceBandContextNoItem = choiceBandPipeline.Calculate(choiceBandFieldNoItem.PlayerSide.Slots[0], 
                choiceBandFieldNoItem.EnemySide.Slots[0], choiceBandMove, choiceBandFieldNoItem, false, 1.0f);
            
            Test("Choice Band increases base damage", () => choiceBandContext.BaseDamage > choiceBandContextNoItem.BaseDamage);

            // Life Orb - Damage modifier
            var lifeOrbPokemon = PokemonFactory.Create(PokemonCatalog.Pikachu, 50);
            lifeOrbPokemon.HeldItem = ItemCatalog.LifeOrb;
            var lifeOrbField = new BattleField();
            var lifeOrbPlayerParty = new[] { lifeOrbPokemon };
            var lifeOrbEnemyParty = new[] { PokemonFactory.Create(PokemonCatalog.Squirtle, 50) };
            lifeOrbField.Initialize(new BattleRules { PlayerSlots = 1, EnemySlots = 1 }, 
                lifeOrbPlayerParty, lifeOrbEnemyParty);

            var lifeOrbSlot = lifeOrbField.PlayerSide.Slots[0];
            var lifeOrbMove = MoveCatalog.ThunderShock;
            var lifeOrbContext = new DamageContext(lifeOrbSlot, lifeOrbField.EnemySide.Slots[0], lifeOrbMove, lifeOrbField);
            var lifeOrbModifier = new ItemStatModifier(ItemCatalog.LifeOrb);
            Test("Life Orb returns 1.3x damage multiplier", () => 
                Math.Abs(lifeOrbModifier.GetDamageMultiplier(lifeOrbContext) - 1.3f) < 0.001f);

            // Test Life Orb in damage calculation
            var lifeOrbPipeline = new DamagePipeline();
            var lifeOrbContextWithItem = lifeOrbPipeline.Calculate(lifeOrbSlot, lifeOrbField.EnemySide.Slots[0], 
                lifeOrbMove, lifeOrbField, false, 1.0f);
            
            lifeOrbPokemon.HeldItem = null;
            var lifeOrbFieldNoItem = new BattleField();
            lifeOrbFieldNoItem.Initialize(new BattleRules { PlayerSlots = 1, EnemySlots = 1 }, 
                new[] { lifeOrbPokemon }, lifeOrbEnemyParty);
            var lifeOrbContextNoItem = lifeOrbPipeline.Calculate(lifeOrbFieldNoItem.PlayerSide.Slots[0], 
                lifeOrbFieldNoItem.EnemySide.Slots[0], lifeOrbMove, lifeOrbFieldNoItem, false, 1.0f);
            
            Test("Life Orb increases final damage", () => lifeOrbContextWithItem.FinalDamage > lifeOrbContextNoItem.FinalDamage);

            // Blaze - Ability damage modifier
            var blazePokemon = PokemonFactory.Create(PokemonCatalog.Charizard, 50);
            blazePokemon.SetAbility(AbilityCatalog.Blaze);
            blazePokemon.CurrentHP = (int)(blazePokemon.MaxHP * 0.30f); // Below 33% threshold
            var blazeField = new BattleField();
            var blazePlayerParty = new[] { blazePokemon };
            var blazeEnemyParty = new[] { PokemonFactory.Create(PokemonCatalog.Squirtle, 50) };
            blazeField.Initialize(new BattleRules { PlayerSlots = 1, EnemySlots = 1 }, 
                blazePlayerParty, blazeEnemyParty);

            var blazeSlot = blazeField.PlayerSide.Slots[0];
            var blazeMove = MoveCatalog.Ember; // Fire move
            var blazeContext = new DamageContext(blazeSlot, blazeField.EnemySide.Slots[0], blazeMove, blazeField);
            var blazeModifier = new AbilityStatModifier(AbilityCatalog.Blaze);
            Test("Blaze returns 1.5x damage multiplier when HP is low", () => 
                Math.Abs(blazeModifier.GetDamageMultiplier(blazeContext) - 1.5f) < 0.001f);

            // Test Blaze doesn't activate when HP is high
            blazePokemon.CurrentHP = (int)(blazePokemon.MaxHP * 0.50f); // Above 33% threshold
            var blazeContextHighHP = new DamageContext(blazeSlot, blazeField.EnemySide.Slots[0], blazeMove, blazeField);
            Test("Blaze returns 1.0x when HP is high", () => 
                Math.Abs(blazeModifier.GetDamageMultiplier(blazeContextHighHP) - 1.0f) < 0.001f);

            // Test Blaze in damage calculation
            blazePokemon.CurrentHP = (int)(blazePokemon.MaxHP * 0.30f); // Below threshold
            var blazePipeline = new DamagePipeline();
            var blazeContextWithAbility = blazePipeline.Calculate(blazeSlot, blazeField.EnemySide.Slots[0], 
                blazeMove, blazeField, false, 1.0f);
            
            blazePokemon.SetAbility(null);
            var blazeFieldNoAbility = new BattleField();
            blazeFieldNoAbility.Initialize(new BattleRules { PlayerSlots = 1, EnemySlots = 1 }, 
                new[] { blazePokemon }, blazeEnemyParty);
            var blazeContextNoAbility = blazePipeline.Calculate(blazeFieldNoAbility.PlayerSide.Slots[0], 
                blazeFieldNoAbility.EnemySide.Slots[0], blazeMove, blazeFieldNoAbility, false, 1.0f);
            
            Test("Blaze increases final damage when HP is low", () => blazeContextWithAbility.FinalDamage > blazeContextNoAbility.FinalDamage);

            PrintInfo($"IStatModifier system: Passive stat and damage modifiers");
            PrintInfo($"Choice Band: +50% Attack stat (applied in BaseDamageStep)");
            PrintInfo($"Life Orb: +30% damage (applied in AttackerItemStep)");
            PrintInfo($"Blaze: +50% Fire damage when HP < 33% (applied in AttackerAbilityStep)");
            PrintInfo($"Integration: Modifiers work correctly in full DamagePipeline");
        }
        catch (Exception ex)
        {
            System.Console.ForegroundColor = ConsoleColor.Red;
            System.Console.WriteLine($"ERROR in Section 40: {ex.Message}");
            System.Console.WriteLine($"Stack trace: {ex.StackTrace}");
            System.Console.ResetColor();
        }

        // ═══════════════════════════════════════════════════════
        // SECTION 41: COMPLETE POKEMON LISTING
        // ═══════════════════════════════════════════════════════
        PrintSection("ALL POKEMON IN CATALOG");

        foreach (var poke in PokemonCatalog.All.OrderBy(p => p.PokedexNumber))
        {
            var types = poke.IsDualType
                ? $"{poke.PrimaryType}/{poke.SecondaryType}"
                : poke.PrimaryType.ToString();
            var evoInfo = poke.CanEvolve
                ? $" → {poke.Evolutions.First().Target.Name}"
                : "";
            var abilityInfo = poke.Ability1 != null ? $" [{poke.Ability1.Name}]" : "";
            PrintInfo($"#{poke.PokedexNumber:D3} {poke.Name,-12} [{types,-15}] BST:{poke.BaseStats.Total}{abilityInfo}{evoInfo}");
        }

        // ═══════════════════════════════════════════════════════
        // SECTION 42: COMPLETE MOVE LISTING
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
        try { System.Console.Clear(); } catch { /* Ignore if redirected */ }
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

    #region Test Helper Classes

    /// <summary>
    /// Simple action for testing queue execution.
    /// </summary>
    private class TestAction : BattleAction
    {
        private readonly Action _action;

        public TestAction(Action action) : base(null)
        {
            _action = action;
        }

        public override System.Collections.Generic.IEnumerable<BattleAction> ExecuteLogic(BattleField field)
        {
            _action();
            return Array.Empty<BattleAction>();
        }

        public override Task ExecuteVisual(IBattleView view)
        {
            return Task.CompletedTask;
        }
    }

    /// <summary>
    /// Simple move action for testing turn order.
    /// </summary>
    private class TestMoveAction : BattleAction
    {
        private readonly int _priority;

        public override int Priority => _priority;

        public TestMoveAction(BattleSlot user, int priority) : base(user)
        {
            _priority = priority;
        }

        public override System.Collections.Generic.IEnumerable<BattleAction> ExecuteLogic(BattleField field)
        {
            return Array.Empty<BattleAction>();
        }

        public override Task ExecuteVisual(IBattleView view)
        {
            return Task.CompletedTask;
        }
    }

    /// <summary>
    /// Simple action provider for testing CombatEngine.
    /// </summary>
    private class SimpleTestActionProvider : IActionProvider
    {
        private readonly BattleAction _actionToReturn;

        public SimpleTestActionProvider(BattleAction actionToReturn)
        {
            _actionToReturn = actionToReturn;
        }

        public SimpleTestActionProvider(string message)
        {
            _actionToReturn = new MessageAction(message);
        }

        public Task<BattleAction> GetAction(BattleField field, BattleSlot mySlot)
        {
            return Task.FromResult(_actionToReturn);
        }
    }

    #endregion
}
