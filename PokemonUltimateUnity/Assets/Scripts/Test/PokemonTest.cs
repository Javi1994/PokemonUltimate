using UnityEngine;
using PokemonUltimate.Core.Instances;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Content.Catalogs.Pokemon;

/// <summary>
/// Test script to verify PokemonUltimate DLL integration with Unity.
/// 
/// **Feature**: 4: Unity Integration
/// **Sub-Feature**: 4.1: Unity Project Setup
/// **Documentation**: See `docs/features/4-unity-integration/4.1-unity-project-setup/README.md`
/// </summary>
public class PokemonTest : MonoBehaviour
{
    [Header("Test Configuration")]
    [SerializeField] private bool runOnStart = true;
    [SerializeField] private bool logDetailedStats = true;

    void Start()
    {
        if (runOnStart)
        {
            RunTests();
        }
    }

    /// <summary>
    /// Run all integration tests and log results.
    /// </summary>
    public void RunTests()
    {
        Debug.Log("=== PokemonUltimate Unity Integration Tests ===");
        
        TestCreatePokemon();
        TestPokemonStats();
        TestPokemonMoves();
        TestPokemonCatalog();
        
        Debug.Log("=== All Tests Complete ===");
    }

    /// <summary>
    /// Test: Can create a Pokemon instance.
    /// </summary>
    private void TestCreatePokemon()
    {
        Debug.Log("\n[Test 1] Creating Pokemon...");
        
        try
        {
            var pikachu = PokemonFactory.Create(PokemonCatalog.Pikachu, 50);
            Debug.Log($"✓ Successfully created {pikachu.DisplayName} (Level {pikachu.Level})");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"✗ Failed to create Pokemon: {ex.Message}");
        }
    }

    /// <summary>
    /// Test: Pokemon stats are accessible.
    /// </summary>
    private void TestPokemonStats()
    {
        Debug.Log("\n[Test 2] Checking Pokemon stats...");
        
        try
        {
            var pikachu = PokemonFactory.Create(PokemonCatalog.Pikachu, 50);
            
            if (logDetailedStats)
            {
                Debug.Log($"  Name: {pikachu.DisplayName}");
                Debug.Log($"  Level: {pikachu.Level}");
                Debug.Log($"  HP: {pikachu.CurrentHP}/{pikachu.MaxHP}");
                Debug.Log($"  Attack: {pikachu.Attack}");
                Debug.Log($"  Defense: {pikachu.Defense}");
                Debug.Log($"  SpAttack: {pikachu.SpAttack}");
                Debug.Log($"  SpDefense: {pikachu.SpDefense}");
                Debug.Log($"  Speed: {pikachu.Speed}");
            }
            
            Debug.Log($"✓ Stats accessible - HP: {pikachu.CurrentHP}/{pikachu.MaxHP}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"✗ Failed to access stats: {ex.Message}");
        }
    }

    /// <summary>
    /// Test: Pokemon moves are accessible.
    /// </summary>
    private void TestPokemonMoves()
    {
        Debug.Log("\n[Test 3] Checking Pokemon moves...");
        
        try
        {
            var pikachu = PokemonFactory.Create(PokemonCatalog.Pikachu, 50);
            
            if (pikachu.Moves.Count > 0)
            {
                Debug.Log($"✓ Pokemon has {pikachu.Moves.Count} moves:");
                foreach (var move in pikachu.Moves)
                {
                    Debug.Log($"  - {move.Move.Name} (PP: {move.CurrentPP}/{move.MaxPP})");
                }
            }
            else
            {
                Debug.LogWarning("⚠ Pokemon has no moves (this may be expected)");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"✗ Failed to access moves: {ex.Message}");
        }
    }

    /// <summary>
    /// Test: Pokemon catalog is accessible.
    /// </summary>
    private void TestPokemonCatalog()
    {
        Debug.Log("\n[Test 4] Checking Pokemon catalog...");
        
        try
        {
            // Try accessing different Pokemon from catalog
            var pokemonList = new[]
            {
                PokemonCatalog.Pikachu,
                PokemonCatalog.Charmander,
                PokemonCatalog.Squirtle,
                PokemonCatalog.Bulbasaur
            };
            
            int successCount = 0;
            foreach (var species in pokemonList)
            {
                try
                {
                    var pokemon = PokemonFactory.Create(species, 50);
                    successCount++;
                }
                catch
                {
                    // Skip if Pokemon doesn't exist
                }
            }
            
            Debug.Log($"✓ Successfully created {successCount}/{pokemonList.Length} Pokemon from catalog");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"✗ Failed to access catalog: {ex.Message}");
        }
    }

    /// <summary>
    /// Test: Can create multiple Pokemon (for future battle testing).
    /// </summary>
    [ContextMenu("Test Multiple Pokemon")]
    public void TestMultiplePokemon()
    {
        Debug.Log("\n[Test 5] Creating multiple Pokemon...");
        
        try
        {
            var playerTeam = new[]
            {
                PokemonFactory.Create(PokemonCatalog.Pikachu, 50),
                PokemonFactory.Create(PokemonCatalog.Charmander, 50)
            };
            
            var enemyTeam = new[]
            {
                PokemonFactory.Create(PokemonCatalog.Squirtle, 50),
                PokemonFactory.Create(PokemonCatalog.Bulbasaur, 50)
            };
            
            Debug.Log($"✓ Created player team: {playerTeam.Length} Pokemon");
            Debug.Log($"✓ Created enemy team: {enemyTeam.Length} Pokemon");
            Debug.Log("  (Ready for future battle integration tests)");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"✗ Failed to create multiple Pokemon: {ex.Message}");
        }
    }
}

