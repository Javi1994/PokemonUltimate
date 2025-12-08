using System.Collections.Generic;
using System.Threading.Tasks;
using PokemonUltimate.Combat.Field;
using PokemonUltimate.Combat.View.Definition;

namespace PokemonUltimate.Combat.Actions
{
    /// <summary>
    /// Base class for all battle actions.
    /// Actions have two phases: Logic (instant, updates game state) and Visual (async, shows to player).
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.2: Action Queue System
    /// **Documentation**: See `docs/features/2-combat-system/2.2-action-queue-system/README.md`
    /// </remarks>
    public abstract class BattleAction
    {
        /// <summary>
        /// The slot that initiated this action. Can be null for system actions.
        /// </summary>
        public BattleSlot User { get; }

        /// <summary>
        /// Priority modifier for turn order (higher = goes first).
        /// Most actions have 0. Quick Attack has +1, Vital Throw has -1, etc.
        /// </summary>
        public virtual int Priority => 0;

        /// <summary>
        /// Whether this action can be blocked by effects like Protect.
        /// Most reaction actions return false, move actions return true.
        /// </summary>
        public virtual bool CanBeBlocked => false;

        /// <summary>
        /// Creates a battle action.
        /// </summary>
        /// <param name="user">The slot that initiated this action. Can be null for system actions.</param>
        protected BattleAction(BattleSlot user = null)
        {
            User = user;
        }

        /// <summary>
        /// Phase 1: Execute the game logic (instant).
        /// Updates the data model (HP, status, etc.) and returns any reaction actions.
        /// </summary>
        /// <param name="field">The battlefield.</param>
        /// <returns>Actions triggered by this action (e.g., Damage -> Faint).</returns>
        public abstract IEnumerable<BattleAction> ExecuteLogic(BattleField field);

        /// <summary>
        /// Phase 2: Execute the visual presentation (async).
        /// Shows animations, messages, and UI updates to the player.
        /// </summary>
        /// <param name="view">The battle view for presentation.</param>
        public abstract Task ExecuteVisual(IBattleView view);
    }
}

