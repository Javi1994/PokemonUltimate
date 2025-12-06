using NUnit.Framework;
using PokemonUltimate.Core.Instances;
using PokemonUltimate.Combat;
using PokemonUltimate.Content.Catalogs.Pokemon;

/// <summary>
/// Tests to verify DLLs load without errors.
/// 
/// **Feature**: 4: Unity Integration
/// **Sub-Feature**: 4.1: Unity Project Setup
/// **Documentation**: See `docs/features/4-unity-integration/4.1-unity-project-setup/testing.md`
/// </summary>
public class DLLs_LoadWithoutErrors
{
    [Test]
    public void CoreDLL_LoadsWithoutErrors()
    {
        // Arrange & Act
        var type = typeof(PokemonInstance);
        
        // Assert
        Assert.That(type, Is.Not.Null, "PokemonInstance type should be accessible from Core DLL");
    }
    
    [Test]
    public void CombatDLL_LoadsWithoutErrors()
    {
        // Arrange & Act
        var type = typeof(CombatEngine);
        
        // Assert
        Assert.That(type, Is.Not.Null, "CombatEngine type should be accessible from Combat DLL");
    }
    
    [Test]
    public void ContentDLL_LoadsWithoutErrors()
    {
        // Arrange & Act
        var catalog = PokemonCatalog.Pikachu;
        
        // Assert
        Assert.That(catalog, Is.Not.Null, "PokemonCatalog should be accessible from Content DLL");
    }
}

