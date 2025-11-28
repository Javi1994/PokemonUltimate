using PokemonUltimate.Core.Catalogs;
using PokemonUltimate.Core.Data;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Tests.Catalogs
{
    // Tests for MoveCatalog: static access, All enumeration, and RegisterAll
    public class MoveCatalogTests
    {
        #region Direct Access Tests

        [Test]
        public void Test_Thunderbolt_Has_Correct_Data()
        {
            var thunderbolt = MoveCatalog.Thunderbolt;

            Assert.Multiple(() =>
            {
                Assert.That(thunderbolt.Name, Is.EqualTo("Thunderbolt"));
                Assert.That(thunderbolt.Type, Is.EqualTo(PokemonType.Electric));
                Assert.That(thunderbolt.Category, Is.EqualTo(MoveCategory.Special));
                Assert.That(thunderbolt.Power, Is.EqualTo(90));
                Assert.That(thunderbolt.Accuracy, Is.EqualTo(100));
                Assert.That(thunderbolt.MaxPP, Is.EqualTo(15));
            });
        }

        [Test]
        public void Test_Priority_Move_QuickAttack()
        {
            var quickAttack = MoveCatalog.QuickAttack;

            Assert.Multiple(() =>
            {
                Assert.That(quickAttack.Name, Is.EqualTo("Quick Attack"));
                Assert.That(quickAttack.Priority, Is.EqualTo(1));
                Assert.That(quickAttack.Category, Is.EqualTo(MoveCategory.Physical));
            });
        }

        [Test]
        public void Test_Status_Move_ThunderWave()
        {
            var thunderWave = MoveCatalog.ThunderWave;

            Assert.Multiple(() =>
            {
                Assert.That(thunderWave.Category, Is.EqualTo(MoveCategory.Status));
                Assert.That(thunderWave.Power, Is.EqualTo(0));
                Assert.That(thunderWave.Accuracy, Is.EqualTo(90));
            });
        }

        [Test]
        public void Test_AoE_Move_Earthquake()
        {
            var earthquake = MoveCatalog.Earthquake;

            Assert.Multiple(() =>
            {
                Assert.That(earthquake.TargetScope, Is.EqualTo(TargetScope.AllOthers));
                Assert.That(earthquake.Power, Is.EqualTo(100));
                Assert.That(earthquake.Type, Is.EqualTo(PokemonType.Ground));
            });
        }

        [Test]
        public void Test_AoE_Move_Surf()
        {
            var surf = MoveCatalog.Surf;

            Assert.That(surf.TargetScope, Is.EqualTo(TargetScope.AllEnemies));
        }

        #endregion

        #region Type Coverage Tests

        [Test]
        public void Test_Fire_Moves_Exist()
        {
            Assert.Multiple(() =>
            {
                Assert.That(MoveCatalog.Ember.Type, Is.EqualTo(PokemonType.Fire));
                Assert.That(MoveCatalog.Flamethrower.Type, Is.EqualTo(PokemonType.Fire));
                Assert.That(MoveCatalog.FireBlast.Type, Is.EqualTo(PokemonType.Fire));
            });
        }

        [Test]
        public void Test_Water_Moves_Exist()
        {
            Assert.Multiple(() =>
            {
                Assert.That(MoveCatalog.WaterGun.Type, Is.EqualTo(PokemonType.Water));
                Assert.That(MoveCatalog.Surf.Type, Is.EqualTo(PokemonType.Water));
                Assert.That(MoveCatalog.HydroPump.Type, Is.EqualTo(PokemonType.Water));
            });
        }

        [Test]
        public void Test_Grass_Moves_Exist()
        {
            Assert.Multiple(() =>
            {
                Assert.That(MoveCatalog.VineWhip.Type, Is.EqualTo(PokemonType.Grass));
                Assert.That(MoveCatalog.RazorLeaf.Type, Is.EqualTo(PokemonType.Grass));
                Assert.That(MoveCatalog.SolarBeam.Type, Is.EqualTo(PokemonType.Grass));
            });
        }

        [Test]
        public void Test_Electric_Moves_Exist()
        {
            Assert.Multiple(() =>
            {
                Assert.That(MoveCatalog.ThunderShock.Type, Is.EqualTo(PokemonType.Electric));
                Assert.That(MoveCatalog.Thunderbolt.Type, Is.EqualTo(PokemonType.Electric));
                Assert.That(MoveCatalog.Thunder.Type, Is.EqualTo(PokemonType.Electric));
            });
        }

        #endregion

        #region All Enumeration Tests

        [Test]
        public void Test_All_Returns_Correct_Count()
        {
            var allMoves = MoveCatalog.All.ToList();

            Assert.That(allMoves, Has.Count.EqualTo(MoveCatalog.Count));
        }

        [Test]
        public void Test_All_Moves_Have_Unique_Names()
        {
            var allMoves = MoveCatalog.All.ToList();
            var names = allMoves.Select(m => m.Name).ToList();

            Assert.That(names.Distinct().Count(), Is.EqualTo(names.Count), "All moves should have unique names");
        }

        [Test]
        public void Test_All_Damaging_Moves_Have_Power_Greater_Than_Zero()
        {
            var damagingMoves = MoveCatalog.All
                .Where(m => m.Category != MoveCategory.Status)
                .ToList();

            Assert.That(damagingMoves.All(m => m.Power > 0), Is.True, "All damaging moves should have Power > 0");
        }

        [Test]
        public void Test_All_Status_Moves_Have_Zero_Power()
        {
            var statusMoves = MoveCatalog.All
                .Where(m => m.Category == MoveCategory.Status)
                .ToList();

            Assert.That(statusMoves.All(m => m.Power == 0), Is.True, "All status moves should have Power = 0");
        }

        #endregion

        #region RegisterAll Tests

        [Test]
        public void Test_RegisterAll_Populates_Registry()
        {
            var registry = new MoveRegistry();

            MoveCatalog.RegisterAll(registry);

            Assert.That(registry.GetAll().Count(), Is.EqualTo(MoveCatalog.Count));
        }

        [Test]
        public void Test_RegisterAll_Moves_Are_Retrievable_By_Name()
        {
            var registry = new MoveRegistry();
            MoveCatalog.RegisterAll(registry);

            Assert.Multiple(() =>
            {
                Assert.That(registry.GetByName("Thunderbolt"), Is.EqualTo(MoveCatalog.Thunderbolt));
                Assert.That(registry.GetByName("Flamethrower"), Is.EqualTo(MoveCatalog.Flamethrower));
                Assert.That(registry.GetByName("Earthquake"), Is.EqualTo(MoveCatalog.Earthquake));
            });
        }

        [Test]
        public void Test_RegisterAll_Moves_Filterable_By_Type()
        {
            var registry = new MoveRegistry();
            MoveCatalog.RegisterAll(registry);

            var fireMoves = registry.GetByType(PokemonType.Fire).ToList();

            Assert.Multiple(() =>
            {
                Assert.That(fireMoves, Has.Count.EqualTo(3));
                Assert.That(fireMoves, Does.Contain(MoveCatalog.Ember));
                Assert.That(fireMoves, Does.Contain(MoveCatalog.Flamethrower));
                Assert.That(fireMoves, Does.Contain(MoveCatalog.FireBlast));
            });
        }

        #endregion
    }
}

