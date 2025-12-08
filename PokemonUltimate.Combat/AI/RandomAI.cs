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
    /// Simple AI that selects a random valid move.
    /// Used for testing and basic enemy behavior.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.7: Integration
    /// **Documentation**: See `docs/features/2-combat-system/2.7-integration/architecture.md`
    /// </remarks>
    public class RandomAI : IActionProvider
    {
        private readonly Random _random;

        /// <summary>
        /// Creates a new RandomAI instance.
        /// </summary>
        /// <param name="seed">Optional seed for random number generator. If null, uses time-based seed.</param>
        public RandomAI(int? seed = null)
        {
            _random = seed.HasValue ? new Random(seed.Value) : new Random();
        }

        /// <summary>
        /// Gets a random action for the Pokemon slot.
        /// </summary>
        /// <param name="field">The current battlefield state. Cannot be null.</param>
        /// <param name="mySlot">The slot requesting an action. Cannot be null.</param>
        /// <returns>A UseMoveAction with a random valid move, or null if no moves available.</returns>
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

            // Get moves with PP > 0
            var availableMoves = mySlot.Pokemon.Moves
                .Where(m => m.HasPP)
                .ToList();

            if (availableMoves.Count == 0)
                return Task.FromResult<BattleAction>(null);

            // Pick a random move
            var selectedMove = availableMoves[_random.Next(availableMoves.Count)];

            // Get valid targets for this move
            var targetResolver = new TargetResolver();
            var validTargets = targetResolver.GetValidTargets(mySlot, selectedMove.Move, field);

            if (validTargets.Count == 0)
            {
                // No valid targets (e.g., Field move or all targets fainted)
                // For Field moves, we still create the action (target can be null or self)
                // For now, return null if no targets
                return Task.FromResult<BattleAction>(null);
            }

            // Pick a random target (or use self if only one target and it's self)
            BattleSlot target;
            if (validTargets.Count == 1)
            {
                target = validTargets[0];
            }
            else
            {
                target = validTargets[_random.Next(validTargets.Count)];
            }

            // Return UseMoveAction
            return Task.FromResult<BattleAction>(new UseMoveAction(mySlot, target, selectedMove));
        }
    }
}

