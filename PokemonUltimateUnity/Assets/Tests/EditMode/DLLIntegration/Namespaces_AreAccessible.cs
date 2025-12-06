using NUnit.Framework;
using PokemonUltimate.Core.Instances;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Content.Catalogs.Pokemon;
using PokemonUltimate.Combat;

/// <summary>
/// Tests to verify namespaces are accessible from Unity.
/// 
/// **Feature**: 4: Unity Integration
/// **Sub-Feature**: 4.1: Unity Project Setup
/// **Documentation**: See `docs/features/4-unity-integration/4.1-unity-project-setup/testing.md`
/// </summary>
public class Namespaces_AreAccessible
{
    [Test]
    public void CoreInstances_NamespaceIsAccessible()
    {
        // Arrange & Act
        var type = typeof(PokemonInstance);
        
        // Assert
        Assert.That(type.Namespace, Is.EqualTo("PokemonUltimate.Core.Instances"));
    }
    
    [Test]
    public void CoreFactories_NamespaceIsAccessible()
    {
        // Arrange & Act
        var type = typeof(PokemonFactory);
        
        // Assert
        Assert.That(type.Namespace, Is.EqualTo("PokemonUltimate.Core.Factories"));
    }
    
    [Test]
    public void CoreBlueprints_NamespaceIsAccessible()
    {
        // Arrange & Act
        var type = typeof(PokemonSpeciesData);
        
        // Assert
        Assert.That(type.Namespace, Is.EqualTo("PokemonUltimate.Core.Blueprints"));
    }
    
    [Test]
    public void ContentCatalogs_NamespaceIsAccessible()
    {
        // Arrange & Act
        var catalog = PokemonCatalog.Pikachu;
        
        // Assert
        Assert.That(catalog, Is.Not.Null);
        Assert.That(catalog.GetType().Namespace, Is.EqualTo("PokemonUltimate.Core.Blueprints"));
    }
    
    [Test]
    public void Combat_NamespaceIsAccessible()
    {
        // Arrange & Act
        var type = typeof(CombatEngine);
        
        // Assert
        Assert.That(type.Namespace, Is.EqualTo("PokemonUltimate.Combat"));
    }
    
    [Test]
    public void CombatField_NamespaceIsAccessible()
    {
        // Arrange & Act
        var type = typeof(BattleField);
        
        // Assert
        // Note: BattleField is in PokemonUltimate.Combat namespace, not PokemonUltimate.Combat.Field
        Assert.That(type.Namespace, Is.EqualTo("PokemonUltimate.Combat"));
    }
}

