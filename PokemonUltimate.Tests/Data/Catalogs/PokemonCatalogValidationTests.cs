using System.Linq;
using NUnit.Framework;
using PokemonUltimate.Content.Catalogs.Pokemon;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Tests.Data.Catalogs.Pokemon
{
    /// <summary>
    /// Validation tests for Pokemon Catalog data integrity.
    /// Verifies that Pokemon data matches official game data.
    /// </summary>
    [TestFixture]
    public class PokemonCatalogValidationTests
    {
        #region BST (Base Stat Total) Validation - Real Pokemon Data

        [Test]
        [Description("Pikachu BST should be 320")]
        public void Pikachu_BST_Is320()
        {
            // Official: 35+55+40+50+50+90 = 320
            Assert.That(PokemonCatalog.Pikachu.BaseStats.Total, Is.EqualTo(320));
        }

        [Test]
        [Description("Charizard BST should be 534")]
        public void Charizard_BST_Is534()
        {
            // Official: 78+84+78+109+85+100 = 534
            Assert.That(PokemonCatalog.Charizard.BaseStats.Total, Is.EqualTo(534));
        }

        [Test]
        [Description("Blastoise BST should be 530")]
        public void Blastoise_BST_Is530()
        {
            // Official: 79+83+100+85+105+78 = 530
            Assert.That(PokemonCatalog.Blastoise.BaseStats.Total, Is.EqualTo(530));
        }

        [Test]
        [Description("Venusaur BST should be 525")]
        public void Venusaur_BST_Is525()
        {
            // Official: 80+82+83+100+100+80 = 525
            Assert.That(PokemonCatalog.Venusaur.BaseStats.Total, Is.EqualTo(525));
        }

        [Test]
        [Description("Mewtwo BST should be 680 (legendary)")]
        public void Mewtwo_BST_Is680()
        {
            // Official: 106+110+90+154+90+130 = 680
            Assert.That(PokemonCatalog.Mewtwo.BaseStats.Total, Is.EqualTo(680));
        }

        [Test]
        [Description("Mew BST should be 600 (mythical)")]
        public void Mew_BST_Is600()
        {
            // Official: 100+100+100+100+100+100 = 600 (balanced)
            Assert.That(PokemonCatalog.Mew.BaseStats.Total, Is.EqualTo(600));
        }

        #endregion

        #region Individual Stat Validation

        [Test]
        public void Pikachu_Stats_AreCorrect()
        {
            var stats = PokemonCatalog.Pikachu.BaseStats;
            Assert.That(stats.HP, Is.EqualTo(35));
            Assert.That(stats.Attack, Is.EqualTo(55));
            Assert.That(stats.Defense, Is.EqualTo(40));
            Assert.That(stats.SpAttack, Is.EqualTo(50));
            Assert.That(stats.SpDefense, Is.EqualTo(50));
            Assert.That(stats.Speed, Is.EqualTo(90));
        }

        [Test]
        public void Charizard_Stats_AreCorrect()
        {
            var stats = PokemonCatalog.Charizard.BaseStats;
            Assert.That(stats.HP, Is.EqualTo(78));
            Assert.That(stats.Attack, Is.EqualTo(84));
            Assert.That(stats.Defense, Is.EqualTo(78));
            Assert.That(stats.SpAttack, Is.EqualTo(109));
            Assert.That(stats.SpDefense, Is.EqualTo(85));
            Assert.That(stats.Speed, Is.EqualTo(100));
        }

        [Test]
        public void Mewtwo_Stats_AreCorrect()
        {
            var stats = PokemonCatalog.Mewtwo.BaseStats;
            Assert.That(stats.HP, Is.EqualTo(106));
            Assert.That(stats.Attack, Is.EqualTo(110));
            Assert.That(stats.Defense, Is.EqualTo(90));
            Assert.That(stats.SpAttack, Is.EqualTo(154));
            Assert.That(stats.SpDefense, Is.EqualTo(90));
            Assert.That(stats.Speed, Is.EqualTo(130));
        }

        [Test]
        public void Mew_AllStats_Are100()
        {
            var stats = PokemonCatalog.Mew.BaseStats;
            Assert.That(stats.HP, Is.EqualTo(100));
            Assert.That(stats.Attack, Is.EqualTo(100));
            Assert.That(stats.Defense, Is.EqualTo(100));
            Assert.That(stats.SpAttack, Is.EqualTo(100));
            Assert.That(stats.SpDefense, Is.EqualTo(100));
            Assert.That(stats.Speed, Is.EqualTo(100));
        }

        #endregion

        #region Type Validation

        [Test]
        public void Pikachu_IsPureElectric()
        {
            Assert.That(PokemonCatalog.Pikachu.PrimaryType, Is.EqualTo(PokemonType.Electric));
            Assert.That(PokemonCatalog.Pikachu.SecondaryType, Is.Null);
            Assert.That(PokemonCatalog.Pikachu.IsDualType, Is.False);
        }

        [Test]
        public void Charizard_IsFireFlying()
        {
            Assert.That(PokemonCatalog.Charizard.PrimaryType, Is.EqualTo(PokemonType.Fire));
            Assert.That(PokemonCatalog.Charizard.SecondaryType, Is.EqualTo(PokemonType.Flying));
            Assert.That(PokemonCatalog.Charizard.IsDualType, Is.True);
        }

        [Test]
        public void Venusaur_IsGrassPoison()
        {
            Assert.That(PokemonCatalog.Venusaur.PrimaryType, Is.EqualTo(PokemonType.Grass));
            Assert.That(PokemonCatalog.Venusaur.SecondaryType, Is.EqualTo(PokemonType.Poison));
        }

        [Test]
        public void Blastoise_IsPureWater()
        {
            Assert.That(PokemonCatalog.Blastoise.PrimaryType, Is.EqualTo(PokemonType.Water));
            Assert.That(PokemonCatalog.Blastoise.SecondaryType, Is.Null);
        }

        [Test]
        public void Mewtwo_IsPurePsychic()
        {
            Assert.That(PokemonCatalog.Mewtwo.PrimaryType, Is.EqualTo(PokemonType.Psychic));
            Assert.That(PokemonCatalog.Mewtwo.SecondaryType, Is.Null);
        }

        #endregion

        #region Pokedex Number Validation

        [Test]
        public void Bulbasaur_IsNumber1()
        {
            Assert.That(PokemonCatalog.Bulbasaur.PokedexNumber, Is.EqualTo(1));
        }

        [Test]
        public void Charmander_IsNumber4()
        {
            Assert.That(PokemonCatalog.Charmander.PokedexNumber, Is.EqualTo(4));
        }

        [Test]
        public void Squirtle_IsNumber7()
        {
            Assert.That(PokemonCatalog.Squirtle.PokedexNumber, Is.EqualTo(7));
        }

        [Test]
        public void Pikachu_IsNumber25()
        {
            Assert.That(PokemonCatalog.Pikachu.PokedexNumber, Is.EqualTo(25));
        }

        [Test]
        public void Mewtwo_IsNumber150()
        {
            Assert.That(PokemonCatalog.Mewtwo.PokedexNumber, Is.EqualTo(150));
        }

        [Test]
        public void Mew_IsNumber151()
        {
            Assert.That(PokemonCatalog.Mew.PokedexNumber, Is.EqualTo(151));
        }

        #endregion

        #region Catalog Integrity Tests

        [Test]
        public void AllPokemon_HavePositiveStats()
        {
            foreach (var pokemon in PokemonCatalog.All)
            {
                Assert.That(pokemon.BaseStats.HP, Is.GreaterThan(0), $"{pokemon.Name} HP");
                Assert.That(pokemon.BaseStats.Attack, Is.GreaterThan(0), $"{pokemon.Name} Attack");
                Assert.That(pokemon.BaseStats.Defense, Is.GreaterThan(0), $"{pokemon.Name} Defense");
                Assert.That(pokemon.BaseStats.SpAttack, Is.GreaterThan(0), $"{pokemon.Name} SpAttack");
                Assert.That(pokemon.BaseStats.SpDefense, Is.GreaterThan(0), $"{pokemon.Name} SpDefense");
                Assert.That(pokemon.BaseStats.Speed, Is.GreaterThan(0), $"{pokemon.Name} Speed");
            }
        }

        [Test]
        public void AllPokemon_HaveValidBST()
        {
            foreach (var pokemon in PokemonCatalog.All)
            {
                // Minimum BST is around 180 (Sunkern), Maximum is 720+ (Mega/Primal)
                // Gen 1 range: ~250-680
                Assert.That(pokemon.BaseStats.Total, Is.InRange(200, 800), $"{pokemon.Name} BST");
            }
        }

        [Test]
        public void AllPokemon_HaveUniquePokedexNumbers()
        {
            var numbers = PokemonCatalog.All.Select(p => p.PokedexNumber).ToList();
            var uniqueNumbers = numbers.Distinct().ToList();
            
            Assert.That(uniqueNumbers.Count, Is.EqualTo(numbers.Count), "Duplicate Pokedex numbers found");
        }

        [Test]
        public void AllPokemon_HaveUniqueNames()
        {
            var names = PokemonCatalog.All.Select(p => p.Name).ToList();
            var uniqueNames = names.Distinct().ToList();
            
            Assert.That(uniqueNames.Count, Is.EqualTo(names.Count), "Duplicate names found");
        }

        [Test]
        public void AllPokemon_HaveNonEmptyNames()
        {
            foreach (var pokemon in PokemonCatalog.All)
            {
                Assert.That(pokemon.Name, Is.Not.Null.And.Not.Empty, "Pokemon with null/empty name");
            }
        }

        [Test]
        public void AllPokemon_HaveDefinedPrimaryType()
        {
            // All Pokemon should have a valid type (Normal is a valid type, not default)
            foreach (var pokemon in PokemonCatalog.All)
            {
                // Just verify the type is a valid enum value
                Assert.That(System.Enum.IsDefined(typeof(PokemonType), pokemon.PrimaryType), 
                    Is.True, $"{pokemon.Name} has undefined type");
            }
        }

        #endregion

        #region Evolution Chain Validation

        [Test]
        public void Starters_HaveTwoEvolutions()
        {
            // Bulbasaur -> Ivysaur -> Venusaur
            Assert.That(PokemonCatalog.Bulbasaur.CanEvolve, Is.True);
            Assert.That(PokemonCatalog.Ivysaur.CanEvolve, Is.True);
            Assert.That(PokemonCatalog.Venusaur.CanEvolve, Is.False);
            
            // Charmander -> Charmeleon -> Charizard
            Assert.That(PokemonCatalog.Charmander.CanEvolve, Is.True);
            Assert.That(PokemonCatalog.Charmeleon.CanEvolve, Is.True);
            Assert.That(PokemonCatalog.Charizard.CanEvolve, Is.False);
            
            // Squirtle -> Wartortle -> Blastoise
            Assert.That(PokemonCatalog.Squirtle.CanEvolve, Is.True);
            Assert.That(PokemonCatalog.Wartortle.CanEvolve, Is.True);
            Assert.That(PokemonCatalog.Blastoise.CanEvolve, Is.False);
        }

        [Test]
        public void Pikachu_EvolvesToRaichu()
        {
            Assert.That(PokemonCatalog.Pikachu.CanEvolve, Is.True);
            Assert.That(PokemonCatalog.Pikachu.Evolutions[0].Target.Name, Is.EqualTo("Raichu"));
        }

        [Test]
        public void Legendaries_CannotEvolve()
        {
            Assert.That(PokemonCatalog.Mewtwo.CanEvolve, Is.False);
            Assert.That(PokemonCatalog.Mew.CanEvolve, Is.False);
        }

        #endregion

        #region Gender Ratio Validation

        [Test]
        public void Starters_Have87_5PercentMale()
        {
            Assert.That(PokemonCatalog.Bulbasaur.GenderRatio, Is.EqualTo(87.5f));
            Assert.That(PokemonCatalog.Charmander.GenderRatio, Is.EqualTo(87.5f));
            Assert.That(PokemonCatalog.Squirtle.GenderRatio, Is.EqualTo(87.5f));
        }

        [Test]
        public void Pikachu_Has50PercentMale()
        {
            Assert.That(PokemonCatalog.Pikachu.GenderRatio, Is.EqualTo(50.0f));
            Assert.That(PokemonCatalog.Pikachu.HasBothGenders, Is.True);
        }

        [Test]
        public void Legendaries_AreGenderless()
        {
            Assert.That(PokemonCatalog.Mewtwo.IsGenderless, Is.True);
            Assert.That(PokemonCatalog.Mew.IsGenderless, Is.True);
        }

        #endregion

        #region Stat Range Extremes

        [Test]
        public void HighestSpAttack_IsMewtwo()
        {
            var highest = PokemonCatalog.All
                .OrderByDescending(p => p.BaseStats.SpAttack)
                .First();
            
            Assert.That(highest.Name, Is.EqualTo("Mewtwo"));
            Assert.That(highest.BaseStats.SpAttack, Is.EqualTo(154));
        }

        [Test]
        public void HighestSpeed_IsMewtwo()
        {
            var highest = PokemonCatalog.All
                .OrderByDescending(p => p.BaseStats.Speed)
                .First();
            
            Assert.That(highest.Name, Is.EqualTo("Mewtwo"));
            Assert.That(highest.BaseStats.Speed, Is.EqualTo(130));
        }

        [Test]
        public void LowestBST_InCatalog()
        {
            var lowest = PokemonCatalog.All
                .OrderBy(p => p.BaseStats.Total)
                .First();
            
            // Should be a basic Pokemon, probably a starter base form
            Assert.That(lowest.BaseStats.Total, Is.LessThan(400));
        }

        [Test]
        public void HighestBST_InCatalog()
        {
            var highest = PokemonCatalog.All
                .OrderByDescending(p => p.BaseStats.Total)
                .First();
            
            // Should be Mewtwo (680) in Gen 1
            Assert.That(highest.Name, Is.EqualTo("Mewtwo"));
            Assert.That(highest.BaseStats.Total, Is.EqualTo(680));
        }

        #endregion

        #region Cross-Evolution Stat Comparison

        [Test]
        public void Evolved_HasHigherBST_ThanPreEvolution()
        {
            // Charmander -> Charmeleon
            Assert.That(PokemonCatalog.Charmeleon.BaseStats.Total, 
                Is.GreaterThan(PokemonCatalog.Charmander.BaseStats.Total));
            
            // Charmeleon -> Charizard
            Assert.That(PokemonCatalog.Charizard.BaseStats.Total, 
                Is.GreaterThan(PokemonCatalog.Charmeleon.BaseStats.Total));
            
            // Pikachu -> Raichu
            Assert.That(PokemonCatalog.Raichu.BaseStats.Total, 
                Is.GreaterThan(PokemonCatalog.Pikachu.BaseStats.Total));
        }

        [Test]
        public void FinalEvolutions_HaveMinimum500BST()
        {
            // Final evolutions of starters should have 500+ BST
            Assert.That(PokemonCatalog.Venusaur.BaseStats.Total, Is.GreaterThanOrEqualTo(500));
            Assert.That(PokemonCatalog.Charizard.BaseStats.Total, Is.GreaterThanOrEqualTo(500));
            Assert.That(PokemonCatalog.Blastoise.BaseStats.Total, Is.GreaterThanOrEqualTo(500));
        }

        #endregion
    }
}

