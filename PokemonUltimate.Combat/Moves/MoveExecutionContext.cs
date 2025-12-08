using System.Collections.Generic;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Field;
using PokemonUltimate.Core.Domain.Instances.Move;

namespace PokemonUltimate.Combat.Moves
{
    /// <summary>
    /// Contains all data needed for move execution.
    /// Inputs are immutable, execution state is mutable as it passes through the pipeline.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.5: Combat Actions
    /// **Documentation**: See `docs/features/2-combat-system/2.5-combat-actions/architecture.md`
    /// </remarks>
    public class MoveExecutionContext
    {
        #region Inputs (Immutable)

        /// <summary>
        /// The slot using the move.
        /// </summary>
        public BattleSlot User { get; }

        /// <summary>
        /// The target slot.
        /// </summary>
        public BattleSlot Target { get; }

        /// <summary>
        /// The move instance being used.
        /// </summary>
        public MoveInstance MoveInstance { get; }

        /// <summary>
        /// The move data blueprint.
        /// </summary>
        public Core.Data.Blueprints.MoveData Move => MoveInstance.Move;

        /// <summary>
        /// The battlefield state.
        /// </summary>
        public BattleField Field { get; }

        /// <summary>
        /// Whether this move can be blocked by effects like Protect.
        /// </summary>
        public bool CanBeBlocked { get; }

        #endregion

        #region Execution State (Mutable)

        /// <summary>
        /// The list of actions generated during execution.
        /// Steps add actions to this list.
        /// </summary>
        public List<BattleAction> Actions { get; }

        /// <summary>
        /// Whether the move execution should stop (e.g., blocked by ability, protection, etc.).
        /// </summary>
        public bool ShouldStop { get; set; }

        /// <summary>
        /// Whether the move has multi-turn behavior.
        /// </summary>
        public bool HasMultiTurnEffect { get; set; }

        /// <summary>
        /// Whether the move has Focus Punch behavior.
        /// </summary>
        public bool HasFocusPunchEffect { get; set; }

        /// <summary>
        /// Whether PP was consumed (set after PP deduction step).
        /// </summary>
        public bool PPConsumed { get; set; }

        /// <summary>
        /// The total damage dealt by the move (set after damage processing).
        /// </summary>
        public int DamageDealt { get; set; }

        #endregion

        /// <summary>
        /// Creates a new move execution context.
        /// </summary>
        /// <param name="user">The slot using the move. Cannot be null.</param>
        /// <param name="target">The target slot. Cannot be null.</param>
        /// <param name="moveInstance">The move instance to use. Cannot be null.</param>
        /// <param name="field">The battlefield. Cannot be null.</param>
        /// <param name="canBeBlocked">Whether this move can be blocked.</param>
        public MoveExecutionContext(
            BattleSlot user,
            BattleSlot target,
            MoveInstance moveInstance,
            BattleField field,
            bool canBeBlocked = true)
        {
            User = user ?? throw new System.ArgumentNullException(nameof(user));
            Target = target ?? throw new System.ArgumentNullException(nameof(target));
            MoveInstance = moveInstance ?? throw new System.ArgumentNullException(nameof(moveInstance));
            Field = field ?? throw new System.ArgumentNullException(nameof(field));
            CanBeBlocked = canBeBlocked;
            Actions = new List<BattleAction>();
            ShouldStop = false;
        }
    }
}
