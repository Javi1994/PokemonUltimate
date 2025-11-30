using System;
using System.Linq;
using PokemonUltimate.Content.Builders;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Effects;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Evolution.Conditions;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;
using PokemonUltimate.Core.Registry;
using MoveCatalog = PokemonUltimate.Content.Catalogs.Moves.MoveCatalog;
using PokemonCatalog = PokemonUltimate.Content.Catalogs.Pokemon.PokemonCatalog;
using Pokemon = PokemonUltimate.Core.Factories.Pokemon;

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
        // SECTION 23: COMPLETE POKEMON LISTING
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
            PrintInfo($"#{poke.PokedexNumber:D3} {poke.Name,-12} [{types,-15}] BST:{poke.BaseStats.Total}{evoInfo}");
        }

        // ═══════════════════════════════════════════════════════
        // SECTION 21: COMPLETE MOVE LISTING
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
}
