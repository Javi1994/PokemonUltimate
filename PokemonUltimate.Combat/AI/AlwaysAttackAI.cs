using System;
using System.Linq;
using System.Threading.Tasks;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Field;
using PokemonUltimate.Combat.Infrastructure.Providers.Definition;
using PokemonUltimate.Combat.Utilities;
using PokemonUltimate.Combat.Utilities.Extensions;
using PokemonUltimate.Core.Data.Constants;

namespace PokemonUltimate.Combat.AI
{
    /// <summary>
    /// Simple AI that always uses the first available move.
    /// Used for testing and predictable enemy behavior.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.7: Integration
    /// **Documentation**: See `docs/features/2-combat-system/2.7-integration/architecture.md`
    /// </remarks>
    public class AlwaysAttackAI : IActionProvider
    {
        /// <summary>
        /// Gets an action using the first available move.
        /// </summary>
        /// <param name="field">The current battlefield state. Cannot be null.</param>
        /// <param name="mySlot">The slot requesting an action. Cannot be null.</param>
        /// <returns>A UseMoveAction with the first available move, or null if no moves available.</returns>
        /// <exception cref="ArgumentNullException">If field or mySlot is null.</exception>
        public Task<BattleAction> GetAction(BattleField field, BattleSlot mySlot)
        {
            if (field == null)
                throw new ArgumentNullException(nameof(field), ErrorMessages.FieldCannotBeNull);
            if (mySlot == null)
                throw new ArgumentNullException(nameof(mySlot), ErrorMessages.PokemonCannotBeNull);

            // Return null if slot is empty or Pokemon is fainted
            if (!mySlot.IsActive())
                return Task.FromResult<BattleAction>(null);

            // Get first move with PP > 0
            var selectedMove = mySlot.Pokemon.Moves.FirstOrDefault(m => m.HasPP);

            if (selectedMove == null)
                return Task.FromResult<BattleAction>(null);

            // Get valid targets for this move
            var targetResolver = new TargetResolver();
            var validTargets = targetResolver.GetValidTargets(mySlot, selectedMove.Move, field);

            if (validTargets.Count == 0)
            {
                // No valid targets
                return Task.FromResult<BattleAction>(null);
            }

            // Use first target (or self if only one target and it's self)
            var target = validTargets[0];

            // Return UseMoveAction
            return Task.FromResult<BattleAction>(new UseMoveAction(mySlot, target, selectedMove));
        }
    }
}

