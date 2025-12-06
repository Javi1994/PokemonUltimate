using UnityEngine;
using PokemonUltimate.Core.Instances;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Content.Catalogs.Pokemon;

/// <summary>
/// Example script showing how to use UI components together.
/// This can be attached to a GameObject in the battle scene to test UI components.
/// 
/// **Feature**: 4: Unity Integration
/// **Sub-Feature**: 4.2: UI Foundation
/// **Documentation**: See `docs/features/4-unity-integration/4.2-ui-foundation/README.md`
/// </summary>
public class BattleUISetup : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private HPBar playerHPBar;
    [SerializeField] private HPBar enemyHPBar;
    [SerializeField] private PokemonDisplay playerDisplay;
    [SerializeField] private PokemonDisplay enemyDisplay;
    [SerializeField] private BattleDialog dialog;
    
    // Public properties for editor script access
    public HPBar PlayerHPBar => playerHPBar;
    public HPBar EnemyHPBar => enemyHPBar;
    public PokemonDisplay PlayerDisplay => playerDisplay;
    public PokemonDisplay EnemyDisplay => enemyDisplay;
    public BattleDialog Dialog => dialog;

    [Header("Test Pokemon")]
    [SerializeField] private bool createTestPokemonOnStart = true;

    private PokemonInstance _playerPokemon;
    private PokemonInstance _enemyPokemon;

    void Start()
    {
        if (createTestPokemonOnStart)
        {
            SetupTestBattle();
        }
    }

    /// <summary>
    /// Sets up a test battle with sample Pokemon.
    /// </summary>
    public void SetupTestBattle()
    {
        // Create test Pokemon
        _playerPokemon = PokemonFactory.Create(PokemonCatalog.Pikachu, 50);
        _enemyPokemon = PokemonFactory.Create(PokemonCatalog.Charmander, 50);

        // Display Pokemon
        if (playerDisplay != null)
        {
            playerDisplay.Display(_playerPokemon);
        }

        if (enemyDisplay != null)
        {
            enemyDisplay.Display(_enemyPokemon);
        }

        // Update HP bars
        if (playerHPBar != null)
        {
            playerHPBar.UpdateHP(_playerPokemon.CurrentHP, _playerPokemon.MaxHP);
        }

        if (enemyHPBar != null)
        {
            enemyHPBar.UpdateHP(_enemyPokemon.CurrentHP, _enemyPokemon.MaxHP);
        }

        // Show welcome message
        if (dialog != null)
        {
            _ = dialog.ShowMessage($"A wild {_enemyPokemon.DisplayName} appeared!", waitForInput: false);
        }
    }

    /// <summary>
    /// Updates HP bars with current Pokemon HP values.
    /// </summary>
    public void UpdateHPBars()
    {
        if (_playerPokemon != null && playerHPBar != null)
        {
            playerHPBar.UpdateHP(_playerPokemon.CurrentHP, _playerPokemon.MaxHP);
        }

        if (_enemyPokemon != null && enemyHPBar != null)
        {
            enemyHPBar.UpdateHP(_enemyPokemon.CurrentHP, _enemyPokemon.MaxHP);
        }
    }

    /// <summary>
    /// Simulates damage to player Pokemon (for testing).
    /// </summary>
    [ContextMenu("Test Player Damage")]
    public void TestPlayerDamage()
    {
        if (_playerPokemon != null)
        {
            int damage = 20;
            _playerPokemon.TakeDamage(damage);
            UpdateHPBars();

            if (dialog != null)
            {
                _ = dialog.ShowMessage($"{_playerPokemon.DisplayName} took {damage} damage!", waitForInput: false);
            }
        }
    }

    /// <summary>
    /// Simulates damage to enemy Pokemon (for testing).
    /// </summary>
    [ContextMenu("Test Enemy Damage")]
    public void TestEnemyDamage()
    {
        if (_enemyPokemon != null)
        {
            int damage = 25;
            _enemyPokemon.TakeDamage(damage);
            UpdateHPBars();

            if (dialog != null)
            {
                _ = dialog.ShowMessage($"{_enemyPokemon.DisplayName} took {damage} damage!", waitForInput: false);
            }
        }
    }
}

