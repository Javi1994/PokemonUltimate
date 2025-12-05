using System;
using System.Collections.Generic;
using NUnit.Framework;
using PokemonUltimate.Content.Builders;
using PokemonUltimate.Content.Catalogs.Moves;
using PokemonUltimate.Content.Catalogs.Pokemon;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;
using MoveCatalog = PokemonUltimate.Content.Catalogs.Moves.MoveCatalog;
using PokemonBuilder = PokemonUltimate.Content.Builders.Pokemon;

namespace PokemonUltimate.Tests.Data.Builders
{
    /// <summary>
    /// Edge case tests for all builders in the system.
    /// Tests null handling, limits, invalid values, and boundary conditions.
    /// </summary>
    [TestFixture]
    public class BuilderEdgeCasesTests
    {
        #region PokemonBuilder Edge Cases

        [Test]
        public void PokemonBuilder_EmptyName_Allowed()
        {
            // Empty name is technically allowed (though not recommended)
            var pokemon = PokemonBuilder.Define("", 1)
                .Type(PokemonType.Normal)
                .Stats(100, 100, 100, 100, 100, 100)
                .Build();

            Assert.That(pokemon.Name, Is.EqualTo(""));
        }

        [Test]
        public void PokemonBuilder_PokedexNumber0_Allowed()
        {
            var pokemon = PokemonBuilder.Define("TestMon", 0)
                .Type(PokemonType.Normal)
                .Stats(100, 100, 100, 100, 100, 100)
                .Build();

            Assert.That(pokemon.PokedexNumber, Is.EqualTo(0));
        }

        [Test]
        public void PokemonBuilder_NegativePokedexNumber_Allowed()
        {
            // Not recommended but allowed for special cases
            var pokemon = PokemonBuilder.Define("TestMon", -1)
                .Type(PokemonType.Normal)
                .Stats(100, 100, 100, 100, 100, 100)
                .Build();

            Assert.That(pokemon.PokedexNumber, Is.EqualTo(-1));
        }

        [Test]
        public void PokemonBuilder_ZeroStats_Allowed()
        {
            var pokemon = PokemonBuilder.Define("ZeroStatMon", 1)
                .Type(PokemonType.Normal)
                .Stats(0, 0, 0, 0, 0, 0)
                .Build();

            Assert.That(pokemon.BaseStats.Total, Is.EqualTo(0));
        }

        [Test]
        public void PokemonBuilder_VeryHighStats_Allowed()
        {
            var pokemon = PokemonBuilder.Define("SuperMon", 1)
                .Type(PokemonType.Normal)
                .Stats(999, 999, 999, 999, 999, 999)
                .Build();

            Assert.That(pokemon.BaseStats.Total, Is.EqualTo(5994));
        }

        [Test]
        public void PokemonBuilder_SameTypeTwice_Allowed()
        {
            // Fire/Fire - should work but treated as single type
            var pokemon = PokemonBuilder.Define("FireFire", 1)
                .Types(PokemonType.Fire, PokemonType.Fire)
                .Stats(100, 100, 100, 100, 100, 100)
                .Build();

            Assert.That(pokemon.PrimaryType, Is.EqualTo(PokemonType.Fire));
            Assert.That(pokemon.SecondaryType, Is.EqualTo(PokemonType.Fire));
        }

        [Test]
        public void PokemonBuilder_GenderRatio_EdgeValues()
        {
            var allMale = PokemonBuilder.Define("AllMale", 1)
                .Type(PokemonType.Normal)
                .Stats(100, 100, 100, 100, 100, 100)
                .GenderRatio(100.0f)
                .Build();
            Assert.That(allMale.IsMaleOnly, Is.True);

            var allFemale = PokemonBuilder.Define("AllFemale", 2)
                .Type(PokemonType.Normal)
                .Stats(100, 100, 100, 100, 100, 100)
                .GenderRatio(0.0f)
                .Build();
            Assert.That(allFemale.IsFemaleOnly, Is.True);

            var genderless = PokemonBuilder.Define("Genderless", 3)
                .Type(PokemonType.Normal)
                .Stats(100, 100, 100, 100, 100, 100)
                .Genderless()
                .Build();
            Assert.That(genderless.IsGenderless, Is.True);
        }

        [Test]
        public void PokemonBuilder_NoEvolutions_Allowed()
        {
            var pokemon = PokemonBuilder.Define("NoEvo", 1)
                .Type(PokemonType.Normal)
                .Stats(100, 100, 100, 100, 100, 100)
                .Build();

            Assert.That(pokemon.Evolutions, Is.Empty);
            Assert.That(pokemon.CanEvolve, Is.False);
        }

        [Test]
        public void PokemonBuilder_NoLearnset_Allowed()
        {
            var pokemon = PokemonBuilder.Define("NoMoves", 1)
                .Type(PokemonType.Normal)
                .Stats(100, 100, 100, 100, 100, 100)
                .Build();

            Assert.That(pokemon.Learnset, Is.Empty);
        }

        #endregion

        #region MoveBuilder Edge Cases

        [Test]
        public void MoveBuilder_EmptyName_Allowed()
        {
            var move = Move.Define("")
                .Type(PokemonType.Normal)
                .Physical(50, 100, 10)
                .Build();

            Assert.That(move.Name, Is.EqualTo(""));
        }

        [Test]
        public void MoveBuilder_ZeroPower_Allowed()
        {
            // Status moves have 0 power
            var move = Move.Define("StatusMove")
                .Type(PokemonType.Normal)
                .Status(100, 10)
                .Build();

            Assert.That(move.Power, Is.EqualTo(0));
            Assert.That(move.IsStatus, Is.True);
        }

        [Test]
        public void MoveBuilder_ZeroAccuracy_Allowed()
        {
            // Some moves like Swift have 0 accuracy (never miss)
            var move = Move.Define("NeverMiss")
                .Type(PokemonType.Normal)
                .Physical(60, 0, 20)
                .Build();

            Assert.That(move.Accuracy, Is.EqualTo(0));
        }

        [Test]
        public void MoveBuilder_Over100Accuracy_Allowed()
        {
            var move = Move.Define("SuperAccurate")
                .Type(PokemonType.Normal)
                .Physical(50, 200, 10)
                .Build();

            Assert.That(move.Accuracy, Is.EqualTo(200));
        }

        [Test]
        public void MoveBuilder_VeryHighPower_Allowed()
        {
            var move = Move.Define("SuperPower")
                .Type(PokemonType.Normal)
                .Physical(500, 100, 5)
                .Build();

            Assert.That(move.Power, Is.EqualTo(500));
        }

        [Test]
        public void MoveBuilder_ZeroPP_Allowed()
        {
            var move = Move.Define("NoPP")
                .Type(PokemonType.Normal)
                .Physical(50, 100, 0)
                .Build();

            Assert.That(move.MaxPP, Is.EqualTo(0));
        }

        [Test]
        public void MoveBuilder_NegativePriority_Allowed()
        {
            var move = Move.Define("Revenge")
                .Type(PokemonType.Fighting)
                .Physical(60, 100, 10)
                .Priority(-4)
                .Build();

            Assert.That(move.Priority, Is.EqualTo(-4));
        }

        [Test]
        public void MoveBuilder_HighPriority_Allowed()
        {
            var move = Move.Define("QuickAttack")
                .Type(PokemonType.Normal)
                .Physical(40, 100, 30)
                .Priority(1)
                .Build();

            Assert.That(move.Priority, Is.EqualTo(1));
        }

        [Test]
        public void MoveBuilder_AllFlags_CanBeSet()
        {
            var move = Move.Define("AllFlags")
                .Type(PokemonType.Normal)
                .Physical(50, 100, 10)
                .Build();

            move.MakesContact = true;
            move.IsSoundBased = true;
            move.NeverMisses = true;
            move.HighCritRatio = true;
            move.AlwaysCrits = true;
            move.RequiresRecharge = true;
            move.IsTwoTurn = true;
            move.IsPunch = true;
            move.IsBite = true;
            move.IsPulse = true;
            move.IsBullet = true;
            move.BypassesProtect = true;
            move.IsReflectable = true;
            move.IsSnatched = true;
            move.IgnoresTargetStatChanges = true;
            move.IgnoresUserStatChanges = true;

            Assert.That(move.MakesContact, Is.True);
            Assert.That(move.IsSoundBased, Is.True);
            Assert.That(move.NeverMisses, Is.True);
            Assert.That(move.HighCritRatio, Is.True);
            Assert.That(move.AlwaysCrits, Is.True);
            Assert.That(move.RequiresRecharge, Is.True);
            Assert.That(move.IsTwoTurn, Is.True);
            Assert.That(move.IsPunch, Is.True);
            Assert.That(move.IsBite, Is.True);
            Assert.That(move.IsPulse, Is.True);
            Assert.That(move.IsBullet, Is.True);
            Assert.That(move.BypassesProtect, Is.True);
            Assert.That(move.IsReflectable, Is.True);
            Assert.That(move.IsSnatched, Is.True);
            Assert.That(move.IgnoresTargetStatChanges, Is.True);
            Assert.That(move.IgnoresUserStatChanges, Is.True);
        }

        #endregion

        #region PokemonInstanceBuilder Edge Cases

        [Test]
        public void InstanceBuilder_Level0_ThrowsOrHandles()
        {
            // Level 0 is not valid
            Assert.Throws<ArgumentException>(() =>
                PokemonInstanceBuilder.Create(PokemonCatalog.Pikachu, 0).Build());
        }

        [Test]
        public void InstanceBuilder_Level1_Works()
        {
            var pokemon = PokemonInstanceBuilder.Create(PokemonCatalog.Pikachu, 1).Build();
            Assert.That(pokemon.Level, Is.EqualTo(1));
        }

        [Test]
        public void InstanceBuilder_Level100_Works()
        {
            var pokemon = PokemonInstanceBuilder.Create(PokemonCatalog.Pikachu, 100).Build();
            Assert.That(pokemon.Level, Is.EqualTo(100));
        }

        [Test]
        public void InstanceBuilder_Level101_ThrowsOrClamps()
        {
            Assert.Throws<ArgumentException>(() =>
                PokemonInstanceBuilder.Create(PokemonCatalog.Pikachu, 101).Build());
        }

        [Test]
        public void InstanceBuilder_NegativeLevel_Throws()
        {
            Assert.Throws<ArgumentException>(() =>
                PokemonInstanceBuilder.Create(PokemonCatalog.Pikachu, -5).Build());
        }

        [Test]
        public void InstanceBuilder_NullSpecies_Throws()
        {
            Assert.Throws<ArgumentNullException>(() =>
                PokemonInstanceBuilder.Create(null, 50).Build());
        }

        [Test]
        public void InstanceBuilder_FriendshipNegative_Throws()
        {
            Assert.Throws<ArgumentException>(() =>
                PokemonInstanceBuilder.Create(PokemonCatalog.Pikachu, 50)
                    .WithFriendship(-50)
                    .Build());
        }

        [Test]
        public void InstanceBuilder_FriendshipOver255_Throws()
        {
            Assert.Throws<ArgumentException>(() =>
                PokemonInstanceBuilder.Create(PokemonCatalog.Pikachu, 50)
                    .WithFriendship(500)
                    .Build());
        }

        [Test]
        public void InstanceBuilder_FriendshipBoundary_0and255_Work()
        {
            var low = PokemonInstanceBuilder.Create(PokemonCatalog.Pikachu, 50)
                .WithFriendship(0)
                .Build();
            Assert.That(low.Friendship, Is.EqualTo(0));

            var high = PokemonInstanceBuilder.Create(PokemonCatalog.Pikachu, 50)
                .WithFriendship(255)
                .Build();
            Assert.That(high.Friendship, Is.EqualTo(255));
        }

        [Test]
        public void InstanceBuilder_HealthPercentNegative_Throws()
        {
            Assert.Throws<ArgumentException>(() =>
                PokemonInstanceBuilder.Create(PokemonCatalog.Pikachu, 50)
                    .AtHealthPercent(-0.5f)
                    .Build());
        }

        [Test]
        public void InstanceBuilder_HealthPercentOver100_Throws()
        {
            Assert.Throws<ArgumentException>(() =>
                PokemonInstanceBuilder.Create(PokemonCatalog.Pikachu, 50)
                    .AtHealthPercent(2.0f)
                    .Build());
        }

        [Test]
        public void InstanceBuilder_HealthPercentBoundary_0and1_Work()
        {
            var fainted = PokemonInstanceBuilder.Create(PokemonCatalog.Pikachu, 50)
                .AtHealthPercent(0.0f)
                .Build();
            Assert.That(fainted.CurrentHP, Is.EqualTo(0));

            var full = PokemonInstanceBuilder.Create(PokemonCatalog.Pikachu, 50)
                .AtHealthPercent(1.0f)
                .Build();
            Assert.That(full.CurrentHP, Is.EqualTo(full.MaxHP));
        }

        [Test]
        public void InstanceBuilder_MoreThan4Moves_LimitedTo4()
        {
            // Use moves that are actually in Pikachu's learnset
            var pokemon = PokemonInstanceBuilder.Create(PokemonCatalog.Pikachu, 50)
                .WithMoves(
                    MoveCatalog.ThunderShock,  // Starting move
                    MoveCatalog.Growl,         // Starting move
                    MoveCatalog.QuickAttack,   // Level 11
                    MoveCatalog.Thunderbolt,   // Level 26
                    MoveCatalog.Thunder)       // TM - 5th move should be ignored
                .Build();

            Assert.That(pokemon.Moves.Count, Is.EqualTo(4));
        }

        [Test]
        public void InstanceBuilder_InvalidGenderForGenderlessSpecies_Throws()
        {
            // Mewtwo is genderless, trying to set Male should throw
            Assert.Throws<ArgumentException>(() =>
                PokemonInstanceBuilder.Create(PokemonCatalog.Mewtwo, 50)
                    .Male()
                    .Build());
        }

        [Test]
        public void InstanceBuilder_GenderlessSpecies_DefaultsToGenderless()
        {
            var pokemon = PokemonInstanceBuilder.Create(PokemonCatalog.Mewtwo, 50).Build();
            Assert.That(pokemon.Gender, Is.EqualTo(Gender.Genderless));
        }

        [Test]
        public void InstanceBuilder_ChainedBuilderResets()
        {
            // Verify that chained calls properly override previous settings
            var pokemon = PokemonInstanceBuilder.Create(PokemonCatalog.Pikachu, 50)
                .WithNature(Nature.Jolly)
                .WithNature(Nature.Modest)  // Should override
                .Build();

            Assert.That(pokemon.Nature, Is.EqualTo(Nature.Modest));
        }

        [Test]
        public void InstanceBuilder_Shiny_ThenNotShiny()
        {
            var pokemon = PokemonInstanceBuilder.Create(PokemonCatalog.Pikachu, 50)
                .Shiny()
                .NotShiny()  // Should override
                .Build();

            Assert.That(pokemon.IsShiny, Is.False);
        }

        #endregion

        #region EvolutionBuilder Edge Cases

        [Test]
        public void EvolutionBuilder_Level0_Allowed()
        {
            var target = PokemonBuilder.Define("Evo", 2)
                .Type(PokemonType.Normal)
                .Stats(100, 100, 100, 100, 100, 100)
                .Build();

            var pokemon = PokemonBuilder.Define("Base", 1)
                .Type(PokemonType.Normal)
                .Stats(50, 50, 50, 50, 50, 50)
                .EvolvesTo(target, e => e.AtLevel(0))
                .Build();

            Assert.That(pokemon.Evolutions.Count, Is.EqualTo(1));
        }

        [Test]
        public void EvolutionBuilder_Level100_Works()
        {
            var target = PokemonBuilder.Define("Evo", 2)
                .Type(PokemonType.Normal)
                .Stats(100, 100, 100, 100, 100, 100)
                .Build();

            var pokemon = PokemonBuilder.Define("Base", 1)
                .Type(PokemonType.Normal)
                .Stats(50, 50, 50, 50, 50, 50)
                .EvolvesTo(target, e => e.AtLevel(100))
                .Build();

            Assert.That(pokemon.Evolutions[0].GetCondition<Core.Evolution.Conditions.LevelCondition>().MinLevel, Is.EqualTo(100));
        }

        [Test]
        public void EvolutionBuilder_EmptyItemName_Allowed()
        {
            var target = PokemonBuilder.Define("Evo", 2)
                .Type(PokemonType.Normal)
                .Stats(100, 100, 100, 100, 100, 100)
                .Build();

            var pokemon = PokemonBuilder.Define("Base", 1)
                .Type(PokemonType.Normal)
                .Stats(50, 50, 50, 50, 50, 50)
                .EvolvesTo(target, e => e.WithItem(""))
                .Build();

            Assert.That(pokemon.Evolutions.Count, Is.EqualTo(1));
        }

        [Test]
        public void EvolutionBuilder_MultipleSameCondition_Allowed()
        {
            // Two level conditions - weird but allowed
            var target = PokemonBuilder.Define("Evo", 2)
                .Type(PokemonType.Normal)
                .Stats(100, 100, 100, 100, 100, 100)
                .Build();

            var pokemon = PokemonBuilder.Define("Base", 1)
                .Type(PokemonType.Normal)
                .Stats(50, 50, 50, 50, 50, 50)
                .EvolvesTo(target, e => e.AtLevel(16).WithFriendship().AtLevel(20))  // Two levels
                .Build();

            // Should have at least one evolution with conditions
            Assert.That(pokemon.Evolutions.Count, Is.EqualTo(1));
        }

        #endregion

        #region LearnsetBuilder Edge Cases

        [Test]
        public void LearnsetBuilder_Level0Moves_Allowed()
        {
            var pokemon = PokemonBuilder.Define("Test", 1)
                .Type(PokemonType.Normal)
                .Stats(100, 100, 100, 100, 100, 100)
                .Moves(m => m.AtLevel(0, MoveCatalog.Tackle))
                .Build();

            Assert.That(pokemon.Learnset.Count, Is.GreaterThan(0));
        }

        [Test]
        public void LearnsetBuilder_Level100Move_Allowed()
        {
            var pokemon = PokemonBuilder.Define("Test", 1)
                .Type(PokemonType.Normal)
                .Stats(100, 100, 100, 100, 100, 100)
                .Moves(m => m.AtLevel(100, MoveCatalog.Tackle))
                .Build();

            var move = pokemon.GetMovesAtLevel(100).First();
            Assert.That(move.Level, Is.EqualTo(100));
        }

        [Test]
        public void LearnsetBuilder_SameMoveMultipleTimes_Allowed()
        {
            var pokemon = PokemonBuilder.Define("Test", 1)
                .Type(PokemonType.Normal)
                .Stats(100, 100, 100, 100, 100, 100)
                .Moves(m => m
                    .AtLevel(5, MoveCatalog.Tackle)
                    .AtLevel(10, MoveCatalog.Tackle)
                    .AtLevel(15, MoveCatalog.Tackle))
                .Build();

            Assert.That(pokemon.Learnset.Count(l => l.Move == MoveCatalog.Tackle), Is.EqualTo(3));
        }

        [Test]
        public void LearnsetBuilder_ManyMovesAtSameLevel_Allowed()
        {
            var pokemon = PokemonBuilder.Define("Test", 1)
                .Type(PokemonType.Normal)
                .Stats(100, 100, 100, 100, 100, 100)
                .Moves(m => m.AtLevel(10,
                    MoveCatalog.Tackle,
                    MoveCatalog.Growl,
                    MoveCatalog.Scratch,
                    MoveCatalog.Ember,
                    MoveCatalog.WaterGun))
                .Build();

            Assert.That(pokemon.GetMovesAtLevel(10).Count(), Is.EqualTo(5));
        }

        #endregion

        #region EffectBuilder Edge Cases

        [Test]
        public void EffectBuilder_ZeroDamage_Allowed()
        {
            var move = Move.Define("ZeroDamage")
                .Type(PokemonType.Normal)
                .Physical(0, 100, 10)
                .WithEffects(e => e.Damage())
                .Build();

            Assert.That(move.Effects.Count, Is.GreaterThan(0));
        }

        [Test]
        public void EffectBuilder_Over100Chance_Allowed()
        {
            var move = Move.Define("SuperChance")
                .Type(PokemonType.Fire)
                .Special(80, 100, 10)
                .WithEffects(e => e.Damage().MayBurn(200))  // 200% chance
                .Build();

            Assert.That(move.Effects.Count, Is.EqualTo(2));
        }

        [Test]
        public void EffectBuilder_NegativeChance_Allowed()
        {
            // Negative chance would never trigger, but allowed for edge testing
            var move = Move.Define("NeverBurn")
                .Type(PokemonType.Fire)
                .Special(80, 100, 10)
                .WithEffects(e => e.Damage().MayBurn(-10))
                .Build();

            Assert.That(move.Effects.Count, Is.EqualTo(2));
        }

        [Test]
        public void EffectBuilder_MultipleStatChanges_Allowed()
        {
            var move = Move.Define("SuperBoost")
                .Type(PokemonType.Normal)
                .Status(100, 20)
                .WithEffects(e => e
                    .RaiseAttack(1)
                    .RaiseDefense(1)
                    .RaiseSpeed(1)
                    .RaiseSpAttack(1))
                .Build();

            Assert.That(move.Effects.Count, Is.EqualTo(4));
        }

        [Test]
        public void EffectBuilder_ExtremeStatStages_Allowed()
        {
            // +12 stages (beyond normal -6 to +6 range)
            var move = Move.Define("SuperDuper")
                .Type(PokemonType.Normal)
                .Status(100, 5)
                .WithEffects(e => e.RaiseAttack(12))
                .Build();

            Assert.That(move.Effects.Count, Is.EqualTo(1));
        }

        #endregion
    }
}

