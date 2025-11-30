using PokemonUltimate.Core.Effects;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Registry;
using MoveCatalog = PokemonUltimate.Content.Catalogs.Moves.MoveCatalog;

namespace PokemonUltimate.Tests.Catalogs.Moves
{
    /// <summary>
    /// General tests for MoveCatalog: All enumeration, Count, RegisterAll, and global validations.
    /// Type-specific tests are in MoveCatalog[Type]Tests.cs files.
    /// </summary>
    public class MoveCatalogTests
    {
        #region All Enumeration Tests

        [Test]
        public void Test_All_Returns_Correct_Count()
        {
            var allMoves = MoveCatalog.All.ToList();

            Assert.That(allMoves, Has.Count.EqualTo(MoveCatalog.Count));
        }

        [Test]
        public void Test_Count_Is_Positive()
        {
            Assert.That(MoveCatalog.Count, Is.GreaterThan(0));
        }

        [Test]
        public void Test_All_Moves_Have_Unique_Names()
        {
            var allMoves = MoveCatalog.All.ToList();
            var names = allMoves.Select(m => m.Name).ToList();

            Assert.That(names.Distinct().Count(), Is.EqualTo(names.Count), 
                "All moves should have unique names");
        }

        #endregion

        #region Global Validation Tests

        [Test]
        public void Test_All_Moves_Have_Valid_Type()
        {
            foreach (var move in MoveCatalog.All)
            {
                Assert.That(Enum.IsDefined(typeof(PokemonType), move.Type), Is.True,
                    $"{move.Name} should have a valid Type");
            }
        }

        [Test]
        public void Test_All_Moves_Have_Valid_Category()
        {
            foreach (var move in MoveCatalog.All)
            {
                Assert.That(Enum.IsDefined(typeof(MoveCategory), move.Category), Is.True,
                    $"{move.Name} should have a valid Category");
            }
        }

        [Test]
        public void Test_All_Damaging_Moves_Have_Power_Greater_Than_Zero()
        {
            var damagingMoves = MoveCatalog.All
                .Where(m => m.Category != MoveCategory.Status)
                .ToList();

            foreach (var move in damagingMoves)
            {
                Assert.That(move.Power, Is.GreaterThan(0),
                    $"{move.Name} is a damaging move and should have Power > 0");
            }
        }

        [Test]
        public void Test_All_Status_Moves_Have_Zero_Power()
        {
            var statusMoves = MoveCatalog.All
                .Where(m => m.Category == MoveCategory.Status)
                .ToList();

            foreach (var move in statusMoves)
            {
                Assert.That(move.Power, Is.EqualTo(0),
                    $"{move.Name} is a Status move and should have Power = 0");
            }
        }

        [Test]
        public void Test_All_Moves_Have_Valid_Accuracy()
        {
            foreach (var move in MoveCatalog.All)
            {
                Assert.That(move.Accuracy, Is.InRange(0, 100),
                    $"{move.Name} should have Accuracy between 0 and 100");
            }
        }

        [Test]
        public void Test_All_Moves_Have_Positive_MaxPP()
        {
            foreach (var move in MoveCatalog.All)
            {
                Assert.That(move.MaxPP, Is.GreaterThan(0),
                    $"{move.Name} should have MaxPP > 0");
            }
        }

        [Test]
        public void Test_All_Damaging_Moves_Have_DamageEffect()
        {
            var damagingMoves = MoveCatalog.All
                .Where(m => m.Category != MoveCategory.Status && m.Power > 0)
                .ToList();

            foreach (var move in damagingMoves)
            {
                Assert.That(move.HasEffect<DamageEffect>(), Is.True,
                    $"{move.Name} should have DamageEffect");
            }
        }

        [Test]
        public void Test_All_Moves_Have_At_Least_One_Effect()
        {
            foreach (var move in MoveCatalog.All)
            {
                Assert.That(move.Effects.Count, Is.GreaterThan(0),
                    $"{move.Name} should have at least one effect");
            }
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
        public void Test_RegisterAll_All_Moves_Retrievable_By_Name()
        {
            var registry = new MoveRegistry();
            MoveCatalog.RegisterAll(registry);

            foreach (var move in MoveCatalog.All)
            {
                var retrieved = registry.GetByName(move.Name);
                Assert.That(retrieved, Is.SameAs(move),
                    $"{move.Name} should be retrievable by name");
            }
        }

        [Test]
        public void Test_RegisterAll_Moves_Filterable_By_Type()
        {
            var registry = new MoveRegistry();
            MoveCatalog.RegisterAll(registry);

            var types = MoveCatalog.All.Select(m => m.Type).Distinct();
            
            foreach (var type in types)
            {
                var catalogCount = MoveCatalog.All.Count(m => m.Type == type);
                var registryCount = registry.GetByType(type).Count();
                
                Assert.That(registryCount, Is.EqualTo(catalogCount),
                    $"Registry should have same {type} moves as catalog");
            }
        }

        #endregion
    }
}

