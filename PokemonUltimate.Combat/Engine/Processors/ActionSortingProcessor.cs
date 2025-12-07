using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Helpers;

namespace PokemonUltimate.Combat.Processors.Phases
{
    /// <summary>
    /// Sorts actions by turn order (priority, then speed).
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `docs/features/2-combat-system/2.6-combat-engine/architecture.md`
    /// </remarks>
    public class ActionSortingProcessor : IActionGeneratingPhaseProcessor
    {
        private readonly TurnOrderResolver _turnOrderResolver;

        /// <summary>
        /// Creates a new ActionSortingProcessor.
        /// </summary>
        /// <param name="turnOrderResolver">Turn order resolver for sorting actions. Cannot be null.</param>
        /// <exception cref="ArgumentNullException">If turnOrderResolver is null.</exception>
        public ActionSortingProcessor(TurnOrderResolver turnOrderResolver)
        {
            _turnOrderResolver = turnOrderResolver ?? throw new ArgumentNullException(nameof(turnOrderResolver));
        }

        /// <summary>
        /// Gets the phase this processor handles.
        /// </summary>
        public BattlePhase Phase => BattlePhase.ActionSorting;

        /// <summary>
        /// Sorts actions by turn order (priority, then speed).
        /// </summary>
        /// <param name="actions">Actions to sort.</param>
        /// <param name="field">The battlefield for context.</param>
        /// <returns>Sorted actions.</returns>
        public List<BattleAction> SortActions(IEnumerable<BattleAction> actions, BattleField field)
        {
            if (actions == null)
                throw new ArgumentNullException(nameof(actions));
            if (field == null)
                throw new ArgumentNullException(nameof(field), Core.Constants.ErrorMessages.FieldCannotBeNull);

            return _turnOrderResolver.SortActions(actions, field);
        }

        /// <summary>
        /// Processes the sorting phase (required by interface).
        /// </summary>
        /// <param name="field">The battlefield. Cannot be null.</param>
        /// <returns>Empty list (sorting is done via SortActions method).</returns>
        public async Task<List<BattleAction>> ProcessAsync(BattleField field)
        {
            // This processor doesn't process the field directly
            // It's used via SortActions method
            return await Task.FromResult(new List<BattleAction>());
        }
    }
}
