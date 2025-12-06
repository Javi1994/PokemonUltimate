using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using PokemonUltimate.Combat;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Core.Instances;

/// <summary>
/// Unity implementation of IBattleView interface.
/// Connects the battle engine to Unity UI components.
/// 
/// **Feature**: 4: Unity Integration
/// **Sub-Feature**: 4.3: IBattleView Implementation
/// **Documentation**: See `docs/features/4-unity-integration/4.3-ibattleview-implementation/README.md`
/// </summary>
public class UnityBattleView : MonoBehaviour, IBattleView
{
    [Header("UI Components")]
    [SerializeField] private BattleDialog dialog;
    [SerializeField] private HPBar playerHPBar;
    [SerializeField] private HPBar enemyHPBar;
    [SerializeField] private PokemonDisplay playerDisplay;
    [SerializeField] private PokemonDisplay enemyDisplay;

    // Public properties for BattleManager access
    public BattleDialog Dialog => dialog;
    public HPBar PlayerHPBar => playerHPBar;
    public HPBar EnemyHPBar => enemyHPBar;
    public PokemonDisplay PlayerDisplay => playerDisplay;
    public PokemonDisplay EnemyDisplay => enemyDisplay;

    // Mapping from BattleSlot to UI components
    private Dictionary<BattleSlot, HPBar> _slotToHPBar = new Dictionary<BattleSlot, HPBar>();
    private Dictionary<BattleSlot, PokemonDisplay> _slotToDisplay = new Dictionary<BattleSlot, PokemonDisplay>();

    /// <summary>
    /// Binds a BattleSlot to its corresponding UI components.
    /// Should be called when a Pokemon is sent into battle.
    /// </summary>
    public void BindSlot(BattleSlot slot, HPBar hpBar, PokemonDisplay display)
    {
        if (slot == null) return;
        
        _slotToHPBar[slot] = hpBar;
        if (display != null)
        {
            _slotToDisplay[slot] = display;
        }
    }

    #region IBattleView Implementation - Visual Updates

    public async Task ShowMessage(string message)
    {
        Debug.Log($"[PROCESS] UnityBattleView.ShowMessage() called from battle engine - Processing message: \"{message}\"");
        if (dialog != null)
        {
            Debug.Log($"[PROCESS] UnityBattleView - Found BattleDialog component, calling ShowMessage() to update UI...");
            await dialog.ShowMessage(message, waitForInput: false);
            Debug.Log($"[PROCESS] UnityBattleView - BattleDialog.ShowMessage() call completed");
        }
        else
        {
            Debug.LogWarning("[PROCESS] UnityBattleView - dialog is null, message will not be displayed!");
        }
    }

    public async Task PlayDamageAnimation(BattleSlot slot)
    {
        // TODO: Implement damage animation in Phase 4.5
        await Task.CompletedTask;
    }

    public Task UpdateHPBar(BattleSlot slot)
    {
        if (slot == null || slot.Pokemon == null)
        {
            Debug.LogWarning("[PROCESS] UnityBattleView.UpdateHPBar() - Slot or Pokemon is null, skipping UI update");
            return Task.CompletedTask;
        }

        string pokemonName = slot.Pokemon.DisplayName;
        int currentHP = slot.Pokemon.CurrentHP;
        int maxHP = slot.Pokemon.MaxHP;
        string side = slot.Side?.IsPlayer == true ? "Player" : "Enemy";
        
        Debug.Log($"[PROCESS] UnityBattleView.UpdateHPBar() called from battle engine - {side} {pokemonName} - HP: {currentHP}/{maxHP} ({(currentHP * 100f / maxHP):F1}%)");

        HPBar hpBar = GetHPBarForSlot(slot);
        if (hpBar != null)
        {
            Debug.Log($"[PROCESS] UnityBattleView - Found HPBar component, calling UpdateHP() to update UI...");
            hpBar.UpdateHP(currentHP, maxHP);
            Debug.Log($"[PROCESS] UnityBattleView - HPBar.UpdateHP() call completed");
        }
        else
        {
            Debug.LogWarning($"[PROCESS] UnityBattleView - No HPBar found for {side} {pokemonName}, UI will not update!");
        }
        
        return Task.CompletedTask;
    }

    public async Task PlayMoveAnimation(BattleSlot user, BattleSlot target, string moveId)
    {
        string userName = user?.Pokemon?.DisplayName ?? "Unknown";
        string targetName = target?.Pokemon?.DisplayName ?? "Unknown";
        Debug.Log($"[UnityBattleView] PlayMoveAnimation: {userName} uses {moveId} on {targetName}");
        // TODO: Implement move animation in Phase 4.5
        await Task.CompletedTask;
    }

    public async Task PlayFaintAnimation(BattleSlot slot)
    {
        string pokemonName = slot?.Pokemon?.DisplayName ?? "Unknown";
        Debug.Log($"[UnityBattleView] PlayFaintAnimation: {pokemonName} fainted!");
        // TODO: Implement faint animation in Phase 4.5
        await Task.CompletedTask;
    }

    public async Task PlayStatusAnimation(BattleSlot slot, string statusName)
    {
        string pokemonName = slot?.Pokemon?.DisplayName ?? "Unknown";
        Debug.Log($"[UnityBattleView] PlayStatusAnimation: {pokemonName} - Status: {statusName}");
        // TODO: Implement status animation in Phase 4.5
        await Task.CompletedTask;
    }

    public async Task ShowStatChange(BattleSlot slot, string statName, int stages)
    {
        string pokemonName = slot?.Pokemon?.DisplayName ?? "Unknown";
        string changeText = stages > 0 ? $"raised by {stages}" : stages < 0 ? $"lowered by {Mathf.Abs(stages)}" : "unchanged";
        Debug.Log($"[UnityBattleView] ShowStatChange: {pokemonName}'s {statName} {changeText}");
        // TODO: Implement stat change display in Phase 4.5
        await Task.CompletedTask;
    }

    public async Task PlaySwitchOutAnimation(BattleSlot slot)
    {
        string pokemonName = slot?.Pokemon?.DisplayName ?? "Unknown";
        Debug.Log($"[UnityBattleView] PlaySwitchOutAnimation: {pokemonName} switching out");
        // TODO: Implement switch-out animation in Phase 4.5
        await Task.CompletedTask;
    }

    public async Task PlaySwitchInAnimation(BattleSlot slot)
    {
        if (slot == null || slot.Pokemon == null)
            return;

        string pokemonName = slot.Pokemon.DisplayName;
        string side = slot.Side?.IsPlayer == true ? "Player" : "Enemy";
        Debug.Log($"[UnityBattleView] PlaySwitchInAnimation: {side} sends out {pokemonName} (Lv.{slot.Pokemon.Level})");

        // Update Pokemon display
        Debug.Log($"[PROCESS] UnityBattleView - Updating Pokemon display for {pokemonName}...");
        PokemonDisplay display = GetDisplayForSlot(slot);
        if (display != null)
        {
            Debug.Log($"[PROCESS] UnityBattleView - Found PokemonDisplay component, calling Display() to update UI...");
            display.Display(slot.Pokemon);
            Debug.Log($"[PROCESS] UnityBattleView - PokemonDisplay.Display() call completed");
        }
        else
        {
            Debug.LogWarning($"[PROCESS] UnityBattleView - No PokemonDisplay found for {side} {pokemonName}, UI will not update!");
        }

        // Update HP bar
        Debug.Log($"[PROCESS] UnityBattleView - Updating HP bar for {pokemonName}...");
        await UpdateHPBar(slot);

        // TODO: Implement switch-in animation in Phase 4.5
    }

    #endregion

    #region IBattleView Implementation - Player Input

    public async Task<BattleActionType> SelectActionType(BattleSlot slot)
    {
        string pokemonName = slot?.Pokemon?.DisplayName ?? "Unknown";
        Debug.Log($"[UnityBattleView] SelectActionType: {pokemonName} selecting action (default: Fight)");
        // TODO: Implement action menu in Phase 4.4
        // For now, return Fight as default
        return await Task.FromResult(BattleActionType.Fight);
    }

    public async Task<MoveInstance> SelectMove(IReadOnlyList<MoveInstance> moves)
    {
        if (moves == null || moves.Count == 0)
        {
            Debug.LogWarning("[UnityBattleView] SelectMove: No moves available!");
            return null;
        }
        
        var selectedMove = moves[0];
        Debug.Log($"[UnityBattleView] SelectMove: Selected {selectedMove.Move.Name} (default: first move)");
        // TODO: Implement move selection menu in Phase 4.4
        // For now, return first move
        return await Task.FromResult(selectedMove);
    }

    public async Task<BattleSlot> SelectTarget(IReadOnlyList<BattleSlot> validTargets)
    {
        if (validTargets == null || validTargets.Count == 0)
        {
            Debug.LogWarning("[UnityBattleView] SelectTarget: No valid targets!");
            return null;
        }
        
        var selectedTarget = validTargets[0];
        string targetName = selectedTarget?.Pokemon?.DisplayName ?? "Unknown";
        Debug.Log($"[UnityBattleView] SelectTarget: Selected {targetName} (default: first target)");
        // TODO: Implement target selection in Phase 4.4
        // For now, return first target
        return await Task.FromResult(selectedTarget);
    }

    public async Task<PokemonInstance> SelectSwitch(IReadOnlyList<PokemonInstance> availablePokemon)
    {
        if (availablePokemon == null || availablePokemon.Count == 0)
        {
            Debug.LogWarning("[UnityBattleView] SelectSwitch: No Pokemon available to switch!");
            return null;
        }
        
        var selectedPokemon = availablePokemon[0];
        Debug.Log($"[UnityBattleView] SelectSwitch: Selected {selectedPokemon.DisplayName} (default: first available)");
        // TODO: Implement switch selection menu in Phase 4.4
        // For now, return first available Pokemon
        return await Task.FromResult(selectedPokemon);
    }

    #endregion

    #region Helper Methods

    private HPBar GetHPBarForSlot(BattleSlot slot)
    {
        if (slot == null)
            return null;

        // Check if slot is bound
        if (_slotToHPBar.TryGetValue(slot, out HPBar boundHPBar))
        {
            return boundHPBar;
        }

        // Fallback to default HP bars based on side
        return slot.Side?.IsPlayer == true ? playerHPBar : enemyHPBar;
    }

    private PokemonDisplay GetDisplayForSlot(BattleSlot slot)
    {
        if (slot == null)
            return null;

        // Check if slot is bound
        if (_slotToDisplay.TryGetValue(slot, out PokemonDisplay boundDisplay))
        {
            return boundDisplay;
        }

        // Fallback to default displays based on side
        return slot.Side?.IsPlayer == true ? playerDisplay : enemyDisplay;
    }

    #endregion
}

