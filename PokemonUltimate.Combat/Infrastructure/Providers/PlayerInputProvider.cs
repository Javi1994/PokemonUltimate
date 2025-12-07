using System;
using System.Linq;
using System.Threading.Tasks;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Extensions;
using PokemonUltimate.Combat.Foundation.Field;
using PokemonUltimate.Combat.Infrastructure.Helpers;
using PokemonUltimate.Combat.Integration.View;
using PokemonUltimate.Core.Data.Constants;

namespace PokemonUltimate.Combat.Infrastructure.Providers
{
    /// <summary>
    /// Provides actions from player input via IBattleView.
    /// Connects the battle logic to the UI/view layer for manual player control.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.7: Integration
    /// **Documentation**: See `docs/features/2-combat-system/2.7-integration/architecture.md`
    /// </remarks>
    public class PlayerInputProvider : IActionProvider
    {
        private readonly IBattleView _view;

        /// <summary>
        /// Creates a new player input provider.
        /// </summary>
        /// <param name="view">The battle view for collecting player input. Cannot be null.</param>
        /// <exception cref="ArgumentNullException">If view is null.</exception>
        public PlayerInputProvider(IBattleView view)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view), ErrorMessages.ViewCannotBeNull);
        }

        /// <summary>
        /// Gets the action the player wants to perform this turn.
        /// Prompts the player for action type, move/target selection, or switch selection.
        /// </summary>
        /// <param name="field">The current battlefield state.</param>
        /// <param name="mySlot">The slot requesting an action.</param>
        /// <returns>The action to perform this turn, or null if no action is available or player cancelled.</returns>
        /// <exception cref="ArgumentNullException">If field or mySlot is null.</exception>
        public async Task<BattleAction> GetAction(BattleField field, BattleSlot mySlot)
        {
            if (field == null)
                throw new ArgumentNullException(nameof(field), ErrorMessages.FieldCannotBeNull);
            if (mySlot == null)
                throw new ArgumentNullException(nameof(mySlot), ErrorMessages.PokemonCannotBeNull);

            // Can't act if slot is empty or Pokemon is fainted
            if (!mySlot.IsActive())
                return null;

            // 1. Show action menu (Fight/Switch/Item/Run)
            var actionType = await _view.SelectActionType(mySlot);

            // 2. Route based on selection
            switch (actionType)
            {
                case BattleActionType.Fight:
                    return await HandleFightAction(field, mySlot);

                case BattleActionType.Switch:
                    return await HandleSwitchAction(field, mySlot);

                case BattleActionType.Item:
                    // Future: Item usage
                    throw new NotImplementedException("Item usage not yet implemented");

                case BattleActionType.Run:
                    // Future: Flee from battle
                    throw new NotImplementedException("Run not yet implemented");

                default:
                    throw new ArgumentException($"Unknown action type: {actionType}", nameof(actionType));
            }
        }

        /// <summary>
        /// Handles the Fight action flow: move selection and target selection.
        /// </summary>
        private async Task<BattleAction> HandleFightAction(BattleField field, BattleSlot mySlot)
        {
            // 1. Get available moves (with PP > 0)
            var availableMoves = mySlot.Pokemon.Moves.Where(m => m.HasPP).ToList();
            if (availableMoves.Count == 0)
                return null; // No moves available

            // 2. Select move
            var moveInstance = await _view.SelectMove(availableMoves);
            if (moveInstance == null)
                return null; // Player cancelled

            // 3. Get valid targets
            var targetResolver = new TargetResolver();
            var validTargets = targetResolver.GetValidTargets(mySlot, moveInstance.Move, field);
            if (validTargets.Count == 0)
                return null; // No valid targets

            // 4. Select target (auto-select if only one)
            BattleSlot target;
            if (validTargets.Count == 1)
            {
                target = validTargets[0]; // Auto-select
            }
            else
            {
                target = await _view.SelectTarget(validTargets);
                if (target == null)
                    return null; // Player cancelled
            }

            return new UseMoveAction(mySlot, target, moveInstance);
        }

        /// <summary>
        /// Handles the Switch action flow: Pokemon selection.
        /// </summary>
        private async Task<BattleAction> HandleSwitchAction(BattleField field, BattleSlot mySlot)
        {
            // 1. Get available Pokemon to switch to
            var side = mySlot.Side;
            var availablePokemon = side.GetAvailableSwitches().ToList();
            if (availablePokemon.Count == 0)
                return null; // No Pokemon available to switch

            // 2. Select Pokemon to switch in
            var newPokemon = await _view.SelectSwitch(availablePokemon);
            if (newPokemon == null)
                return null; // Player cancelled

            return new SwitchAction(mySlot, newPokemon);
        }
    }
}

