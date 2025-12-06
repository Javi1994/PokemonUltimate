using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using PokemonUltimate.Combat;
using PokemonUltimate.Combat.AI;
using PokemonUltimate.Combat.Engine;
using PokemonUltimate.Combat.Events;
using PokemonUltimate.Combat.Factories;
using PokemonUltimate.Combat.Providers;
using PokemonUltimate.Core.Instances;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Content.Catalogs.Pokemon;

/// <summary>
/// Manages battle initialization and execution in Unity.
/// Creates and configures CombatEngine, handles battle lifecycle.
/// 
/// **Feature**: 4: Unity Integration
/// **Sub-Feature**: 4.3: IBattleView Implementation
/// **Documentation**: See `docs/features/4-unity-integration/4.3-ibattleview-implementation/README.md`
/// </summary>
public class BattleManager : MonoBehaviour
{
    [Header("Battle View")]
    [SerializeField] private UnityBattleView battleView;

    [Header("Test Configuration")]
    [SerializeField] private bool startBattleOnStart = false;
    [SerializeField] private bool useRandomAIForEnemy = true;

    private CombatEngine _engine;
    private bool _isBattleRunning = false;

    void Start()
    {
        if (startBattleOnStart)
        {
            StartTestBattle();
        }
    }

    /// <summary>
    /// Starts a test battle with default Pokemon.
    /// </summary>
    [ContextMenu("Start Test Battle")]
    public async void StartTestBattle()
    {
        if (_isBattleRunning)
        {
            Debug.LogWarning("Battle is already running!");
            return;
        }

        // Create test parties
        var playerParty = new[]
        {
            PokemonFactory.Create(PokemonCatalog.Pikachu, 50)
        };
        var enemyParty = new[]
        {
            PokemonFactory.Create(PokemonCatalog.Charmander, 50)
        };

        await StartBattle(playerParty, enemyParty);
    }

    /// <summary>
    /// Starts a battle with the given parties.
    /// </summary>
    public async Task StartBattle(
        IReadOnlyList<PokemonInstance> playerParty,
        IReadOnlyList<PokemonInstance> enemyParty)
    {
        if (_isBattleRunning)
        {
            Debug.LogWarning("Battle is already running!");
            return;
        }

        if (battleView == null)
        {
            Debug.LogError("BattleView is not assigned!");
            return;
        }

        _isBattleRunning = true;

        try
        {
            // Create CombatEngine
            _engine = CreateCombatEngine();

            // Create action providers
            var playerProvider = new PlayerInputProvider(battleView);
            IActionProvider enemyProvider = useRandomAIForEnemy
                ? new RandomAI()
                : new PlayerInputProvider(battleView); // For testing

            // Initialize battle
            _engine.Initialize(
                BattleRules.Singles,
                playerParty,
                enemyParty,
                playerProvider,
                enemyProvider,
                battleView);

            // Bind slots to UI components
            BindSlotsToUI();

            // Show initial message
            await battleView.ShowMessage($"A wild {enemyParty[0].DisplayName} appeared!");

            // Run battle
            var result = await _engine.RunBattle();

            // Handle battle end
            HandleBattleEnd(result);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Battle error: {ex.Message}");
            _isBattleRunning = false;
        }
    }

    /// <summary>
    /// Creates a CombatEngine with all required dependencies.
    /// </summary>
    private CombatEngine CreateCombatEngine()
    {
        var randomProvider = new RandomProvider();
        var battleFieldFactory = new BattleFieldFactory();
        var battleQueueFactory = new BattleQueueFactory();
        var damageContextFactory = new DamageContextFactory();
        var endOfTurnProcessor = new EndOfTurnProcessor(damageContextFactory);
        var battleTriggerProcessor = new BattleTriggerProcessor();

        return new CombatEngine(
            battleFieldFactory,
            battleQueueFactory,
            randomProvider,
            endOfTurnProcessor,
            battleTriggerProcessor);
    }

    /// <summary>
    /// Binds BattleSlots to their corresponding UI components.
    /// </summary>
    private void BindSlotsToUI()
    {
        if (_engine?.Field == null || battleView == null)
            return;

        // Bind player slots
        for (int i = 0; i < _engine.Field.PlayerSide.Slots.Count; i++)
        {
            var slot = _engine.Field.PlayerSide.Slots[i];
            HPBar hpBar = i == 0 ? battleView.PlayerHPBar : null; // For singles, use first HP bar
            PokemonDisplay display = i == 0 ? battleView.PlayerDisplay : null;
            
            if (hpBar != null)
            {
                battleView.BindSlot(slot, hpBar, display);
            }
        }

        // Bind enemy slots
        for (int i = 0; i < _engine.Field.EnemySide.Slots.Count; i++)
        {
            var slot = _engine.Field.EnemySide.Slots[i];
            HPBar hpBar = i == 0 ? battleView.EnemyHPBar : null; // For singles, use first HP bar
            PokemonDisplay display = i == 0 ? battleView.EnemyDisplay : null;
            
            if (hpBar != null)
            {
                battleView.BindSlot(slot, hpBar, display);
            }
        }

        // Update initial displays
        UpdateInitialDisplays();
    }

    /// <summary>
    /// Updates UI displays with initial Pokemon data.
    /// </summary>
    private void UpdateInitialDisplays()
    {
        if (_engine?.Field == null || battleView == null)
            return;

        // Update player display
        var playerSlot = _engine.Field.PlayerSide.Slots[0];
        if (playerSlot.Pokemon != null && battleView.PlayerDisplay != null)
        {
            battleView.PlayerDisplay.Display(playerSlot.Pokemon);
        }
        if (battleView.PlayerHPBar != null)
        {
            battleView.PlayerHPBar.UpdateHP(playerSlot.Pokemon.CurrentHP, playerSlot.Pokemon.MaxHP);
        }

        // Update enemy display
        var enemySlot = _engine.Field.EnemySide.Slots[0];
        if (enemySlot.Pokemon != null && battleView.EnemyDisplay != null)
        {
            battleView.EnemyDisplay.Display(enemySlot.Pokemon);
        }
        if (battleView.EnemyHPBar != null)
        {
            battleView.EnemyHPBar.UpdateHP(enemySlot.Pokemon.CurrentHP, enemySlot.Pokemon.MaxHP);
        }
    }

    /// <summary>
    /// Handles battle end result.
    /// </summary>
    private async void HandleBattleEnd(BattleResult result)
    {
        _isBattleRunning = false;

        string message = result.Outcome switch
        {
            BattleOutcome.Victory => "You won the battle!",
            BattleOutcome.Defeat => "You lost the battle...",
            BattleOutcome.Draw => "The battle ended in a draw.",
            BattleOutcome.Fled => "You fled from the battle.",
            BattleOutcome.Caught => "You caught the Pokemon!",
            _ => "Battle ended."
        };

        if (battleView != null)
        {
            await battleView.ShowMessage(message);
        }

        Debug.Log($"Battle ended: {result.Outcome}");
    }

    /// <summary>
    /// Gets the current CombatEngine instance.
    /// </summary>
    public CombatEngine Engine => _engine;

    /// <summary>
    /// Checks if a battle is currently running.
    /// </summary>
    public bool IsBattleRunning => _isBattleRunning;
}

