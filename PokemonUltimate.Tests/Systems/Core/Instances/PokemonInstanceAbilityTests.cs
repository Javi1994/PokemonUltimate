using NUnit.Framework;
using PokemonUltimate.Content.Catalogs.Abilities;
using PokemonUltimate.Content.Catalogs.Items;
using PokemonUltimate.Content.Catalogs.Pokemon;
using PokemonUltimate.Core.Factories;

namespace PokemonUltimate.Tests.Systems.Core.Instances
{
    /// <summary>
    /// Tests for Pokemon ability and held item functionality.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Pokemon Data
    /// **Sub-Feature**: 1.3: PokemonInstance
    /// **Documentation**: See `docs/features/1-pokemon-data/architecture.md`
    /// </remarks>
    [TestFixture]
    public class PokemonInstanceAbilityTests
    {
        #region Ability Tests

        [Test]
        public void Create_DefaultAbility_GetsPrimaryAbility()
        {
            // Arrange & Act
            var pokemon = PokemonInstanceBuilder.Create(PokemonCatalog.Pikachu, 25).Build();

            // Assert
            Assert.That(pokemon.Ability, Is.EqualTo(AbilityCatalog.Static));
            Assert.That(pokemon.HasAbility, Is.True);
        }

        [Test]
        public void Create_WithHiddenAbility_GetsHiddenAbility()
        {
            // Arrange & Act
            var pokemon = PokemonInstanceBuilder.Create(PokemonCatalog.Pikachu, 25)
                .WithHiddenAbility()
                .Build();

            // Assert
            Assert.That(pokemon.Ability, Is.EqualTo(AbilityCatalog.LightningRod));
            Assert.That(pokemon.IsUsingHiddenAbility, Is.True);
        }

        [Test]
        public void Create_WithSpecificAbility_GetsSpecifiedAbility()
        {
            // Arrange & Act
            var pokemon = PokemonInstanceBuilder.Create(PokemonCatalog.Pikachu, 25)
                .WithAbility(AbilityCatalog.Levitate)
                .Build();

            // Assert
            Assert.That(pokemon.Ability, Is.EqualTo(AbilityCatalog.Levitate));
        }

        [Test]
        public void HasAbilityNamed_CorrectName_ReturnsTrue()
        {
            // Arrange
            var pokemon = PokemonInstanceBuilder.Create(PokemonCatalog.Pikachu, 25).Build();

            // Act & Assert
            Assert.That(pokemon.HasAbilityNamed("Static"), Is.True);
            Assert.That(pokemon.HasAbilityNamed("static"), Is.True); // Case insensitive
            Assert.That(pokemon.HasAbilityNamed("Intimidate"), Is.False);
        }

        [Test]
        public void SetAbility_ChangesAbility()
        {
            // Arrange
            var pokemon = PokemonInstanceBuilder.Create(PokemonCatalog.Pikachu, 25).Build();
            Assert.That(pokemon.Ability, Is.EqualTo(AbilityCatalog.Static));

            // Act
            pokemon.SetAbility(AbilityCatalog.Intimidate);

            // Assert
            Assert.That(pokemon.Ability, Is.EqualTo(AbilityCatalog.Intimidate));
        }

        [Test]
        public void IsUsingHiddenAbility_WhenNotHidden_ReturnsFalse()
        {
            // Arrange & Act
            var pokemon = PokemonInstanceBuilder.Create(PokemonCatalog.Pikachu, 25).Build();

            // Assert
            Assert.That(pokemon.IsUsingHiddenAbility, Is.False);
        }

        [Test]
        public void Species_GetAllAbilities_ReturnsAllAvailableAbilities()
        {
            // Act
            var abilities = PokemonCatalog.Snorlax.GetAllAbilities();

            // Assert - Snorlax has Immunity, Thick Fat, and Gluttony (hidden)
            Assert.That(abilities, Does.Contain(AbilityCatalog.Immunity));
            Assert.That(abilities, Does.Contain(AbilityCatalog.ThickFat));
            Assert.That(abilities, Does.Contain(AbilityCatalog.Gluttony));
        }

        [Test]
        public void Species_HasSecondaryAbility_ReturnsCorrectValue()
        {
            // Assert
            Assert.That(PokemonCatalog.Snorlax.HasSecondaryAbility, Is.True); // Has Thick Fat
            Assert.That(PokemonCatalog.Pikachu.HasSecondaryAbility, Is.False); // Only Static
        }

        [Test]
        public void Species_HasHiddenAbility_ReturnsCorrectValue()
        {
            // Assert
            Assert.That(PokemonCatalog.Pikachu.HasHiddenAbility, Is.True); // Lightning Rod
            Assert.That(PokemonCatalog.Mew.HasHiddenAbility, Is.False); // Mew only has Synchronize
        }

        [Test]
        public void QuickCreate_WithHiddenAbility_Works()
        {
            // Arrange & Act
            var pokemon = PokemonInstanceBuilder.CreateWithHiddenAbility(PokemonCatalog.Bulbasaur, 10);

            // Assert
            Assert.That(pokemon.Ability, Is.EqualTo(AbilityCatalog.Chlorophyll));
        }

        #endregion

        #region Held Item Tests

        [Test]
        public void Create_WithItem_HasItem()
        {
            // Arrange & Act
            var pokemon = PokemonInstanceBuilder.Create(PokemonCatalog.Pikachu, 25)
                .Holding(ItemCatalog.Leftovers)
                .Build();

            // Assert
            Assert.That(pokemon.HeldItem, Is.EqualTo(ItemCatalog.Leftovers));
            Assert.That(pokemon.HasHeldItem, Is.True);
        }

        [Test]
        public void Create_WithoutItem_HasNoItem()
        {
            // Arrange & Act
            var pokemon = PokemonInstanceBuilder.Create(PokemonCatalog.Pikachu, 25).Build();

            // Assert
            Assert.That(pokemon.HeldItem, Is.Null);
            Assert.That(pokemon.HasHeldItem, Is.False);
        }

        [Test]
        public void GiveItem_AssignsItem()
        {
            // Arrange
            var pokemon = PokemonInstanceBuilder.Create(PokemonCatalog.Pikachu, 25).Build();
            Assert.That(pokemon.HasHeldItem, Is.False);

            // Act
            pokemon.GiveItem(ItemCatalog.ChoiceBand);

            // Assert
            Assert.That(pokemon.HeldItem, Is.EqualTo(ItemCatalog.ChoiceBand));
            Assert.That(pokemon.HasHeldItem, Is.True);
        }

        [Test]
        public void TakeItem_RemovesAndReturnsItem()
        {
            // Arrange
            var pokemon = PokemonInstanceBuilder.Create(PokemonCatalog.Pikachu, 25)
                .Holding(ItemCatalog.LifeOrb)
                .Build();

            // Act
            var item = pokemon.TakeItem();

            // Assert
            Assert.That(item, Is.EqualTo(ItemCatalog.LifeOrb));
            Assert.That(pokemon.HeldItem, Is.Null);
            Assert.That(pokemon.HasHeldItem, Is.False);
        }

        [Test]
        public void TakeItem_WhenNoItem_ReturnsNull()
        {
            // Arrange
            var pokemon = PokemonInstanceBuilder.Create(PokemonCatalog.Pikachu, 25).Build();

            // Act
            var item = pokemon.TakeItem();

            // Assert
            Assert.That(item, Is.Null);
        }

        [Test]
        public void QuickCreate_WithItem_Works()
        {
            // Arrange & Act
            var pokemon = PokemonInstanceBuilder.CreateWithItem(
                PokemonCatalog.Bulbasaur, 10, ItemCatalog.SitrusBerry);

            // Assert
            Assert.That(pokemon.HeldItem, Is.EqualTo(ItemCatalog.SitrusBerry));
        }

        [Test]
        public void WithItem_AliasSyntax_Works()
        {
            // Arrange & Act
            var pokemon = PokemonInstanceBuilder.Create(PokemonCatalog.Pikachu, 25)
                .WithItem(ItemCatalog.FocusSash)
                .Build();

            // Assert
            Assert.That(pokemon.HeldItem, Is.EqualTo(ItemCatalog.FocusSash));
        }

        [Test]
        public void NoItem_ClearsItem()
        {
            // Arrange & Act
            var pokemon = PokemonInstanceBuilder.Create(PokemonCatalog.Pikachu, 25)
                .Holding(ItemCatalog.Leftovers)
                .NoItem()
                .Build();

            // Assert
            Assert.That(pokemon.HeldItem, Is.Null);
        }

        #endregion

        #region Combined Tests

        [Test]
        public void Create_WithAbilityAndItem_BothSet()
        {
            // Arrange & Act
            var pokemon = PokemonInstanceBuilder.Create(PokemonCatalog.Charizard, 50)
                .WithHiddenAbility()
                .Holding(ItemCatalog.ChoiceScarf)
                .Build();

            // Assert
            Assert.That(pokemon.Ability, Is.EqualTo(AbilityCatalog.SolarPower));
            Assert.That(pokemon.HeldItem, Is.EqualTo(ItemCatalog.ChoiceScarf));
        }

        [Test]
        public void Create_AllStartersHaveAbilities()
        {
            // Act
            var bulbasaur = PokemonInstanceBuilder.Create(PokemonCatalog.Bulbasaur, 5).Build();
            var charmander = PokemonInstanceBuilder.Create(PokemonCatalog.Charmander, 5).Build();
            var squirtle = PokemonInstanceBuilder.Create(PokemonCatalog.Squirtle, 5).Build();

            // Assert
            Assert.That(bulbasaur.Ability, Is.EqualTo(AbilityCatalog.Overgrow));
            Assert.That(charmander.Ability, Is.EqualTo(AbilityCatalog.Blaze));
            Assert.That(squirtle.Ability, Is.EqualTo(AbilityCatalog.Torrent));
        }

        #endregion
    }
}

