using System.Collections.Generic;
using System.Linq;
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
using PokemonUltimate.Combat.Logging;
using PokemonUltimate.Combat.Messages;
using PokemonUltimate.Core.Localization;

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

    [Header("Localization")]
    [SerializeField] private string languageCode = "es"; // Spanish by default
    [SerializeField] private bool useLocalizationManager = true;

    private CombatEngine _engine;
    private bool _isBattleRunning = false;
    private ILocalizationProvider _localizationProvider;

    void Start()
    {
        // Initialize localization
        InitializeLocalization();

        if (startBattleOnStart)
        {
            StartTestBattle();
        }
    }

    /// <summary>
    /// Initializes localization system.
    /// Uses LocalizationManager singleton if available, otherwise creates new provider.
    /// </summary>
    private void InitializeLocalization()
    {
        if (useLocalizationManager && LocalizationManager.Instance != null)
        {
            _localizationProvider = LocalizationManager.Instance.Provider;
            Debug.Log($"[BattleManager] Using LocalizationManager. Language: {_localizationProvider.CurrentLanguage}");
        }
        else
        {
            // Create new provider with specified language
            _localizationProvider = new PokemonUltimate.Content.Providers.LocalizationProvider();
            _localizationProvider.CurrentLanguage = languageCode;
            Debug.Log($"[BattleManager] Created new LocalizationProvider. Language: {languageCode}");
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

        Debug.Log("=== 🎮 BATTLE STARTING ===");
        Debug.Log($"Player Party: {string.Join(", ", playerParty.Select(p => $"{p.DisplayName} Lv.{p.Level}"))}");
        Debug.Log($"Enemy Party: {string.Join(", ", enemyParty.Select(p => $"{p.DisplayName} Lv.{p.Level}"))}");

        try
        {
            // Create CombatEngine
            Debug.Log("Creating CombatEngine...");
            _engine = CreateCombatEngine();

            // Create action providers
            Debug.Log($"Creating action providers... (Enemy AI: {(useRandomAIForEnemy ? "RandomAI" : "PlayerInput")})");
            var playerProvider = new PlayerInputProvider(battleView);
            IActionProvider enemyProvider = useRandomAIForEnemy
                ? new RandomAI()
                : new PlayerInputProvider(battleView); // For testing

            // Initialize battle
            Debug.Log("Initializing battle...");
            _engine.Initialize(
                BattleRules.Singles,
                playerParty,
                enemyParty,
                playerProvider,
                enemyProvider,
                battleView);

            Debug.Log($"Battle initialized! Player: {_engine.Field.PlayerSide.Slots[0].Pokemon?.DisplayName}, Enemy: {_engine.Field.EnemySide.Slots[0].Pokemon?.DisplayName}");

            // Bind slots to UI components
            Debug.Log("Binding slots to UI components...");
            Debug.Log("[PROCESS] BattleManager - Binding slots to UI components...");
            BindSlotsToUI();
            Debug.Log("[PROCESS] BattleManager - Slots bound to UI components successfully");

            // Show initial message
            Debug.Log($"Showing initial message: A wild {enemyParty[0].DisplayName} appeared!");
            Debug.Log("[PROCESS] BattleManager - Calling battleView.ShowMessage() to display initial message...");
            await battleView.ShowMessage($"A wild {enemyParty[0].DisplayName} appeared!");
            Debug.Log("[PROCESS] BattleManager - Initial message displayed successfully");

            // Run battle
            Debug.Log("=== ⚔️ RUNNING BATTLE ===");
            Debug.Log("[PROCESS] BattleManager - Starting battle loop (CombatEngine.RunBattle())...");
            var result = await _engine.RunBattle();
            Debug.Log($"[PROCESS] BattleManager - Battle loop completed. Result: {result.Outcome}");

            // Handle battle end
            Debug.Log($"=== 🏁 BATTLE ENDED: {result.Outcome} ===");
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
        
        // Create Unity logger for better visibility in Unity Console
        var logger = new UnityBattleLogger("CombatEngine", logDebug: true, logInfo: true, 
            logWarnings: true, logErrors: true, logBattleEvents: true);

        return new CombatEngine(
            battleFieldFactory,
            battleQueueFactory,
            randomProvider,
            endOfTurnProcessor,
            battleTriggerProcessor,
            logger: logger);
    }

    /// <summary>
    /// Binds BattleSlots to their corresponding UI components.
    /// </summary>
    private void BindSlotsToUI()
    {
        if (_engine?.Field == null || battleView == null)
        {
            Debug.LogWarning("[PROCESS] BattleManager.BindSlotsToUI() - Engine or BattleView is null, cannot bind slots");
            return;
        }

        Debug.Log("[PROCESS] BattleManager.BindSlotsToUI() - Starting slot binding...");

        // Bind player slots
        Debug.Log($"[PROCESS] BattleManager - Binding {_engine.Field.PlayerSide.Slots.Count} player slot(s)...");
        for (int i = 0; i < _engine.Field.PlayerSide.Slots.Count; i++)
        {
            var slot = _engine.Field.PlayerSide.Slots[i];
            HPBar hpBar = i == 0 ? battleView.PlayerHPBar : null; // For singles, use first HP bar
            PokemonDisplay display = i == 0 ? battleView.PlayerDisplay : null;
            
            if (hpBar != null)
            {
                Debug.Log($"[PROCESS] BattleManager - Binding player slot {i} ({slot.Pokemon?.DisplayName}) to UI components...");
                battleView.BindSlot(slot, hpBar, display);
                Debug.Log($"[PROCESS] BattleManager - Player slot {i} bound successfully");
            }
            else
            {
                Debug.LogWarning($"[PROCESS] BattleManager - No HPBar found for player slot {i}");
            }
        }

        // Bind enemy slots
        Debug.Log($"[PROCESS] BattleManager - Binding {_engine.Field.EnemySide.Slots.Count} enemy slot(s)...");
        for (int i = 0; i < _engine.Field.EnemySide.Slots.Count; i++)
        {
            var slot = _engine.Field.EnemySide.Slots[i];
            HPBar hpBar = i == 0 ? battleView.EnemyHPBar : null; // For singles, use first HP bar
            PokemonDisplay display = i == 0 ? battleView.EnemyDisplay : null;
            
            if (hpBar != null)
            {
                Debug.Log($"[PROCESS] BattleManager - Binding enemy slot {i} ({slot.Pokemon?.DisplayName}) to UI components...");
                battleView.BindSlot(slot, hpBar, display);
                Debug.Log($"[PROCESS] BattleManager - Enemy slot {i} bound successfully");
            }
            else
            {
                Debug.LogWarning($"[PROCESS] BattleManager - No HPBar found for enemy slot {i}");
            }
        }

        // Update initial displays
        Debug.Log("[PROCESS] BattleManager - Updating initial UI displays...");
        UpdateInitialDisplays();
        Debug.Log("[PROCESS] BattleManager - Initial UI displays updated");
    }

    /// <summary>
    /// Updates UI displays with initial Pokemon data.
    /// </summary>
    private void UpdateInitialDisplays()
    {
        if (_engine?.Field == null || battleView == null)
        {
            Debug.LogWarning("[PROCESS] BattleManager.UpdateInitialDisplays() - Engine or BattleView is null, cannot update displays");
            return;
        }

        Debug.Log("[PROCESS] BattleManager.UpdateInitialDisplays() - Updating initial UI displays...");

        // Update player display
        var playerSlot = _engine.Field.PlayerSide.Slots[0];
        Debug.Log($"[PROCESS] BattleManager - Updating player display for {playerSlot.Pokemon?.DisplayName}...");
        if (playerSlot.Pokemon != null && battleView.PlayerDisplay != null)
        {
            Debug.Log("[PROCESS] BattleManager - Calling PlayerDisplay.Display() to update UI...");
            battleView.PlayerDisplay.Display(playerSlot.Pokemon);
            Debug.Log("[PROCESS] BattleManager - PlayerDisplay.Display() completed");
        }
        else
        {
            Debug.LogWarning("[PROCESS] BattleManager - PlayerDisplay is null, cannot update player display!");
        }
        
        if (battleView.PlayerHPBar != null)
        {
            Debug.Log("[PROCESS] BattleManager - Calling PlayerHPBar.UpdateHP() to update UI...");
            battleView.PlayerHPBar.UpdateHP(playerSlot.Pokemon.CurrentHP, playerSlot.Pokemon.MaxHP);
            Debug.Log("[PROCESS] BattleManager - PlayerHPBar.UpdateHP() completed");
        }
        else
        {
            Debug.LogWarning("[PROCESS] BattleManager - PlayerHPBar is null, cannot update player HP bar!");
        }

        // Update enemy display
        var enemySlot = _engine.Field.EnemySide.Slots[0];
        Debug.Log($"[PROCESS] BattleManager - Updating enemy display for {enemySlot.Pokemon?.DisplayName}...");
        if (enemySlot.Pokemon != null && battleView.EnemyDisplay != null)
        {
            Debug.Log("[PROCESS] BattleManager - Calling EnemyDisplay.Display() to update UI...");
            battleView.EnemyDisplay.Display(enemySlot.Pokemon);
            Debug.Log("[PROCESS] BattleManager - EnemyDisplay.Display() completed");
        }
        else
        {
            Debug.LogWarning("[PROCESS] BattleManager - EnemyDisplay is null, cannot update enemy display!");
        }
        
        if (battleView.EnemyHPBar != null)
        {
            Debug.Log("[PROCESS] BattleManager - Calling EnemyHPBar.UpdateHP() to update UI...");
            battleView.EnemyHPBar.UpdateHP(enemySlot.Pokemon.CurrentHP, enemySlot.Pokemon.MaxHP);
            Debug.Log("[PROCESS] BattleManager - EnemyHPBar.UpdateHP() completed");
        }
        else
        {
            Debug.LogWarning("[PROCESS] BattleManager - EnemyHPBar is null, cannot update enemy HP bar!");
        }
        
        Debug.Log("[PROCESS] BattleManager.UpdateInitialDisplays() - All initial displays updated successfully");
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

