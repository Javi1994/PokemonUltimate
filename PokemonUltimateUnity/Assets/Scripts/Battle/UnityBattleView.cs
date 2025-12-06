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
        if (dialog != null)
        {
            await dialog.ShowMessage(message, waitForInput: false);
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
            return Task.CompletedTask;

        HPBar hpBar = GetHPBarForSlot(slot);
        if (hpBar != null)
        {
            hpBar.UpdateHP(slot.Pokemon.CurrentHP, slot.Pokemon.MaxHP);
        }
        
        return Task.CompletedTask;
    }

    public async Task PlayMoveAnimation(BattleSlot user, BattleSlot target, string moveId)
    {
        // TODO: Implement move animation in Phase 4.5
        await Task.CompletedTask;
    }

    public async Task PlayFaintAnimation(BattleSlot slot)
    {
        // TODO: Implement faint animation in Phase 4.5
        await Task.CompletedTask;
    }

    public async Task PlayStatusAnimation(BattleSlot slot, string statusName)
    {
        // TODO: Implement status animation in Phase 4.5
        await Task.CompletedTask;
    }

    public async Task ShowStatChange(BattleSlot slot, string statName, int stages)
    {
        // TODO: Implement stat change display in Phase 4.5
        await Task.CompletedTask;
    }

    public async Task PlaySwitchOutAnimation(BattleSlot slot)
    {
        // TODO: Implement switch-out animation in Phase 4.5
        await Task.CompletedTask;
    }

    public async Task PlaySwitchInAnimation(BattleSlot slot)
    {
        if (slot == null || slot.Pokemon == null)
            return;

        // Update Pokemon display
        PokemonDisplay display = GetDisplayForSlot(slot);
        if (display != null)
        {
            display.Display(slot.Pokemon);
        }

        // Update HP bar
        await UpdateHPBar(slot);

        // TODO: Implement switch-in animation in Phase 4.5
    }

    #endregion

    #region IBattleView Implementation - Player Input

    public async Task<BattleActionType> SelectActionType(BattleSlot slot)
    {
        // TODO: Implement action menu in Phase 4.4
        // For now, return Fight as default
        return await Task.FromResult(BattleActionType.Fight);
    }

    public async Task<MoveInstance> SelectMove(IReadOnlyList<MoveInstance> moves)
    {
        // TODO: Implement move selection menu in Phase 4.4
        // For now, return first move
        if (moves == null || moves.Count == 0)
            return null;
        
        return await Task.FromResult(moves[0]);
    }

    public async Task<BattleSlot> SelectTarget(IReadOnlyList<BattleSlot> validTargets)
    {
        // TODO: Implement target selection in Phase 4.4
        // For now, return first target
        if (validTargets == null || validTargets.Count == 0)
            return null;
        
        return await Task.FromResult(validTargets[0]);
    }

    public async Task<PokemonInstance> SelectSwitch(IReadOnlyList<PokemonInstance> availablePokemon)
    {
        // TODO: Implement switch selection menu in Phase 4.4
        // For now, return first available Pokemon
        if (availablePokemon == null || availablePokemon.Count == 0)
            return null;
        
        return await Task.FromResult(availablePokemon[0]);
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

