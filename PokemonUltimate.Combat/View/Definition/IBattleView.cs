using System.Collections.Generic;
using System.Threading.Tasks;
using PokemonUltimate.Combat.Field;
using PokemonUltimate.Combat.Infrastructure.Constants;
using PokemonUltimate.Core.Domain.Instances.Move;
using PokemonUltimate.Core.Domain.Instances.Pokemon;

namespace PokemonUltimate.Combat.View.Definition
{
    /// <summary>
    /// Interface for battle visualization/presentation and player input.
    /// Implementations handle animations, UI updates, player feedback, and input collection.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.7: Integration
    /// **Documentation**: See `docs/features/2-combat-system/2.7-integration/architecture.md`
    /// </remarks>
    public interface IBattleView
    {
        /// <summary>
        /// Displays a message to the player.
        /// </summary>
        /// <param name="message">The message to display.</param>
        Task ShowMessage(string message);

        /// <summary>
        /// Plays a damage animation for a slot.
        /// </summary>
        /// <param name="slot">The slot that took damage.</param>
        Task PlayDamageAnimation(BattleSlot slot);

        /// <summary>
        /// Updates the HP bar for a slot (smooth drain effect).
        /// </summary>
        /// <param name="slot">The slot to update.</param>
        Task UpdateHPBar(BattleSlot slot);

        /// <summary>
        /// Plays a move animation.
        /// </summary>
        /// <param name="user">The slot using the move.</param>
        /// <param name="target">The target slot.</param>
        /// <param name="moveId">The move identifier for animation lookup.</param>
        Task PlayMoveAnimation(BattleSlot user, BattleSlot target, string moveId);

        /// <summary>
        /// Plays the faint animation for a Pokemon.
        /// </summary>
        /// <param name="slot">The slot with the fainted Pokemon.</param>
        Task PlayFaintAnimation(BattleSlot slot);

        /// <summary>
        /// Plays an animation for a status effect being applied.
        /// </summary>
        /// <param name="slot">The slot receiving the status.</param>
        /// <param name="statusName">The name of the status for animation lookup.</param>
        Task PlayStatusAnimation(BattleSlot slot, string statusName);

        /// <summary>
        /// Updates the display after a stat change.
        /// </summary>
        /// <param name="slot">The slot whose stat changed.</param>
        /// <param name="statName">The name of the stat.</param>
        /// <param name="stages">The number of stages changed (+/-).</param>
        Task ShowStatChange(BattleSlot slot, string statName, int stages);

        /// <summary>
        /// Plays a switch-out animation.
        /// </summary>
        /// <param name="slot">The slot being switched out.</param>
        Task PlaySwitchOutAnimation(BattleSlot slot);

        /// <summary>
        /// Plays a switch-in animation.
        /// </summary>
        /// <param name="slot">The slot being switched in.</param>
        Task PlaySwitchInAnimation(BattleSlot slot);

        // ========== Player Input Methods ==========

        /// <summary>
        /// Prompts the player to select an action type (Fight/Switch/Item/Run).
        /// </summary>
        /// <param name="slot">The slot requesting input.</param>
        /// <returns>The selected action type.</returns>
        Task<BattleActionType> SelectActionType(BattleSlot slot);

        /// <summary>
        /// Prompts the player to select a move from available moves.
        /// </summary>
        /// <param name="moves">List of available moves (with PP > 0).</param>
        /// <returns>The selected move instance, or null if cancelled.</returns>
        Task<MoveInstance> SelectMove(IReadOnlyList<MoveInstance> moves);

        /// <summary>
        /// Prompts the player to select a target slot.
        /// </summary>
        /// <param name="validTargets">List of valid target slots.</param>
        /// <returns>The selected target slot, or null if cancelled.</returns>
        Task<BattleSlot> SelectTarget(IReadOnlyList<BattleSlot> validTargets);

        /// <summary>
        /// Prompts the player to select a Pokemon to switch in.
        /// </summary>
        /// <param name="availablePokemon">List of available Pokemon (not fainted, not active).</param>
        /// <returns>The selected Pokemon instance, or null if cancelled.</returns>
        Task<PokemonInstance> SelectSwitch(IReadOnlyList<PokemonInstance> availablePokemon);
    }
}

