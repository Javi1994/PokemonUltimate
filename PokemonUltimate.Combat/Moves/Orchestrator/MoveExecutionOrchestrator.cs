using System.Collections.Generic;
using System.Linq;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Field;
using PokemonUltimate.Combat.Moves.Definition;
using PokemonUltimate.Core.Domain.Instances.Move;

namespace PokemonUltimate.Combat.Moves.Orchestrator
{
    /// <summary>
    /// Orchestrates the execution of move execution steps in order.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.5: Combat Actions
    /// **Documentation**: See `docs/features/2-combat-system/2.5-combat-actions/architecture.md`
    /// </remarks>
    public class MoveExecutionOrchestrator
    {
        private readonly List<IMoveExecutionStep> _steps;

        /// <summary>
        /// Creates a new move execution orchestrator with the specified steps.
        /// </summary>
        /// <param name="steps">The execution steps to run in order. Cannot be null.</param>
        public MoveExecutionOrchestrator(IEnumerable<IMoveExecutionStep> steps)
        {
            if (steps == null)
                throw new System.ArgumentNullException(nameof(steps));

            // Sort steps by execution order
            _steps = steps.OrderBy(s => s.ExecutionOrder).ToList();
        }

        /// <summary>
        /// Executes all steps in order and returns the generated actions.
        /// </summary>
        /// <param name="user">The slot using the move. Cannot be null.</param>
        /// <param name="target">The target slot. Cannot be null.</param>
        /// <param name="moveInstance">The move instance to use. Cannot be null.</param>
        /// <param name="field">The battlefield. Cannot be null.</param>
        /// <param name="canBeBlocked">Whether this move can be blocked.</param>
        /// <returns>The list of actions generated during execution.</returns>
        public List<BattleAction> Execute(
            BattleSlot user,
            BattleSlot target,
            MoveInstance moveInstance,
            BattleField field,
            bool canBeBlocked = true)
        {
            var context = new MoveExecutionContext(user, target, moveInstance, field, canBeBlocked);

            // Execute each step in order until one stops execution
            foreach (var step in _steps)
            {
                var result = step.Process(context);

                if (result == MoveExecutionStepResult.Stop || context.ShouldStop)
                {
                    break;
                }
            }

            return context.Actions;
        }
    }
}
