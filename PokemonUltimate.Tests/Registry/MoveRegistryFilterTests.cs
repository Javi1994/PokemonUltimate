using System.Linq;
using NUnit.Framework;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Registry;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Tests.Registry
{
    /// <summary>
    /// Tests for MoveRegistry filtering operations: GetByType, GetByCategory
    /// </summary>
    [TestFixture]
    public class MoveRegistryFilterTests
    {
        private MoveRegistry _registry;

        [SetUp]
        public void Setup()
        {
            _registry = new MoveRegistry();
            RegisterTestMoves();
        }

        private void RegisterTestMoves()
        {
            // Fire moves
            _registry.Register(CreateMove("Flamethrower", PokemonType.Fire, MoveCategory.Special, 90, 100));
            _registry.Register(CreateMove("Fire Punch", PokemonType.Fire, MoveCategory.Physical, 75, 100));
            _registry.Register(CreateMove("Will-O-Wisp", PokemonType.Fire, MoveCategory.Status, 0, 85));

            // Electric moves
            _registry.Register(CreateMove("Thunderbolt", PokemonType.Electric, MoveCategory.Special, 90, 100));
            _registry.Register(CreateMove("Thunder Punch", PokemonType.Electric, MoveCategory.Physical, 75, 100));
            _registry.Register(CreateMove("Thunder Wave", PokemonType.Electric, MoveCategory.Status, 0, 90));

            // Water moves
            _registry.Register(CreateMove("Surf", PokemonType.Water, MoveCategory.Special, 90, 100));
            _registry.Register(CreateMove("Waterfall", PokemonType.Water, MoveCategory.Physical, 80, 100));

            // Normal moves
            _registry.Register(CreateMove("Tackle", PokemonType.Normal, MoveCategory.Physical, 40, 100));
            _registry.Register(CreateMove("Growl", PokemonType.Normal, MoveCategory.Status, 0, 100));
        }

        #region GetByType Tests

        [Test]
        public void Test_GetByType_Returns_All_Fire_Moves()
        {
            var fireMoves = _registry.GetByType(PokemonType.Fire).ToList();

            Assert.Multiple(() =>
            {
                Assert.That(fireMoves, Has.Count.EqualTo(3));
                Assert.That(fireMoves.All(m => m.Type == PokemonType.Fire), Is.True);
                Assert.That(fireMoves.Select(m => m.Name), Does.Contain("Flamethrower"));
                Assert.That(fireMoves.Select(m => m.Name), Does.Contain("Fire Punch"));
                Assert.That(fireMoves.Select(m => m.Name), Does.Contain("Will-O-Wisp"));
            });
        }

        [Test]
        public void Test_GetByType_Returns_All_Electric_Moves()
        {
            var electricMoves = _registry.GetByType(PokemonType.Electric).ToList();

            Assert.Multiple(() =>
            {
                Assert.That(electricMoves, Has.Count.EqualTo(3));
                Assert.That(electricMoves.All(m => m.Type == PokemonType.Electric), Is.True);
            });
        }

        [Test]
        public void Test_GetByType_Returns_Empty_For_Type_With_No_Moves()
        {
            var iceMoves = _registry.GetByType(PokemonType.Ice).ToList();

            Assert.That(iceMoves, Is.Empty);
        }

        [Test]
        public void Test_GetByType_Returns_Correct_Count_For_Water()
        {
            var waterMoves = _registry.GetByType(PokemonType.Water).ToList();

            Assert.That(waterMoves, Has.Count.EqualTo(2));
        }

        #endregion

        #region GetByCategory Tests

        [Test]
        public void Test_GetByCategory_Returns_All_Physical_Moves()
        {
            var physicalMoves = _registry.GetByCategory(MoveCategory.Physical).ToList();

            Assert.Multiple(() =>
            {
                Assert.That(physicalMoves, Has.Count.EqualTo(4)); // Fire Punch, Thunder Punch, Waterfall, Tackle
                Assert.That(physicalMoves.All(m => m.Category == MoveCategory.Physical), Is.True);
            });
        }

        [Test]
        public void Test_GetByCategory_Returns_All_Special_Moves()
        {
            var specialMoves = _registry.GetByCategory(MoveCategory.Special).ToList();

            Assert.Multiple(() =>
            {
                Assert.That(specialMoves, Has.Count.EqualTo(3)); // Flamethrower, Thunderbolt, Surf
                Assert.That(specialMoves.All(m => m.Category == MoveCategory.Special), Is.True);
            });
        }

        [Test]
        public void Test_GetByCategory_Returns_All_Status_Moves()
        {
            var statusMoves = _registry.GetByCategory(MoveCategory.Status).ToList();

            Assert.Multiple(() =>
            {
                Assert.That(statusMoves, Has.Count.EqualTo(3)); // Will-O-Wisp, Thunder Wave, Growl
                Assert.That(statusMoves.All(m => m.Category == MoveCategory.Status), Is.True);
                Assert.That(statusMoves.All(m => m.Power == 0), Is.True, "Status moves should have 0 power");
            });
        }

        #endregion

        #region Combined Filters (Manual)

        [Test]
        public void Test_Can_Chain_Filters_Manually()
        {
            // Get all Fire Special moves
            var fireSpecialMoves = _registry
                .GetByType(PokemonType.Fire)
                .Where(m => m.Category == MoveCategory.Special)
                .ToList();

            Assert.Multiple(() =>
            {
                Assert.That(fireSpecialMoves, Has.Count.EqualTo(1));
                Assert.That(fireSpecialMoves[0].Name, Is.EqualTo("Flamethrower"));
            });
        }

        [Test]
        public void Test_Can_Filter_By_Power_Range()
        {
            // Get all moves with power >= 90
            var strongMoves = _registry
                .GetAll()
                .Where(m => m.Power >= 90)
                .ToList();

            Assert.Multiple(() =>
            {
                Assert.That(strongMoves, Has.Count.EqualTo(3)); // Flamethrower, Thunderbolt, Surf
                Assert.That(strongMoves.All(m => m.Power >= 90), Is.True);
            });
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

