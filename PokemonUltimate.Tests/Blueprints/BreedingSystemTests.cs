using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Tests.Blueprints
{
    /// <summary>
    /// Functional tests for Breeding System (Egg Groups and Egg Cycles).
    /// </summary>
    [TestFixture]
    public class BreedingSystemTests
    {
        #region EggGroup Enum Tests

        [Test]
        public void EggGroup_Enum_HasAllRequiredValues()
        {
            // Arrange & Act - Verify all egg groups exist
            var eggGroups = System.Enum.GetValues(typeof(EggGroup)).Cast<EggGroup>().ToList();

            // Assert
            Assert.That(eggGroups, Contains.Item(EggGroup.Monster));
            Assert.That(eggGroups, Contains.Item(EggGroup.Water1));
            Assert.That(eggGroups, Contains.Item(EggGroup.Bug));
            Assert.That(eggGroups, Contains.Item(EggGroup.Flying));
            Assert.That(eggGroups, Contains.Item(EggGroup.Field));
            Assert.That(eggGroups, Contains.Item(EggGroup.Fairy));
            Assert.That(eggGroups, Contains.Item(EggGroup.Grass));
            Assert.That(eggGroups, Contains.Item(EggGroup.HumanLike));
            Assert.That(eggGroups, Contains.Item(EggGroup.Mineral));
            Assert.That(eggGroups, Contains.Item(EggGroup.Amorphous));
            Assert.That(eggGroups, Contains.Item(EggGroup.Dragon));
            Assert.That(eggGroups, Contains.Item(EggGroup.Ditto));
            Assert.That(eggGroups, Contains.Item(EggGroup.Undiscovered));
            Assert.That(eggGroups.Count, Is.EqualTo(13));
        }

        #endregion

        #region EggGroups Property Tests

        [Test]
        public void PokemonSpeciesData_EggGroups_Default_IsEmpty()
        {
            // Arrange
            var pokemon = new PokemonSpeciesData();

            // Assert
            Assert.That(pokemon.EggGroups, Is.Not.Null);
            Assert.That(pokemon.EggGroups, Is.Empty);
        }

        [Test]
        public void PokemonSpeciesData_EggGroups_CanBeSet()
        {
            // Arrange
            var pokemon = new PokemonSpeciesData
            {
                EggGroups = new List<EggGroup> { EggGroup.Field, EggGroup.Fairy }
            };

            // Assert
            Assert.That(pokemon.EggGroups.Count, Is.EqualTo(2));
            Assert.That(pokemon.EggGroups, Contains.Item(EggGroup.Field));
            Assert.That(pokemon.EggGroups, Contains.Item(EggGroup.Fairy));
        }

        [Test]
        public void PokemonSpeciesData_EggGroups_CanHaveSingleGroup()
        {
            // Arrange
            var pokemon = new PokemonSpeciesData
            {
                EggGroups = new List<EggGroup> { EggGroup.Monster }
            };

            // Assert
            Assert.That(pokemon.EggGroups.Count, Is.EqualTo(1));
            Assert.That(pokemon.EggGroups, Contains.Item(EggGroup.Monster));
        }

        [Test]
        public void PokemonSpeciesData_EggGroups_CanHaveMultipleGroups()
        {
            // Arrange
            var pokemon = new PokemonSpeciesData
            {
                EggGroups = new List<EggGroup> { EggGroup.Field, EggGroup.Fairy }
            };

            // Assert
            Assert.That(pokemon.EggGroups.Count, Is.EqualTo(2));
        }

        #endregion

        #region EggCycles Property Tests

        [Test]
        public void PokemonSpeciesData_EggCycles_Default_Is20()
        {
            // Arrange
            var pokemon = new PokemonSpeciesData();

            // Assert
            Assert.That(pokemon.EggCycles, Is.EqualTo(20));
        }

        [Test]
        public void PokemonSpeciesData_EggCycles_CanBeSet()
        {
            // Arrange
            var pokemon = new PokemonSpeciesData
            {
                EggCycles = 5
            };

            // Assert
            Assert.That(pokemon.EggCycles, Is.EqualTo(5));
        }

        [Test]
        public void PokemonSpeciesData_EggCycles_CanBeSetToMinimum()
        {
            // Arrange
            var pokemon = new PokemonSpeciesData
            {
                EggCycles = 1
            };

            // Assert
            Assert.That(pokemon.EggCycles, Is.EqualTo(1));
        }

        [Test]
        public void PokemonSpeciesData_EggCycles_CanBeSetToMaximum()
        {
            // Arrange
            var pokemon = new PokemonSpeciesData
            {
                EggCycles = 40
            };

            // Assert
            Assert.That(pokemon.EggCycles, Is.EqualTo(40));
        }

        #endregion

        #region Breeding Compatibility Tests

        [Test]
        public void CanBreedWith_WithSharedEggGroup_ReturnsTrue()
        {
            // Arrange
            var pikachu = new PokemonSpeciesData
            {
                Name = "Pikachu",
                EggGroups = new List<EggGroup> { EggGroup.Field, EggGroup.Fairy }
            };

            var growlithe = new PokemonSpeciesData
            {
                Name = "Growlithe",
                EggGroups = new List<EggGroup> { EggGroup.Field }
            };

            // Act
            bool canBreed = pikachu.CanBreedWith(growlithe);

            // Assert
            Assert.That(canBreed, Is.True, "Pikachu and Growlithe share Field egg group");
        }

        [Test]
        public void CanBreedWith_WithNoSharedEggGroup_ReturnsFalse()
        {
            // Arrange
            var pikachu = new PokemonSpeciesData
            {
                Name = "Pikachu",
                EggGroups = new List<EggGroup> { EggGroup.Field }
            };

            var bulbasaur = new PokemonSpeciesData
            {
                Name = "Bulbasaur",
                EggGroups = new List<EggGroup> { EggGroup.Grass }
            };

            // Act
            bool canBreed = pikachu.CanBreedWith(bulbasaur);

            // Assert
            Assert.That(canBreed, Is.False, "Pikachu and Bulbasaur don't share egg groups");
        }

        [Test]
        public void CanBreedWith_WithDitto_ReturnsTrue()
        {
            // Arrange
            var pikachu = new PokemonSpeciesData
            {
                Name = "Pikachu",
                EggGroups = new List<EggGroup> { EggGroup.Field }
            };

            var ditto = new PokemonSpeciesData
            {
                Name = "Ditto",
                EggGroups = new List<EggGroup> { EggGroup.Ditto }
            };

            // Act
            bool canBreed = pikachu.CanBreedWith(ditto);

            // Assert
            Assert.That(canBreed, Is.True, "Ditto can breed with any Pokemon (except Undiscovered)");
        }

        [Test]
        public void CanBreedWith_WithUndiscovered_ReturnsFalse()
        {
            // Arrange
            var pikachu = new PokemonSpeciesData
            {
                Name = "Pikachu",
                EggGroups = new List<EggGroup> { EggGroup.Field }
            };

            var mewtwo = new PokemonSpeciesData
            {
                Name = "Mewtwo",
                EggGroups = new List<EggGroup> { EggGroup.Undiscovered }
            };

            // Act
            bool canBreed = pikachu.CanBreedWith(mewtwo);

            // Assert
            Assert.That(canBreed, Is.False, "Undiscovered egg group cannot breed");
        }

        [Test]
        public void CanBreedWith_DittoWithUndiscovered_ReturnsFalse()
        {
            // Arrange
            var ditto = new PokemonSpeciesData
            {
                Name = "Ditto",
                EggGroups = new List<EggGroup> { EggGroup.Ditto }
            };

            var mewtwo = new PokemonSpeciesData
            {
                Name = "Mewtwo",
                EggGroups = new List<EggGroup> { EggGroup.Undiscovered }
            };

            // Act
            bool canBreed = ditto.CanBreedWith(mewtwo);

            // Assert
            Assert.That(canBreed, Is.False, "Ditto cannot breed with Undiscovered egg group");
        }

        [Test]
        public void CanBreedWith_NullOther_ThrowsException()
        {
            // Arrange
            var pikachu = new PokemonSpeciesData
            {
                Name = "Pikachu",
                EggGroups = new List<EggGroup> { EggGroup.Field }
            };

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => pikachu.CanBreedWith(null));
        }

        [Test]
        public void IsInEggGroup_WithMatchingGroup_ReturnsTrue()
        {
            // Arrange
            var pikachu = new PokemonSpeciesData
            {
                Name = "Pikachu",
                EggGroups = new List<EggGroup> { EggGroup.Field, EggGroup.Fairy }
            };

            // Act & Assert
            Assert.That(pikachu.IsInEggGroup(EggGroup.Field), Is.True);
            Assert.That(pikachu.IsInEggGroup(EggGroup.Fairy), Is.True);
        }

        [Test]
        public void IsInEggGroup_WithNonMatchingGroup_ReturnsFalse()
        {
            // Arrange
            var pikachu = new PokemonSpeciesData
            {
                Name = "Pikachu",
                EggGroups = new List<EggGroup> { EggGroup.Field }
            };

            // Act & Assert
            Assert.That(pikachu.IsInEggGroup(EggGroup.Grass), Is.False);
        }

        [Test]
        public void CannotBreed_WithUndiscovered_ReturnsTrue()
        {
            // Arrange
            var mewtwo = new PokemonSpeciesData
            {
                Name = "Mewtwo",
                EggGroups = new List<EggGroup> { EggGroup.Undiscovered }
            };

            // Act & Assert
            Assert.That(mewtwo.CannotBreed, Is.True);
        }

        [Test]
        public void CannotBreed_WithoutUndiscovered_ReturnsFalse()
        {
            // Arrange
            var pikachu = new PokemonSpeciesData
            {
                Name = "Pikachu",
                EggGroups = new List<EggGroup> { EggGroup.Field }
            };

            // Act & Assert
            Assert.That(pikachu.CannotBreed, Is.False);
        }

        #endregion
    }
}

