using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Registry;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Tests.Systems.Core.Registry
{
    /// <summary>
    /// Tests for basic MoveRegistry operations: Register, Retrieve by Name, Exists, GetAll
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Pokemon Data
    /// **Sub-Feature**: 1.4: Registry System
    /// **Documentation**: See `docs/features/1-pokemon-data/architecture.md`
    /// </remarks>
    [TestFixture]
    public class MoveRegistryTests
    {
        private MoveRegistry _registry;

        [SetUp]
        public void Setup()
        {
            _registry = new MoveRegistry();
        }

        #region Register and Retrieve by Name

        [Test]
        public void Test_Register_And_Retrieve_By_Name()
        {
            var thunderbolt = CreateMove("Thunderbolt", PokemonType.Electric, MoveCategory.Special, 90, 100);
            _registry.Register(thunderbolt);

            Assert.Multiple(() =>
            {
                Assert.That(_registry.Exists("Thunderbolt"), Is.True);
                Assert.That(_registry.GetByName("Thunderbolt"), Is.EqualTo(thunderbolt));
            });
        }

        [Test]
        public void Test_GetByName_NonExistent_Throws()
        {
            Assert.Throws<KeyNotFoundException>(() => _registry.GetByName("NonExistentMove"));
        }

        [Test]
        public void Test_Name_Is_Case_Sensitive()
        {
            var flamethrower = CreateMove("Flamethrower", PokemonType.Fire, MoveCategory.Special, 90, 100);
            _registry.Register(flamethrower);

            Assert.Multiple(() =>
            {
                Assert.That(_registry.Exists("Flamethrower"), Is.True);
                Assert.That(_registry.Exists("flamethrower"), Is.False);
                Assert.That(_registry.Exists("FLAMETHROWER"), Is.False);
            });
        }

        #endregion

        #region Multiple Moves

        [Test]
        public void Test_Register_Multiple_Moves()
        {
            var tackle = CreateMove("Tackle", PokemonType.Normal, MoveCategory.Physical, 40, 100);
            var ember = CreateMove("Ember", PokemonType.Fire, MoveCategory.Special, 40, 100);
            var thunderWave = CreateMove("Thunder Wave", PokemonType.Electric, MoveCategory.Status, 0, 90);

            _registry.Register(tackle);
            _registry.Register(ember);
            _registry.Register(thunderWave);

            Assert.Multiple(() =>
            {
                Assert.That(_registry.GetByName("Tackle"), Is.EqualTo(tackle));
                Assert.That(_registry.GetByName("Ember"), Is.EqualTo(ember));
                Assert.That(_registry.GetByName("Thunder Wave"), Is.EqualTo(thunderWave));
            });
        }

        [Test]
        public void Test_GetAll_Returns_All_Registered_Moves()
        {
            var tackle = CreateMove("Tackle", PokemonType.Normal, MoveCategory.Physical, 40, 100);
            var ember = CreateMove("Ember", PokemonType.Fire, MoveCategory.Special, 40, 100);

            _registry.Register(tackle);
            _registry.Register(ember);

            var allMoves = _registry.GetAll().ToList();

            Assert.Multiple(() =>
            {
                Assert.That(allMoves, Has.Count.EqualTo(2));
                Assert.That(allMoves, Does.Contain(tackle));
                Assert.That(allMoves, Does.Contain(ember));
            });
        }

        [Test]
        public void Test_GetAll_Returns_Empty_When_No_Moves_Registered()
        {
            var allMoves = _registry.GetAll().ToList();

            Assert.That(allMoves, Is.Empty);
        }

        #endregion

        #region Edge Cases

        [Test]
        public void Test_Overwrite_Duplicate_Name()
        {
            var tackle1 = CreateMove("Tackle", PokemonType.Normal, MoveCategory.Physical, 40, 100);
            var tackle2 = CreateMove("Tackle", PokemonType.Normal, MoveCategory.Physical, 50, 100); // Different power

            _registry.Register(tackle1);
            _registry.Register(tackle2);

            Assert.Multiple(() =>
            {
                Assert.That(_registry.GetByName("Tackle"), Is.EqualTo(tackle2));
                Assert.That(_registry.GetByName("Tackle").Power, Is.EqualTo(50));
            });
        }

        [Test]
        public void Test_Register_Null_Throws()
        {
            Assert.Throws<NullReferenceException>(() => _registry.Register(null!));
        }

        [Test]
        public void Test_Id_Property_Returns_Name()
        {
            var move = CreateMove("Earthquake", PokemonType.Ground, MoveCategory.Physical, 100, 100);

            Assert.That(move.Id, Is.EqualTo("Earthquake"));
        }

        #endregion

        #region Helper Methods

        private static MoveData CreateMove(string name, PokemonType type, MoveCategory category, int power, int accuracy)
        {
            return new MoveData
            {
                Name = name,
                Type = type,
                Category = category,
                Power = power,
                Accuracy = accuracy,
                MaxPP = 15,
                Priority = 0,
                TargetScope = TargetScope.SingleEnemy
            };
        }

        #endregion
    }
}

