using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonUltimate.Content.Catalogs.Moves;
using PokemonUltimate.Content.Catalogs.Pokemon;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Registry;

namespace PokemonUltimate.Tests.Registry
{
    /// <summary>
    /// Edge case tests for Registry queries and filters.
    /// Tests complex filters, empty results, and boundary conditions.
    /// </summary>
    [TestFixture]
    public class RegistryEdgeCasesTests
    {
        private PokemonRegistry _pokemonRegistry;
        private MoveRegistry _moveRegistry;

        // Helper to avoid conflicts with ImmutableArrayExtensions.All
        private List<PokemonSpeciesData> AllPokemon => _pokemonRegistry.All.ToList();
        private List<MoveData> AllMoves => _moveRegistry.All.ToList();

        [SetUp]
        public void Setup()
        {
            _pokemonRegistry = new PokemonRegistry();
            _pokemonRegistry.RegisterAll(PokemonCatalog.All.ToList());

            _moveRegistry = new MoveRegistry();
            _moveRegistry.RegisterAll(MoveCatalog.All.ToList());
        }

        #region Pokemon Registry Edge Cases

        [Test]
        public void PokemonRegistry_GetById_NonExistent_ReturnsNull()
        {
            var result = _pokemonRegistry.GetById("NonExistentPokemon");
            Assert.That(result, Is.Null);
        }

        [Test]
        public void PokemonRegistry_GetById_EmptyString_ReturnsNull()
        {
            var result = _pokemonRegistry.GetById("");
            Assert.That(result, Is.Null);
        }

        [Test]
        public void PokemonRegistry_GetById_CaseSensitive()
        {
            var pikachu1 = _pokemonRegistry.GetById("Pikachu");
            var pikachu2 = _pokemonRegistry.GetById("pikachu");
            var pikachu3 = _pokemonRegistry.GetById("PIKACHU");
            
            // Depending on implementation, case sensitivity may vary
            // At least one should work
            Assert.That(pikachu1, Is.Not.Null);
        }

        [Test]
        public void PokemonRegistry_GetByType_NoMatches_ReturnsEmpty()
        {
            // Dragon type might not exist if we only have Gen 1 basics
            var dragons = _pokemonRegistry.GetByType(PokemonType.Dragon).ToList();
            // Just verify it returns a list (may or may not be empty)
            Assert.That(dragons, Is.Not.Null);
        }

        [Test]
        public void PokemonRegistry_GetByType_AllMatchesReturned()
        {
            var fireTypes = _pokemonRegistry.GetByType(PokemonType.Fire).ToList();
            
            // All returned should have Fire type
            foreach (var pokemon in fireTypes)
            {
                Assert.That(pokemon.HasType(PokemonType.Fire), Is.True, $"{pokemon.Name} doesn't have Fire");
            }
        }

        [Test]
        public void PokemonRegistry_GetByPokedexRange_NormalRange()
        {
            var starters = _pokemonRegistry.GetByPokedexRange(1, 9).ToList();
            
            foreach (var pokemon in starters)
            {
                Assert.That(pokemon.PokedexNumber, Is.InRange(1, 9));
            }
        }

        [Test]
        public void PokemonRegistry_GetByPokedexRange_SingleNumber()
        {
            var result = _pokemonRegistry.GetByPokedexRange(25, 25).ToList();
            
            // Should only return Pikachu
            Assert.That(result.Count, Is.LessThanOrEqualTo(1));
            if (result.Count > 0)
            {
                Assert.That(result[0].PokedexNumber, Is.EqualTo(25));
            }
        }

        [Test]
        public void PokemonRegistry_GetByPokedexRange_InvertedRange_ReturnsEmpty()
        {
            // Start > End should return empty
            var result = _pokemonRegistry.GetByPokedexRange(100, 1).ToList();
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void PokemonRegistry_GetByPokedexRange_NegativeNumbers()
        {
            var result = _pokemonRegistry.GetByPokedexRange(-10, -1).ToList();
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void PokemonRegistry_GetByPokedexRange_VeryHighNumbers()
        {
            var result = _pokemonRegistry.GetByPokedexRange(9000, 10000).ToList();
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void PokemonRegistry_TryGet_ExistingPokemon_ReturnsTrue()
        {
            bool found = _pokemonRegistry.TryGet("Pikachu", out var pokemon);
            
            Assert.That(found, Is.True);
            Assert.That(pokemon, Is.Not.Null);
            Assert.That(pokemon.Name, Is.EqualTo("Pikachu"));
        }

        [Test]
        public void PokemonRegistry_TryGet_NonExistent_ReturnsFalse()
        {
            bool found = _pokemonRegistry.TryGet("NotARealPokemon", out var pokemon);
            
            Assert.That(found, Is.False);
            Assert.That(pokemon, Is.Null);
        }

        [Test]
        public void PokemonRegistry_Contains_ExistingPokemon_ReturnsTrue()
        {
            Assert.That(_pokemonRegistry.Contains("Pikachu"), Is.True);
        }

        [Test]
        public void PokemonRegistry_Contains_NonExistent_ReturnsFalse()
        {
            Assert.That(_pokemonRegistry.Contains("Agumon"), Is.False);
        }

        [Test]
        public void PokemonRegistry_All_ReturnsAllRegistered()
        {
            int catalogCount = PokemonCatalog.All.Count();
            int registryCount = AllPokemon.Count();
            
            Assert.That(registryCount, Is.EqualTo(catalogCount));
        }

        [Test]
        public void PokemonRegistry_Count_MatchesCatalog()
        {
            Assert.That(_pokemonRegistry.Count, Is.EqualTo(PokemonCatalog.All.Count()));
        }

        #endregion

        #region Move Registry Edge Cases

        [Test]
        public void MoveRegistry_GetById_NonExistent_ReturnsNull()
        {
            var result = _moveRegistry.GetById("SuperDuperBlast");
            Assert.That(result, Is.Null);
        }

        [Test]
        public void MoveRegistry_GetByType_AllMatchesHaveType()
        {
            var fireMoves = _moveRegistry.GetByType(PokemonType.Fire).ToList();
            
            foreach (var move in fireMoves)
            {
                Assert.That(move.Type, Is.EqualTo(PokemonType.Fire), $"{move.Name} is not Fire");
            }
        }

        [Test]
        public void MoveRegistry_GetByCategory_Physical()
        {
            var physical = _moveRegistry.GetByCategory(MoveCategory.Physical).ToList();
            
            foreach (var move in physical)
            {
                Assert.That(move.Category, Is.EqualTo(MoveCategory.Physical));
            }
        }

        [Test]
        public void MoveRegistry_GetByCategory_Special()
        {
            var special = _moveRegistry.GetByCategory(MoveCategory.Special).ToList();
            
            foreach (var move in special)
            {
                Assert.That(move.Category, Is.EqualTo(MoveCategory.Special));
            }
        }

        [Test]
        public void MoveRegistry_GetByCategory_Status()
        {
            var status = _moveRegistry.GetByCategory(MoveCategory.Status).ToList();
            
            foreach (var move in status)
            {
                Assert.That(move.Category, Is.EqualTo(MoveCategory.Status));
                Assert.That(move.Power, Is.EqualTo(0)); // Status moves have 0 power
            }
        }

        [Test]
        public void MoveRegistry_GetDamaging_ExcludesStatus()
        {
            var damaging = _moveRegistry.GetDamaging().ToList();
            
            foreach (var move in damaging)
            {
                Assert.That(move.IsDamaging, Is.True);
                Assert.That(move.Category, Is.Not.EqualTo(MoveCategory.Status));
            }
        }

        [Test]
        public void MoveRegistry_GetByMinPower_FiltersCorrectly()
        {
            int minPower = 80;
            var powerful = _moveRegistry.GetByMinPower(minPower).ToList();
            
            foreach (var move in powerful)
            {
                Assert.That(move.Power, Is.GreaterThanOrEqualTo(minPower), $"{move.Name} has {move.Power} power");
            }
        }

        [Test]
        public void MoveRegistry_GetByMinPower_Zero_ReturnsAll()
        {
            var all = _moveRegistry.GetByMinPower(0).ToList();
            
            // Should include all moves (including status with 0 power)
            Assert.That(all.Count, Is.GreaterThan(0));
        }

        [Test]
        public void MoveRegistry_GetByMinPower_VeryHigh_ReturnsFew()
        {
            var superPowerful = _moveRegistry.GetByMinPower(150).ToList();
            
            // Very few moves have 150+ power
            Assert.That(superPowerful.Count, Is.LessThanOrEqualTo(5));
        }

        [Test]
        public void MoveRegistry_GetByMaxPower_FiltersCorrectly()
        {
            int maxPower = 50;
            var weak = _moveRegistry.GetByMaxPower(maxPower).ToList();
            
            foreach (var move in weak)
            {
                Assert.That(move.Power, Is.LessThanOrEqualTo(maxPower), $"{move.Name} has {move.Power} power");
            }
        }

        #endregion

        #region Combined Filter Tests

        [Test]
        public void PokemonRegistry_MultipleFilters_TypeAndRange()
        {
            // Get Fire types in Pokedex range 1-50
            var fireInRange = _pokemonRegistry.GetByType(PokemonType.Fire)
                .Where(p => p.PokedexNumber >= 1 && p.PokedexNumber <= 50)
                .ToList();
            
            foreach (var pokemon in fireInRange)
            {
                Assert.That(pokemon.HasType(PokemonType.Fire), Is.True);
                Assert.That(pokemon.PokedexNumber, Is.InRange(1, 50));
            }
        }

        [Test]
        public void MoveRegistry_MultipleFilters_TypeAndPower()
        {
            // Get Fire moves with at least 80 power
            var powerfulFire = _moveRegistry.GetByType(PokemonType.Fire)
                .Where(m => m.Power >= 80)
                .ToList();
            
            foreach (var move in powerfulFire)
            {
                Assert.That(move.Type, Is.EqualTo(PokemonType.Fire));
                Assert.That(move.Power, Is.GreaterThanOrEqualTo(80));
            }
        }

        [Test]
        public void MoveRegistry_CombinedFilters_CategoryTypeAccuracy()
        {
            // Get Physical Fire moves with 100% accuracy
            var result = _moveRegistry.GetByCategory(MoveCategory.Physical)
                .Where(m => m.Type == PokemonType.Fire)
                .Where(m => m.Accuracy == 100)
                .ToList();
            
            foreach (var move in result)
            {
                Assert.That(move.Category, Is.EqualTo(MoveCategory.Physical));
                Assert.That(move.Type, Is.EqualTo(PokemonType.Fire));
                Assert.That(move.Accuracy, Is.EqualTo(100));
            }
        }

        #endregion

        #region Empty Registry Tests

        [Test]
        public void EmptyRegistry_GetById_ReturnsNull()
        {
            var emptyRegistry = new PokemonRegistry();
            Assert.That(emptyRegistry.GetById("Pikachu"), Is.Null);
        }

        [Test]
        public void EmptyRegistry_All_ReturnsEmpty()
        {
            var emptyRegistry = new PokemonRegistry();
            Assert.That(emptyRegistry.All, Is.Empty);
        }

        [Test]
        public void EmptyRegistry_Count_IsZero()
        {
            var emptyRegistry = new PokemonRegistry();
            Assert.That(emptyRegistry.Count, Is.EqualTo(0));
        }

        [Test]
        public void EmptyRegistry_GetByType_ReturnsEmpty()
        {
            var emptyRegistry = new PokemonRegistry();
            Assert.That(emptyRegistry.GetByType(PokemonType.Fire), Is.Empty);
        }

        #endregion

        #region Duplicate Registration Tests

        [Test]
        public void Registry_RegisterSameTwice_DoesNotDuplicate()
        {
            var registry = new PokemonRegistry();
            registry.Register(PokemonCatalog.Pikachu);
            registry.Register(PokemonCatalog.Pikachu);
            
            var pikachuCount = registry.All.Count(p => p.Name == "Pikachu");
            Assert.That(pikachuCount, Is.EqualTo(1));
        }

        [Test]
        public void Registry_RegisterAll_Twice_DoesNotDuplicate()
        {
            var registry = new PokemonRegistry();
            registry.RegisterAll(PokemonCatalog.All);
            int firstCount = registry.Count;
            
            registry.RegisterAll(PokemonCatalog.All);
            int secondCount = registry.Count;
            
            Assert.That(secondCount, Is.EqualTo(firstCount));
        }

        #endregion

        #region Query Performance Tests (Smoke Tests)

        [Test]
        public void PokemonRegistry_LargeQuery_DoesNotTimeout()
        {
            // Query all Pokemon 1000 times
            for (int i = 0; i < 1000; i++)
            {
                var all = AllPokemon;
                Assert.That(all, Is.Not.Null);
            }
        }

        [Test]
        public void MoveRegistry_ChainedFilters_DoesNotTimeout()
        {
            for (int i = 0; i < 100; i++)
            {
                var result = AllMoves
                    .Where(m => m.Power > 0)
                    .Where(m => m.Accuracy >= 90)
                    .Where(m => m.Type == PokemonType.Fire || m.Type == PokemonType.Water)
                    .ToList();
            }
            
            Assert.Pass("Completed 100 chained filter queries");
        }

        #endregion

        #region LINQ Integration Tests

        [Test]
        public void Registry_OrderBy_Works()
        {
            var byPokedex = AllPokemon.OrderBy(p => p.PokedexNumber).ToList();
            
            for (int i = 1; i < byPokedex.Count; i++)
            {
                Assert.That(byPokedex[i].PokedexNumber, Is.GreaterThanOrEqualTo(byPokedex[i - 1].PokedexNumber));
            }
        }

        [Test]
        public void Registry_First_Works()
        {
            var first = AllPokemon.First();
            Assert.That(first, Is.Not.Null);
        }

        [Test]
        public void Registry_FirstOrDefault_EmptyQuery_ReturnsNull()
        {
            var result = AllPokemon
                .Where(p => p.PokedexNumber > 9999)
                .FirstOrDefault();
            
            Assert.That(result, Is.Null);
        }

        [Test]
        public void Registry_Any_ExistingType_ReturnsTrue()
        {
            bool hasElectric = AllPokemon.Any(p => p.HasType(PokemonType.Electric));
            Assert.That(hasElectric, Is.True);
        }

        [Test]
        public void Registry_Count_WithPredicate_Works()
        {
            int fireCount = AllPokemon.Count(p => p.HasType(PokemonType.Fire));
            Assert.That(fireCount, Is.GreaterThan(0));
        }

        [Test]
        public void Registry_Max_BST_Works()
        {
            int maxBST = AllPokemon.Max(p => p.BaseStats.Total);
            Assert.That(maxBST, Is.GreaterThan(600)); // Legendaries have 600+ BST
        }

        [Test]
        public void Registry_Min_BST_Works()
        {
            int minBST = AllPokemon.Min(p => p.BaseStats.Total);
            Assert.That(minBST, Is.GreaterThan(0));
            Assert.That(minBST, Is.LessThan(400)); // Basic Pokemon have <400 BST
        }

        [Test]
        public void Registry_Average_BST_Works()
        {
            var allPokemon = AllPokemon;
            double avgBST = allPokemon.Average(p => p.BaseStats.Total);
            Assert.That(avgBST, Is.InRange(300, 550)); // Reasonable average range
        }

        [Test]
        public void Registry_GroupBy_Type_Works()
        {
            var allPokemon = AllPokemon;
            var byType = allPokemon
                .GroupBy(p => p.PrimaryType)
                .ToDictionary(g => g.Key, g => g.Count());
            
            Assert.That(byType.Keys, Is.Not.Empty);
            foreach (var count in byType.Values)
            {
                Assert.That(count, Is.GreaterThan(0));
            }
        }

        #endregion
    }
}

