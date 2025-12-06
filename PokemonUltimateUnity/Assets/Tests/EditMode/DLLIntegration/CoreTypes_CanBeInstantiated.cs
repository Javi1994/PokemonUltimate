using NUnit.Framework;
using PokemonUltimate.Core.Instances;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Content.Catalogs.Pokemon;

/// <summary>
/// Tests to verify core types can be instantiated from Unity.
/// 
/// **Feature**: 4: Unity Integration
/// **Sub-Feature**: 4.1: Unity Project Setup
/// **Documentation**: See `docs/features/4-unity-integration/4.1-unity-project-setup/testing.md`
/// </summary>
public class CoreTypes_CanBeInstantiated
{
    [Test]
    public void PokemonInstance_CanBeCreated()
    {
        // Arrange
        var species = PokemonCatalog.Pikachu;
        int level = 50;
        
        // Act
        var pokemon = PokemonFactory.Create(species, level);
        
        // Assert
        Assert.That(pokemon, Is.Not.Null, "PokemonInstance should be created successfully");
        Assert.That(pokemon.Species, Is.EqualTo(species), "Pokemon species should match");
        Assert.That(pokemon.Level, Is.EqualTo(level), "Pokemon level should match");
    }
    
    [Test]
    public void PokemonInstance_HasValidStats()
    {
        // Arrange
        var pokemon = PokemonFactory.Create(PokemonCatalog.Pikachu, 50);
        
        // Act & Assert
        Assert.That(pokemon.MaxHP, Is.GreaterThan(0), "MaxHP should be greater than 0");
        Assert.That(pokemon.CurrentHP, Is.GreaterThan(0), "CurrentHP should be greater than 0");
        Assert.That(pokemon.CurrentHP, Is.LessThanOrEqualTo(pokemon.MaxHP), "CurrentHP should not exceed MaxHP");
        Assert.That(pokemon.Attack, Is.GreaterThan(0), "Attack should be greater than 0");
        Assert.That(pokemon.Defense, Is.GreaterThan(0), "Defense should be greater than 0");
        Assert.That(pokemon.Speed, Is.GreaterThan(0), "Speed should be greater than 0");
    }
    
    [Test]
    public void PokemonCatalog_CanAccessMultiplePokemon()
    {
        // Arrange & Act
        var pikachu = PokemonCatalog.Pikachu;
        var charmander = PokemonCatalog.Charmander;
        
        // Assert
        Assert.That(pikachu, Is.Not.Null, "Pikachu should be accessible");
        Assert.That(charmander, Is.Not.Null, "Charmander should be accessible");
        Assert.That(pikachu.Name, Is.Not.EqualTo(charmander.Name), "Different Pokemon should have different names");
    }
    
    [Test]
    public void PokemonFactory_CanCreateMultiplePokemon()
    {
        // Arrange
        var species1 = PokemonCatalog.Pikachu;
        var species2 = PokemonCatalog.Charmander;
        
        // Act
        var pokemon1 = PokemonFactory.Create(species1, 50);
        var pokemon2 = PokemonFactory.Create(species2, 50);
        
        // Assert
        Assert.That(pokemon1, Is.Not.Null, "First Pokemon should be created");
        Assert.That(pokemon2, Is.Not.Null, "Second Pokemon should be created");
        Assert.That(pokemon1.Species.Name, Is.Not.EqualTo(pokemon2.Species.Name), "Pokemon should be different");
    }
}

