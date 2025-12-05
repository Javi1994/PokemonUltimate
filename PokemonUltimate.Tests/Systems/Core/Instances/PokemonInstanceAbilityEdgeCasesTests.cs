using NUnit.Framework;
using PokemonUltimate.Content.Catalogs.Abilities;
using PokemonUltimate.Content.Catalogs.Items;
using PokemonUltimate.Content.Catalogs.Pokemon;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Providers;

namespace PokemonUltimate.Tests.Systems.Core.Instances
{
    /// <summary>
    /// Edge case tests for Pokemon ability and held item functionality.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Pokemon Data
    /// **Sub-Feature**: 1.3: PokemonInstance
    /// **Documentation**: See `docs/features/1-pokemon-data/architecture.md`
    /// </remarks>
    [TestFixture]
    public class PokemonInstanceAbilityEdgeCasesTests
    {
        #region Ability Edge Cases

        [Test]
        public void Create_PokemonWithNoAbilitiesDefined_AbilityIsNull()
        {
            // Arrange - Create a species without abilities using the builder
            var speciesWithoutAbilities = Content.Builders.Pokemon.Define("TestMon", 9999)
                .Type(PokemonUltimate.Core.Enums.PokemonType.Normal)
                .Stats(50, 50, 50, 50, 50, 50)
                .Build();

            // Act
            var pokemon = PokemonInstanceBuilder.Create(speciesWithoutAbilities, 10).Build();

            // Assert
            Assert.That(pokemon.Ability, Is.Null);
            Assert.That(pokemon.HasAbility, Is.False);
        }

        [Test]
        public void WithHiddenAbility_WhenNoHiddenAbilityDefined_GetsRandomNormalAbility()
        {
            // Arrange - Mew only has Synchronize, no hidden ability
            // Act
            var pokemon = PokemonInstanceBuilder.Create(PokemonCatalog.Mew, 50)
                .WithHiddenAbility()
                .Build();

            // Assert - Should fall back to normal ability
            Assert.That(pokemon.Ability, Is.EqualTo(AbilityCatalog.Synchronize));
        }

        [Test]
        public void HasAbilityNamed_WhenAbilityIsNull_ReturnsFalse()
        {
            // Arrange
            var speciesWithoutAbilities = Content.Builders.Pokemon.Define("TestMon", 9998)
                .Type(PokemonUltimate.Core.Enums.PokemonType.Normal)
                .Stats(50, 50, 50, 50, 50, 50)
                .Build();
            var pokemon = PokemonInstanceBuilder.Create(speciesWithoutAbilities, 10).Build();

            // Act & Assert
            Assert.That(pokemon.HasAbilityNamed("Static"), Is.False);
        }

        [Test]
        public void HasAbilityNamed_EmptyString_ReturnsFalse()
        {
            // Arrange
            var pokemon = PokemonInstanceBuilder.Create(PokemonCatalog.Pikachu, 25).Build();

            // Act & Assert
            Assert.That(pokemon.HasAbilityNamed(""), Is.False);
        }

        [Test]
        public void IsUsingHiddenAbility_WhenNoHiddenAbilityDefined_ReturnsFalse()
        {
            // Arrange - Mew has no hidden ability
            var pokemon = PokemonInstanceBuilder.Create(PokemonCatalog.Mew, 50).Build();

            // Act & Assert
            Assert.That(pokemon.IsUsingHiddenAbility, Is.False);
        }

        [Test]
        public void SetAbility_ToNull_ClearsAbility()
        {
            // Arrange
            var pokemon = PokemonInstanceBuilder.Create(PokemonCatalog.Pikachu, 25).Build();

            // Act
            pokemon.SetAbility(null);

            // Assert
            Assert.That(pokemon.Ability, Is.Null);
            Assert.That(pokemon.HasAbility, Is.False);
        }

        [Test]
        public void Species_GetAllAbilities_WhenOnlyPrimaryDefined_ReturnsOnlyPrimary()
        {
            // Act - Mew only has Synchronize
            var abilities = new System.Collections.Generic.List<PokemonUltimate.Core.Blueprints.AbilityData>(
                PokemonCatalog.Mew.GetAllAbilities());

            // Assert
            Assert.That(abilities.Count, Is.EqualTo(1));
            Assert.That(abilities[0], Is.EqualTo(AbilityCatalog.Synchronize));
        }

        [Test]
        public void Species_GetRandomAbility_WithSecondaryAbility_CanReturnEither()
        {
            // Arrange
            var randomProvider = new RandomProvider(12345);
            bool gotPrimary = false;
            bool gotSecondary = false;

            // Act - Run multiple times to check randomness
            for (int i = 0; i < 100; i++)
            {
                var ability = PokemonCatalog.Eevee.GetRandomAbility(randomProvider);
                if (ability == AbilityCatalog.RunAway) gotPrimary = true;
                if (ability == AbilityCatalog.Adaptability) gotSecondary = true;
            }

            // Assert - Both should have been selected at least once
            Assert.That(gotPrimary, Is.True, "Primary ability should be selectable");
            Assert.That(gotSecondary, Is.True, "Secondary ability should be selectable");
        }

        [Test]
        public void Species_GetRandomAbility_WithNullParameter_UsesNewRandom()
        {
            // Act - Should not throw
            var ability = PokemonCatalog.Pikachu.GetRandomAbility(null);

            // Assert
            Assert.That(ability, Is.EqualTo(AbilityCatalog.Static)); // Only has one normal ability
        }

        #endregion

        #region Held Item Edge Cases

        [Test]
        public void TakeItem_MultipleTimes_ReturnsNullAfterFirst()
        {
            // Arrange
            var pokemon = PokemonInstanceBuilder.Create(PokemonCatalog.Pikachu, 25)
                .Holding(ItemCatalog.Leftovers)
                .Build();

            // Act
            var firstTake = pokemon.TakeItem();
            var secondTake = pokemon.TakeItem();
            var thirdTake = pokemon.TakeItem();

            // Assert
            Assert.That(firstTake, Is.EqualTo(ItemCatalog.Leftovers));
            Assert.That(secondTake, Is.Null);
            Assert.That(thirdTake, Is.Null);
        }

        [Test]
        public void GiveItem_ReplacesExistingItem()
        {
            // Arrange
            var pokemon = PokemonInstanceBuilder.Create(PokemonCatalog.Pikachu, 25)
                .Holding(ItemCatalog.Leftovers)
                .Build();

            // Act
            pokemon.GiveItem(ItemCatalog.ChoiceBand);

            // Assert
            Assert.That(pokemon.HeldItem, Is.EqualTo(ItemCatalog.ChoiceBand));
        }

        [Test]
        public void GiveItem_Null_ClearsItem()
        {
            // Arrange
            var pokemon = PokemonInstanceBuilder.Create(PokemonCatalog.Pikachu, 25)
                .Holding(ItemCatalog.Leftovers)
                .Build();

            // Act
            pokemon.GiveItem(null);

            // Assert
            Assert.That(pokemon.HeldItem, Is.Null);
            Assert.That(pokemon.HasHeldItem, Is.False);
        }

        [Test]
        public void HeldItem_CanBeModifiedDirectly()
        {
            // Arrange
            var pokemon = PokemonInstanceBuilder.Create(PokemonCatalog.Pikachu, 25).Build();

            // Act
            pokemon.HeldItem = ItemCatalog.ChoiceScarf;

            // Assert
            Assert.That(pokemon.HeldItem, Is.EqualTo(ItemCatalog.ChoiceScarf));
        }

        #endregion

        #region Builder Edge Cases

        [Test]
        public void WithAbility_ThenWithRandomAbility_ResetsToRandom()
        {
            // Arrange
            PokemonInstanceBuilder.SetSeed(42); // For consistent results

            // Act - Set specific ability, then random
            var pokemon = PokemonInstanceBuilder.Create(PokemonCatalog.Eevee, 10)
                .WithAbility(AbilityCatalog.Intimidate)
                .WithRandomAbility()
                .Build();

            // Assert - Should be one of Eevee's normal abilities
            Assert.That(pokemon.Ability, Is.AnyOf(AbilityCatalog.RunAway, AbilityCatalog.Adaptability));
        }

        [Test]
        public void WithHiddenAbility_ThenWithAbility_UsesSpecificAbility()
        {
            // Act - Use Pikachu's valid primary ability (Static) after setting hidden ability
            var pokemon = PokemonInstanceBuilder.Create(PokemonCatalog.Pikachu, 25)
                .WithHiddenAbility()
                .WithAbility(AbilityCatalog.Static)  // Override with primary ability
                .Build();

            // Assert
            Assert.That(pokemon.Ability, Is.EqualTo(AbilityCatalog.Static));
        }

        [Test]
        public void Holding_ThenNoItem_ClearsItem()
        {
            // Act
            var pokemon = PokemonInstanceBuilder.Create(PokemonCatalog.Pikachu, 25)
                .Holding(ItemCatalog.Leftovers)
                .NoItem()
                .Build();

            // Assert
            Assert.That(pokemon.HasHeldItem, Is.False);
        }

        [Test]
        public void MultipleBuildsFromSameBuilder_CreateIndependentInstances()
        {
            // Arrange
            var builder = PokemonInstanceBuilder.Create(PokemonCatalog.Pikachu, 25)
                .Holding(ItemCatalog.Leftovers);

            // Act
            var pokemon1 = builder.Build();
            var pokemon2 = builder.Build();

            // Modify one
            pokemon1.TakeItem();

            // Assert - They should be independent
            Assert.That(pokemon1.HasHeldItem, Is.False);
            Assert.That(pokemon2.HasHeldItem, Is.True);
        }

        #endregion
    }
}

